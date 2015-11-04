using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;
using System.IO;
using HCMUT.EMRCorefResol.IO;
using HCMUT.EMRCorefResol;
using ICSharpCode.AvalonEdit.Document;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using IOPath = System.IO.Path;
using HCMUT.EMRCorefResol.English;
using ICSharpCode.AvalonEdit.Search;

namespace EMRCorefResol.UITest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Regex ConceptPattern = new Regex("c=\"[^|]+\" \\d+:\\d+ \\d+:\\d+");
        private readonly WinForms.FolderBrowserDialog FolderDialog = new WinForms.FolderBrowserDialog();

        private int currentEMRIndex = -1;
        private IDataReader dataReader = new I2B2DataReader();
        private EMR currentEMR = null;
        private Concept currentConcept = null;

        private SelectionInfo conceptSelectionInfo = new SelectionInfo(); // stores selection info on concepts text area
        private SelectionInfo chainSelectionInfo = new SelectionInfo(); // stores selection info on chains text area
        private SelectionInfo emrSelectionInfo = new SelectionInfo(); // stores selection info on emr text area
        private SelectionInfo featuresSelectionInfo = new SelectionInfo();
        private SelectionInfo systemChainsSelectionInfo = new SelectionInfo();

        private EMRCollection emrCollection;

        public MainWindow()
        {
            InitializeComponent();

            txtEMRPath.Text = @"..\..\..\..\..\dataset\i2b2_Test";
            emrCollection = new EMRCollection(txtEMRPath.Text);

            var emrHighlightBrush = new SolidColorBrush(Color.FromRgb(112, 183, 255));
            var conceptHighlightBrush = new SolidColorBrush(Color.FromRgb(226, 230, 214));

            initTextEditor(txtEMR, false, emrSelectionInfo, emrHighlightBrush, Brushes.White, false);
            initTextEditor(txtCons, true, conceptSelectionInfo, conceptHighlightBrush, null, true);
            initTextEditor(txtChains, true, chainSelectionInfo, conceptHighlightBrush, null, true);
            initTextEditor(txtSystemChains, true, systemChainsSelectionInfo, conceptHighlightBrush, null, true);
            initTextEditor(txtFeatures, true, featuresSelectionInfo, conceptHighlightBrush, null, true);

            txtScores.ShowLineNumbers = true;
            txtScores.TextArea.SelectionCornerRadius = 0;
            txtScores.Document = new TextDocument();
        }

        private void initTextEditor(TextEditor textEditor, bool wordWrap, SelectionInfo selectionInfo, 
            Brush selectionBgBrush, Brush selectionFgBrush, bool registerHighlight)
        {
            textEditor.ShowLineNumbers = true;
            textEditor.WordWrap = wordWrap;
            textEditor.TextArea.SelectionCornerRadius = 0;
            textEditor.TextArea.TextView.LineTransformers
                .Add(new SelectionHighlighter(selectionInfo, selectionBgBrush, selectionFgBrush));

            if (registerHighlight)
            {
                textEditor.TextArea.Caret.PositionChanged += (s, e) => txt_Caret_PositionChanged(textEditor, selectionInfo);
                textEditor.PreviewMouseDoubleClick += txt_PreviewMouseDoubleClick;
            }

            SearchPanel.Install(textEditor.TextArea);
            textEditor.Document = new TextDocument();
        }

        private void txt_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // highlight the raw concept text in emr when it's double clicked on concepts or chains text area

            var oldSelection = emrSelectionInfo.Clone();

            if (currentEMR != null && currentConcept != null)
            {
                var beginIndex = currentEMR.BeginIndexOf(currentConcept);
                var endIndex = currentEMR.EndIndexOf(currentConcept);

                // scroll to the raw text
                txtEMR.TextArea.Caret.Offset = beginIndex;
                txtEMR.ScrollTo(txtEMR.TextArea.Caret.Line, txtEMR.TextArea.Caret.Column);

                // store these values for the Highligher to work
                emrSelectionInfo.IsSelected = true;
                emrSelectionInfo.StartOffset = beginIndex;
                emrSelectionInfo.EndOffset = endIndex + 1;

                // trigger the redraw process to highlight the raw text
                txtEMR.TextArea.TextView.Redraw(beginIndex, endIndex - beginIndex + 1);
                e.Handled = true;
            }
            else
            {
                emrSelectionInfo.IsSelected = false;
            }

            if (oldSelection.IsSelected)
            {
                // if the old selection is highlighted, clear it
                // the Highlighter will know because the old offsets will not fall between the start and end offset
                // currently stored in the emrSelectionInfo field
                // or the IsSelected property is false.
                txtEMR.TextArea.TextView.Redraw(oldSelection.StartOffset,
                    oldSelection.EndOffset - oldSelection.StartOffset + 1);
            }
        }

        private void txt_Caret_PositionChanged(TextEditor textEditor, SelectionInfo selectionInfo)
        {
            highlightConcept(textEditor, selectionInfo);
        }

        private void highlightConcept(TextEditor txt, SelectionInfo selectionInfo)
        {
            var caretOffset = txt.CaretOffset; // first get the caret offset
            if (selectionInfo.IsSelected && caretOffset >= selectionInfo.StartOffset
                && caretOffset <= selectionInfo.EndOffset)
            {
                // if the caret lies on the current selection, do nothing
                return;
            }

            var location = txt.TextArea.Caret.Location; // mainly to get the caret column
            var line = txt.Document.GetLineByOffset(caretOffset); // get the line info the caret currently lies on
            var lineText = txt.Document.GetText(line.Offset, line.Length); // retrieve the line text

            var startAt = location.Column; // we will start at the caret column
            var selected = string.Empty; // stores the selected text (if any)

            var oldSelection = selectionInfo.Clone();

            if (!string.IsNullOrEmpty(lineText) && startAt >= 0 && startAt < lineText.Length)
            {
                // we will travel from the current caret column to the start of the current line (i.e. 0)
                // each time we go back, we check if there is a match with the concept pattern start at our column
                // if there is, check if the caret lies within the match value, if it is store the value and stop, 
                // otherwise go back one char and repeat the above process until we reach the start of the line.
                while (true)
                {
                    var match = ConceptPattern.Match(lineText, startAt);

                    if (match.Success)
                    {
                        var value = match.Value;
                        var beginIndex = lineText.IndexOf(value);
                        var endIndex = beginIndex + value.Length + 1;
                        if (beginIndex < location.Column && endIndex >= location.Column)
                        {
                            selected = value;
                            break;
                        }
                    }

                    if (startAt > 0)
                        startAt -= 1;
                    else break;
                }
            }

            if (!string.IsNullOrEmpty(selected))
            {
                // if there is a selected value, parse it to a Concept instance
                currentConcept = dataReader.ReadSingle(selected);

                // store the value position
                selectionInfo.StartOffset = line.Offset + startAt;
                selectionInfo.EndOffset = selectionInfo.StartOffset + selected.Length;
                selectionInfo.IsSelected = true;

                // highlight it
                txt.TextArea.TextView.Redraw(line.Offset + startAt, selected.Length);
            }
            else
            {
                selectionInfo.IsSelected = false;
                currentConcept = null;

                if (emrSelectionInfo.IsSelected)
                {
                    // if there is no selected value but there is highlighted text on emr text area (the old one)
                    // clear it
                    emrSelectionInfo.IsSelected = false;
                    txtEMR.TextArea.TextView.Redraw(emrSelectionInfo.StartOffset,
                        emrSelectionInfo.EndOffset - emrSelectionInfo.StartOffset + 1);
                }
            }

            if (oldSelection.IsSelected)
            {
                // clear the old highlight on txt area
                txt.TextArea.TextView.Redraw(oldSelection.StartOffset,
                    oldSelection.EndOffset - oldSelection.StartOffset);
            }
        }

        private void btnEMRPath_Click(object sender, RoutedEventArgs e)
        {
            if (FolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtEMRPath.Text = FolderDialog.SelectedPath;
                emrCollection = new EMRCollection(txtEMRPath.Text);
                currentEMRIndex = -1;
                currentEMR = null;
                currentConcept = null;

                chainSelectionInfo.IsSelected = false;
                conceptSelectionInfo.IsSelected = false;
                emrSelectionInfo.IsSelected = false;
            }
        }

        private void btnSystemChainsPath_Click(object sender, RoutedEventArgs e)
        {
            if (FolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSystemChainsPath.Text = FolderDialog.SelectedPath;                
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            OpenEMR(currentEMRIndex + 1);
        }

        private void OpenEMR(int index)
        {
            var nextEMRIndex = index;

            if (emrCollection == null)
                return;

            StreamReader sr = null;

            try
            {
                if (nextEMRIndex >= 0 && nextEMRIndex < emrCollection.Count)
                {
                    conceptSelectionInfo.IsSelected = false;
                    emrSelectionInfo.IsSelected = false;
                    chainSelectionInfo.IsSelected = false;

                    var emrPath = emrCollection.GetEMRPath(nextEMRIndex);
                    var conceptsPath = emrCollection.GetConceptsPath(nextEMRIndex);
                    var chainsPath = emrCollection.GetChainsPath(nextEMRIndex);                    

                    currentEMRIndex = nextEMRIndex;
                    currentEMR = new EMR(emrPath, conceptsPath, dataReader);
                    txtEMR.Document.Text = currentEMR.Content;
                    txtEMR.ScrollTo(1, 1);

                    sr = new StreamReader(conceptsPath);
                    txtCons.Document.Text = sr.ReadToEnd();
                    txtCons.ScrollTo(1, 1);
                    sr.Close();

                    sr = new StreamReader(chainsPath);
                    txtChains.Document.Text = sr.ReadToEnd();
                    txtChains.ScrollTo(1, 1);
                    sr.Close();

                    if (!string.IsNullOrEmpty(txtSystemChainsPath.Text))
                    {
                        var chainsFileName = IOPath.GetFileName(chainsPath);
                        var systemChainsPath = IOPath.Combine(txtSystemChainsPath.Text, chainsFileName);

                        if (File.Exists(systemChainsPath))
                        {
                            sr = new StreamReader(systemChainsPath);
                            txtSystemChains.Document.Text = sr.ReadToEnd();
                            txtSystemChains.ScrollTo(1, 1);
                            sr.Close();
                        }

                        var emrFileName = IOPath.GetFileName(emrPath);
                        var scoresPath = IOPath.Combine(txtSystemChainsPath.Text, $"{emrFileName}.scores");
                        if (File.Exists(scoresPath))
                        {
                            sr = new StreamReader(scoresPath);
                            txtScores.Document.Text = sr.ReadToEnd();
                            txtScores.ScrollTo(1, 1);
                            sr.Close();
                        }
                    }

                    tabEMR.Header = IOPath.GetFileName(emrPath);
                    btnPrev.IsEnabled = nextEMRIndex > 0;
                    btnNext.IsEnabled = nextEMRIndex < emrCollection.Count - 1;
                    btnExtract.IsEnabled = nextEMRIndex >= 0 && nextEMRIndex <= emrCollection.Count - 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\nStack trace:\n{ex.StackTrace}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }

            txtCurrentEMRIndex.Text = $"{currentEMRIndex + 1}";
            txtTotalEMR.Text = $"{emrCollection.Count}";
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            OpenEMR(currentEMRIndex - 1);
        }

        private void txtCurrentEMRIndex_LostFocus(object sender, RoutedEventArgs e)
        {
            int emrNumber;
            if (int.TryParse(txtCurrentEMRIndex.Text, out emrNumber))
            {
                if (emrNumber - 1 != currentEMRIndex)
                    OpenEMR(emrNumber - 1);
            }
            else
            {
                txtCurrentEMRIndex.Text = $"{currentEMRIndex + 1}";
            }
        }

        private void txtCurrentEMRIndex_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int emrNumber;
                if (int.TryParse(txtCurrentEMRIndex.Text, out emrNumber))
                {
                    if (emrNumber - 1 != currentEMRIndex)
                        OpenEMR(emrNumber - 1);
                }
                else
                {
                    txtCurrentEMRIndex.Text = $"{currentEMRIndex + 1}";
                }
            }
        }

        private void txtCurrentEMRIndex_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtCurrentEMRIndex.SelectAll();
        }

        private async void btnExtract_Click(object sender, RoutedEventArgs e)
        {            
            if (currentEMR != null)
            {
                var conceptsPath = emrCollection.GetConceptsPath(currentEMRIndex);
                var chainsPath = emrCollection.GetChainsPath(currentEMRIndex);

                var reader = new I2B2DataReader();
                var emr = currentEMR;
                var chains = new CorefChainCollection(chainsPath, reader);
                var instances = new AllInstancesGenerator().Generate(emr, chains);

                var extractor = new EnglishTrainingFeatureExtractor();
                extractor.EMR = emr;
                extractor.GroundTruth = chains;

                tab.SelectedIndex = 2;                

                var features = new IFeatureVector[instances.Count];
                var sb = new StringBuilder();
                var count = 0;

                await ExtractFeatures(instances, extractor, features, 
                    new Progress<int>(i =>
                    {
                        var fv = features[i];
                        sb.Append($"{instances[i]}\nClass-Value:{fv.ClassValue} {string.Join(" ", fv.Select(f => f.ToString()))}\n\n");
                        count += 1;
                        txtFeatures.Text = $"Extracting features...\n{count}/{instances.Count}";
                    }));

                txtFeatures.Text = sb.ToString();
            }
        }

        private static Task ExtractFeatures(IIndexedEnumerable<IClasInstance> instances, 
            IFeatureExtractor fExtractor, IFeatureVector[] features, IProgress<int> progress)
        {
            return Task.Run(() =>
            {
                Parallel.For(0, instances.Count, k =>
                {
                    var i = instances[k];
                    features[k] = i.GetFeatures(fExtractor);
                    progress.Report(k);
                });
            });
        }
    }
}

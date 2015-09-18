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
using System.Windows.Forms;
using System.IO;
using HCMUT.EMRCorefResol.IO;
using HCMUT.EMRCorefResol;
using ICSharpCode.AvalonEdit.Document;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;

namespace EMRCorefResol.UITest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Regex ConceptPattern = new Regex("c=\"[^|]+\" \\d+:\\d+ \\d+:\\d+");
        private readonly FolderBrowserDialog FolderDialog = new FolderBrowserDialog();
        private string[] emrFiles;

        private int currentEMRIndex = -1;
        private IDataReader dataReader = new I2B2DataReader();
        private EMR currentEMR;
        private Concept currentConcept = null;

        private SelectionInfo conceptSelectionInfo = new SelectionInfo(); // stores selection info on concepts text area
        private SelectionInfo chainSelectionInfo = new SelectionInfo(); // stores selection info on chains text area
        private SelectionInfo emrSelectionInfo = new SelectionInfo(); // stores selection info on emr text area

        private bool conChainMouseDown = false;

        public MainWindow()
        {
            InitializeComponent();

            txtEMRPath.Text = @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\docs";
            txtConPath.Text = @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\concepts";
            txtChainPath.Text = @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\chains";
            emrFiles = Directory.GetFiles(txtEMRPath.Text);

            txtEMR.ShowLineNumbers = true;
            txtEMR.TextArea.TextView.LineTransformers.Add(
                new Highlighter(emrSelectionInfo, new SolidColorBrush(Color.FromRgb(112, 183, 255)),
                    Brushes.White));

            txtCons.ShowLineNumbers = true;
            txtCons.TextArea.SelectionBrush = null;
            txtCons.TextArea.SelectionBorder = null;
            txtCons.TextArea.SelectionForeground = null;
            txtCons.TextArea.TextView.LineTransformers.Add(
                new Highlighter(conceptSelectionInfo, new SolidColorBrush(Color.FromRgb(226, 230, 214)), null));

            txtChains.ShowLineNumbers = true;
            txtChains.WordWrap = true;
            txtChains.TextArea.SelectionForeground = null;
            txtChains.TextArea.SelectionBorder = null;
            txtChains.TextArea.SelectionBrush = null;
            txtChains.TextArea.TextView.LineTransformers.Add(
                new Highlighter(chainSelectionInfo, new SolidColorBrush(Color.FromRgb(226, 230, 214)), null));

            txtCons.TextArea.Caret.PositionChanged += txtCon_Caret_PositionChanged;
            txtCons.PreviewMouseDoubleClick += txtConChain_PreviewMouseDoubleClick;
            txtCons.TextArea.PreviewMouseDown += txtConChain_TextArea_PreviewMouseDown;
            txtCons.TextArea.PreviewMouseUp += txtConChain_TextArea_PreviewMouseUp;

            txtChains.TextArea.Caret.PositionChanged += txtChains_Caret_PositionChanged;
            txtChains.PreviewMouseDoubleClick += txtConChain_PreviewMouseDoubleClick;
            txtChains.TextArea.PreviewMouseDown += txtConChain_TextArea_PreviewMouseDown;
            txtChains.TextArea.PreviewMouseUp += txtConChain_TextArea_PreviewMouseUp;

            txtEMR.Document = new TextDocument();
            txtCons.Document = new TextDocument();
            txtChains.Document = new TextDocument();
        }

        private void txtConChain_TextArea_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var txt = sender as TextEditor;
            if (txt != null && e.LeftButton == MouseButtonState.Released && conChainMouseDown)
            {
                txt.TextArea.ClearSelection();
                conChainMouseDown = false;
            }
        }

        private void txtConChain_TextArea_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var txt = sender as TextEditor;
            if (txt != null && e.LeftButton == MouseButtonState.Pressed)
                conChainMouseDown = true;
        }

        private void txtConChain_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
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

        private void txtCon_Caret_PositionChanged(object sender, EventArgs e)
        {
            highlightConcept(txtCons, conceptSelectionInfo);
        }

        private void txtChains_Caret_PositionChanged(object sender, EventArgs e)
        {
            highlightConcept(txtChains, chainSelectionInfo);
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
                // if there is, check if the caret lies within the match value, if it is store the value, 
                // otherwise go back one char and repeat the above process until we reach the start of the line.
                while (true)
                {
                    var match = ConceptPattern.Match(lineText, startAt);

                    if (match.Success)
                    {
                        var value = match.Value;
                        var beginIndex = lineText.IndexOf(value);
                        var endIndex = beginIndex + value.Length - 1;
                        if (beginIndex <= location.Column && endIndex >= location.Column)
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

        private void btnEMR_Click(object sender, RoutedEventArgs e)
        {
            if (FolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtEMRPath.Text = FolderDialog.SelectedPath;
                emrFiles = Directory.GetFiles(txtEMRPath.Text);
            }
        }

        private void btnCon_Click(object sender, RoutedEventArgs e)
        {
            if (FolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtConPath.Text = FolderDialog.SelectedPath;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            OpenEMR(currentEMRIndex + 1);
        }

        private void OpenEMR(int index)
        {
            var nextEMRIndex = index;

            if (emrFiles == null)
                return;

            if (nextEMRIndex >= 0 && nextEMRIndex < emrFiles.Length)
            {
                conceptSelectionInfo.IsSelected = false;
                emrSelectionInfo.IsSelected = false;
                chainSelectionInfo.IsSelected = false;

                var emrPath = emrFiles[nextEMRIndex];
                var emrFileName = tabEMR.Header = System.IO.Path.GetFileName(emrPath);
                var conPath = System.IO.Path.Combine(txtConPath.Text, emrFileName + ".con");
                var chainPath = System.IO.Path.Combine(txtChainPath.Text, emrFileName + ".chains");

                currentEMRIndex = nextEMRIndex;
                currentEMR = new EMR(emrPath, conPath, dataReader);
                txtEMR.Document.Text = currentEMR.Content;
                txtEMR.ScrollTo(1, 1);

                var sr = new StreamReader(conPath);
                txtCons.Document.Text = sr.ReadToEnd();
                txtCons.ScrollTo(1, 1);
                sr.Close();

                sr = new StreamReader(chainPath);
                txtChains.Document.Text = sr.ReadToEnd();
                txtChains.ScrollTo(1, 1);
                sr.Close();

                btnPrev.IsEnabled = nextEMRIndex > 0;
                btnNext.IsEnabled = nextEMRIndex < emrFiles.Length - 1;
            }

            txtCurrentEMRIndex.Text = $"{currentEMRIndex + 1}";
            txtTotalEMR.Text = $"{emrFiles.Length}";
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
    }
}

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

        private SelectionInfo conceptSelectionInfo = new SelectionInfo();
        private SelectionInfo chainSelectionInfo = new SelectionInfo();
        private SelectionInfo emrSelectionInfo = new SelectionInfo();

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
            txtCons.TextArea.TextView.LineTransformers.Add(
                new Highlighter(conceptSelectionInfo, new SolidColorBrush(Color.FromRgb(226, 230, 214)), null));

            txtChains.ShowLineNumbers = true;
            txtChains.WordWrap = true;
            txtChains.TextArea.TextView.LineTransformers.Add(
                new Highlighter(chainSelectionInfo, new SolidColorBrush(Color.FromRgb(226, 230, 214)), null));

            txtCons.TextArea.Caret.PositionChanged += txtCon_Caret_PositionChanged;
            txtCons.PreviewMouseDoubleClick += txtCon_PreviewMouseDoubleClick;

            txtChains.TextArea.Caret.PositionChanged += txtChains_Caret_PositionChanged;
            txtChains.PreviewMouseDoubleClick += txtCon_PreviewMouseDoubleClick;
        }

        private void txtCon_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var oldSelection = emrSelectionInfo.Clone();

            if (currentEMR != null && currentConcept != null)
            {
                var beginIndex = currentEMR.BeginIndexOf(currentConcept);
                var endIndex = currentEMR.EndIndexOf(currentConcept);

                txtEMR.TextArea.Caret.Offset = beginIndex;
                txtEMR.ScrollTo(txtEMR.TextArea.Caret.Line, txtEMR.TextArea.Caret.Column);

                emrSelectionInfo.IsSelected = true;
                emrSelectionInfo.StartOffset = beginIndex;
                emrSelectionInfo.EndOffset = endIndex + 1;

                txtEMR.TextArea.TextView.Redraw(beginIndex, endIndex - beginIndex + 1);
                e.Handled = true;
            }
            else
            {
                emrSelectionInfo.IsSelected = false;
            }

            if (oldSelection.IsSelected)
            {
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
            var offset = txt.CaretOffset;
            if (selectionInfo.IsSelected && offset >= selectionInfo.StartOffset
                && offset <= selectionInfo.EndOffset)
            {
                return;
            }

            var location = txt.TextArea.Caret.Location;
            var line = txt.Document.GetLineByOffset(offset);
            var lineText = txt.Document.GetText(line.Offset, line.Length);

            var startAt = location.Column;
            var selected = string.Empty;

            var oldSelection = selectionInfo.Clone();

            if (!string.IsNullOrEmpty(lineText) && startAt >= 0 && startAt < lineText.Length)
            {
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
                currentConcept = dataReader.ReadSingle(selected);

                selectionInfo.StartOffset = line.Offset + startAt;
                selectionInfo.EndOffset = selectionInfo.StartOffset + selected.Length;
                selectionInfo.IsSelected = true;

                txt.TextArea.TextView.Redraw(line.Offset + startAt, selected.Length);
            }
            else
            {
                selectionInfo.IsSelected = false;
                currentConcept = null;

                if (emrSelectionInfo.IsSelected)
                {
                    emrSelectionInfo.IsSelected = false;
                    txtEMR.TextArea.TextView.Redraw(emrSelectionInfo.StartOffset,
                        emrSelectionInfo.EndOffset - emrSelectionInfo.StartOffset + 1);
                }                
            }

            if (oldSelection.IsSelected)
            {
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
            OpenEMR(1);
        }

        private void OpenEMR(int offset)
        {
            string emrFileName, cons, chains;
            EMR emr;

            if (OpenEMR(offset, out emrFileName, out emr, out cons, out chains))
            {
                tabEMR.Header = emrFileName;
                currentEMR = emr;

                conceptSelectionInfo.IsSelected = false;
                emrSelectionInfo.IsSelected = false;
                chainSelectionInfo.IsSelected = false;

                txtEMR.Document = new TextDocument(currentEMR.Content);
                txtCons.Document = new TextDocument(cons);
                txtChains.Document = new TextDocument(chains);
            }
        }

        private bool OpenEMR(int offset, out string emrFileName, out EMR emr, out string cons, out string chains)
        {
            var nextEMRIndex = currentEMRIndex + offset;

            if (nextEMRIndex >= 0 && nextEMRIndex < emrFiles.Length)
            {
                var emrPath = emrFiles[nextEMRIndex];
                emrFileName = System.IO.Path.GetFileName(emrPath);
                var conPath = System.IO.Path.Combine(txtConPath.Text, emrFileName + ".con");
                var chainPath = System.IO.Path.Combine(txtChainPath.Text, emrFileName + ".chains");

                currentEMRIndex = nextEMRIndex;
                emr = new EMR(emrPath, conPath, dataReader);

                var sr = new StreamReader(conPath);
                cons = sr.ReadToEnd();
                sr.Close();

                sr = new StreamReader(chainPath);
                chains = sr.ReadToEnd();
                sr.Close();

                return true;
            }

            emr = null;
            emrFileName = string.Empty;
            cons = string.Empty;
            chains = string.Empty;
            return false;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            OpenEMR(-1);
        }
    }
}

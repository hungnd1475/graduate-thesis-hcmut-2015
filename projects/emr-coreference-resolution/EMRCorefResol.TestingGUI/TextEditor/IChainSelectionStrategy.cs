using HCMUT.EMRCorefResol;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EMRCorefResol.TestingGUI
{
    public interface IChainSelectionStrategy
    {
        event EventHandler<ChainSelectedEventArgs> ChainSelected;
    }

    public class ChainSelectedEventArgs : EventArgs
    {
        public TextSegmentCollection<TextSegment> SelectedSegments { get; }

        public ChainSelectedEventArgs(TextSegmentCollection<TextSegment> segments)
        {
            SelectedSegments = segments;
        }
    }

    public class ChainSelectionInConcepts : DependencyObject, IChainSelectionStrategy
    {
        public event EventHandler<ChainSelectedEventArgs> ChainSelected;

        public string ConceptsText
        {
            get { return (string)GetValue(ConceptsTextProperty); }
            set { SetValue(ConceptsTextProperty, value); }
        }
        
        public static readonly DependencyProperty ConceptsTextProperty =
            DependencyProperty.Register(nameof(ConceptsText), typeof(string), typeof(ChainSelectionInConcepts), 
                new PropertyMetadata(null, ConceptsTextChangedCallback));

        private static void ConceptsTextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public CorefChain SelectedChain
        {
            get { return (CorefChain)GetValue(SelectedChainProperty); }
            set { SetValue(SelectedChainProperty, value); }
        }

        public static readonly DependencyProperty SelectedChainProperty =
            DependencyProperty.Register(nameof(SelectedChain), typeof(CorefChain), typeof(ChainSelectionInConcepts), 
                new PropertyMetadata(null, SelectedChainChangedCallback));

        private static void SelectedChainChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void FindSelectedSegments()
        {

        }

        private void RaiseChainSelectedEvent(TextSegmentCollection<TextSegment> segments)
        {
            ChainSelected?.Invoke(this, new ChainSelectedEventArgs(segments));
        }
    }
}

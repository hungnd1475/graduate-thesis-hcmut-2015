using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Represents a class used for adapting <see cref="DockingManager"/> to Prism region
    /// </summary>
    class AvalonDockRegionAdapter : RegionAdapterBase<DockingManager>
    {
        private bool updatingActiveViewFromTarget = false;

        /// <summary>
        /// Stores documents that was added to the <see cref="IRegion.Views"/>
        /// </summary>
        public ObservableCollection<object> Documents { get; } = new ObservableCollection<object>();

        /// <summary>
        /// Stores anchorables that was added to the <see cref="IRegion.Views"/>
        /// </summary>
        public ObservableCollection<object> Anchorables { get; } = new ObservableCollection<object>();

        public AvalonDockRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, DockingManager regionTarget)
        {
            // TODO: add checking null and whether documents/achorables source is set

            // binds DockingManager.DocumentsSource to this adapter Documents collection
            // so it can be automatically updated when documents was added to the region's views
            BindingOperations.SetBinding(regionTarget,
                DockingManager.DocumentsSourceProperty, 
                new Binding(nameof(Documents)) { Source = this });

            BindingOperations.SetBinding(regionTarget,
                DockingManager.AnchorablesSourceProperty,
                new Binding(nameof(Anchorables)) { Source = this });

            // add all views to Documents/Anchorables accordingly
            foreach (var v in region.Views)
            {
                var t = v as IDockableView;
                if (t == null)
                {
                    throw new InvalidOperationException("Only view that implement IDockableView interface can be registered with this region.");
                }
                
                if (t.DockableType == DockableType.Document)
                {
                    Documents.Add(v);
                }
                else
                {
                    Anchorables.Add(v);
                }
            }

            // handle this region's views changed
            region.Views.CollectionChanged += (s, e) =>
            {
                OnViewsChanged(s, e, region, regionTarget);
            };

            // handle this region's active views changed
            region.ActiveViews.CollectionChanged += (s, e) =>
            {
                OnActiveViewsChanged(s, e, region, regionTarget);
            };

            // handle the DockingManager.ActiveContent changed
            regionTarget.ActiveContentChanged += (s, e) =>
            {
                RegionTarget_ActiveContentChanged(s, e, region, regionTarget);
            };

            regionTarget.DocumentClosed += RegionTarget_DocumentClosed;
        }

        private void RegionTarget_DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            Documents.Remove(e.Document.Content);
            Anchorables.Remove(e.Document.Content);
        }

        private void RegionTarget_ActiveContentChanged(object sender, EventArgs e,
            IRegion region, DockingManager regionTarget)
        {
            try
            {
                updatingActiveViewFromTarget = true;
                var activeContent = regionTarget.ActiveContent;

                foreach (var item in region.ActiveViews.Where(i => i != activeContent))
                {
                    region.Deactivate(item);
                }

                if (region.Views.Contains(activeContent) && !region.ActiveViews.Contains(activeContent))
                {
                    region.Activate(activeContent);
                }
            }
            finally
            {
                updatingActiveViewFromTarget = false;
            }
        }

        private void OnActiveViewsChanged(object s, NotifyCollectionChangedEventArgs e, 
            IRegion region, DockingManager regionTarget)
        {
            if (updatingActiveViewFromTarget)
            {
                return;
            }

            var targetActiveContent = regionTarget.ActiveContent;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var regionActiveContent = e.NewItems[0];

                if (targetActiveContent != null
                    && targetActiveContent != regionActiveContent
                    && region.ActiveViews.Contains(targetActiveContent))
                {
                    region.Deactivate(targetActiveContent);
                }

                regionTarget.ActiveContent = regionActiveContent;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove
                && e.OldItems.Contains(targetActiveContent))
            {
                regionTarget.ActiveContent = null;
            }
        }

        private void OnViewsChanged(object sender, NotifyCollectionChangedEventArgs e, 
            IRegion region, DockingManager dockingManager)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    var t = newItem as IDockableView;
                    if (t == null)
                    {
                        throw new InvalidOperationException("Only view that implement IDockableView interface can be registered with this region.");
                    }
                    
                    if (t.DockableType == DockableType.Document)
                    {
                        Documents.Add(newItem);
                    }
                    else
                    {
                        Anchorables.Add(newItem);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    Documents.Remove(item);
                    Anchorables.Remove(item);
                }
            }
        }

        protected override IRegion CreateRegion()
        {
            return new Region();
        }
    }
}

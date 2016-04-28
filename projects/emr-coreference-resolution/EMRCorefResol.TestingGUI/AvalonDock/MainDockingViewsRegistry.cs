using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRCorefResol.TestingGUI.Properties;
using System.Collections.Specialized;
using Xceed.Wpf.AvalonDock;

namespace EMRCorefResol.TestingGUI
{
    public class MainDockingViewsRegistry : IAvalonDockViewsRegistry
    {
        private bool _isRegistered = false;

        public string LastViewsSettingName { get; set; }
        public string RegionName { get; set; }

        public void RegisterViews(DockingManager dockingManager)
        {
            var regionManager = ServiceLocator.Current.TryResolve<IRegionManager>();
            if (!_isRegistered && regionManager != null)
            {
                var lastViews = Settings.Default[LastViewsSettingName] as StringCollection;
                if (lastViews != null && lastViews.Count > 0)
                {
                    foreach (var view in lastViews)
                    {
                        var viewType = Type.GetType(view, false);
                        if (viewType != null)
                        {
                            regionManager.RegisterViewWithRegion(RegionName, viewType);
                        }
                    }
                }
                else
                {
                    regionManager.RegisterViewWithRegion(RegionName, typeof(EMRContentView));
                    regionManager.RegisterViewWithRegion(RegionName, typeof(EMRConceptsView));
                    regionManager.RegisterViewWithRegion(RegionName, typeof(GroundTruthView));
                    regionManager.RegisterViewWithRegion(RegionName, typeof(OutputView));
                }
                _isRegistered = true;
            }
        }
    }
}

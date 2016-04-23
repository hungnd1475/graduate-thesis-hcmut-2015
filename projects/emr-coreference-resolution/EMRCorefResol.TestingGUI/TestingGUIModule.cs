using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using System.ComponentModel.Composition;

namespace EMRCorefResol.TestingGUI
{
    [ModuleExport(typeof(TestingGUIModule))]
    public class TestingGUIModule : IModule
    {
        private readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public TestingGUIModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.Workspace, typeof(EMRContentView));
            _regionManager.RegisterViewWithRegion(RegionNames.Workspace, typeof(EMRConceptsView));
            _regionManager.RegisterViewWithRegion(RegionNames.Workspace, typeof(GroundTruthView));
        }
    }
}

using Microsoft.Practices.ServiceLocation;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using System.ComponentModel.Composition;
using System.Diagnostics;

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
        }
    }
}

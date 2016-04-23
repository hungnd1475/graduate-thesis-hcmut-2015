using Prism.Events;
using Prism.Mef;
using Prism.Regions;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Xceed.Wpf.AvalonDock;

namespace EMRCorefResol.TestingGUI
{
    class TestingGUIBootstrapper : MefBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.ComposeExportedValue<IEventAggregator>(new EventAggregator());
        }

        protected override void ConfigureAggregateCatalog()
        {
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(TestingGUIModule).Assembly));            
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.GetExportedValue<MainWindow>();
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var m = base.ConfigureRegionAdapterMappings();
            if (m == null)
            {
                return null;
            }

            m.RegisterMapping(typeof(DockingManager), new AvalonDockRegionAdapter(ConfigureDefaultRegionBehaviors()));
            return m;
        }
    }
}

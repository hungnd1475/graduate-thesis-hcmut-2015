using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock;

namespace EMRCorefResol.TestingGUI
{
    public interface IAvalonDockViewsRegistry
    {
        void RegisterViews(DockingManager dockingManager);
    }
}

using System.Windows;
using System.Windows.Controls;

namespace Opium.MVVM.Framework.Fluent
{
   
    public interface IFluentRegionManager
    {
        
        void ExportViewToRegion(string viewName, string regionTag);
        
        void ExportViewToRegion<T>(string regionTag) where T : UserControl;

        void RegisterRegion(UIElement region, string regionTag);
    }
}

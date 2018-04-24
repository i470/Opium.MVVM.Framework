using System.Windows.Controls;

namespace Opium.MVVM.Framework.Regions
{
      
        public interface IRegionAdapterBase
        {
           
            void AddRegion(object region, string targetRegion);
        
            void AddView(UserControl view, string viewName, string targetRegion);
        
            void ActivateControl(string viewName, string targetRegion);
        
            void DeactivateControl(string viewName);
        
            bool HasView(string viewName);
        }
    }
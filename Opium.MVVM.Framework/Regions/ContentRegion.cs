using System.Windows.Controls;

namespace Opium.MVVM.Framework.Regions
{


    [RegionAdapterFor(typeof(ContentControl))]
    public class ContentRegion : RegionAdapterBase<ContentControl>
    {
        public override void ActivateControl(string viewName, string targetRegion)
        {
            ValidateControlName(viewName);
            ValidateRegionName(targetRegion);

            var region = Regions[targetRegion];
            region.Content = Controls[viewName];
        }
    }
}
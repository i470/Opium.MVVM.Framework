using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Opium.MVVM.Framework.Regions
{


    [RegionAdapterFor(typeof(ContentControl))]
    public class ContentRegion : RegionAdapterBase<ContentControl>
    {
        /// <summary>
        ///     Activates a control for a region
        /// </summary>
        /// <param name="viewName">The name of the control</param>
        /// <param name="targetRegion">The name of the region</param>
        public override void ActivateControl(string viewName, string targetRegion)
        {
            ValidateControlName(viewName);
            ValidateRegionName(targetRegion);

            var region = Regions[targetRegion];
            region.Content = Controls[viewName];
        }
    }
}
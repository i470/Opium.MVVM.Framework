using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opium.MVVM.Framework.Regions
{
    
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RegionAdapterForAttribute : ExportAttribute
    {
        
        public RegionAdapterForAttribute(Type targetType) : base(typeof(IRegionAdapterBase))
        {
            TargetType = targetType;
        }

        
        public Type TargetType { get; set; }
    }
}
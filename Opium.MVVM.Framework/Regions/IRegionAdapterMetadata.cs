using System;

namespace Opium.MVVM.Framework.Regions
{
    /// <summary>
    /// Meta data for exporting a region adapter
    /// </summary>
    public interface IRegionAdapterMetadata
    {
        /// <summary>
        /// The type of the control the region adapter manages
        /// </summary>
        Type TargetType { get; }
    }
}
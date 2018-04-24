using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Opium.MVVM.Framework.Properties;
using Opium.MVVM.Framework.Services;

namespace Opium.MVVM.Framework.Regions
{
  
    public abstract class RegionAdapterBase<TRegionType> : IRegionAdapterBase where TRegionType : Control
    {
        protected readonly Dictionary<string, TRegionType> Regions = new Dictionary<string, TRegionType>();
        
        protected readonly Dictionary<string, UserControl> Controls = new Dictionary<string, UserControl>();

    
        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }
        
        public virtual void AddRegion(object region, string targetRegion)
        {
            if (region == null)
            {
                Logger.LogFormat(LogSeverity.Error, GetType().FullName, "AddRegion: null region.");
                throw new ArgumentNullException(nameof(region));
            }

            if (!(region is TRegionType))
            {
                Logger.LogFormat(LogSeverity.Error, GetType().FullName, "Region {0} is not a {1}.", region.GetType().FullName, typeof(TRegionType).FullName);
                throw new ArgumentOutOfRangeException($@"region");
            }


            if (Regions.ContainsKey(targetRegion))
            {
                Logger.LogFormat(LogSeverity.Error, GetType().FullName, "Attempt to add duplicate region name: {0}",
                                 targetRegion);
                throw new Exception($"Duplicate region is not allowed: {targetRegion}");
            }

            Regions.Add(targetRegion, (TRegionType)region);
        }

        public virtual void AddView(UserControl view, string viewName, string targetRegion)
        {
            if (view == null)
            {
                throw new ArgumentNullException($"view");
            }

            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException($"viewName");
            }

            if (Controls.ContainsKey(viewName))
            {
                return;
            }

            ValidateRegionName(targetRegion);

            Controls.Add(viewName, view);
        }


        public abstract void ActivateControl(string viewName, string targetRegion);
        
        public virtual void DeactivateControl(string viewName)
        {
            ValidateControlName(viewName);
        }

        public virtual bool HasView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException($"viewName");
            }

            return Controls.ContainsKey(viewName);
        }

        protected virtual void ValidateControlName(string controlName)
        {
            if (string.IsNullOrEmpty(controlName))
            {
                throw new ArgumentNullException($"controlName");
            }

            if (!Controls.ContainsKey(controlName))
            {
                throw new Exception(string.Format(Resources.RegionAdapterBase_ValidateControlName_Control_not_found, controlName));
            }
        }
        
        protected virtual void ValidateRegionName(string targetRegion)
        {
            if (!Regions.ContainsKey(targetRegion))
            {
                throw new Exception(string.Format(Resources.RegionAdapterBase_ValidateRegionName_Region_not_found, targetRegion));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Opium.MVVM.Framework.View;

namespace Opium.MVVM.Framework.ViewModel
{
    /// <summary>
    ///     Router for views and view models
    /// </summary>
    public interface IViewModelRouter
    {
        
        bool ActivateView(string viewName, IDictionary<string, object> parameters);

        bool DeactivateView(string viewName);
        
        IViewModel ResolveViewModel(Type viewModelType);
        
        IViewModel ResolveViewModel(string viewModelType);
        
        T ResolveViewModel<T>(string viewModelType) where T : IViewModel;
        
        T ResolveViewModel<T>(bool activate, string viewModelType) where T : IViewModel;
        
        IViewModel GetNonSharedViewModel(string viewModelType);

        T GetNonSharedViewModel<T>() where T : IViewModel;

        UserControl GetNonSharedView(string viewTag, object dataContext);
  
        UserControl GetNonSharedView(string viewTag, object dataContext, Dictionary<string, object> parameters);

        T GetNonSharedView<T>(object dataContext) where T : UserControl;
        
        T GetNonSharedView<T>(object dataContext, Dictionary<string, object> parameters) where T : UserControl;

        IExportAsViewMetadata GetMetadataForView(string view);
        
        string GetViewModelTagForView(string view);
        
        string[] GetViewTagsForViewModel(string viewModel);

        UserControl this[string name] { get; }

        UserControl ViewQuery(string name);
        
        bool HasView(string name);

        List<ViewModelRoute> RouteList { get; }
        
        IExportAsViewMetadata GetViewMetadata(string viewName);
    }
}

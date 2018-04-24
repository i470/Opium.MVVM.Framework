using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Opium.MVVM.Framework.Fluent;
using Opium.MVVM.Framework.Properties;
using System.ComponentModel.Composition;
using Opium.MVVM.Framework.Services;
using Opium.MVVM.Framework.View;


namespace Opium.MVVM.Framework.ViewModel
{
    [Export(typeof(IViewModelRouter))]
    [Export(typeof(IFluentViewModelRouter))]
    public class ViewModelRouter : IViewModelRouter, IFluentViewModelRouter
    {
        
        [ImportMany(AllowRecomposition = true)]
        public ViewModelRoute[] Routes { get; set; }

      
        public List<ViewModelRoute> RouteList => new List<ViewModelRoute>(Routes.ToList().Concat(_fluentRoutes));

        private readonly List<ViewModelRoute> _fluentRoutes = new List<ViewModelRoute>();

    
        [ImportMany(AllowRecomposition = true)]
        public Lazy<UserControl, IExportAsViewMetadata>[] Views { get; set; }
        
        [ImportMany(AllowRecomposition = true)]
        public List<ExportFactory<UserControl, IExportAsViewMetadata>> ViewFactory { get; set; }

       
        [ImportMany(AllowRecomposition = true)]
        public Lazy<IViewModel, IExportAsViewModelMetadata>[] ViewModels { get; set; }

        [ImportMany(AllowRecomposition = true)]
        public List<ExportFactory<IViewModel, IExportAsViewModelMetadata>> ViewModelFactory { get; set; }
       public string GetViewModelTagForView(string view)
        {
            var vm = GetViewModelInfoForView(view);
            return vm == null ? string.Empty : vm.Metadata.ViewModelType;
        }

      
        public string[] GetViewTagsForViewModel(string viewModel)
        {
            var fluent = from r in _fluentRoutes where r.ViewModelType.Equals(viewModel) select r.ViewType;
            var exported = from r in Routes where r.ViewModelType.Equals(viewModel) select r.ViewType;

            return (from v in fluent.Union(exported) select v).Distinct().ToArray();
        }
        
        public UserControl this[string name] => ViewQuery(name);

        
        public UserControl ViewQuery(string name)
        {
            var v = GetViewInfo(name);
            return v?.Value;
        }

        public bool HasView(string name)
        {
            return GetViewInfo(name) != null;
        }

        
        public IExportAsViewMetadata GetViewMetadata(string viewName)
        {
            var info = GetViewInfo(viewName);
            return info == null ? null : info.Metadata;
        }

        private Lazy<UserControl, IExportAsViewMetadata> GetViewInfo(string viewName)
        {
            return (from v in Views where v.Metadata.ExportedViewType.Equals(viewName) select v).FirstOrDefault();
        }

        
        private Lazy<IViewModel, IExportAsViewModelMetadata> GetViewModelInfoForView(string view)
        {
            return (from r in _fluentRoutes
                    from vm in ViewModels
                    where r.ViewType.Equals(view)
                          && r.ViewModelType.Equals(vm.Metadata.ViewModelType)
                    select vm).FirstOrDefault() ??
                   (from r in Routes
                    from vm in ViewModels
                    where r.ViewType.Equals(view)
                          && r.ViewModelType.Equals(vm.Metadata.ViewModelType)
                    select vm).FirstOrDefault();
        }

        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

     
        public bool DeactivateView(string viewName)
        {
            if (HasView(viewName))
            {
                var vm = GetViewModelInfoForView(viewName);
                if (vm != null)
                {
                    if (vm.IsValueCreated)
                    {
                        vm.Value.Deactivate(viewName);
                    }
                    return true;
                }
            }
            return false;
        }

        public IViewModel ResolveViewModel(string viewModelType)
        {
            return
                (from vm in ViewModels where vm.Metadata.ViewModelType.Equals(viewModelType) select vm.Value).
                    FirstOrDefault();
        }
        
        public IViewModel ResolveViewModel(Type viewModelType)
        {
            return ResolveViewModel(viewModelType.FullName);
        }

        
        public T ResolveViewModel<T>(string viewModelType = null) where T : IViewModel
        {
            return ResolveViewModel<T>(true, viewModelType);
        }

        
        public T ResolveViewModel<T>(bool activate, string viewModelType = null) where T : IViewModel
        {
            return ResolveViewModel<T>(activate, viewModelType, new Dictionary<string, object>());
        }

        
        public IViewModel GetNonSharedViewModel(string viewModelType)
        {
            return (from factory in ViewModelFactory
                    where factory.Metadata.ViewModelType.Equals(viewModelType)
                    select factory.CreateExport().Value).FirstOrDefault();
        }

       
        public T GetNonSharedViewModel<T>() where T : IViewModel
        {
            return (T)GetNonSharedViewModel(typeof(T).FullName);
        }

        public UserControl GetNonSharedView(string viewTag, object dataContext, Dictionary<string, object> parameters)
        {
            var viewEntry = (from factory in ViewFactory
                             where factory.Metadata.ExportedViewType.Equals(viewTag)
                             select factory).FirstOrDefault();

            if (viewEntry == null)
            {
                return null;
            }

            var view = viewEntry.CreateExport().Value;
            var viewMetadata = viewEntry.Metadata;

            BindViewModel(view, dataContext);

            var viewModel = (IViewModel) dataContext;
            if (viewModel != null)
            {
                if (viewMetadata.DeactivateOnUnload)
                {
                    view.Unloaded += (o, e) =>
                        viewModel.Deactivate(viewMetadata.ExportedViewType);
                }

                viewModel.RegisterVisualState(viewTag,
                                                    (state, transitions) =>
                                                    OpiumHelper.ExecuteOnUI(
                                                        () => VisualStateManager.GoToState(view, state,
                                                                                            transitions)));
                viewModel.RegisteredViews.Add(viewTag);
                viewModel.Initialize();
                RoutedEventHandler loaded = null;
                loaded = (o, e) =>
                {
                    // ReSharper disable AccessToModifiedClosure
                    ((UserControl)o).Loaded -= loaded;
                    // ReSharper restore AccessToModifiedClosure
                    viewModel.Activate(viewTag, parameters);
                };
                view.Loaded += loaded;
            }
            return view;
        }
        
        public UserControl GetNonSharedView(string viewTag, object dataContext)
        {
            return GetNonSharedView(viewTag, dataContext, new Dictionary<string, object>());
        }

       
        public T GetNonSharedView<T>(object dataContext) where T : UserControl
        {
            return GetNonSharedView<T>(dataContext, new Dictionary<string, object>());
        }

        
        public T GetNonSharedView<T>(object dataContext, Dictionary<string, object> parameters) where T : UserControl
        {
            return (T)GetNonSharedView(typeof(T).FullName, dataContext, parameters);
        }

       
        public T ResolveViewModel<T>(bool activate, string viewModelType = null,
                                     IDictionary<string, object> parameters = null) where T : IViewModel
        {
            if (viewModelType == null)
            {
                viewModelType = typeof(T).FullName;
            }

            var vmInfo = (from vm in ViewModels
                          where vm.Metadata.ViewModelType.Equals(viewModelType)
                          select vm).FirstOrDefault();

            if (vmInfo == null)
            {
                return default(T);
            }

            var initialize = !vmInfo.IsValueCreated;

            var viewModel = vmInfo.Value;

            if (initialize)
            {
                viewModel.Initialize();
            }

            if (activate)
            {
                viewModel.Activate(string.Empty, parameters);
            }

            return (T)viewModel;
        }

        
        public IExportAsViewMetadata GetMetadataForView(string view)
        {
            var viewInfo = GetViewInfo(view);
            return viewInfo == null ? null : viewInfo.Metadata;
        }

        
        public bool ActivateView(string viewName, IDictionary<string, object> parameters)
        {
            Logger.LogFormat(LogSeverity.Verbose, GetType().FullName, Resources.ViewModelRouter_ActivateView,
                             MethodBase.GetCurrentMethod().Name,
                             viewName);

            if (HasView(viewName))
            {
                var viewInfo = GetViewInfo(viewName);
                var view = viewInfo.Value;

                var viewModelInfo = GetViewModelInfoForView(viewName);

                if (viewModelInfo != null)
                {
                    var firstTime = !viewModelInfo.IsValueCreated;

                    var viewModel = viewModelInfo.Value;

                    if (!viewModel.RegisteredViews.Contains(viewName))
                    {
                        viewModel.RegisterVisualState(viewName,
                                                          (state, transitions) =>
                                                          OpiumHelper.ExecuteOnUI(
                                                              () => VisualStateManager.GoToState(view, state,
                                                                                                 transitions)));
                        BindViewModel(view, viewModel);
                        viewModel.RegisteredViews.Add(viewName);
                    }

                    if (firstTime)
                    {
                        if (viewInfo.Metadata.DeactivateOnUnload)
                        {
                            view.Unloaded += (o, e) => viewModel.Deactivate(viewName);
                        }
                        viewModel.Initialize();
                        RoutedEventHandler loaded = null;
                        loaded = (o, e) =>
                        {
                            // ReSharper disable AccessToModifiedClosure
                            ((UserControl)o).Loaded -= loaded;
                            // ReSharper restore AccessToModifiedClosure
                            viewModel.Activate(viewName, parameters);
                        };
                        view.Loaded += loaded;
                    }
                    else
                    {
                        viewModel.Activate(viewName, parameters);
                    }

                    if (parameters.ContainsKey(Constants.WINDOW_TITLE))
                    {
                        viewModel.SetTitle(parameters.ParameterValue<string>(Constants.WINDOW_TITLE));
                    }
                }

                if (parameters.ContainsKey(Constants.AS_WINDOW) &&
                    parameters.ParameterValue<bool>(Constants.AS_WINDOW))
                {
                    var title = string.Empty;

                    if (parameters.ContainsKey(Constants.WINDOW_TITLE))
                    {
                        title = parameters.ParameterValue<string>(Constants.WINDOW_TITLE);
                    }

                    var height = 480.0;
                    if (parameters.ContainsKey(Constants.WINDOW_HEIGHT))
                    {
                        height = parameters.ParameterValue<double>(Constants.WINDOW_HEIGHT);
                    }

                    var width = 640.0;
                    if (parameters.ContainsKey(Constants.WINDOW_WIDTH))
                    {
                        width = parameters.ParameterValue<double>(Constants.WINDOW_WIDTH);
                    }

                    var window = new Window
                    {
                        Height = height,
                        Width = width,
                        WindowState = WindowState.Normal,
                        Topmost = true,
                        Title = title,
                        Content = view,
                        Visibility = Visibility.Visible
                    };
                    parameters.Add(Constants.WINDOW_REFERENCE, window);
                }

                return true;
            }
            return false;
        }

       
        private static void BindViewModel(FrameworkElement view, object viewModel)
        {
            var root = VisualTreeHelper.GetChild(view, 0);

            if (root != null)
            {
                ((FrameworkElement)root).DataContext = viewModel;
            }
            else
            {
                view.Loaded += (o, e) => view.DataContext = viewModel;
            }
        }

        
        public void RouteViewModelForView(string viewModel, string view)
        {
            _fluentRoutes.Add(ViewModelRoute.Create(viewModel, view));
        }
        
        public void RouteViewModelForView<T, TView>() where T : IViewModel where TView : UserControl
        {
            RouteViewModelForView(typeof(T).FullName, typeof(TView).FullName);
        }
    }
}

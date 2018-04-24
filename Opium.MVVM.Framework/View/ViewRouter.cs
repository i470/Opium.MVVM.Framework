using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Castle.Core.Logging;
using Opium.MVVM.Framework.Event;
using Opium.MVVM.Framework.Fluent;
using Opium.MVVM.Framework.ViewModel;
using UserControl = System.Windows.Controls.UserControl;

namespace Opium.MVVM.Framework.View
{
    /// <summary>
    ///     The view router is responsible for on-demand loading
    ///     It listens to view events, asks the deployment service to load, then
    ///     activates the view
    /// </summary>
    /// <remarks>
    /// This is the main router to locate and parse views.
    /// </remarks>
    [Export]
    [Export(typeof(IFluentViewRouter))]
    public class ViewRouter : IFluentViewRouter, IPartImportsSatisfiedNotification, IEventSink<ViewNavigationArgs>
    {
        private bool _initialized;

        /// <summary>
        /// The deployment service reference to <see cref="IDeploymentService"/>
        /// </summary>
        [Import]
        public IDeploymentService DeploymentService { get; set; }

        /// <summary>
        /// The instance of the <see cref="IViewModelRouter"/>
        /// </summary>
        [Import]
        public IViewModelRouter ViewModelRouter { get; set; }

        /// <summary>
        ///  A list of view locations using the <see cref="ViewRoute"/>
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public ViewRoute[] ViewLocations { get; set; }

        /// <summary>
        /// List of fluently configured <see cref="ViewRoute"/>
        /// </summary>
        private readonly List<ViewRoute> _fluentRoutes = new List<ViewRoute>();

        /// <summary>
        /// Event aggregator instance that implements <see cref="IEventAggregator"/>
        /// </summary>
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// Instance of the <see cref="ILogger"/>
        /// </summary>
        [Import(AllowDefault = true, AllowRecomposition = true)]
        public ILogger Logger { get; set; }

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            EventAggregator.SubscribeOnDispatcher(this);
        }

        /// <summary>
        ///     Hook into navigation event
        /// </summary>
        /// <param name="e">View navigation args</param>
        public void HandleEvent(ViewNavigationArgs e)
        {
            if (e.Deactivate)
            {
                ViewModelRouter.DeactivateView(e.ViewType);
                EventAggregator.Publish(new ViewNavigatedArgs(e.ViewType) { Deactivate = true });
                return;
            }

            // does a view location exist?
            var viewLocation =
                (from location in _fluentRoutes
                 where location.ViewName.Equals(e.ViewType, StringComparison.InvariantCultureIgnoreCase)
                 select location).FirstOrDefault() ??
                (from location in ViewLocations
                 where location.ViewName.Equals(e.ViewType, StringComparison.InvariantCultureIgnoreCase)
                 select location).FirstOrDefault();

            // if so, try to load the dll, then activate the view
            if (viewLocation != null)
            {
                DeploymentService.RequestXap(viewLocation.View,
                                             exception =>
                                             {
                                                 if (exception != null)
                                                 {
                                                     throw exception;
                                                 }
                                                 _ActivateView(e.ViewType, e.ViewParameters);
                                             });
            }
            else
            {
                // just activate the view directly
                _ActivateView(e.ViewType, e.ViewParameters);
            }
        }

        /// <summary>
        ///     Activate the view
        /// </summary>
        /// <param name="viewName">The name of the view</param>
        /// <param name="parameters">Parameters for the view</param>
        private void _ActivateView(string viewName, IDictionary<string, object> parameters)
        {
            ViewModelRouter.ActivateView(viewName, parameters);
            EventAggregator.Publish(new ViewNavigatedArgs(viewName));
        }

        /// <summary>
        /// Use to fluently route a view to a dll file
        /// </summary>
        /// <param name="view">The tag for the view</param>
        /// <param name="dll">The name of the DLL module</param>
        public void RouteViewInDll(string view, string dll)
        {
            _fluentRoutes.Add(ViewRoute.Create(view, dll));
        }

        public void RouteViewInDll<T>(string dll) where T : UserControl
        {
           //todo
        }
    }
}

using System.Windows.Controls;
using Opium.MVVM.Framework.Properties;

namespace Opium.MVVM.Framework.View
{
    /// <summary>
    ///     The view locator element - maps views to dynamically loaded views
    /// </summary>
    /// <remarks>
    /// With this route, Opium is informed that a view lives in another dll file and 
    /// can automatically load the dll prior to navigation
    /// </remarks>
    public class ViewRoute
    {
        /// <summary>
        /// Supress public creation
        /// </summary>
        private ViewRoute()
        {
        }

        /// <summary>
        /// Create a route using a view tag and the name of the XAP
        /// </summary>
        /// <param name="viewName">The tag for the view</param>
        /// <param name="view">The name of the view file</param>
        /// <returns>A new instance of the route</returns>
        public static ViewRoute Create(string viewName, string view)
        {
            return new ViewRoute { ViewName = viewName, View = view };
        }

        /// <summary>
        /// Create a route using the full name of the type for the view as the tag and the name of the XAP
        /// </summary>
        /// <typeparam name="T">The type of the view</typeparam>
        /// <param name="view">The name of the XAP file</param>
        /// <returns>A new instance of the route</returns>
        public static ViewRoute Create<T>(string view) where T : UserControl
        {
            return new ViewRoute { ViewName = typeof(T).FullName, View = view };
        }

        /// <summary>
        /// Tag for the view
        /// </summary>
        public string ViewName { get; private set; }

        /// <summary>
        /// The dll file the view lives in
        /// </summary>
        public string View { get; private set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>The string representation</returns>
        public override string ToString()
        {
            return string.Format(Resources.ViewXapRoute_ToString_View_Route_View, ViewName, View);
        }
    }
}

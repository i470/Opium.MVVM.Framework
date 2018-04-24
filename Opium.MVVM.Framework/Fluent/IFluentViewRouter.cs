using System.Windows.Controls;

namespace Opium.MVVM.Framework.Fluent
{
  
    public interface IFluentViewRouter
    {
        /// <summary>
        /// Route a view to a dll module file
        /// </summary>
        /// <param name="view">The tag for the view</param>
        /// <param name="dll">The name of the dll file</param>
        void RouteViewInDll(string view, string dll);

        /// <summary>
        /// Route a view to a DLL file
        /// </summary>
        /// <typeparam name="T">The type of the view (full name will be used for the tag)</typeparam>
        /// <param name="dll">The name of the DLL file</param>
        void RouteViewInDll<T>(string dll) where T : UserControl;
    }
}

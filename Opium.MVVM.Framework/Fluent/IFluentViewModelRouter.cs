using System.Windows.Controls;
using Opium.MVVM.Framework.ViewModel;

namespace Opium.MVVM.Framework.Fluent
{
    
    public interface IFluentViewModelRouter
    {
        void RouteViewModelForView(string viewModel, string view);
        void RouteViewModelForView<T, TView>() where T : IViewModel where TView : UserControl;
    }
}

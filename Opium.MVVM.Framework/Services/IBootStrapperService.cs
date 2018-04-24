using System;

namespace Opium.MVVM.Framework.Services
{
    public interface IBootStrapperService
    {
        void RequestModule(string viewLocationView, Action<Exception> action);
    }
}

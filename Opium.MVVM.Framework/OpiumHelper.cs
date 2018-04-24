using System;
using System.Collections.Generic;
using System.Windows;
using Opium.MVVM.Framework.View;

namespace Opium.MVVM.Framework
{
   
    public static class OpiumHelper
    {
        public static ViewNavigationArgs AsViewNavigationArgs(this Type view)
            {
                return new ViewNavigationArgs(view);
            }
        
            public static ViewNavigationArgs AsViewNavigationArgs(this string viewName)
            {
                return new ViewNavigationArgs(viewName);
            }
        
            public static ViewNavigationArgs AsOutOfBrowserWindow(this ViewNavigationArgs args)
            {
                args.AddNamedParameter(Constants.AS_WINDOW, true);
                return args;
            }
        
            public static ViewNavigationArgs WithTitle(this ViewNavigationArgs args, string title)
            {
                args.AddNamedParameter(Constants.WINDOW_TITLE, title);
                return args;
            }
        
            public static ViewNavigationArgs WindowWidth(this ViewNavigationArgs args, double width)
            {
                args.AddNamedParameter(Constants.WINDOW_WIDTH, width);
                return args;
            }
        
            public static ViewNavigationArgs WindowHeight(this ViewNavigationArgs args, double height)
            {
                args.AddNamedParameter(Constants.WINDOW_HEIGHT, height);
                return args;
            }
        
            public static ViewNavigationArgs AddNamedParameter<T>(this ViewNavigationArgs args, string name, T value)
            {
                args.ViewParameters.Add(name, value);
                return args;
            }
        
            public static T ParameterValue<T>(this IDictionary<string, object> parameters, string name)
            {
                if (parameters.ContainsKey(name) && parameters[name] is T)
                {
                    return (T)parameters[name];
                }

                return default(T);
            }

        
            public static void ExecuteOnUI(Action action)
            {
                var dispatcher = Application.Current.Dispatcher;
                if (dispatcher.CheckAccess())
                    action();
                else dispatcher.BeginInvoke(action);
            }
        }
    }

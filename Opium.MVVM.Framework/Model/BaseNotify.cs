
﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
 using Opium.MVVM.Framework.Properties;

namespace Opium.MVVM.Framework.Model
{
    
    public abstract class BaseNotify : INotifyPropertyChanged
    {
       
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                RaisePropertyChanged(name);
            }
        }

       
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = ExtractPropertyName(propertyExpression);
            RaisePropertyChanged(propertyName);
        }

     
        protected string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException($"propertyExpression");
            }

            if (!(propertyExpression.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException(Resources.BaseNotify_ExtractPropertyName_NotAMember, "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException(Resources.BaseNotify_ExtractPropertyName_NotAProperty, "propertyExpression");
            }

            var getMethod = property.GetGetMethod(true);

            if (getMethod == null)
            {
                // this shouldn't happen - the expression would reject the property before reaching this far
                throw new ArgumentException(Resources.BaseNotify_ExtractPropertyName_NoGetter, "propertyExpression");
            }

            if (getMethod.IsStatic)
            {
                throw new ArgumentException(Resources.BaseNotify_ExtractPropertyName_Static, "propertyExpression");
            }

            return memberExpression.Member.Name;
        }
    }
}
using System;
using System.Composition;
using System.Windows;
using System.Windows.Interactivity;
using Opium.MVVM.Framework.Event;

namespace Opium.MVVM.Framework.Services
{
    
    public class NavigationTrigger : TriggerAction<UIElement>
    {
    
        private static IEventAggregator _eventAggregator;

      
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target",
            typeof(string),
            typeof(NavigationTrigger),
            new PropertyMetadata(string.Empty));

       
        public string Target
        {
            get { return GetValue(TargetProperty).ToString(); }
            set { SetValue(TargetProperty, value); }
        }

       
        protected override void Invoke(object parameter)
        {
            if (_eventAggregator == null)
            {
                CompositionInitializer.SatisfyImports(this);
                _eventAggregator = EventAggregator;
            }
            _eventAggregator.Publish(Target.AsViewNavigationArgs());
        }

    }
}
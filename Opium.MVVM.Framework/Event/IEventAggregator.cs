namespace Opium.MVVM.Framework.Event
{
   
    public interface IEventAggregator
    {
        
        void Publish<TEvent>(TEvent sampleEvent);

        void Subscribe<TEvent>(IEventSink<TEvent> subscriber);
        
        void SubscribeOnDispatcher<TEvent>(IEventSink<TEvent> subscriber);
        
        void Unsubscribe<TEvent>(IEventSink<TEvent> unsubscriber);
    }
}

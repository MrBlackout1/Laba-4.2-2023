using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba_4._2_2023
{
    public class EventBus
    {
        private Dictionary<string, List<Delegate>> _eventHandlers;
        private DateTime _lastEventTime;
        private int _eventThrottleMilliseconds;

        public EventBus(int eventThrottleMilliseconds)
        {
            _eventHandlers = new Dictionary<string, List<Delegate>>();
            _eventThrottleMilliseconds = eventThrottleMilliseconds;
            _lastEventTime = DateTime.MinValue;
        }

        public void RegisterEventHandler(string eventName, Delegate eventHandler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = new List<Delegate>();
            }

            _eventHandlers[eventName].Add(eventHandler);
        }

        public void UnregisterEventHandler(string eventName, Delegate eventHandler)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName].Remove(eventHandler);
            }
        }

        public void RaiseEvent(string eventName, object sender, EventArgs args)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                var eventHandlers = _eventHandlers[eventName];

                var timeSinceLastEvent = DateTime.Now - _lastEventTime;
                if (timeSinceLastEvent.TotalMilliseconds < _eventThrottleMilliseconds)
                {
                    var waitTime = _eventThrottleMilliseconds - timeSinceLastEvent.TotalMilliseconds;
                    Thread.Sleep((int)waitTime);
                }
                foreach (var eventHandler in eventHandlers)
                {
                    eventHandler.DynamicInvoke(sender, args);
                }

                _lastEventTime = DateTime.Now;
            }
        }

        public class Publisher
        {
            private Dictionary<string, List<Action>> _eventHandlers;
            public Publisher()
            {
                _eventHandlers = new Dictionary<string, List<Action>>();
            }
            public void RegisterEventHandler(string eventName, Action eventHandler, int priority)
            {
                if (!_eventHandlers.ContainsKey(eventName))
                {
                    _eventHandlers[eventName] = new List<Action>();
                }
                var handlers = _eventHandlers[eventName];
                var index = handlers.FindIndex(h => h.Target == null || h.Target.Equals(eventHandler.Target) && h.Method.Equals(eventHandler.Method));
                if (index < 0)
                {
                    index = handlers.Count;
                }
                handlers.Insert(index, eventHandler);
            }
            public void UnregisterEventHandler(string eventName, Action eventHandler)
            {
                if (_eventHandlers.ContainsKey(eventName))
                {
                    var handlers = _eventHandlers[eventName];
                    handlers.RemoveAll(h => h.Target == null || h.Target.Equals(eventHandler.Target) && h.Method.Equals(eventHandler.Method));
                }
            }

            public void RaiseEvent(string eventName)
            {
                if (_eventHandlers.ContainsKey(eventName))
                {
                    var handlers = _eventHandlers[eventName];
                    foreach (var handler in handlers)
                    {
                        handler();
                    }
                }
            }
        }
        public class Subscriber
        {
            private Publisher _publisher;
            private int _priority;

            public Subscriber(Publisher publisher, int priority)
            {
                _publisher = publisher;
                _priority = priority;
            }

            public void Subscribe(string eventName, Action eventHandler)
            {
                _publisher.RegisterEventHandler(eventName, eventHandler, _priority);
            }

            public void Unsubscribe(string eventName, Action eventHandler)
            {
                _publisher.UnregisterEventHandler(eventName, eventHandler);
            }
        }
    }
}

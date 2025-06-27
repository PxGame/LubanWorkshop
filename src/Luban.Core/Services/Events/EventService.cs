using Luban.Core.Containers;
using Luban.Core.Services.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Events
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ListenerRootAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ListenerAttribute : Attribute
    {
        public string EventName { get; init; }

        public ListenerAttribute(string eventName)
        {
            EventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
        }
    }

    public abstract class IListenerSource
    {
        public string GroupName { get; init; }
        public string EventName { get; init; }

        public void Trigger(object[] args)
        {
        }
    }

    internal class EventService : IEventService
    {
        private List<IListenerSource> _listeners = new List<IListenerSource>();

        public override void OnResolved()
        {
        }

        public void AddListener(IListenerSource eventSource)
        {
            if (eventSource == null) { throw new ArgumentNullException(nameof(eventSource)); }
            _listeners.Add(eventSource);
        }

        public void RemoveListener(IListenerSource eventSource)
        {
            _listeners.RemoveAll(e => e == eventSource);
        }

        public void AddListeners(object owner)
        {
        }

        public void RemoveListeners(object owner)
        {
        }

        public void Trigger(string groupName, string eventName, object[] args)
        {
            foreach (var listener in _listeners.Where(l => l.GroupName == groupName && l.EventName == eventName))
            {
                listener.Trigger(args);
            }
        }

        public override void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public override async Task OnServiceInitialing()
        {
            await Task.CompletedTask;
        }

        public override async Task OnServiceInitialized()
        {
            await base.OnServiceInitialized();
            Log.Information($"OnServiceInitialized");
            await Task.CompletedTask;
        }

        public override async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");
            await Task.CompletedTask;
        }
    }
}
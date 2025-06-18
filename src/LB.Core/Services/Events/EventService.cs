using LB.Core.Containers;
using LB.Core.Services.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LB.Core.Services.Events
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
        public string EventName { get; init; }

        public void Trigger(object[] args)
        {
        }
    }

    internal class EventService : IEventService
    {
        [Inject]
        [Log(Tag = "事件服务")]
        private ILog Log { get; init; }

        private List<IListenerSource> _listeners = new List<IListenerSource>();

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

        public void Trigger(string eventName, object[] args)
        {
            Log.Debug($"Trigger event: {eventName} with args: {string.Join(", ", args.Select(a => a?.ToString() ?? "null"))}");
            foreach (var listener in _listeners.Where(l => l.EventName == eventName))
            {
                listener.Trigger(args);
            }
        }

        public void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public void OnResolved()
        {
            Log.Information($"OnResolved");
        }

        public async Task OnServiceInitialize()
        {
            Log.Information($"OnServiceInitialize");
            await Task.CompletedTask;
        }

        public async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");
            await Task.CompletedTask;
        }
    }
}
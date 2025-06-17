using LB.Core.Containers;
using LB.Core.Services.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LB.Core.Services.Events
{
    public class EventSource
    {
    }

    internal class EventService : IEventService
    {
        [Inject]
        [Log(Tag = "事件服务")]
        private ILog Log { get; init; }

        private Dictionary<string, List<EventSource>> _eventDict = new Dictionary<string, List<EventSource>>();

        //public void Trigger(string eventName, params object[] args)
        //{
        //    Log.Information($"Trigger event: {eventName} with args: {string.Join(", ", args.Select(a => a?.ToString() ?? "null"))}");
        //    if (_eventDict.TryGetValue(eventName, out var sources))
        //    {
        //        foreach (var source in sources)
        //        {
        //            // Handle the event for each source
        //            // This is where you would invoke the event handlers
        //        }
        //    }
        //}

        //public void Register(string eventName, EventSource source)
        //{
        //    Log.Information($"Register event: {eventName} for source: {source}");
        //    if (!_eventDict.ContainsKey(eventName))
        //    {
        //        _eventDict[eventName] = new List<EventSource>();
        //    }
        //    _eventDict[eventName].Add(source);
        //}

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
using System;
using System.Collections.Generic;

namespace Plugins.GetStreamIO.Core.Events
{
    /// <summary>
    /// Repository with event handlers
    /// </summary>
    public class EventHandlersRepository : IEventHandlersRepository
    {
        public void Register<TType>(string key, Action<string> handler)
        {
            _mapping.Add(key, handler);
        }

        public bool TryHandleEvent(string key, string msg)
        {
            if (!_mapping.TryGetValue(key, out var handler))
            {
                return false;
            }

            handler(msg);
            return true;
        }

        private readonly Dictionary<string, Action<string>> _mapping = new Dictionary<string, Action<string>>();
    }
}
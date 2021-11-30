using System;

namespace Plugins.GetStreamIO.Core.Events
{
    /// <summary>
    /// Repository with event handlers
    /// </summary>
    public interface IEventHandlersRepository
    {
        void Register<TType>(string key, Action<string> handler);

        bool TryHandleEvent(string key, string msg);
    }
}
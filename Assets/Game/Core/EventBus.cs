using System;
using System.Collections.Generic;

namespace StartupEmpire.Core
{
    /// Pub/sub simples e tipado, sem dependência de UnityEngine, usado para
    /// desacoplar o domínio (Economy, Products, Missions, ...) da camada de UI.
    public sealed class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _handlers[type] = list;
            }
            list.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (_handlers.TryGetValue(typeof(T), out var list))
            {
                list.Remove(handler);
            }
        }

        public void Publish<T>(T evt)
        {
            if (!_handlers.TryGetValue(typeof(T), out var list)) return;
            var snapshot = list.ToArray();
            foreach (var handler in snapshot)
            {
                ((Action<T>)handler).Invoke(evt);
            }
        }
    }
}

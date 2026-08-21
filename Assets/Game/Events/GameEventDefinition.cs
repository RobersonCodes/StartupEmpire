using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Economy;

namespace StartupEmpire.Events
{
    public sealed class EventChoice
    {
        public string Id;
        public string Label;
        public Action<GameState, EconomyEngine> Apply;
    }

    /// Evento data-driven com escolhas e consequências reais (seção 14 da missão).
    /// "Data-driven" aqui significa declarado uma única vez em EventCatalog, desacoplado
    /// da UI — a UI só lê Title/Description/Choices e chama EventService.ResolveChoice.
    public sealed class GameEventDefinition
    {
        public string Id;
        public string Title;
        public string Description;
        public Func<GameState, bool> TriggerCondition;
        public List<EventChoice> Choices = new();
    }
}

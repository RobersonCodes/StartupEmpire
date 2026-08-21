using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Economy;

namespace StartupEmpire.Events
{
    public sealed class EventService
    {
        private readonly List<GameEventDefinition> _definitions;
        private readonly EventConfigValues _config;
        private readonly EconomyEngine _economy;
        private readonly EventBus _eventBus;
        private readonly Random _random;

        public EventService(List<GameEventDefinition> definitions, EventConfigValues config,
            EconomyEngine economy, EventBus eventBus, Random random = null)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _economy = economy ?? throw new ArgumentNullException(nameof(economy));
            _eventBus = eventBus;
            _random = random ?? new Random();
        }

        /// Sorteia, no máximo, um evento elegível por chamada (tipicamente um ciclo de jogo).
        public GameEventDefinition TryTriggerRandomEvent(GameState state)
        {
            if (_random.NextDouble() > _config.BaseTriggerChancePerCycle) return null;

            var eligible = new List<GameEventDefinition>();
            foreach (var definition in _definitions)
            {
                if (definition.TriggerCondition == null || definition.TriggerCondition(state))
                {
                    eligible.Add(definition);
                }
            }
            if (eligible.Count == 0) return null;

            var chosen = eligible[_random.Next(eligible.Count)];
            _eventBus?.Publish(new GameEventTriggeredEvent(chosen.Id));
            return chosen;
        }

        public bool ResolveChoice(GameState state, GameEventDefinition eventDefinition, string choiceId)
        {
            var choice = eventDefinition.Choices.Find(c => c.Id == choiceId);
            if (choice == null) return false;

            choice.Apply?.Invoke(state, _economy);
            _eventBus?.Publish(new GameEventResolvedEvent(eventDefinition.Id, choiceId));
            return true;
        }
    }
}

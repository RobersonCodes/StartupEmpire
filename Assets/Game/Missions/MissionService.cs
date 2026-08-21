using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Economy;

namespace StartupEmpire.Missions
{
    public sealed class MissionService
    {
        private readonly List<MissionDefinition> _definitions;
        private readonly EventBus _eventBus;
        private readonly EconomyEngine _economy;

        public MissionService(List<MissionDefinition> definitions, EventBus eventBus, EconomyEngine economy)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _eventBus = eventBus;
            _economy = economy;
        }

        public IReadOnlyList<string> EvaluateAll(GameState state)
        {
            var justCompleted = new List<string>();
            foreach (var definition in _definitions)
            {
                if (state.Missions.CompletedMissionIds.Contains(definition.Id)) continue;
                if (!definition.IsComplete(state)) continue;

                state.Missions.CompletedMissionIds.Add(definition.Id);
                if (definition.RewardCash > 0)
                {
                    _economy?.Earn(state.Economy, definition.RewardCash, $"mission:{definition.Id}");
                }
                _eventBus?.Publish(new MissionCompletedEvent(definition.Id));
                justCompleted.Add(definition.Id);
            }
            return justCompleted;
        }
    }
}

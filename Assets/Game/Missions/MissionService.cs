using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Premium;

namespace StartupEmpire.Missions
{
    public sealed class MissionService
    {
        private readonly List<MissionDefinition> _definitions;
        private readonly EventBus _eventBus;
        private readonly EconomyEngine _economy;
        private readonly GemWalletService _gemWallet;

        public MissionService(List<MissionDefinition> definitions, EventBus eventBus, EconomyEngine economy,
            GemWalletService gemWallet = null)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _eventBus = eventBus;
            _economy = economy;
            _gemWallet = gemWallet;
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
                if (definition.RewardGems > 0)
                {
                    _gemWallet?.Grant(state.GemWallet, definition.RewardGems, GemLedgerCategory.Reward, $"mission:{definition.Id}");
                }
                _eventBus?.Publish(new MissionCompletedEvent(definition.Id));
                justCompleted.Add(definition.Id);
            }
            return justCompleted;
        }
    }
}

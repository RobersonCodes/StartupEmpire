using System;
using System.Collections.Generic;
using StartupEmpire.Core;

namespace StartupEmpire.Achievements
{
    public sealed class AchievementService
    {
        private readonly List<AchievementDefinition> _definitions;
        private readonly EventBus _eventBus;

        public AchievementService(List<AchievementDefinition> definitions, EventBus eventBus)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _eventBus = eventBus;
        }

        public IReadOnlyList<string> EvaluateAll(GameState state)
        {
            var justUnlocked = new List<string>();
            foreach (var definition in _definitions)
            {
                if (state.UnlockedAchievements.Contains(definition.Id)) continue;
                if (!definition.IsUnlocked(state)) continue;

                state.UnlockedAchievements.Add(definition.Id);
                _eventBus?.Publish(new AchievementUnlockedEvent(definition.Id));
                justUnlocked.Add(definition.Id);
            }
            return justUnlocked;
        }
    }
}

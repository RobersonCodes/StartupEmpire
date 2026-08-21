using System;
using StartupEmpire.Core;

namespace StartupEmpire.Achievements
{
    public sealed class AchievementDefinition
    {
        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public Func<GameState, bool> IsUnlocked { get; }

        public AchievementDefinition(string id, string title, string description, Func<GameState, bool> isUnlocked)
        {
            Id = id;
            Title = title;
            Description = description;
            IsUnlocked = isUnlocked;
        }
    }
}

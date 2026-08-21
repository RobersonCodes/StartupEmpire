using System;
using StartupEmpire.Core;

namespace StartupEmpire.Missions
{
    /// Definição de missão desacoplada da UI: a condição é uma função pura sobre GameState.
    public sealed class MissionDefinition
    {
        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public Func<GameState, bool> IsComplete { get; }
        public double RewardCash { get; }
        public int RewardGems { get; }

        public MissionDefinition(string id, string title, string description,
            Func<GameState, bool> isComplete, double rewardCash = 0, int rewardGems = 0)
        {
            Id = id;
            Title = title;
            Description = description;
            IsComplete = isComplete;
            RewardCash = rewardCash;
            RewardGems = rewardGems;
        }
    }
}

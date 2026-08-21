using System.Collections.Generic;

namespace StartupEmpire.Upgrades
{
    public sealed class UpgradeState
    {
        public Dictionary<string, int> LevelByUpgradeId { get; } = new();

        public int GetLevel(string upgradeId) =>
            LevelByUpgradeId.TryGetValue(upgradeId, out var level) ? level : 0;

        public void SetLevel(string upgradeId, int level) => LevelByUpgradeId[upgradeId] = level;
    }
}

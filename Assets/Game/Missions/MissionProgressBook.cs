using System.Collections.Generic;

namespace StartupEmpire.Missions
{
    public sealed class MissionProgressBook
    {
        public HashSet<string> CompletedMissionIds { get; } = new();
    }
}

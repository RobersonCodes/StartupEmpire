using System.Collections.Generic;

namespace StartupEmpire.Core
{
    public sealed class PlayerState
    {
        public string Name { get; set; } = "Founder";
        public Dictionary<string, int> KnowledgeByTrack { get; } = new();
        public int WorkCyclesPerDay { get; set; } = 4;

        public int GetKnowledge(string track) =>
            KnowledgeByTrack.TryGetValue(track, out var value) ? value : 0;

        public void AddKnowledge(string track, int amount)
        {
            KnowledgeByTrack[track] = GetKnowledge(track) + amount;
        }
    }
}

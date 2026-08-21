using System;
using System.Collections.Generic;

namespace StartupEmpire.Core
{
    public sealed class PlayerState
    {
        /// Identificador estável gerado no cliente (tipo device/install id), usado
        /// para falar com o backend de Ranking/Referrals. Não é autenticação real —
        /// ver seção 3 da missão ("autenticação futura" é item separado).
        public string PlayerId { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "Founder";
        public Dictionary<string, int> KnowledgeByTrack { get; } = new();
        public int WorkCyclesPerDay { get; internal set; } = 4;
        public int CurrentDay { get; internal set; } = 1;
        public int RemainingWorkCycles { get; internal set; } = 4;

        public int GetKnowledge(string track) =>
            KnowledgeByTrack.TryGetValue(track, out var value) ? value : 0;

        public void AddKnowledge(string track, int amount)
        {
            KnowledgeByTrack[track] = GetKnowledge(track) + amount;
        }

        public bool TryConsumeWorkCycles(int cycles)
        {
            if (cycles <= 0 || cycles > RemainingWorkCycles) return false;
            RemainingWorkCycles -= cycles;
            return true;
        }

        public void StartNextDay()
        {
            CurrentDay++;
            RemainingWorkCycles = WorkCyclesPerDay;
        }

        internal void RestoreWorkSchedule(int workCyclesPerDay, int currentDay, int remainingWorkCycles)
        {
            WorkCyclesPerDay = Math.Max(1, workCyclesPerDay);
            CurrentDay = Math.Max(1, currentDay);
            RemainingWorkCycles = Math.Clamp(remainingWorkCycles, 0, WorkCyclesPerDay);
        }
    }
}

using System;
using StartupEmpire.Core;

namespace StartupEmpire.Research
{
    /// Ação "Aprender" do loop principal — ganha conhecimento em uma trilha.
    public sealed class LearningService
    {
        private readonly LearningConfigValues _config;

        public LearningService(LearningConfigValues config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Study(PlayerState player, string track, int cycles)
        {
            if (cycles <= 0) return;
            player.AddKnowledge(track, _config.KnowledgePointsPerCycle * cycles);
        }
    }
}

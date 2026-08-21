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

        /// knowledgeMultiplier vem de Upgrades (ex.: cursos online aceleram o aprendizado).
        public void Study(PlayerState player, string track, int cycles, double knowledgeMultiplier = 1.0)
        {
            if (cycles <= 0) return;
            var amount = (int)Math.Round(_config.KnowledgePointsPerCycle * cycles * knowledgeMultiplier);
            player.AddKnowledge(track, amount);
        }
    }
}

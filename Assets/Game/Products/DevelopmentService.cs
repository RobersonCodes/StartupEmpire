using System;
using StartupEmpire.Core;

namespace StartupEmpire.Products
{
    public sealed class DevelopmentConfigValues
    {
        public double DevPointsPerCycle = 10.0;
        public double KnowledgeBonusPerPoint = 0.02;
        public double TestEfficiencyPerCycle = 0.6;
        public double FixPointsPerCycle = 3.0;
        public double LaunchReputationPenaltyPerOutstandingBug = 0.01;
    }

    /// Regras de "Desenvolver / Testar / Corrigir / Lançar" do loop principal (seção 5 da missão).
    public sealed class DevelopmentService
    {
        private readonly DevelopmentConfigValues _config;
        private readonly EventBus _eventBus;

        public DevelopmentService(DevelopmentConfigValues config, EventBus eventBus)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _eventBus = eventBus;
        }

        public void Develop(ProductState product, PlayerState player, string knowledgeTrack, int cycles)
        {
            if (cycles <= 0) return;
            if (product.Stage == ProductStage.Planning) product.Stage = ProductStage.Development;
            if (product.Stage != ProductStage.Development) return;

            var knowledge = player.GetKnowledge(knowledgeTrack);
            var speedMultiplier = 1.0 + knowledge * _config.KnowledgeBonusPerPoint;
            var progressGained = _config.DevPointsPerCycle * speedMultiplier * cycles;
            product.DevProgress += progressGained;

            var qualityFactor = Math.Clamp(0.3 + knowledge * 0.01, 0.3, 0.95);
            var bugsIntroduced = (int)Math.Round(progressGained * product.Definition.BugRatePerProgress * (1 - qualityFactor));
            if (bugsIntroduced > 0)
            {
                product.BugCount += bugsIntroduced;
                _eventBus?.Publish(new BugFoundEvent(product.Id, bugsIntroduced));
            }
            product.Quality = qualityFactor;

            if (product.IsReadyToTest)
            {
                product.Stage = ProductStage.Testing;
            }
        }

        /// Retorna quantos bugs (já presentes em BugCount) a rodada de testes revelou/confirmou.
        public int TestForBugs(ProductState product, int cycles)
        {
            if (product.Stage != ProductStage.Testing && product.Stage != ProductStage.Maintenance) return 0;
            if (cycles <= 0) return 0;
            var found = (int)Math.Round(product.BugCount * (1 - Math.Pow(1 - _config.TestEfficiencyPerCycle, cycles)));
            return Math.Min(found, product.BugCount);
        }

        public void FixBugs(ProductState product, int cycles)
        {
            if (cycles <= 0) return;
            var toFix = (int)Math.Min(product.BugCount, _config.FixPointsPerCycle * cycles);
            if (toFix <= 0) return;
            product.BugCount -= toFix;
            product.Stability = Math.Clamp(product.Stability + toFix * 0.01, 0, 1);
            _eventBus?.Publish(new BugFixedEvent(product.Id, toFix));
        }

        public void Launch(ProductState product)
        {
            if (product.Stage == ProductStage.Launched) return;
            var penalty = product.BugCount * _config.LaunchReputationPenaltyPerOutstandingBug;
            product.Reputation = Math.Clamp(product.Reputation - penalty, 0, 1);
            product.Stage = ProductStage.Launched;
            _eventBus?.Publish(new ProductLaunchedEvent(product.Id));
        }
    }
}

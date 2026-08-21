using System;
using System.Collections.Generic;

namespace StartupEmpire.Competitors
{
    /// Simulação simples de mercado: concorrentes crescem por uma taxa fixa por ciclo
    /// (sem IA pesada, conforme a missão pede) e a participação de mercado é recalculada
    /// comparando usuários do jogador contra a soma de usuários de todos os concorrentes.
    public sealed class CompetitorService
    {
        private readonly CompetitorConfigValues _config;

        public CompetitorService(CompetitorConfigValues config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void RunCycle(List<CompetitorState> competitors, int cycles)
        {
            if (cycles <= 0) return;
            foreach (var competitor in competitors)
            {
                competitor.Users *= Math.Pow(1 + _config.UserGrowthRatePerCycle, cycles);
                competitor.Valuation *= Math.Pow(1 + _config.ValuationGrowthRatePerCycle, cycles);
            }
        }

        public void RecomputeMarketShare(List<CompetitorState> competitors, double playerUsers)
        {
            double totalMarket = playerUsers;
            foreach (var competitor in competitors) totalMarket += competitor.Users;
            if (totalMarket <= 0) return;

            foreach (var competitor in competitors)
            {
                competitor.MarketShare = competitor.Users / totalMarket;
            }
        }

        public double GetPlayerMarketShare(List<CompetitorState> competitors, double playerUsers)
        {
            double totalMarket = playerUsers;
            foreach (var competitor in competitors) totalMarket += competitor.Users;
            return totalMarket <= 0 ? 0 : playerUsers / totalMarket;
        }
    }
}

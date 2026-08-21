using System.Collections.Generic;
using StartupEmpire.Progression;

namespace StartupEmpire.Investment
{
    public static class InvestmentCatalog
    {
        public static List<InvestmentOffer> CreateDefaultCatalog()
        {
            return new List<InvestmentOffer>
            {
                new InvestmentOffer(InvestmentRoundType.Angel, "Investidor-Anjo",
                    cashAmount: 5000, equityPercentRequested: 0.10,
                    minValuationRequired: 0, minStageRequired: CompanyStage.Freelancer),

                new InvestmentOffer(InvestmentRoundType.Seed, "Seed",
                    cashAmount: 25000, equityPercentRequested: 0.12,
                    minValuationRequired: 20000, minStageRequired: CompanyStage.Microempresa),

                new InvestmentOffer(InvestmentRoundType.SeriesA, "Series A",
                    cashAmount: 150000, equityPercentRequested: 0.15,
                    minValuationRequired: 150000, minStageRequired: CompanyStage.Startup),

                new InvestmentOffer(InvestmentRoundType.SeriesB, "Series B",
                    cashAmount: 800000, equityPercentRequested: 0.12,
                    minValuationRequired: 1_000_000, minStageRequired: CompanyStage.Startup),

                new InvestmentOffer(InvestmentRoundType.SeriesC, "Series C",
                    cashAmount: 3_000_000, equityPercentRequested: 0.10,
                    minValuationRequired: 5_000_000, minStageRequired: CompanyStage.Empresa),
            };
        }
    }
}

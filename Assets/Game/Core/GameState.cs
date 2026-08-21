using System;
using System.Collections.Generic;
using StartupEmpire.Competitors;
using StartupEmpire.Economy;
using StartupEmpire.Employees;
using StartupEmpire.Investment;
using StartupEmpire.Missions;
using StartupEmpire.Products;
using StartupEmpire.Progression;
using StartupEmpire.Upgrades;

namespace StartupEmpire.Core
{
    /// Raiz de agregação do estado de domínio. Não conhece UI nem UnityEngine.
    public sealed class GameState
    {
        public PlayerState Player { get; }
        public EconomyState Economy { get; }
        public List<ProductState> Products { get; } = new();
        public CompanyStage Stage { get; internal set; } = CompanyStage.PessoaFisica;
        public MissionProgressBook Missions { get; } = new();
        public HashSet<string> UnlockedAchievements { get; } = new();
        public UpgradeState Upgrades { get; } = new();
        public EmployeeRoster Employees { get; } = new();
        public List<CompetitorState> Competitors { get; } = new();
        public HashSet<InvestmentRoundType> RaisedInvestmentRounds { get; } = new();
        public DateTime LastSavedUtc { get; set; }

        public GameState(PlayerState player, EconomyState economy)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Economy = economy ?? throw new ArgumentNullException(nameof(economy));
        }

        public ProductState FindProduct(string productId) => Products.Find(p => p.Id == productId);
    }
}

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;

namespace StartupEmpire.UI.Screens
{
    public sealed class StatisticsScreenPanel : IScreenPanel
    {
        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "StatisticsPanel");
            _text = UiFactory.CreateText(root.transform, "", 26, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.97f), "StatisticsText");
            screenManager.Register("Statistics", root);
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var snapshot = GameRoot.Instance.GetStatistics();
            var sb = new StringBuilder();
            sb.AppendLine("ESTATÍSTICAS\n");
            sb.AppendLine($"Caixa: R$ {snapshot.Cash:F2}");
            sb.AppendLine($"Valuation: R$ {snapshot.Valuation:F2}");
            sb.AppendLine($"MRR: R$ {snapshot.MonthlyRecurringRevenue:F2}");
            sb.AppendLine($"Equity do Fundador: {snapshot.FounderEquity:P1}");
            sb.AppendLine($"Usuários totais: {snapshot.TotalUsers}   Pagantes: {snapshot.TotalPayingCustomers}");
            sb.AppendLine($"Produtos: {snapshot.ProductCount} ({snapshot.LaunchedProductCount} lançados)");
            sb.AppendLine($"Funcionários: {snapshot.EmployeeCount}");
            sb.AppendLine($"Níveis de upgrade comprados: {snapshot.UpgradesPurchasedLevels}");
            sb.AppendLine($"Conquistas: {snapshot.UnlockedAchievementCount}   Missões: {snapshot.CompletedMissionCount}");
            sb.AppendLine($"Gems: {snapshot.GemBalance}   Rodadas captadas: {snapshot.RaisedInvestmentRoundCount}");
            sb.AppendLine($"Participação de mercado: {snapshot.PlayerMarketShare:P1}");
            sb.AppendLine($"Estágio: {snapshot.CompanyStage}");
            _text.text = sb.ToString();
        }
    }
}

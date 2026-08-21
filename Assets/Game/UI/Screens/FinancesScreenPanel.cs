using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;
using StartupEmpire.Investment;

namespace StartupEmpire.UI.Screens
{
    public sealed class FinancesScreenPanel : IScreenPanel
    {
        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "FinancesPanel");

            _text = UiFactory.CreateText(root.transform, "", 26, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.20f), new Vector2(0.97f, 0.97f), "FinancesText");

            UiFactory.CreateButton(root.transform, "Aceitar Investidor-Anjo", new Vector2(0.03f, 0.11f), new Vector2(0.97f, 0.18f),
                () => { GameRoot.Instance.AcceptInvestmentOffer(InvestmentRoundType.Angel); Refresh(); });

            screenManager.Register("Finances", root);
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var economy = GameRoot.Instance.State.Economy;
            var sb = new StringBuilder();
            sb.AppendLine("FINANÇAS\n");
            sb.AppendLine($"Caixa: R$ {economy.Cash:F2}");
            sb.AppendLine($"MRR: R$ {economy.MonthlyRecurringRevenue:F2}");
            sb.AppendLine($"Valuation: R$ {economy.Valuation:F2}");
            sb.AppendLine($"Equity do Fundador: {economy.FounderEquity:P1}");
            sb.AppendLine();

            var rounds = GameRoot.Instance.State.RaisedInvestmentRounds;
            sb.AppendLine(rounds.Count == 0
                ? "Nenhuma rodada de investimento captada ainda."
                : $"Rodadas captadas: {string.Join(", ", rounds)}");

            _text.text = sb.ToString();
        }
    }
}

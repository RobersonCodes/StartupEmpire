using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;

namespace StartupEmpire.UI.Screens
{
    public sealed class CompanyScreenPanel : IScreenPanel
    {
        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "CompanyPanel");
            _text = UiFactory.CreateText(root.transform, "", 24, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.97f), "CompanyText");
            screenManager.Register("Company", root);
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var state = GameRoot.Instance.State;
            var sb = new StringBuilder();
            sb.AppendLine("EMPRESA\n");
            sb.AppendLine($"Estágio: {state.Stage}");
            sb.AppendLine($"Equity do Fundador: {state.Economy.FounderEquity:P1}");
            sb.AppendLine(state.RaisedInvestmentRounds.Count == 0
                ? "Rodadas captadas: nenhuma"
                : $"Rodadas captadas: {string.Join(", ", state.RaisedInvestmentRounds)}");

            sb.AppendLine("\nCONCORRENTES\n");
            foreach (var competitor in state.Competitors)
            {
                sb.AppendLine($"{competitor.Definition.DisplayName}");
                sb.AppendLine($"  Usuários: {competitor.Users:F0}   Valuation: R$ {competitor.Valuation:F0}   Participação de mercado: {competitor.MarketShare:P1}\n");
            }

            _text.text = sb.ToString();
        }
    }
}

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;

namespace StartupEmpire.UI.Screens
{
    public sealed class UpgradesScreenPanel : IScreenPanel
    {
        private static readonly string[] UpgradeIds =
        {
            "better_computer", "better_internet", "productivity_tools", "online_courses"
        };

        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "UpgradesPanel");

            _text = UiFactory.CreateText(root.transform, "", 26, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.40f), new Vector2(0.97f, 0.97f), "UpgradesText");

            var y = 0.32f;
            foreach (var upgradeId in UpgradeIds)
            {
                var capturedId = upgradeId;
                UiFactory.CreateButton(root.transform, $"Comprar {upgradeId}", new Vector2(0.03f, y), new Vector2(0.97f, y + 0.06f),
                    () => { GameRoot.Instance.PurchaseUpgrade(capturedId); Refresh(); });
                y -= 0.08f;
            }

            screenManager.Register("Upgrades", root);
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("UPGRADES\n");

            foreach (var definition in GameRoot.Instance.Upgrades.Definitions)
            {
                var level = GameRoot.Instance.State.Upgrades.GetLevel(definition.Id);
                var cost = GameRoot.Instance.Upgrades.GetCostForNextLevel(definition, GameRoot.Instance.State.Upgrades);
                sb.AppendLine($"{definition.DisplayName} — nível {level}/{definition.MaxLevel}   próximo custo: R$ {cost:F0}");
            }

            _text.text = sb.ToString();
        }
    }
}

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;

namespace StartupEmpire.UI.Screens
{
    public sealed class StoreScreenPanel : IScreenPanel
    {
        private static readonly string[] ItemIds =
        {
            "dev_boost_small", "marketing_boost_small", "cash_injection", "cosmetic_dark_theme"
        };

        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "StorePanel");

            _text = UiFactory.CreateText(root.transform, "", 26, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.40f), new Vector2(0.97f, 0.97f), "StoreText");

            var y = 0.32f;
            foreach (var itemId in ItemIds)
            {
                var capturedId = itemId;
                UiFactory.CreateButton(root.transform, $"Comprar {itemId}", new Vector2(0.03f, y), new Vector2(0.97f, y + 0.06f),
                    () => { GameRoot.Instance.PurchaseStoreItem(capturedId); Refresh(); });
                y -= 0.08f;
            }

            screenManager.Register("Store", root);
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine($"LOJA — Gems: {GameRoot.Instance.State.GemWallet.Balance}\n");

            foreach (var item in GameRoot.Instance.Store.Catalog)
            {
                sb.AppendLine($"{item.DisplayName} ({item.Category}) — {item.GemCost} gems");
            }

            _text.text = sb.ToString();
        }
    }
}

using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Audio;

namespace StartupEmpire.UI.Screens
{
    public sealed class SettingsScreenPanel : IScreenPanel
    {
        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "SettingsPanel");

            _text = UiFactory.CreateText(root.transform, "", 26, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.55f), new Vector2(0.97f, 0.97f), "SettingsText");

            var y = 0.46f;
            foreach (AudioCategory category in Enum.GetValues(typeof(AudioCategory)))
            {
                var capturedCategory = category;
                UiFactory.CreateButton(root.transform, $"{category} -10%", new Vector2(0.03f, y), new Vector2(0.48f, y + 0.06f),
                    () => AdjustVolume(capturedCategory, -0.1f));
                UiFactory.CreateButton(root.transform, $"{category} +10%", new Vector2(0.52f, y), new Vector2(0.97f, y + 0.06f),
                    () => AdjustVolume(capturedCategory, 0.1f));
                y -= 0.08f;
            }

            screenManager.Register("Settings", root);
        }

        private void AdjustVolume(AudioCategory category, float delta)
        {
            if (AudioManager.Instance == null) return;
            var current = AudioManager.Instance.MixState.GetVolume(category);
            AudioManager.Instance.SetVolume(category, current + delta);
            Refresh();
        }

        public void Refresh()
        {
            var sb = new StringBuilder();
            sb.AppendLine("CONFIGURAÇÕES DE ÁUDIO\n");

            if (AudioManager.Instance == null)
            {
                sb.AppendLine("(AudioManager não está presente na cena)");
            }
            else
            {
                foreach (AudioCategory category in Enum.GetValues(typeof(AudioCategory)))
                {
                    sb.AppendLine($"{category}: {AudioManager.Instance.MixState.GetVolume(category):P0}");
                }
            }

            _text.text = sb.ToString();
        }
    }
}

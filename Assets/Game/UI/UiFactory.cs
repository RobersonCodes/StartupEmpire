using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace StartupEmpire.UI
{
    /// Helpers de construção de UI em runtime, compartilhados por todas as telas —
    /// evita repetir a mesma configuração de RectTransform/Image/Text em cada uma.
    /// Visual propositalmente simples (cores sólidas, fonte padrão) — placeholder
    /// funcional até existir direção de arte própria (seção 27 da missão).
    public static class UiFactory
    {
        public static readonly Color PanelBackground = new(0.07f, 0.09f, 0.13f, 1f);
        public static readonly Color ButtonColor = new(0.15f, 0.45f, 0.75f, 1f);
        public static readonly Color NavBarColor = new(0.04f, 0.05f, 0.08f, 1f);

        public static GameObject CreatePanel(Transform parent, Color color, string name = "Panel")
        {
            var go = new GameObject(name, typeof(Image));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = color;
            Stretch(go.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
            return go;
        }

        public static Text CreateText(Transform parent, string content, int fontSize, TextAnchor alignment,
            Vector2 anchorMin, Vector2 anchorMax, string name = "Text")
        {
            var go = new GameObject(name, typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = alignment;
            text.text = content;
            Stretch(go.GetComponent<RectTransform>(), anchorMin, anchorMax);
            return text;
        }

        public static Button CreateButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax,
            UnityAction onClick)
        {
            var go = new GameObject($"Button_{label}", typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = ButtonColor;
            Stretch(go.GetComponent<RectTransform>(), anchorMin, anchorMax);

            var button = go.GetComponent<Button>();
            if (onClick != null) button.onClick.AddListener(onClick);

            CreateText(go.transform, label, 22, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, "Label");
            return button;
        }

        public static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}

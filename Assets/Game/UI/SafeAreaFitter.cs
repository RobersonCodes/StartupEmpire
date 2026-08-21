using UnityEngine;

namespace StartupEmpire.UI
{
    /// Mantém um RectTransform dentro da área utilizável do aparelho (notch,
    /// cantos arredondados e barra de gestos). Só recalcula quando a tela muda.
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            ApplyCurrentSafeArea();
        }

        private void Update()
        {
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (Screen.safeArea == _lastSafeArea && screenSize == _lastScreenSize) return;
            ApplyCurrentSafeArea();
        }

        private void ApplyCurrentSafeArea()
        {
            _lastSafeArea = Screen.safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            CalculateAnchors(_lastSafeArea, _lastScreenSize, out var anchorMin, out var anchorMax);
            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

        public static void CalculateAnchors(Rect safeArea, Vector2Int screenSize,
            out Vector2 anchorMin, out Vector2 anchorMax)
        {
            if (screenSize.x <= 0 || screenSize.y <= 0)
            {
                anchorMin = Vector2.zero;
                anchorMax = Vector2.one;
                return;
            }

            anchorMin = new Vector2(
                Mathf.Clamp01(safeArea.xMin / screenSize.x),
                Mathf.Clamp01(safeArea.yMin / screenSize.y));
            anchorMax = new Vector2(
                Mathf.Clamp01(safeArea.xMax / screenSize.x),
                Mathf.Clamp01(safeArea.yMax / screenSize.y));
            anchorMax = Vector2.Max(anchorMax, anchorMin);
        }
    }
}

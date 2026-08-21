using System.Collections.Generic;
using UnityEngine;

namespace StartupEmpire.UI
{
    /// Mostra uma tela por vez dentro da área de conteúdo compartilhada.
    public sealed class ScreenManager : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> _screens = new();
        private string _activeScreenId;

        public void Register(string screenId, GameObject screenRoot)
        {
            _screens[screenId] = screenRoot;
            screenRoot.SetActive(false);
        }

        public void Show(string screenId)
        {
            if (!_screens.TryGetValue(screenId, out var root)) return;

            if (_activeScreenId != null && _screens.TryGetValue(_activeScreenId, out var previous))
            {
                previous.SetActive(false);
            }

            root.SetActive(true);
            _activeScreenId = screenId;
        }

        public string ActiveScreenId => _activeScreenId;
    }
}

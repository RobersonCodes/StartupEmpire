using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;
using StartupEmpire.Events;

namespace StartupEmpire.UI
{
    /// Modal de evento (seção 14 da missão): aparece por cima de qualquer tela
    /// quando GameRoot.PendingEvent existe, com um botão por escolha. Não é uma
    /// tela de navegação como as outras — é sempre um overlay, chamado via Tick()
    /// a cada frame pelo GameShellBuilder.
    public sealed class EventModalBuilder
    {
        private GameObject _root;
        private Text _titleText;
        private Text _descriptionText;
        private Transform _choicesParent;
        private readonly List<GameObject> _choiceButtons = new();
        private string _shownForEventId;

        public void Build(Transform canvasParent)
        {
            _root = UiFactory.CreatePanel(canvasParent, new Color(0f, 0f, 0f, 0.85f), "EventModal");
            _root.transform.SetAsLastSibling();

            var box = UiFactory.CreatePanel(_root.transform, UiFactory.PanelBackground, "EventModalBox");
            UiFactory.Stretch(box.GetComponent<RectTransform>(), new Vector2(0.08f, 0.30f), new Vector2(0.92f, 0.70f));

            _titleText = UiFactory.CreateText(box.transform, "", 30, TextAnchor.UpperCenter,
                new Vector2(0.05f, 0.78f), new Vector2(0.95f, 0.95f), "EventTitle");
            _descriptionText = UiFactory.CreateText(box.transform, "", 22, TextAnchor.UpperLeft,
                new Vector2(0.05f, 0.45f), new Vector2(0.95f, 0.76f), "EventDescription");

            var choicesGo = new GameObject("Choices", typeof(RectTransform));
            choicesGo.transform.SetParent(box.transform, false);
            UiFactory.Stretch(choicesGo.GetComponent<RectTransform>(), new Vector2(0.05f, 0.03f), new Vector2(0.95f, 0.42f));
            _choicesParent = choicesGo.transform;

            _root.SetActive(false);
        }

        public void Tick()
        {
            if (GameRoot.Instance == null) return;
            var pending = GameRoot.Instance.PendingEvent;

            if (pending == null)
            {
                if (_root.activeSelf) _root.SetActive(false);
                _shownForEventId = null;
                return;
            }

            if (_shownForEventId == pending.Id) return;

            ShowEvent(pending);
        }

        private void ShowEvent(GameEventDefinition definition)
        {
            _shownForEventId = definition.Id;
            _titleText.text = definition.Title;
            _descriptionText.text = definition.Description;

            foreach (var button in _choiceButtons) Object.Destroy(button);
            _choiceButtons.Clear();

            var count = definition.Choices.Count;
            var height = count > 0 ? 1f / count : 1f;
            for (var i = 0; i < count; i++)
            {
                var choice = definition.Choices[i];
                var minY = 1f - (i + 1) * height;
                var maxY = 1f - i * height;
                var button = UiFactory.CreateButton(_choicesParent, choice.Label,
                    new Vector2(0.02f, minY + 0.01f), new Vector2(0.98f, maxY - 0.01f),
                    () => GameRoot.Instance.ResolveEvent(choice.Id));
                _choiceButtons.Add(button.gameObject);
            }

            _root.SetActive(true);
        }
    }
}

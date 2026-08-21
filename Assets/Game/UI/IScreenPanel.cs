using UnityEngine;

namespace StartupEmpire.UI
{
    /// Cada tela do jogo implementa isso. Build monta a hierarquia visual uma vez;
    /// Refresh só releitura o estado atual do GameRoot e atualiza os textos/estado
    /// visual — nunca o contrário (a UI nunca é fonte de verdade do estado do jogo).
    public interface IScreenPanel
    {
        void Build(Transform contentParent, ScreenManager screenManager);
        void Refresh();
    }
}

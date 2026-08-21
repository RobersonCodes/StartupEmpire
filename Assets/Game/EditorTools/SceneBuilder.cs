using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using StartupEmpire.Audio;
using StartupEmpire.Core;
using StartupEmpire.UI;

namespace StartupEmpire.EditorTools
{
    /// Gera cenas via código em vez de exigir montagem manual no Editor — a cena
    /// em si fica minúscula (um GameObject com GameRoot + o builder de UI), e toda
    /// a hierarquia visual é montada em runtime por OfficeScreenBuilder. Rodável
    /// também via CLI: `Unity.exe -batchmode -executeMethod
    /// StartupEmpire.EditorTools.SceneBuilder.BuildOfficeScene -quit`.
    public static class SceneBuilder
    {
        private const string ScenesFolder = "Assets/Game/UI/Scenes";
        private const string OfficeScenePath = ScenesFolder + "/Office.unity";

        [MenuItem("StartupEmpire/Build Office Scene")]
        public static void BuildOfficeScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            new GameObject("GameRoot", typeof(GameRoot), typeof(AudioManager), typeof(GameShellBuilder));

            if (!AssetDatabase.IsValidFolder(ScenesFolder))
            {
                AssetDatabase.CreateFolder("Assets/Game/UI", "Scenes");
            }

            EditorSceneManager.SaveScene(scene, OfficeScenePath);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(OfficeScenePath, true)
            };

            Debug.Log($"[SceneBuilder] Office scene salva em {OfficeScenePath} e definida como cena 0 do build.");
        }
    }
}

using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace StartupEmpire.EditorTools
{
    /// Gera um APK de desenvolvimento de verdade. Rodável via CLI:
    /// `Unity.exe -batchmode -executeMethod StartupEmpire.EditorTools.AndroidBuilder.BuildDebugApk -quit`.
    /// Não afirma "build funcionando" sem isso ter rodado de verdade (seção 35 da missão) —
    /// o resultado (sucesso/erro, tamanho, tempo) sempre é logado a partir do BuildReport real.
    public static class AndroidBuilder
    {
        private const string OutputPath = "Builds/Android/StartupEmpire-debug.apk";
        private const string AppIconPath = "Assets/Game/Art/StartupEmpireAppIcon.png";

        [MenuItem("StartupEmpire/Build Android APK (Debug)")]
        public static void BuildDebugApk()
        {
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.startupempire.game");
            ConfigureAndroidIcons();

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Game/UI/Scenes/Office.unity" },
                locationPathName = OutputPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            Debug.Log(
                $"[AndroidBuilder] result={summary.result} " +
                $"totalErrors={summary.totalErrors} totalWarnings={summary.totalWarnings} " +
                $"sizeBytes={summary.totalSize} timeSeconds={summary.totalTime.TotalSeconds:F1} " +
                $"outputPath={summary.outputPath}");
        }

        private static void ConfigureAndroidIcons()
        {
            var iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AppIconPath);
            if (iconTexture == null)
            {
                throw new System.InvalidOperationException($"App icon not found at {AppIconPath}");
            }

            SetSingleLayerIcons(AndroidPlatformIconKind.Legacy, iconTexture);
            SetSingleLayerIcons(AndroidPlatformIconKind.Round, iconTexture);
            Debug.Log($"[AndroidBuilder] configured app icon from {AppIconPath}");
        }

        private static void SetSingleLayerIcons(PlatformIconKind kind, Texture2D texture)
        {
            var icons = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, kind);
            foreach (var icon in icons)
            {
                icon.SetTexture(texture, 0);
            }
            PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, kind, icons);
        }
    }
}

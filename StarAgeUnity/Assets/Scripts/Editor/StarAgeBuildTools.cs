using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace StarAgeReborn.Unity.Editor
{
    public static class StarAgeBuildTools
    {
        const string PlayScenePath = "Assets/StarAgePlayScene.unity";
        static readonly string[] Scenes = { PlayScenePath };

        [MenuItem("Star Age/Build/Windows x64")]
        public static void BuildWindows()
        {
            Directory.CreateDirectory("Builds/Windows");
            BuildPipeline.BuildPlayer(Scenes, "Builds/Windows/StarAgeReborn.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        }

        [MenuItem("Star Age/Build/WebGL")]
        public static void BuildWebGl()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            PlayerSettings.stripEngineCode = false;
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.Disabled);
            Directory.CreateDirectory("Builds/WebGL");
            BuildReport report = BuildPipeline.BuildPlayer(Scenes, "Builds/WebGL", BuildTarget.WebGL, BuildOptions.None);
            UnityEngine.Debug.Log($"Star Age WebGL build result: {report.summary.result}, errors: {report.summary.totalErrors}, warnings: {report.summary.totalWarnings}");
            if (report.summary.result != BuildResult.Succeeded)
            {
                EditorApplication.Exit(1);
            }
        }
    }
}

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace StarAgeReborn.Unity.Editor
{
    public static class StarAgeSceneTools
    {
        const string PlayScenePath = "Assets/StarAgePlayScene.unity";

        [MenuItem("Star Age/Create Play Scene")]
        public static void CreatePlayScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 260f;
            camera.backgroundColor = new Color(0.01f, 0.012f, 0.04f);
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            EditorSceneManager.SaveScene(scene, PlayScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(PlayScenePath, true)
            };
        }
    }
}

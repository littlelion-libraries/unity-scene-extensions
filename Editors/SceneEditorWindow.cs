#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneEditorWindow : EditorWindow
{
    // private HashSet<string> _buildScenes;
    // private string[] _scenes;
    private Vector2 _scrollPosition;
    private static IEnumerable<string> FindScenes()
    {
        return AssetDatabase.FindAssets("t:Scene").Select(AssetDatabase.GUIDToAssetPath);
    }

    [MenuItem("Window/Scene")]
    private static void Init()
    {
        var window = GetWindow<SceneEditorWindow>();
        window.position = new Rect(50f, 50f, 200f, 24f);
        window.Show();
    }

    // private void OnEnable()
    // {
    //     _buildScenes = EditorBuildSettings.scenes.Select(it => it.path).ToHashSet();
    //     _scenes = FindScenes().ToArray();
    // }

    private void OnGUI()
    {
        var dirty = false;
        var scenes = FindScenes().ToHashSet();
        var buildScenes = EditorBuildSettings.scenes.Where(it =>
        {
            if (scenes.Contains(it.path)) return true;
            dirty = true;
            return false;
        }).Select(it => it.path).ToHashSet();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        foreach (var scene in scenes)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(scene);
            if (GUILayout.Button("Open"))
            {
                LoadScene(scene);
            }

            var contain = buildScenes.Contains(scene);

            if (GUILayout.Toggle(contain, ""))
            {
                if (!contain)
                {
                    buildScenes.Add(scene);
                    dirty = true;
                }
            }
            else
            {
                if (contain)
                {
                    buildScenes.Remove(scene);
                    dirty = true;
                }
            }

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        if (dirty)
        {
            EditorBuildSettings.scenes = buildScenes.Select(it => new EditorBuildSettingsScene(it, true)).ToArray();
        }
    }

    private static void LoadScene(string scenePath)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(scenePath);
    }
}
#endif
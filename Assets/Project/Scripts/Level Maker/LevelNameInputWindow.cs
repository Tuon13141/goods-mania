using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelNameInputWindow : EditorWindow
{
    private string levelName = "NewLevel";

    public System.Action<string> onSubmit;

    public static void ShowWindow(System.Action<string> onSubmit)
    {
        var window = ScriptableObject.CreateInstance<LevelNameInputWindow>();
        window.titleContent = new GUIContent("Enter Level Name");
        window.onSubmit = onSubmit;
        window.ShowUtility(); // Show as popup
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter level name to save:", EditorStyles.boldLabel);
        levelName = EditorGUILayout.TextField("Level Name", levelName);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            if (!string.IsNullOrEmpty(levelName))
            {
                onSubmit?.Invoke(levelName);
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid Name", "Level name cannot be empty.", "OK");
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }

        GUILayout.EndHorizontal();
    }
}

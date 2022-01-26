using UnityEditor;
using UnityEngine;

public static class MenuItems
{
    [MenuItem("ABTool/View/Download")]
    private static void ViewDownload()
    {
        EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
    }
}

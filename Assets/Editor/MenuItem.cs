using UnityEditor;
using UnityEngine;

public static class MenuItems
{
    [MenuItem("MonitorTool/Download")]
    private static void ViewDownload()
    {
        EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
    }
}

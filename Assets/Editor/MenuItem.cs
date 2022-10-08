using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class MenuItems
{
    [MenuItem("MonitorTool/Download")]
    private static void ViewDownload()
    {
        EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
    }

    [MenuItem("MonitorTool/º¯Êý¹æ·¶ÐÔ¼ì²â")]
    private static void ProcessExcuteFunctionCheck()
    {
        var batPath = Application.dataPath.Replace("/Assets", "") + "/Tools/net6.0/run.bat";
        MonitorLib.GOT.Tools.BatRunner(batPath,"");
    }
}

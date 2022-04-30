using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

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
        var bat = Application.dataPath.Replace("/Assets", "") + "/Tools/net6.0/run.bat";
        UnityEngine.Debug.Log(bat);
        ProcessStartInfo proc = new ProcessStartInfo("cmd.exe", bat);
        proc.WindowStyle = ProcessWindowStyle.Normal;
        //proc.CreateNoWindow = false;
        //proc.UseShellExecute = true;
        //proc.RedirectStandardError = false;
        //proc.RedirectStandardInput = false;
        //proc.RedirectStandardOutput = false;
        Process.Start(proc);
    }
}

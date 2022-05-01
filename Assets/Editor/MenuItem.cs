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

    [MenuItem("MonitorTool/函数规范性检测")]
    private static void ProcessExcuteFunctionCheck()
    {
        var batPath = Application.dataPath.Replace("/Assets", "") + "/Tools/net6.0/run.bat";
        RunBat(batPath,"");
    }
    
    /// <summary>
    /// 调用Bat
    /// </summary>
    /// <param name="batPath"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    private static bool RunBat(string batPath, string arg)
    {
        // System.Console.InputEncoding = System.Text.Encoding.UTF8;
        using (Process proc = new Process())
        {
            proc.StartInfo.Verb = "call";
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(batPath);
            proc.StartInfo.FileName = batPath;
            if (!string.IsNullOrEmpty(arg))
            {
                proc.StartInfo.Arguments = arg;
            }
            proc.StartInfo.UseShellExecute = false;

            // set up output redirection
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.CreateNoWindow = true;

            StringBuilder sbError = new StringBuilder();
            StringBuilder sbNormal = new StringBuilder();
            
            // Set the data received handlers
            proc.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    sbError.Append(e.Data + Environment.NewLine);
                }
            };
            proc.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    sbNormal.Append(e.Data + Environment.NewLine);
                }
            };

            proc.Start();
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();
            proc.WaitForExit();

            bool isError = sbError.ToString().Contains("error");
            if (proc.ExitCode == 0 && !isError)
            {
                Debug.Log($"Success. {sbNormal}");
                return true;
            }
            else
            {
                Debug.Log($"Failed. {sbError}");
                return false;
            }
        }
    }
}

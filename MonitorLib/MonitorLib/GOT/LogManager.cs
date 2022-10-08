using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace MonitorLib.GOT
{
    public class LogManager
    {
        static FileStream logFileStream;
        static string logFilePath = null;
        static float lastInfoLogWriteTime;

        public static void CreateLogFile(string path, FileMode fileMode = FileMode.Append)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("创建的Log路径不存在");
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
            }
            logFileStream = new FileStream(path, fileMode);
            logFilePath = path;
        }

        public static void CloseLogFile()
        {
            if (logFileStream != null)
            {
                logFilePath = null;
                logFileStream.Flush();
                logFileStream.Close();
                logFileStream.Dispose();
                logFileStream = null;
            }
        }

        public static void Log(string msg)
        {
            if (ShareDatas.ShowDebugLog)
                Debug.Log(msg);
            if (ShareDatas.WriteLogToFile)
                LogToFile(LogType.Log, $"<font color=\"#0000FF\">[Log]</font>{msg}", true);
        }

        public static void LogError(string msg)
        {
            if (ShareDatas.ShowDebugLog)
                Debug.LogError(msg);
            if (ShareDatas.WriteLogToFile)
                LogToFile(LogType.Error, $"<font color=\"#FF0000\">[Error]</font>{msg}", true);
        }

        public static void LogWarning(string msg)
        {
            if (ShareDatas.ShowDebugLog)
                Debug.LogWarning(msg);
            if (ShareDatas.WriteLogToFile)
                LogToFile(LogType.Warning, $"<font color=\"#FFD700\">[Warning]</font>{msg}", true);
        }

        public static void LogAssert(string msg)
        {
            if (ShareDatas.ShowDebugLog)
                Debug.LogAssertion(msg);
            if (ShareDatas.WriteLogToFile)
                LogToFile(LogType.Assert, $"<font color=\"#FF0000\">[Assert]{msg}</font>", true);
        }

        public static void LogException(Exception ex)
        {
            if (ShareDatas.ShowDebugLog)
                Debug.LogException(ex);
            if (ShareDatas.WriteLogToFile)
                LogToFile(LogType.Exception, $"<font color=\"#FF0000\">[Exception]</font>{ex.ToString()}", true);
        }

        public static void LogToFile(string logString, string stackTrace, LogType type)
        {
            LogToFile(type, logString, true, stackTrace);
        }

        static string ColorTypeLog(LogType type, string msg)
        {
            string log = "";
            switch (type)
            {
                case LogType.Log:
                    log = $"<font color=\"#0000FF\">[Log]{msg.TrimEnd()}</font>";
                    break;
                case LogType.Error:
                    log = $"<font color=\"#FF0000\">[Error]{msg.TrimEnd()}</font>";
                    break;
                case LogType.Warning:
                    log = $"<font color=\"#FFD700\">[Warning]{msg.TrimEnd()}</font>";
                    break;
                case LogType.Assert:
                    log = $"<font color=\"#FF0000\">[Assert]{msg.TrimEnd()}</font>";
                    break;
                case LogType.Exception:
                    log = $"<font color=\"#FF0000\">[Assert]{msg.TrimEnd()}</font>";
                    break;
            }
            return log;
        }

        /// <summary>
        /// 写入文件的操作
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="msg"></param>
        /// <param name="isForceWrite"></param>
        public static void LogToFile(LogType logType, string msg, bool isForece = false, string stackTrace = null)
        {
            bool isForceWrite = true;
            try
            {
                if (!msg.EndsWith("\n"))
                {
                    msg += "\n";
                }
                var logStr = $"[{DateTime.Now.ToString()}]{ColorTypeLog(logType, msg)} \n\rstackTrace:{stackTrace}";
                byte[] data = Encoding.Default.GetBytes(logStr);

                if (logFileStream == null)
                {
                    if (string.IsNullOrEmpty(logFilePath))
                    {
                        if (string.IsNullOrEmpty(ShareDatas.StartTimeStr))
                            ShareDatas.StartTimeStr = DateTime.Now.ToString().Replace(" ", "_").Replace("/", "_").Replace(":", "_");
                        logFilePath = Application.persistentDataPath + "/" + $"log_{ShareDatas.StartTime}.txt";
                    }
                    logFileStream = new FileStream(logFilePath, FileMode.Append);
                }

                logFileStream.Write(data, 0, data.Length);
                if (isForceWrite || Time.time - lastInfoLogWriteTime > 1)
                {
                    logFileStream.Flush();
                    lastInfoLogWriteTime = Time.time;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
    }
}

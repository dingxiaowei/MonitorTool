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
                LogToFile(LogType.Log, $"[Log]{msg}", true);
        }

        public static void LogError(string msg)
        {
            if (ShareDatas.ShowDebugLog)
                Debug.LogError(msg);
            if (ShareDatas.WriteLogToFile)
                LogToFile(LogType.Error, $"[Error]{msg}", true);
        }

        public static void LogWarning(string msg)
        {
            if (ShareDatas.ShowDebugLog)
                Debug.LogWarning(msg);
            if (ShareDatas.WriteLogToFile)
                LogToFile(LogType.Warning, $"[Warning]{msg}", true);
        }

        public static void LogAssert(string msg)
        {
            if (ShareDatas.ShowDebugLog)
                Debug.LogAssertion(msg);
            if (ShareDatas.WriteLogToFile)
                LogToFile(LogType.Assert, $"[Assert]{msg}", true);
        }

        public static void LogException(Exception ex)
        {
            if (ShareDatas.ShowDebugLog)
                Debug.LogException(ex);
            if (ShareDatas.WriteLogToFile)
                LogToFile(LogType.Exception, $"[Exception]{ex.ToString()}", true);
        }

        public static void LogToFile(string logString, string stackTrace, LogType type)
        {
            LogToFile(type, logString, true, stackTrace);
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
                byte[] data = Encoding.Default.GetBytes(($"[{logType.ToString()}]msg:{msg} stackTrace:{stackTrace}").ToString());

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

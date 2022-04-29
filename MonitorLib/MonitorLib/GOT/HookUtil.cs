using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace MonitorLib.GOT
{
    public static class HookUtil
    {
        static Thread mainThread = Thread.CurrentThread;
        static Dictionary<string, FuncData> FunctionDatas = new Dictionary<string, FuncData>();
        public static void Begin(string methodName)
        {
            //一些方法不能再Unity主线程调用
            if (Thread.CurrentThread == mainThread)
            {
                long tmpMemory = Profiler.GetTotalAllocatedMemoryLong();
                float tmpTime = Time.realtimeSinceStartup;
                if (FunctionDatas.ContainsKey(methodName))
                {
                    var tmp = FunctionDatas[methodName];
                    tmp.BeginMemory = tmpMemory;
                    tmp.BeginTime = tmpTime;
                    FunctionDatas[methodName] = tmp;
                }
                else
                {
                    var tmp = new FuncData();
                    tmp.FuncName = methodName;
                    tmp.FuncMemory = 0L;
                    tmp.FuncTime = 0f;
                    tmp.FuncCalls = 0;
                    tmp.FuncTotalMemory = 0L;
                    tmp.FuncTotalTime = 0f;
                    tmp.BeginMemory = tmpMemory;
                    tmp.BeginTime = tmpTime;
                    FunctionDatas.Add(methodName, tmp);
                }
            }
        }

        public static void End(string methodName)
        {
            if (Thread.CurrentThread == mainThread)
            {
                long tmpMem = Profiler.GetTotalAllocatedMemoryLong();
                float tmpTime = Time.realtimeSinceStartup;
                FuncData tmp = FunctionDatas[methodName];
                //过滤因为GC而统计不正确的数据
                if (tmpMem - tmp.BeginMemory >= 0)
                {
                    tmp.FuncMemory = tmpMem - tmp.BeginMemory;
                    tmp.FuncTime = tmpTime - tmp.BeginTime;
                    tmp.FuncTotalMemory += tmp.FuncMemory;
                    tmp.FuncTotalTime += tmp.FuncTime;
                    tmp.FuncCalls += 1;
                    tmp.BeginMemory = 0L;
                    tmp.BeginTime = 0f;
                    FunctionDatas[methodName] = tmp;
                }
            }
        }

        public static void MethodAnalysisReport()
        {
            string nowTime = System.DateTime.Now.ToString("[yyyy-MM-dd]-[HH-mm-ss]");
            string fileName = nowTime + ".csv";
            string header = "FuncName,FuncMemory/k,FuncAverageMemory/k,FuncUseTime/s,FuncAverageTime/ms,FuncCalls";
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine(header);
                var ge = FunctionDatas.GetEnumerator();
                while (ge.MoveNext())
                {
                    var tmp = ge.Current.Value;
                    //过滤调用次数0的函数
                    if (tmp.FuncCalls <= 0) continue;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0},", tmp.FuncName);
                    sb.AppendFormat("{0:f4},", tmp.FuncMemory / 1024.0); //本地调用占用内存
                    sb.AppendFormat("{0:f4},", tmp.FuncTotalMemory / (tmp.FuncCalls * 1024.0));
                    sb.AppendFormat("{0},", tmp.FuncTime);
                    sb.AppendFormat("{0},", tmp.FuncTotalTime / tmp.FuncCalls * 1000);
                    sb.AppendFormat("{0}", tmp.FuncCalls);
                    sw.WriteLine(sb);
                }
            }
            Debug.Log("文件输出完成");
        }

        public static void BeginSample(string methodName)
        {
            Profiler.BeginSample(methodName);
        }

        public static void EndSample()
        {
            Profiler.EndSample();
        }

        public static void BeginDebugLog(string methodName)
        {
            Debug.Log($"---Hook BeginDebugLog:{methodName}");
        }

        public static void EndDebugLog(string methodName)
        {
            Debug.Log($"---Hook EndDebugLog:{methodName}");
        }

        public static void PrintMethodDatas()
        {
            Debug.Log("------------打印函数执行效率-----------------");
            var ge = FunctionDatas.GetEnumerator();
            while (ge.MoveNext())
            {
                var tmp = ge.Current.Value;
                //过滤调用次数0的函数
                if (tmp.FuncCalls <= 0) continue;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0},", tmp.FuncName);
                sb.AppendFormat("{0:f4},", tmp.FuncMemory / 1024.0); //本地调用占用内存
                sb.AppendFormat("{0:f4},", tmp.FuncTotalMemory / (tmp.FuncCalls * 1024.0));
                sb.AppendFormat("{0},", tmp.FuncTime);
                sb.AppendFormat("{0},", tmp.FuncTotalTime / tmp.FuncCalls);
                sb.AppendFormat("{0}", tmp.FuncCalls);
                Debug.Log(sb.ToString());
            }
        }

        //public static List<FunctionMonitorFileDatas> GetFunctionMonitorFileDatas()
        //{
        //    List<FunctionMonitorFileDatas> datas = new List<FunctionMonitorFileDatas>();
        //    foreach (var pair in ProfilersDatas)
        //    {
        //        if (pair.Value.Count == 1)
        //        {
        //            datas.Add(new FunctionMonitorFileDatas()
        //            {
        //                FunctionName = $"{pair.Key}()",
        //                CallCount = 1,
        //                AverageAllocatedMemory = pair.Value[0].EndTotalAllocatedMemory - pair.Value[0].BeginTotalAllocatedMemory,
        //                AverageRunTime = pair.Value[0].EndTime - pair.Value[0].BeginTime,
        //                AverageAllocatedMemoryStr = ConverUtils.ByteConversionGBMBKB(pair.Value[0].EndTotalAllocatedMemory - pair.Value[0].BeginTotalAllocatedMemory),
        //                AverageRunTimeStr = $"{(pair.Value[0].EndTime - pair.Value[0].BeginTime) * 1000}ms"
        //            });
        //        }
        //        else
        //        {
        //            int count = 0;
        //            long deltaTotalAllocMemory = 0;
        //            float deltaTotalTime = 0;
        //            foreach (var funcData in pair.Value)
        //            {
        //                count++;
        //                deltaTotalAllocMemory += funcData.AverageAllocatedMemory;
        //                deltaTotalTime += funcData.AverageTime;
        //            }
        //            datas.Add(new FunctionMonitorFileDatas()
        //            {
        //                FunctionName = $"{pair.Key}()",
        //                CallCount = count,
        //                AverageAllocatedMemory = deltaTotalAllocMemory / count,
        //                AverageRunTime = deltaTotalTime / count,
        //                AverageAllocatedMemoryStr = ConverUtils.ByteConversionGBMBKB(deltaTotalAllocMemory / count),
        //                AverageRunTimeStr = $"{deltaTotalTime / count * 1000}ms"
        //            });
        //        }
        //    }
        //    return datas;
        //}
    }
}

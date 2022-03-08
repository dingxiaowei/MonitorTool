using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace MonitorLib.GOT
{
    public static class HookUtil
    {
        public static Dictionary<string, List<FunctionMonitorDatas>> ProfilersDatas = new Dictionary<string, List<FunctionMonitorDatas>>();
        static Thread mainThread = Thread.CurrentThread;
        public static void Begin(string methodName)
        {
            //一些方法不能再Unity主线程调用
            if (Thread.CurrentThread == mainThread)
            {

            }
            FunctionMonitorDatas data = new FunctionMonitorDatas()
            {
                BeginTotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong(),
                BeginTime = Time.realtimeSinceStartup
            };
            if (ProfilersDatas.ContainsKey(methodName))
            {
                var datas = ProfilersDatas[methodName];
                data.Index = datas.Count + 1;
                datas.Add(data);
            }
            else
            {
                var methodProfileDatas = new List<FunctionMonitorDatas>();
                data.Index = 1;
                methodProfileDatas.Add(data);
                ProfilersDatas.Add(methodName, methodProfileDatas);
            }
        }

        public static void End(string methodName)
        {
            if (!ProfilersDatas.ContainsKey(methodName))
            {
                Debug.LogError($"没有注册方法{methodName}");
                return;
            }
            var lastMethodProfileData = ProfilersDatas[methodName].Last();
            lastMethodProfileData.EndTotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
            lastMethodProfileData.AverageAllocatedMemory = lastMethodProfileData.EndTotalAllocatedMemory - lastMethodProfileData.BeginTotalAllocatedMemory;
            lastMethodProfileData.EndTime = Time.realtimeSinceStartup;
            lastMethodProfileData.AverageTime = lastMethodProfileData.EndTime - lastMethodProfileData.BeginTime;

            //需要屏蔽
            //Debug.LogError($"{methodName}   " + lastMethodProfileData.ToString());
        }

        public static void BeginSample(string methodName)
        {
            Profiler.BeginSample(methodName);
        }

        public static void EndSample()
        {
            Profiler.EndSample();
        }

        public static void PrintProfilerDatas()
        {
            Debug.Log("------------打印函数执行效率-----------------");
            foreach (var pair in ProfilersDatas)
            {
                Debug.Log($"当前函数是:{pair.Key}()");
                if (pair.Value.Count == 1)
                {
                    Debug.Log($"{pair.Value[0]}");
                }
                else
                {
                    int count = 0;
                    long deltaTotalAllocMemory = 0;
                    float deltaTotalTime = 0;
                    foreach (var funcData in pair.Value)
                    {
                        Debug.Log($"{funcData.ToString()}");
                        count++;
                        deltaTotalAllocMemory += funcData.AverageAllocatedMemory;
                        deltaTotalTime += funcData.AverageTime;
                    }
                    //计算一个平均值
                    Debug.Log($"函数总共调用{count}次 平均开辟内存{ConverUtils.ByteConversionGBMBKB(deltaTotalAllocMemory / count)} 平均执行耗时:{deltaTotalTime / count * 1000}ms");
                }
            }
        }

        public static List<FunctionMonitorFileDatas> GetFunctionMonitorFileDatas()
        {
            List<FunctionMonitorFileDatas> datas = new List<FunctionMonitorFileDatas>();
            foreach (var pair in ProfilersDatas)
            {
                if (pair.Value.Count == 1)
                {
                    datas.Add(new FunctionMonitorFileDatas()
                    {
                        FunctionName = $"{pair.Key}()",
                        CallCount = 1,
                        AverageAllocatedMemory = pair.Value[0].EndTotalAllocatedMemory - pair.Value[0].BeginTotalAllocatedMemory,
                        AverageRunTime = pair.Value[0].EndTime - pair.Value[0].BeginTime,
                        AverageAllocatedMemoryStr = ConverUtils.ByteConversionGBMBKB(pair.Value[0].EndTotalAllocatedMemory - pair.Value[0].BeginTotalAllocatedMemory),
                        AverageRunTimeStr = $"{(pair.Value[0].EndTime - pair.Value[0].BeginTime) * 1000}ms"
                    });
                }
                else
                {
                    int count = 0;
                    long deltaTotalAllocMemory = 0;
                    float deltaTotalTime = 0;
                    foreach (var funcData in pair.Value)
                    {
                        count++;
                        deltaTotalAllocMemory += funcData.AverageAllocatedMemory;
                        deltaTotalTime += funcData.AverageTime;
                    }
                    datas.Add(new FunctionMonitorFileDatas()
                    {
                        FunctionName = $"{pair.Key}()",
                        CallCount = count,
                        AverageAllocatedMemory = deltaTotalAllocMemory / count,
                        AverageRunTime = deltaTotalTime / count,
                        AverageAllocatedMemoryStr = ConverUtils.ByteConversionGBMBKB(deltaTotalAllocMemory / count),
                        AverageRunTimeStr = $"{deltaTotalTime / count * 1000}ms"
                    });
                }
            }
            return datas;
        }
    }
}

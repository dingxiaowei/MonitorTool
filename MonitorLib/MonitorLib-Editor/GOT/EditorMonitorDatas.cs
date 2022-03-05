using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace MonitorLib.GOT.Editor
{
    public class FunctionMonitorDatas
    {
        public int Index;
        public long BeginTotalAllocatedMemory;
        public long EndTotalAllocatedMemory;
        public long DeltaAllocatedMemory;
        public float BeginTime;
        public float EndTime;
        public float DeltaTime;

        public override string ToString()
        {
            return $"Index:{Index} DeltaAllocatedMemory:{DeltaAllocatedMemory} DeltaTime:{DeltaTime}";
        }
    }

    public static class HookUtil
    {
        static Dictionary<string, List<FunctionMonitorDatas>> profilersDatas = new Dictionary<string, List<FunctionMonitorDatas>>();
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
            if (profilersDatas.ContainsKey(methodName))
            {
                var datas = profilersDatas[methodName];
                data.Index = datas.Count + 1;
                datas.Add(data);
            }
            else
            {
                var methodProfileDatas = new List<FunctionMonitorDatas>();
                data.Index = 1;
                methodProfileDatas.Add(data);
                profilersDatas.Add(methodName, methodProfileDatas);
            }
        }

        public static void End(string methodName)
        {
            if (!profilersDatas.ContainsKey(methodName))
            {
                Debug.LogError($"没有注册方法{methodName}");
                return;
            }
            var lastMethodProfileData = profilersDatas[methodName].Last();
            lastMethodProfileData.EndTotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
            lastMethodProfileData.DeltaAllocatedMemory = lastMethodProfileData.EndTotalAllocatedMemory - lastMethodProfileData.BeginTotalAllocatedMemory;
            lastMethodProfileData.EndTime = Time.realtimeSinceStartup;
            lastMethodProfileData.DeltaTime = lastMethodProfileData.EndTime - lastMethodProfileData.BeginTime;

            Debug.LogError(lastMethodProfileData.ToString());
        }
    }
}

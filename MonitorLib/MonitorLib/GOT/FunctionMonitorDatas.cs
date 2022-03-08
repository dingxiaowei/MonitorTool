using System;
using System.Collections.Generic;

namespace MonitorLib.GOT
{
    [Serializable]
    public class FunctionMonitorDatas
    {
        public int Index;
        public long BeginTotalAllocatedMemory;
        public long EndTotalAllocatedMemory;
        public long AverageAllocatedMemory;
        public float BeginTime;
        public float EndTime;
        public float AverageTime;

        public override string ToString()
        {
            return $"函数第{Index}次调用 开辟的内存:{ConverUtils.ByteConversionGBMBKB(AverageAllocatedMemory)} 函数运行耗时:{AverageTime * 1000}ms";
        }
    }

    [Serializable]
    public class FunctionMonitorFileDatas
    {
        /// <summary>
        /// 函数名
        /// </summary>
        public string FunctionName;
        /// <summary>
        /// 调用次数
        /// </summary>
        public int CallCount;
        /// <summary>
        /// 平均开辟内存
        /// </summary>
        public long AverageAllocatedMemory;
        /// <summary>
        /// 平均执行时间
        /// </summary>
        public float AverageRunTime;
        public string AverageAllocatedMemoryStr;
        public string AverageRunTimeStr;

        public override string ToString()
        {
            return $"函数名:{FunctionName} 调用次数:{CallCount} 平均开辟内存:{ConverUtils.ByteConversionGBMBKB(AverageAllocatedMemory)} 平均执行时间:{AverageRunTime * 1000}ms";
        }
    }

    [Serializable]
    public class FunctionAnalysisDatas
    {
        public List<FunctionMonitorFileDatas> FunctionAnalysDatas;
    }
}

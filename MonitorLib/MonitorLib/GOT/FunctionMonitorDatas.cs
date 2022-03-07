using System;

namespace MonitorLib.GOT
{
    [Serializable]
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
            return $"函数第{Index}次调用 开辟的内存:{ConverUtils.ByteConversionGBMBKB(DeltaAllocatedMemory)} 函数运行耗时:{DeltaTime * 1000}ms";
        }
    }
}

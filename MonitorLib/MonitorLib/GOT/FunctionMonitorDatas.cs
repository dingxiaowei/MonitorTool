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
        public long DeltaAllocatedMemory;
        public float BeginTime;
        public float EndTime;
        public float DeltaTime;

        public override string ToString()
        {
            return $"Index:{Index} DeltaAllocatedMemory:{DeltaAllocatedMemory}Byte DeltaTime:{DeltaTime}s";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonitorLib.GOT
{
    /// <summary>
    /// 帧率信息
    /// </summary>
    [Serializable]
    public struct MonitorFrameInfo
    {
        public int FrameIndex;
        public int Frame;
    }

    /// <summary>
    /// 内存信息
    /// </summary>
    [Serializable]
    public struct MonitorMemoryInfo
    {
        public int FrameIndex;
        public int MemorySize;
    }

    /// <summary>
    /// 电量信息
    /// </summary>
    [Serializable]
    public struct MonitorBatteryLevelInfo
    {
        public int FrameIndex;
        public float BatteryLevel;
    }

    [Serializable]
    public struct MonitorInfo
    {
        public int FrameIndex;
        public float BatteryLevel;
        public int MemorySize;
        public int Frame;
        /// <summary>
        /// 托管堆内存
        /// </summary>
        public long MonoHeapSize;
        /// <summary>
        /// Mono堆内存使用大小
        /// </summary>
        public long MonoUsedSize;
        
        public long AllocatedMemoryForGraphicsDriver;
        /// <summary>
        /// Unity分配的内存
        /// </summary>
        public long TotalAllocatedMemory;
        /// <summary>
        /// Unity保留的总内存
        /// </summary>
        public long UnityTotalReservedMemory;
        /// <summary>
        /// 未使用内存
        /// </summary>
        public long TotalUnusedReservedMemory;


        public void DeSerialize(BinaryReader reader)
        {
            FrameIndex = reader.ReadInt32();
            BatteryLevel = reader.ReadSingle();
            MemorySize = reader.ReadInt32();
            Frame = reader.ReadInt32();
            MonoHeapSize = reader.ReadInt64();
            MonoUsedSize = reader.ReadInt64();
            AllocatedMemoryForGraphicsDriver = reader.ReadInt64();
            TotalAllocatedMemory = reader.ReadInt64();
            UnityTotalReservedMemory = reader.ReadInt64();
            TotalUnusedReservedMemory = reader.ReadInt64();
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FrameIndex);
            writer.Write(BatteryLevel);
            writer.Write(MemorySize);
            writer.Write(Frame);
            writer.Write(MonoHeapSize);
            writer.Write(MonoUsedSize);
            writer.Write(AllocatedMemoryForGraphicsDriver);
            writer.Write(TotalAllocatedMemory);
            writer.Write(UnityTotalReservedMemory);
            writer.Write(TotalUnusedReservedMemory);
        }

        public override string ToString()
        {
            return $"Frame:{FrameIndex} BatteryLevel:{BatteryLevel} MemorySize:{MemorySize} Frame:{Frame} MonoHeapSize:{MonoHeapSize} MonoUsedSize:{MonoUsedSize} AllocatedMemoryForGraphicsDriver:{AllocatedMemoryForGraphicsDriver} TotalAllocatedMemory:{TotalAllocatedMemory} UnityTotalReservedMemory:{UnityTotalReservedMemory} TotalUnusedReservedMemory:{TotalUnusedReservedMemory}";
        }
    }

    [Serializable]
    public class MonitorInfos
    {
        public List<MonitorInfo> MonitorInfoList = new List<MonitorInfo>();
        public void DeSerialize(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                MonitorInfo tempData = new MonitorInfo();
                tempData.DeSerialize(reader);
                MonitorInfoList.Add(tempData);
            }
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(MonitorInfoList.Count);
            for (int i = 0; i < MonitorInfoList.Count; i++)
            {
                MonitorInfoList[i].Serialize(writer);
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < MonitorInfoList.Count; i++)
            {
                sb.Append($"{MonitorInfoList[i].ToString()}\n");
            }
            return sb.ToString();
        }
    }
}

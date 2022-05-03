using System;
using System.IO;

namespace MonitorLib.GOT
{
    [Serializable]
    public struct FuncAnalysisInfo : IBinarySerializable
    {
        public string Name;
        //k
        public double Memory;
        //k
        public double AverageMemory;
        //s
        public float UseTime;
        /// <summary>
        /// 平均调用时间ms
        /// </summary>
        public float AverageTime;
        /// <summary>
        /// 调用次数
        /// </summary>
        public int Calls;
        public void DeSerialize(BinaryReader reader)
        {
            Name = reader.ReadString();
            Memory = reader.ReadDouble();
            AverageMemory = reader.ReadDouble();
            UseTime = reader.ReadSingle();
            AverageTime = reader.ReadSingle();
            Calls = reader.ReadInt32();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Memory);
            writer.Write(AverageMemory);
            writer.Write(UseTime);
            writer.Write(AverageTime);
            writer.Write(Calls);
        }

        public override string ToString()
        {
            return $"函数名:{Name} 使用内存:{Memory}kb 平均使用内存:{AverageMemory}kb 使用时间:{UseTime}s 平均使用时间:{AverageTime}ms 调用次数:{Calls}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorLib.GOT
{
    /// <summary>
    /// 内存使用信息
    /// </summary>
    [Serializable]
    public struct MemoryUseData : IBinarySerializable
    {
        public int FrameIndex;
        public float PssMemorySize;//M

        public void DeSerialize(BinaryReader reader)
        {
            FrameIndex = reader.ReadInt32();
            PssMemorySize = reader.ReadSingle();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FrameIndex);
            writer.Write(PssMemorySize);
        }
    }

    /// <summary>
    /// PSS内存使用
    /// </summary>
    public class MemoryUseDatas : IBinarySerializable
    {
        public List<MemoryUseData> MemoryUsedList= new List<MemoryUseData>();

        public void DeSerialize(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                MemoryUseData tempData = new MemoryUseData();
                tempData.DeSerialize(reader);
                MemoryUsedList.Add(tempData);
            }
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(MemoryUsedList.Count);
            for (int i = 0; i < MemoryUsedList.Count; i++)
            {
                MemoryUsedList[i].Serialize(writer);
            }
        }
    }
}

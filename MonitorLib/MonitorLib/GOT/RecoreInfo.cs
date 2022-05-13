using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonitorLib.GOT
{
    [Serializable]
    public class RecoreInfo : IBinarySerializable
    {
        public string Name;
        public int Count;
        public long Size;

        public RecoreInfo(string name)
        {
            Name = name;
            Count = 0;
            Size = 0L;
        }

        public RecoreInfo() { }

        public void DeSerialize(BinaryReader reader)
        {
            Name = reader.ReadString();
            Count = reader.ReadInt32();
            Size = reader.ReadInt64();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Count);
            writer.Write(Size);
        }

        public override string ToString()
        {
            return $"Name:{Name} Count:{Count} Size:{Size}";
        }
    }

    //一帧里面的各个资源数据
    [Serializable]
    public class RecordInfos : IBinarySerializable
    {
        long Size = 0L;
        int Count = 0;
        List<RecoreInfo> RecordInfoList = new List<RecoreInfo>();

        public void AddInfo(RecoreInfo info)
        {
            RecordInfoList.Add(info);
            Size += info.Size;
            Count += info.Count;
        }

        public void DeSerialize(BinaryReader reader)
        {
            Size = reader.ReadInt64();
            Count = reader.ReadInt32();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                RecoreInfo tempData = new RecoreInfo();
                tempData.DeSerialize(reader);
                RecordInfoList.Add(tempData);
            }
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Size);
            writer.Write(Count);
            writer.Write(RecordInfoList.Count);
            for (int i = 0; i < RecordInfoList.Count; i++)
            {
                RecordInfoList[i].Serialize(writer);
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Count:{Count} Size:{Size}\n");
            for (int i = 0; i < RecordInfoList.Count; i++)
            {
                sb.Append($"{RecordInfoList[i].ToString()}\n");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 采样帧内存分类数据记录集合
    /// </summary>
    [Serializable]
    public class RecordInfosCollections : IBinarySerializable
    {
        public List<RecordInfos> RecordInfosList = new List<RecordInfos>();
        public void DeSerialize(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                RecordInfos tempData = new RecordInfos();
                tempData.DeSerialize(reader);
                RecordInfosList.Add(tempData);
            }
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(RecordInfosList.Count);
            for (int i = 0; i < RecordInfosList.Count; i++)
            {
                RecordInfosList[i].Serialize(writer);
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < RecordInfosList.Count; i++)
            {
                sb.Append($"{RecordInfosList[i].ToString()}\n");
            }
            return sb.ToString();
        }
    }
}

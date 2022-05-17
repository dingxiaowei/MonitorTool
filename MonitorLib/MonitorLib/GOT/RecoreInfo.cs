using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonitorLib.GOT
{
    [Serializable]
    public class RecoreInfo : IBinarySerializable
    {
        public int FrameIndex;
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
            FrameIndex = reader.ReadInt32();
            Name = reader.ReadString();
            Count = reader.ReadInt32();
            Size = reader.ReadInt64();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FrameIndex);
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

    [Serializable]
    public struct RecordResInfo : IBinarySerializable
    {
        public int FrameIndex;
        public long TextureSize;
        public int TextureCount;
        public long MeshSize;
        public int MeshCount;
        public long MaterialSize;
        public int MaterialCount;
        public long ShaderSize;
        public int ShaderCount;
        public long AnimationClipSize;
        public int AnimationClipCount;
        public long AudioClipSize;
        public int AudioClipCount;
        public long FontSize;
        public int FontCount;
        public long TextAssetSize;
        public int TextAssetCount;
        public long ScriptableObjectSize;
        public int ScriptableObjectCount;
        public long TotalSize; //统计部分的总量
        public int TotalCount;//统计部分的数量

        public void DeSerialize(BinaryReader reader)
        {
            FrameIndex = reader.ReadInt32();
            TextureSize = reader.ReadInt64();
            TextureCount = reader.ReadInt32();
            MeshSize = reader.ReadInt64();
            MeshCount = reader.ReadInt32();
            MaterialSize = reader.ReadInt64();
            MaterialCount = reader.ReadInt32();
            ShaderSize = reader.ReadInt64();
            ShaderCount = reader.ReadInt32();
            AnimationClipSize = reader.ReadInt64();
            AnimationClipCount = reader.ReadInt32();
            AudioClipSize = reader.ReadInt64();
            AudioClipSize = reader.ReadInt32();
            FontSize = reader.ReadInt64();
            FontCount = reader.ReadInt32();
            TextAssetSize = reader.ReadInt64();
            TextAssetCount = reader.ReadInt32();
            ScriptableObjectSize = reader.ReadInt64();
            ScriptableObjectCount = reader.ReadInt32();
            TotalSize = reader.ReadInt64();
            TotalCount = reader.ReadInt32();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FrameIndex);
            writer.Write(TextureSize);
            writer.Write(TextureCount);
            writer.Write(MeshSize);
            writer.Write(MeshCount);
            writer.Write(MaterialSize);
            writer.Write(MaterialCount);
            writer.Write(ShaderSize);
            writer.Write(ShaderCount);
            writer.Write(AnimationClipSize);
            writer.Write(AnimationClipCount);
            writer.Write(AudioClipSize);
            writer.Write(AudioClipCount);
            writer.Write(FontSize);
            writer.Write(FontCount);
            writer.Write(TextAssetSize);
            writer.Write(TextAssetCount);
            writer.Write(ScriptableObjectSize);
            writer.Write(ScriptableObjectCount);
            writer.Write(TotalSize);
            writer.Write(TotalCount);
        }
    }

    [Serializable]
    public class RecoreResInfos : IBinarySerializable
    {
        public List<RecordResInfo> RecordResInfosList = new List<RecordResInfo>();
        public void DeSerialize(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                RecordResInfo tempData = new RecordResInfo();
                tempData.DeSerialize(reader);
                RecordResInfosList.Add(tempData);
            }
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(RecordResInfosList.Count);
            for (int i = 0; i < RecordResInfosList.Count; i++)
            {
                RecordResInfosList[i].Serialize(writer);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonitorLib.GOT
{
    [Serializable]
    public class LogInfos : IBinarySerialize
    {
        public List<string> LogDatas = new List<string>();

        public void DeSerialize(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string str = reader.ReadString();
                LogDatas.Add(str);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(LogDatas.Count);
            for (int i = 0; i < LogDatas.Count; i++)
            {
                writer.Write(LogDatas[i]);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < LogDatas.Count; i++)
            {
                sb.AppendLine(LogDatas[i]);
            }
            return sb.ToString();
        }
    }
}

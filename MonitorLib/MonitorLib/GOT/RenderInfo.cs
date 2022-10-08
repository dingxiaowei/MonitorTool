using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonitorLib.GOT
{
    [Serializable]
    public struct RenderInfo : IBinarySerializable
    {
        public int FrameIndex;
        public long SetPassCall;
        public long DrawCall;
        /// <summary>
        /// 定点数据
        /// </summary>
        public long Vertices;
        /// <summary>
        /// 三角面个数
        /// </summary>
        public long Triangles;
        public void DeSerialize(BinaryReader reader)
        {
            FrameIndex = reader.ReadInt32();
            SetPassCall = reader.ReadInt64();
            DrawCall = reader.ReadInt64();
            Vertices = reader.ReadInt64();
            Triangles = reader.ReadInt64();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FrameIndex);
            writer.Write(SetPassCall);
            writer.Write(DrawCall);
            writer.Write(Vertices);
            writer.Write(Triangles);
        }

        public override string ToString()
        {
            return $"第{FrameIndex}帧 SetPassCall:{SetPassCall} DrawCall:{DrawCall} 顶点数:{Vertices} 三角面数:{Triangles}";
        }
    }

    [Serializable]
    public class RenderInfos : IBinarySerializable
    {
        public List<RenderInfo> RenderInfoList = new List<RenderInfo>();
        public void DeSerialize(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                RenderInfo tempData = new RenderInfo();
                tempData.DeSerialize(reader);
                RenderInfoList.Add(tempData);
            }
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(RenderInfoList.Count);
            for (int i = 0; i < RenderInfoList.Count; i++)
            {
                RenderInfoList[i].Serialize(writer);
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < RenderInfoList.Count; i++)
            {
                sb.Append($"{RenderInfoList[i].ToString()}\n");
            }
            return sb.ToString();
        }
    }
}

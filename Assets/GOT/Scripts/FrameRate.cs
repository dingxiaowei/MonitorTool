using MonitorLib.GOT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public struct FrameRate : IBinarySerializable
{
    public int FrameIndex;
    public int Frame;

    public void DeSerialize(BinaryReader reader)
    {
        FrameIndex = reader.ReadInt32();
        Frame = reader.ReadInt32();
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(FrameIndex);
        writer.Write(Frame);
    }
}

public class FrameRateInfos : IBinarySerializable
{
    public List<FrameRate> FrameRateList = new List<FrameRate>();
    public void DeSerialize(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            FrameRate tempData = new FrameRate();
            tempData.DeSerialize(reader);
            FrameRateList.Add(tempData);
        }
    }
    public void Serialize(BinaryWriter writer)
    {
        writer.Write(FrameRateList.Count);
        for (int i = 0; i < FrameRateList.Count; i++)
        {
            FrameRateList[i].Serialize(writer);
        }
    }
}

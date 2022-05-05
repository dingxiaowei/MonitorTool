using System;
using System.IO;

namespace MonitorLib.GOT
{
    /// <summary>
    /// 一次测试数据
    /// </summary>
    [Serializable]
    public struct TestInfo : IBinarySerializable
    {
        public string ProductName;
        public string PackageName;
        public string Platform;
        public string Version;
        public string TestTime;
        public int IntervalFrame;//检测多少帧截图/获取原生数据一次
        public override string ToString()
        {
            return
                $"产品名：{ProductName}\n" +
                $"包名：{PackageName}\n" +
                $"平台：{Platform}\n" +
                $"版本号：{Version}\n" +
                $"测试时长：{TestTime}\n" +
                $"检测多少帧检测一次:{IntervalFrame}";
        }
        public void DeSerialize(BinaryReader reader)
        {
            ProductName = reader.ReadString();
            PackageName = reader.ReadString();
            Platform = reader.ReadString();
            Version = reader.ReadString();
            TestTime = reader.ReadString();
            IntervalFrame = reader.ReadInt32();
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ProductName);
            writer.Write(PackageName);
            writer.Write(Platform);
            writer.Write(Version);
            writer.Write(TestTime);
            writer.Write(IntervalFrame);
        }
    }
}

//二进制文件读取
//IBinarySerializable testInfo = new TestInfo();
//var res = FileManager.ReadBinaryDataFromFile(binaryFile, ref testInfo);
//if(res)
//{
//    Debug.LogError("解析成功");
//    Debug.Log(testInfo.ToString());
//}

//二进制文件写入
//writeRes = FileManager.WriteBinaryDataToFile(testFilePath, testInfo);

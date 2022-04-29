using System;
using System.IO;

namespace MonitorLib.GOT
{
    [Serializable]
    public struct DeviceInfo : IBinarySerialize
    {
        /// <summary>
        /// Unity版本
        /// </summary>
        public string UnityVersion;
        /// <summary>
        /// 操作系统名称
        /// </summary>
        public string OperatingSystem;
        /// <summary>
        /// 设备模型
        /// </summary>
        public string DeviceModel;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName;
        /// <summary>
        /// 设备唯一标识符
        /// </summary>
        public string DeviceUniqueIdentifier;
        /// <summary>
        /// 系统内存大小
        /// </summary>
        public int SystemMemorySize;
        /// <summary>
        /// 显存大小
        /// </summary>
        public int GraphicsMemorySize;
        /// <summary>
        /// 处理器名称
        /// </summary>
        public string ProcessorType;
        /// <summary>
        /// 处理器频率
        /// </summary>
        public int ProcessorFrequency;
        /// <summary>
        /// 处理器数量
        /// </summary>
        public int ProcessorCount;
        /// <summary>
        /// 显卡的名字
        /// </summary>
        public string GraphicsDeviceName;
        /// <summary>
        /// 显卡厂商
        /// </summary>
        public string GraphicsDeviceVendor;
        /// <summary>
        /// 显卡所支持的图形API版本
        /// </summary>
        public string GraphicsDeviceVersion;
        /// <summary>
        /// 是否支持内置阴影
        /// </summary>
        public bool SupportsShadows;
        /// <summary>
        /// 电池电量
        /// </summary>
        public float BatteryLevel;
        //屏幕分辨率
        public int ScreenWidth;
        public int ScreenHeight;

        public void DeSerialize(BinaryReader reader)
        {
            UnityVersion = reader.ReadString();
            OperatingSystem = reader.ReadString();
            DeviceModel = reader.ReadString();
            DeviceName = reader.ReadString();
            DeviceUniqueIdentifier = reader.ReadString();
            SystemMemorySize = reader.ReadInt32();
            GraphicsMemorySize = reader.ReadInt32();
            ProcessorType = reader.ReadString();
            ProcessorFrequency = reader.ReadInt32();
            ProcessorCount = reader.ReadInt32();
            GraphicsDeviceName = reader.ReadString();
            GraphicsDeviceVendor = reader.ReadString();
            GraphicsDeviceVersion = reader.ReadString();
            SupportsShadows = reader.ReadBoolean();
            BatteryLevel = reader.ReadSingle();
            ScreenWidth = reader.ReadInt32();
            ScreenHeight = reader.ReadInt32();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(UnityVersion);
            writer.Write(OperatingSystem);
            writer.Write(DeviceModel);
            writer.Write(DeviceName);
            writer.Write(DeviceUniqueIdentifier);
            writer.Write(SystemMemorySize);
            writer.Write(GraphicsMemorySize);
            writer.Write(ProcessorType);
            writer.Write(ProcessorFrequency);
            writer.Write(ProcessorCount);
            writer.Write(GraphicsDeviceName);
            writer.Write(GraphicsDeviceVendor);
            writer.Write(GraphicsDeviceVersion);
            writer.Write(SupportsShadows);
            writer.Write(BatteryLevel);
            writer.Write(ScreenWidth);
            writer.Write(ScreenHeight);
        }

        public override string ToString()
        {
            return
                $"Unity版本：{UnityVersion}\n" +
                $"操作系统名称：{OperatingSystem}\n" +
                $"设备模型：{DeviceModel}\n" +
                $"设备名称：{DeviceName}\n" +
                $"设备唯一标识符：{DeviceUniqueIdentifier}\n" +
                $"设备内存大小：{SystemMemorySize}\n" +
                $"设备显存大小：{GraphicsMemorySize}\n" +
                $"处理器名称：{ProcessorType}\n" +
                $"处理器频率：{ProcessorFrequency}\n" +
                $"处理器数量：{ProcessorCount}\n" +
                $"显卡名字：{GraphicsDeviceName}\n" +
                $"显卡厂商：{GraphicsDeviceVendor}\n" +
                $"显卡所支持的图形版本：{GraphicsDeviceVersion}\n" +
                $"是否内置阴影：{SupportsShadows}\n" +
                $"电池电量：{BatteryLevel}\n" +
                $"屏幕分辨率：width:{ScreenWidth} height:{ScreenHeight}";
        }
    }
}

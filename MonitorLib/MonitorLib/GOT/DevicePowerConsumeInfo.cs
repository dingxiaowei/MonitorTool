using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonitorLib.GOT
{
    [Serializable]
    public struct DevicePowerConsumeInfo : IBinarySerializable
    {
        /// <summary>
        /// 帧率
        /// </summary>
        public int FrameIndex;
        /// <summary>
        /// 电池总容量
        /// </summary>
        public int Capacity;
        /// <summary>
        /// 电池温度
        /// </summary>
        public int Temperature;
        /// <summary>
        /// 电池电压
        /// </summary>
        public float BatteryV;
        /// <summary>
        /// 剩余电量百分比（value）
        /// </summary>
        public int BatteryCapacity;
        /// <summary>
        /// 当前剩余容量
        /// </summary>
        public int BatteryChargeCounter;
        /// <summary>
        /// 瞬时电流
        /// </summary>
        public int BatteryCurrentNow;
        /// <summary>
        /// 瞬时功率
        /// </summary>
        public float BatteryPower;
        /// <summary>
        /// 剩余使用时长
        /// </summary>
        public float UseLeftHours;
        /// <summary>
        /// cpu温度
        /// </summary>
        public int CpuTemperate;

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FrameIndex);
            writer.Write(Capacity);
            writer.Write(Temperature);
            writer.Write(BatteryV);
            writer.Write(BatteryCapacity);
            writer.Write(BatteryChargeCounter);
            writer.Write(BatteryCurrentNow);
            writer.Write(BatteryPower);
            writer.Write(UseLeftHours);
            writer.Write(CpuTemperate);

        }

        public void DeSerialize(BinaryReader reader)
        {
            FrameIndex = reader.ReadInt32();
            Capacity = reader.ReadInt32();
            Temperature = reader.ReadInt32();
            BatteryV = reader.ReadSingle();
            BatteryCapacity = reader.ReadInt32();
            BatteryChargeCounter = reader.ReadInt32();
            BatteryCurrentNow = reader.ReadInt32();
            BatteryPower = reader.ReadSingle();
            UseLeftHours = reader.ReadSingle();
            CpuTemperate = reader.ReadInt32();
        }

        public override string ToString()
        {
            return
                $"当前帧:{FrameIndex}\n" +
                $"电池总容量:{Capacity}\n" +
                $"电池温度:{Temperature}\n" +
                $"电池电压:{BatteryV}\n" +
                $"剩余电量百分比:{BatteryCapacity}\n" +
                $"当前剩余容量:{BatteryChargeCounter}\n" +
                $"瞬时电流:{BatteryCurrentNow}\n" +
                $"瞬时功率:{BatteryPower}\n" +
                $"剩余使用时长:{UseLeftHours}\n" +
                $"cpu温度:{CpuTemperate}\n";
        }
    }
    [Serializable]
    public class DevicePowerConsumeInfos : IBinarySerializable
    {
        public List<DevicePowerConsumeInfo> devicePowerConsumeInfos = new List<DevicePowerConsumeInfo>();
        public void DeSerialize(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                DevicePowerConsumeInfo tempData = new DevicePowerConsumeInfo();
                tempData.DeSerialize(reader);
                devicePowerConsumeInfos.Add(tempData);
            }
        }
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(devicePowerConsumeInfos.Count);
            for (int i = 0; i < devicePowerConsumeInfos.Count; i++)
            {
                devicePowerConsumeInfos[i].Serialize(writer);
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < devicePowerConsumeInfos.Count; i++)
            {
                sb.Append($"{devicePowerConsumeInfos[i].ToString()}\n");
            }
            return sb.ToString();
        }
    }
}
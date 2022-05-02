using System;
using System.IO;

namespace MonitorLib.GOT
{
    [Serializable]
    public struct DevicePowerConsumeArgs:IBinarySerializable
    {
        /// <summary>
        /// 电池总容量
        /// </summary>
        public int capacity;
        /// <summary>
        /// 电池温度
        /// </summary>
        public int temperature;
        /// <summary>
        /// 电池电压
        /// </summary>
        public float batteryV;
        /// <summary>
        /// 剩余电量百分比（value）
        /// </summary>
        public int batteryCapacity;
        /// <summary>
        /// 当前剩余容量
        /// </summary>
        public int batteryChargeCounter;
        /// <summary>
        /// 瞬时电流
        /// </summary>
        public int batteryCurrentNow;
        /// <summary>
        /// 瞬时功率
        /// </summary>
        public float power;
        /// <summary>
        /// 剩余使用时长
        /// </summary>
        public float useLeftHours;
        /// <summary>
        /// cpu温度
        /// </summary>
        public int cpuTemperate;
        
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(capacity);
            writer.Write(temperature);
            writer.Write(batteryV);
            writer.Write(batteryCapacity);
            writer.Write(batteryChargeCounter);
            writer.Write(batteryCurrentNow);
            writer.Write(power);
            writer.Write(useLeftHours);
            writer.Write(cpuTemperate);
            
        }
        
        public void DeSerialize(BinaryReader reader)
        {
            capacity = reader.ReadInt32();
            temperature = reader.ReadInt32();
            batteryV = reader.ReadSingle();
            batteryCapacity = reader.ReadInt32();
            batteryChargeCounter = reader.ReadInt32();
            batteryCurrentNow = reader.ReadInt32();
            power = reader.ReadSingle();
            useLeftHours = reader.ReadSingle();
            cpuTemperate = reader.ReadInt32();
        }

        public override string ToString()
        {
            return
                $"电池总容量:{capacity}\n"+
                $"电池温度:{temperature}\n"+
                $"电池电压:{batteryV}\n"+
                $"剩余电量百分比:{batteryCapacity}\n"+
                $"当前剩余容量:{batteryChargeCounter}\n"+
                $"瞬时电流:{batteryCurrentNow}\n"+
                $"瞬时功率:{power}\n"+
                $"剩余使用时长:{useLeftHours}\n"+
                $"cpu温度:{cpuTemperate}\n";
        }
    }
}
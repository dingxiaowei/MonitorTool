using System;
using MonitorLib.GOT;
using UnityEngine;

namespace GOT.Scripts
{
    public class UnityAndroidProxy
    {
        private AndroidJavaClass jc;
        private AndroidJavaObject jo;
    
        public void Init()
        {
#if UNITY_ANDROID
            try
            {
                jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }

        public DevicePowerConsumeArgs GetPowerConsumeArgs()
        {
#if UNITY_ANDROID
            if (null == jo || null == jc)
            {
                Init();
            } 
            string result = jo.Call<string>("GetCurPowerConsumeArgs");
            if (string.IsNullOrEmpty(result))
            {
                return default;
            }
            Debug.Log($"从安卓获取结果:{result}");
            string[] args = result.Split('|');
            DevicePowerConsumeArgs devicePowerConsumeArgs = new DevicePowerConsumeArgs();
            devicePowerConsumeArgs.capacity = Convert.ToInt32(args[0]);
            devicePowerConsumeArgs.temperature = Convert.ToInt32(args[1]);
            devicePowerConsumeArgs.batteryV = Convert.ToSingle(args[2]);
            devicePowerConsumeArgs.batteryCapacity = Convert.ToInt32(args[3]);
            devicePowerConsumeArgs.batteryChargeCounter = Convert.ToInt32(args[4]);
            devicePowerConsumeArgs.batteryCurrentNow = Convert.ToInt32(args[5]);
            devicePowerConsumeArgs.power = Convert.ToSingle(args[6]);
            devicePowerConsumeArgs.useLeftHours = Convert.ToSingle(args[7]);
            devicePowerConsumeArgs.cpuTemperate = Convert.ToInt32(args[8]);
            return devicePowerConsumeArgs;
        }
#endif
    }
}
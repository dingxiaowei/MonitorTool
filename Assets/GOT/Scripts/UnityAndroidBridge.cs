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

        public DevicePowerConsumeInfo GetPowerConsumeInfo()
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
            DevicePowerConsumeInfo devicePowerConsumeInfo = new DevicePowerConsumeInfo();
            devicePowerConsumeInfo.capacity = Convert.ToInt32(args[0]);
            devicePowerConsumeInfo.temperature = Convert.ToInt32(args[1]);
            devicePowerConsumeInfo.batteryV = Convert.ToSingle(args[2]);
            devicePowerConsumeInfo.batteryCapacity = Convert.ToInt32(args[3]);
            devicePowerConsumeInfo.batteryChargeCounter = Convert.ToInt32(args[4]);
            devicePowerConsumeInfo.batteryCurrentNow = Convert.ToInt32(args[5]);
            devicePowerConsumeInfo.power = Convert.ToSingle(args[6]);
            devicePowerConsumeInfo.useLeftHours = Convert.ToSingle(args[7]);
            devicePowerConsumeInfo.cpuTemperate = Convert.ToInt32(args[8]);
            return devicePowerConsumeInfo;
        }
#endif
    }
}
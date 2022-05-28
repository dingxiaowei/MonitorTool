using System;
using MonitorLib.GOT;
using UnityEngine;

public class UnityAndroidProxy
{
    private AndroidJavaClass jc;
    private AndroidJavaObject jo;
    private bool isInit = false;

    public UnityAndroidProxy()
    {
        Init();
    }

    public void Init()
    {
#if UNITY_ANDROID
        if (isInit)
        {
            return;
        }
        try
        {
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //jc = new AndroidJavaClass("com.sc.testextendlibrary.MainActivity");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            isInit = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
#endif
    }

#if UNITY_ANDROID
    public DevicePowerConsumeInfo GetPowerConsumeInfo(int frameIndex = 0)
    {
        if (null == jo || null == jc)
        {
            Init();
        }
        string result = jo.Call<string>("GetCurPowerConsumeArgs");
        if (string.IsNullOrEmpty(result))
        {
            return default;
        }
        //Debug.Log($"从安卓获取结果:{result}");
        string[] args = result.Split('|');
        DevicePowerConsumeInfo devicePowerConsumeInfo = new DevicePowerConsumeInfo();
        devicePowerConsumeInfo.FrameIndex = frameIndex;
        devicePowerConsumeInfo.Capacity = Convert.ToInt32(args[0]);
        devicePowerConsumeInfo.Temperature = Convert.ToInt32(args[1]);
        devicePowerConsumeInfo.BatteryV = Convert.ToSingle(args[2]);
        devicePowerConsumeInfo.BatteryCapacity = Convert.ToInt32(args[3]);
        devicePowerConsumeInfo.BatteryChargeCounter = Convert.ToInt32(args[4]);
        devicePowerConsumeInfo.BatteryCurrentNow = Convert.ToInt32(args[5]);
        devicePowerConsumeInfo.BatteryPower = Convert.ToSingle(args[6]);
        devicePowerConsumeInfo.UseLeftHours = Convert.ToSingle(args[7]);
        devicePowerConsumeInfo.CpuTemperate = Convert.ToInt32(args[8]);
        return devicePowerConsumeInfo;
    }

    public MemoryUseData GetPssMemory(int frameIndex = 0)
    {
        if (null == jo || null == jc)
        {
            Init();
        }
        var pss = jo.Call<float>("GetCurAppMemorySize");
        var memoryUseData = new MemoryUseData()
        {
            FrameIndex = frameIndex,
            PssMemorySize = pss
        };
        return memoryUseData;
    }
#endif
}
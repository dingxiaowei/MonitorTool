using MonitorLib.GOT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class GOTProfiler : MonoBehaviour
{
    [Header("是否监控日志")]
    public bool EnableLog = false;
    [Header("是否采集帧图")]
    public bool EnableFrameTexture = false;
    [Header("是否采集函数性能")]
    public bool EnableFunctionAnalysis = false;
    [Header("是否采集CPU温度")]
    public bool EnableCPUInfo = true;
    public int CPUInfoFrame = 5;
    [Header("是否采集电池功耗")]
    public bool EnableBatteryInfo = true;
    public int BatteryInfoFrame = 5;
    [Header("忽略前面的帧数")]
    public int IgnoreFrameCount = 5;

    public Text UploadTips;
    public Text ReportUrl;
    int m_FPS = 0;
    int m_TickTime = 0;
    string m_StartTime;
    float m_Accumulator = 0;
    int m_Frames = 0;
    float m_TimeLeft;
    float m_UpdateInterval = 0.5f;
    bool btnMonitor = false;
    string btnMsg = "开始监控";
    int m_frameIndex = 0;
    Action<bool> MonitorCallback;
    MonitorInfos monitorInfos = null;
    string funcAnalysisFilePath;
    //log日志路径
    string logFilePath;
    //设备信息路径
    string deviceFilePath;
    //测试信息路径
    string testFilePath;
    //性能监控
    string monitorFilePath;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        MonitorCallback += (res) =>
        {
            if (res)
            {
                Debug.Log(Config.Monitoring);
                m_frameIndex = 0;
                ShareDatas.StartTime = DateTime.Now; //当前时间
                m_StartTime = ShareDatas.StartTime.ToString().Replace(" ", "_").Replace("/", "_").Replace(":", "_");
                ShareDatas.StartTimeStr = m_StartTime;

                if (EnableFrameTexture)
                {
                    FileManager.CreateDir($"{Application.persistentDataPath}/{m_StartTime}/");
                }
                if (EnableFunctionAnalysis)
                    funcAnalysisFilePath = $"{Application.persistentDataPath}/funcAnalysis_{m_StartTime}.txt";
                if (EnableLog)
                    logFilePath = $"{Application.persistentDataPath}/log_{m_StartTime}.txt";
                deviceFilePath = $"{Application.persistentDataPath}/device_{m_StartTime}.txt";
                testFilePath = $"{Application.persistentDataPath}/test_{m_StartTime}.txt";
                monitorFilePath = $"{Application.persistentDataPath}/monitor_{m_StartTime}.txt";
                if (EnableLog)
                {
                    LogManager.CreateLogFile(logFilePath, System.IO.FileMode.Append);
                    Application.logMessageReceived += LogManager.LogToFile;
                }

                m_TickTime = 0;
                InvokeRepeating("Tick", 1.0f, 1.0f);

                GetSystemInfo();

                if (ReportUrl != null)
                {
                    ReportUrl.gameObject.SetActive(false);
                }

                //测试Log颜色
                Debug.LogError("测试Error Log");
                Debug.LogWarning("测试Warning Log");

                StartMonitor();
            }
            else
            {
                Debug.Log(Config.MonitorStop);
                ShareDatas.EndTime = DateTime.Now;
                string testTime = ShareDatas.GetTestTime();
                //上传测试时间
                FileManager.WriteToFile(testFilePath, $"应用名：{Application.productName}&nbsp&nbsp&nbsp包名：{Application.identifier}&nbsp&nbsp&nbsp测试系统：{Application.platform}&nbsp&nbsp&nbsp版本号：{Application.version}&nbsp&nbsp&nbsp本次测试时长:{testTime}");
                UploadFile(testFilePath);

                CancelInvoke("Tick");
                m_TickTime = 0;

                if (EnableLog)
                {
                    Application.logMessageReceived -= LogManager.LogToFile;
                    LogManager.CloseLogFile();
                }
                if (EnableLog)
                {
                    FileManager.ReplaceContent(logFilePath, "[Log]", "<font color=\"#0000FF\">[Log]</font>");
                    FileManager.ReplaceContent(logFilePath, "[Error]", "<font color=\"#FF0000\">[Error]</font>");
                    FileManager.ReplaceContent(logFilePath, "[Warning]", "<font color=\"#FFD700\">[Warning]</font>");
                    UploadFile(logFilePath);
                }
//#if ENABLE_ANALYSIS
                if (EnableFunctionAnalysis)
                    //HookUtil.PrintProfilerDatas();
//#endif
                StopMonitor();

                if (ReportUrl != null)
                {
                    ReportUrl.gameObject.SetActive(true);
                    var url = string.Format(ShareDatas.ReportUrl, m_StartTime);
                    ReportUrl.text = $"<a href={url}>[{url}]</a>";
                }
            }
        };
    }

    void StartMonitor()
    {
        monitorInfos = new MonitorInfos();
    }

    void StopMonitor()
    {
        if (monitorInfos.MonitorInfoList.Count > 1)
        {
            monitorInfos.MonitorInfoList.RemoveAt(monitorInfos.MonitorInfoList.Count - 1);
        }

        var monitorToFile = FileManager.WriteToFile(monitorFilePath, JsonUtility.ToJson(monitorInfos));
        if (monitorToFile)
        {
            UploadFile(monitorFilePath);
        }
    }

    void Tick()
    {
        m_TickTime++;
    }

    void UploadFile(string filePath)
    {
        FileUploadManager.UploadFile(filePath, (sender, e) =>
        {
            Debug.Log("Uploading Progreess: " + e.ProgressPercentage);
            if (e.ProgressPercentage > 0 && e.ProgressPercentage < 100)
            {
                if (UploadTips != null && !UploadTips.gameObject.activeSelf)
                {
                    UploadTips.gameObject.SetActive(true);
                }
            }
            else if (e.ProgressPercentage >= 100)
            {
                if (UploadTips != null && UploadTips.gameObject.activeSelf)
                {
                    UploadTips.gameObject.SetActive(false);
                }
            }
            UploadTips.text = $"数据上传中,进度{e.ProgressPercentage}%";
        }, (sender, e) =>
        {
            Debug.Log($"File Uploaded :{e.Result}");
        });
    }

    [HideAnalysis]
    void OnGUI()
    {
        if (GUI.Button(new Rect(150, 350, 200, 100), btnMsg))
        {
            btnMonitor = !btnMonitor;
            btnMsg = btnMonitor ? Config.Monitoring : Config.MonitorBegin;
            if (MonitorCallback != null)
                MonitorCallback.Invoke(btnMonitor);
        }
        if (btnMonitor)
            btnMsg = $"{Config.Monitoring}{m_TickTime}s";
        GUI.Label(new Rect(Screen.width / 2, 0, 100, 100), "FPS:" + m_FPS);
    }

    [HideAnalysis]
    void Update()
    {
        m_Frames++;
        m_Accumulator += Time.unscaledDeltaTime;
        m_TimeLeft -= Time.unscaledDeltaTime;
        if (m_TimeLeft <= 0f)
        {
            m_FPS = (int)(m_Accumulator > 0f ? m_Frames / m_Accumulator : 0f);
            m_Frames = 0;
            m_Accumulator = 0f;
            m_TimeLeft += m_UpdateInterval;
        }

        if (btnMonitor)
        {
            ++m_frameIndex;
            if (m_frameIndex > IgnoreFrameCount)
            {
                var monitorInfo = new MonitorInfo() { FrameIndex = m_frameIndex - IgnoreFrameCount, BatteryLevel = SystemInfo.batteryLevel, MemorySize = 0, Frame = m_FPS, MonoHeapSize = Profiler.GetMonoHeapSizeLong(), MonoUsedSize = Profiler.GetMonoUsedSizeLong(), TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong(), TotalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong(), UnityTotalReservedMemory = Profiler.GetTotalReservedMemoryLong(), AllocatedMemoryForGraphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver() };
                monitorInfos.MonitorInfoList.Add(monitorInfo);
                if (EnableFrameTexture)
                {
                    ScreenCapture.CaptureScreenshot($"{Application.persistentDataPath}/{m_StartTime}/img_{m_StartTime}_{m_frameIndex - IgnoreFrameCount}.png");
                }
            }
        }
    }

    void GetSystemInfo()
    {
        DeviceInfo deviceInfo = new DeviceInfo()
        {
            UnityVersion = Application.unityVersion,
            DeviceModel = SystemInfo.deviceModel,
            BatteryLevel = SystemInfo.batteryLevel,
            DeviceName = SystemInfo.deviceName,
            DeviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier,
            GraphicsDeviceName = SystemInfo.graphicsDeviceName,
            GraphicsDeviceVendor = SystemInfo.graphicsDeviceVendor,
            GraphicsDeviceVersion = SystemInfo.graphicsDeviceVersion,
            GraphicsMemorySize = SystemInfo.graphicsMemorySize,
            OperatingSystem = SystemInfo.operatingSystem,
            ProcessorCount = SystemInfo.processorCount,
            ProcessorFrequency = SystemInfo.processorFrequency,
            ProcessorType = SystemInfo.processorType,
            SupportsShadows = SystemInfo.supportsShadows,
            SystemMemorySize = SystemInfo.systemMemorySize,
            ScreenHeight = Screen.height,
            ScreenWidth = Screen.width
        };

        var deviceInfoToFile = FileManager.WriteToFile(deviceFilePath, JsonUtility.ToJson(deviceInfo));
        if (deviceInfoToFile)
        {
            UploadFile(deviceFilePath);
        }
    }
}

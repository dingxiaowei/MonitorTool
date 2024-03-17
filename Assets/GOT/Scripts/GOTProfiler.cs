using MonitorLib.GOT;
using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class GOTProfiler : MonoBehaviour
{
    [Header("是否监控日志")]
    public bool EnableLog = false;
    [Header("是否采集帧图")]
    public bool EnableFrameTexture = false;
    [Header("是否采集函数性能,统计之前需要先点击菜单栏Hook/所有函数性能分析")]
    public bool EnableFunctionAnalysis = false;
    [Header("是否采集手机功耗信息")]
    public bool EnableMobileConsumptionInfo = true;
    [Header("忽略前面的帧数")]
    public int IgnoreFrameCount = 5;
    [Header("是否使用二进制文件(否就是使用txt)")]
    public bool UseBinary = false;

    public Text UploadTips;
    public Text ReportUrl;
    int m_FPS = 0;
    int m_TickTime = 0;
    string m_StartTime = "";
    float m_Accumulator = 0;
    int m_Frames = 0;
    float m_TimeLeft;
    float m_UpdateInterval = 0.5f;
    bool btnMonitor = false;
    string btnMsg = "开始监控";
    int m_frameIndex = 0;
    Action<bool> MonitorCallback;
    MonitorInfos monitorInfos = null;
    FrameRateInfos frameRateInfos = null;
    //函数性能分析
    string funcAnalysisFilePath;
    //log日志路径
    string logFilePath;
    //设备信息路径
    string deviceFilePath;
    //功耗信息路径
    string powerConsumeFilePath;
    string frameRateFilePath;
    //测试信息路径
    string testFilePath;
    //性能监控
    string monitorFilePath;
    //文件后缀类型
    string fileExt;

    string FTPHost = "ftp://127.0.0.1:2121/";

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
                fileExt = UseBinary ? ConstString.BinaryExt : ConstString.TextExt;
                Debug.Log(ConstString.Monitoring);
                m_frameIndex = 0;
                ShareDatas.StartTime = DateTime.Now; //当前时间
                m_StartTime = ShareDatas.StartTime.ToString().Replace(" ", "_").Replace("/", "_").Replace(":", "_");
                ShareDatas.StartTimeStr = m_StartTime;
#if UNITY_EDITOR
                PlayerPrefs.SetString("TestTime", m_StartTime);
                PlayerPrefs.Save();
#endif
                if (EnableFrameTexture)
                {
                    FileManager.CreateDir($"{Application.persistentDataPath}/{m_StartTime}/");
                }
                if (EnableFunctionAnalysis)
                    funcAnalysisFilePath = $"{Application.persistentDataPath}/{ConstString.FuncAnalysisPrefix}{m_StartTime}{fileExt}";
                if (EnableLog)
                    logFilePath = $"{Application.persistentDataPath}/{ConstString.LogPrefix}{m_StartTime}{fileExt}";
                deviceFilePath = $"{Application.persistentDataPath}/{ConstString.DevicePrefix}{m_StartTime}{fileExt}";
                testFilePath = $"{Application.persistentDataPath}/{ConstString.TestPrefix}{m_StartTime}{fileExt}";
                monitorFilePath = $"{Application.persistentDataPath}/{ConstString.MonitorPrefix}{m_StartTime}{fileExt}";
                frameRateFilePath = $"{Application.persistentDataPath}/frameRate_{m_StartTime}{fileExt}";
                if (EnableMobileConsumptionInfo)
                    powerConsumeFilePath = $"{Application.persistentDataPath}/{ConstString.PowerConsumePrefix}{m_StartTime}{fileExt}";
                if (EnableLog)
                {
                    LogManager.CreateLogFile(logFilePath, System.IO.FileMode.Append);
                    Application.logMessageReceived += LogManager.LogToFile;
                }

                m_TickTime = 0;
                InvokeRepeating("Tick", 1.0f, 1.0f);

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
                Debug.Log(ConstString.MonitorStop);
                ShareDatas.EndTime = DateTime.Now;
                //上传测试时间
                UploadTestInfo();
                //写入设备信息
                GetSystemInfo();

                CancelInvoke("Tick");
                m_TickTime = 0;

                MonitorInfosReport();
                FuncAnalysisReport();

                FrameRateReport();

                if (EnableLog)
                {
                    Application.logMessageReceived -= LogManager.LogToFile;
                    LogManager.CloseLogFile();
                }

                //#if UNITY_EDITOR
                //                if (EnableFunctionAnalysis)
                //                {
                //                    HookUtil.PrintMethodDatas();
                //                }
                //#endif
                if (EnableLog)
                {
                    //FileManager.ReplaceContent(logFilePath, "[Log]", "<font color=\"#0000FF\">[Log]</font>");
                    //FileManager.ReplaceContent(logFilePath, "[Error]", "<font color=\"#FF0000\">[Error]</font>");
                    //FileManager.ReplaceContent(logFilePath, "[Warning]", "<font color=\"#FFD700\">[Warning]</font>");
                    UploadFile(logFilePath);
                }

                var ReportRecordUpdateRequestUrl = "http://127.0.0.1:8083/ReceiveDataHandler.ashx?PackageName={0}&TestTime={1}";
                //Config.IP = "127.0.0.1";
                var reportUrl = "http://127.0.0.1/report/{0}.html";


                HttpGet(string.Format(ReportRecordUpdateRequestUrl, Application.identifier, m_StartTime), (result) =>
                 {
                     if (result)
                     {
                         if (ReportUrl != null)
                         {
                             ReportUrl.gameObject.SetActive(true);
                             var url = string.Format(reportUrl, m_StartTime);
                             //ReportUrl.text = $"<a href={url}>[{url}]</a>"; //TODO:修改成动态网页的连接
                             ReportUrl.text = $"<a href=http://127.0.0.1:8083/Default.aspx>[http://127.0.0.1:8083/Default.aspx]</a>";
                         }
                         Debug.Log("文件上传完毕");
                     }
                 });
            }
        };
    }

    public void HttpGet(string url, Action<bool> callback)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        StartCoroutine(GetUrl(unityWebRequest, callback));
    }

    private IEnumerator GetUrl(UnityWebRequest unityWebRequest, Action<bool> callback)
    {
        yield return unityWebRequest.SendWebRequest();
#if UNITY_2020
        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            var res = unityWebRequest.downloadHandler.text;
            if (res.Equals("success"))
            {
                callback.Invoke(true);
                Debug.Log("http get请求存档成功");
            }
            else
            {
                Debug.LogError("http get请求存档失败");
                callback.Invoke(false);
            }
        }
#else
        if (unityWebRequest.isDone)
        {
            if (string.IsNullOrEmpty(unityWebRequest.error))
            {
                var res = unityWebRequest.downloadHandler.text;
                if (res.Equals("success"))
                {
                    callback.Invoke(true);
                    Debug.Log("http get请求存档成功");
                }
                else
                {
                    Debug.LogError("http get请求存档失败,服务器返回异常");
                    callback.Invoke(false);
                }
            }
            else
            {
                Debug.LogError("http get请求存档失败");
                callback.Invoke(false);
            }
        }
#endif
        else
        {
            Debug.LogError(unityWebRequest.error);
        }
    }

    void UploadTestInfo()
    {
        TestInfo testInfo = new TestInfo()
        {
            ProductName = Application.productName,
            PackageName = Application.identifier,
            Platform = Application.platform.ToString(),
            Version = Application.version,
            TestTime = ShareDatas.GetTestTime()
        };
        //FileManager.WriteToFile(testFilePath, $"应用名：{Application.productName}&nbsp&nbsp&nbsp包名：{Application.identifier}&nbsp&nbsp&nbsp测试系统：{Application.platform}&nbsp&nbsp&nbsp版本号：{Application.version}&nbsp&nbsp&nbsp本次测试时长:{testTime}");
        bool writeRes = false;
        if (!UseBinary)
        {
            writeRes = FileManager.WriteToFile(testFilePath, JsonUtility.ToJson(testInfo));
        }
        else
        {
            writeRes = FileManager.WriteBinaryDataToFile(testFilePath, testInfo);
        }
        if (writeRes)
            UploadFile(testFilePath);
    }

    void StartMonitor()
    {
        monitorInfos = new MonitorInfos();
        frameRateInfos = new FrameRateInfos();
    }

    void FuncAnalysisReport()
    {
        if (EnableFunctionAnalysis)
        {
            HookUtil.MethodAnalysisReport(m_StartTime);
            if (File.Exists(funcAnalysisFilePath))
            {
                UploadFile(funcAnalysisFilePath);
            }
            else
            {
                Debug.LogError($"当前函数性能分析报告  {funcAnalysisFilePath}不存在");
            }
        }
    }

    void FrameRateReport()
    {
        bool writeRes = false;
        if (!UseBinary)
        {
            writeRes = FileManager.WriteToFile(frameRateFilePath, JsonUtility.ToJson(frameRateInfos));
        }
        else
        {
            writeRes = FileManager.WriteBinaryDataToFile(frameRateFilePath, frameRateInfos);
        }
        if (writeRes)
        {
            UploadFile(frameRateFilePath);
        }
    }

    void MonitorInfosReport()
    {
        if (monitorInfos.MonitorInfoList.Count > 1)
        {
            monitorInfos.MonitorInfoList.RemoveAt(monitorInfos.MonitorInfoList.Count - 1);
        }
        bool writeRes = false;
        if (!UseBinary)
        {
            writeRes = FileManager.WriteToFile(monitorFilePath, JsonUtility.ToJson(monitorInfos));
        }
        else
        {
            writeRes = FileManager.WriteBinaryDataToFile(monitorFilePath, monitorInfos);
        }
        if (writeRes)
        {
            UploadFile(monitorFilePath);
        }
    }

    void Tick()
    {
        m_TickTime++;
    }

    void UploadFileFunc(string filePath, Action<object, UploadProgressChangedEventArgs> OnFileUploadProgressChanged, Action<object, UploadFileCompletedEventArgs> OnFileUploadCompleted)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            WebClient webClient = new WebClient();
            Uri address = new Uri(FTPHost + new FileInfo(filePath).Name);
            webClient.UploadProgressChanged += OnFileUploadProgressChanged.Invoke;
            webClient.UploadFileCompleted += OnFileUploadCompleted.Invoke;
            webClient.UploadFileAsync(address, "STOR", filePath);
        }
    }

    void UploadFile(string filePath)
    {
        UploadFileFunc(filePath, (sender, e) =>
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
            btnMsg = btnMonitor ? ConstString.Monitoring : ConstString.MonitorBegin;
            if (MonitorCallback != null)
                MonitorCallback.Invoke(btnMonitor);
        }
        if (btnMonitor)
            btnMsg = $"{ConstString.Monitoring}{m_TickTime}s";
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
                var frameInfo = new FrameRate() { FrameIndex = m_frameIndex - IgnoreFrameCount, Frame = m_FPS };
                frameRateInfos.FrameRateList.Add(frameInfo);
                if (EnableFrameTexture)
                {
                    ScreenCapture.CaptureScreenshot($"{Application.persistentDataPath}/{m_StartTime}/img_{m_StartTime}_{m_frameIndex - IgnoreFrameCount}.png");
                }

                if (m_frameIndex % 1000 == 0)
                {
                    if (EnableMobileConsumptionInfo)
                        GetPowerConsume();
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
        bool writeRes = false;
        if (!UseBinary)
        {
            writeRes = FileManager.WriteToFile(deviceFilePath, JsonUtility.ToJson(deviceInfo));
        }
        else
        {
            writeRes = FileManager.WriteBinaryDataToFile(deviceFilePath, deviceInfo);
        }
        if (writeRes)
        {
            UploadFile(deviceFilePath);
        }
    }

    /// <summary>
    /// 获取功耗参数
    /// </summary>
    void GetPowerConsume()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("GetPowerConsume");
        UnityAndroidProxy unityAndroidProxy = new UnityAndroidProxy();
        unityAndroidProxy.Init();
        DevicePowerConsumeInfo devicePowerConsumeInfo = unityAndroidProxy.GetPowerConsumeInfo();
        Debug.Log($"获取安卓功耗参数:{devicePowerConsumeInfo.ToString()}");
        bool writeRes = false;
        if (!UseBinary)
        {
            writeRes = FileManager.WriteToFile(powerConsumeFilePath, JsonUtility.ToJson(devicePowerConsumeInfo));
        }
        else
        {
            writeRes = FileManager.WriteBinaryDataToFile(powerConsumeFilePath, devicePowerConsumeInfo);
        }
        if (writeRes)
        {
            UploadFile(powerConsumeFilePath);
        }
#endif
    }
}

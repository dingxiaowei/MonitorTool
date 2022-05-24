using MonitorLib.GOT;
using System;
using System.Collections;
using System.IO;
using Unity.Profiling;
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
    [Header("是否统计内存资源分布信息")]
    public bool EnableResMemoryDistributionInfo = true;
#if UNITY_2020_1_OR_NEWER
    [Header("是否采集渲染数据")]
    public bool EnableRenderInfo = true;
#endif
    [Header("采集手机功耗间隔帧")]
    [Range(10, 1000)]
    public int IntervalFrame = 100;
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
#if UNITY_2020_1_OR_NEWER
    RenderInfos renderInfos = null;
#endif
    //设备功耗采集记录
    DevicePowerConsumeInfos devicePowerConsumeInfos = null;
    /// <summary>
    /// 资源内存分布
    /// </summary>
    RecoreResInfos recordResInfos = null;
    //函数性能分析
    string funcAnalysisFilePath;
    //log日志路径
    string logFilePath;
    //设备信息路径
    string deviceFilePath;
    //功耗信息路径
    string powerConsumeFilePath;
    //截图信息路径
    string captureFilePath;
    //测试信息路径
    string testFilePath;
    //性能监控
    string monitorFilePath;
    //内存分布
    string resMemoryDistributionPath;
#if UNITY_2020_1_OR_NEWER
    //渲染信息
    string renderFilePath;
#endif
    //文件后缀类型
    string fileExt;

#if UNITY_2020_1_OR_NEWER
    private ProfilerRecorder setPassCallRecord;
    private ProfilerRecorder drawCallRecord;//dc技术
    private ProfilerRecorder verticesRecord;//顶点数
    private ProfilerRecorder trianglesRecord;//三角面
    //private ProfilerRecorder gcMemoryRecord;//gc
    //private ProfilerRecorder mainThreadTimeRecord;
#endif

    private UnityAndroidProxy unityAndroidProxy = null;
    void Awake()
    {
        Application.targetFrameRate = 60;

#if UNITY_2020_1_OR_NEWER
        if (EnableRenderInfo)
        {
            setPassCallRecord = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            drawCallRecord = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            verticesRecord = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
            trianglesRecord = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        }
#endif
    }

    void MonitorCallBackFunc(bool res)
    {
        if (res)
        {
            fileExt = UseBinary ? ConstString.BinaryExt : ConstString.TextExt;
            Debug.Log(ConstString.Monitoring);
            m_frameIndex = 0;
            ShareDatas.StartTime = DateTime.Now; //当前时间
            m_StartTime = ShareDatas.StartTime.ToString("yyyy_MM_dd_HH_mm_ss");
            ShareDatas.StartTimeStr = m_StartTime;
#if UNITY_EDITOR
            PlayerPrefs.SetString("TestTime", m_StartTime);
            PlayerPrefs.Save();
#endif      
            if (EnableFrameTexture)
            {
                captureFilePath = $"{Application.persistentDataPath}/{ConstString.CaptureFramePrefix}{m_StartTime}";
                FileManager.CreateDir(captureFilePath);
            }
            if (EnableFunctionAnalysis)
                funcAnalysisFilePath = $"{Application.persistentDataPath}/{ConstString.FuncAnalysisPrefix}{m_StartTime}{fileExt}";
            if (EnableLog)
                logFilePath = $"{Application.persistentDataPath}/{ConstString.LogPrefix}{m_StartTime}{fileExt}";
            deviceFilePath = $"{Application.persistentDataPath}/{ConstString.DevicePrefix}{m_StartTime}{fileExt}";
            testFilePath = $"{Application.persistentDataPath}/{ConstString.TestPrefix}{m_StartTime}{fileExt}";
            monitorFilePath = $"{Application.persistentDataPath}/{ConstString.MonitorPrefix}{m_StartTime}{fileExt}";
#if UNITY_ANDROID && !UNITY_EDITOR
            if (EnableMobileConsumptionInfo)
                powerConsumeFilePath = $"{Application.persistentDataPath}/{ConstString.PowerConsumePrefix}{m_StartTime}{fileExt}";
#endif
            if (EnableResMemoryDistributionInfo)
                resMemoryDistributionPath = $"{Application.persistentDataPath}/{ConstString.ResMemoryDistributionPrefix}{m_StartTime}{fileExt}";
#if UNITY_2020_1_OR_NEWER
            if (EnableRenderInfo)
                renderFilePath = $"{Application.persistentDataPath}/{ConstString.RenderPrefix}{m_StartTime}{fileExt}";
#endif
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
#if UNITY_2020_1_OR_NEWER
            if (EnableRenderInfo)
                RenderInfosReport();
#endif
            if (EnableResMemoryDistributionInfo)
                ResMemoryReport();
            if (EnableFunctionAnalysis)
                FuncAnalysisReport();
#if UNITY_ANDROID && !UNITY_EDITOR
            if (EnableMobileConsumptionInfo) //上报手机数据
                MobileConsumptionInfoReport();
#endif
            if (EnableFrameTexture)
                ZipCaptureFiles();

            if (EnableLog)
            {
                Application.logMessageReceived -= LogManager.LogToFile;
                LogManager.CloseLogFile();
            }

            if (EnableLog)
            {
                UploadFile(logFilePath);
            }

            HttpGet(string.Format(Config.ReportRecordUpdateRequestUrl, Application.identifier, m_StartTime), (result) =>
            {
                if (result)
                {
                    if (ReportUrl != null)
                    {
                        ReportUrl.gameObject.SetActive(true);
                        ReportUrl.text = $"<a href={Config.ReportUrl}>{Config.ReportUrl}</a>";
                    }
                }
            });
        }
    }

    [FunctionAnalysis]
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        MonitorCallback += MonitorCallBackFunc;
    }

    [FunctionAnalysis]
    public void HttpGet(string url, Action<bool> callback)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        StartCoroutine(GetUrl(unityWebRequest, callback));
    }

    [FunctionAnalysis]
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
            TestTime = ShareDatas.GetTestTime(),
            IntervalFrame = this.IntervalFrame
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
        renderInfos = new RenderInfos();
        devicePowerConsumeInfos = new DevicePowerConsumeInfos();
        recordResInfos = new RecoreResInfos();
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    void MobileConsumptionInfoReport()
    {
        bool writeRes = false;
        if (!UseBinary)
        {
            writeRes = FileManager.WriteToFile(powerConsumeFilePath, JsonUtility.ToJson(devicePowerConsumeInfos));
        }
        else
        {
            writeRes = FileManager.WriteBinaryDataToFile(powerConsumeFilePath, devicePowerConsumeInfos);
        }
        if (writeRes)
        {
            UploadFile(powerConsumeFilePath);
        }
    }
#endif

    void FuncAnalysisReport()
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

    void ResMemoryReport()
    {
        bool writeRes = false;
        if (!UseBinary)
        {
            writeRes = FileManager.WriteToFile(resMemoryDistributionPath, JsonUtility.ToJson(recordResInfos));
        }
        else
        {
            writeRes = FileManager.WriteBinaryDataToFile(resMemoryDistributionPath, recordResInfos);
        }
        if (writeRes)
        {
            UploadFile(resMemoryDistributionPath);
        }
    }

#if UNITY_2020_1_OR_NEWER
    void RenderInfosReport()
    {
        bool writeRes = false;
        if (!UseBinary)
        {
            writeRes = FileManager.WriteToFile(renderFilePath, JsonUtility.ToJson(renderInfos));
        }
        else
        {
            writeRes = FileManager.WriteBinaryDataToFile(renderFilePath, renderInfos);
        }
        if (writeRes)
        {
            UploadFile(renderFilePath);
        }
    }
#endif

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

    void UploadFile(string filePath)
    {
        FileFTPUploadManager.UploadFile(filePath, (sender, e) =>
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
                var relativeIndex = m_frameIndex - IgnoreFrameCount;
                var monitorInfo = new MonitorInfo() { FrameIndex = relativeIndex, BatteryLevel = SystemInfo.batteryLevel, MemorySize = 0, Frame = m_FPS, MonoHeapSize = Profiler.GetMonoHeapSizeLong(), MonoUsedSize = Profiler.GetMonoUsedSizeLong(), TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong(), TotalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong(), UnityTotalReservedMemory = Profiler.GetTotalReservedMemoryLong(), AllocatedMemoryForGraphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver() };
                monitorInfos.MonitorInfoList.Add(monitorInfo);
                if ((m_frameIndex - IgnoreFrameCount) % IntervalFrame == 0)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    if (EnableMobileConsumptionInfo)
                        GetPowerConsume(relativeIndex);
#endif
                    if (EnableResMemoryDistributionInfo)
                        GetResMemoryInfo(relativeIndex);
                    if (EnableFrameTexture)
                        ScreenCapture.CaptureScreenshot($"{captureFilePath}/img_{m_StartTime}_{relativeIndex}.png");
#if UNITY_2020_1_OR_NEWER
                    if (EnableRenderInfo)
                        GetRenderInfo(relativeIndex);
#endif
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

#if UNITY_2020_1_OR_NEWER
    void GetRenderInfo(int index)
    {
        var renderInfo = new RenderInfo() { FrameIndex = index, DrawCall = drawCallRecord.LastValue, SetPassCall = setPassCallRecord.LastValue, Triangles = trianglesRecord.LastValue, Vertices = verticesRecord.LastValue };
        renderInfos.RenderInfoList.Add(renderInfo);
    }
#endif

    void GetResMemoryInfo(int index)
    {
        RecordResInfo record = new RecordResInfo();
        record.FrameIndex = index;
        var pair = CollectResFrameDatas<Texture>.TakeSample();
        record.TextureSize = pair.Key;
        record.TotalSize += pair.Key;
        record.TextureCount = pair.Value;
        record.TotalCount += pair.Value;
        pair = CollectResFrameDatas<Mesh>.TakeSample();
        record.MeshSize = pair.Key;
        record.TotalSize += pair.Key;
        record.MeshCount = pair.Value;
        record.TotalCount += pair.Value;
        pair = CollectResFrameDatas<Material>.TakeSample();
        record.MaterialSize = pair.Key;
        record.TotalSize += pair.Key;
        record.MaterialCount = pair.Value;
        record.TotalCount += pair.Value;
        pair = CollectResFrameDatas<Shader>.TakeSample();
        record.ShaderSize = pair.Key;
        record.TotalSize += pair.Key;
        record.ShaderCount = pair.Value;
        record.TotalCount += pair.Value;
        pair = CollectResFrameDatas<AnimationClip>.TakeSample();
        record.AnimationClipSize = pair.Key;
        record.TotalSize += pair.Key;
        record.AnimationClipCount = pair.Value;
        record.TotalCount += pair.Value;
        pair = CollectResFrameDatas<AudioClip>.TakeSample();
        record.AudioClipSize = pair.Key;
        record.TotalSize += pair.Key;
        record.AudioClipCount = pair.Value;
        record.TotalCount += pair.Value;
        pair = CollectResFrameDatas<Texture>.TakeSample();
        record.TextureSize = pair.Key;
        record.TotalSize += pair.Key;
        record.TextureCount = pair.Value;
        record.TotalCount += pair.Value;
        pair = CollectResFrameDatas<Font>.TakeSample();
        record.FontSize = pair.Key;
        record.TotalSize += pair.Key;
        record.FontCount = pair.Value;
        record.TotalCount += pair.Value;
        pair = CollectResFrameDatas<TextAsset>.TakeSample();
        record.TextAssetSize = pair.Key;
        record.TotalSize += pair.Key;
        record.TextAssetCount = pair.Value;
        record.TotalCount += pair.Value;
        pair = CollectResFrameDatas<ScriptableObject>.TakeSample();
        record.ScriptableObjectSize = pair.Key;
        record.TotalSize += pair.Key;
        record.ScriptableObjectCount = pair.Value;
        record.TotalCount += pair.Value;
        recordResInfos.RecordResInfosList.Add(record);
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    /// <summary>
    /// 获取功耗参数
    /// </summary>
    void GetPowerConsume(int index)
    {
        Debug.Log("GetPowerConsume");
        unityAndroidProxy ??= new UnityAndroidProxy();
        DevicePowerConsumeInfo devicePowerConsumeInfo = unityAndroidProxy.GetPowerConsumeInfo(index);
        //Debug.Log($"获取安卓功耗参数:{devicePowerConsumeInfo.ToString()}");
        devicePowerConsumeInfos.devicePowerConsumeInfos.Add(devicePowerConsumeInfo);
    }
#endif

    /// <summary>
    /// 压缩采集帧并上传
    /// </summary>
    private void ZipCaptureFiles()
    {
        string srcFileDir = captureFilePath;
        string zipFilePath = captureFilePath + ".zip";
        bool zipSuccess = ZipUtils.ZipFile(srcFileDir, zipFilePath);
        Debug.Log($"采集帧压缩完成:{zipFilePath}");

        if (zipSuccess)
        {
            UploadFile(zipFilePath);
            Debug.Log("压缩成功后上传");
        }
        else
        {
            Debug.LogError("压缩文件失败!");
        }
    }

    private void OnDestroy()
    {
        MonitorCallback -= MonitorCallBackFunc;
    }

    void OnDisable()
    {
        if (EnableRenderInfo)
        {
            setPassCallRecord.Dispose();
            drawCallRecord.Dispose();
            verticesRecord.Dispose();
            trianglesRecord.Dispose();
        }
    }
}

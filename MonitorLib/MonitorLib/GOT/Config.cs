using System.Collections.Generic;

namespace MonitorLib.GOT
{
    public class Config
    {
        public static string IP = "116.205.247.142";
        public static string ReportUrl = "http://116.205.247.142:8080";
        public static string ReportRecordUpdateRequestUrl = "http://116.205.247.142:8080/ReceiveDataHandler.ashx?PackageName={0}&TestTime={1}";
        public static string PostFileUrl = "https://apigw-cn-south.huawei.com/api/cybersim/performance/unity/v1/energy/addUnityPerformanceInfo";
        //public static string PostFileUrl = "http://116.205.247.142:8080/TestHandler.ashx";
        public static Dictionary<string, string> PostFileHeaders = new Dictionary<string, string>() { { "X-HW-ID", "com.huawei.xr.cyberverse.cybersim" }, { "X-HW-APPKEY", "bA2J8D1u9djyOVtS8efNTQ==" } };
    }

    public class ConstString
    {
        //配置语言
        public const string MonitorBegin = "开始监控";
        public const string Monitoring = "监控中";
        public const string MonitorStop = "停止监控";
        public const string BinaryExt = ".data";
        public const string TextExt = ".txt";

        //文件前缀
        public const string FrameRatefix = "frameRate_";
        public const string LogPrefix = "log_";
        public const string DevicePrefix = "device_";
        public const string TestPrefix = "test_";
        public const string MonitorPrefix = "monitor_";
        //函数性能分析
        public const string FuncAnalysisPrefix = "funcAnalysis_";
        //函数规划范分析
        public const string FuncCodeAnalysisPrefix = "funcCodeAnalysis_";
        public const string CPUTemperaturePrefix = "cpuTemperature_";
        //功耗模块
        public const string PowerConsumePrefix = "powerConsume_";

        //采集帧率
        public const string CaptureFramePrefix = "captureFrame_";
        //内存分布
        public const string ResMemoryDistributionPrefix = "resMemoryDistribution_";
        //渲染信息
        public const string RenderPrefix = "renderInfo_";
        public const string PssMemoryPrefix = "pssMemoryInfo_";
    }
}

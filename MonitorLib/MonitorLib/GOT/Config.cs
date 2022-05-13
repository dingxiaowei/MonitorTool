namespace MonitorLib.GOT
{
    public class Config
    {
        public static string IP = "124.223.54.98";
        public static string ReportUrl = "http://124.223.54.98:888/Index.aspx";
        public static string ReportRecordUpdateRequestUrl = "http://124.223.54.98:888/ReceiveDataHandler.ashx?PackageName={0}&TestTime={1}";
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
    }
}

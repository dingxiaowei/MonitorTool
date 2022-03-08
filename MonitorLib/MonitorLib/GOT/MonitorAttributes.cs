using System;

namespace MonitorLib.GOT
{
    public class HideAnalysisAttribute : Attribute
    {
    }

    public class TestInjectAttribute : Attribute
    {
    }

    public class ProfilerSampleAttribute : Attribute
    {
    }

    /// <summary>
    /// 自定义Profiler.BeginSample命名
    /// </summary>
    public class ProfilerSampleWithDefineNameAttribute : Attribute
    {
        public string Param1 { get; set; }
        public ProfilerSampleWithDefineNameAttribute(string beginFuncName)
        {
            this.Param1 = beginFuncName;
        }
    }

    public class FunctionAnalysisAttribute : Attribute
    {
    }
}

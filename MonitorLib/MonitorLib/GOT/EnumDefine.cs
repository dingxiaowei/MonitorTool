namespace MonitorLib.GOT
{
    /// <summary>
    /// 函数分析类型
    /// </summary>
    public enum EAnalyzeType
    {
        /// <summary>
        /// 所有的函数检测
        /// </summary>
        ALLFUNC = 0,
        /// <summary>
        /// 自定义函数检测
        /// </summary>
        DEFINEFUNC,
        /// <summary>
        /// Profile Sample
        /// </summary>
        PROFILESAMPLE
    }

    /// <summary>
    /// 带有Begin和End函数的注入
    /// </summary>
    public enum EBeginEndType
    {
        /// <summary>
        /// 打log
        /// </summary>
        LOG = 1,
        /// <summary>
        /// 性能分析
        /// </summary>
        ANALYZE
    }
}

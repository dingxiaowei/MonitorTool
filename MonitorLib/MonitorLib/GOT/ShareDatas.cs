using System;

namespace MonitorLib.GOT
{
    public class ShareDatas
    {
        public static DateTime StartTime;
        public static DateTime EndTime;
        public static string StartTimeStr;
        public static bool WriteLogToFile = true;
        public static bool ShowDebugLog = true;

        public static string ReportUrl = $"http://{Config.IP}/report_" + "{0}.html";

        /// <summary>
        /// 获取测试时长
        /// </summary>
        /// <returns></returns>
        public static string GetTestTime()
        {
            string str = "";
            var ts = EndTime - StartTime;
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString() + "小时 " + ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = ts.Seconds + "秒";
            }
            return str;
        }
    }
}

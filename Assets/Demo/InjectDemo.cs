using MonitorLib.GOT;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace TestModule
{
    public class Normal
    {
        public static int GetMax(int a, int b)
        {
            Debug.LogFormat("a = {0}, b = {1}", a, b);
            return a > b ? a : b;
        }
    }

    [TestInject]
    public class Inject
    {
        public static int GetMax(int a, int b)
        {
            return a;
        }
    }

    public class InjectDemo : MonoBehaviour
    {
        public Button btn_ShowFuncAnalysicClick;

        [ProfilerSample]
        void Start()
        {
            Debug.LogFormat("Normal Max: {0}", Normal.GetMax(6, 9));
            Debug.LogFormat("Inject Max: {0}", Inject.GetMax(6, 9));
            //for (int i = 0; i < 3; i++)
            Test();
            for (int i = 0; i < 3; i++)
                TestDefine();

            if (btn_ShowFuncAnalysicClick != null)
            {
                btn_ShowFuncAnalysicClick.onClick.AddListener(() =>
                {
#if ENABLE_ANALYSIS
                    HookUtil.PrintProfilerDatas();

                    var datas = HookUtil.GetFunctionMonitorFileDatas();
                    if (datas != null && datas.Count > 0)
                    {
                        Debug.Log("--------输出所有函数的性能数据-------");
                        foreach (var data in datas)
                        {
                            Debug.Log(data);
                        }
                        FunctionAnalysisDatas funcAnalysisData = new FunctionAnalysisDatas();
                        funcAnalysisData.FunctionAnalysDatas = new System.Collections.Generic.List<FunctionMonitorFileDatas>();
                        funcAnalysisData.FunctionAnalysDatas.AddRange(datas);
                        var datasJsonStr = JsonUtility.ToJson(funcAnalysisData);
                        Debug.Log(datasJsonStr);
                        EmailManager.Send(datasJsonStr);
                        var funcAnalysisFile = FileManager.WriteToFile($"{Application.persistentDataPath}/a.txt", datasJsonStr);
                        if (funcAnalysisFile)
                        {
                            //UploadFile(funcAnalysisFilePath);
                        }
                    }
                    else
                    {
                        Debug.Log("--------没有函数性能监控数据---------");
                    }
                    ////测试发送邮件
                    //var datas = HookUtil.GetFunctionMonitorFileDatas();
                    //var jsonDatas = JsonUtility.ToJson(datas);
                    //EmailManager.Send(jsonDatas);
#endif
                });
            }
        }

        [FunctionAnalysis]
        [ProfilerSample]
        public void Test()
        {
            Debug.Log("开始循环100次");
            for (int i = 0; i < 100; i++)
            {
                Debug.Log(i);
            }
            Debug.Log("结束循环100次");
        }
        //[ProfilerSampleWithDefineName("-------自定义Sample命名")]
        [FunctionAnalysis]
        [ProfilerSample]
        public void TestDefine()
        {
            Profiler.BeginSample("****************");
            Debug.Log("检测带有特性的方法");
            Profiler.EndSample();
        }

        [HideAnalysis] //不需要分析的函数
        private void OnGUI()
        {

        }
    }
}
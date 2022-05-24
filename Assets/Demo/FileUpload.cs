using MonitorLib.GOT;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FileUpload : MonoBehaviour
{
    private void Start()
    {
        var uploadManager = gameObject.AddComponent<FileUploadManager>();
        uploadManager.UploadFiles(Config.PostFileHeaders, new System.Collections.Generic.Dictionary<string, string>() { { "testcase_name", "testaladdin1" } }, "folder", new System.Collections.Generic.List<string>() { { "D:/captureFrame_2022_5_1_23_10_50.zip" } }, (res, errorInfo) =>
        {
            Debug.Log($"传输结果:{res}  error:{errorInfo}");
        });
    }
}

/*
public class FileUpload : MonoBehaviour
{
    string url = "https://apigw-cn-south.huawei.com/api/cybersim/performance/unity/v1/energy/addUnityPerformanceInfo";
    //string url = "http://124.223.54.98:888/TestHandler.ashx";
    //string url = "http://192.168.0.110:8083/TestHandler.ashx";
    //public Button button_Test;//测试btn
    //public Text text;//提示txt

    void Start()
    {
        //button_Test.onClick.AddListener(To_FileUpload);
        //text.text = "";
        To_FileUpload();
    }

    private void To_FileUpload()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("testcase_name", "testaladdin1");
        wWWForm.AddBinaryData("folder", GetStreamBytes("D:/resMemoryDistribution_2022_5_1_23_10_50.txt"));
        //wWWForm.AddBinaryData("folder", GetStreamBytes("D:/captureFrame_2022_5_1_23_10_50.zip"));

        StartCoroutine(UploadResult(url, wWWForm));
    }

    //byte[] GetStreamBytes(string filePath)
    //{
    //    return File.ReadAllBytes(filePath);
    //}

    //读取文件转byte[]
    public byte[] GetStreamBytes(string path)
    {
        try
        {
            FileStream stream = new FileInfo(path).OpenRead();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            stream.Close();
            stream.Dispose();
            return buffer;
        }
        catch (Exception ex)
        {
            Debug.LogError("错误：" + ex.Message);
        }
        return null;
    }
    //文件上传
    public IEnumerator UploadResult(string URL, WWWForm wWWForm)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(URL, wWWForm))
        {
            webRequest.SetRequestHeader("X-HW-ID", "com.huawei.xr.cyberverse.cybersim");
            webRequest.SetRequestHeader("X-HW-APPKEY", "bA2J8D1u9djyOVtS8efNTQ==");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.LogError("上传成功");
                //text.text = "上传成功";
            }
            else
            {
                Debug.LogError("上传失败:" + " error:" + webRequest.error + " result:" + webRequest.result.ToString());
                //text.text = "上传失败:" + " error:" + webRequest.error + " result:" + webRequest.result.ToString();
            }
        }
    }
}
*/
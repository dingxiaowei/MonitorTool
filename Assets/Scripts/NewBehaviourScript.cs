using CSVTool;
//using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        var bytes = Resources.Load<TextAsset>("avatarkneadface");
        if (bytes == null)
        {
            return;
        }
        IBinarySerializable newavList = new avatarkneadfaceConfig();
        //创建Stopwatch实例
        Stopwatch sw = new Stopwatch();
        //开始计时
        sw.Start();
        for (int i = 0; i < 100; i++)
        {
            var readOK = FileManager.ReadBinaryDataFromBytes(bytes.bytes, ref newavList);
            if (readOK)
            {
                //查询
                var avatarguideTest = (avatarguideTestConfig)newavList;
                var obj = avatarguideTest.QueryById(2);
                if (obj != null && obj.Count() > 0)
                    UnityEngine.Debug.Log(obj.FirstOrDefault().ToString());
            }
            else
            {
                UnityEngine.Debug.LogError("读取失败");
            }
        }
        sw.Stop();
        UnityEngine.Debug.Log($"二进制读取时间 {sw.ElapsedMilliseconds}");
        //var jsonStr = Resources.Load<TextAsset>("avatarkneadface.json").ToString();
        sw.Reset();
        TextAsset asset = Resources.Load<TextAsset>("avatarkneadface.json");
        if (asset == null)
        {
            return;
        }
        //sw.Start();
        //for (int i = 0; i < 100; i++)
        //{
        //    var items = JsonConvert.DeserializeObject<avatarkneadface[]>(LitJson.JsonMapper.ToObject(asset.ToString())["Items"].ToJson());

        //}
        //sw.Stop();
        //UnityEngine.Debug.Log($"二进制读取时间 {sw.ElapsedMilliseconds}");
    }
}

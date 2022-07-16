using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSVTool
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public class FileManager
    {
        public static bool CreateDir(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                return false;
            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, true);
            }
            Directory.CreateDirectory(dirPath);
            return true;
        }

        /// <summary>
        /// 将数据写入二进制文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data">继承自IBinarySerialize的数据</param>
        public static bool WriteBinaryDataToFile(string filePath, IBinarySerializable data)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                using (var bw = new BinaryWriter(fileStream))
                {
                    data.Serialize(bw);
                    bw.Flush();
                    bw.Close();
                }
                fileStream.Close();
            }
            return true;
        }

        /// <summary>
        /// 将数据写入二进制文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="datas">类型(小写)和value的字符串键值对</param>
        /// <returns></returns>
        public static bool WriteBinaryDatasToFile(string filePath, List<Tuple<string, string>> datas)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    using (var bw = new BinaryWriter(fileStream))
                    {
                        foreach (var data in datas)
                        {
                            if (data.Item1.Equals("int"))
                            {
                                if (string.IsNullOrEmpty(data.Item2))
                                {
                                    bw.Write(Convert.ToInt32(0));
                                }
                                else
                                {
                                    bw.Write(Convert.ToInt32(data.Item2));
                                }
                            }
                            else if (data.Item1.Equals("bool"))
                            {
                                if (string.IsNullOrEmpty(data.Item2))
                                {
                                    bw.Write(Convert.ToBoolean(false));
                                }
                                else
                                {
                                    bw.Write(Convert.ToBoolean(data.Item2));
                                }
                            }
                            else if (data.Item1.Equals("float"))
                            {
                                if (string.IsNullOrEmpty(data.Item2))
                                {
                                    bw.Write(Convert.ToSingle(0));
                                }
                                else
                                {
                                    bw.Write(Convert.ToSingle(data.Item2));
                                }
                            }
                            else if (data.Item1.Equals("double"))
                            {
                                if (string.IsNullOrEmpty(data.Item2))
                                {
                                    bw.Write(Convert.ToDouble(0));
                                }
                                else
                                {
                                    bw.Write(Convert.ToDouble(data.Item2));
                                }
                            }
                            else if (data.Item1.Equals("string"))
                            {
                                if (string.IsNullOrEmpty(data.Item2))
                                {
                                    bw.Write("");
                                }
                                else
                                {
                                    bw.Write(data.Item2.ToString());
                                }
                            }
                            else if (data.Item1.Equals("long"))
                            {
                                if (string.IsNullOrEmpty(data.Item2))
                                {
                                    bw.Write(Convert.ToInt64(0));
                                }
                                else
                                {
                                    bw.Write(Convert.ToInt64(data.Item2));
                                }
                            }
                            else if (data.Item1.Equals("vector"))
                            {
                                //[1.2,3.4,5.6]
                                var str = data.Item2.ToString();
                                if (string.IsNullOrEmpty(str))
                                {
                                    bw.Write(Convert.ToInt32(0));
                                }
                                else
                                {
                                    str = str.Replace("]", "").Replace("[", "");
                                    var numStrs = str.Split(',');
                                    int vectorCount = 3;
                                    bw.Write(vectorCount);
                                    for (int i = 0; i < vectorCount; i++)
                                    {
                                        float v = Convert.ToSingle(numStrs[i]);
                                        bw.Write(v);
                                    }
                                }
                            }
                            else if (data.Item1.Equals("list"))
                            {
                                //[[1.2,3.4,5.6],[2.2,3.4,5.6],[3.2,3.4,5.6]]
                                string str = data.Item2.ToString();
                                if (string.IsNullOrEmpty(str))
                                {
                                    bw.Write(Convert.ToInt32(0));
                                }
                                else
                                {
                                    str = str.Replace("]", "").Replace("[", "");
                                    var numStrs = str.Split(',');
                                    bw.Write(Convert.ToInt32(numStrs.Length / 3));
                                    for (int i = 0; i < numStrs.Length; i++)
                                    {
                                        if (i % 3 == 0)
                                            bw.Write(Convert.ToInt32(3));
                                        bw.Write(Convert.ToSingle(numStrs[i]));
                                    }
                                }
                            }
                            else
                            {
                                UnityEngine.Debug.LogError($"写入二进制文件，数据类型{data.Item1}没有适配");
                                return false;
                            }
                        }
                        bw.Flush();
                        bw.Close();
                    }
                    fileStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 从内存流中读取二进制
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ReadBinaryDataFromBytes(byte[] bytes, ref IBinarySerializable data)
        {
            if (bytes == null)
                return false;
            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var br = new BinaryReader(memoryStream))
                {
                    data.DeSerialize(br);
                    br.Close();
                }
                memoryStream.Close();
            }
            return true;
        }

        /// <summary>
        /// 读取二进制文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ReadBinaryDataFromFile(string filePath, ref IBinarySerializable data)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (var br = new BinaryReader(fileStream))
                {
                    data.DeSerialize(br);
                    br.Close();
                }
                fileStream.Close();
            }
            return true;
        }

        public static bool WriteBytesToFile(string filePath, byte[] data)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            var file = new FileInfo(filePath);
            using (Stream sw = file.Create())
            {
                sw.Write(data, 0, data.Length);
                sw.Flush();
                sw.Close();
            }
            return true;
        }

        /// <summary>
        /// 将字符串写入文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool WriteToFile(string filePath, string context)
        {
            return WriteToFile(filePath, context, Encoding.Default);
        }

        public static bool WriteToFile(string filePath, string context, Encoding encoding)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                var data = encoding.GetBytes(context);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
            }
            return true;
        }

        /// <summary>
        /// 按行读取
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadAllByLine(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
                sr.Close();
            }
            return sb.ToString();
        }

        public static byte[] ReadAllBytes(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return null;
            }
            return File.ReadAllBytes(path);
        }

        /// <summary>
        /// 修改文件内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="normalStr"></param>
        /// <param name="newStr"></param>
        public static void ReplaceContent(string path, string normalStr, string newStr)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return;
            }
            string strContent = File.ReadAllText(path);
            strContent = strContent.Replace(normalStr, newStr);
            File.WriteAllText(path, strContent);
        }

        /// <summary>
        /// 批量修改文件内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newStr"></param>
        /// <param name="normalStrs"></param>
        public static void ReplaceContent(string path, string newStr, params string[] normalStrs)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return;
            }
            string strContent = File.ReadAllText(path);
            for (int i = 0; i < normalStrs.Length; i++)
            {
                strContent = strContent.Replace(normalStrs[i], newStr);
            }
            File.WriteAllText(path, strContent);
        }
    }
}

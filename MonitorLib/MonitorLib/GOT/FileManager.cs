using System.IO;
using System.Text;

namespace MonitorLib.GOT
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

        public static bool WriteBytesToFile(string filePath, byte[] data)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            if(File.Exists(filePath))
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

using System;
using System.IO;
using System.Net;

namespace MonitorLib.GOT
{
    public class FileUploadManager
    {
        //备注:远程服务器需要端口映射
        public static string FTPHost = $"ftp://{Config.IP}:2121/";
        public static void UploadFile(string filePath, Action<object, UploadProgressChangedEventArgs> OnFileUploadProgressChanged, Action<object, UploadFileCompletedEventArgs> OnFileUploadCompleted)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            var client = new WebClient();
            var uri = new Uri(FTPHost + new FileInfo(filePath).Name);
            client.UploadProgressChanged += new UploadProgressChangedEventHandler(OnFileUploadProgressChanged);
            client.UploadFileCompleted += new UploadFileCompletedEventHandler(OnFileUploadCompleted);
            //client.Credentials = new System.Net.NetworkCredential(FTPUserName, FTPPassword);
            client.UploadFileAsync(uri, "STOR", filePath);
        }
    }
}

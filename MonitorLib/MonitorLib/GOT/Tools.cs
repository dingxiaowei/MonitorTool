using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace MonitorLib.GOT
{
    public class Tools
    {
        public static void EmailSend(string msg)
        {
            Task task = new Task(() =>
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("1213250243@qq.com");
                mail.To.Add("1241631510@qq.com.com");
                mail.Subject = "hola性能分析报告";
                mail.Body = msg;
                //if (File.Exists(@"D:\\2020-09-15.txt"))
                //    mail.Attachments.Add(new Attachment(@"D:\\2020-09-15.txt"));

                SmtpClient smtpServer = new SmtpClient("smtp.qq.com");
                smtpServer.Credentials = new System.Net.NetworkCredential("1213250243@qq.com", "svbfnefvcgluhfig") as ICredentialsByHost;
                smtpServer.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };

                smtpServer.Send(mail);
                Debug.Log("send email success");
            });
            task.Start();
        }

        public static bool BatRunner(string batPath, string arg)
        {
            // System.Console.InputEncoding = System.Text.Encoding.UTF8;
            using (Process proc = new Process())
            {
                proc.StartInfo.Verb = "call";
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(batPath);
                proc.StartInfo.FileName = batPath;
                if (!string.IsNullOrEmpty(arg))
                {
                    proc.StartInfo.Arguments = arg;
                }
                proc.StartInfo.UseShellExecute = false;

                // set up output redirection
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.EnableRaisingEvents = true;
                proc.StartInfo.CreateNoWindow = true;

                StringBuilder sbError = new StringBuilder();
                StringBuilder sbNormal = new StringBuilder();

                // Set the data received handlers
                proc.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        sbError.Append(e.Data + Environment.NewLine);
                    }
                };
                proc.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        sbNormal.Append(e.Data + Environment.NewLine);
                    }
                };

                proc.Start();
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                proc.WaitForExit();

                bool isError = sbError.ToString().Contains("error");
                if (proc.ExitCode == 0 && !isError)
                {
                    Debug.Log($"Success. {sbNormal}");
                    return true;
                }
                else
                {
                    Debug.Log($"Failed. {sbError}");
                    return false;
                }
            }
        }
    }
}

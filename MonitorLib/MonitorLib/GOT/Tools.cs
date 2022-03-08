using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityEngine;

namespace MonitorLib.GOT
{
    public class EmailManager
    {
        public static void Send(string msg)
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
                smtpServer.Credentials = new System.Net.NetworkCredential("1213250243@qq.com", "msogfvzadhpbhebh") as ICredentialsByHost;
                smtpServer.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };

                smtpServer.Send(mail);
                Debug.Log("send email success");
            });
            task.Start();
        }
    }
}

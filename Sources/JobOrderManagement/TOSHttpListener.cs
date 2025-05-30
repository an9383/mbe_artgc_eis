using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using KR.MBE.CommonLibrary.Handler;

namespace JobOrderManagement
{
    public class TOSHttpListener
    {

        public static TOSHttpListener m_TOSHttpListener = null;
        public static string m_httpReceiveURL = string.Empty;
        private static string m_configFileName = "httpserver.config";

        HttpListener httpListener = null;

        public TOSHttpListener()
        {
            string sFullPath = System.Environment.CurrentDirectory + @"\" + m_configFileName;
            Hashtable htConfig = Middleware.ActiveMQ.Util.ReadXml(sFullPath);

            m_httpReceiveURL = "http://+:" + htConfig["HttpReceivePort"].ToString() + "/";
            string sMethod = htConfig["HttpReceiveMethod"].ToString().Trim();
            if (!string.IsNullOrEmpty(sMethod))
            {
                m_httpReceiveURL = m_httpReceiveURL + sMethod + "/";
            }
        }

        public static TOSHttpListener This()
        {
            if (m_TOSHttpListener == null)
                m_TOSHttpListener = new TOSHttpListener();

            return m_TOSHttpListener;
        }

        public void Listen()
        {
            // 관리자 컨솔에서 아래 주소를 등록해야 한다. 
            //netsh http add urlacl url=http://+:60000/IFTOS/ user=everyone
            //netsh http delete urlacl url=http://+:60000/IFTOS/
            //netsh http show urlacl

            //String sAddress = "http://+:60000/IFTOS/";
            string sAddress = m_httpReceiveURL;
            NetAclAdd(sAddress);

            if (httpListener == null)
            {
                httpListener = new HttpListener();
                httpListener.Prefixes.Add(string.Format(sAddress));
                serverStart();
            }
        }

        private void serverStart()
        {

            if (!httpListener.IsListening)
            {
                httpListener.Start();
                Console.WriteLine("###### Starting Http Listener ######################################################################");

                Task.Factory.StartNew(() =>
                {
                    while (httpListener != null)
                    {
                        HttpListenerContext context = this.httpListener.GetContext();

                        string rawurl = context.Request.RawUrl;
                        string httpmethod = context.Request.HttpMethod;

                        string result = "";

                        //result += string.Format("httpmethod = {0}\r\n", httpmethod);
                        //result += string.Format("rawurl = {0}\r\n", rawurl);

                        // 위 2가지 를 베이스로 RestAPI 파서를 구현한다. 
                        // 기능 호출은 POST
                        // 상태 확인은 GET
                        if (context.Request.HttpMethod == HttpMethod.Post.Method)
                        {
                            // body 데이터를 json 으로 받아서 Parsing 
                            using (var reader = new StreamReader(context.Request.InputStream,
                                     context.Request.ContentEncoding))
                            {
                                result += reader.ReadToEnd();
                            }

                            ;
                            // The action is a post 
                            XmlDocument docJson = JsonConvert.DeserializeXmlNode(result);
                            string strreceiveMessage = docJson.DocumentElement.OuterXml;
                            string strMessageName = MessageHandler.getXMLResult(strreceiveMessage, "<messagename>", "</messagename>");
                            string strBody = MessageHandler.getXMLResult(strreceiveMessage, "<body>", "</body>");
                            // 받은 메세지 그대로 SendMessage 구현
                            
                            MessageHandler.SendMessageAsync(strMessageName, strBody);

                        }
                        else if (context.Request.HttpMethod == HttpMethod.Put.Method)
                        {
                            // The action is a put 
                            ;
                        }
                        else if (context.Request.HttpMethod == HttpMethod.Delete.Method)
                        {
                            // The action is a DELETE
                            ;
                        }
                        else if (context.Request.HttpMethod == HttpMethod.Get.Method)
                        {
                            // The action is a Get
                            ;
                        }


                        Console.WriteLine(result);
                        /*
                        if (txtLog.InvokeRequired)
                            txtLog.Invoke(new MethodInvoker(delegate { txtLog.AppendText(result + Environment.NewLine); }));
                        else
                            txtLog.AppendText(result + Environment.NewLine);
                        */

                        context.Response.Close();

                    }
                });

            }

        }

        public void ListenClose()
        {
            httpListener.Close();
        }


        private void NetAclAdd(string address)
        {
            //netsh http add urlacl url=http://+:60000/IFTOS/ user=everyone
            string args = string.Format(@"http add urlacl url={0} user=everyone", address);

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;

            Process.Start(psi).WaitForExit();



        }

        public static void InBoundRole()
        {
            string args = string.Format("advfirewall firewall add rule name = HTTP dir =in action = allow protocol = tcp localport = 60000");

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;

            Process.Start(psi).WaitForExit();

            //ProcessStartInfo cmd = new ProcessStartInfo();
            //Process pro = new Process();

            //cmd.FileName = "cmd";
            //cmd.WindowStyle = ProcessWindowStyle.Hidden;
            //cmd.CreateNoWindow = true;
            //cmd.UseShellExecute = false;
            //cmd.RedirectStandardOutput = true;
            //cmd.RedirectStandardInput = true;
            //cmd.RedirectStandardError = true;

            //pro.EnableRaisingEvents = false;
            //pro.StartInfo = cmd;
            //pro.Start();

            //pro.StandardInput.Write("netsh advfirewall firewall add rule name = HTTP dir =in action = allow protocol = tcp localport = 60000" + Environment.NewLine);

            //pro.StandardInput.Close();
            //pro.WaitForExit();
            //pro.Close();
        }

        public static void DeleteInboundRole()
        {

            string args = string.Format("advfirewall firewall delete rule name = HTTP");

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;

            Process.Start(psi).WaitForExit();

            //ProcessStartInfo cmd = new ProcessStartInfo();
            //Process pro = new Process();

            //cmd.FileName = "cmd";
            //cmd.WindowStyle = ProcessWindowStyle.Hidden;
            //cmd.CreateNoWindow = true;
            //cmd.UseShellExecute = false;
            //cmd.RedirectStandardOutput = true;
            //cmd.RedirectStandardInput = true;
            //cmd.RedirectStandardError = true;

            //pro.EnableRaisingEvents = false;
            //pro.StartInfo = cmd;
            //pro.Start();

            //pro.StandardInput.Write("netsh advfirewall firewall delete rule name = HTTP" + Environment.NewLine);
            
            //pro.StandardInput.Close();
            //pro.WaitForExit();
            //pro.Close();
        }
    }
}

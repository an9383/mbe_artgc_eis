using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KR.MBE.CommonLibrary.Manager
{
    /// <summary>
    /// Http Mnaager Resource Class는 실제로 사용안함.
    /// </summary>
    public class HttpManager
    {
        public static HttpManager m_HttpSender = null;
        public static string m_httpSendURL = string.Empty;
        private static string m_configFileName = "httpserver.config";

        public HttpManager()
        {
            string sFullPath = System.Environment.CurrentDirectory + @"\" + m_configFileName;
            Hashtable htConfig = Middleware.ActiveMQ.Util.ReadXml(sFullPath);

            m_httpSendURL = "http://" + htConfig["HttpSendIP"].ToString() + ":" + htConfig["HttpSendPort"].ToString() + "/";
            string sMethod = htConfig["HttpSendMethod"].ToString().Trim();
            if (!string.IsNullOrEmpty(sMethod))
            {
                m_httpSendURL = m_httpSendURL + sMethod + "/";
            }
        }

        public static HttpManager This()
        {
            if (m_HttpSender == null)
                m_HttpSender = new HttpManager();
            return m_HttpSender;
        }

        public string HttpSendData(string data)
        {

            // ActiveMQ Send URL 일때
            //string url = "http://localhost:8161/api/message/ecsTopic?type=topic&subject=PROD.KR.ITIER.ECS.TESTsvr";

            // HttpListener Send URL
            string url = m_httpSendURL;

            // body 값(json)
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            //request.ContentType = "text/xml";

            // ActiveMQ Send URL 일때
            /*
            request.Timeout = 30 * 1000;
            request.Headers.Add("destination", "ecsTopic");
            request.Headers.Add("subject", "PROD.KR.ITIER.ECS.TESTsvr");
            request.Headers.Add("type", "queue");
            string authInfo = "admin" + ":" + "admin";
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers.Add("Authorization", "Basic " + authInfo);
            */

            // POST할 Data를 Request Stream에 write(data)
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            // Data byte 배열화
            request.ContentLength = bytes.Length;
            // Byte 수 지정

            // Response 처리(Get과 동일)
            string responseText = string.Empty;

            try
            {
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {
                responseText = "The connection to the server is not smooth.\r\nContact your administrator.";
            }

            Console.WriteLine(responseText);

            return responseText;

        }


        public string HttpSendData(string httpSnedURL, string data)
        {
            // body 값(json)
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(httpSnedURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            //request.ContentType = "text/xml";

            data = "{\"state\" : \"" + data + "\"}";

            // POST할 Data를 Request Stream에 write(data)
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            // Data byte 배열화
            request.ContentLength = bytes.Length;
            // Byte 수 지정

            // Response 처리(Get과 동일)
            string responseText = string.Empty;

            try
            {
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {
                responseText = "The connection to the server is not smooth.\r\nContact your administrator.";
            }

            Console.WriteLine(responseText);

            return responseText;

        }
    }
}

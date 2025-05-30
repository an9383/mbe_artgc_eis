using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Xml;
using KR.MBE.CommonLibrary.Handler;
using TOS.Driver.CLT.Struct.Common;
using TOS.Driver.CLT.Struct.YC;
using System.IO.Pipes;
using System.Security.Policy;
using JsonServices;
using JsonServices.Messages;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Transport.Fleck;
using TOS.Driver.CLT.Service;
using System.Diagnostics.Metrics;
using KR.MBE.CommonLibrary.Manager;
using ServiceStack;
using static TOS.Driver.CLT.Struct.YC.YCMethod;
using static KR.MBE.Data.Constant;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TOS.Driver.CLT
{
    public partial class Station
    {
        private static Station instance = null; 
        private string m_URL = string.Empty;
        private string m_configFileName = "httpserver.config";
        public JsonClient jsonClient;
        private long m_mSeq = 0;
        public bool IsConnected { get; set; } = false;

        private string GetMsgCount()
        {
            return string.Format($"{m_mSeq += 1}");
        }

        public Station()
        {
        }

        public static Station Instance
        {
            get
            {
                instance ??= new Station();

                return instance;
            }
        }

        public void Initialize()
        {
            string sFullPath = Environment.CurrentDirectory + @"\" + m_configFileName;
            Hashtable htConfig = Middleware.ActiveMQ.Util.ReadXml(sFullPath);
            m_URL = $"{htConfig["HttpUrl"]}";

            if (htConfig["HttpPort"] != null)
            {
                m_URL += $":{htConfig["HttpPort"]}";
            }

            var client = new FleckClient(m_URL);
            var serializer = new Serializer();
            var provider = new StubMessageTypeProvider();
            jsonClient = new JsonClient(client, provider, serializer);

            jsonClient.Client.Disconnected += ClientOnDisconnected;

        }

        private void ClientOnDisconnected(object? sender, EventArgs e)
        {
            Start();
        }

        public async void Start()
        {
            try
            {
                Initialize();
                await jsonClient.ConnectAsync();

                IsConnected = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] {this.GetType().Name} : {ex.ToString()}");

                IsConnected = false;

                if (jsonClient != null)
                {
                    jsonClient.Dispose();
                    jsonClient = null;
                }
            }
            finally
            {
                if(IsConnected == true)
                    SubscribeMessage();
                else
                {
                    await Task.Delay(1000);                   

                    Start();
                }
                    
            }
        }

        public async void Stop()
        {
            try
            {
                if (jsonClient.IsDisposed == false && IsConnected == true)
                {
                    await jsonClient.DisconnectAsync();
                }

                IsConnected = false;
                jsonClient.Dispose();
                jsonClient = null;
            }
            catch (Exception ex)
            {
            }
        }

        public void SendMessage(string msgId, string data)
        {
            switch (msgId)
            {
                case "SendAcceptJob":
                    LogManager.Instance.Information($"SendAcceptJob : {data}");
                    sendAcceptJob(data);
                    break;
                case "AbortJobResponse":
                    break;
                case "MoveJobResponse":
                    break;
                case "ClearanceRequest":
                    break;
                case "sendAycJob":
                    //jsonClient.Call();
                        break;
                case "sendAbortJob":
                    //jsonClient.Send("sendAbortJob", data);
                        break;
                case "sendMoveJob":
                    //jsonClient.Send("sendMoveJob", data);
                        break;
                case "sendClearance":
                    //jsonClient.Send("sendClearance", data);
                        break;
            }
        }
    }
}
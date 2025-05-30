/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Xml;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.Util;
using Newtonsoft.Json;

namespace Middleware.ActiveMQ
{

    public class Listener
    {
        //private string host = "192.168.0.14";
        private string m_sHostIP = "";

        private int m_iPort = 7001;
        private string m_sConnectionFacatoryName = "tmsConnection";
        private string m_sQueueName = "tmsQueue";
        private string m_sTopicName = "tmsTopic";
        private string m_sMonitorTopicName = String.Empty;
        private string m_sEisTopicName = String.Empty;
        private int m_iReplyTimeOut = 5000;

        private string m_sClientID = String.Empty;
        private string m_sSubScriberID = String.Empty;
        private string m_sSyncID = String.Empty;
        private string m_sClientIP = String.Empty;
        private string m_sTargetSubject = String.Empty;
        private string m_sReplySubjectPrefix = String.Empty;
        private string m_sReplySubject = String.Empty;
        private string m_sInterfaceStructure = String.Empty;

        private string m_configFileName = "Middleware.ActiveMQ.xml";
        //private Middleware.Util m_oMiddlewareUtil = new Middleware.Util();
        private Hashtable htConfig = new Hashtable();

        private List<Hashtable> m_htCustomListenerList = new List<Hashtable>();


        //protected static AutoResetEvent semaphore = new AutoResetEvent(false);
        //protected static TimeSpan receiveTimeout = TimeSpan.FromSeconds(10);
        //private static string recvMessage = String.Empty;

        public delegate void OnMessageDelegate(string receivedMsg);
        public event OnMessageDelegate ReceiveMessage;

        private IConnectionFactory m_Connectionfactory = null;
        private IConnection m_Connection = null;
        private ISession m_Session = null;
        private IDestination m_Destination = null;
        private IMessageConsumer m_Consumer = null;


        // 
        private enum CreateContextType
        {
            OneTime = 0, EveryTime = 1
        }


        public bool IsConnected { get; set; }

        private CreateContextType m_CreateContextType = CreateContextType.OneTime;

        public Listener()
        {
            m_sClientIP = Middleware.ActiveMQ.Util.FindActiveIPAddress();
            htConfig = getConfigData();
            if (htConfig["REPLY"].ToString() == "OK")
            {
                m_sHostIP = htConfig["HostIP"].ToString();
                m_iPort = int.Parse(htConfig["Port"].ToString());
                m_sConnectionFacatoryName = htConfig["ConnectionFactoryName"].ToString();
                m_sQueueName = htConfig["QueueName"].ToString();
                m_sTopicName = htConfig["TopicName"].ToString();
                m_sEisTopicName = htConfig["EisTopicName"].ToString();
                m_sMonitorTopicName = htConfig["MonitorTopicName"].ToString();
                m_iReplyTimeOut = int.Parse(htConfig["ReplyTimeOut"].ToString());
                m_sClientID = htConfig["ClientID"].ToString();
                m_sReplySubjectPrefix = htConfig["ReplySubjectPrefix"].ToString();
                m_sTargetSubject = htConfig["TargetSubject"].ToString();
                m_sInterfaceStructure = htConfig["InterfaceStructure"].ToString();

                if (htConfig["CreateContextType"].ToString() == "OneTime")
                {
                    m_CreateContextType = CreateContextType.OneTime;
                }
                else if (htConfig["CreateContextType"].ToString() == "EveryTime")
                {
                    m_CreateContextType = CreateContextType.EveryTime;
                }
            }
            else
            {
                // Exception : Can Not Load Config File
            }
            m_sSyncID = "SYNCID01";    // 사용의미 확인힐것
            m_sClientID = m_sClientID + "-" + m_sClientIP; // Util.KeyGenerator.GetUniqueKey(4);
            m_sSubScriberID = m_sClientID;


        }

        ~Listener()
        {
            ListenClose();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Hashtable getConfigData()
        {
            if( htConfig.Count <= 1 )
            {
                String sFullPath = System.Environment.CurrentDirectory + @"\Config\" + m_configFileName;
                htConfig = Middleware.ActiveMQ.Util.ReadXml( sFullPath );
            }
            return htConfig;
        }

        private void SetConnection()
        {
            m_Connectionfactory ??= new ConnectionFactory(new Uri($"activemq:tcp://{htConfig["HostIP"]}:{htConfig["Port"]}"));

            m_Connection ??= m_Connectionfactory.CreateConnection();

            m_Session ??= m_Connection.CreateSession();

            if (m_Connection.IsStarted == false)
                m_Connection.Start();
        }

        public void Listen()
        {
            try
            {
                SetConnection();

                if (m_Destination == null)
                {
                    if ((htConfig.Contains("EisTopicName")) && htConfig["EisTopicName"].ToString().Length > 0)
                    {
                        m_Destination = SessionUtil.GetDestination(m_Session, $"topic://{m_sEisTopicName}");

                        m_Consumer ??= m_Session.CreateConsumer(m_Destination, $"subject like '{m_sReplySubjectPrefix}%'");
                        //m_Consumer ??= m_Session.CreateConsumer(m_Destination);

                        m_Consumer.Listener += new MessageListener(OnMessage);
                    }
                }

                IsConnected = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public void Listen(string sTopicName, string sListenSubjectName)
        {
            try
            {
                SetConnection();

                var m_Destination = SessionUtil.GetDestination(m_Session, $"topic://{sTopicName}");
                var m_Consumer = m_Session.CreateConsumer(m_Destination, $"subject like '{sListenSubjectName}%'");
                var hashTable = new Hashtable();

                m_Consumer.Listener += new MessageListener(OnMessage);

                hashTable.Add("TopicName", sTopicName);
                hashTable.Add("ListenSubjectName", sListenSubjectName);
                hashTable.Add("Destination", m_Destination);
                hashTable.Add("Consumer", m_Consumer);

                m_htCustomListenerList.Add(hashTable);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public void ListenClose()
        {
            try
            {
                if (m_Consumer != null)
                {
                    m_Consumer.Close();
                    m_Consumer.Dispose();
                }

                if (m_Destination != null)
                {
                    SessionUtil.DeleteDestination(m_Session, $"topic://{m_sEisTopicName}");
                    m_Destination.Dispose();
                }

                if (m_htCustomListenerList.Count > 0)
                {
                    foreach (var ht in m_htCustomListenerList)
                    {
                        if (ht.Contains("Consumer"))
                        {
                            ((IMessageConsumer)ht["Consumer"]).Close();
                            ((IMessageConsumer)ht["Consumer"]).Dispose();
                        } 

                        if (ht.Contains("Destination"))
                        {
                            SessionUtil.DeleteDestination(m_Session, $"topic://{ht["TopicName"]}");
                            ((IDestination)ht["Destination"]).Dispose();
                        }
                    }

                    m_htCustomListenerList.Clear();
                }

                if (m_Session != null)
                {
                    m_Session.Close();
                    m_Session.Dispose();
                }

                if (m_Connection != null)
                {
                    m_Connection.Close();
                    m_Connection.Dispose();
                }

                m_Consumer = null;
                m_Destination = null;
                m_Session = null;
                m_Connection = null;
                m_Connectionfactory = null;

                IsConnected = false;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        // IBytesMessage 변환후 처리
        protected void OnMessage(IMessage receivedMsg)
        {
            string sReturnMessage = string.Empty;

            switch (receivedMsg)
            {
                case ActiveMQTextMessage mqTxtMsg:
                    sReturnMessage = mqTxtMsg.Text;
                    break;
                case ActiveMQBytesMessage mqByteMsg:
                    var tmpByteMsg = mqByteMsg as IBytesMessage;
                    if (tmpByteMsg.BodyLength > 0)
                    {
                        var msg = new byte[(int)tmpByteMsg.BodyLength];
                        tmpByteMsg.ReadBytes(msg);
                        sReturnMessage = Encoding.UTF8.GetString(msg);
                    }
                    break;
                case ITextMessage txtMsg:
                    sReturnMessage = txtMsg.Text;
                    break;
                case IBytesMessage byteMsg:
                    if (byteMsg.BodyLength > 0)
                    {
                        var msg = new byte[(int)byteMsg.BodyLength];
                        byteMsg.ReadBytes(msg);
                        sReturnMessage = Encoding.UTF8.GetString(msg);
                    }
                    break;
            }


            if (string.IsNullOrEmpty(sReturnMessage) == false)
            {
                if (m_sInterfaceStructure.Equals("JSON"))
                {
                    var docJson = JsonConvert.DeserializeXmlNode(sReturnMessage);

                    if (docJson != null)
                        sReturnMessage = docJson.DocumentElement.OuterXml;
                }

                if (ReceiveMessage != null)
                    ReceiveMessage(sReturnMessage);
            }
        }
    }
}
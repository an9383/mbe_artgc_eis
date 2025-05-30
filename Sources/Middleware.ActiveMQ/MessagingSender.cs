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

using Apache.NMS;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Diagnostics;
using System.Xml;

namespace Middleware.ActiveMQ
{

    public class Sender
    {
        private bool m_debugLog = false;
        //private string host = "192.168.0.14";
        private string m_sHostIP = "";

        private int m_iPort = 7001;
        private string m_sConnectionFacatoryName = "jmsConnection";
        private string m_sQueueName = "jmsQueue";
        private string m_sTopicName = "jmsTopic";
        private string m_sEisTopicName = "jmsTopic";
        private string m_sMonitorTopicName = "jmsTopic";
        private int m_iReplyTimeOut = 5000;

        private string m_sClientID = String.Empty;
        private string m_sSubScriberID = String.Empty;
        private string m_sSyncID = String.Empty;
        private string m_sClientIP = String.Empty;
        private string m_sTargetSubject = String.Empty;
        private string m_sMonitoringTargetSubject = String.Empty;
        private string m_sReplySubjectPrefix = String.Empty;
        private string m_sReplySubject = String.Empty;
        private string m_sInterfaceStructure = String.Empty;

        private string m_configFileName = "Middleware.ActiveMQ.xml";
        //private Middleware.Util m_oMiddlewareUtil = new Middleware.Util();
        private Hashtable htConfig = new Hashtable();

        // 
        private enum CreateContextType
        {
            OneTime = 0, EveryTime = 1
        }

        private CreateContextType m_CreateContextType = CreateContextType.OneTime;

        public Sender()
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
                m_sMonitoringTargetSubject = htConfig["MonitoringTargetSubject"].ToString();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Hashtable getConfigData()
        {
            if (htConfig.Count <= 1)
            {
                String sFullPath = System.Environment.CurrentDirectory + @"\Config\" + m_configFileName;
                htConfig = Middleware.ActiveMQ.Util.ReadXml(sFullPath);
            }
            return htConfig;
        }

        public String getReplySubject()
        {
            if (m_sReplySubject == string.Empty)
            {
                // -----------------
                // Create Unique ID 
                // ReplySubject 뒤에 Unique ID 생성 ( ReplySubject + ClientID + UniqueKey )
                // -----------------
                String sKey = Middleware.ActiveMQ.Util.KeyGenerator.GetUniqueKey(16);
                m_sReplySubject = m_sReplySubjectPrefix + "." + m_sClientID + "." + sKey;
            }
            return m_sReplySubject;
        }

        public String getSourceSubject()
        {
            // Weblogic 은 SourceSubject과 Reply Subject이 동일함.
            return m_sReplySubject;
        }

        public String getTargetSubject()
        {
            return m_sTargetSubject;
        }

        public String getTrsanctionID()
        {
            // -----------------
            // Create Unique ID 
            // ReplySubject 뒤에 Unique ID 생성 ( ReplySubject + ClientID + UniqueKey )
            // -----------------
            String sKey = Middleware.ActiveMQ.Util.KeyGenerator.GetUniqueKey(16);
            String sTransactionID = m_sReplySubject + "." + m_sClientID + "." + sKey;

            return sTransactionID;
        }


        public String SendReplyTopic(String sMessage)
        {
            return SendReplyTopic(sMessage, this.m_iReplyTimeOut);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMessage"></param>
        /// <returns></returns>
        public String SendReplyTopic(String sMessage, int iReplyTimeOutSec)
        {
            String sReturnMessage = String.Empty;

            // Example connection strings:
            //    activemq:tcp://activemqhost:61616
            //    stomp:tcp://activemqhost:61613
            //    ems:tcp://tibcohost:7222
            //    msmq://localhost

            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?jms.useAsyncSend=true";
            String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?wireFormat.maxInactivityDuration=0";
            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort;
            Uri connecturi = new Uri(sURI);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            //ConnectionFactory factory = new ConnectionFactory(connecturi);
            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {

                /*
                // Examples for getting a destination:
                //
                // Hard coded destinations:
                //    IDestination destination = session.GetQueue("FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //    IDestination destination = session.GetTopic("FOO.BAR");
                //    Debug.Assert(destination is ITopic);
                //
                // Embedded destination type in the name:
                //    IDestination destination = SessionUtil.GetDestination(session, "queue://FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //    IDestination destination = SessionUtil.GetDestination(session, "topic://FOO.BAR");
                //    Debug.Assert(destination is ITopic);
                //
                // Defaults to queue if type is not specified:
                //    IDestination destination = SessionUtil.GetDestination(session, "FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //
                // .NET 3.5 Supports Extension methods for a simplified syntax:
                //    IDestination destination = session.GetDestination("queue://FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //    IDestination destination = session.GetDestination("topic://FOO.BAR");
                //    Debug.Assert(destination is ITopic);
                //IDestination destination = SessionUtil.GetDestination(session, "queue://FOO.BAR");
                */

                // Virtual Topic 사용시 Queue 로 받아야 하므로 Consumaer, Producer Destination 를 분리 관리한다.
                // 초기값은 동일한 TopicName 이며
                // Active MQ 의 Virtual Topic 을 사용하는 경우 QueueName 을 설정해 주어야 한다.
                IDestination topicDestination = session.GetTopic(this.m_sTopicName);
                IDestination queueDestination = session.GetTopic(this.m_sTopicName);
                if (this.m_sQueueName != null && this.m_sQueueName != "-" && this.m_sQueueName == "")
                {
                    queueDestination = session.GetQueue(this.m_sQueueName);
                }


                Console.WriteLine("Using queueDestination: " + queueDestination);
                Console.WriteLine("Using topicDestination: " + topicDestination);

                String sTransactionID = Middleware.ActiveMQ.Util.GetXMLRecord(sMessage, "replysubject");
                String sReplySubject = sTransactionID;

                // Create a consumer and producer
                using (IMessageConsumer consumer = session.CreateConsumer(queueDestination, "subject='" + sReplySubject + "'"))
                using (IMessageProducer producer = session.CreateProducer(topicDestination))
                {
                    // Start the connection so that messages will be processed.
                    connection.Start();
                    //producer.DeliveryMode = MsgDeliveryMode.Persistent;
                    producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                    if (m_sInterfaceStructure.Equals("JSON"))
                    {
                        XmlDocument doc = new XmlDocument();
                        sMessage = sMessage.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                        doc.LoadXml(sMessage);
                        sMessage = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, false);
                    }

                    // ByteMessage 로 변경 -> Character set 이슈
                    byte[] byteMessage = System.Text.UTF8Encoding.UTF8.GetBytes(sMessage);
                    IBytesMessage request = session.CreateBytesMessage(byteMessage);

                    request.NMSCorrelationID = this.m_sClientID;
                    //request.Properties["NMSXGroupID"] = "hyperMES";
                    //request.Properties["myHeader"] = "hyperMES";

                    request.Properties["transactionid"] = sTransactionID;
                    request.Properties["subject"] = m_sTargetSubject;
                    request.Properties["replysubject"] = sReplySubject;

                    if (m_debugLog)
                        Debug.WriteLine($"[{m_sHostIP}:{m_iPort}:{topicDestination}]\n {sMessage}");

                    producer.Send(request);

                    TimeSpan replyTimeOut = TimeSpan.FromSeconds(iReplyTimeOutSec);

                    IBytesMessage recvMessage = (IBytesMessage)consumer.Receive(replyTimeOut);
                    if (recvMessage == null)
                    {
                        String sMsg = replyTimeOut.ToString() + "초동안 서버에서 처리를 하지 못해 결과를 받지 못했습니다.\r\n";
                        sMsg += "짐시후 해당 데이터 처리를 확인해 주시기 바랍니다.";

                        sReturnMessage = "<returncode>CM-900</returncode>";
                        sReturnMessage += "<returntype>Error</returntype>";
                        sReturnMessage += "<returnmessage>" + sMsg + "</returnmessage>";
                        sReturnMessage += "<returndetailmessage>" + sMessage + "</returndetailmessage>";

                        return sReturnMessage;
                    }
                    else
                    {
                        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
                        byte[] msg = new byte[(int)recvMessage.BodyLength];
                        recvMessage.ReadBytes(msg);
                        sReturnMessage = utf8.GetString(msg);
                        //recvMessage.Acknowledge();
                    }

                }
            }

            if (m_sInterfaceStructure.Equals("JSON"))
            {
                XmlDocument docJson = JsonConvert.DeserializeXmlNode(sReturnMessage);
                sReturnMessage = docJson.DocumentElement.OuterXml;
            }

            return sReturnMessage;
        }

        /// <summary>
        /// Async Send Message
        /// </summary>
        /// <param name="sMessage"></param>
        /// <returns></returns>
        public void SendTopic(string sMessage)
        {
            string sReturnMessage = string.Empty;

            // Example connection strings:
            //    activemq:tcp://activemqhost:61616
            //    stomp:tcp://activemqhost:61613
            //    ems:tcp://tibcohost:7222
            //    msmq://localhost

            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?jms.useAsyncSend=true";
            string sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?wireFormat.maxInactivityDuration=0&jms.useAsyncSend=true";
            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort;
            Uri connecturi = new Uri(sURI);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            //ConnectionFactory factory = new ConnectionFactory(connecturi);
            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {

                /*
                // Examples for getting a destination:
                //
                // Hard coded destinations:
                //    IDestination destination = session.GetQueue("FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //    IDestination destination = session.GetTopic("FOO.BAR");
                //    Debug.Assert(destination is ITopic);
                //
                // Embedded destination type in the name:
                //    IDestination destination = SessionUtil.GetDestination(session, "queue://FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //    IDestination destination = SessionUtil.GetDestination(session, "topic://FOO.BAR");
                //    Debug.Assert(destination is ITopic);
                //
                // Defaults to queue if type is not specified:
                //    IDestination destination = SessionUtil.GetDestination(session, "FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //
                // .NET 3.5 Supports Extension methods for a simplified syntax:
                //    IDestination destination = session.GetDestination("queue://FOO.BAR");
                //    Debug.Assert(destination is IQueue);
                //    IDestination destination = session.GetDestination("topic://FOO.BAR");
                //    Debug.Assert(destination is ITopic);
                //IDestination destination = SessionUtil.GetDestination(session, "queue://FOO.BAR");
                */

                // Virtual Topic 사용시 Queue 로 받아야 하므로 Consumaer, Producer Destination 를 분리 관리한다.
                // 초기값은 동일한 TopicName 이며
                // Active MQ 의 Virtual Topic 을 사용하는 경우 QueueName 을 설정해 주어야 한다.
                IDestination topicDestination = session.GetTopic(this.m_sTopicName);
                IDestination queueDestination = session.GetTopic(this.m_sTopicName);
                if (this.m_sQueueName != null && this.m_sQueueName != "-" && this.m_sQueueName == "")
                {
                    queueDestination = session.GetQueue(this.m_sQueueName);
                }


                Console.WriteLine("Using queueDestination: " + queueDestination);
                Console.WriteLine("Using topicDestination: " + topicDestination);

                string sTransactionID = Middleware.ActiveMQ.Util.GetXMLRecord(sMessage, "replysubject");
                String sReplySubject = sTransactionID;

                // Create a consumer and producer
                using (IMessageConsumer consumer = session.CreateConsumer(queueDestination, "subject='" + sReplySubject + "'"))
                using (IMessageProducer producer = session.CreateProducer(topicDestination))
                {
                    // Start the connection so that messages will be processed.
                    connection.Start();
                    //producer.DeliveryMode = MsgDeliveryMode.Persistent;
                    producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                    if (m_sInterfaceStructure.Equals("JSON"))
                    {
                        XmlDocument doc = new XmlDocument();
                        sMessage = sMessage.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                        doc.LoadXml(sMessage);
                        sMessage = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, false);
                    }

                    // ByteMessage 로 변경 -> Character set 이슈
                    byte[] byteMessage = System.Text.UTF8Encoding.UTF8.GetBytes(sMessage);
                    IBytesMessage request = session.CreateBytesMessage(byteMessage);

                    request.NMSCorrelationID = this.m_sClientID;
                    //request.Properties["NMSXGroupID"] = "hyperMES";
                    //request.Properties["myHeader"] = "hyperMES";

                    request.Properties["transactionid"] = sTransactionID;
                    request.Properties["subject"] = m_sTargetSubject;
                    request.Properties["replysubject"] = sReplySubject;

                    if (m_debugLog)
                        Debug.WriteLine($"[{m_sHostIP}:{m_iPort}:{topicDestination}]\n {sMessage}");

                    producer.Send(request);

                    /*
                    TimeSpan replyTimeOut = TimeSpan.FromSeconds(this.m_iReplyTimeOut / 1000);

                    IBytesMessage recvMessage = (IBytesMessage)consumer.Receive(replyTimeOut);
                    if (recvMessage == null)
                    {
                        String sMsg = replyTimeOut.ToString() + "초동안 서버에서 처리를 하지 못해 결과를 받지 못했습니다.\r\n";
                        sMsg += "짐시후 해당 데이터 처리를 확인해 주시기 바랍니다.";

                        sReturnMessage = "<returncode>CM-900</returncode>";
                        sReturnMessage += "<returntype>Error</returntype>";
                        sReturnMessage += "<returnmessage>" + sMsg + "</returnmessage>";
                        sReturnMessage += "<returndetailmessage>" + sMessage + "</returndetailmessage>";

                        return sReturnMessage;
                    }
                    else
                    {
                        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
                        byte[] msg = new byte[(int)recvMessage.BodyLength];
                        recvMessage.ReadBytes(msg);
                        sReturnMessage = utf8.GetString(msg);
                        //recvMessage.Acknowledge();
                    }
                    */

                }
            }

        }


        /// <summary>
        /// Async Send Message
        /// </summary>
        /// <param name="sMessage"></param>
        /// <returns></returns>
        public void SendTopicToMonitoringECS(String sMessage)
        {
            String sReturnMessage = String.Empty;

            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?jms.useAsyncSend=true";
            String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?wireFormat.maxInactivityDuration=0&jms.useAsyncSend=true";
            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort;
            Uri connecturi = new Uri(sURI);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            //ConnectionFactory factory = new ConnectionFactory(connecturi);
            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {
                // Virtual Topic 사용시 Queue 로 받아야 하므로 Consumaer, Producer Destination 를 분리 관리한다.
                // 초기값은 동일한 TopicName 이며
                // Active MQ 의 Virtual Topic 을 사용하는 경우 QueueName 을 설정해 주어야 한다.
                IDestination topicDestination = session.GetTopic(this.m_sMonitorTopicName);
                IDestination queueDestination = session.GetTopic(this.m_sMonitorTopicName);
                if (this.m_sQueueName != null && this.m_sQueueName != "-" && this.m_sQueueName == "")
                {
                    queueDestination = session.GetQueue(this.m_sQueueName);
                }

                Console.WriteLine("Using queueDestination: " + queueDestination);
                Console.WriteLine("Using topicDestination: " + topicDestination);

                String sTransactionID = Middleware.ActiveMQ.Util.GetXMLRecord(sMessage, "replysubject");
                String sReplySubject = sTransactionID;

                // Create a consumer and producer
                using (IMessageConsumer consumer = session.CreateConsumer(queueDestination, "subject='" + sReplySubject + "'"))
                using (IMessageProducer producer = session.CreateProducer(topicDestination))
                {
                    // Start the connection so that messages will be processed.
                    connection.Start();
                    //producer.DeliveryMode = MsgDeliveryMode.Persistent;
                    producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                    if (m_sInterfaceStructure.Equals("JSON"))
                    {
                        XmlDocument doc = new XmlDocument();
                        sMessage = sMessage.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                        doc.LoadXml(sMessage);
                        sMessage = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, false);
                    }

                    // ByteMessage 로 변경 -> Character set 이슈
                    byte[] byteMessage = System.Text.UTF8Encoding.UTF8.GetBytes(sMessage);
                    IBytesMessage request = session.CreateBytesMessage(byteMessage);

                    request.NMSCorrelationID = this.m_sClientID;

                    request.Properties["transactionid"] = sTransactionID;
                    request.Properties["subject"] = m_sTargetSubject;
                    request.Properties["replysubject"] = sReplySubject;

                    if (m_debugLog)
                        Debug.WriteLine($"[{m_sHostIP}:{m_iPort}:{topicDestination}]\n {sMessage}");

                    producer.Send(request);
                }
            }

        }


        /// <summary>
        /// Async Send Message
        /// </summary>
        /// <param name="sMessage"></param>
        /// <returns></returns>
        public void SendTopicToMonitoring(String sMessage)
        {
            String sReturnMessage = String.Empty;

            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?jms.useAsyncSend=true";
            String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?wireFormat.maxInactivityDuration=0&jms.useAsyncSend=true";
            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort;
            Uri connecturi = new Uri(sURI);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            //ConnectionFactory factory = new ConnectionFactory(connecturi);
            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {
                // Virtual Topic 사용시 Queue 로 받아야 하므로 Consumaer, Producer Destination 를 분리 관리한다.
                // 초기값은 동일한 TopicName 이며
                // Active MQ 의 Virtual Topic 을 사용하는 경우 QueueName 을 설정해 주어야 한다.
                IDestination topicDestination = session.GetTopic(this.m_sMonitorTopicName);
                IDestination queueDestination = session.GetTopic(this.m_sMonitorTopicName);
                if (this.m_sQueueName != null && this.m_sQueueName != "-" && this.m_sQueueName == "")
                {
                    queueDestination = session.GetQueue(this.m_sQueueName);
                }

                Console.WriteLine("Using queueDestination: " + queueDestination);
                Console.WriteLine("Using topicDestination: " + topicDestination);

                String sTransactionID = Middleware.ActiveMQ.Util.GetXMLRecord(sMessage, "replysubject");
                String sReplySubject = sTransactionID;

                // Create a consumer and producer
                using (IMessageConsumer consumer = session.CreateConsumer(queueDestination, "subject='" + sReplySubject + "'"))
                using (IMessageProducer producer = session.CreateProducer(topicDestination))
                {
                    // Start the connection so that messages will be processed.
                    connection.Start();
                    //producer.DeliveryMode = MsgDeliveryMode.Persistent;
                    producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                    if (m_sInterfaceStructure.Equals("JSON"))
                    {
                        XmlDocument doc = new XmlDocument();
                        sMessage = sMessage.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                        doc.LoadXml(sMessage);
                        sMessage = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, false);
                    }

                    // ByteMessage 로 변경 -> Character set 이슈
                    byte[] byteMessage = System.Text.UTF8Encoding.UTF8.GetBytes(sMessage);
                    IBytesMessage request = session.CreateBytesMessage(byteMessage);

                    request.NMSCorrelationID = this.m_sClientID;

                    request.Properties["transactionid"] = sTransactionID;
                    request.Properties["subject"] = m_sMonitoringTargetSubject;
                    request.Properties["replysubject"] = sReplySubject;

                    if (m_debugLog)
                        Debug.WriteLine($"[{m_sHostIP}:{m_iPort}:{topicDestination}]\n {sMessage}");

                    producer.Send(request);
                }
            }

        }

        /// <summary>
        /// Async Send Message
        /// </summary>
        /// <param name="sMessage"></param>
        /// <returns></returns>
        public void SendTopicEco(String sMessage)
        {
            String sReturnMessage = String.Empty;

            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?jms.useAsyncSend=true";
            String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?wireFormat.maxInactivityDuration=0&jms.useAsyncSend=true";
            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort;
            Uri connecturi = new Uri(sURI);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            //ConnectionFactory factory = new ConnectionFactory(connecturi);
            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {
                // Virtual Topic 사용시 Queue 로 받아야 하므로 Consumaer, Producer Destination 를 분리 관리한다.
                // 초기값은 동일한 TopicName 이며
                // Active MQ 의 Virtual Topic 을 사용하는 경우 QueueName 을 설정해 주어야 한다.
                IDestination topicDestination = session.GetTopic(this.m_sEisTopicName);
                IDestination queueDestination = session.GetTopic(this.m_sEisTopicName);
                if (this.m_sQueueName != null && this.m_sQueueName != "-" && this.m_sQueueName == "")
                {
                    queueDestination = session.GetQueue(this.m_sQueueName);
                }

                Console.WriteLine("Using queueDestination: " + queueDestination);
                Console.WriteLine("Using topicDestination: " + topicDestination);

                String sTransactionID = Middleware.ActiveMQ.Util.GetXMLRecord(sMessage, "replysubject");
                String sReplySubject = sTransactionID;

                // Create a consumer and producer
                using (IMessageConsumer consumer = session.CreateConsumer(queueDestination, "subject='" + sReplySubject + "'"))
                using (IMessageProducer producer = session.CreateProducer(topicDestination))
                {
                    // Start the connection so that messages will be processed.
                    connection.Start();
                    //producer.DeliveryMode = MsgDeliveryMode.Persistent;
                    producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                    if (m_sInterfaceStructure.Equals("JSON"))
                    {
                        XmlDocument doc = new XmlDocument();
                        sMessage = sMessage.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "");
                        doc.LoadXml(sMessage);
                        sMessage = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, false);
                    }

                    // ByteMessage 로 변경 -> Character set 이슈
                    byte[] byteMessage = System.Text.UTF8Encoding.UTF8.GetBytes(sMessage);
                    IBytesMessage request = session.CreateBytesMessage(byteMessage);

                    request.NMSCorrelationID = this.m_sClientID;

                    request.Properties["transactionid"] = sTransactionID;
                    request.Properties["subject"] = m_sReplySubjectPrefix;
                    request.Properties["replysubject"] = sReplySubject;

                    if (m_debugLog)
                        Debug.WriteLine($"[{m_sHostIP}:{m_iPort}:{topicDestination}]\n {sMessage}");

                    producer.Send(request);
                }
            }

        }

        public bool CheckConnection()
        {
            bool bReturn = false;

            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?jms.useAsyncSend=true";
            String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort + "?wireFormat.maxInactivityDuration=0&jms.useAsyncSend=true";
            //String sURI = "activemq:tcp://" + this.m_sHostIP + ":" + this.m_iPort;
            Uri connecturi = new Uri(sURI);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);

            try
            {
                IConnection connection = factory.CreateConnection();
                bReturn = true;
            }
            catch (NMSConnectionException ex)
            {

                Console.WriteLine(ex.Message.ToString());

            }

            return bReturn;

        }


    }
}
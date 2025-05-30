using KR.MBE.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace KR.MBE.Message
{

    public class MessageHandler
    {

        #region xml Message Sample
        /*  XML Message Sample 1
                <?xml version="1.0" encoding="utf-8"?>
                <message>
	                <header>
		                <messagename>GetQueryResult</messagename>
		                <sourcesubject>KCC.CO.KR.UI</sourcesubject>
		                <replysubject>KCC.PROD.PDAtopic.1376294423891.939151.1476431</replysubject>
		                <transactionid>ID:-63759c80:1404c3efcc5:-80001476265</transactionid>
		                <targetsubject>KCC.J2PROD.UIFsvr</targetsubject>
	                </header>
	                <body>
		                <EVENTUSER>19508040</EVENTUSER>
		                <QUERYID>GetCommonArea</QUERYID>
		                <VERSION>00001</VERSION>
		                <LANGUAGE>ko-KR</LANGUAGE>
		                <SITEID>A26</SITEID>
		                <SYS_DATE>201308</SYS_DATE>
		                <BINDV>
			                <SITEID>A30</SITEID>
			                <LANGUAGE>A30</LANGUAGE>
		                </BINDV>
	                </body>
                </message>
        */

        /*  XML Message Sample 2
            <?xml version="1.0" encoding="UTF-8"?>
            <message>
            <header>
                <messagename>GetQueryResult</messagename>
                <replysubject>PROD.KR.ITIER.UI192.168.0.12.TEST</replysubject>
                <sourcesubject>PROD.KR.ITIER.UI192.168.0.12.TEST</sourcesubject>
                <targetsubject>PROD.KR.ITIER.TESTsvr</targetsubject>
            </header>
            <body>
                <SITEID>ITIER</SITEID>
                <LANGUAGE>ko</LANGUAGE>
                <QUERYID>TEST1</QUERYID>
                <QUERYVERSION>00001</QUERYVERSION>
            </body>
            </message> 
        */
        #endregion

        #region Member Valiables
        private static string m_sSiteID = string.Empty;
        private static string m_sEventUserID = string.Empty;

        private static string m_configFileName = "message.config";
        private static MiddlewareType m_Middleware = MiddlewareType.NONE;
        private const string m_Delimeter = "│";
        /// <summary>
        /// Middle ware Type을 위한 Enumerate Type
        /// </summary>
        private enum MiddlewareType
        {
            NONE = 0, WEBLOGIC = 1, TIBCO, JMS, ACTIVEMQ
        }

        #endregion

        //private static Middleware.Weblogic.Sender m_oSender_Weblogic = new Middleware.Weblogic.Sender();

        private static Middleware.ActiveMQ.Sender m_oSender_ActiveMQ = new Middleware.ActiveMQ.Sender();

        /// <summary>
        /// Message Handler Creator
        /// </summary>
        public MessageHandler()
        {
            ReadConfigFile();
        }

        /// <summary>
        /// Message Body Common Parameter 설정
        /// </summary>
        /// <param name="htBody"></param>
        public static void SetCommonBodyData( Hashtable htBody )
        {
            m_sSiteID = htBody["SITEID"].ToString();
            m_sEventUserID = htBody["EVENTUSER"].ToString();
        }

        /// <summary>
        /// Read Config File
        /// </summary>
        private static void ReadConfigFile()
        {
            if( m_Middleware == MiddlewareType.NONE )
            {
                string sFullPath = System.Environment.CurrentDirectory + @"\" + m_configFileName;
                Hashtable htConfig = Middleware.ActiveMQ.Util.ReadXml( sFullPath );

                string sMiddlewareType = htConfig["Middleware"].ToString();

                if( sMiddlewareType.ToUpper() == "ACTIVEMQ" )
                {
                    m_Middleware = MiddlewareType.ACTIVEMQ;
                }
            }

        }

        #region Public makeMessage


        public static bool AliveCheck()
        {
            bool isAlive = false;

            DataSet dsReturn = null;

            string strMessage = string.Empty;
            string strHeader = string.Empty;
            string strBody = string.Empty;

            Middleware.ActiveMQ.Sender oSender = m_oSender_ActiveMQ;

            if (oSender.CheckConnection())
            {
                // Header String
                strHeader += addXMLTag("messagename", "AliveCheck");
                strHeader += addXMLTag("replysubject", oSender.getReplySubject());
                strHeader += addXMLTag("sourcesubject", oSender.getReplySubject());
                strHeader += addXMLTag("targetsubject", oSender.getTargetSubject());
                strHeader = addParentXMLTag("header", strHeader);
                // Body String
                //strBody += addXMLTag("CHECK", "");
                strBody += addParentXMLTag("body", strBody);
                // XML Message String
                strMessage = @"<?xml version=""1.0"" encoding=""utf-8""?>";
                strMessage += addParentXMLTag("message", strHeader + strBody);

                // Send Message
                string strReply = oSender.SendReplyTopic(strMessage, 2);

                // Reply Message Parsing
                dsReturn = MessageHandler.GetXmlToDataSet(strReply, "_REPLYDATA");

                if (dsReturn.Tables.Contains("_REPLYDATA"))
                {
                    isAlive = true;
                }
            }

            return isAlive;

        }

        public static ReturnData LogInTransaction( string strSiteID, string strUserID, string strUserPassword )
        {
            ReturnData oReturn = new ReturnData();

            Hashtable htBody = new Hashtable();
            htBody.Add( "SITEID", strSiteID );
            htBody.Add( "USERID", strUserID );
            htBody.Add( "USERPASSWORD", strUserPassword );

            DataSet dsReturn = sendMessage( "TxnLogin", htBody, "", null );

            if( dsReturn.Tables.Contains( "_ERROR" ) )
            {
                oReturn.returncode = dsReturn.Tables["_ERROR"].Rows[0]["returncode"].ToString();
                oReturn.returnmessage = dsReturn.Tables["_ERROR"].Rows[0]["returnmessage"].ToString();
                oReturn.returntype = dsReturn.Tables["_ERROR"].Rows[0]["returntype"].ToString();
                oReturn.returndetailmessage = dsReturn.Tables["_ERROR"].Rows[0]["returndetailmessage"].ToString();
            }
            else
            {
                oReturn.returncode = "0";
            }
            return oReturn;

        }

        public static DataSet getCustomQuery( string strSiteID, string strQueryID, string strQueryVersion, string strLanguage, Hashtable htBindValue )
        {

            Hashtable htBody = new Hashtable();
            htBody.Add("SITEID", strSiteID);
            htBody.Add("LANGUAGE", strLanguage);  // "ko"
            htBody.Add("QUERYID", strQueryID);
            htBody.Add("QUERYVERSION", strQueryVersion);

            ArrayList arrBindValue = new ArrayList();
            if ((htBindValue != null) && (htBindValue.Count > 0))
            {
                arrBindValue.Add(htBindValue);
            }
            DataSet dsReturn = sendMessage(Constant.MessgeName.GetQueryResult, htBody, "BINDV", arrBindValue);

            //조회결과 오류처리.
            if ( (dsReturn != null) && (dsReturn.Tables["_ERROR"] != null) )
            {
                string errorCode = dsReturn.Tables["_ERROR"].Rows[0]["returncode"].ToString();
                string errorMessage = dsReturn.Tables["_ERROR"].Rows[0]["returnmessage"].ToString();

                string error = $"ERROR \r\n \r\n"
                                + $"QueryID : {strQueryID} ({strQueryVersion}) \r\n"
                                + $"Code : {errorCode} \r\n"
                                + $"Message : {errorMessage}";
                MessageBox.Show(error);

                //비어있는 DataSet 생성 후 Return.
                DataTable dt = new DataTable();
                dt.TableName = "_REPLYDATA";
                dsReturn.Tables.Add(dt);
            }

            return dsReturn;
        }

        public static ReturnData executeTransaction( string strTransactionID, Hashtable htBody )
        {
            ReturnData oReturn = new ReturnData();

            DataSet dsReturn = sendMessage( strTransactionID, htBody, "", null );

            if( dsReturn.Tables.Contains( "_ERROR" ) )
            {
                oReturn.returncode = dsReturn.Tables["_ERROR"].Rows[0]["returncode"].ToString();
                oReturn.returnmessage = dsReturn.Tables["_ERROR"].Rows[0]["returnmessage"].ToString();
                oReturn.returntype = dsReturn.Tables["_ERROR"].Rows[0]["returntype"].ToString();
                oReturn.returndetailmessage = dsReturn.Tables["_ERROR"].Rows[0]["returndetailmessage"].ToString();
            }
            else
            {
                oReturn.returncode = "0";
            }
            return oReturn;

        }


        public static string executeTransactionRowStatus( string strSiteID, string strTransactionID, DataTable dtTransactionDataTable, bool bAllData )
        {
            string sReturn = string.Empty;

            Hashtable htBody = new Hashtable();
            htBody.Add( "SITEID", strSiteID );

            ArrayList arrData = new ArrayList();

            dtTransactionDataTable.AcceptChanges();

            for( int i = 0; i < dtTransactionDataTable.Rows.Count; i++ )
            {
                Hashtable htCurrent = new Hashtable();
                DataRow drData = dtTransactionDataTable.Rows[i];
                string sRowStatus = string.Empty;

                if( dtTransactionDataTable.Columns.Contains( "_ROWSTATUS" ) )
                {
                    sRowStatus = drData["_ROWSTATUS"].ToString();
                }

                if( ( bAllData == true ) || ( sRowStatus.Length > 0 ) )
                {
                    // Column Name 과 값 생성
                    for( int j = 0; j < dtTransactionDataTable.Columns.Count; j++ )
                    {
                        DataColumn dcData = dtTransactionDataTable.Columns[j];
                        htCurrent.Add( dcData.ColumnName, drData[dcData].ToString() );
                    }

                    if( htCurrent.ContainsKey( "_ROWSTATUS" ) )
                    {
                        htCurrent["_ROWSTATUS"] = sRowStatus;
                    }
                    else
                    {
                        htCurrent.Add( "_ROWSTATUS", sRowStatus );
                    }
                    arrData.Add( htCurrent );
                }
            }

            DataSet dsReturn = sendMessage( strTransactionID, htBody, "DATALIST", "DATASET", "DATAINFO", arrData );

            if ( dsReturn.Tables.Contains( "_ERROR" ) )
            {
                sReturn = dsReturn.Tables["_ERROR"].Rows[0]["returncode"].ToString();
            }
            else
            {
                dtTransactionDataTable.AcceptChanges();
                sReturn = "0";
            }

            return sReturn;

        }


        public static ReturnData executeTransaction( string strSiteID, string strTransactionID, DataTable dtTransactionDataTable, bool bAllData )
        {
            string sReturn = string.Empty;

            Hashtable htBody = new Hashtable();
            htBody.Add( "SITEID", strSiteID );

            ArrayList arrData = new ArrayList();

            dtTransactionDataTable.AcceptChanges();

            for( int i = 0; i < dtTransactionDataTable.Rows.Count; i++ )
            {
                Hashtable htCurrent = new Hashtable();
                DataRow drData = dtTransactionDataTable.Rows[i];
                string sRowStatus = string.Empty;

                if( dtTransactionDataTable.Columns.Contains( "_ROWSTATUS" ) )
                {
                    sRowStatus = drData["_ROWSTATUS"].ToString();
                }

                if( ( bAllData == true ) || ( sRowStatus.Length > 0 ) )
                {
                    // Column Name 과 값 생성
                    for( int j = 0; j < dtTransactionDataTable.Columns.Count; j++ )
                    {
                        DataColumn dcData = dtTransactionDataTable.Columns[j];
                        String sData = String.Empty;
                        if( dcData.DataType.Name == "DateTime" )
                        {
                            try
                            {
                                DateTime dData = ( DateTime )drData[dcData];
                                sData = dData.ToString( "yyyy-MM-dd HH:mm:ss" );
                            }
                            catch( Exception e )
                            {
                                ;
                            }
                        }
                        else
                        {
                            sData = drData[dcData].ToString();
                        }
                        htCurrent.Add( dcData.ColumnName, sData );
                    }

                    if( htCurrent.ContainsKey( "_ROWSTATUS" ) )
                    {
                        htCurrent["_ROWSTATUS"] = sRowStatus;
                    }
                    else
                    {
                        htCurrent.Add( "_ROWSTATUS", sRowStatus );
                    }
                    arrData.Add( htCurrent );
                }
            }

            DataSet dsReturn = sendMessage( strTransactionID, htBody, "DATALIST", "DATASET", "DATAINFO", arrData );

            ReturnData oReturn = new ReturnData();

            if( dsReturn.Tables.Contains( "_ERROR" ) )
            {
                oReturn.returncode = dsReturn.Tables["_ERROR"].Rows[0]["returncode"].ToString();
                oReturn.returnmessage = dsReturn.Tables["_ERROR"].Rows[0]["returnmessage"].ToString();
                oReturn.returntype = dsReturn.Tables["_ERROR"].Rows[0]["returntype"].ToString();
                oReturn.returndetailmessage = dsReturn.Tables["_ERROR"].Rows[0]["returndetailmessage"].ToString();
            }
            else
            {
                dtTransactionDataTable.AcceptChanges();
                oReturn.returncode = "0";
                oReturn.returndataset = dsReturn.Copy();
            }

            return oReturn;
        }

        public static ReturnData executeTransaction( string strSiteID, string strTransactionID, DataTable dtTransactionDataTable )
        {
            string sReturn = string.Empty;

            Hashtable htBody = new Hashtable();
            htBody.Add( "SITEID", strSiteID );

            ArrayList arrData = new ArrayList();

            //dtTransactionDataTable.AcceptChanges();

            for( int i = 0; i < dtTransactionDataTable.Rows.Count; i++ )
            {
                Hashtable htCurrent = new Hashtable();
                DataRow drData = dtTransactionDataTable.Rows[i];
                string sRowStatus = string.Empty;

                if( dtTransactionDataTable.Columns.Contains( "_ROWSTATUS" ) )
                {
                    sRowStatus = drData["_ROWSTATUS"].ToString();
                }
                else
                {
                    if( drData.RowState == DataRowState.Deleted )
                        sRowStatus = "D";
                    if( drData.RowState == DataRowState.Added )
                        sRowStatus = "C";
                    if( drData.RowState == DataRowState.Modified )
                        sRowStatus = "U";
                }
                if( sRowStatus.Length > 0 )
                {
                    // Column Name 과 값 생성
                    for( int j = 0; j < dtTransactionDataTable.Columns.Count; j++ )
                    {
                        DataColumn dcData = dtTransactionDataTable.Columns[j];
                        htCurrent.Add( dcData.ColumnName, drData[dcData].ToString() );
                    }

                    if( htCurrent.ContainsKey( "_ROWSTATUS" ) )
                    {
                        htCurrent["_ROWSTATUS"] = sRowStatus;
                    }
                    else
                    {
                        htCurrent.Add( "_ROWSTATUS", sRowStatus );
                    }
                    arrData.Add( htCurrent );
                }
            }

            DataSet dsReturn = sendMessage( strTransactionID, htBody, "DATALIST", "DATASET", "DATAINFO", arrData );

            ReturnData oReturn = new ReturnData();

            if( dsReturn.Tables.Contains( "_ERROR" ) )
            {
                oReturn.returncode = dsReturn.Tables["_ERROR"].Rows[0]["returncode"].ToString();
                oReturn.returnmessage = dsReturn.Tables["_ERROR"].Rows[0]["returnmessage"].ToString();
                oReturn.returntype = dsReturn.Tables["_ERROR"].Rows[0]["returntype"].ToString();
                oReturn.returndetailmessage = dsReturn.Tables["_ERROR"].Rows[0]["returndetailmessage"].ToString();
            }
            else
            {
                dtTransactionDataTable.AcceptChanges();
                oReturn.returncode = "0";
                oReturn.returndataset = dsReturn.Copy();
            }

            return oReturn;
        }

        #endregion

        #region Private makeMessage

        private static DataSet sendMessage( string strMessageName, Hashtable htBody, string strDataListTag, ArrayList arrBindValue )
        {
            return sendMessage( strMessageName, htBody, strDataListTag, "", "", arrBindValue );
        }


        private static DataSet sendMessage( string strMessageName, Hashtable htBody, string strDataListTag, string strDataID, string strDataInfoTag, ArrayList arrBindValue )
        {
            DataSet dsReturn = new DataSet();

            ReadConfigFile();

            if( m_Middleware == MiddlewareType.WEBLOGIC )
            {
                //dsReturn = sendMessage_WEBLOGIC( strMessageName, htBody, strDataListTag, strDataID, strDataInfoTag, arrBindValue );
            }
            if( m_Middleware == MiddlewareType.TIBCO )
            {
                //dsReturn = sendMessage_TIBCO(strMessageName, htBody, strDataListTag, strDataID, strDataInfoTag, arrBindValue);
            }
            if( m_Middleware == MiddlewareType.JMS )
            {
                //dsReturn = sendMessage_JMS(strMessageName, htBody, strDataListTag, strDataID, strDataInfoTag, arrBindValue);
            }
            if( m_Middleware == MiddlewareType.ACTIVEMQ )
            {
                //dsReturn = sendMessage_ACTIVEMQ( strMessageName, htBody, strDataListTag, strDataID, strDataInfoTag, arrBindValue );
                // JSON 에서 XML Attribute 표현 방법이 없음. 현재 사용하고 있지 않는 거라 삭제
                dsReturn = sendMessage_ACTIVEMQ(strMessageName, htBody, strDataListTag, "", strDataInfoTag, arrBindValue);
            }

            return dsReturn;
        }

        /*
        private static DataSet sendMessage_WEBLOGIC( string strMessageName, Hashtable htBody, string strDataListTag, string strDataID, string strDataInfoTag, ArrayList arrBindValue )
        {

            DataSet dsReturn = null;

            string strMessage = string.Empty;
            string strHeader = string.Empty;
            string strBody = string.Empty;

            //Middleware.Weblogic.Sender oSender = new Middleware.Weblogic.Sender();
            Middleware.Weblogic.Sender oSender = m_oSender_Weblogic;

            string strReplySubject = oSender.getReplySubject();
            string strSourceSubject = oSender.getReplySubject();
            string strTargetSubject = oSender.getTargetSubject();

            // Header String
            strHeader += addXMLTag( "messagename", strMessageName );

            strHeader += addXMLTag( "replysubject", strReplySubject );
            strHeader += addXMLTag( "sourcesubject", strSourceSubject );
            strHeader += addXMLTag( "targetsubject", strTargetSubject );

            strHeader = addParentXMLTag( "header", strHeader );

            // Body String
            strBody = makeXMLBody( htBody, strDataListTag, strDataID, strDataInfoTag, arrBindValue );

            if( strHeader == string.Empty )
            {
                ITIER.CommonLibrary.LogManager.ErrorLog( "make XML Header Message. Please Check the middleware config file" );
                return dsReturn;
            }

            // XML Message String
            strMessage = @"<?xml version=""1.0"" encoding=""utf-8""?>";
            strMessage += addParentXMLTag( "message", strHeader + strBody );

            // Send Message
            string strReply = oSender.SendReplyTopic( strMessage );

            // Reply Message Parsing
            dsReturn = MessageHandler.GetXmlToDataSet( strReply, "_REPLYDATA" );

            return dsReturn;
        }
        */

        private static DataSet sendMessage_ACTIVEMQ( string strMessageName, Hashtable htBody, string strDataListTag, string strDataID, string strDataInfoTag, ArrayList arrBindValue )
        {
            DataSet dsReturn = null;

            using (Util.BeginBusyBlock())
            {
                string strMessage = string.Empty;
                string strHeader = string.Empty;
                string strBody = string.Empty;

                //Middleware.Weblogic.Sender oSender = new Middleware.Weblogic.Sender();
                Middleware.ActiveMQ.Sender oSender = m_oSender_ActiveMQ;

                string strReplySubject = oSender.getReplySubject();
                string strSourceSubject = oSender.getReplySubject();
                string strTargetSubject = oSender.getTargetSubject();

                // Header String
                strHeader += addXMLTag("messagename", strMessageName);

                strHeader += addXMLTag("replysubject", strReplySubject);
                strHeader += addXMLTag("sourcesubject", strSourceSubject);
                strHeader += addXMLTag("targetsubject", strTargetSubject);

                strHeader = addParentXMLTag("header", strHeader);

                // Body String
                strBody = makeXMLBody(htBody, strDataListTag, strDataID, strDataInfoTag, arrBindValue);

                if (strHeader == string.Empty)
                {
                    LogManager.Instance.ErrorLog("make XML Header Message. Please Check the middleware config file");
                    return dsReturn;
                }

                // XML Message String
                strMessage = @"<?xml version=""1.0"" encoding=""utf-8""?>";
                strMessage += addParentXMLTag("message", strHeader + strBody);
                try
                {
                    // Send Message
                    String strReply = oSender.SendReplyTopic(strMessage);
                    // Reply Message Parsing
                    dsReturn = MessageHandler.GetXmlToDataSet(strReply, "_REPLYDATA");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            return dsReturn;
        }


        /// <summary>
        /// make XML Header
        /// </summary>
        /// <param name="strMessageName">Message Name</param>
        /// <param name="strReplySubject">Reply Subject : PROD.KR.ITIER.UI-192.168.0.12.TEST</param>
        /// <param name="strSourceSubject">Source Subject : PROD.KR.ITIER.UI-192.168.0.12.TEST</param>
        /// <param name="strTargetSubject">Target Subject : PROD.KR.ITIER.TESTsvr</param>
        /// <returns></returns>
        private static string makeXMLHeader( string strMessageName )
        {
            string strReturn = string.Empty;

            ReadConfigFile();

            if( m_Middleware == MiddlewareType.WEBLOGIC )
            {
                /*
                Middleware.Weblogic.Sender oSender = m_oSender_Weblogic;

                string sReplySubject = oSender.getReplySubject();
                string sSourceSubject = oSender.getReplySubject();
                string sTargetSubject = oSender.getTargetSubject();

                strReturn += addXMLTag( "messagename", strMessageName );
                strReturn += addXMLTag( "replysubject", sReplySubject );
                strReturn += addXMLTag( "sourcesubject", sSourceSubject );
                strReturn += addXMLTag( "targetsubject", sTargetSubject );
                strReturn += addParentXMLTag( "header", strReturn );
                */
            }

            if( m_Middleware == MiddlewareType.ACTIVEMQ )
            {
                Middleware.ActiveMQ.Sender oSender = m_oSender_ActiveMQ;

                string sReplySubject = oSender.getReplySubject();
                string sSourceSubject = oSender.getReplySubject();
                string sTargetSubject = oSender.getTargetSubject();

                strReturn += addXMLTag( "messagename", strMessageName );
                strReturn += addXMLTag( "replysubject", sReplySubject );
                strReturn += addXMLTag( "sourcesubject", sSourceSubject );
                strReturn += addXMLTag( "targetsubject", sTargetSubject );
                strReturn += addParentXMLTag( "header", strReturn );
            }

            return strReturn;
        }


        private static string makeXMLBody( Hashtable htBody, string strDataListTag, string strDataID, string strDataInfoTag, ArrayList arrBindValue )
        {
            string strReturn = string.Empty;

            // 공통 Body 처리
            if( htBody.ContainsKey( "SITEID" ) == false )
            {
                htBody.Add( "SITEID", m_sSiteID );
            }
            if( htBody.ContainsKey( "EVENTUSER" ) == false )
            {
                htBody.Add( "EVENTUSER", m_sEventUserID );
            }

            IDictionaryEnumerator deBody = htBody.GetEnumerator();

            for( int i = 0; i < htBody.Count; i++ )
            {
                deBody.MoveNext();
                string sTag = deBody.Key.ToString();
                string sValue = deBody.Value.ToString();
                strReturn += addXMLTag( sTag, sValue );
            }

            Hashtable htBindValue = new Hashtable();

            if( ( arrBindValue != null ) && ( arrBindValue.Count > 0 ) )
            {

                string strDataListMessage = string.Empty;
                for( int i = 0; i < arrBindValue.Count; i++ )
                {
                    Hashtable htCurrent = ( Hashtable )arrBindValue[i];
                    if( ( htCurrent != null ) && ( htCurrent.Count > 0 ) )
                    {
                        String strSubMessage = string.Empty;
                        IDictionaryEnumerator deCurrent = htCurrent.GetEnumerator();

                        for( int j = 0; j < htCurrent.Count; j++ )
                        {
                            deCurrent.MoveNext();
                            string sTag = deCurrent.Key.ToString();
                            //string sValue = deCurrent.Value.ToString();
                            if( deCurrent.Value == null )
                            {
                                strSubMessage += addXMLTag( sTag, "" );
                            }
                            else
                            {
                                strSubMessage += addXMLTag( sTag, deCurrent.Value.ToString() );
                            }
                        }

                        if( strDataInfoTag != string.Empty )
                        {
                            strSubMessage = addParentXMLTag( strDataInfoTag, strSubMessage );
                        }

                        strDataListMessage += strSubMessage;
                    }

                }
                strDataListMessage = addParentXMLTag( strDataListTag, strDataID, strDataListMessage );

                strReturn += strDataListMessage;
            }

            strReturn = addParentXMLTag( "body", strReturn );

            return strReturn;
        }



        private static string addXMLTag( string strTag, string strValue )
        {
            string strReturn = string.Empty;
            strReturn += "<" + strTag + ">" + strValue + "</" + strTag + ">";
            strReturn += "\n";
            return strReturn;
        }

        private static string addXMLTag( string strTag, string strValue, bool bAddNewLine )
        {
            string strReturn = string.Empty;
            strReturn += "<" + strTag + ">" + strValue + "</" + strTag + ">";
            if( bAddNewLine )
            {
                strReturn += "\n";
            }
            return strReturn;
        }

        private static string addParentXMLTag( string strTag, string strValue )
        {
            return addParentXMLTag( strTag, "", strValue );
        }

        private static string addParentXMLTag( string strTag, string strID, string strValue )
        {
            string strReturn = string.Empty;
            if( strTag == string.Empty )
            {
                strReturn += strValue;
            }
            else
            {
                if( strID == string.Empty )
                {
                    strReturn += "<" + strTag + ">" + "\n";
                }
                else
                {
                    strReturn += "<" + strTag + " ID='" + strID + "' >" + "\n";
                }
                strReturn += strValue;
                strReturn += "</" + strTag + ">" + "\n";
            }
            return strReturn;
        }


        #endregion

        #region XML ResultSet을 DataSet으로 변환하여 Return [ public Method : GetXmlToDataSet(string recvXML, string dsTableName) ]

        public static DataSet GetXmlToDataSet( string recvXML, string dsTableName )
        {
            string colName = "", sTemp = "", sSelectNodes = String.Empty;
            int iCnt = 0, iChildCnt = 0;
            XmlNode child = null;
            String returnStrXML = string.Empty;

            try
            {

                ArrayList _arrdata = new ArrayList();
                XmlDocument _doc = new XmlDocument();
                string sReturnCode = getXMLResult( recvXML, "<returncode>", "</returncode>" );

                DataSet dsReturn = new DataSet();


                if( sReturnCode == "0" || sReturnCode == "" )
                {
                    returnStrXML = getXMLResult( recvXML, "<body>", "</body>" );
                    sSelectNodes = "DATALIST";
                    if( returnStrXML != "" )
                    {
                        _doc.LoadXml( returnStrXML );
                        XmlNodeList childList = _doc.SelectNodes( sSelectNodes );//"/message/body/DATALIST/DATA");
                        iChildCnt = childList.Count;

                        for( int i = 0; i < iChildCnt; i++ )
                        {
                            sTemp = "";
                            child = null;
                            child = childList.Item( i );

                            if( child != null )
                            {
                                for( int j = 0; j < child.ChildNodes.Count; j++ )
                                {
                                    if( child.ChildNodes[j].ChildNodes.Count > 0 )
                                    {
                                        sTemp = "";
                                        for( int k = 0; k < child.ChildNodes[j].ChildNodes.Count; k++ )
                                        {
                                            sTemp += child.ChildNodes[j].ChildNodes[k].InnerText + m_Delimeter;

                                            if( iCnt == 0 )
                                                colName += child.ChildNodes[j].ChildNodes[k].Name + m_Delimeter;
                                        }

                                    }
                                    else
                                    {
                                        sTemp += child.ChildNodes[j].InnerText + m_Delimeter;
                                        if( iCnt == 0 )
                                            colName += child.ChildNodes[j].Name + m_Delimeter;
                                    }
                                    sTemp = sTemp.Remove( sTemp.Length - 1, 1 );
                                    //sTemp += "\r";  // 마지막 Column 표시


                                    if( iCnt == 0 )
                                    {
                                        colName = colName.Remove( colName.Length - 1, 1 );
                                        //colName += "\r";   // 마지막 Column 표시
                                        _arrdata.Add( colName );
                                    }
                                    ++iCnt;

                                    _arrdata.Add( sTemp );
                                }
                            }
                        }
                    }
                    dsReturn = BuildDataSet( _arrdata, dsTableName, m_Delimeter );

                }
                else
                {
                    //returnStrXML = getXMLResult(recvXML, "<returncode>", "</returncode>") + " | ";
                    //returnStrXML += getXMLResult(recvXML, "<returntype>", "</returntype>") + " | ";
                    //returnStrXML += getXMLResult(recvXML, "<returnmessage>", "</returnmessage>") + " | ";
                    //returnStrXML += getXMLResult(recvXML, "<returndetailmessage>", "</returndetailmessage>");
                    //SendLogToServer(returnStrXML.ToString());
                    //throw new Exception(returnStrXML.ToString());

                    // Error Reply Message to DataTable
                    DataTable dtReply = new DataTable( "_ERROR" );
                    dtReply.Columns.Add( "returncode" );
                    dtReply.Columns.Add( "returntype" );
                    dtReply.Columns.Add( "returnmessage" );
                    dtReply.Columns.Add( "returndetailmessage" );

                    if( sReturnCode == string.Empty )
                    {
                        sReturnCode = recvXML;
                    }
                    DataRow drData = dtReply.NewRow();

                    drData["returncode"] = sReturnCode;
                    drData["returntype"] = getXMLResult( recvXML, "<returntype>", "</returntype>" );
                    drData["returnmessage"] = getXMLResult( recvXML, "<returnmessage>", "</returnmessage>" );
                    drData["returndetailmessage"] = getXMLResult( recvXML, "<returndetailmessage>", "</returndetailmessage>" );
                    dtReply.Rows.Add( drData );

                    dsReturn.Tables.Add( dtReply );

                }

                return dsReturn;
            }
            catch( Exception ex )
            {
                throw new Exception( ex.Message.ToString() );
            }
        }

        #endregion

        #region xml데이터중에서 해당 데이터만  구한다 [ private method ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sRecvMsg"></param>
        /// <param name="Param"></param>
        /// <param name="Param2"></param>
        /// <returns></returns>
        public static string getXMLResult( string sRecvMsg, string Param, string Param2 )
        {
            string[] sTemp = new string[2];
            StringBuilder strEmptyXml = new StringBuilder();
            string returnMsg = "";

            try
            {

                sTemp[0] = Param;
                sTemp[1] = Param2;


                // 각문자열 인덱스 위치 
                int[] iTemp = new int[2];
                iTemp[0] = sRecvMsg.IndexOf( sTemp[0] );  // Param의 시작위치
                iTemp[1] = sRecvMsg.IndexOf( sTemp[1] );  // Param2의 시작위치

                iTemp[0] = iTemp[0] + sTemp[0].Length;

                int iReturnCodeLen = iTemp[1] - iTemp[0];       // Return Message 길이  


                //조회 데이터 0건시 에러.. 빈 xml생성~!! 2012.02.01 박성신

                if( iReturnCodeLen < 0 )
                {
                    //strEmptyXml.Append("<DATALIST>");
                    //strEmptyXml.Append(" <DATA>");
                    //strEmptyXml.Append("</DATA>");
                    //strEmptyXml.Append(" </DATALIST>");

                    returnMsg = "";
                }
                else
                {
                    returnMsg = sRecvMsg.Substring( iTemp[0], iReturnCodeLen );
                }

                return returnMsg;
            }
            catch
            {

                return null;
            }
        }

        #endregion

        #region ArrayList 데이터를 DataSet으로 가공 [ 컬럼명 / 데이터셋 맵핑 처리 ] : private method

        private static DataSet BuildDataSet( ArrayList arrInput, string tableName, string delimeter )
        {
            string _message = "";
            DataSet ds = new DataSet();
            ds.Tables.Add( tableName );
            try
            {

                if( arrInput.Count > 0 )
                {
                    // 첫라인 읽어서 컬럼으로 맵핑 처리 
                    string[] columns = arrInput[0].ToString().Split( delimeter.ToCharArray() );

                    foreach( string col in columns )
                    {
                        bool added = false;
                        int i = 0;

                        while( !( added ) )
                        {
                            string columnName = col;

                            if( !( ds.Tables[tableName].Columns.Contains( columnName ) ) )
                            {
                                if( columnName != "\r" )
                                {
                                    ds.Tables[tableName].Columns.Add( columnName, typeof( string ) );
                                }
                                added = true;
                            }
                            else
                            {
                                ++i;
                            }
                        }
                    }

                    for( int i = 1; i < arrInput.Count; i++ )
                    {
                        string[] items = arrInput[i].ToString().Split( delimeter.ToCharArray() );
                        ds.Tables[tableName].Rows.Add( items );
                    }
                }
            }

            catch( FileNotFoundException ex )
            {
                _message = ex.Message;
                return null;
            }
            catch( Exception ex )
            {
                _message = ex.Message;
                return null;
            }

            return ds;
        }

        #endregion

        public static String getMiddlewareSettingValue(String Name)
        {
            String sReturn = String.Empty;
            sReturn = m_oSender_ActiveMQ.getConfigData()[Name].ToString();
            return sReturn;
        }

        #region Active MQ Async Message API
        public static String makeAsyncMessage(string strSiteID, string strMessageName, DataTable dtTransactionDataTable, bool bAllData, String format)
        {
            return makeAsyncMessage(strSiteID, strMessageName, "DATALIST", "DATAINFO", dtTransactionDataTable, bAllData, format);
        }

        public static String makeAsyncMessage(string strSiteID, string strMessageName, string strDataListTag, string strDataInfoTag, DataTable dtTransactionDataTable, bool bAllData, String format)
        {
            String sReturn = String.Empty;

            Hashtable htBody = new Hashtable();
            htBody.Add("SITEID", strSiteID);


            // DataTable에서 Message에 포함할 Data를 ArrayList로 저장함.
            ArrayList arrData = new ArrayList();

            if (dtTransactionDataTable != null)
            {
                dtTransactionDataTable.AcceptChanges();
                for (int i = 0; i < dtTransactionDataTable.Rows.Count; i++)
                {
                    Hashtable htCurrent = new Hashtable();
                    DataRow drData = dtTransactionDataTable.Rows[i];
                    string sRowStatus = string.Empty;

                    if (dtTransactionDataTable.Columns.Contains("_ROWSTATUS"))
                    {
                        sRowStatus = drData["_ROWSTATUS"].ToString();
                    }

                    if ((bAllData == true) || (sRowStatus.Length > 0))
                    {
                        // Column Name 과 값 생성
                        for (int j = 0; j < dtTransactionDataTable.Columns.Count; j++)
                        {
                            DataColumn dcData = dtTransactionDataTable.Columns[j];
                            htCurrent.Add(dcData.ColumnName, drData[dcData].ToString());
                        }

                        if (htCurrent.ContainsKey("_ROWSTATUS"))
                        {
                            htCurrent["_ROWSTATUS"] = sRowStatus;
                        }
                        else
                        {
                            htCurrent.Add("_ROWSTATUS", sRowStatus);
                        }
                        arrData.Add(htCurrent);
                    }
                }

            }

            string strHeader = string.Empty;

            //Middleware.Weblogic.Sender oSender = new Middleware.Weblogic.Sender();
            Middleware.ActiveMQ.Sender oSender = m_oSender_ActiveMQ;

            string strReplySubject = ""; // oSender.getReplySubject();
            string strSourceSubject = ""; // oSender.getReplySubject();
            string strTargetSubject = ""; //oSender.getTargetSubject();

            // Header String
            strHeader += addXMLTag("messagename", strMessageName);

            strHeader += addXMLTag("replysubject", strReplySubject);
            strHeader += addXMLTag("sourcesubject", strSourceSubject);
            strHeader += addXMLTag("targetsubject", strTargetSubject);

            strHeader = addParentXMLTag("header", strHeader);

            // Body String
            String strBody = makeXMLBody(htBody, strDataListTag, "", strDataInfoTag, arrData);

            if (strHeader == string.Empty)
            {
                LogManager.ErrorLog("make XML Header Message. Please Check the middleware config file");
                return sReturn;
            }

            if (format.Equals("JSON"))
            {
                sReturn = addParentXMLTag("message", strHeader + strBody);
                sReturn = getJSONFormat(sReturn);
            }
            else
            {
                // XML Message String
                sReturn = @"<?xml version=""1.0"" encoding=""utf-8""?>";
                sReturn += addParentXMLTag("message", strHeader + strBody);
            }

            return sReturn;
        }

        public static void SendMessageAsync(string strTransactionID, string strBody)
        {
            string strHeader = string.Empty;

            string strReplySubject = ""; // oSender.getReplySubject();
            string strSourceSubject = ""; // oSender.getReplySubject();
            string strTargetSubject = ""; //oSender.getTargetSubject();

            // Header String
            strHeader += addXMLTag("messagename", strTransactionID);

            strHeader += addXMLTag("replysubject", strReplySubject);
            strHeader += addXMLTag("sourcesubject", strSourceSubject);
            strHeader += addXMLTag("targetsubject", strTargetSubject);

            strHeader = addParentXMLTag("header", strHeader);

            strBody = addParentXMLTag("body", strBody);

            // XML Message String
            string strSendMessage = @"<?xml version=""1.0"" encoding=""utf-8""?>";
            strSendMessage += addParentXMLTag("message", strHeader + strBody);

            try
            {
                ReadConfigFile();

                if (m_Middleware == MiddlewareType.WEBLOGIC)
                {
                    //m_oSender_WebLogic.SendTopic(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.TIBCO)
                {
                    //m_oSender_Tibco(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.JMS)
                {
                    //m_oSender_JMS(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.ACTIVEMQ)
                {
                    m_oSender_ActiveMQ.SendTopic(strSendMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendMessageAsync(string strTransactionID, Hashtable htBody)
        {
            string strHeader = string.Empty;

            string strReplySubject = ""; // oSender.getReplySubject();
            string strSourceSubject = ""; // oSender.getReplySubject();
            string strTargetSubject = ""; //oSender.getTargetSubject();

            // Header String
            strHeader += addXMLTag("messagename", strTransactionID);

            strHeader += addXMLTag("replysubject", strReplySubject);
            strHeader += addXMLTag("sourcesubject", strSourceSubject);
            strHeader += addXMLTag("targetsubject", strTargetSubject);

            strHeader = addParentXMLTag("header", strHeader);

            // Body String
            String strBody = String.Empty;
            IDictionaryEnumerator deBody = htBody.GetEnumerator();
            for (int i = 0; i < htBody.Count; i++)
            {
                deBody.MoveNext();
                string sTag = deBody.Key.ToString();
                string sValue = deBody.Value.ToString();
                strBody += addXMLTag(sTag, sValue);
            }
            SendMessageAsync(strTransactionID, strBody);

            /*
            strBody = addParentXMLTag("body", strBody);

            // XML Message String
            string strSendMessage = @"<?xml version=""1.0"" encoding=""utf-8""?>";
            strSendMessage += addParentXMLTag("message", strHeader + strBody);

            try
            {
                ReadConfigFile();

                if (m_Middleware == MiddlewareType.WEBLOGIC)
                {
                    //m_oSender_WebLogic.SendTopic(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.TIBCO)
                {
                    //m_oSender_Tibco(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.JMS)
                {
                    //m_oSender_JMS(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.ACTIVEMQ)
                {
                    m_oSender_ActiveMQ.SendTopic(strSendMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            */

        }

        public static void SendMessageAsyncEco(string strTransactionID, string strBody)
        {
            string strHeader = string.Empty;

            string strReplySubject = ""; // oSender.getReplySubject();
            string strSourceSubject = ""; // oSender.getReplySubject();
            string strTargetSubject = ""; //oSender.getTargetSubject();

            // Header String
            strHeader += addXMLTag("messagename", strTransactionID);

            strHeader += addXMLTag("replysubject", strReplySubject);
            strHeader += addXMLTag("sourcesubject", strSourceSubject);
            strHeader += addXMLTag("targetsubject", strTargetSubject);

            strHeader = addParentXMLTag("header", strHeader);

            strBody = addParentXMLTag("body", strBody);

            // XML Message String
            string strSendMessage = @"<?xml version=""1.0"" encoding=""utf-8""?>";
            strSendMessage += addParentXMLTag("message", strHeader + strBody);

            try
            {
                ReadConfigFile();

                if (m_Middleware == MiddlewareType.WEBLOGIC)
                {
                    //m_oSender_WebLogic.SendTopic(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.TIBCO)
                {
                    //m_oSender_Tibco(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.JMS)
                {
                    //m_oSender_JMS(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.ACTIVEMQ)
                {
                    m_oSender_ActiveMQ.SendTopicEco(strSendMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendMessageAsync(string strSiteID, string strTransactionID, DataTable dtTransactionDataTable, bool bAllData)
        {
            try
            {
                String sSendMessage = makeAsyncMessage(strSiteID, strTransactionID, dtTransactionDataTable, bAllData, "");
                if (m_Middleware == MiddlewareType.WEBLOGIC)
                {
                    //m_oSender_WebLogic.SendTopic(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.TIBCO)
                {
                    //m_oSender_Tibco(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.JMS)
                {
                    //m_oSender_JMS(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.ACTIVEMQ)
                {
                    m_oSender_ActiveMQ.SendTopic(sSendMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
        public static ReturnData sendMessage(string sMeesageID, Hashtable htBody)
        {
            ReturnData oReturn = new ReturnData();

            DataSet dsReturn = sendMessage(sMeesageID, htBody, "", null);

            if (dsReturn.Tables.Contains("_ERROR"))
            {
                oReturn.returncode = dsReturn.Tables["_ERROR"].Rows[0]["returncode"].ToString();
                oReturn.returnmessage = dsReturn.Tables["_ERROR"].Rows[0]["returnmessage"].ToString();
                oReturn.returntype = dsReturn.Tables["_ERROR"].Rows[0]["returntype"].ToString();
                oReturn.returndetailmessage = dsReturn.Tables["_ERROR"].Rows[0]["returndetailmessage"].ToString();
            }
            else
            {
                oReturn.returncode = "0";
            }
            return oReturn;

        }


        public static String getJSONFormat(String xmlMessage)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlMessage);
            String sReturn = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, false);
            return sReturn;
        }



        public static void SendMessageAsyncToMonitoring(string strTransactionID, string strBody)
        {
            string strHeader = string.Empty;

            string strReplySubject = ""; // oSender.getReplySubject();
            string strSourceSubject = ""; // oSender.getReplySubject();
            string strTargetSubject = ""; //oSender.getTargetSubject();

            // Header String
            strHeader += addXMLTag("messagename", strTransactionID);

            strHeader += addXMLTag("replysubject", strReplySubject);
            strHeader += addXMLTag("sourcesubject", strSourceSubject);
            strHeader += addXMLTag("targetsubject", strTargetSubject);

            strHeader = addParentXMLTag("header", strHeader);

            strBody = addParentXMLTag("body", strBody);

            // XML Message String
            string strSendMessage = @"<?xml version=""1.0"" encoding=""utf-8""?>";
            strSendMessage += addParentXMLTag("message", strHeader + strBody);

            try
            {
                ReadConfigFile();

                if (m_Middleware == MiddlewareType.WEBLOGIC)
                {
                    //m_oSender_WebLogic.SendTopic(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.TIBCO)
                {
                    //m_oSender_Tibco(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.JMS)
                {
                    //m_oSender_JMS(sSendMessage);
                }
                if (m_Middleware == MiddlewareType.ACTIVEMQ)
                {
                    m_oSender_ActiveMQ.SendTopicToMonitoring(strSendMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        #endregion Active MQ Async Message API
    }

}

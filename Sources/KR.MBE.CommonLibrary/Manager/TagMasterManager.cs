using KR.MBE.CommonLibrary.Utils;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KR.MBE.CommonLibrary.Manager
{
    /// <summary>
    /// TagMasterManager에 대한 요약 설명입니다.
    /// </summary>
    /// 
    public class TagMasterManager
    {
        /// <summary>
        /// EquipmentFactoryManager 내부 사용변수 선언.
        /// </summary>
        /// <param name="m_TagMasterMgr">Tag Master Management</param>
        /// <param name="m_dtTagDefinition">TagMaster 기준정보</param>
        public static TagMasterManager m_TagMasterMgr = null;

        private string appPath;
        private string iniFileName;
        public Hashtable m_htTagDefinition = null;
        public DataTable m_dtEventTag = null;
        public static DataTable m_dtMonitoringTagList = null;
        public static DataTable m_dtAlarmTagList = null;

        public static DataTable m_dtEqpProdTag = null;
        public static DataTable m_dtEqpTag = null;
        public static DataTable m_dtWriteTag = null;

        // EIS
        public SqlConnection m_cnnLocalDB = null;
        public SqlCommand m_cmdLocalDB = null;


        /// <summary>
        /// EquipmentFactoryData 선언.
        /// </summary>
        /// <param name="TAGLIST">EIS Tag Defintion Data Query</param>
        public string TAGLIST = " SELECT EQUIPMENTID, TAGID, DESCRIPTION, ADDRESS "
                           + " , EVENTFLAG, PARAMETERFLAG, ALARMFLAG, MONITORINGFLAG, DATATYPE "
                           + " , STATIONID, DRIVERID "
                           + " FROM TB_TAG"
                           + " WHERE USEFLAG = 'Yes' ";


        #region 클래스에서 공통적으로 사용되는 함수들과 생성자

        /// <summary>
        /// EquipmentFactoryManager 기본 생성자.
        /// </summary>	
        public TagMasterManager()
        {
            appPath = Application.StartupPath;
            iniFileName = appPath + @"\SystemInfo.ini";
            LogManager.Instance.Information("Tag Master Manager Starting");

            m_htTagDefinition = new Hashtable();
            m_dtEventTag = new DataTable();
            m_dtMonitoringTagList = new DataTable();
            m_dtAlarmTagList = new DataTable();

            InitializeEventTagDataList();
            InitializeMonitoringTagDataList();
            InitializeAlarmTagDataList();

            MakeTagList();
            MakeMonitoringTagList();
            MakeAlarmTagList();
        }

        private void InitializeEventTagDataList()
        {
            m_dtEventTag.TableName = "EventTag";

            m_dtEventTag.Columns.Add("EquipmentID", typeof(string));
            m_dtEventTag.Columns.Add("ObjectType", typeof(string));
            m_dtEventTag.Columns.Add("ObjectName", typeof(string));
            m_dtEventTag.Columns.Add("ObjectEventType", typeof(string));
            m_dtEventTag.Columns.Add("TagID", typeof(string));
            m_dtEventTag.Columns.Add("TagValue", typeof(string));
            m_dtEventTag.Columns.Add("ReferenceTagID", typeof(string));
        }

        private void InitializeMonitoringTagDataList()
        {
            m_dtMonitoringTagList.TableName = "MonitoringTag";

            m_dtMonitoringTagList.Columns.Add("EQUIPMENTID", typeof(string));
            m_dtMonitoringTagList.Columns.Add("TAGID", typeof(string));
            m_dtMonitoringTagList.Columns.Add("EVENTFLAG", typeof(string));
            m_dtMonitoringTagList.Columns.Add("PARAMETERFLAG", typeof(string));
            m_dtMonitoringTagList.Columns.Add("ALARMFLAG", typeof(string));
            m_dtMonitoringTagList.Columns.Add("MONITORINGFLAG", typeof(string));
            m_dtMonitoringTagList.Columns.Add("DATATYPE", typeof(string));
            m_dtMonitoringTagList.Columns.Add("STATIONID", typeof(string));
            m_dtMonitoringTagList.Columns.Add("DRIVERID", typeof(string));
        }

        private void InitializeAlarmTagDataList()
        {
            m_dtAlarmTagList.TableName = "AlarmTag";

            m_dtAlarmTagList.Columns.Add("EQUIPMENTID", typeof(string));
            m_dtAlarmTagList.Columns.Add("TAGID", typeof(string));
            m_dtAlarmTagList.Columns.Add("EVENTFLAG", typeof(string));
            m_dtAlarmTagList.Columns.Add("PARAMETERFLAG", typeof(string));
            m_dtAlarmTagList.Columns.Add("ALARMFLAG", typeof(string));
            m_dtAlarmTagList.Columns.Add("MONITORINGFLAG", typeof(string));
            m_dtAlarmTagList.Columns.Add("DATATYPE", typeof(string));
            m_dtAlarmTagList.Columns.Add("STATIONID", typeof(string));
            m_dtAlarmTagList.Columns.Add("DRIVERID", typeof(string));
        }

        public static TagMasterManager This()
        {
            if (m_TagMasterMgr == null)
                m_TagMasterMgr = new TagMasterManager();
            return m_TagMasterMgr;

        }

        /// <summary>
        /// 통합 EIS Service를 위한 기준정보 Tag Master Data Load Function
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MakeTagList(string sDataBase); 
        /// </remarks>
        /// <param name="sDatabase">MES Or EIS</param>
        /// <returns>void</returns>
        public void MakeTagList()
        {
            string strSql = "";
            //strSql = TAGLIST + " and siteID = '" + eventLogger.g_SiteID + "' " + " ORDER BY EQUIPMENTID, TAGID ";
            strSql = TAGLIST + " ORDER BY EQUIPMENTID, TAGID ";

            if (!ConnectLocalDB())
            {
                return;
            }

            DataSet dsResult = GetDataTable(strSql, m_cnnLocalDB);

            closeLocalDB();

            for (int i = 0; i < dsResult.Tables[0].Rows.Count; i++)
            {
                TagMasterData tdTagInfo = new TagMasterData();

                tdTagInfo.sEquipmentID = dsResult.Tables[0].Rows[i]["EQUIPMENTID"].ToString();
                tdTagInfo.sTagID = dsResult.Tables[0].Rows[i]["TAGID"].ToString();
                tdTagInfo.sDescription = dsResult.Tables[0].Rows[i]["DESCRIPTION"].ToString();
                tdTagInfo.sAddress = dsResult.Tables[0].Rows[i]["ADDRESS"].ToString();
                tdTagInfo.sEventFlag = dsResult.Tables[0].Rows[i]["EVENTFLAG"].ToString();
                tdTagInfo.sParameterFlag = dsResult.Tables[0].Rows[i]["PARAMETERFLAG"].ToString();
                tdTagInfo.sAlarmFlag = dsResult.Tables[0].Rows[i]["ALARMFLAG"].ToString();
                tdTagInfo.sMonitoringFlag = dsResult.Tables[0].Rows[i]["MONITORINGFLAG"].ToString();
                tdTagInfo.sDataType = dsResult.Tables[0].Rows[i]["DATATYPE"].ToString();
                tdTagInfo.sStationID = dsResult.Tables[0].Rows[i]["STATIONID"].ToString();
                tdTagInfo.sDriverID = dsResult.Tables[0].Rows[i]["DRIVERID"].ToString();

                tdTagInfo.sCurrentValue = "";

                m_htTagDefinition.Add(tdTagInfo.sTagID, tdTagInfo);
            }
        }

        /// <summary>
        /// 통합 EIS Service를 위한 기준정보 Tag Master Data Load Function
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MakeTagList(string sDataBase); 
        /// </remarks>
        /// <param name="sDatabase">MES Or EIS</param>
        /// <returns>void</returns>
        public void MakeMonitoringTagList()
        {
            string strSql = "";
            //strSql = TAGLIST + " and siteID = '" + eventLogger.g_SiteID + "' and MonitoringFlag = 'Yes'";
            strSql = TAGLIST + " and MonitoringFlag = 'Yes'";

            if (!ConnectLocalDB())
            {
                return;
            }

            DataSet dsResult = GetDataTable(strSql, m_cnnLocalDB);

            closeLocalDB();

            if (dsResult == null)
            {
                return;
            }

            DataTable dtResult = dsResult.Tables[0];

            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                DataRow drMonitoring = m_dtMonitoringTagList.NewRow();

                drMonitoring["EQUIPMENTID"] = dsResult.Tables[0].Rows[i]["EQUIPMENTID"].ToString();
                drMonitoring["TAGID"] = dsResult.Tables[0].Rows[i]["TAGID"].ToString();
                drMonitoring["EVENTFLAG"] = dsResult.Tables[0].Rows[i]["EVENTFLAG"].ToString();
                drMonitoring["PARAMETERFLAG"] = dsResult.Tables[0].Rows[i]["PARAMETERFLAG"].ToString();
                drMonitoring["ALARMFLAG"] = dsResult.Tables[0].Rows[i]["ALARMFLAG"].ToString();
                drMonitoring["MONITORINGFLAG"] = dsResult.Tables[0].Rows[i]["MONITORINGFLAG"].ToString();
                drMonitoring["DATATYPE"] = dsResult.Tables[0].Rows[i]["DATATYPE"].ToString();
                drMonitoring["STATIONID"] = dsResult.Tables[0].Rows[i]["STATIONID"].ToString();
                drMonitoring["DRIVERID"] = dsResult.Tables[0].Rows[i]["DRIVERID"].ToString();

                m_dtMonitoringTagList.Rows.Add(drMonitoring);
            }
        }

        public void MakeEventTagList(string sDataBase)
        {
            string strSql = "";
            //strSql = TAGLIST + " and siteID = '" + eventLogger.g_SiteID + "' and EventFlag = 'Yes'";
            strSql = TAGLIST + " and EventFlag = 'Yes'";
            if (!ConnectLocalDB())
            {
                return;
            }

            DataSet dsResult = GetDataTable(strSql, m_cnnLocalDB);

            closeLocalDB();

            if (dsResult == null)
            {
                return;
            }

            DataTable dtResult = dsResult.Tables[0];

            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                DataRow drEvent = m_dtEventTag.NewRow();

                drEvent["EquipmentID"] = dtResult.Rows[i]["EQUIPMENTID"].ToString();
                drEvent["ObjectType"] = dtResult.Rows[i]["OBJECTTYPE"].ToString();
                drEvent["ObjectName"] = dtResult.Rows[i]["OBJECTNAME"].ToString();
                drEvent["ObjectEventType"] = dtResult.Rows[i]["OBJECTEVENTTYPE"].ToString();
                drEvent["TagID"] = dtResult.Rows[i]["TAGID"].ToString();
                drEvent["TagValue"] = dtResult.Rows[i]["TAGVALUE"].ToString();
                drEvent["ReferenceTagID"] = dtResult.Rows[i]["REFERENCETAGID"].ToString();

                m_dtEventTag.Rows.Add(drEvent);
            }
        }

        public void MakeAlarmTagList()
        {
            string strSql = "";
            //strSql = TAGLIST + " and siteID = '" + eventLogger.g_SiteID + "' and AlarmFlag = 'Yes'";
            strSql = TAGLIST + " and AlarmFlag = 'Yes'";
            if (!ConnectLocalDB())
            {
                return;
            }

            DataSet dsResult = GetDataTable(strSql, m_cnnLocalDB);

            closeLocalDB();

            if (dsResult == null)
            {
                return;
            }

            DataTable dtResult = dsResult.Tables[0];

            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                DataRow drAlarm = m_dtAlarmTagList.NewRow();

                drAlarm["EQUIPMENTID"] = dsResult.Tables[0].Rows[i]["EQUIPMENTID"].ToString();
                drAlarm["TAGID"] = dsResult.Tables[0].Rows[i]["TAGID"].ToString();
                drAlarm["EVENTFLAG"] = dsResult.Tables[0].Rows[i]["EVENTFLAG"].ToString();
                drAlarm["PARAMETERFLAG"] = dsResult.Tables[0].Rows[i]["PARAMETERFLAG"].ToString();
                drAlarm["ALARMFLAG"] = dsResult.Tables[0].Rows[i]["ALARMFLAG"].ToString();
                drAlarm["MONITORINGFLAG"] = dsResult.Tables[0].Rows[i]["MONITORINGFLAG"].ToString();
                drAlarm["DATATYPE"] = dsResult.Tables[0].Rows[i]["DATATYPE"].ToString();
                drAlarm["STATIONID"] = dsResult.Tables[0].Rows[i]["STATIONID"].ToString();
                drAlarm["DRIVERID"] = dsResult.Tables[0].Rows[i]["DRIVERID"].ToString();

                m_dtAlarmTagList.Rows.Add(drAlarm);
            }
        }

        #endregion

        private bool ConnectLocalDB()
        {
            try
            {
                if (m_cnnLocalDB == null)
                    m_cnnLocalDB = new SqlConnection();

                if (m_cnnLocalDB.State == ConnectionState.Closed)
                {
                    m_cnnLocalDB.ConnectionString = StaticUtil.getINIValue(iniFileName, "DBCONNECTION", "MAINDBConnection");
                    m_cnnLocalDB.Open();
                }

                if (m_cmdLocalDB == null)
                    m_cmdLocalDB = new SqlCommand();

                m_cmdLocalDB.CommandType = CommandType.Text;
                m_cmdLocalDB.Connection = m_cnnLocalDB;

            }
            catch (Exception ex)
            {
                LogManager.Instance.Exception(ex);
                return false;
            }
            return true;
        }

        private void closeLocalDB()
        {
            if (m_cnnLocalDB == null) return;
            m_cnnLocalDB.Close();
        }

        public DataSet GetDataTable(string strSQL, SqlConnection cnnDB)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(strSQL, cnnDB);
                da.SelectCommand.CommandType = CommandType.Text;

                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Exception(ex);
                return null;
            }
        }

        /// <summary>
        /// Parameter Tag Value를 변경하기 위한 Function
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  UpdateCurrentParameterTagValue(Hashtable htParam); 
        /// </remarks>
        /// <param name="htParam">Parameter Result Hash Table</param>
        /// <returns>void</returns>
        public void UpdateCurrentTagValue(Hashtable htParam)
        {
            lock (this)
            {
                string sTagID;
                string sCurrentValue;    

                foreach (object obj in htParam.Keys)
                {
                    TagMasterData tdUpdate = (TagMasterData)htParam[obj];
                    sTagID = tdUpdate.sTagID;
                    sCurrentValue = tdUpdate.sCurrentValue;

                    if (sCurrentValue != "")
                    {
                        TagMasterData tdDest = (TagMasterData)m_htTagDefinition[sTagID];

                        if (tdDest != null)
                        {
                            tdDest.sCurrentValue = sCurrentValue;
                        }                 
                    }
                }
            }
        }

        /// <summary>
        /// TagMasterManager의 Tag Master Hashtable을 반환하여 주는 Function
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  GetTagMasterHashTable(); 
        /// </remarks>
        /// <returns>Hashtable</returns>
        public Hashtable GetTagMasterHashTable()
        {
            Hashtable htTagMaster = null;
            htTagMaster = m_htTagDefinition;

            return htTagMaster;
        }

        public DataTable GetTagDataTable(string sEventType)
        {
            DataTable dtCommonTag = null;

            if (sEventType == "Material")
            {
                //dtCommonTag = m_dtMaterialTag;
            }
            else if (sEventType == "Event")
            {
                dtCommonTag = m_dtEventTag;
            }
            else if (sEventType == "Alternative")
            {
                //dtCommonTag = m_dtAlternativeTag;
            }
            else if (sEventType == "Monitoring")
            {
                dtCommonTag = m_dtMonitoringTagList;
            }

            return dtCommonTag;
        }
    }

    // <summary>
    /// Tag Master Data를 HashTable에 담기 위한 Data Class
    /// </summary>
    public class TagMasterData
    {
        public string sSiteID;
        public string sEquipmentID;
        public string sTagID;
        public string sDescription;
        public string sAddress;
        public string sDataType;
        public string sStationID;
        public string sDriverID;

        public string sEventFlag;
        public string sParameterFlag;
        public string sAlarmFlag;
        public string sMonitoringFlag;

        public string sCurrentValue;
        public bool bCurrentValueUpdated = false;

        public int itmHandleServer;
        public int TagIdx;
    }
}
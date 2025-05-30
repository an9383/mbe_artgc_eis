using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text;

using System.Timers;
using System.Net;
using System.Threading;

using System.Windows.Forms;
using System.Collections;
using System.Reflection.Metadata;
using KR.MBE.CommonLibrary.Handler;
using KR.MBE.CommonLibrary.Manager;
using KR.MBE.CommonLibrary.Utils;
using TOS.Driver.CLT;
using Apache.NMS.ActiveMQ.Commands;
using Middleware.ActiveMQ;
using Newtonsoft.Json;
using static KR.MBE.Data.Constant;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MBE.Driver.LSElectric;
using Apache.NMS.ActiveMQ.Util;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;


namespace ARTGC.EIS
{
    public partial class frmMain : Form
    {
        public string appPath;
        public string iniFileName;

        public SqlConnection m_cnnLocalDB = null;
        public SqlCommand m_cmdLocalDB = null;

        public DataTable g_dtGridList;
        public DataTable m_dtTagList;

        public DataManager p_DataLibrary;

        public static CraneManager m_pXGTEnetDoc = null;

        private string beDrvName = "";
        private string beStnName = "";

        private string cuDrvName = "";
        private string cuStnName = "";

        private int tagInfoRowIndex = 0;
        private System.Windows.Forms.Timer reconnectTimer = new System.Windows.Forms.Timer();

        Listener MQListner = new Listener();

        public frmMain()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Form 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            p_DataLibrary = new DataManager();
            m_dtTagList = new DataTable();

            m_pXGTEnetDoc = CraneManager.This();

            GridInitialize();

            IFInitStart();
            IFGetStnInfo();

            if (m_pXGTEnetDoc.SetCrane() == false)
            {
                Application.Exit();
            }


            MQListner.ReceiveMessage += ReceiveOnMessage;

            // TOS Listen
            TOS.Driver.CLT.Station.Instance.Start();

            tvStation.NodeMouseClick += tvStation_NodeMouseClick;

            getProcessingJobOrder();

            dgvTagList.CellValueChanged += dgvTagList_CellValueChanged;

            timerEventChecker.Enabled = true;

            reconnectTimer.Interval = 1000;
            reconnectTimer.Tick += ReconnectTimer_Tick;
            reconnectTimer.Start();


            JobManager.Instance.Start();

        }

        private void ReconnectTimer_Tick(object sender, EventArgs e)
        {
            if (MQListner.IsConnected == false)
            {
                MQListner.Listen();
            }
        }

        /// <summary>
        /// Tag 테이블 Cell Value 변경시 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvTagList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Hashtable htTagMaster = TagMasterManager.This().GetTagMasterHashTable();

            var sTagID = dgvTagList.Rows[e.RowIndex].Cells["TAGID"].Value.ToString();
            var sTagValue = dgvTagList.Rows[e.RowIndex].Cells["WRITE"].Value.ToString();
            var sTagIDSplit = sTagID.Split('_');
            var tagData = (TagMasterData)htTagMaster[sTagID];
            var crane = m_pXGTEnetDoc.GetCrane(sTagIDSplit[0]);

            if (dgvTagList.Columns[e.ColumnIndex].Name.ToUpper().Equals("WRITE") == false)
                return;

            if (tagData == null)
                return;

            if (crane == null)
                return;

            tagData.sCurrentValue = sTagValue;

            if (sTagID.Contains("."))
            {
                var sTagIdSplit = sTagID.Split(".");
                var bitArr = new System.Collections.BitArray(16);

                for (int i = 0; i < 16; i++)
                {
                    bitArr[i] = ((TagMasterData)htTagMaster[$"{sTagIdSplit[0]}.{i}"]).sCurrentValue == "1"
                        ? true
                        : false;
                }

                int result = 0;

                for (int i = 0; i < bitArr.Length; i++)
                {
                    if (bitArr[i])
                        result |= (1 << i); // Shift left and OR operation to set the correct bit
                }

                crane.WriteTag(sTagIdSplit[0], result.ToString());
            }
            else
            {
                crane.WriteTag(sTagID, sTagValue);
            }
        }

        /// <summary>
        /// Station TreeView Node Click 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvStation_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level.Equals(1))
            {
                cuDrvName = e.Node.Parent.Text;
                cuStnName = e.Node.Text;

                UpdateTagList();

                beDrvName = cuDrvName;
                beStnName = cuStnName;
            }
        }

        class ColumnSetting
        {
            public string Name { get; set; }
            public bool IsReadOnly { get; set; } = true;
            public DataGridViewContentAlignment Alignment { get; set; } = DataGridViewContentAlignment.MiddleLeft;
        }


        /// <summary>
        /// 테그 테이블 초기화
        /// </summary>
        private void GridInitialize()
        {
            Dictionary<string, ColumnSetting> dicColumn = new Dictionary<string, ColumnSetting>();
            dicColumn.Add("NO", new ColumnSetting { Name = "No", Alignment = DataGridViewContentAlignment.MiddleRight });
            dicColumn.Add("TAGID", new ColumnSetting { Name = "TAGID" });
            dicColumn.Add("WRITE", new ColumnSetting { Name = "Write", IsReadOnly = false, Alignment = DataGridViewContentAlignment.MiddleRight });
            dicColumn.Add("ADDRESS", new ColumnSetting { Name = "Address" });
            dicColumn.Add("DESCRIPTION", new ColumnSetting { Name = "Description" });
            dicColumn.Add("EISVALUE", new ColumnSetting { Name = "EIS Value", Alignment = DataGridViewContentAlignment.MiddleRight });
            dicColumn.Add("PLCVALUE", new ColumnSetting { Name = "PLC Value", Alignment = DataGridViewContentAlignment.MiddleRight });
            dicColumn.Add("EVENT", new ColumnSetting { Name = "Event" });
            dicColumn.Add("PARAMETER", new ColumnSetting { Name = "Parameter" });
            dicColumn.Add("READTIME", new ColumnSetting { Name = "ReadTime" });

            foreach (var columnName in dicColumn.Keys)
            {
                m_dtTagList.Columns.Add(columnName);

                var dgvAddColumn = new DataGridViewTextBoxColumn()
                {
                    Name = columnName,
                    DataPropertyName = columnName,
                    HeaderText = dicColumn[columnName].Name,
                    ReadOnly = dicColumn[columnName].IsReadOnly,
                    Visible = true
                };

                dgvAddColumn.DefaultCellStyle.Alignment = dicColumn[columnName].Alignment;

                switch (columnName)  //ALIGNMENT 
                {
                    case "NO":
                        dgvAddColumn.Width = 50;
                        break;
                    case "TAGID":
                        dgvAddColumn.Width = 150;
                        break;
                    case "DESCRIPTION":
                        dgvAddColumn.Width = 200;
                        break;
                    case "READTIME":
                        dgvAddColumn.Width = 200;
                        break;
                    default:
                        dgvAddColumn.Width = 80;
                        break;
                }

                dgvTagList.Columns.Add(dgvAddColumn);
            }
        }

        /// <summary>
        /// Active MQ 데이터 처리 이벤트 (MQ에서 데이터가 수신되면 발생)
        /// </summary>
        /// <param name="message"></param>
        private void ReceiveOnMessage(string message)
        {
            DataSet dsReturn = null;
            string messageName = ConvertUtil.getXMLResult(message, "<messagename>", "</messagename>");
            Dictionary<string, string> messageBody = ConvertUtil.getXmlResultDictionary(message, "<body>", "</body>");

            switch (messageName)
            {
                case "SendAcceptJob":
                    TOS.Driver.CLT.Station.Instance.SendMessage(messageName, JsonConvert.SerializeObject(messageBody));
                    break;
                case "AbortJobResponse":
                case "MoveJobResponse":
                case "ClearanceRequest":
                    TOS.Driver.CLT.Station.Instance.SendMessage(messageName, JsonConvert.SerializeObject(messageBody));
                    break;
                case "CurrentStepJobRequest":
                    JobOrderFactoryManager.This().deleteStepJob(message);
                    break;
                case "StepJobRequest":
                    LogManager.Instance.Information($"StepJobRequest : {message}");
                    //JobOrderFactoryManager.This().CreateStepJobInfo(message);

                    JobManager.Instance.Enqueue(new JobOrderManager(message));
                    break;
                case "CraneJobCancelRequest":
                    break;
                case "changeJobStatus":
                    break;
                case "SpreaderPositionData":
                    break;
                case "AssignedToRCS":
                    break;
                case "stepJobEvent":
                    LogManager.Instance.Information($"stepJobEvent : {DateTime.Now.ToString("yyyyMMddHHmmss")}");
                    JobOrderFactoryManager.This().processStepJob();
                    break;
                case "SetInventory":
                    Hashtable htBody = new Hashtable();
                    MessageHandler.SendMessageAsync("SetInventory", htBody);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 현재 작업중인 Step Job 처리 요청
        /// </summary>
        private void getProcessingJobOrder()
        {
            MessageHandler.SendMessageAsync("CurrentStepJobRequest", new Hashtable());
        }
        /// <summary>
        /// 프로그램 종료시 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerEventChecker.Enabled = false;
            TOS.Driver.CLT.Station.Instance.Stop();
            m_pXGTEnetDoc.ProcessEnd();
        }
        /// <summary>
        /// 태그 업데이트 Click 이벤트
        /// </summary>
        /// <param name="sender">전달자</param>
        /// <param name="e"></param>
        private void cmdTagRefresh_Click(object sender, EventArgs e)
        {
            UpdateTagList();
        }

        /// <summary>
        /// 태그 리스트 데이터 업데이트
        /// </summary>
        private void UpdateTagList()
        {
            if (cuDrvName.Equals("") || cuStnName.Equals(""))
            {
                return;
            }

            if (dgvTagList.FirstDisplayedCell == null)
                tagInfoRowIndex = 0;
            else
                tagInfoRowIndex = dgvTagList.FirstDisplayedCell.RowIndex;

            int idx = 0;

            DataTable dtTagList = m_dtTagList.Copy();
            Hashtable htTagMaster = TagMasterManager.This().GetTagMasterHashTable();

            SortedList sortedList = new SortedList(htTagMaster);

            foreach (DictionaryEntry dic in sortedList)
            {
                TagMasterData tagData = (TagMasterData)htTagMaster[dic.Key];

                if (cuStnName.Equals(tagData.sStationID))
                {
                    idx++;

                    DateTime Cur = DateTime.Now;

                    DataRow drTag = dtTagList.NewRow();

                    drTag["NO"] = idx.ToString();
                    drTag["TAGID"] = tagData.sTagID;
                    drTag["ADDRESS"] = tagData.sAddress;
                    drTag["EISVALUE"] = "";
                    drTag["PLCVALUE"] = tagData.sCurrentValue;
                    drTag["DESCRIPTION"] = tagData.sDescription;
                    drTag["READTIME"] = Cur.ToString();
                    drTag["EVENT"] = tagData.sEventFlag;
                    drTag["PARAMETER"] = tagData.sParameterFlag;
                    drTag["WRITE"] = tagData.sCurrentValue;

                    dtTagList.Rows.Add(drTag);
                }
            }

            beStnName = cuStnName;

            UpdateDataTable(dtTagList);

            dgvTagList.FirstDisplayedScrollingRowIndex = tagInfoRowIndex;
        }


        private int GetRowInEditMode()
        {
            if (dgvTagList.IsCurrentCellInEditMode && dgvTagList.CurrentCell != null)
            {
                return dgvTagList.CurrentCell.RowIndex;
            }

            return -1;
        }

        private void UpdateDataTable(DataTable dtTagList)
        {
            if (dgvTagList.DataSource == null)
            {
                dgvTagList.DataSource = dtTagList;
                return;
            }

            DataTable dataSource = (DataTable)dgvTagList.DataSource;


            while (dtTagList.Rows.Count < dataSource.Rows.Count)
            {
                dataSource.Rows.RemoveAt(dataSource.Rows.Count - 1);
            }

            while (dataSource.Rows.Count < dtTagList.Rows.Count)
            {
                dataSource.Rows.Add(dataSource.NewRow());
            }

            int editRow = GetRowInEditMode();

            for (int i = 0; i < dtTagList.Rows.Count; i++)
            {
                DataRow newRow = dtTagList.Rows[i];
                DataRow row = dataSource.Rows[i];
                foreach (DataColumn column in dtTagList.Columns)
                {
                    if (newRow[column.ColumnName] != row[column.ColumnName])
                    {
                        //if (column.ColumnName != "READTIME" && i != 0)
                        //{
                        //    Debug.WriteLine($"[{i}:{column.ColumnName}] old = {row[column.ColumnName]}, new = {newRow[column.ColumnName]} Changed");
                        //}

                        if (i != editRow)
                        {
                            row[column.ColumnName] = newRow[column.ColumnName];
                        }
                        //dgvTagList.Rows[i].Cells[column.ColumnName].Value = newRow[column.ColumnName];
                    }
                }

            }

        }

        /// <summary>
        /// DB 연결
        /// </summary>
        /// <returns>성공 : true, 실패 : false</returns>
        private bool ConnectLocalDB()
        {
            iniFileName = $"{appPath}\\SystemInfo.ini";

            try
            {
                m_cnnLocalDB ??= new SqlConnection();

                if (m_cnnLocalDB.State == ConnectionState.Closed)
                {
                    m_cnnLocalDB.ConnectionString = StaticUtil.getINIValue(iniFileName, "DBCONNECTION", "MAINDBConnection");
                    m_cnnLocalDB.Open();
                }

                m_cmdLocalDB ??= new SqlCommand();

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

        /// <summary>
        /// Interface 초기화 (DB 접속 체크)
        /// </summary>
        /// <returns>성공 : true, 실패 : false</returns>
        public bool IFInitStart()
        {
            appPath = Application.StartupPath;

            if (!ConnectLocalDB())
                return false;

            closeLocalDB();

            return true;
        }

        /// <summary>
        /// DB 연결 해제
        /// </summary>
        private void closeLocalDB()
        {
            if (m_cnnLocalDB == null) return;

            m_cnnLocalDB.Close();
        }

        /// <summary>
        /// Station 정보 조회 ( PLC 정보를 포함한 크레인 리스트 )
        /// </summary>
        /// <returns>조회된 DataSet</returns>
        public DataSet IFGetStnInfo()
        {
            //if (!ConnectLocalDB()) return null;

            string strSQL;
            DataSet ds_StnInfo = new DataSet();
            DataTable dt = new DataTable();

            strSQL = "SELECT '' as PARENTMEMBER ";
            strSQL += " , A.DRIVERNAME as VALUEMEMBER ";
            strSQL += " , 'DRIVER' as LEVELMEMBER ";
            strSQL += " , ROW_NUMBER() over(order by a.drivername) as SORTMEMBER ";
            strSQL += " FROM TB_DRIVERINFO A ";
            strSQL += " UNION ALL ";
            strSQL += " SELECT A.DRIVERNAME as PARENTMEMBER ";
            strSQL += " , B.STATIONID ";
            strSQL += " , 'STATION' as LEVELMEMBER ";
            strSQL += " , ROW_NUMBER() over(order by a.drivername, b.STATIONID) as SORTMEMBER ";
            strSQL += " FROM TB_DRIVERINFO A ";
            strSQL += " INNER JOIN TB_STATIONINFO B ON A.PROTOCOL = B.PROTOCOLNAME ";

            dt = GetDataTable(strSQL, m_cnnLocalDB);
            ds_StnInfo.Tables.Add(dt);

            //closeLocalDB();

            tvStation.initTreeView(dt, "PARENTMEMBER", "VALUEMEMBER", "VALUEMEMBER", "SORTMEMBER", "LEVELMEMBER");
            tvStation.ExpandAll();

            return ds_StnInfo;
        }

        /// <summary>
        /// DB Query 실행
        /// </summary>
        /// <param name="strSQL">실행할 쿼리</param>
        /// <param name="cnnDB">SqlConnection Object</param>
        /// <returns></returns>
        public DataTable GetDataTable(string strSQL, SqlConnection cnnDB)
        {
            DataTable dt = new DataTable();
            try
            {
                //OleDbDataAdapter da = new OleDbDataAdapter(strSQL, cnnDB);
                SqlDataAdapter da = new SqlDataAdapter(strSQL, cnnDB);
                da.SelectCommand.CommandType = CommandType.Text;

                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Exception(ex);
                return null;
            }
        }

        private void mnuCommStatus_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// TOS Accept Job 테스트 버튼 Click 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestAcceptJob_Click(object sender, EventArgs e)
        {
            testMessage();
        }

        /// <summary>
        /// 타이머 이벤트 (1sec Refresh)
        /// JobOrder List에 대한 StepJob 처리 및 Tag 정보 업데이트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerEventChecker_Tick(object sender, EventArgs e)
        {
            //LogManager.Instance.Information($"Self stepJobEvent : {DateTime.Now.ToString("yyyyMMddHHmmss")}");
            //JobOrderFactoryManager.This().processStepJob();
            UpdateTagList();
        }

        /// <summary>
        /// StepJob 테스트 버튼 Click 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestStepJob_Click(object sender, EventArgs e)
        {
            testStepJob();
        }

        /// <summary>
        /// Tag List Cell 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvTagList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvTagList.FirstDisplayedCell == null)
                tagInfoRowIndex = 0;
            else
                tagInfoRowIndex = dgvTagList.FirstDisplayedCell.RowIndex;
        }


        #region 테스트 Method
        private void testMessage()
        {
            string message =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><message>\r\n<header>\r\n<messagename>SendAcceptJob</messagename>\r\n<replysubject></replysubject>\r\n<sourcesubject></sourcesubject>\r\n<targetsubject></targetsubject>\r\n</header>\r\n<body>\r\n<msgId>8814441b-eb96-4cc5-9685-0b4cb10cb8b9</msgId>\r\n<jobID>AAAA8133060363964312744084-840007</jobID>\r\n<eqId>YC01</eqId>\r\n</body>\r\n</message>";

            TOS.Driver.CLT.Station.Instance.SendMessage("SendAcceptJob", JsonConvert.SerializeObject(message));
        }

        private void testStepJob()
        {
            string message =
                @"<message>
    <header>
        <messagename>StepJobRequest</messagename>
        <replysubject></replysubject>
        <sourcesubject></sourcesubject>
        <targetsubject></targetsubject>
    </header>
    <body>
        <message>
            <header>
                <messagename>StepJobRequest</messagename>
                <replysubject></replysubject>
                <sourcesubject></sourcesubject>
                <targetsubject></targetsubject>
                <transactionid></transactionid>
            </header>
            <body>
                <JOBORDERID>864499951847644-527206</JOBORDERID>
                <EQUIPMENTID>CR001</EQUIPMENTID>
                <STEPJOBID>Gantry2</STEPJOBID>
                <STEPJOBTYPE>StartEnd</STEPJOBTYPE>
                <STEPSEQUENCE>1000</STEPSEQUENCE>
                <COMPOSITIONID>MV-1000-Gantry2</COMPOSITIONID>
                <DATALIST>
                    <DATAINFO>
                        <ACTIONTYPE>End</ACTIONTYPE>
                        <PARAMETERID>moveEndReport</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW4086</TAGID>
                        <DATATYPE>BOOL</DATATYPE>
                        <ADDRESS>MW4086</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>EventReport</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>End</ACTIONTYPE>
                        <PARAMETERID>moveEndReportReply</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2387</TAGID>
                        <DATATYPE>BOOL</DATATYPE>
                        <ADDRESS>MW2387</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE>1</TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>EventReply</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveGantryTargetBayFromTos</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2240</TAGID>
                        <DATATYPE>USHORT</DATATYPE>
                        <ADDRESS>MW2240</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE>04</TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveGantryTargetBayPos</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2236</TAGID>
                        <DATATYPE>INTEGER</DATATYPE>
                        <ADDRESS>MW2236</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveGantryTargetBlockFromTos</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2242</TAGID>
                        <DATATYPE>STRING</DATATYPE>
                        <ADDRESS>MW2242</ADDRESS>
                        <STRINGLEN>16</STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveHoistTargetTierFromTos</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2238</TAGID>
                        <DATATYPE>USHORT</DATATYPE>
                        <ADDRESS>MW2238</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveHoistTargetTierPos</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2232</TAGID>
                        <DATATYPE>INTEGER</DATATYPE>
                        <ADDRESS>MW2232</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE>-1</TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveReport</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2385</TAGID>
                        <DATATYPE>BOOL</DATATYPE>
                        <ADDRESS>MW2385</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE>1</TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>EventReport</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveReportErrorCode</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW4003</TAGID>
                        <DATATYPE>USHORT</DATATYPE>
                        <ADDRESS>MW4003</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveReportReply</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW4084</TAGID>
                        <DATATYPE>BOOL</DATATYPE>
                        <ADDRESS>MW4084</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>EventReply</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveSpreaderHeight</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2262</TAGID>
                        <DATATYPE>INTEGER</DATATYPE>
                        <ADDRESS>MW2262</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE>1500</TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveSpreaderPosition</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2260</TAGID>
                        <DATATYPE>INTEGER</DATATYPE>
                        <ADDRESS>MW2260</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE>9150</TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveSpreaderRowNumber</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2258</TAGID>
                        <DATATYPE>USHORT</DATATYPE>
                        <ADDRESS>MW2258</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveSpreaderSize</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2264</TAGID>
                        <DATATYPE>USHORT</DATATYPE>
                        <ADDRESS>MW2264</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveStartReport</PARAMETERID>
                        <PARAMETERLEVEL>2</PARAMETERLEVEL>
                        <TAGID>CR001_MW4085</TAGID>
                        <DATATYPE>BOOL</DATATYPE>
                        <ADDRESS>MW4085</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>EventReport</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveStartReportReply</PARAMETERID>
                        <PARAMETERLEVEL>2</PARAMETERLEVEL>
                        <TAGID>CR001_MW2386</TAGID>
                        <DATATYPE>BOOL</DATATYPE>
                        <ADDRESS>MW2386</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE>1</TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>EventReply</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveStepJobId</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2200</TAGID>
                        <DATATYPE>STRING</DATATYPE>
                        <ADDRESS>MW2200</ADDRESS>
                        <STRINGLEN>16</STRINGLEN>
                        <TAGVALUE>Gantry2</TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveStepJobType</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2216</TAGID>
                        <DATATYPE>STRING</DATATYPE>
                        <ADDRESS>MW2216</ADDRESS>
                        <STRINGLEN>16</STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveTrolleyTargetRowFromTos</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2239</TAGID>
                        <DATATYPE>USHORT</DATATYPE>
                        <ADDRESS>MW2239</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                    <DATAINFO>
                        <ACTIONTYPE>Start</ACTIONTYPE>
                        <PARAMETERID>moveTrolleyTargetRowPos</PARAMETERID>
                        <PARAMETERLEVEL>1</PARAMETERLEVEL>
                        <TAGID>CR001_MW2234</TAGID>
                        <DATATYPE>INTEGER</DATATYPE>
                        <ADDRESS>MW2234</ADDRESS>
                        <STRINGLEN></STRINGLEN>
                        <TAGVALUE></TAGVALUE>
                        <DATAACTIONTYPE>Set</DATAACTIONTYPE>
                        <DATAPROCESSTYPE>Data</DATAPROCESSTYPE>
                        <RECEIVEDID></RECEIVEDID>
                        <DATATARGET></DATATARGET>
                        <DATAFAILTARGET></DATAFAILTARGET>
                    </DATAINFO>
                </DATALIST>
            </body>
        </message>
    </body>
</message>";

            JobOrderFactoryManager.This().CreateStepJobInfo(message);
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}

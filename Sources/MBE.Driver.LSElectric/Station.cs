using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KR.MBE.CommonLibrary.Interface;
using KR.MBE.CommonLibrary.Manager;
using KR.MBE.CommonLibrary.Struct;
using MBE.Driver.LSElectric.FEnet;
using MBE.Driver.LSEletric;
using Newtonsoft.Json.Linq;


namespace MBE.Driver.LSElectric
{

    public partial class Station : PlcInterface
    {
        public DataManager p_DataLibrary;

        public PlcCommBlock[] m_Commblock;
        public TagBaseList m_TagList;

        public TagMasterManager CTagManager;

        public Hashtable m_htTagDef = null;

        public string EQUIPMENTID { get; set; }
        public string EQUIPMENTNAME;
        public string STATIONID { get; set; }
        public string PROTOCOLNAME { get; set; } = "LSIS-XGIENET";
        public string LOCALIP;
        public int    LOCALPORT;   // 
        public string REMOTEIP;
        public int    REMOTEPORT;
        public int    RACKNO;                    // Siemens PLC에서 Enet Card Rack Number
        public int    CPUSLOTNO;                 // Siemens PLC에서 CPU Slot Number
        public int    STATIONNO;                 // Melsec PLC에서 Station Number

        public string EquipmentType { get; set; }
        public string EquipmentDetailType;
        public string SuperEquipmentID;

        public bool   bUseFlag;
        public bool   bAutoFlag;

        public string IfMode;            // None, Tag, MSG, … , AutoFlag = 'Yes' 인경우 필수 현재는 TAG
        public string EquipmentIP;         // Crane/PLC IP Address, RCS에 전달 용도, EIS 통신용으로 사용안함.
        public int    Port;

        public int    COMMINTERVAL;              // 통신 주기( * 100ms)
        public int    TIMEOUT;                   // 통신 타임아웃( * 100ms)
        public string OPCSERVER;                 // OPC SERVER Name
        public string OPCLOCALREMOTE;            // OPC Server PC Name or PC IP
        public string OPCCLSID;                  // OPC Server Class ID(CLSID)
        public bool   COMMSTATUS;

        public string[] CPUType = { "XGK", "XGI", "XGR", "XGB(MK)", "XGB(IEC)" };

        public bool   InitStart;
        public bool   commEnable { get; set; }                // 통신 Active

        public bool commStatus { get; set; } // 통신 상태

        public int    nBlkNum;               // 통신블럭 Number
        public int    nTagNum;               // Tag Number

        public long t_beforeTick;

        public WriteQueue<KeyValuePair<string, string>> writeQueue;

        //public CCommQueue WriteQueue = null;
        
        public int p_ScanTime;

        public Station()
        {
            p_DataLibrary = new DataManager();

            p_ScanTime = p_DataLibrary.GetScanTime();

            commEnable = false;
            writeQueue = new WriteQueue<KeyValuePair<string, string>>();

            nRxCnt = 0;
            nRxErr = 0;
            nTxCnt = 0;
            nTxErr = 0;
            nErrCnt = 0;

            nTaskID = 0;

            m_htTagDef = new Hashtable();
            CTagManager = TagMasterManager.This();

            InitStart = true;
        }

        ~Station()
        {
            m_Commblock = null;
            m_TagList = null;
            writeQueue = null;

            m_htTagDef = null;
            CTagManager = null;
        }

        public TagBaseList GetTagList()
        {
            return m_TagList;
        } 

        public void SetStation(DataRow drRow)
        {
            try
            {

                EQUIPMENTID = drRow["EQUIPMENTID"].ToString();

                if (drRow.IsNull("EQUIPMENTNAME"))
                    EQUIPMENTNAME = "";
                else
                    EQUIPMENTNAME = drRow["EQUIPMENTNAME"].ToString();

                if (drRow.IsNull("EQUIPMENTTYPE"))
                    EquipmentType = "";
                else
                    EquipmentType = drRow["EQUIPMENTTYPE"].ToString();

                if (drRow.IsNull("EQUIPMENTDETAILTYPE"))
                    EquipmentDetailType = "";
                else
                    EquipmentDetailType = drRow["EQUIPMENTDETAILTYPE"].ToString();

                if (drRow.IsNull("SUPEREQUIPMENTID"))
                    SuperEquipmentID = "";
                else
                    SuperEquipmentID = drRow["SUPEREQUIPMENTID"].ToString();

                if (drRow.IsNull("USEFLAG"))
                    bUseFlag = false;
                else
                {
                    if (drRow["USEFLAG"].ToString() == "YES")
                        bUseFlag = true;
                    else
                        bUseFlag = false;
                }

                if (drRow.IsNull("AUTOFLAG"))
                    bAutoFlag = false;
                else
                {
                    if (drRow["AUTOFLAG"].ToString() == "YES")
                        bAutoFlag = true;
                    else
                        bAutoFlag = false;
                }

                if (drRow.IsNull("IFMODE"))
                    IfMode = "";
                else
                    IfMode = drRow["IFMODE"].ToString();

                if (drRow.IsNull("IPADDRESS"))
                    EquipmentIP = "";
                else
                    EquipmentIP = drRow["IPADDRESS"].ToString();

                if (drRow.IsNull("PORT") || drRow["PORT"].ToString() == string.Empty)
                    Port = 0;
                else
                    Port = Convert.ToInt32(drRow["PORT"].ToString());


                if (drRow.IsNull("STATIONID"))
                    STATIONID = "";
                else
                    STATIONID = drRow["STATIONID"].ToString();

                if (drRow.IsNull("PROTOCOLNAME"))
                    PROTOCOLNAME = "";
                else
                    PROTOCOLNAME = drRow["PROTOCOLNAME"].ToString();

                if (drRow.IsNull("LOCALIP"))
                    LOCALIP = "";
                else
                    LOCALIP = drRow["LOCALIP"].ToString();

                if (drRow.IsNull("LOCALPORT") || drRow["LOCALPORT"].ToString() == string.Empty)
                    LOCALPORT = 0;
                else
                    LOCALPORT = Convert.ToInt32(drRow["LOCALPORT"].ToString());

                if (drRow.IsNull("REMOTEIP"))
                    REMOTEIP = "";
                else
                    REMOTEIP = drRow["REMOTEIP"].ToString();

                if (drRow.IsNull("REMOTEPORT") || drRow["REMOTEPORT"].ToString() == string.Empty)
                    REMOTEPORT = 0;
                else
                    REMOTEPORT = Convert.ToInt32(drRow["REMOTEPORT"].ToString());

                if (drRow.IsNull("PORT") || drRow["RACKNO"].ToString() == string.Empty)
                    RACKNO = 0;
                else
                    RACKNO = Convert.ToInt32(drRow["RACKNO"].ToString());

                if (drRow.IsNull("CPUSLOTNO") || drRow["CPUSLOTNO"].ToString() == string.Empty)
                    CPUSLOTNO = 0;
                else
                    CPUSLOTNO = Convert.ToInt32(drRow["CPUSLOTNO"].ToString());

                if (drRow.IsNull("STATIONNO") || drRow["STATIONNO"].ToString() == string.Empty)
                    STATIONNO = 0;
                else
                    STATIONNO = Convert.ToInt32(drRow["STATIONNO"].ToString());

                if (drRow.IsNull("COMMINTERVAL") || drRow["COMMINTERVAL"].ToString() == string.Empty)
                    COMMINTERVAL = 0;
                else
                    COMMINTERVAL = Convert.ToInt32(drRow["COMMINTERVAL"].ToString());


                if (drRow.IsNull("TIMEOUT") || drRow["TIMEOUT"].ToString() == string.Empty)
                    TIMEOUT = 0;
                else
                    TIMEOUT = Convert.ToInt32(drRow["TIMEOUT"].ToString());

                if (drRow.IsNull("OPCSERVER"))
                    OPCSERVER = "";
                else
                    OPCSERVER = drRow["OPCSERVER"].ToString();

                if (drRow.IsNull("OPCLOCALREMOTE"))
                    OPCLOCALREMOTE = "";
                else
                    OPCLOCALREMOTE = drRow["OPCLOCALREMOTE"].ToString();

                if (drRow.IsNull("OPCCLSID"))
                    OPCCLSID = "";
                else
                    OPCCLSID = drRow["OPCCLSID"].ToString();


                SetCommBlock();
                SetTagList();
                for (int i = 0; i < m_TagList.Count; i++)
                {
                    m_TagList[i].SetCommTagInfo();

                    m_TagList[i].SetBlockIndex(m_Commblock);
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void SetCommBlock()
        {
            bool ret = p_DataLibrary.OpenMainDB();

            if (!ret)
            {
                string sMsg = "Cannot open the SQL-Server Databse";

                LogManager.Instance.Error(sMsg);
                return;
            }

            DataTable dtCommBlock = new DataTable();
            dtCommBlock = p_DataLibrary.GetCommBlock(EQUIPMENTID, STATIONID);

            nBlkNum = dtCommBlock.Rows.Count;

            if (nBlkNum < 1)
            {
                p_DataLibrary.CloseMainDB();
                return;
            }

            CcommBlock[] m_CommblockTemp = new CcommBlock[nBlkNum];

            for (int i = 0; i < nBlkNum; i++)
            {
                m_CommblockTemp[i] = new CcommBlock();

                m_CommblockTemp[i].EQUIPMENTID = dtCommBlock.Rows[i]["EQUIPMENTID"].ToString();
                m_CommblockTemp[i].STATIONID = dtCommBlock.Rows[i]["STATIONID"].ToString();

                if (dtCommBlock.Rows[i].IsNull("BLOCKNO") || dtCommBlock.Rows[i]["BLOCKNO"].ToString() == string.Empty)
                    m_CommblockTemp[i].BLOCKNO = 0;
                else
                {
                    m_CommblockTemp[i].BLOCKNO = Convert.ToInt32(dtCommBlock.Rows[i]["BLOCKNO"].ToString());
                    m_CommblockTemp[i].d_BlockIdx = i;
                }

                if (dtCommBlock.Rows[i].IsNull("BLOCKTYPE"))
                    m_CommblockTemp[i].DEVCODE = "";
                else
                    m_CommblockTemp[i].DEVCODE = dtCommBlock.Rows[i]["BLOCKTYPE"].ToString();

                if (dtCommBlock.Rows[i].IsNull("STARTADDRESS") || dtCommBlock.Rows[i]["STARTADDRESS"].ToString() == string.Empty)
                    m_CommblockTemp[i].STARTADDRESS = 0;
                else
                    m_CommblockTemp[i].STARTADDRESS = Convert.ToInt32(dtCommBlock.Rows[i]["STARTADDRESS"].ToString());

                if (dtCommBlock.Rows[i].IsNull("READDATANUMBER") || dtCommBlock.Rows[i]["READDATANUMBER"].ToString() == string.Empty)
                    m_CommblockTemp[i].READDATANUMBER = 0;
                else
                    m_CommblockTemp[i].READDATANUMBER = Convert.ToInt32(dtCommBlock.Rows[i]["READDATANUMBER"].ToString());

                if (dtCommBlock.Rows[i].IsNull("COMMINTERVAL") || dtCommBlock.Rows[i]["COMMINTERVAL"].ToString() == string.Empty)
                    m_CommblockTemp[i].COMMINTERVAL = 0;
                else
                    m_CommblockTemp[i].COMMINTERVAL = Convert.ToInt32(dtCommBlock.Rows[i]["COMMINTERVAL"].ToString()) * 1000;
            }

            m_Commblock = m_CommblockTemp;

            p_DataLibrary.CloseMainDB();
        }

        public void SetTagList()
        {
            bool ret = p_DataLibrary.OpenMainDB();
            m_TagList = new TagBaseList();

            if (!ret)
            {
                string sMsg = "Cannot open the SQL-Server Databse";
                LogManager.Instance.Error(sMsg);
                return;
            }

            DataTable dtTagList = p_DataLibrary.GetTagList(EQUIPMENTID, STATIONID);

            if (dtTagList == null)
            {
                LogManager.Instance.Error("Cannot get the Tag List");
                p_DataLibrary.CloseMainDB();
                return;
            }

            if (dtTagList.Rows.Count < 1)
            {
                LogManager.Instance.Error("Cannot get the Tag List");
                p_DataLibrary.CloseMainDB();
                return;
            }

            foreach (DataRow row in dtTagList.Rows)
            {
                var tag = new CTagBase
                {
                    EQUIPMENTID = row["EQUIPMENTID"].ToString(),
                    STATIONID = row["STATIONID"].ToString(),
                    TAGID = row.IsNull("TAGID") ? "" : row["TAGID"].ToString(),
                    TAGNAME = row.IsNull("TAGNAME") ? "" : row["TAGNAME"].ToString(),
                    DESCRIPTION = row.IsNull("DESCRIPTION") ? "" : row["DESCRIPTION"].ToString(),
                    USEFLAG = row.IsNull("USEFLAG") ? "" : row["USEFLAG"].ToString(),
                    TAGKIND = row.IsNull("TAGKIND") ? "" : row["TAGKIND"].ToString(),
                    TAGTYPE = row.IsNull("TAGTYPE") ? "" : row["TAGTYPE"].ToString(),
                    DRIVERID = row.IsNull("DRIVERID") ? "" : row["DRIVERID"].ToString(),
                    DRIVERNAME = row.IsNull("DRIVERNAME") ? "" : row["DRIVERNAME"].ToString(),
                    ADDRESS = row.IsNull("ADDRESS") ? "" : row["ADDRESS"].ToString(),
                    DATATYPE = row.IsNull("DATATYPE") ? "" : row["DATATYPE"].ToString(),
                    EVENTFLAG = row.IsNull("EVENTFLAG") ? "NO" : row["EVENTFLAG"].ToString(),
                    PARAMETERFLAG = row.IsNull("PARAMETERFLAG") ? "NO" : row["PARAMETERFLAG"].ToString(),
                    STRINGLEN = row.IsNull("STRINGLEN") || row["STRINGLEN"].ToString() == string.Empty
                        ? 0
                        : Convert.ToInt32(row["STRINGLEN"].ToString()),
                    WRITEUSE = row.IsNull("WRITEUSE") ? "" : row["WRITEUSE"].ToString(),
                    SCALEUSE = row.IsNull("SCALEUSE") ? "" : row["SCALEUSE"].ToString(),
                    SCALEVALUE = row.IsNull("SCALEVALUE") || row["SCALEVALUE"].ToString() == string.Empty
                        ? 0
                        : Convert.ToDouble(row["SCALEVALUE"].ToString()),
                    OFFSET = row.IsNull("OFFSET") || row["OFFSET"].ToString() == string.Empty
                        ? 0
                        : Convert.ToDouble(row["OFFSET"].ToString()),
                    PLCMIN = row.IsNull("PLCMIN") || row["PLCMIN"].ToString() == string.Empty
                        ? 0
                        : Convert.ToDouble(row["PLCMIN"].ToString()),
                    PLCMAX = row.IsNull("PLCMAX") || row["PLCMAX"].ToString() == string.Empty
                        ? 0
                        : Convert.ToDouble(row["PLCMAX"].ToString()),
                    TAGMIN = row.IsNull("TAGMIN") || row["TAGMIN"].ToString() == string.Empty
                        ? 0
                        : Convert.ToDouble(row["TAGMIN"].ToString()),
                    TAGMAX = row.IsNull("TAGMAX") || row["TAGMAX"].ToString() == string.Empty
                        ? 0
                        : Convert.ToDouble(row["TAGMAX"].ToString()),
                    TAGVALUE = row.IsNull("TAGVALUE") || row["TAGVALUE"].ToString() == string.Empty
                        ? 0
                        : row["TAGVALUE"],
                    SETVALUE = row.IsNull("SETVALUE") || row["SETVALUE"].ToString() == string.Empty
                        ? 0
                        : Convert.ToDouble(row["SETVALUE"].ToString()),
                    MINVALUE = row.IsNull("MINVALUE") || row["MINVALUE"].ToString() == string.Empty
                        ? 0
                        : Convert.ToDouble(row["MINVALUE"].ToString()),
                    MAXVALUE = row.IsNull("MAXVALUE") || row["MAXVALUE"].ToString() == string.Empty
                        ? 0
                        : Convert.ToDouble(row["MAXVALUE"].ToString()),
                    INTERVAL = row.IsNull("INTERVAL") || row["INTERVAL"].ToString() == string.Empty
                        ? 0
                        : Convert.ToInt32(row["INTERVAL"].ToString()),
                    UNIT = row.IsNull("UNIT") ? "" : row["UNIT"].ToString(),
                    ALARMFLAG = row.IsNull("ALARMFLAG") ? "NO" : row["ALARMFLAG"].ToString(),
                    MONITORINGFLAG = row.IsNull("MONITORINGFLAG") ? "NO" : row["MONITORINGFLAG"].ToString(),
                    HEARTBEATFLAG = row.IsNull("HEARTBEATFLAG") ? "NO" : row["HEARTBEATFLAG"].ToString(),
                    COSFLAG = row.IsNull("COSFLAG") ? "NO" : row["COSFLAG"].ToString(),
                    COSPARAMETERNAME = row.IsNull("COSPARAMETERNAME") ? "" : row["COSPARAMETERNAME"].ToString(),
                    PARAMETERID = row.IsNull("PARAMETERID") ? "" : row["PARAMETERID"].ToString(),
                    station = this
                };

                m_TagList.Add(tag);
            }

            p_DataLibrary.CloseMainDB();
            /*
            for (int i = 0; i < nTagNum; i++)
            {
                m_TagList[i] = new CTagBase();

                //m_TagList[i].EQUIPMENTID = dtTagList.Rows[i]["EQUIPMENTID"].ToString();
                //m_TagList[i].STATIONID = dtTagList.Rows[i]["STATIONID"].ToString();

                //if (dtTagList.Rows[i].IsNull("TAGID"))
                //    m_TagList[i].TAGID = "";
                //else
                //    m_TagList[i].TAGID = dtTagList.Rows[i]["TAGID"].ToString();

                //if (dtTagList.Rows[i].IsNull("TAGNAME"))
                //    m_TagList[i].TAGNAME = "";
                //else
                //    m_TagList[i].TAGNAME = dtTagList.Rows[i]["TAGNAME"].ToString();

                //if (dtTagList.Rows[i].IsNull("DESCRIPTION"))
                //    m_TagList[i].DESCRIPTION = "";
                //else
                //    m_TagList[i].DESCRIPTION = dtTagList.Rows[i]["DESCRIPTION"].ToString();

                //if (dtTagList.Rows[i].IsNull("USEFLAG"))
                //    m_TagList[i].USEFLAG = "";
                //else
                //    m_TagList[i].USEFLAG = dtTagList.Rows[i]["USEFLAG"].ToString();

                //if (dtTagList.Rows[i].IsNull("TAGKIND"))
                //    m_TagList[i].TAGKIND = "";
                //else
                //    m_TagList[i].TAGKIND = dtTagList.Rows[i]["TAGKIND"].ToString();

                //if (dtTagList.Rows[i].IsNull("TAGTYPE"))
                //    m_TagList[i].TAGTYPE = "";
                //else
                //    m_TagList[i].TAGTYPE = dtTagList.Rows[i]["TAGTYPE"].ToString();

                //if (dtTagList.Rows[i].IsNull("DRIVERID"))
                //    m_TagList[i].DRIVERID = "";
                //else
                //    m_TagList[i].DRIVERID = dtTagList.Rows[i]["DRIVERID"].ToString();

                //if (dtTagList.Rows[i].IsNull("DRIVERNAME"))
                //    m_TagList[i].DRIVERNAME = "";
                //else
                //    m_TagList[i].DRIVERNAME = dtTagList.Rows[i]["DRIVERNAME"].ToString();

                //if (dtTagList.Rows[i].IsNull("ADDRESS"))
                //    m_TagList[i].ADDRESS = "";
                //else
                //    m_TagList[i].ADDRESS = dtTagList.Rows[i]["ADDRESS"].ToString();

                //if (dtTagList.Rows[i].IsNull("DATATYPE"))
                //    m_TagList[i].DATATYPE = "";
                //else
                //    m_TagList[i].DATATYPE = dtTagList.Rows[i]["DATATYPE"].ToString();

                //if (dtTagList.Rows[i].IsNull("EVENTFLAG"))
                //    m_TagList[i].EVENTFLAG = "NO";
                //else
                //    m_TagList[i].EVENTFLAG = dtTagList.Rows[i]["EVENTFLAG"].ToString();

                //if (dtTagList.Rows[i].IsNull("PARAMETERFLAG"))
                //    m_TagList[i].PARAMETERFLAG = "NO";
                //else
                //    m_TagList[i].PARAMETERFLAG = dtTagList.Rows[i]["PARAMETERFLAG"].ToString();

                //if (dtTagList.Rows[i].IsNull("STRINGLEN") || dtTagList.Rows[i]["STRINGLEN"].ToString() == string.Empty)
                //    m_TagList[i].STRINGLEN = 0;
                //else
                //    m_TagList[i].STRINGLEN = Convert.ToInt32(dtTagList.Rows[i]["STRINGLEN"].ToString());

                //if (dtTagList.Rows[i].IsNull("WRITEUSE"))
                //    m_TagList[i].WRITEUSE = "";
                //else
                //    m_TagList[i].WRITEUSE = dtTagList.Rows[i]["WRITEUSE"].ToString();

                //if (dtTagList.Rows[i].IsNull("SCALEUSE"))
                //    m_TagList[i].SCALEUSE = "";
                //else
                //    m_TagList[i].SCALEUSE = dtTagList.Rows[i]["SCALEUSE"].ToString();

                //if (dtTagList.Rows[i].IsNull("SCALEVALUE") || dtTagList.Rows[i]["SCALEVALUE"].ToString() == string.Empty)
                //    m_TagList[i].SCALEVALUE = 0;
                //else
                //    m_TagList[i].SCALEVALUE = Convert.ToDouble(dtTagList.Rows[i]["SCALEVALUE"].ToString());

                //if (dtTagList.Rows[i].IsNull("OFFSET") || dtTagList.Rows[i]["OFFSET"].ToString() == string.Empty)
                //    m_TagList[i].OFFSET = 0;
                //else
                //    m_TagList[i].OFFSET = Convert.ToDouble(dtTagList.Rows[i]["OFFSET"].ToString());

                //if (dtTagList.Rows[i].IsNull("PLCMIN") || dtTagList.Rows[i]["PLCMIN"].ToString() == string.Empty)
                //    m_TagList[i].PLCMIN = 0;
                //else
                //    m_TagList[i].PLCMIN = Convert.ToDouble(dtTagList.Rows[i]["PLCMIN"].ToString());

                //if (dtTagList.Rows[i].IsNull("PLCMAX") || dtTagList.Rows[i]["PLCMAX"].ToString() == string.Empty)
                //    m_TagList[i].PLCMAX = 0;
                //else
                //    m_TagList[i].PLCMAX = Convert.ToDouble(dtTagList.Rows[i]["PLCMAX"].ToString());

                if (dtTagList.Rows[i].IsNull("TAGMIN") || dtTagList.Rows[i]["TAGMIN"].ToString() == string.Empty)
                    m_TagList[i].TAGMIN = 0;
                else
                    m_TagList[i].TAGMIN = Convert.ToDouble(dtTagList.Rows[i]["TAGMIN"].ToString());

                if (dtTagList.Rows[i].IsNull("TAGMAX") || dtTagList.Rows[i]["TAGMAX"].ToString() == string.Empty)
                    m_TagList[i].TAGMAX = 0;
                else
                    m_TagList[i].TAGMAX = Convert.ToDouble(dtTagList.Rows[i]["TAGMAX"].ToString());

                if (dtTagList.Rows[i].IsNull("TAGVALUE") || dtTagList.Rows[i]["TAGVALUE"].ToString() == string.Empty)
                    m_TagList[i].TAGVALUE = 0;
                else
                    m_TagList[i].TAGVALUE = Convert.ToDouble(dtTagList.Rows[i]["TAGVALUE"].ToString());

                if (dtTagList.Rows[i].IsNull("SETVALUE") || dtTagList.Rows[i]["SETVALUE"].ToString() == string.Empty)
                    m_TagList[i].SETVALUE = 0;
                else
                    m_TagList[i].SETVALUE = Convert.ToDouble(dtTagList.Rows[i]["SETVALUE"].ToString());

                if (dtTagList.Rows[i].IsNull("MINVALUE") || dtTagList.Rows[i]["MINVALUE"].ToString() == string.Empty)
                    m_TagList[i].MINVALUE = 0;
                else
                    m_TagList[i].MINVALUE = Convert.ToDouble(dtTagList.Rows[i]["MINVALUE"].ToString());

                if (dtTagList.Rows[i].IsNull("MAXVALUE") || dtTagList.Rows[i]["MAXVALUE"].ToString() == string.Empty)
                    m_TagList[i].MAXVALUE = 0;
                else
                    m_TagList[i].MAXVALUE = Convert.ToDouble(dtTagList.Rows[i]["MAXVALUE"].ToString());

                if (dtTagList.Rows[i].IsNull("INTERVAL") || dtTagList.Rows[i]["INTERVAL"].ToString() == string.Empty)
                    m_TagList[i].INTERVAL = 0;
                else
                    m_TagList[i].INTERVAL = Convert.ToInt32(dtTagList.Rows[i]["INTERVAL"].ToString());

                if (dtTagList.Rows[i].IsNull("UNIT"))
                    m_TagList[i].UNIT = "";
                else
                    m_TagList[i].UNIT = dtTagList.Rows[i]["UNIT"].ToString();

                if (dtTagList.Rows[i].IsNull("ALARMFLAG"))
                    m_TagList[i].ALARMFLAG = "NO";
                else
                    m_TagList[i].ALARMFLAG = dtTagList.Rows[i]["ALARMFLAG"].ToString();

                if (dtTagList.Rows[i].IsNull("MONITORINGFLAG"))
                    m_TagList[i].MONITORINGFLAG = "NO";
                else
                    m_TagList[i].MONITORINGFLAG = dtTagList.Rows[i]["MONITORINGFLAG"].ToString();

                if (dtTagList.Rows[i].IsNull("COSFLAG"))
                    m_TagList[i].COSFLAG = "NO";
                else
                    m_TagList[i].COSFLAG = dtTagList.Rows[i]["COSFLAG"].ToString();

                if (dtTagList.Rows[i].IsNull("COSPARAMETERNAME"))
                    m_TagList[i].COSPARAMETERNAME = "";
                else
                    m_TagList[i].COSPARAMETERNAME = dtTagList.Rows[i]["COSPARAMETERNAME"].ToString();


            }*/
        }

        public bool WriteTag(DataRow dr)
        {
            int i;
            string Addr, DataType, Value;


            if (dr["DATAACTIONTYPE"].ToString().ToUpper().Equals("SET"))
            {
                DataType = dr["DATATYPE"].ToString().ToUpper().Trim();
                Addr = dr["ADDRESS"].ToString().Trim();
                Value = dr["TAGVALUE"].ToString().Trim();

                var requestData = new FEnetWriteSingleRequest(LSElectric.DataType.Word)
                {
                    [$"%{Addr}"] = int.Parse(Value)
                };

                var nakResponse = _clientSocket.Request(requestData) as FEnetNAKResponse;

                if (nakResponse != null)
                {
                    LogManager.Instance.Channel("Exception",
                        $"쓰기 오류 발생:{nakResponse.NAKCode}, {nakResponse.NAKCodeValue}");
                }
            }

            return true;
        }

        public bool WriteTag(string tagId, string value)
        {
            ushort ByteAddr;
            List<byte> tmpBytes = new List<byte>();
            
            var tag = m_TagList.FindByTagId(tagId);

            if (tag == null)
            {
                LogManager.Instance.Error($"Tag ID {tagId} not found in the tag list.");

                return false;
            }

            ByteAddr = (ushort)(ushort.Parse(tag.ADDRESS.Substring(2)) * 2);
            LogManager.Instance.Debug($"Tag ID {tagId}({ByteAddr}) = {value}");

            try
            {
                switch (tag.DATATYPE)
                {
                    case "BOOL":
                    case "UINT8":
                    case "UBCD8":
                    case "UINT16":
                    case "UBCD16":
                    case "USINT":
                    case "UINT":
                    case "USHORT":
                        tmpBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(value)));
                        break;
                    case "BCD8":
                    case "INT8":
                    case "INT16":
                    case "BCD16":
                    case "SINT":
                    case "INT":
                    case "INTEGER":
                        tmpBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(value)));
                        break;
                    case "INT32":
                    case "BCD32":
                    case "DINT":
                        tmpBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(value)));
                        break;
                    case "UINT32":
                    case "UBCD32":
                    case "UDINT":
                        tmpBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(value)));
                        break;
                    case "INT64":
                    case "LINT":
                        tmpBytes.AddRange(BitConverter.GetBytes(Convert.ToInt64(value)));
                        break;
                    case "UINT64":
                    case "ULINT":
                        tmpBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt64(value)));
                        break;
                    case "FLOAT":
                    case "REAL":
                        tmpBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(value)));
                        break;
                    case "DOUBLE":
                    case "LREAL":
                        tmpBytes.AddRange(BitConverter.GetBytes(Convert.ToDouble(value)));
                        break;
                    case "STRING":
                        tmpBytes.AddRange(Encoding.UTF8.GetBytes(value));

                        if (tag.d_ByteSize > tmpBytes.Count)
                        {
                            for (int i = 0; i < tag.d_ByteSize - tmpBytes.Count; i++)
                            {
                                tmpBytes.Add(0);
                            }
                        }
                        break;
                }

                foreach (var b in tmpBytes)
                {
                    _clientSocket.Write(DeviceType.M, ByteAddr, b);
                    ByteAddr++;
                    Thread.Sleep(100);
                }

                tag.SendTick = DateTime.Now.Ticks;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string WriteTag(DataTable dt)
        {
            try
            {
                int i;
                string Addr, DataType, Value;

                if (dt.Rows.Count < 1)
                    return "FAIL";

                int rowCount = dt.Rows.Count;

                for (i = 0; i < rowCount; i++)
                {
                    if (dt.Rows[i]["DATAACTIONTYPE"].Equals("SET"))
                    {
                        DataType = dt.Rows[i]["DATATYPE"].ToString().ToUpper().Trim();
                        Addr = dt.Rows[i]["ADDRESS"].ToString().Trim();
                        Value = dt.Rows[i]["VALUE"].ToString().Trim();

                        var requestData = new FEnetWriteSingleRequest(LSElectric.DataType.Word)
                        {
                            [$"%{Addr}"] = int.Parse(Value)
                        };

                        var nakResponse = _clientSocket.Request(requestData) as FEnetNAKResponse;

                        if (nakResponse != null)
                        {
                            LogManager.Instance.Channel("Exception",
                                $"쓰기 오류 발생:{nakResponse.NAKCode}, {nakResponse.NAKCodeValue}");
                        }
                    }
                }

                return "SUCCESS";
            }
            catch
            {
                return "COMM ERROR";
            }
        }
    }
}

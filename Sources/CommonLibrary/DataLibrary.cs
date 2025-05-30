using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using EventLogger;

namespace CommonLibrary
{
    public class DataLibrary
    {
        private string appPath;
        private string iniFileName;

        private SqlConnection g_cnnMainDB;
        private SqlCommand g_cmdMain;

        public DataLibrary()
        {
            appPath = Application.StartupPath;
            iniFileName = appPath + @"\SystemInfo.ini";


            g_cnnMainDB = new SqlConnection();

            g_cmdMain = new SqlCommand();
        }

        public int GetDriverCount()
        {
            string strSQL;
            DataTable dtData;

            strSQL = "SELECT DriverName FROM Driver ORDER BY DriverName";
            dtData = GetDataTable(strSQL);

            return dtData.Rows.Count;
        }

        public DataTable GetCraneTable()
        {
            string strSQL;
            DataTable dtData;

            strSQL = "SELECT EQUIPMENTID, " +
                            "EQUIPMENTNAME, " +
                            "Upper(EQUIPMENTTYPE), " +
                            "EQUIPMENTDETAILTYPE, " +
                            "SUPEREQUIPMENTID, " +
                            "Upper(USEFLAG), " +
                            "Upper((AUTOFLAG), " +
                            "Upper((IFMODE), " +
                            "IPADDRESS, " +
                            "PORT " +
                     "FROM EQUIPMENTDEFINITION " +
                     "WHERE Upper(EQUIPMENTTYPE) = 'CRANE' " +
                     "ORDER BY EQUIPMENTID";

            dtData = GetDataTable(strSQL);

            return dtData;
        }

        public DataTable GetStationTable(string sEquipmentID)
        {
            string strSQL;
            DataTable dtData;

            strSQL = "SELECT EQUIPMENTID, " +
                            "STATIONID, " +
                            "PROTOCOLNAME, " +
                            "LOCALIP, " +
                            "LOCALPORT, " +
                            "REMOTEIP, " +
                            "REMOTEPORT, " +
                            "RACKNO, " +
                            "CPUSLOTNO, " +
                            "STATIONNO, " +
                            "COMMINTERVAL, " +
                            "TIMEOUT, " +
                            "OPCSERVER, " +
                            "OPCLOCALREMOTE, " +
                            "OPCCLSID, " +
                            "COMMSTATUS " +
                     "FROM STATIONINFO " +
                     "WHERE EQUIPMENTID = '" + sEquipmentID + "' " +
                     "ORDER BY STATIONID";

            dtData = GetDataTable(strSQL);

            return dtData;
        }


        public DataTable GetStationInfo()
        {
            string strSQL;
            DataTable dtData;

            strSQL = "SELECT DriverName, StationName FROM StationList GROUP BY DriverName, StationName";

            dtData = GetDataTable(strSQL);

            return dtData;
        }

        public DataTable GetEquipmentTable()
        {
            string strSQL;
            DataTable dtData;

            strSQL = "SELECT A.EQUIPMENTID, " +
                    "A.EQUIPMENTNAME, " +
                    "Upper(A.EQUIPMENTTYPE) As EQUIPMENTTYPE, " +
                    "A.EQUIPMENTDETAILTYPE, " +
                    "A.SUPEREQUIPMENTID, " +
                    "Upper(A.USEFLAG) As USEFLAG," +
                    "Upper(A.AUTOFLAG) As AUTOFLAG," +
                    "Upper(A.IFMODE) As IFMODE," +
                    "A.IPADDRESS, " +
                    "A.PORT ," +
                    "B.STATIONID, " +
                    "B.PROTOCOLNAME, " +
                    "B.LOCALIP, " +
                    "B.LOCALPORT, " +
                    "B.REMOTEIP, " +
                    "B.REMOTEPORT, " +
                    "B.RACKNO, " +
                    "B.CPUSLOTNO, " +
                    "B.STATIONNO, " +
                    "B.COMMINTERVAL, " +
                    "B.TIMEOUT, " +
                    "B.OPCSERVER, " +
                    "B.OPCLOCALREMOTE, " +
                    "B.OPCCLSID, " +
                    "B.COMMSTATUS " +
                    "FROM EQUIPMENTDEFINITION A " +
                    "LEFT OUTER JOIN STATIONINFO B ON A.EQUIPMENTID = B.EQUIPMENTID " +
                    "WHERE(Upper(A.EQUIPMENTTYPE) = 'CRANE' OR Upper(A.EQUIPMENTTYPE) = 'STATION') AND B.PROTOCOLNAME = 'LSIS-XGIENET' " +
                    "AND B.STATIONID IS NOT NULL " +
                    "AND B.REMOTEIP IS NOT NULL " +
                    "ORDER BY EQUIPMENTID";

            dtData = GetDataTable(strSQL);

            return dtData;
        }

        public DataTable GetCommBlock(string sEquipmentID, string sStationID)
        {
            string strSQL;
            DataTable dtData;

            strSQL = "SELECT EQUIPMENTID, " +
                            "STATIONID, " +
                            "BLOCKNO, " +
                            "BLOCKTYPE, " +
                            "STARTADDRESS, " +
                            "READDATANUMBER, " +
                            "COMMINTERVAL " +
                     "FROM COMMBLOCKINFO " +
                     "WHERE EQUIPMENTID = '" + sEquipmentID + "' AND STATIONID = '" + sStationID + "' " +
                     "ORDER BY BLOCKNO";

            dtData = GetDataTable(strSQL);

            return dtData;
        }

        public DataTable GetTagList(string sEquipmentID, string sStationID)
        {
            string strSQL = "";
            DataTable dtData;

            strSQL += "SELECT A.EQUIPMENTID                       ";
            strSQL += "     , A.STATIONID                         ";
            strSQL += "     , A.TAGID                             ";
            strSQL += "     , A.TAGNAME                           ";
            strSQL += "     , A.DESCRIPTION                       ";
            strSQL += "     , Upper(A.USEFLAG) As USEFLAG         ";
            strSQL += "     , Upper(A.TAGKIND) As TAGKIND         ";
            strSQL += "     , Upper(A.TAGTYPE) As TAGTYPE         ";
            strSQL += "     , A.DRIVERID                          ";
            strSQL += "     , Upper(B.DRIVERNAME) As DRIVERNAME   ";
            strSQL += "     , A.ADDRESS                           ";
            strSQL += "     , Upper(A.DATATYPE) As DATATYPE       ";
            strSQL += "     , Upper(A.EVENTFLAG) As EVENTFLAG     ";
            strSQL += "     , Upper(A.PARAMETERFLAG) As PARAMETERFLAG     ";
            strSQL += "     , A.STRINGLEN                         ";
            strSQL += "     , Upper(A.WRITEUSE) As WRITEUSE       ";
            strSQL += "     , Upper(A.SCALEUSE) As SCALEUSE       ";
            strSQL += "     , A.SCALEVALUE  ";
            strSQL += "     , A.OFFSET      ";
            strSQL += "     , A.PLCMIN      ";
            strSQL += "     , A.PLCMAX      ";
            strSQL += "     , A.TAGMIN      ";
            strSQL += "     , A.TAGMAX      ";
            strSQL += "     , A.TAGVALUE    ";
            strSQL += "     , A.SETVALUE    ";
            strSQL += "     , A.MINVALUE    ";
            strSQL += "     , A.MAXVALUE    ";
            strSQL += "     , A.INTERVAL    ";
            strSQL += "     , A.UNIT        ";
            strSQL += "     , Upper(A.ALARMFLAG) As ALARMFLAG     ";
            strSQL += "     , Upper(A.MONITORINGFLAG) As MONITORINGFLAG     ";
            strSQL += "     , Upper(A.COSFLAG) As COSFLAG    ";
            strSQL += "     , A.COSPARAMETERNAME             ";
            strSQL += "     , A.LASTUPDATETIME               ";
            strSQL += "FROM TAGDEFINITION A                  ";
            strSQL += "LEFT OUTER JOIN DRIVERINFO B ON A.DRIVERID = B.DRIVERID ";
            strSQL += "WHERE A.EQUIPMENTID = '" + sEquipmentID + "'  ";
            strSQL += "  AND A.STATIONID = '" + sStationID + "' ";
            strSQL += "  AND A.ADDRESS IS NOT NULL ";
            strSQL += "  AND A.DATATYPE IS NOT NULL ";
            strSQL += " ORDER BY A.STATIONID, A.TAGID ";

            dtData = GetDataTable(strSQL);

            return dtData;
        }


        public DataTable GetTagList()
        {
            string strSQL = "";
            DataTable dtData;

            strSQL += "SELECT A.EQUIPMENTID                       ";
            strSQL += "     , A.STATIONID                         ";
            strSQL += "     , A.TAGID                             ";
            strSQL += "     , A.TAGNAME                           ";
            strSQL += "     , A.DESCRIPTION                       ";
            strSQL += "     , Upper(A.USEFLAG) As USEFLAG         ";
            strSQL += "     , Upper(A.TAGKIND) As TAGKIND         ";
            strSQL += "     , Upper(A.TAGTYPE) As TAGTYPE         ";
            strSQL += "     , A.DRIVERID                          ";
            strSQL += "     , Upper(B.DRIVERNAME) As DRIVERNAME   ";
            strSQL += "     , A.ADDRESS                           ";
            strSQL += "     , Upper(A.DATATYPE) As DATATYPE       ";
            strSQL += "     , Upper(A.EVENTFLAG) As EVENTFLAG     ";
            strSQL += "     , Upper(A.PARAMETERFLAG) As PARAMETERFLAG     ";
            strSQL += "     , A.STRINGLEN                         ";
            strSQL += "     , Upper(A.WRITEUSE) As WRITEUSE       ";
            strSQL += "     , Upper(A.SCALEUSE) As SCALEUSE       ";
            strSQL += "     , A.SCALEVALUE  ";
            strSQL += "     , A.OFFSET      ";
            strSQL += "     , A.PLCMIN      ";
            strSQL += "     , A.PLCMAX      ";
            strSQL += "     , A.TAGMIN      ";
            strSQL += "     , A.TAGMAX      ";
            strSQL += "     , A.TAGVALUE    ";
            strSQL += "     , A.SETVALUE    ";
            strSQL += "     , A.MINVALUE    ";
            strSQL += "     , A.MAXVALUE    ";
            strSQL += "     , A.INTERVAL    ";
            strSQL += "     , A.UNIT        ";
            strSQL += "     , Upper(A.ALARMFLAG) As ALARMFLAG     ";
            strSQL += "     , Upper(A.MONITORINGFLAG) As MONITORINGFLAG     ";
            strSQL += "     , Upper(A.COSFLAG) As COSFLAG    ";
            strSQL += "     , A.COSPARAMETERNAME             ";
            strSQL += "     , A.LASTUPDATETIME               ";
            strSQL += " FROM TAGDEFINITION A                  ";
            strSQL += " ORDER BY A.STATIONID, A.TAGID ";

            dtData = GetDataTable(strSQL);

            return dtData;
        }

        public DataTable GetDataTable(string strSQL)
        {
            DataTable dt = new DataTable();

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(strSQL, g_cnnMainDB);
                da.SelectCommand.CommandType = CommandType.Text;

                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                EventLogger.EventLogger.Instance.Exception(ex);
                return null;
            }
        }


        public bool OpenMainDB()
        {
            string DBType, cnStr;

            DBType = getINIValue("DataBase", "MAINDB");
            cnStr = getINIValue("DBCONNECTION", "MAINDBConnection");

            try
            {
                if (g_cnnMainDB == null)
                    g_cnnMainDB = new SqlConnection();

                if (g_cnnMainDB.State == ConnectionState.Closed)
                {
                    g_cnnMainDB.ConnectionString = cnStr;
                    g_cnnMainDB.Open();
                }

                if (g_cmdMain == null)
                    g_cmdMain = new SqlCommand();

                g_cmdMain.CommandType = CommandType.Text;
                g_cmdMain.Connection = g_cnnMainDB;

            }
            catch (Exception ex)
            {
                EventLogger.EventLogger.Instance.Exception(ex);
                return false;
            }

            return true;
        }

        public void CloseMainDB()
        {
            if (g_cnnMainDB != null)
            {
                g_cnnMainDB.Dispose();
                g_cnnMainDB.Close();
                g_cnnMainDB = null;
            }

            if (g_cmdMain != null)
            {
                g_cmdMain.Dispose();
                g_cmdMain = null; 
            }
        }


        private string getINIValue(string Section, string Key)
        {
            //if (this.path == null || this.path.Equals(string.Empty))
            //    path = defaultPath;

            if (Section == null)
                return null;

            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniFileName);

            return temp.ToString();
        }

        public int GetScanTime()
        {
            string sScanTime = getINIValue("SETTINGS", "ScanInterval");

            return Convert.ToInt16(sScanTime);
        }

    }
}

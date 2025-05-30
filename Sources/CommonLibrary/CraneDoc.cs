using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using EventLogger;

namespace CommonLibrary
{
    public class CraneDoc : Object
    {
        private readonly object _lock = new object();
        public static CraneDoc m_CraneDoc = null;

        public DataLibrary p_DataLibrary;

        public PlcList<PlcInterface> m_pStation = new PlcList<PlcInterface>();

        public CraneDoc()
        {
            p_DataLibrary = new DataLibrary();
        }

        public static CraneDoc This()
        {
            if (m_CraneDoc == null)
                m_CraneDoc = new CraneDoc();

            return m_CraneDoc;
        }


        public bool SetCrane()
        {
            bool ret = p_DataLibrary.OpenMainDB();

            if (!ret)
            {
                EventLogger.EventLogger.Instance.Error("Cannot open the SQL-Server Databse");
                return false;
            }

            DataTable dtStation = new DataTable();
            dtStation = p_DataLibrary.GetEquipmentTable();

            int StationNum = dtStation.Rows.Count;

            try
            {
                foreach (DataRow dr in dtStation.Rows)
                {
                    string sType = dr["PROTOCOLNAME"].ToString().Trim();

                    Assembly u = Assembly.LoadFile($"{Environment.CurrentDirectory}\\{sType}.dll");

                    Module[] modules = u.GetModules();
                    Type t = null;

                    foreach (var module in modules)
                    {
                        foreach (var type in module.GetTypes())
                        {
                            if (type.Name.Equals("Station"))
                            {
                                t = type;
                                break;
                            }
                        }
                    }

                    if (t == null)
                    {
                        EventLogger.EventLogger.Instance.Error($"Cannot Find PLC Driver - {sType}");
                        return false;
                    }

                    var pStation = (PlcInterface)Activator.CreateInstance(t);

                    pStation.SetStation(dr);
                    pStation.MakeReadFrame();
                    pStation.CommStart();
                    m_pStation.Add(pStation);
                }
            }
            catch (Exception ex)
            {
                EventLogger.EventLogger.Instance.Exception(ex);
            }

            p_DataLibrary.CloseMainDB();
            return true;
        }

        public void ProcessEnd()
        {
            try
            {
                if (m_pStation.Count > 0)
                {
                    foreach (var station in m_pStation)
                    {
                        station.CommEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.EventLogger.Instance.Exception(ex);
            }
        }

        public bool WriteTagStation(string EquipmentID, DataRow dr)
        {
            lock (_lock)
            {
                try
                {
                    bool ret = false;


                    // Station search
                    var station = m_pStation.FindByEquipmentId(EquipmentID);

                    if (station != null)
                    {
                        ret = station.WriteTag(dr);
                    }

                    return ret;
                }
                catch (Exception ex)
                {
                    EventLogger.EventLogger.Instance.Exception(ex);
                    return false;
                }
            }
        }

        public string WriteTagStation(DataTable dt)
        {
            lock (_lock)
            {
                try
                {
                    if (dt.Rows.Count < 1) return "FAIL";

                    //int TagCount = dt.Rows.Count;

                    //int i = 0, k = 0;

                    string RtnString = string.Empty;

                    // Station search
                    var station = m_pStation.FindByEquipmentId(dt.Rows[0]["EQUIPMENTID"].ToString());

                    if (station != null)
                    {
                        RtnString = station.WriteTag(dt);
                    }

                    // DataTable Dispose
                    //for (i = 0; i < StationCNT; i++)
                    //    dtStTag[i].Dispose();

                    return RtnString;
                }
                catch (Exception ex)
                {
                    EventLogger.EventLogger.Instance.Exception(ex);
                    return "FAIL";
                }
            }
        }

        public DataTable MakeTagDataTable()
        {
            //DataRow dRow;
            try
            {
                DataTable dTb = new DataTable("WriteTagInfo");

                dTb.Columns.Add("EQUIPMENTID", typeof(String));
                dTb.Columns.Add("TAGID", typeof(String));
                dTb.Columns.Add("DATATYPE", typeof(String));
                dTb.Columns.Add("ADDRESS", typeof(String));
                dTb.Columns.Add("TAGVALUE", typeof(String));
                dTb.Columns.Add("DATAACTIONTYPE", typeof(String));

                return dTb;
            }
            catch (Exception ex)
            {
                EventLogger.EventLogger.Instance.Exception(ex);
            }

            return null;
        }
    }
}

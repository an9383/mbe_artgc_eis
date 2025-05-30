using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using KR.MBE.CommonLibrary.Interface;
using KR.MBE.CommonLibrary.Manager;

namespace KR.MBE.CommonLibrary.Manager
{
    public class CraneManager : Object
    {
        private readonly object _lock = new();

        public static CraneManager m_CraneDoc = null;

        public DataManager p_DataLibrary;

        public PlcList<PlcInterface> m_pStation = new();

        public CraneManager()
        {
            p_DataLibrary = new DataManager();
        }

        public static CraneManager This()
        {
            if (m_CraneDoc == null)
                m_CraneDoc = new CraneManager();

            return m_CraneDoc;
        }

        /// <summary>
        /// 크레인 등록
        /// </summary>
        /// <returns></returns>
        public bool SetCrane()
        {
            bool ret = p_DataLibrary.OpenMainDB();

            if (!ret)
            {
                LogManager.Instance.Error("Cannot open the SQL-Server Databse");
                return false;
            }

            DataTable dtStation = new DataTable();
            dtStation = p_DataLibrary.GetEquipmentTable();

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
                        LogManager.Instance.Error($"Cannot Find PLC Driver - {sType}");
                        return false;
                    }

                    var pStation = (PlcInterface)Activator.CreateInstance(t);

                    pStation.SetStation(dr);
                    pStation.CommStart();
                    m_pStation.Add(pStation);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Exception(ex);
            }

            p_DataLibrary.CloseMainDB();
            return true;
        }

        /// <summary>
        /// 크레인 검색
        /// </summary>
        /// <param name="craneName">크레인 고유ID</param>
        /// <returns></returns>
        public PlcInterface GetCrane(string craneName)
        {
            return m_pStation.FindByEquipmentId(craneName);
        }

        /// <summary>
        /// Shutdown Process (크레인 PLC 드라이버 종료처리)
        /// </summary>
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
                LogManager.Instance.Exception(ex);
            }
        }
    }
}

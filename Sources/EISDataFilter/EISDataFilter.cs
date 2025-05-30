using System.Collections;
using System.Data;
using EISDataFilter.LotTracking;
using EISDataFilter.Utils;
using KR.MBE.CommonLibrary.Manager;

namespace EISDataFilter
{
    public class CEISDataFilter
    {
        public CEISDataFilter()
        {
            SQLPoolManager.This().CreatePoolList();
        }

        //public void SendMonitoringInfo(ref DataTable dt)
        //{
        //    BUSHINGTAGDATA BD = new BUSHINGTAGDATA();

        //    BD.Dowork(dt);

        //    //EISDataFilter.LotTracking.BUSHINGTAGDATA.Dowork(dt);
        //}

        public bool MakeMessageSet(ref DataSet dataSet)
        {
            bool ret;
            DataSet ds = new DataSet();
            ds = dataSet;

            if (ds.Tables[0].Rows.Count < 1)
            {
                ds.Dispose();
                return false;
            }

            if (SQLPoolManager.This().PoolCount != 0)
            {
                PARAMETERRESULT PR = new PARAMETERRESULT();

                ret = PR.Dowork(ds);
                ret = PR.DoworkParam(ds);

                if (ret)
                {
                    ds.Dispose();
                }
                else
                {
                    //작업 완료 후 Connection종료..
                    //dbHandler.CloseDB();
                    ds.Dispose();
                    //return false;
                }
                return true;
            }
            else
                return false;
            //DispatchMessage(MessageName, ds);
        }

        public void MakeAlarmMessageSet(string OpcSvrName, string AlarmID, string AlarmType)
        {
            PROCESSEVENT PE = new PROCESSEVENT();
            bool ret = PE.DoworkAlarm(OpcSvrName, AlarmID, AlarmType);
        }

        public bool SetEventTag(Hashtable ht)
        {
            PROCESSEVENT PE = new PROCESSEVENT();
            bool ret = PE.Dowork(ht);

            return ret;
        }

        public bool SetEventTag(DataTable dt)
        {
            PROCESSEVENT PE = new PROCESSEVENT();
            bool ret = PE.Dowork(dt);

            return ret;
        }
    }
}

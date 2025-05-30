using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using EISDataFilter.LotTracking;
using KR.MBE.CommonLibrary.Manager;

namespace MBE.Driver.LSElectric
{
    public partial class Station
    {

        public bool SendTagInfo()
        {
            int k , i;
            long interval;
            DateTime dt = DateTime.Now;
            bool ret;

            long CurTick = dt.Ticks;
            DateTime Cur = new DateTime(CurTick);

            // TagMasterManagement에 Tag Value Update
            CTagManager.UpdateCurrentTagValue(m_htTagDef);

            DataRow dRow;

            DataTable dtEventTag = new DataTable("EventTag");
            DataTable dtParameterTag = new DataTable("ParameterTag");
            DataTable dtAlarmTag = new DataTable("AlarmTag");
            DataTable dtMonitoringTag = new DataTable("MonitoringTag");
            //DataTable dtCOSTag = new DataTable("COSTag");

            PROCESSEVENT dfEvent = new PROCESSEVENT();
            PARAMETERRESULT dfParameter = new PARAMETERRESULT();

            dtEventTag.Columns.Add("EQUIPMENTID", typeof(String));
            dtEventTag.Columns.Add("TAGID", typeof(String));
            dtEventTag.Columns.Add("EVENTFLAG", typeof(String));
            dtEventTag.Columns.Add("TAGVALUE", typeof(String));

            dtParameterTag.Columns.Add("EQUIPMENTID", typeof(String));
            dtParameterTag.Columns.Add("TAGID", typeof(String));
            dtParameterTag.Columns.Add("EVENTFLAG", typeof(String));
            dtParameterTag.Columns.Add("TAGVALUE", typeof(String));

            dtAlarmTag.Columns.Add("EQUIPMENTID", typeof(String));
            dtAlarmTag.Columns.Add("TAGID", typeof(String));
            dtAlarmTag.Columns.Add("EVENTFLAG", typeof(String));
            dtAlarmTag.Columns.Add("TAGVALUE", typeof(String));

            dtMonitoringTag.Columns.Add("EQUIPMENTID", typeof(String));
            dtMonitoringTag.Columns.Add("TAGID", typeof(String));
            dtMonitoringTag.Columns.Add("EVENTFLAG", typeof(String));
            dtMonitoringTag.Columns.Add("TAGVALUE", typeof(String));

            //dtCOSTag.Columns.Add("EQUIPMENTID", typeof(String));
            //dtCOSTag.Columns.Add("TAGID", typeof(String));
            //dtCOSTag.Columns.Add("EVENTFLAG", typeof(String));
            //dtCOSTag.Columns.Add("TAGVALUE", typeof(String));



            //try
            //{
            // 초기 시작할 때 한번 전체를 보냄
                if (InitStart)
                {
                    for (i = 0; i < m_TagList.Count; i++)
                    {
                        // EQUIPMENT ID is not empty 
                        if (!m_TagList[i].EQUIPMENTID.Equals(""))
                        {

                            dRow = dtEventTag.NewRow();

                            dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                            dRow["TAGID"] = m_TagList[i].TAGID;
                            dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                            dRow["TAGVALUE"] = m_TagList[i].sTagVal;

                            dtEventTag.Rows.Add(dRow);
                        

                            m_TagList[i]._LastTick = CurTick;

                            if(m_TagList[i].DATATYPE.Equals("STRING"))
                                m_TagList[i].BeforeStringVal = m_TagList[i].sTagVal;
                            else
                                m_TagList[i]._BeforeTagVal = m_TagList[i]._TagVal;
                        }
                    }

                    ret = dfEvent.Dowork(dtEventTag);
                }
                else
                {
                    for (i = 0; i < m_TagList.Count; i++)
                    {
                        // EQUIPMENT ID is not empty 
                        if (!m_TagList[i].EQUIPMENTID.Equals(""))
                        {
                            // Parameter Tag 일 경우 Interval Check
                            if (m_TagList[i].EVENTFLAG.ToUpper() == "YES")
                            {
                                if (m_TagList[i].DATATYPE == "STRING")
                                {
                                    if (m_TagList[i].BeforeStringVal != m_TagList[i].sTagVal)
                                    {

                                        dRow = dtEventTag.NewRow();

                                        dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                                        dRow["TAGID"] = m_TagList[i].TAGID;
                                        dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                                        dRow["TAGVALUE"] = m_TagList[i].sTagVal;

                                        dtEventTag.Rows.Add(dRow);
                                    }

                                }
                                else
                                {
                                    if (m_TagList[i]._BeforeTagVal != m_TagList[i]._TagVal)
                                    {

                                        dRow = dtEventTag.NewRow();

                                        dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                                        dRow["TAGID"] = m_TagList[i].TAGID;
                                        dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                                        dRow["TAGVALUE"] = m_TagList[i].sTagVal;

                                        dtEventTag.Rows.Add(dRow);
                                    }
                                }
                            }


                            if (m_TagList[i].PARAMETERFLAG.ToUpper() == "YES")
                            {
                                if (m_TagList[i].DATATYPE == "STRING")
                                {
                                    if (m_TagList[i].BeforeStringVal != m_TagList[i].sTagVal)
                                    {
                                        dRow = dtParameterTag.NewRow();

                                        dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                                        dRow["TAGID"] = m_TagList[i].TAGID;
                                        dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                                        dRow["TAGVALUE"] = m_TagList[i].sTagVal;

                                        dtParameterTag.Rows.Add(dRow);
                                    }
                                    else
                                    {
                                        {
                                            if (m_TagList[i]._BeforeTagVal != m_TagList[i]._TagVal)
                                            {
                                                dRow = dtParameterTag.NewRow();
                                            
                                                dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                                                dRow["TAGID"] = m_TagList[i].TAGID;
                                                dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                                                dRow["TAGVALUE"] = m_TagList[i].sTagVal;

                                                dtParameterTag.Rows.Add(dRow);
                                            }
                                        }
                                    }
                                }
                            }

                            if (m_TagList[i].ALARMFLAG.ToUpper() == "YES")
                            {   // Parameter Tag가 아닐 경우 값이 변경된 Tag 만 전달
                                if (m_TagList[i]._BeforeTagVal != m_TagList[i]._TagVal)
                                {
                                    dRow = dtAlarmTag.NewRow();
                                
                                    dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                                    dRow["TAGID"] = m_TagList[i].TAGID;
                                    dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                                    dRow["TAGVALUE"] = m_TagList[i].sTagVal;

                                    dtAlarmTag.Rows.Add(dRow);
                                }
                            }

                            if (m_TagList[i].MONITORINGFLAG.ToUpper() == "YES")
                            {   // Parameter Tag가 아닐 경우 값이 변경된 Tag 만 전달
                                if (m_TagList[i]._BeforeTagVal != m_TagList[i]._TagVal)
                                {
                                    dRow = dtMonitoringTag.NewRow();

                                    dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                                    dRow["TAGID"] = m_TagList[i].TAGID;
                                    dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                                    dRow["TAGVALUE"] = m_TagList[i].sTagVal;

                                    dtMonitoringTag.Rows.Add(dRow);
                                }
                            }

                            //if (m_TagList[i].COSFLAG.ToUpper() == "YES")
                            //{   // Parameter Tag가 아닐 경우 값이 변경된 Tag 만 전달
                            //    if (m_TagList[i]._BeforeTagVal != m_TagList[i]._TagVal)
                            //    {
                            //        dRow = dtCOSTag.NewRow();

                            //        //dRow["SITEID"] = "ITIER";
                            //        dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                            //        dRow["TAGID"] = m_TagList[i].TAGID;
                            //        dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                            //        dRow["TAGVALUE"] = m_TagList[i].sTagVal;

                            //        dtCOSTag.Rows.Add(dRow);
                            //    }
                            //}

                        }
                    m_TagList[i]._LastTick = CurTick;

                    if (m_TagList[i].DATATYPE.Equals("STRING"))
                        m_TagList[i].BeforeStringVal = m_TagList[i].sTagVal;
                    else
                        m_TagList[i]._BeforeTagVal = m_TagList[i]._TagVal;

                    //m_TagList[i]._BeforeRawVal = m_TagList[i]._RawVal;
                    //m_TagList[i]._BeforeTagVal = m_TagList[i]._TagVal;
                }


                    if(dtEventTag.Rows.Count>0)
                        ret = dfEvent.Dowork(dtEventTag);
                    if (dtParameterTag.Rows.Count > 0)
                        ret = dfParameter.Dowork(dtParameterTag);
                    
                    //ret = dfEvent.DoworkAlarm(dtAlarmTag);
                    //ret = dfParameter.Dowork(dtParameterTag);

                }

                InitStart = false;


                dtEventTag.Dispose();
                dtParameterTag.Dispose();
                dtAlarmTag.Dispose();
                dtMonitoringTag.Dispose();
                //dtCOSTag.Dispose();

            //}
            //catch (Exception Err)
            //{
            //    errLogger.SaveErrLog(Err.ToString());
            //    return false;
            //}
            return true;
        }
        /*
        public DataTable MakeEventData()
        {
            DataRow dRow;
            DateTime dt = DateTime.Now;

            long CurTick = dt.Ticks;

            DataTable dTb = new DataTable("TagInfo");

            AddFilterTagInfoColumn(out dTb);


            for (int i = 0; i < m_TagList.Count; i++)
            {
                // EQUIPMENT ID is not empty 
                if (!m_TagList[i].EQUIPMENTID.Equals(""))
                {
                    // Parameter Tag 일 경우 Interval Check
                    if (m_TagList[i].EVENTFLAG.ToUpper() == "YES" && m_TagList[i]._BeforeTagVal != m_TagList[i]._TagVal)
                    {
                        if ((CurTick - m_TagList[i]._LastTick) > m_TagList[i]._Interval)
                        {

                            dRow = dTb.NewRow();

                            dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                            dRow["TAGID"] = m_TagList[i].TAGID;
                            dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                            dRow["TAGVALUE"] = ((double)((int)(m_TagList[i].TAGVALUE * 1000))) / 1000;

                            dTb.Rows.Add(dRow);
                        }

                    }

                    else if (m_TagList[i].EVENTFLAG.ToUpper() == "YES")
                    {   // Parameter Tag가 아닐 경우 값이 변경된 Tag 만 전달
                        if (m_TagList[i]._BeforeTagVal != m_TagList[i]._TagVal)
                        {
                            dRow = dTb.NewRow();

                            dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                            dRow["TAGID"] = m_TagList[i].TAGID;
                            dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                            dRow["TAGVALUE"] = ((double)((int)(m_TagList[i].TAGVALUE * 1000))) / 1000;

                            dTb.Rows.Add(dRow);
                        }
                    }

                    else if (m_TagList[i].EVENTFLAG.ToUpper() == "YES")
                    {   // Parameter Tag가 아닐 경우 값이 변경된 Tag 만 전달
                        if (m_TagList[i]._BeforeTagVal != m_TagList[i]._TagVal || (CurTick - m_TagList[i]._LastTick) > m_TagList[i]._Interval)
                        {
                            dRow = dTb.NewRow();

                            dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                            dRow["TAGID"] = m_TagList[i].TAGID;
                            dRow["EVENTFLAG"] = m_TagList[i].EVENTFLAG;
                            dRow["TAGVALUE"] = ((double)((int)(m_TagList[i].TAGVALUE * 1000))) / 1000;

                            dTb.Rows.Add(dRow);
                        }
                    }
                }
            }
            return dTb;
        }
        */
        public DataTable MakeMonitorinTagDataTable()
        {
            DataRow dRow;
            DateTime dt = DateTime.Now;

            long CurTick = dt.Ticks;

            DataTable dTb = new DataTable("TagInfo");

            dTb.Columns.Add("EQUIPMENTID", typeof(String));
            dTb.Columns.Add("TAGID", typeof(String));
            dTb.Columns.Add("TAGVALUE", typeof(String));

            for (int i = 0; i < m_TagList.Count; i++)
            {
                // EQUIPMENT ID is not empty 
                if (!m_TagList[i].EQUIPMENTID.Equals(""))    // && m_TagList[i].MonitoringFlag.Equals("YES"))
                {
                    if ((CurTick - m_TagList[i].MonitoringLastTick) > m_TagList[i].MonitoringInterval)
                    {

                        dRow = dTb.NewRow();

                        dRow["EQUIPMENTID"] = m_TagList[i].EQUIPMENTID;
                        dRow["TAGID"] = m_TagList[i].TAGID;
                        dRow["TAGVALUE"] = m_TagList[i].sTagVal;

                        dTb.Rows.Add(dRow);
                    }
                }
            }
            return dTb;
        }

        public void MakeHashTagList()
        {
            //if (m_htTagDef != null) return;

            for (int i = 0; i < m_TagList.Count; i++)
            {
                TagMasterData tdTagInfo = new TagMasterData();

                tdTagInfo.sTagID = m_TagList[i].TAGID;

                //if (m_TagList[i].DATATYPE == "STRING")
                    tdTagInfo.sCurrentValue = m_TagList[i].sTagVal;
                //else
                //    tdTagInfo.sCurrentValue = m_TagList[i]._TagVal.ToString();

                tdTagInfo.itmHandleServer = 0;
                tdTagInfo.TagIdx = m_TagList[i].d_TagIdx;

                m_htTagDef.Add(tdTagInfo.sTagID, tdTagInfo);
            }

        }

        public void AddFilterTagInfoColumn(out DataTable dTable)
        {
            dTable = new DataTable();

            dTable.Columns.Add("EQUIPMENTID", typeof(String));
            dTable.Columns.Add("TAGID", typeof(String));
            dTable.Columns.Add("EVENTFLAG", typeof(String));
            dTable.Columns.Add("TAGVALUE", typeof(double));
        }

    }
}

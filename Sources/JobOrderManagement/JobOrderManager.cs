using System.Data;
using System.Linq;
using Middleware.ActiveMQ;
using System.Collections;
using KR.MBE.CommonLibrary.Manager;
using KR.MBE.CommonLibrary.Utils;

namespace JobOrderManagement
{
    public class JobOrderManager
    {
        private DataTable m_dtStepJobInfo;
        private Hashtable m_htJobOrderList = null;
        private Hashtable m_htWriteTagList = null;
        public Hashtable m_htReadList = null;
        public string m_sSiteID = "";
        public string m_sJobOrderID = "";
        public string m_sEquipmentID = "";
        public string m_sStepJobID = "";
        public string m_sStepJobType = "";
        public string m_sStepSequence = "";
        public string m_sCompositionID = "";
        public string m_sStatus = "";

        public static CraneManager m_pXGTEnetDoc = null;

        public JobOrderManager()
        {
            m_dtStepJobInfo = new DataTable();
            m_htJobOrderList = new Hashtable();
            m_htWriteTagList = new Hashtable();
            m_htReadList = new Hashtable();

            m_pXGTEnetDoc = CraneDoc.This();

            initializeDataTable();
        }

        private void initializeDataTable()
        {
            m_dtStepJobInfo.TableName = "StepJobList";

            m_dtStepJobInfo.Columns.Add("SEQUENCE", typeof(int));
            m_dtStepJobInfo.Columns.Add("ACTIONTYPE", typeof(string));
            m_dtStepJobInfo.Columns.Add("PARAMETERID", typeof(string));
            m_dtStepJobInfo.Columns.Add("PARAMETERLEVEL", typeof(string));
            m_dtStepJobInfo.Columns.Add("TAGID", typeof(string));
            m_dtStepJobInfo.Columns.Add("DATATYPE", typeof(string));
            m_dtStepJobInfo.Columns.Add("ADDRESS", typeof(string));              //  추가
            m_dtStepJobInfo.Columns.Add("TAGVALUE", typeof(string));
            m_dtStepJobInfo.Columns.Add("DATAACTIONTYPE", typeof(string));
            m_dtStepJobInfo.Columns.Add("DATAPROCESSTYPE", typeof(string));
            m_dtStepJobInfo.Columns.Add("JOBSTATUS", typeof(string));
        }

        public void setStepJobInformation(string sMessage)
        {
            m_sSiteID = ConvertUtil.GetXMLRecord(sMessage, "SITEID");
            m_sJobOrderID = ConvertUtil.GetXMLRecord(sMessage, "JOBORDERID");
            m_sEquipmentID = ConvertUtil.GetXMLRecord(sMessage, "EQUIPMENTID");
            m_sStepJobID = ConvertUtil.GetXMLRecord(sMessage, "STEPJOBID");
            m_sStepJobType = ConvertUtil.GetXMLRecord(sMessage, "STEPJOBTYPE");
            m_sStepSequence = ConvertUtil.GetXMLRecord(sMessage, "STEPSEQUENCE");
            m_sCompositionID = ConvertUtil.GetXMLRecord(sMessage, "COMPOSITIONID");
            
            if ("Execute".Equals(m_sStepJobType))
            { 
                m_sStatus = "Start";
            }
            else if ("StartEnd".Equals(m_sStepJobType))
            {
                m_sStatus = "Create";
            }

            Hashtable htDataInfoList = ConvertUtil.GetXMLRecords(sMessage, "DATAINFO");

            string sDataInfo = "";

            m_dtStepJobInfo.Rows.Clear();

            for (int i = 0; i < htDataInfoList.Count; i++)
            {
                sDataInfo = (string)htDataInfoList[i];

                makeStepJobInfo(i, sDataInfo);
            }
        }

        public bool stepJobProcess()
        {
            bool isComplete = false;

            if ("Create".Equals(m_sStatus))
            {
                if (m_dtStepJobInfo != null && m_dtStepJobInfo.Rows.Count > 0)
                {
                    //m_dtStepJobInfo.DefaultView.Sort = "ACTIONTYPE DESC, PARAMETERLEVEL";
                    //m_dtStepJobInfo.AcceptChanges();

                    //DataTable aa = new DataTable();
                    //aa = m_dtStepJobInfo.Select("", "ACTIONTYPE DESC, PARAMETERLEVEL, DATAACTIONTYPE DESC").CopyToDataTable();

                    if (m_dtStepJobInfo.Select("ACTIONTYPE = 'Start'").Length > 0)
                    {

                        int iMaxParameterLevel = m_dtStepJobInfo.AsEnumerable().Where(row => (row["ACTIONTYPE"].ToString() == "Start"))
                                                                               .Max(x => int.Parse(x.Field<string>("PARAMETERLEVEL")));

                        bool bLevelComplete = true;

                        for (int iLevel = 1; iLevel <= iMaxParameterLevel; iLevel++)
                        {
                            if (bLevelComplete)
                            {
                                int iCompleteCount = 0;
                                int iLevelCount = 0;

                                DataTable dtLevel = new DataTable();
                                dtLevel = m_dtStepJobInfo.Select("ACTIONTYPE in ('Start') AND PARAMETERLEVEL = '" + iLevel.ToString() + "'", "ACTIONTYPE DESC, PARAMETERLEVEL, DATAACTIONTYPE DESC").CopyToDataTable();

                                iLevelCount = dtLevel.Rows.Count;

                                for (int j = 0; j < m_dtStepJobInfo.Rows.Count; j++)
                                {
                                    int iParameterLevel = int.Parse(m_dtStepJobInfo.Rows[j]["PARAMETERLEVEL"].ToString());
                                    string sJobStatus = m_dtStepJobInfo.Rows[j]["JOBSTATUS"].ToString();
                                    string sActionType = m_dtStepJobInfo.Rows[j]["ACTIONTYPE"].ToString();
                                    string sDataActionType = m_dtStepJobInfo.Rows[j]["DATAACTIONTYPE"].ToString();
                                    string sParameterID = m_dtStepJobInfo.Rows[j]["PARAMETERID"].ToString();
                                    string sParameterTagID = m_dtStepJobInfo.Rows[j]["TAGID"].ToString();
                                    string sParameterValue = m_dtStepJobInfo.Rows[j]["TAGVALUE"].ToString();

                                    if (iLevel.Equals(iParameterLevel))
                                    {
                                        if ("Start".Equals(sActionType))
                                        {
                                            if ("Complete".Equals(sJobStatus))
                                            {
                                                iCompleteCount++;
                                            }
                                            else
                                            {
                                                if ("Set".Equals(sDataActionType))
                                                {
                                                    if ("SHOWCCTVLIST".Equals(sParameterID))
                                                    {
                                                        string sDevice = m_dtStepJobInfo.Rows[j]["TAGVALUE"].ToString();
                                                        string[] sDeviceList = sDevice.Split(',');

                                                        if (sDeviceList.Length > 0)
                                                        {
                                                            foreach (string sDeviceAddress in sDeviceList)
                                                            {
                                                                if (!"".Equals(sDeviceAddress))
                                                                {
                                                                    HttpSender.This().HttpSendData(sDeviceAddress, "opened");
                                                                }
                                                            }
                                                        }

                                                        m_dtStepJobInfo.Rows[j]["JOBSTATUS"] = "Complete";
                                                        iCompleteCount++;
                                                    }
                                                    else
                                                    {
                                                        // Tag Write
                                                        bool isWrite = false;

                                                        ////////////////////////////////////////
                                                        isWrite = m_pXGTEnetDoc.WriteTagStation(m_sEquipmentID, m_dtStepJobInfo.Rows[j]);
                                                        //////////////////////////////////////
                                                        ///

                                                        //Hashtable htTagMaster = TagMasterManager.This().GetTagMasterHashTable();

                                                        //if (htTagMaster != null)
                                                        //{
                                                        //    TagMasterData tagMaster = (TagMasterData)htTagMaster[sParameterTagID];

                                                        //    if (tagMaster != null)
                                                        //    {
                                                        //        tagMaster.sCurrentValue = sParameterValue;
                                                        //        isWrite = true;
                                                        //    }
                                                        //}

                                                        if (isWrite)
                                                        {
                                                            m_dtStepJobInfo.Rows[j]["JOBSTATUS"] = "Complete";
                                                            iCompleteCount++;
                                                        }
                                                    }
                                                }
                                                else if ("Get".Equals(sDataActionType))
                                                {
                                                    // Tag 비교
                                                    Hashtable htTagMaster = TagMasterManager.This().GetTagMasterHashTable();

                                                    if (htTagMaster != null)
                                                    {
                                                        TagMasterData tagMaster = (TagMasterData)htTagMaster[sParameterTagID];

                                                        if (tagMaster != null)
                                                        {
                                                            if (sParameterValue.Equals(tagMaster.sCurrentValue))
                                                            {
                                                                m_dtStepJobInfo.Rows[j]["JOBSTATUS"] = "Complete";
                                                                iCompleteCount++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!iLevelCount.Equals(iCompleteCount))
                                {
                                    bLevelComplete = false;
                                }
                            }

                            if (iMaxParameterLevel.Equals(iLevel))
                            {
                                isComplete = bLevelComplete;
                            }
                        }


                        //for (int i = 0; i < m_dtStepJobInfo.Rows.Count; i++)
                        //{
                        //    string sActionType = m_dtStepJobInfo.Rows[i]["ACTIONTYPE"].ToString();
                        //    string sDataActionType = m_dtStepJobInfo.Rows[i]["DATAACTIONTYPE"].ToString();

                        //    if ("Set".Equals(sDataActionType))
                        //    {
                        //        // Tag Write
                        //        bool isWrite = true;

                        //        if (isWrite)
                        //        {
                        //            m_dtStepJobInfo.Rows[i]["JOBSTATUS"] = "Complete";
                        //        }
                        //    }
                        //}

                        //// Read Tag
                        //DataRow[] drReadTag = m_dtStepJobInfo.Select("DATAACTIONTYPE = 'Get'");

                        //for (int i = 0; i < drReadTag.Length; i++)
                        //{ 
                        //    m_htReadList.Add(drReadTag[i]["TAGID"].ToString(), drReadTag[i]["TAGVALUE"].ToString());
                        //}
                    }
                }

                if (isComplete)
                {
                    m_sStatus = "Start";
                }
            }
            else if ("Start".Equals(m_sStatus))
            {
                if (m_dtStepJobInfo != null && m_dtStepJobInfo.Rows.Count > 0)
                {
                    //m_dtStepJobInfo.DefaultView.Sort = "ACTIONTYPE DESC, PARAMETERLEVEL";
                    //m_dtStepJobInfo.AcceptChanges();

                    //DataTable aa = new DataTable();
                    //aa = m_dtStepJobInfo.Select("", "ACTIONTYPE DESC, PARAMETERLEVEL, DATAACTIONTYPE DESC").CopyToDataTable();

                    if (m_dtStepJobInfo.Select("ACTIONTYPE in ('End', 'Execute')").Length > 0)
                    { 
                        int iMaxParameterLevel = m_dtStepJobInfo.AsEnumerable().Where(row => (row["ACTIONTYPE"].ToString() == "End" || row["ACTIONTYPE"].ToString() == "Execute"))
                                                                               .Max(x => int.Parse(x.Field<string>("PARAMETERLEVEL")));

                        bool bLevelComplete = true;

                        for (int iLevel = 1; iLevel <= iMaxParameterLevel; iLevel++)
                        {
                            if (bLevelComplete)
                            {
                                int iCompleteCount = 0;
                                int iLevelCount = 0;

                                DataTable dtLevel = new DataTable();
                                dtLevel = m_dtStepJobInfo.Select("ACTIONTYPE in ('End', 'Execute') AND PARAMETERLEVEL = '" + iLevel.ToString() + "'", "ACTIONTYPE DESC, PARAMETERLEVEL, DATAACTIONTYPE DESC").CopyToDataTable();

                                iLevelCount = dtLevel.Rows.Count;

                                for (int j = 0; j < m_dtStepJobInfo.Rows.Count; j++)
                                {
                                    int iParameterLevel = int.Parse(m_dtStepJobInfo.Rows[j]["PARAMETERLEVEL"].ToString());
                                    string sJobStatus = m_dtStepJobInfo.Rows[j]["JOBSTATUS"].ToString();
                                    string sActionType = m_dtStepJobInfo.Rows[j]["ACTIONTYPE"].ToString();
                                    string sDataActionType = m_dtStepJobInfo.Rows[j]["DATAACTIONTYPE"].ToString();
                                    string sParameterID = m_dtStepJobInfo.Rows[j]["PARAMETERID"].ToString();
                                    string sParameterTagID = m_dtStepJobInfo.Rows[j]["TAGID"].ToString();
                                    string sParameterValue = m_dtStepJobInfo.Rows[j]["TAGVALUE"].ToString();

                                    if (iLevel.Equals(iParameterLevel))
                                    {
                                        if ("End".Equals(sActionType) || "Execute".Equals(sActionType))
                                        {
                                            if ("Complete".Equals(sJobStatus))
                                            {
                                                iCompleteCount++;
                                            }
                                            else
                                            {
                                                if ("Set".Equals(sDataActionType))
                                                {
                                                    if ("SHOWCCTVLIST".Equals(sParameterID))
                                                    {
                                                        string sDevice = m_dtStepJobInfo.Rows[j]["TAGVALUE"].ToString();
                                                        string[] sDeviceList = sDevice.Split(',');

                                                        if (sDeviceList.Length > 0)
                                                        {
                                                            foreach (string sDeviceAddress in sDeviceList)
                                                            {
                                                                if (!"".Equals(sDeviceAddress))
                                                                {
                                                                    HttpSender.This().HttpSendData(sDeviceAddress, "opened");
                                                                }
                                                            }
                                                        }

                                                        m_dtStepJobInfo.Rows[j]["JOBSTATUS"] = "Complete";
                                                        iCompleteCount++;
                                                    }
                                                    else 
                                                    { 
                                                        // Tag Write
                                                        bool isWrite = false;

                                                        isWrite = m_pXGTEnetDoc.WriteTagStation(m_sEquipmentID, m_dtStepJobInfo.Rows[j]);

                                                        //Hashtable htTagMaster = TagMasterManager.This().GetTagMasterHashTable();

                                                        //if (htTagMaster != null)
                                                        //{
                                                        //    TagMasterData tagMaster = (TagMasterData)htTagMaster[sParameterTagID];

                                                        //    if (tagMaster != null)
                                                        //    {
                                                        //        tagMaster.sCurrentValue = sParameterValue;
                                                        //        isWrite = true;
                                                        //    }
                                                        //}

                                                        if (isWrite)
                                                        {
                                                            m_dtStepJobInfo.Rows[j]["JOBSTATUS"] = "Complete";
                                                            iCompleteCount++;
                                                        }
                                                    }
                                                }
                                                else if ("Get".Equals(sDataActionType))
                                                {
                                                    // Tag 비교
                                                    

                                                    Hashtable htTagMaster = TagMasterManager.This().GetTagMasterHashTable();

                                                    if (htTagMaster != null)
                                                    {
                                                        TagMasterData tagMaster = (TagMasterData)htTagMaster[sParameterTagID];

                                                        if (tagMaster != null)
                                                        {
                                                            if (sParameterValue.Equals(tagMaster.sCurrentValue))
                                                            {
                                                                m_dtStepJobInfo.Rows[j]["JOBSTATUS"] = "Complete";
                                                                iCompleteCount++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!iLevelCount.Equals(iCompleteCount))
                                {
                                    bLevelComplete = false;
                                }
                            }

                            if (iMaxParameterLevel.Equals(iLevel))
                            {
                                isComplete = bLevelComplete;
                            }
                        }
                    }
                }

                if (isComplete)
                {
                    m_sStatus = "End";
                }
            }

            return isComplete;
        }

        public bool stepJobCompleteProcess(string sJobStatus)
        {
            bool isComplete = false;

            if ("Create".Equals(sJobStatus))
            {
                m_sStatus = "Start";
                isComplete = true;
            }
            else if ("Start".Equals(sJobStatus))
            {
                m_sStatus = "End";
                isComplete = true;
            }

            return isComplete;
        }

        private void makeStepJobInfo(int iSequence, string sMessage)
        {
            string sTagID = ConvertUtil.GetXMLRecord(sMessage, "TAGID");
            string sTagValue = ConvertUtil.GetXMLRecord(sMessage, "TAGVALUE");
            string sDataActionType = ConvertUtil.GetXMLRecord(sMessage, "DATAACTIONTYPE");

            DataRow drStepJobInfo = m_dtStepJobInfo.NewRow();

            drStepJobInfo["SEQUENCE"] = iSequence;
            drStepJobInfo["ACTIONTYPE"] = ConvertUtil.GetXMLRecord(sMessage, "ACTIONTYPE");
            drStepJobInfo["PARAMETERID"] = ConvertUtil.GetXMLRecord(sMessage, "PARAMETERID");
            drStepJobInfo["PARAMETERLEVEL"] = ConvertUtil.GetXMLRecord(sMessage, "PARAMETERLEVEL");
            drStepJobInfo["TAGID"] = sTagID;
            drStepJobInfo["DATATYPE"] = ConvertUtil.GetXMLRecord(sMessage, "DATATYPE");
            drStepJobInfo["ADDRESS"] = ConvertUtil.GetXMLRecord(sMessage, "ADDRESS");
            drStepJobInfo["TAGVALUE"] = sTagValue;
            drStepJobInfo["DATAACTIONTYPE"] = sDataActionType;
            drStepJobInfo["DATAPROCESSTYPE"] = ConvertUtil.GetXMLRecord(sMessage, "DATAPROCESSTYPE");
            drStepJobInfo["JOBSTATUS"] = ConvertUtil.GetXMLRecord(sMessage, "JOBSTATUS");

            m_dtStepJobInfo.Rows.Add(drStepJobInfo);

            // Read Tag List ( EventTag )
            if ("Get".Equals(sDataActionType))
            {
                m_htReadList.Add(sTagID, sTagValue);
            }
        }
    }
}

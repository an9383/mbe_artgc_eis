using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using KR.MBE.CommonLibrary.Utils;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KR.MBE.CommonLibrary.Interface;
using KR.MBE.CommonLibrary.Struct;
using log4net.Core;
using Microsoft.Extensions.Logging;
using static System.Collections.Specialized.BitVector32;
using Apache.NMS.ActiveMQ.Commands;
using KR.MBE.CommonLibrary.Handler;
using log4net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Eventing.Reader;

namespace KR.MBE.CommonLibrary.Manager
{
    public enum StepJobStatus
    {
        Init = 0,
        Create = 1,
        Start = 2,
        StartReported = 3,
        End = 4,
        Complete = 5,
        CompleteReported = 6,
        Error = 7
    }

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
        public StepJobStatus m_sStatus = StepJobStatus.Init;
        public bool PlcResponseStartFailed = false;
        public Task CreateTaskProcess = null;
        public Task StartTaskProcess = null;
        public Task TaskProcess = null;
        public StepJobInfoList StepJobInfoList = null;
        

        private int m_iMaxLevel = 0;
        private int m_iNowLevel = 0;
        private bool m_bEventCheckerStart = false;
        
        public static PlcInterface station = null;

        public JobOrderManager()
        {
            m_dtStepJobInfo = new DataTable();
            m_htJobOrderList = new Hashtable();
            m_htWriteTagList = new Hashtable();
            m_htReadList = new Hashtable();
            
            initializeDataTable();
        }

        public JobOrderManager(string sMessage) : base()
        {
            setStepJobInformation(sMessage);
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
            m_dtStepJobInfo.Columns.Add("RECEIVEDID", typeof(string));
            m_dtStepJobInfo.Columns.Add("DATATARGET", typeof(string));
            m_dtStepJobInfo.Columns.Add("DATAFAILTARGET", typeof(string));
        }

        public void setStepJobInformation(string sMessage)
        {
            m_sJobOrderID = ConvertUtil.GetXMLRecord(sMessage, "JOBORDERID");
            m_sEquipmentID = ConvertUtil.GetXMLRecord(sMessage, "EQUIPMENTID");
            m_sStepJobID = ConvertUtil.GetXMLRecord(sMessage, "STEPJOBID");
            m_sStepJobType = ConvertUtil.GetXMLRecord(sMessage, "STEPJOBTYPE");
            m_sStepSequence = ConvertUtil.GetXMLRecord(sMessage, "STEPSEQUENCE");
            m_sCompositionID = ConvertUtil.GetXMLRecord(sMessage, "COMPOSITIONID"); 


            station = CraneManager.This().m_pStation.FindByEquipmentId(m_sEquipmentID);

            if ("Execute".Equals(m_sStepJobType))
            {
                m_sStatus = StepJobStatus.Start;
            }
            else if ("StartEnd".Equals(m_sStepJobType))
            {
                m_sStatus = StepJobStatus.Create;
            }

            var dataInfoList = ConvertUtil.GetXMLRecords(sMessage, "DATAINFO");

            if (dataInfoList.Count > 0)
            {
                StepJobInfoList = new StepJobInfoList(dataInfoList);
            }
        }

        public StepStatus ProcessingDataFromPLC(StepJobInfo info)
        {
            // Tag Write
            bool isWrite = false;

            var tags = station.GetTagList();
            var tag = tags.FindByTagId(info.TagId);


            if (tag.IsWriteable && info.DataActionType == DataActionType.Set)
            {
                tag._TagVal = info.TagValue;
                info.StepStatus = StepStatus.Complete;
            }
            else
            {
                if (tag.sTagVal == info.TagValue)
                {
                    info.StepStatus = StepStatus.Complete;
                }
            }

            StepParameterStatusUpdate(info);

            return info.StepStatus;
        }

        public void ProcessingShowCCTVList()
        {
            /*string sDevice = row["TAGVALUE"].ToString();
            string[] sDeviceList = sDevice.Split(',');

            if (sDeviceList.Length > 0)
            {
                foreach (string sDeviceAddress in sDeviceList)
                {
                    if (!"".Equals(sDeviceAddress))
                    {
                        HttpManager.This().HttpSendData(sDeviceAddress, "opened");
                    }
                }
            }

            row["JOBSTATUS"] = "Complete";
            iCompleteCount++;*/
        }

        public bool stepJobLevelProcess(ActionType type, int level)
        {
            var dataList = StepJobInfoList.GetDataList(type, level);

            int iCompleteCount = 0;
            int iLevelCount = dataList.Count();

            foreach (var data in dataList)
            {
                if (data.StepStatus == StepStatus.Complete)
                {
                    iCompleteCount++;
                    continue;
                }

                switch (data.ParameterId)
                {
                    case "SHOWCCTVLIST":
                        //ProcessingShowCCTVList();
                        break;
                    default:
                        {
                            if (ProcessingDataFromPLC(data) == StepStatus.Complete)
                            {
                                iCompleteCount++;
                            }
                        }
                        break;
                }
            }

            if (iCompleteCount == iLevelCount)
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        private void StepParameterStatusUpdate(StepJobInfo info)
        {
            Hashtable htBody = new Hashtable();
            htBody.Add("JOBORDERID", this.m_sJobOrderID);
            htBody.Add("COMPOSITIONID", this.m_sCompositionID);
            htBody.Add("PARAMETERID", info.ParameterId);
            htBody.Add("TAGID", info.TagId);

            switch (info.StepStatus)
            {
                case StepStatus.Processing:
                    htBody.Add("PARAMETERSTATUS", "Processing");
                    break;
                case StepStatus.Complete:
                    htBody.Add("PARAMETERSTATUS", "Complete");
                    break;
                case StepStatus.Error:
                    htBody.Add("PARAMETERSTATUS", "Error");
                    break;
            }
            LogManager.Instance.Information($"StepJobParameterStatusUpdate : {this.m_sJobOrderID}");
            MessageHandler.SendMessageAsync("StepJobParameterStatusUpdate", htBody);
        }

        public bool stepJobProcess()
        {
            bool isComplete = false;

            if (StepJobInfoList == null)
                return false;

            if (StepJobInfoList.Count == 0)
                return false;

            switch (m_sStatus)
            {
                case StepJobStatus.Create:
                    {
                        stepJobProcessing(StepJobStatus.Start, ActionType.Start);
                    }
                    break;
                case StepJobStatus.Start:
                case StepJobStatus.StartReported:
                    {
                        if (m_sStepJobType == "Execute")
                        {
                            stepJobProcessing(StepJobStatus.Complete, ActionType.Execute);
                        }
                        else
                        {
                            stepJobProcessing(StepJobStatus.Complete, ActionType.End);
                        }
                    }
                    break;
            }

            return true;
            /*
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
                                                                    HttpManager.This().HttpSendData(sDeviceAddress, "opened");
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

                                                        Hashtable htTagMaster = TagMasterManager.This().GetTagMasterHashTable();

                                                        if (htTagMaster != null)
                                                        {
                                                            TagMasterData tagMaster = (TagMasterData)htTagMaster[sParameterTagID];

                                                            if (tagMaster != null)
                                                            {
                                                                tagMaster.sCurrentValue = sParameterValue;
                                                                isWrite = true;
                                                            }
                                                        }

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
                                                                    HttpManager.This().HttpSendData(sDeviceAddress, "opened");
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
                    else
                    {
                        isComplete = true;
                    }
                }

                if (isComplete)
                {
                    StartTaskProcess = Task.Run(() => EventChecker(ReportCompleteAddress, ReportCompleteReplyAddress, "Start", "End"));
                }
            }*/
        }

        public void stepJobProcessing(StepJobStatus nextStepJobStatus, ActionType type)
        {
            if (m_iMaxLevel == 0)
            {
                m_iMaxLevel = StepJobInfoList.GetMaxLevelByActionType(type);
                m_iNowLevel = 1;
            }

            if (TaskProcess != null)
            {
                if (TaskProcess.IsCompleted && StepJobInfoList.IsCompleteLevels(type, m_iNowLevel))
                {
                    if (m_iNowLevel < m_iMaxLevel)
                    {
                        TaskProcess = null;
                        m_iNowLevel++;
                    }
                    else if (m_iNowLevel == m_iMaxLevel)
                    {
                        TaskProcess = null;
                        m_iMaxLevel = 0;
                        m_sStatus = nextStepJobStatus;
                    }
                }
            }
            else
            {
                if (TaskProcess == null && stepJobLevelProcess(type, m_iNowLevel))
                {
                    var dataEventReportList = StepJobInfoList.GetEventReportList(type, m_iNowLevel);

                    if (dataEventReportList == null || dataEventReportList.Count() == 0)
                    {
                        m_sStatus = nextStepJobStatus;
                        return;
                    }

                    foreach (var eventReportItem in dataEventReportList)
                    {
                        var eventReportReplyItem = StepJobInfoList.GetDataByParameterId(eventReportItem.ReceivedId);

                        if (eventReportReplyItem == null)
                            continue;

                        TaskProcess = Task.Run(() => EventChecker(eventReportItem, eventReportReplyItem));
                        break;
                    }
                }
            }
        }

        /*
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

        private void WriteEventTag(string address, string value)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("DATAACTIONTYPE", typeof(string));
            dt.Columns.Add("DATATYPE", typeof(string));
            dt.Columns.Add("ADDRESS", typeof(string));
            dt.Columns.Add("VALUE", typeof(string));

            dt.Rows.Add("SET", "BOOL", address, value);

            //m_pXGTEnetDoc.WriteTagStation(m_sEquipmentID, dt);
        }
        */

        private void EventChecker(StepJobInfo eventReport, StepJobInfo eventReportReply)
        {
            var eventReportTag = station.GetTagList().FindByTagId(eventReport.TagId);
            var eventReportReplyTag = station.GetTagList().FindByTagId(eventReportReply.TagId);

            while (true)
            {
                switch (eventReport.DataActionType)
                {
                    case DataActionType.Get:
                        {
                            switch (eventReportTag.sTagVal)
                            {
                                case "1":
                                    if(m_bEventCheckerStart == false && eventReport.StepStatus == StepStatus.Processing)
                                    {
                                        m_bEventCheckerStart = true;
                                        eventReportReplyTag._TagVal = "1";
                                        eventReport.StepStatus = StepStatus.Complete;
                                    }
                                    break;
                                case "0":
                                    if (m_bEventCheckerStart && eventReportReply.StepStatus == StepStatus.Processing)
                                    {
                                        eventReportReplyTag._TagVal = "0";
                                        eventReportReply.StepStatus = StepStatus.Complete;
                                        m_bEventCheckerStart = false;
                                    }
                                    break;
                            }
                            break;
                        }
                    case DataActionType.Set:
                        {
                            if (m_bEventCheckerStart == false)
                            {
                                eventReportTag._TagVal = "1";
                                m_bEventCheckerStart = true;
                            }

                            switch (eventReportReplyTag.sTagVal)
                            {
                                case "1":
                                case "2":
                                    if (m_bEventCheckerStart && eventReport.StepStatus == StepStatus.Processing)
                                    {
                                        var errorData = StepJobInfoList.GetErrorDataByParameterId(eventReportReply.ParameterId, eventReportReply.ActionType, eventReportReply.ParameterLevel);

                                        if (errorData != null)
                                        {
                                            var errorDataTag = station.GetTagList().FindByTagId(errorData.TagId);

                                            if (eventReportReplyTag.sTagVal == "2")
                                            {
                                                errorData.StepStatus = StepStatus.Error;
                                            }
                                            else
                                            {
                                                errorData.StepStatus = StepStatus.Complete;
                                            }

                                            StepParameterStatusUpdate(errorData);
                                        }

                                        eventReportTag._TagVal = "0";
                                        eventReport.StepStatus = StepStatus.Complete;
                                    }
                                    break;
                                case "0":
                                    if (m_bEventCheckerStart && eventReport.StepStatus == StepStatus.Complete)
                                    {
                                        eventReportReply.StepStatus = StepStatus.Complete;
                                        m_bEventCheckerStart = false;
                                    }

                                    break;
                            }
                            break;
                        }
                    }

                StepParameterStatusUpdate(eventReport);
                StepParameterStatusUpdate(eventReportReply);

                if (eventReport.StepStatus == StepStatus.Complete && eventReportReply.StepStatus == StepStatus.Complete)
                    break;
                else
                    Thread.Sleep(1000);
            }
        }

        public async Task RunJob(CancellationToken token)
        {
            SendStatus(StepJobStatus.Start);

            if (StepJobInfoList != null)
            {
                foreach (var stepJob in StepJobInfoList.OrderBy(e => e.ParameterLevel))
                {
                    bool success = false;

                    var tag = station.GetTagList().FindByTagId(stepJob.TagId);



                    stepJob.StepStatus = StepStatus.Processing;


                    switch (stepJob.DataProcessType)
                    {
                        case DataProcessType.Data:
                            {
                                if (stepJob.DataActionType == DataActionType.Set)
                                {
                                    tag._TagVal = stepJob.TagValue;

                                    LogManager.Instance.Debug($"[StepJob] {stepJob.TagId} = {stepJob.TagValue}");
                                }
                                //else if (stepJob.DataActionType == DataActionType.Get)
                                //{
                                //    stepJob.TagValue = tag.sTagVal;
                                //}

                                success = true;
                                break;
                            }
                        case DataProcessType.Event:
                            {
                                var eventTag = station.GetTagList().FindByTagId(stepJob.TagId);
                                var eventAckTag = station.GetTagList().FindByTagId(stepJob.ReceivedId);

                                eventAckTag.Unset();
                                eventTag.Set();
                                LogManager.Instance.Debug($"[StepJob] Send Event({stepJob.TagId}) : {eventTag.sTagVal}");

                                while (true)
                                {
                                    LogManager.Instance.Debug($"[StepJob] Wait Event Ack({stepJob.ReceivedId}) : {eventAckTag.sTagVal}");

                                    if (eventAckTag.IsSet)
                                    {
                                        await Task.Delay(100, token);
                                        eventTag.Unset();
                                        eventAckTag.Unset();
                                        success = true;

                                        LogManager.Instance.Debug($"[StepJob] Event Ack Completed({stepJob.ReceivedId}) : {eventAckTag.sTagVal}");
                                        await Task.Delay(100, token);
                                        break;
                                    }

                                    await Task.Delay(100, token);
                                }

                                break;
                            }
                        case DataProcessType.EventReport:
                            {
                                var eventReportTag = station.GetTagList().FindByTagId(stepJob.TagId);
                                var eventReportAckTag = station.GetTagList().FindByTagId(stepJob.ReceivedId);

                                if (eventReportTag == null)
                                {
                                    throw new Exception($"EventReport Tag Not Found : {stepJob.ParameterId}");
                                }

                                if (eventReportAckTag == null)
                                {
                                    throw new Exception($"EventReport Ack Tag Not Found : {stepJob.ReceivedId}");
                                }

                                eventReportAckTag.Unset();
                                
                                while (true)
                                {
                                    LogManager.Instance.Debug($"[StepJob] Wait EventReport({stepJob.TagId}) : {eventReportTag.sTagVal}");

                                    if (eventReportTag.IsSet)
                                    {
                                        eventReportAckTag.Set();
                                        eventReportTag.Unset();

                                        LogManager.Instance.Debug($"[StepJob] EventReport Ack Completed({stepJob.TagId})");

                                        await Task.Delay(100, token);
                                        eventReportAckTag.Unset();
                                        break;
                                    }
                                    await Task.Delay(100, token);
                                }
                                break;
                            }
                    }

                    if (success)
                        stepJob.StepStatus = StepStatus.Complete;
                    else
                        await Task.Delay(100, token);
                }
            }


            SendStatus(StepJobStatus.Complete);
        }

        public void SetError(string message, string stackTrace)
        {
            LogManager.Instance.Error(message);
            LogManager.Instance.Error(stackTrace);
        }

        private void SendStatus(StepJobStatus status)
        {
            Hashtable htBody = new Hashtable();
            htBody.Add("JOBORDERID", m_sJobOrderID);
            htBody.Add("EQUIPMENTID", m_sEquipmentID);
            htBody.Add("STEPJOBID", m_sStepJobID);
            htBody.Add("STEPSEQUENCE", m_sStepSequence);
            htBody.Add("COMPOSITIONID", m_sCompositionID);

            string sMessageID = string.Empty;
            switch (status)
            {
                case StepJobStatus.Start:
                    sMessageID = "StepJobStart";
                    break;
                case StepJobStatus.Complete:
                    sMessageID = "StepJobEnd";
                    break;
            }

            LogManager.Instance.Information($"{sMessageID} : {m_sJobOrderID}");
            MessageHandler.SendMessageAsync(sMessageID, htBody);
        }


        //throw new NotImplementedException();
    }

        /*private void EventChecker(string sReportAddress, string sReportReplyAddress, string sCurrentJobStatus, string sNextJobStatus)
        {
            Hashtable htTagMaster = TagMasterManager.This().GetTagMasterHashTable();

            if (htTagMaster != null)
            {
                switch (sCurrentJobStatus)
                {
                    case "Create":
                        {
                            var address = sReportAddress.Split("_")[1];
                            TagMasterData tagMaster = (TagMasterData)htTagMaster[sReportReplyAddress];

                            WriteEventTag(address, "1");

                            bool bComplete = false;

                            while (true)
                            {
                                switch (tagMaster.sCurrentValue)
                                {
                                    case "1":
                                        m_sStatus = sNextJobStatus;
                                        WriteEventTag(address, "0");
                                        bComplete = true;
                                        break;
                                    case "2":
                                        PlcResponseStartFailed = true;
                                        m_sStatus = sNextJobStatus;
                                        WriteEventTag(address, "0");
                                        bComplete = true;
                                        break;
                                }

                                if (bComplete)
                                    break;

                                Thread.Sleep(1000);
                            }
                        }
                        break;
                    case "Start":
                        {
                            var address = sReportReplyAddress.Split("_")[1];
                            TagMasterData tagMaster = (TagMasterData)htTagMaster[sReportAddress];

                            while (true)
                            {
                                if (tagMaster.sCurrentValue == "1")
                                {
                                    m_sStatus = sNextJobStatus;
                                    WriteEventTag(address, "1");
                                    break;
                                }

                                Thread.Sleep(1000);
                            }

                            while (true)
                            {
                                if (tagMaster.sCurrentValue == "0")
                                {
                                    WriteEventTag(address, "0");
                                    break;
                                }

                                Thread.Sleep(1000);
                            }
                        }
                        break;
                }
            }
        }*/
    
}

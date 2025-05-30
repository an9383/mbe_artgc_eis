using System;
using System.Collections;

using System.Data;
using KR.MBE.CommonLibrary.Handler;
using KR.MBE.CommonLibrary.Manager;
using KR.MBE.CommonLibrary.Utils;


namespace EISDataFilter.LotTracking
{
    public class PROCESSEVENT
    {
        public bool Dowork(Hashtable htEventTagList)
        {
            Hashtable htTagMaster = TagMasterManager.This().GetTagMasterHashTable();
            DataTable dtEventTag = TagMasterManager.This().GetTagDataTable("Event");

            bool bReturn = false;
            bool bStepJobCheck = false;
            string sCurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sEventMessage = "";
            string sErrSourceMsg = "";
            string sHeader = "";
            string sCurrentValue = "";
            string sDataInfo = "";
            string sFilter = "";
            string TempTagValue = "";

            // StepJob event Check
            //JobOrderFactory oFactory = JobOrderFactory.This();
            //Hashtable htTemp = (Hashtable)oFactory.m_htCreateStepJobList.Clone();

            //foreach (object key in htTemp.Keys)
            //{
            //    string sStepJobID = key.ToString();
            //    JobOrderManager oManager = oManager = (JobOrderManager)oFactory.m_htCreateStepJobList[sStepJobID];

            //    if (oManager!= null)
            //    { 
            //        Hashtable htJobOrderReadTagList = oManager.m_htReadList;

            //        foreach (object readTagID in htJobOrderReadTagList.Keys)
            //        {
            //            if (htEventTagList.ContainsKey(readTagID))
            //            {
            //                bStepJobCheck = true;
            //                break;
            //            }
            //        }

            //        if (bStepJobCheck)
            //        {
            //            oFactory.processStepJob(sStepJobID);
            //        }
            //    }
            //}

            foreach (object obj in htEventTagList.Keys)
            {
                TagMasterData TagInfo = (TagMasterData)htTagMaster[obj];
                Random rd = new Random();

                sDataInfo = "<DATAINFO>";
                sDataInfo += StaticUtil.MakeXmlData(StaticUtil.m_sPlantID, "PLANTID");
                sDataInfo += StaticUtil.MakeXmlData(TagInfo.sEquipmentID, "EQUIPMENTID");
                sDataInfo += StaticUtil.MakeXmlData(TagInfo.sTagID, "TAGID");
                //sDataInfo += CommonLibrary.MakeXmlData(TagInfo.sCurrentValue, "TAGVALUE");
                sDataInfo += StaticUtil.MakeXmlData(rd.Next(1,2).ToString(), "TAGVALUE");
                sDataInfo += StaticUtil.MakeXmlData(sCurrentTime, "EVENTTIME");
                sDataInfo += "</DATAINFO>";

                sEventMessage += sDataInfo;

                // Evnet Reference TagID
                sFilter = "and TagID = '" + TagInfo.sTagID + "'";
                sFilter += "and TagValue = '" + TagInfo.sCurrentValue + "'";

                //DataRow[] drEventTag;
                //drEventTag = dtEventTag.Select(sFilter);

                //for (int i = 0; i < drEventTag.Length; i++)
                //{
                //    TagMasterData EventTagInfo = (TagMasterData)htTagMaster[drEventTag[i]["ReferenceTagID"].ToString()];

                //    if (EventTagInfo != null)
                //    {
                //        sDataInfo = "<DATAINFO>";
                //        sDataInfo += CommonLibrary.MakeXmlData(CommonLibrary.m_sPlantID, "PLANTID");
                //        sDataInfo += CommonLibrary.MakeXmlData(TagInfo.sEquipmentID, "EQUIPMENTID");
                //        sDataInfo += CommonLibrary.MakeXmlData(EventTagInfo.sTagID, "TAGID");
                //        sDataInfo += CommonLibrary.MakeXmlData(EventTagInfo.sCurrentValue, "RESULTVALUE");
                //        sDataInfo += CommonLibrary.MakeXmlData(sCurrentTime, "EVENTTIME");
                //        sDataInfo += "</DATAINFO>";

                //        sEventMessage += sDataInfo;
                //    }
                //}
            }

            if (sEventMessage != null)
            {
                sEventMessage = StaticUtil.MakeXmlData(sEventMessage, "DATALIST");
                //sEventMessage = CommonLibrary.MakeXmlData(sEventMessage, "body");

                try
                {
                    //Send Message..
                    MessageHandler.SendMessageAsyncToMonitoring("MonitoringTagStatus", sEventMessage);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Exception(ex);
                }

                //INSERT IFXMLMSGSET..
                //DBHandler dbHandler = SQLPoolManager.This().getUnuseCollection();

                //bReturn = dbHandler.insertIFXMLMSGSET(sEventMessage, "PROCESSEVENT");

                //SQLPoolManager.This().setUnuseCollection(dbHandler);

                //MessageHandler.SendMessageAsyncEco("stepJobEvent", sEventMessage);
            }

            return bReturn;
        }
        public bool Dowork(DataTable dtTagList)
        {
            bool bReturn = false;
            bool bStepJobCheck = false;
            string sCurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sEventMessage = "";
            string sDataInfo = "";

            if (sEventMessage != null)
            {
                sEventMessage = StaticUtil.MakeXmlData(sEventMessage, "DATALIST");
                //sEventMessage = CommonLibrary.MakeXmlData(sEventMessage, "body");

                try
                {
                    //Send Message..
                    MessageHandler.SendMessageAsyncToMonitoring("MonitoringTagStatus", sEventMessage);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Exception(ex);
                }

                //INSERT IFXMLMSGSET..
                //DBHandler dbHandler = SQLPoolManager.This().getUnuseCollection();

                //bReturn = dbHandler.insertIFXMLMSGSET(sEventMessage, "PROCESSEVENT");

                //SQLPoolManager.This().setUnuseCollection(dbHandler);

                //MessageHandler.SendMessageAsyncEco("stepJobEvent", sEventMessage);
            }

            return bReturn;
        }


        public bool DoworkAlarm(string OpcSrvName, string AlarmID, string AlarmType)
        {
            bool bReturn = false;
            string sCurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sEventMessage = "";
            string sErrSourceMsg = "";
            string sHeader = "";
            string sDataInfo = "";

            if (AlarmID != null)
            {
                sDataInfo = "<DATAINFO>";
                sDataInfo += StaticUtil.MakeXmlData(AlarmType, "ALARMTYPE");
                sDataInfo += StaticUtil.MakeXmlData(OpcSrvName, "STATIONNAME");
                sDataInfo += StaticUtil.MakeXmlData(AlarmID, "ALARMID");
                sDataInfo += StaticUtil.MakeXmlData("", "DESCRIPTION");
                sDataInfo += StaticUtil.MakeXmlData(sCurrentTime, "ALARMTIME");
                sDataInfo += "</DATAINFO>";

                sEventMessage += sDataInfo;
            }

            if (sEventMessage != null)
            {
                sEventMessage = StaticUtil.MakeXmlData(sEventMessage, "DATALIST");
                //sEventMessage = CommonLibrary.MakeXmlData(sEventMessage, "body");

                try
                {
                    //Send Message..
                    MessageHandler.SendMessageAsync("TAGDATA", sEventMessage);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Exception(ex);
                }

                //INSERT IFXMLMSGSET..
                DBHandler dbHandler = SQLPoolManager.This().getUnuseCollection();

                //bReturn = dbHandler.insertIFXMLMSGSET(sEventMessage, "EISAlarmSMSSend");

                SQLPoolManager.This().setUnuseCollection(dbHandler);
            }

            return bReturn;
        }
    }
}

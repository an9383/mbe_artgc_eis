using System;
using System.Linq;
using System.Data;
using System.Xml;
using KR.MBE.CommonLibrary.Utils;

namespace EISDataFilter.LotTracking
{
    public class PARAMETERRESULT
    {
        public bool Dowork(DataTable dtTagList)
        {
            bool bReturn = false;
            string sCurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sEventMessage = "";
            string sDataInfo = "";

            for (int i = 0; i < dtTagList.Rows.Count; i++)
            {
                string sEquipmentID = dtTagList.Rows[i]["EQUIPMENTID"].ToString();
                string sTagID = dtTagList.Rows[i]["TAGID"].ToString();
                string sTagValue = dtTagList.Rows[i]["TAGVALUE"].ToString();

                sDataInfo = "<DATAINFO>";
                sDataInfo += StaticUtil.MakeXmlData(StaticUtil.m_sPlantID, "PLANTID");
                sDataInfo += StaticUtil.MakeXmlData(sEquipmentID, "EQUIPMENTID");
                sDataInfo += StaticUtil.MakeXmlData(sTagID, "TAGID");
                sDataInfo += StaticUtil.MakeXmlData(sTagValue, "RESULTVALUE");
                sDataInfo += StaticUtil.MakeXmlData(sCurrentTime, "EVENTTIME");
                sDataInfo += "</DATAINFO>";

                sEventMessage += sDataInfo;
            }

            return bReturn;
        }

        public bool DoworkParam(DataSet ds)
        {
            //XMLHandler _XMLHandler = new XMLHandler();
            //CEventLogger _log = new CEventLogger();

            bool bReturn = false;
            //DataRow[] drParam ;
            //string sCurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //DataTable dtMainList = new DataTable();
            ////dtMainList = ds.Tables[0];          

            //drParam = ds.Tables[0].Select("ProcessMethodType = 'Batch'", "UnitID ASC");

            //if (drParam.Length > 0)
            //{
            //    //dtMainList = ds.Tables[0];
            //    dtMainList = ds.Tables[0].Select("ProcessMethodType = 'Batch'", "UnitID ASC").CopyToDataTable<DataRow>();

            //    //_log.SaveErrLog("[" + sCurrentTime + "] PARAMETERRESULT DataRowCount : " + ds.Tables[0].Rows.Count.ToString());

            //    //xml Message를 만들기 위한 DataTable
            //    DataTable dtList = new DataTable();

            //    dtList.Columns.Add("nodeType", typeof(string));
            //    dtList.Columns.Add("nodeSEQ", typeof(string));
            //    dtList.Columns.Add("nodeKey", typeof(string));
            //    dtList.Columns.Add("nodeValue", typeof(string));

            //    dtList.Rows.Add("H", "0", "messagename", "PARAMETERRESULT");
            //    dtList.Rows.Add("H", "0", "sourcesubject", "KCC.PROD.KR.A33.EIS.LOCAL");
            //    dtList.Rows.Add("H", "0", "targetsubject", "KCC.PROD.KR.A33.EIS.BMASTER");
            //    dtList.Rows.Add("H", "0", "replysubject", "KCC.PROD.KR.A33.EIS.LOCAL");
            //    dtList.Rows.Add("H", "0", "transactionid", sCurrentTime);

            //    dtList.Rows.Add("B", "0", "DATALIST", "");

            //    //조건에 맞는 데이터를 찾아 해당 Unit ID, 설비ID만 가져온다..
            //    var EqpList = from item in dtMainList.AsEnumerable()
            //                  where item.Field<string>("ParameterType").ToUpper() == "TRUE"
            //                  //orderby item.Field<string>("UnitID") ascending
            //                  select new
            //                  {
            //                      EquipmentID = item.Field<string>("EquipmentID").ToString(),
            //                      OrderEquipmentID = item.Field<string>("OrderEquipmentID").ToString(),
            //                      UNITID = item.Field<string>("UnitID").ToString(),
            //                      TAGID = item.Field<string>("TagID").ToString(),
            //                      TAGNAME = item.Field<string>("Description").ToString(),
            //                      RESULTVALUE = item.Field<string>("TagValue").ToString(),
            //                      PARAMETERTYPE = item.Field<string>("ParameterType").ToString()
            //                  };


            //    foreach (var item in EqpList)
            //    {
            //        //PARAMETERTYPE이 True인것만 MessageSet구성한다..
            //        if (item.PARAMETERTYPE.ToUpper().Equals("TRUE"))
            //        {
            //            dtList.Rows.Add("B", "1", "DATAINFO", "");
            //            dtList.Rows.Add("B", "2", "UNITID", item.UNITID.ToString());
            //            dtList.Rows.Add("B", "2", "EQUIPMENTID", item.EquipmentID.ToString());
            //            dtList.Rows.Add("B", "2", "ORDEREQPID", item.OrderEquipmentID.ToString());
            //            dtList.Rows.Add("B", "2", "TAGID", item.TAGID.ToString());
            //            dtList.Rows.Add("B", "2", "RESULTVALUE", item.RESULTVALUE.ToString());
            //            dtList.Rows.Add("B", "2", "EVENTTIME", sCurrentTime);
            //        }
            //    }

            //    XmlDocument xmlDoc = _XMLHandler.makeEISMessageSet(dtList);

            //    try
            //    {
            //        //Send Message..
            //        MessageHandler _MsgHandler = new MessageHandler();
            //        _MsgHandler.SendMessage_BMEIS(xmlDoc.InnerXml);
            //    }
            //    catch (Exception err)
            //    {
            //        _log.SaveErrLog(err.ToString());
            //    }

            //    //INSERT IFXMLMSGSET..
            //    DBHandler dbHandler = SQLPoolManager.This().getUnuseCollection();

            //    bReturn = dbHandler.insertIFXMLMSGSET(xmlDoc.InnerXml, "PARAMETERRESULT");

            //    SQLPoolManager.This().setUnuseCollection(dbHandler);
            //}
            return bReturn;
        }

        public bool Dowork(DataSet ds)
        {
            bool bReturn = false;
            string sCurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sEventMessage = "";
            string sDataInfo = "";

            //for (int i = 0; i < dtTagList.Rows.Count; i++)
            //{
            //    string sEquipmentID = dtTagList.Rows[i]["EQUIPMENTID"].ToString();
            //    string sTagID = dtTagList.Rows[i]["TAGID"].ToString();
            //    string sTagValue = dtTagList.Rows[i]["TAGVALUE"].ToString();

            //    sDataInfo = "<DATAINFO>";
            //    sDataInfo += CommonLibrary.MakeXmlData(CommonLibrary.m_sPlantID, "PLANTID");
            //    sDataInfo += CommonLibrary.MakeXmlData(sEquipmentID, "EQUIPMENTID");
            //    sDataInfo += CommonLibrary.MakeXmlData(sTagID, "TAGID");
            //    sDataInfo += CommonLibrary.MakeXmlData(sTagValue, "RESULTVALUE");
            //    sDataInfo += CommonLibrary.MakeXmlData(sCurrentTime, "EVENTTIME");
            //    sDataInfo += "</DATAINFO>";

            //    sEventMessage += sDataInfo;
            //}

            //if (sEventMessage != null)
            //{
            //    sEventMessage = CommonLibrary.MakeXmlData(sEventMessage, "DATALIST");
            //    sEventMessage = CommonLibrary.MakeXmlData(sEventMessage, "body");

            //    try
            //    {
            //        //Send Message..
            //        MessageHandler.SendMessageAsync("TAGDATA", sEventMessage);
            //        MessageHandler.SendMessageAsyncToMonitor("TAGDATA", sEventMessage);
            //    }
            //    catch (Exception err)
            //    {
            //        _log.SaveErrLog(err.ToString());
            //    }

            //    //INSERT IFXMLMSGSET..
            //    DBHandler dbHandler = SQLPoolManager.This().getUnuseCollection();

            //    //bReturn = dbHandler.insertIFXMLMSGSET(sEventMessage, "PROCESSEVENT");

            //    SQLPoolManager.This().setUnuseCollection(dbHandler);
            //}

            return bReturn;
        }
    }
}

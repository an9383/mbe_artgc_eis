using KR.MBE.CommonLibrary.Utils;
using log4net.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR.MBE.CommonLibrary.Struct
{
    public class StepJobInfoList : List<StepJobInfo>
    {
        public StepJobInfoList()
        {
        }

        public StepJobInfoList(Hashtable data) : base()
        {
            foreach (var item in data)
            {
                var fTagId = ConvertUtil.GetXMLRecord(item.ToString(), "TAGID").Trim();

                if (string.IsNullOrEmpty(fTagId) || fTagId.IndexOf('_') < 0)
                    continue;

                AddInfo(item.ToString());
            }
        }

        private void AddInfo(string sMessage)
        {
            string sTagID = ConvertUtil.GetXMLRecord(sMessage, "TAGID");
            string sTagValue = ConvertUtil.GetXMLRecord(sMessage, "TAGVALUE");
            string sDataActionType = ConvertUtil.GetXMLRecord(sMessage, "DATAACTIONTYPE");

            this.Add(new StepJobInfo(sMessage));
        }

        public int GetMaxLevelByActionType(ActionType type)
        {
            return this.Where(x => x.ActionType == type).Max(x => x.ParameterLevel);
        }

        public IEnumerable<StepJobInfo> GetDataList(ActionType type, int level, bool isNotFinish = true)
        {
            if (isNotFinish)
            {
                return this.Where(x => x.ParameterLevel == level && x.DataProcessType == DataProcessType.Data && x.ActionType == type && x.StepStatus == StepStatus.Processing);
            }
            else
            {
                return this.Where(x => x.ParameterLevel == level && x.DataProcessType == DataProcessType.Data && x.ActionType == type);
            }
        }

        public IEnumerable<StepJobInfo> GetEventList(ActionType type, int level)
        {
            return this.Where(x => x.ParameterLevel == level && x.ActionType == type && x.DataProcessType != DataProcessType.Data);
        }

        public IEnumerable<StepJobInfo> GetEventReportList(ActionType type, int level, bool isNotFinish = true)
        {
            if (isNotFinish)
            {
                return this.Where(x => x.ParameterLevel == level && x.ActionType == type && x.DataProcessType == DataProcessType.EventReport && x.StepStatus == StepStatus.Processing);
            }
            else
            {
                return this.Where(x => x.ParameterLevel == level && x.ActionType == type && x.DataProcessType == DataProcessType.EventReport);
            }
        }

        public IEnumerable<StepJobInfo> GetEventReportReportList(ActionType type, int level, bool isNotFinish = true)
        {
            if (isNotFinish)
            {
                return this.Where(x => x.ParameterLevel == level && x.ActionType == type && x.DataProcessType == DataProcessType.EventReport && x.StepStatus == StepStatus.Processing);
            }
            else
            {
                return this.Where(x => x.ParameterLevel == level && x.ActionType == type && x.DataProcessType == DataProcessType.EventReport);
            }
        }

        public StepJobInfo? GetDataByTagId(string tagId)
        {
            return this.Where(x => x.TagId == tagId).FirstOrDefault();
        }
        public StepJobInfo? GetDataByParameterId(string parameterId)
        {
            return this.Where(x => x.ParameterId == parameterId).FirstOrDefault();
        }

        public StepJobInfo? GetErrorDataByParameterId(string parameterId, ActionType type, int level)
        {
            return this.Where(x => x.ReceivedId == parameterId && x.ParameterLevel == level && x.ActionType == type).FirstOrDefault();
        }

        public bool IsCompleteLevels(ActionType type, int level)
        {
            var completeCount = this.Where(x => x.ParameterLevel == level && x.ActionType == type && x.StepStatus == StepStatus.Complete).Count();
            var errorCount = this.Where(x => x.ParameterLevel == level && x.ActionType == type && x.StepStatus == StepStatus.Error).Count();
            var totalCount = this.Where(x => x.ParameterLevel == level && x.ActionType == type).Count();

            if (totalCount == (completeCount + errorCount))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public enum ActionType
    {

        [Description("START")] Start = 1,
        [Description("END")] End = 2,
        [Description("EXECUTE")] Execute = 3
    }

    public enum DataActionType
    {

        [Description("GET")] Get = 1,
        [Description("SET")] Set = 2
    }

    public enum DataProcessType
    {
        [Description("EVENT")] Event = 1,
        [Description("DATA")] Data = 2,
        [Description("EVENTREPORT")] EventReport = 3,
        [Description("EVENTREPLY")] EventReply = 4,
    }

    public enum StepStatus
    {
        [Description("Processing")] Processing = 1,
        [Description("Complete")] Complete = 2,
        [Description("Error")] Error = 3

    }
    public class StepJobInfo
    {
        public ActionType ActionType { get; set; }
        public string ParameterId { get; set; }
        public int ParameterLevel { get; set; }
        public string TagId { get; set; }
        public string DataType { get; set; }
        public string Address { get; set; }
        public string TagValue { get; set; }
        public DataActionType DataActionType { get; set; }
        public DataProcessType DataProcessType { get; set; }
        public StepStatus StepStatus { get; set; } = StepStatus.Processing;
        public string ReceivedId { get; set; }
        public string DataTarget { get; set; }
        public string DataFailTarget { get; set; }

        public StepJobInfo()
        {

        }

        public StepJobInfo(string sMessage) : base()
        {
            ParameterId = ConvertUtil.GetXMLRecord(sMessage, "PARAMETERID");
            ParameterLevel = int.Parse(ConvertUtil.GetXMLRecord(sMessage, "PARAMETERLEVEL"));
            TagId = ConvertUtil.GetXMLRecord(sMessage, "TAGID");

            switch (ConvertUtil.GetXMLRecord(sMessage, "ACTIONTYPE").ToUpper())
            {
                case "START":
                    ActionType = ActionType.Start;
                    break;
                case "END":
                    ActionType = ActionType.End;
                    break;
                default:
                case "EXECUTE":
                    ActionType = ActionType.Execute;
                    break;
            }

            DataType = ConvertUtil.GetXMLRecord(sMessage, "DATATYPE");
            Address = ConvertUtil.GetXMLRecord(sMessage, "ADDRESS");
            TagValue = ConvertUtil.GetXMLRecord(sMessage, "TAGVALUE");

            switch (ConvertUtil.GetXMLRecord(sMessage, "DATAACTIONTYPE").ToUpper())
            {
                case "SET":
                    DataActionType = DataActionType.Set;
                    break;
                default:
                case "GET":
                    DataActionType = DataActionType.Get;
                    break;
            }

            switch (ConvertUtil.GetXMLRecord(sMessage, "DATAPROCESSTYPE").ToUpper())
            {
                case "EVENT":
                    DataProcessType = DataProcessType.Event;
                    break;
                case "DATA":
                    DataProcessType = DataProcessType.Data;
                    break;
                case "EVENTREPORT":
                    DataProcessType = DataProcessType.EventReport;
                    break;
                case "EVENTREPLY":
                    DataProcessType = DataProcessType.EventReply;
                    break;
            }

            ReceivedId = ConvertUtil.GetXMLRecord(sMessage, "RECEIVEDID");
            DataTarget = ConvertUtil.GetXMLRecord(sMessage, "DATATARGET");
            DataFailTarget = ConvertUtil.GetXMLRecord(sMessage, "DATAFAILTARGET");

            switch (ConvertUtil.GetXMLRecord(sMessage, "PARAMETERSTATUS").ToUpper())
            {
                case "Processing":
                    StepStatus = StepStatus.Processing;
                    break;
                case "Complete":
                    StepStatus = StepStatus.Complete;
                    break;
                case "Error":
                    StepStatus = StepStatus.Error;
                    break;
            }
        }
    }
}

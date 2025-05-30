using System;
using System.Collections;
using System.Threading;
using Apache.NMS.ActiveMQ.Commands;
using KR.MBE.CommonLibrary.Handler;
using KR.MBE.CommonLibrary.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace KR.MBE.CommonLibrary.Manager
{
    public class JobOrderFactoryManager
    {
        public static JobOrderFactoryManager m_JobOrderFactory = null;
        public Hashtable m_htCreateStepJobList = null;
        public static object m_oLock = new object();

        public JobOrderFactoryManager()
        {
            m_htCreateStepJobList = new Hashtable();
        }

        public static JobOrderFactoryManager This()
        {
            if (m_JobOrderFactory == null)
                m_JobOrderFactory = new JobOrderFactoryManager();
            return m_JobOrderFactory;
        }

        public JobOrderFactoryManager GetJobOrderManager(string sJobOrderID)
        {
            JobOrderFactoryManager oManager = (JobOrderFactoryManager)m_htCreateStepJobList[sJobOrderID];

            if (oManager == null)
            {
                JobOrderFactoryManager oeNewManager = new JobOrderFactoryManager();
                return oeNewManager;
            }
            else
            {
                return oManager;
            }
        }

        public void CreateStepJobInfo(string sMessage)
        {
            string sStepJobOrderID = ConvertUtil.GetXMLRecord(sMessage, "JOBORDERID");

            lock (m_oLock)
            {
                var StepJobCheck = m_htCreateStepJobList[sStepJobOrderID] as JobOrderManager;

                if (StepJobCheck != null)
                {
                    m_htCreateStepJobList.Remove(StepJobCheck.m_sJobOrderID);
                }

                var JobManager = new JobOrderManager(sMessage);
                //JobManager.setStepJobInformation(sMessage);

                m_htCreateStepJobList.Add(JobManager.m_sJobOrderID, JobManager);
            }

            //processStepJob(sStepJobOrderID);
        }

        /// <summary>
        /// StepJob 처리
        /// </summary>
        /// <param name="sStepJobOrderID">Step Job Order ID</param>
        public void processStepJob(string sStepJobOrderID)
        {            
            Hashtable htEndJobOrderList = new Hashtable();

            JobOrderManager JobManager = (JobOrderManager)m_htCreateStepJobList[sStepJobOrderID];

            if (JobManager != null)
            {            
                switch (JobManager.m_sStatus)
                {
                    case StepJobStatus.Start:
                        SendStepJobStatus(JobManager, StepJobStatus.Start);
                        JobManager.m_sStatus = StepJobStatus.StartReported;
                        break;
                    case StepJobStatus.Complete:
                        SendStepJobStatus(JobManager, StepJobStatus.Complete);
                        JobManager.m_sStatus = StepJobStatus.CompleteReported;
                        htEndJobOrderList.Add(sStepJobOrderID, "");
                        break;
                    default:
                        JobManager.stepJobProcess();
                        break;
                }
            }
        }

        private void SendStepJobStatus(JobOrderManager JobManager, StepJobStatus status)
        {
            Hashtable htBody = new Hashtable();
            htBody.Add("JOBORDERID", JobManager.m_sJobOrderID);
            htBody.Add("EQUIPMENTID", JobManager.m_sEquipmentID);
            htBody.Add("STEPJOBID", JobManager.m_sStepJobID);
            htBody.Add("STEPSEQUENCE", JobManager.m_sStepSequence);
            htBody.Add("COMPOSITIONID", JobManager.m_sCompositionID);

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

            LogManager.Instance.Information($"{sMessageID} : {JobManager.m_sJobOrderID}");
            MessageHandler.SendMessageAsync(sMessageID, htBody);
        }

        /// <summary>
        /// StepJob 처리
        /// </summary>
        public void processStepJob() 
        {
            Hashtable htEndJobOrderList = new Hashtable();

            if (m_htCreateStepJobList.Keys.Count <= 0)
                return;

            lock (m_oLock)
            {
                foreach (object key in m_htCreateStepJobList.Keys)
                {
                    var stepJobId = (string)key;
                    
                    if (htEndJobOrderList.ContainsKey(stepJobId) == false)
                    {
                        processStepJob(stepJobId);
                    }
                }

                // StepJobEnd JobOrder 삭제
                foreach (object key in htEndJobOrderList.Keys)
                {
                    JobOrderManager endJobCheck = (JobOrderManager)m_htCreateStepJobList[key];

                    if (endJobCheck != null)
                    {
                        m_htCreateStepJobList.Remove(endJobCheck.m_sJobOrderID);
                    }
                }
            }
        }

        /// <summary>
        /// StepJob 삭제
        /// </summary>
        /// <param name="sMessage">XML 전문 메시지</param>
        public void deleteStepJob(string sMessage)
        {
            string sStepJobOrderID = ConvertUtil.GetXMLRecord(sMessage, "JOBORDERID");

            JobOrderManager StepJobCheck = (JobOrderManager)m_htCreateStepJobList[sStepJobOrderID];

            if (StepJobCheck != null)
            {
                lock (m_oLock)
                {
                    m_htCreateStepJobList.Remove(StepJobCheck.m_sJobOrderID);
                } 
            }
        }
    }
}

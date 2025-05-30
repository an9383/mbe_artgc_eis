using System;
using System.Collections;
using KR.MBE.CommonLibrary.Handler;
using KR.MBE.CommonLibrary.Manager;
using KR.MBE.CommonLibrary.Utils;

namespace JobOrderManagement
{
    public class JobOrderFactory
    {
        public static JobOrderFactory m_JobOrderFactory = null;
        public Hashtable m_htCreateStepJobList = null;

        public JobOrderFactory()
        {
            m_htCreateStepJobList = new Hashtable();
        }

        public static JobOrderFactory This()
        {
            if (m_JobOrderFactory == null)
                m_JobOrderFactory = new JobOrderFactory();
            return m_JobOrderFactory;
        }
        public JobOrderManager GetJobOrderManager(string sJobOrderID)
        {
            JobOrderManager oManager = (JobOrderManager)m_htCreateStepJobList[sJobOrderID];

            if (oManager == null)
            {
                JobOrderManager oeNewManager = new JobOrderManager();
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

            JobOrderManager StepJobCheck = (JobOrderManager)m_htCreateStepJobList[sStepJobOrderID];

            if (StepJobCheck != null)
            {
                m_htCreateStepJobList.Remove(StepJobCheck.m_sJobOrderID);
            }

            JobOrderManager JobManager = new JobOrderManager();
            JobManager.setStepJobInformation(sMessage);

            m_htCreateStepJobList.Add(JobManager.m_sJobOrderID, JobManager);
            processStepJob(sStepJobOrderID);
        }

        public void processStepJob(string sStepJobOrderID)
        {            
            bool bMessageSend = false;
            string sMessageID = string.Empty;
            Hashtable htEndJobOrderList = new Hashtable();

            JobOrderManager StepJobCheck = (JobOrderManager)m_htCreateStepJobList[sStepJobOrderID];

            if (StepJobCheck != null)
            {
                JobOrderManager JobManager = (JobOrderManager)m_htCreateStepJobList[sStepJobOrderID];
                bMessageSend = JobManager.stepJobProcess();

                if (bMessageSend)
                {
                    if ("Start".Equals(JobManager.m_sStatus))
                    {
                        sMessageID = "StepJobStart";
                    }
                    else if ("End".Equals(JobManager.m_sStatus))
                    {
                        sMessageID = "StepJobEnd";
                        htEndJobOrderList.Add(sStepJobOrderID, "");
                    }

                    Hashtable htBody = new Hashtable();
                    htBody.Add("SITEID", JobManager.m_sSiteID);
                    htBody.Add("JOBORDERID", JobManager.m_sJobOrderID);
                    htBody.Add("EQUIPMENTID", JobManager.m_sEquipmentID);
                    htBody.Add("STEPJOBID", JobManager.m_sStepJobID);
                    htBody.Add("STEPSEQUENCE", JobManager.m_sStepSequence);
                    htBody.Add("COMPOSITIONID", JobManager.m_sCompositionID);

                    MessageHandler.SendMessageAsync(sMessageID, htBody);
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

        public void processStepJob()
        {
            Hashtable htEndJobOrderList = new Hashtable();

            foreach (object key in m_htCreateStepJobList.Keys)
            {
                bool bMessageSend = false;
                string sMessageID = string.Empty;

                JobOrderManager JobManager = (JobOrderManager)m_htCreateStepJobList[key];

                bMessageSend = JobManager.stepJobProcess();

                if (bMessageSend)
                {
                    Hashtable htBody = new Hashtable();
                    htBody.Add("SITEID", JobManager.m_sSiteID);
                    htBody.Add("JOBORDERID", JobManager.m_sJobOrderID);
                    htBody.Add("EQUIPMENTID", JobManager.m_sEquipmentID);
                    htBody.Add("STEPJOBID", JobManager.m_sStepJobID);
                    htBody.Add("STEPSEQUENCE", JobManager.m_sStepSequence);
                    htBody.Add("COMPOSITIONID", JobManager.m_sCompositionID);

                    if ("Start".Equals(JobManager.m_sStatus))
                    {
                        sMessageID = "StepJobStart";
                    }
                    else if ("End".Equals(JobManager.m_sStatus))
                    {
                        sMessageID = "StepJobEnd";
                        htEndJobOrderList.Add(key, "");
                    }

                    MessageHandler.SendMessageAsync(sMessageID, htBody);
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

        public void processStepJobComplete(string sJobStatus)
        {
            Hashtable htEndJobOrderList = new Hashtable();

            foreach (object key in m_htCreateStepJobList.Keys)
            {
                bool bMessageSend = false;
                string sMessageID = string.Empty;

                JobOrderManager JobManager = (JobOrderManager)m_htCreateStepJobList[key];

                bMessageSend = JobManager.stepJobCompleteProcess(sJobStatus);

                if (bMessageSend)
                {
                    Hashtable htBody = new Hashtable();
                    htBody.Add("SITEID", JobManager.m_sSiteID);
                    htBody.Add("JOBORDERID", JobManager.m_sJobOrderID);
                    htBody.Add("EQUIPMENTID", JobManager.m_sEquipmentID);
                    htBody.Add("STEPJOBID", JobManager.m_sStepJobID);
                    htBody.Add("STEPSEQUENCE", JobManager.m_sStepSequence);
                    htBody.Add("COMPOSITIONID", JobManager.m_sCompositionID);

                    if ("Start".Equals(JobManager.m_sStatus))
                    {
                        sMessageID = "StepJobStart";
                    }
                    else if ("End".Equals(JobManager.m_sStatus))
                    {
                        sMessageID = "StepJobEnd";
                        htEndJobOrderList.Add(key, "");
                    }

                    MessageHandler.SendMessageAsync(sMessageID, htBody);
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

        public void deleteStepJob(string sMessage)
        {
            string sStepJobOrderID = ConvertUtil.GetXMLRecord(sMessage, "JOBORDERID");

            JobOrderManager StepJobCheck = (JobOrderManager)m_htCreateStepJobList[sStepJobOrderID];

            if (StepJobCheck != null)
            {
                m_htCreateStepJobList.Remove(StepJobCheck.m_sJobOrderID);
            }
        }
    }
}

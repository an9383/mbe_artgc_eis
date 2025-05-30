using KR.MBE.CommonLibrary.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static TOS.Driver.CLT.Struct.YC.YCMethod;

namespace TOS.Driver.CLT
{
    public partial class Station
    {
        private void SubscribeMessage()
        {
            jsonClient.Subscribe<RequestReceiveJobList>("sendAycJob", (sender, list) => sendAycJob(list));
            jsonClient.Subscribe<RequestAbortJobList>("sendAbortJob", (sender, list) => sendAbortJob(list));
            jsonClient.Subscribe<RequestMoveJobList>("sendMoveJob", (sender, list) => sendMoveJob(list));
            jsonClient.Subscribe<ResponseClearanceList>("sendClearance", (sender, list) => sendClearance(list));
        }

        public void sendAycJob(RequestReceiveJobList list)
        {
            MessageHandler.SendMessageAsync("SetJobOrder", JsonConvert.SerializeObject(list));
            /*foreach (var job in list)
            {
            }*/
        }

        public void sendAbortJob(RequestAbortJobList list)
        {
            foreach (var job in list)
            {
                MessageHandler.SendMessageAsync(job.Name, job.getSendData());
            }
        }

        public void sendMoveJob(RequestMoveJobList list)
        {
            MessageHandler.SendMessageAsync("SendMoveJob", JsonConvert.SerializeObject(list));

            //foreach (var job in list)
            //{
            //    MessageHandler.SendMessageAsync(job.Name, job.getSendData());
            //}
        }
        public void sendClearance(ResponseClearanceList list)
        {
            foreach (var job in list)
            {
                MessageHandler.SendMessageAsync(job.Name, job.getSendData());
            }
        }
    }
}

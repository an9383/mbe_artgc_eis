using KR.MBE.CommonLibrary.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using Newtonsoft.Json;
using TOS.Driver.CLT.Struct.Common;
using static TOS.Driver.CLT.Struct.YC.YCMethod;

namespace TOS.Driver.CLT
{
    public partial class Station
    {
        public void sendAcceptJob(string data)
        {
            var deserializeObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            if (deserializeObject.ContainsKey("eqId") && deserializeObject.ContainsKey("jobId"))
            {
                var list = new List<ResponseAcceptJob>();
                var msg = new RequestMessage();
                var eqId = deserializeObject["eqId"];
                var jobId = deserializeObject["jobId"];
                var acceptJob = new ResponseAcceptJob(eqId, jobId);

                list.Add(acceptJob);
                
                msg.Name = acceptJob.Name;
                msg.Id = GetMsgCount();
                msg.Parameters = list; 

                jsonClient.Client.SendAsync(JsonConvert.SerializeObject(msg));
            }
        }
    }
}

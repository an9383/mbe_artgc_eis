using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TOS.Driver.CLT.Struct.Common;

namespace TOS.Driver.CLT.Struct.YC
{
    public partial class YCMethod
    {
        /// <summary>
        /// Send abort order result to TOS from AYC (Yard Crane) to EIS
        /// /equipment/ayc-job/abortStatus
        /// EIS -> IoT Platform -> OPUS (TOS)
        /// </summary>
        [DataContract]
        public class RequestAbortJobResponse
        {
            [JsonProperty("msgId")]
            public string msgId { get; set; }
            [JsonProperty("errCd")]
            public string errCd { get; set; }
            [JsonProperty("errDesc")]
            public string errDesc { get; set; }
        }
    }
}

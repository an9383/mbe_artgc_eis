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
        /// Send abort order to AYC (Yard Crane) from TOS to EIS
        /// /equipment/ayc-job/sendAbortJob
        /// OPUS (TOS) -> IoT Platform -> EIS
        /// </summary>
        [DataContract]
        public class RequestAbortJob : Base
        {
            public new string Name = "ReceiveAbortJob";

            [DataMember(Name = "head")]
            public Head Head { get; set; } = new Head();
            [DataMember(Name = "body")]
            public RequestAbortBody Body { get; set; } = new RequestAbortBody();

            public class RequestAbortBody : Body
            {
                [DataMember(Name = "result")]
                public string result { get; set; }
                [DataMember(Name = "errCd")]
                public string errCd { get; set; }
            }
        }
    }
}

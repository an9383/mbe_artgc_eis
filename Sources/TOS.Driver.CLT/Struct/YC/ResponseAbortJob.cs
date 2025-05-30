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
        public class ResponseAbortJob : Base
        {
            public new string Name = "SendAbortJob";

            [DataMember(Name = "head")]
            public Head Head { get; set; } = new Head();
            [DataMember(Name = "body")]
            public Body Body { get; set; } = new Body();
        }
    }
}

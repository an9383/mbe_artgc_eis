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
        /// Send move order to AYC (Yard Crane) from TOS to EIS
        /// /equipment/ayc-job/sendMoveJob
        /// OPUS (TOS) -> IoT Platform -> EIS
        /// </summary>
        [DataContract]
        public class RequestMoveJob : Base
        {
            public new string Name = "SendMoveJob";

            [DataMember(Name = "head")]
            public Head Head { get; set; } = new Head();
            [DataMember(Name = "body")]
            public ReceiveMoveBody Body { get; set; } = new ReceiveMoveBody();

            public class ReceiveMoveBody : Body
            {
                [DataMember(Name = "wrkLoc")]
                public Location wrkLoc { get; set; } = new Location();
            }
        }
    }
}

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
        /// If the vehicle is in the right position(CPS 00) under the AYC (Yard Crane) from TOS to EIS
        /// /equipment/ayc-job/sendClearance
        /// OPUS (TOS) -> IoT Platform -> EIS
        /// </summary>
        [DataContract]
        public class ResponseClearance : Base
        {
            public new string Name = "ReceiveClearance";

            [DataMember(Name = "head")]
            public Head Head { get; set; } = new Head();
            [DataMember(Name = "body")]
            public ResponseClearanceBody Body { get; set; } = new ResponseClearanceBody();

            public class ResponseClearanceBody : Body
            {
                [DataMember(Name = "lndTpChassis")]
                public string lndTpChassis { get; set; }
                [DataMember(Name = "clearnaceChassis")]
                public string clearnaceChassis { get; set; }
                [DataMember(Name = "vehicleId")]
                public string vehicleId { get; set; }
                [DataMember(Name = "vehicleLoc")]
                public Location vehicleLoc { get; set; } = new Location();
            }
        }
    }
}

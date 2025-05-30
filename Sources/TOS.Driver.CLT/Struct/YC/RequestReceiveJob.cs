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
        /// Send work order to AYC (Yard Crane) from TOS to EIS
        /// /equipment/ayc-job/sendAycJob
        /// OPUS (TOS) -> IoT Platform -> EIS
        /// </summary>
        [DataContract]
        public class RequestReceiveJob : Base
        {
            public new string Name = "SetJobOrder";

            [DataMember(Name = "head")]
            public Head Head { get; set; } = new Head();
            [DataMember(Name = "body")]
            public ReceiveJobBody Body { get; set; } = new ReceiveJobBody();

            public RequestReceiveJob()
            {
            }

            [DataContract]
            public class ReceiveJobBody : Body
            {
                [DataMember(Name = "jobTp")]
                public string jobTp { get; set; }
                [DataMember(Name = "cntrNo")]
                public string cntrNo { get; set; }
                [DataMember(Name = "cntrLen")]
                public string cntrLen { get; set; }
                [DataMember(Name = "cntrWgt")]
                public string cntrWgt { get; set; }
                [DataMember(Name = "cntrHgt")]
                public string cntrHgt { get; set; }
                [DataMember(Name = "cntrTp")]
                public string cntrTp { get; set; }
                [DataMember(Name = "isFull")]
                public string isFull { get; set; }
                [DataMember(Name = "vehicleId")]
                public string vehicleId { get; set; }
                [DataMember(Name = "chssPos")]
                public string chssPos { get; set; }
                [DataMember(Name = "pickupLoc")]
                public Location pickupLoc { get; set; } = new Location();
                [DataMember(Name = "pickupLndTp")]
                public string pickupLndTp { get; set; }
                [DataMember(Name = "pickupClrn")]
                public string pickupClrn { get; set; }
                [DataMember(Name = "setdownLoc")]
                public Location setdownLoc { get; set; } = new Location();
                [DataMember(Name = "setdownLndTp")]
                public string setdownLndTp { get; set; }
                [DataMember(Name = "setdownClrn")]
                public string setdownClrn { get; set; }
            }
        }
    }
}

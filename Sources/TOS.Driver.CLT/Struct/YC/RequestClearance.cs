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
        /// If transmits vehicle information detected under from AYC (Yard Crane) to TOS
        /// /equipment/ayc-job/detectVehicle
        /// EIS -> IoT Platform -> OPUS (TOS)
        /// </summary>
        [DataContract]
        public class RequestClearance : Base
        {
            public new string Name = "ArrivalVehicle";

            [DataMember(Name = "head")]
            public Head Head { get; set; } = new Head();
            [DataMember(Name = "body")]
            public RequestClearanceBody Body { get; set; } = new RequestClearanceBody();

            public class RequestClearanceBody : Body
            {
                [DataMember(Name = "vehicleId")]
                public string vehicleId { get; set; }
                [DataMember(Name = "vehicleLoc")]
                public Location vehicleLoc { get; set; } = new Location();
            }
        }
    }
}

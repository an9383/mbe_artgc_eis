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
        /// Send the 1st part (Pick Up) Job Started from AYC (Yard Crane) from EIS to TOS
        /// /equipment/ayc-job/pickupStart
        /// EIS -> IoT Platform -> OPUS (TOS) 
        /// </summary>
        [DataContract]
        public class PickupJobStart : Base
        {
            public new string Name = "PickUpStart";

            [DataMember(Name = "head")]
            public Head Head { get; set; } = new Head();
            [DataMember(Name = "body")]
            public Body Body { get; set; } = new Body();
        }
    }
}

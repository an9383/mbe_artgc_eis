using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TOS.Driver.CLT.Struct.Common;

namespace TOS.Driver.CLT.Struct.YC
{
    public partial class YCMethod
    {
        /// <summary>
        /// Send the 1st part (Pick Up) Job End from AYC (Yard Crane) from EIS to TOS
        /// /equipment/ayc-job/pickupDone
        /// EIS -> IoT Platform -> OPUS (TOS) 
        /// </summary>
        [DataContract]
        public class PickupJobEnd : Base
        {
            public new string Name = "PickUpDone";

            [DataMember(Name = "head")]
            public Head Head { get; set; } = new Head();
            [DataMember(Name = "body")]
            public JobEndBody Body { get; set; } = new JobEndBody();
        }
    }
}

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
        /// Send the 2st part (Set Down) Job Doned from AYC (Yard Crane) from EIS to TOS
        /// /equipment/ayc-job/setdownStart
        /// EIS -> IoT Platform -> OPUS (TOS) 
        /// </summary>
        [DataContract]
        public class SetdownJobStart : Base
        {
            public new string Name = "SetDownStart";

            [DataMember(Name = "head")]
            public Head Head { get; set; } = new Head();
            [DataMember(Name = "body")]
            public Body Body { get; set; } = new Body();
        }
    }
}

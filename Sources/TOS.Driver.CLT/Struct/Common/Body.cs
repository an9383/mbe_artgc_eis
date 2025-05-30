using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TOS.Driver.CLT.Struct.Common
{
    /// <summary>
    /// CLT IoT Platform Common Body 
    /// </summary>
    [DataContract]
    public class Body
    {
        [DataMember(Name = "eqId")]
        public string eqId { get; set; }
        [DataMember(Name = "jobId")]
        public string jobId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TOS.Driver.CLT.Struct.Common
{
    /// <summary>
    /// CLT IoT Platform Common JobEnd Body 
    /// </summary>
    [DataContract]
    public class JobEndBody : Body
    {
        [DataMember(Name = "cntrWgt")]
        public string cntrWgt { get; set; }
        [DataMember(Name = "wrkLoc")]
        public Location wrkLoc { get; set; } = new Location();
    }
}

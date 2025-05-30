using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TOS.Driver.CLT.Struct.Common
{
    /// <summary>
    /// CLT IoT Platform Common Header 
    /// </summary>
    [DataContract]
    public class Head
    {
        [DataMember(Name = "msgId")]
        public string msgId { get; set; }
        [DataMember(Name = "evntTp")]
        public string evntTp { get; set; }
        [DataMember(Name = "timeStemp")]
        public string timeStemp { get; set; }
        [DataMember(Name = "remark")]
        public string remark { get; set; }

        public Head()
        {
            msgId = Guid.NewGuid().ToString();
            timeStemp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss.fff");
        }
    }
}

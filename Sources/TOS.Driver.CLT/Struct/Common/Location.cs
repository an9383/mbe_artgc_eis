using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TOS.Driver.CLT.Struct.Common
{
    /// <summary>
    /// CLT IoT Platform Common Location 
    /// </summary>
    [DataContract]
    public class Location
    {
        [DataMember(Name = "LocTp")]
        public string LocTp { get; set; }
        [DataMember(Name = "loc1")]
        public string loc1 { get; set; }
        [DataMember(Name = "loc2")]
        public string loc2 { get; set; }
        [DataMember(Name = "loc3")]
        public string loc3 { get; set; }
        [DataMember(Name = "loc4")]
        public string loc4 { get; set; }
    }
}

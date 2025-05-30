using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOS.Driver.CLT.Struct.Common;

namespace TOS.Driver.CLT.Struct.YC
{
    public partial class YCMethod
    {
        /// <summary>
        /// Receive the Job Accepted from AYC (Yard Crane) from EIS to TOS
        /// /equipment/ayc-job/accept
        /// EIS -> IoT Platform -> OPUS (TOS) 
        /// </summary>
        public class ResponseAcceptJob : Base
        {
            public new string Name = "accept";

            public Head Head { get; set; } = new Head();
            public Body Body { get; set; } = new Body();

            public ResponseAcceptJob()
            {
                Head.evntTp = "AcceptReport";
            }

            public ResponseAcceptJob(string eqId, string jobId) : base()
            {
                Body.eqId = eqId;
                Body.jobId = jobId;
            }
        }
    }
}

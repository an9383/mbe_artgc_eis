using JsonServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOS.Driver.CLT.Struct.YC;
using static TOS.Driver.CLT.Struct.YC.YCMethod;

namespace TOS.Driver.CLT.Service
{
    public class StubMessageTypeProvider : MessageTypeProvider
    {
        public StubMessageTypeProvider()
        {
            Register("sendAycJob", typeof(RequestReceiveJobList));
            /*Register(typeof(DelayRequest).FullName, typeof(DelayRequest));
            Register(typeof(GetVersion).FullName, typeof(GetVersion));
            Register(typeof(EventBroadcaster).FullName, typeof(EventBroadcaster));*/
        }
    }
}

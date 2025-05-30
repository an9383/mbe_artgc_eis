using System;
using System.Collections.Generic;
using System.Text;
using MBE.Driver.Common.Channels;

namespace MBE.Driver.Common.Logging
{
    public class ChannelRequestLog : ChannelMessageLog
    {
        public ChannelRequestLog(IChannel channel, IRequest request, byte[] rawMessage) : base(channel, request, rawMessage)
        {
            Request = request;
        }

        public IRequest Request { get; }

        public override string ToString() => $"Request: {base.ToString()}";
    }
}

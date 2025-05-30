using System;
using System.Collections.Generic;
using System.Text;
using MBE.Driver.Common.Channels;

namespace MBE.Driver.Common.Logging
{
    public class ChannelResponseLog : ChannelMessageLog
    {
        public ChannelResponseLog(IChannel channel, IResponse response, byte[] rawMessage, ChannelRequestLog requestLog) : base(channel, response, rawMessage)
        {
            Response = response;
            RequestLog = requestLog;
        }

        public ChannelRequestLog RequestLog { get; }

        public IResponse Response { get; }

        public override string ToString() => $"Response: {base.ToString()}";
    }
}

using System;
using MBE.Driver.Common.Channels;

namespace MBE.Driver.Common.Logging
{
    public class ChannelErrorLog : ChannelLog
    {
        public ChannelErrorLog(IChannel channel, Exception exception) : base(channel)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

        public override string ToString() => $"Comm Error: {Exception?.Message ?? Exception.ToString()}";
    }
}

using System;
using MBE.Driver.Common.Channels;

namespace MBE.Driver.Common.Logging
{
    public abstract class ChannelLog
    {
        protected ChannelLog(IChannel channel)
        {
            TimeStamp = DateTime.Now;
            ChannelDescription = channel?.Description;
        }

        public DateTime TimeStamp { get; }

        public string ChannelDescription { get; }
    }
}

using MBE.Driver.Common.Channels;

namespace MBE.Driver.Common.Logging
{
    public class ChannelOpenEventLog : ChannelLog
    {
        public ChannelOpenEventLog(IChannel channel) : base(channel) { }

        public override string ToString() => "Opened Channel";
    }
}

using MBE.Driver.Common.Channels;

namespace MBE.Driver.Common.Logging
{
    public class ChannelCloseEventLog : ChannelLog
    {
        public ChannelCloseEventLog(IChannel channel) : base(channel) { }

        public override string ToString() => "Closed Channel";
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MBE.Driver.Common.Channels;

namespace MBE.Driver.Common.Logging
{
    public abstract class ChannelMessageLog : ChannelLog
    {
        protected ChannelMessageLog(IChannel channel, IProtocolMessage message, byte[] rawMessage) : base(channel)
        {
            Message = message;
            RawMessage = rawMessage ?? new byte[0];
        }

        public IProtocolMessage Message { get; }

        public IReadOnlyList<byte> RawMessage { get; }
        public override string ToString() => BitConverter.ToString(RawMessage as byte[]).Replace('-', ' ');
    }
}

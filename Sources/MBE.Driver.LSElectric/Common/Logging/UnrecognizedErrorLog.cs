using System;
using System.Collections.Generic;
using MBE.Driver.Common.Channels;

namespace MBE.Driver.Common.Logging
{
    public class UnrecognizedErrorLog : ChannelLog
    {
        public UnrecognizedErrorLog(IChannel channel, byte[] rawMessage) : base(channel)
        {
            RawMessage = rawMessage ?? new byte[0];
        }

        public IReadOnlyList<byte> RawMessage { get; }

        public override string ToString()
            => RawMessage != null && RawMessage.Count > 0 ? $"Error Message: {BitConverter.ToString(RawMessage as byte[]).Replace('-', ' ')}" : base.ToString();
    }
}

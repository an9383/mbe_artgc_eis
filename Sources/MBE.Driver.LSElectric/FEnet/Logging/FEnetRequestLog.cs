using System;
using System.Text;
using MBE.Driver.Common.Channels;
using MBE.Driver.Common.Logging;
using MBE.Driver.LSElectric.FEnet;

namespace MBE.Driver.LSElectric.FeNet.Logging
{
    public class FEnetRequestLog : ChannelRequestLog
    {
        public FEnetRequestLog(IChannel channel, FEnetRequest request, byte[] rawMessage) : base(channel, request, rawMessage)
        {
            FEnetRequest = request;
        }

        public FEnetRequest FEnetRequest { get; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder("REQ: ");
            stringBuilder.Append('"');
            stringBuilder.Append(Encoding.ASCII.GetString(RawMessage as byte[], 0, 10).Replace("\0", "\\0"));
            stringBuilder.Append('"');
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 10, 2).Replace("-", ""));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 12, 1));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 13, 1));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 14, 2).Replace("-", ""));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 16, 2).Replace("-", ""));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 18, 1));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 19, 1));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 20, 2).Replace("-", ""));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 22, 2).Replace("-", ""));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 24, 2).Replace("-", ""));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 26, 2).Replace("-", ""));
            stringBuilder.Append(' ');
            stringBuilder.Append(BitConverter.ToString(RawMessage as byte[], 28).Replace("-", ""));

            return stringBuilder.ToString();
        }
    }
}

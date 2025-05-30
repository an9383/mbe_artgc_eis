using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using MBE.Driver.Common.Channels;
using MBE.Driver.LSElectric.FEnet;

namespace MBE.Driver.LSElectric.FeNet.Logging
{
    public class FEnetNAKLog : FEnetResponseLog
    {
        public FEnetNAKLog(IChannel channel, FEnetNAKResponse message, byte[] rawMessage, FEnetRequestLog requestLog) : base(channel, message, rawMessage, requestLog)
        {
            NAKCode = message.NAKCode;
        }

        public FEnetNAKCode NAKCode { get; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder("NAK: ");
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
            stringBuilder.Append(' ');
            var codeName = NAKCode.ToString();
            stringBuilder.Append($"Error: {(typeof(FEnetNAKCode).GetMember(codeName, BindingFlags.Static | BindingFlags.Public)?.FirstOrDefault()?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? codeName}");

            return stringBuilder.ToString();
        }
    }
}

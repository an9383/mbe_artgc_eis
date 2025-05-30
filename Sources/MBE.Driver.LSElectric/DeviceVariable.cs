using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MBE.Driver.LSElectric
{
    public struct DeviceVariable
    {
        public DeviceVariable(DeviceType deviceType, DataType dataType, uint index, params byte[] subIndices)
        {
            DeviceType = deviceType;
            DataType = dataType;
            Index = index;
            SubIndices = subIndices.ToArray();
        }

        public DeviceType DeviceType { get; }

        public DataType DataType { get; }

        public uint Index { get; }

        public IReadOnlyList<byte> SubIndices { get; }

        public override string ToString() => ToString(false);


        public string ToString(bool useHexBitIndex)
        {
            StringBuilder stringBuilder;

            if (DataType != DataType.Bit || !useHexBitIndex)
            {
                if (DeviceType == DeviceType.U && DataType == DataType.Bit)
                {
                    stringBuilder = new StringBuilder($"%{(char)DeviceType}{(char)DataType}{Index:X}");

                    foreach (var subIndex in SubIndices)
                    {
                        stringBuilder.Append('.');
                        stringBuilder.Append(subIndex.ToString("X"));
                    }
                    return stringBuilder.ToString();
                }
                else
                {
                    stringBuilder = new StringBuilder($"%{(char)DeviceType}{(char)DataType}{Index}");
                    foreach (var subIndex in SubIndices)
                    {
                        stringBuilder.Append('.');
                        stringBuilder.Append(subIndex);
                    }
                    return stringBuilder.ToString();
                }
            }
            else
            {
                //P, M, L, K, F 이면서 Bit일 경우 16진수
                //그 외에는 인덱스가 .으로 나누어져있고 Bit일 경우 마지막 자리만 16진수
                switch (DeviceType)
                {
                    case DeviceType.P:
                    case DeviceType.M:
                    case DeviceType.L:
                    case DeviceType.K:
                    case DeviceType.F:
                        return $"%{(char)DeviceType}{(char)DataType}{Index:X}";
                    case DeviceType.U:
                        stringBuilder = new StringBuilder($"%{(char)DeviceType}{(char)DataType}{Index:X}");
                        foreach (var subIndex in SubIndices)
                        {
                            stringBuilder.Append('.');
                            stringBuilder.Append(subIndex.ToString("X"));
                        }
                        return stringBuilder.ToString();
                    default:
                        stringBuilder = new StringBuilder($"%{(char)DeviceType}{(char)DataType}{Index}");
                        for (int i = 0; i < SubIndices.Count; i++)
                        {
                            stringBuilder.Append('.');
                            stringBuilder.Append(i == SubIndices.Count - 1 ? SubIndices[i].ToString("X") : SubIndices[i].ToString());
                        }
                        return stringBuilder.ToString();
                }
            }
        }

        public byte[] ToBytes() => ToBytes(false);

        public byte[] ToBytes(bool useHexBitIndex) => Encoding.ASCII.GetBytes(ToString(useHexBitIndex));


        public static DeviceVariable Parse(string text) => Parse(text, false);

        public static DeviceVariable Parse(string text, bool useHexBitIndex)
        {
            var exception = TryParseCore(text, useHexBitIndex, out DeviceVariable result);
            if (exception != null)
                throw exception;
            return result;
        }

        public static bool TryParse(string text, out DeviceVariable deviceVariable) => TryParseCore(text, false, out deviceVariable) == null;

        public static bool TryParse(string text, bool useHexBitIndex, out DeviceVariable deviceVariable) => TryParseCore(text, useHexBitIndex, out deviceVariable) == null;

        private static Exception TryParseCore(string text, bool useHexBitIndex, out DeviceVariable deviceVariable)
        {
            if (text == null)
            {
                deviceVariable = new DeviceVariable();
                return new ArgumentNullException(nameof(text));
            }
            else if (text.Length < 4
                || text[0] != '%'
                || !Enum.IsDefined(typeof(DeviceType), (byte)text[1])
                || !Enum.IsDefined(typeof(DataType), (byte)text[2]))
            {
                deviceVariable = new DeviceVariable();
                return new FormatException();
            }
            else
            {
                DeviceType deviceType = (DeviceType)(byte)text[1];
                DataType dataType = (DataType)(byte)text[2];

                var indexTexts = text.Remove(0, 3).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                List<uint> indices = new List<uint>();

                for (int i = 0; i < indexTexts.Length; i++)
                {
                    var indexText = indexTexts[i];
                    uint index = 0;
                    
                    switch (deviceType)
                    {
                        case DeviceType.P:
                        case DeviceType.M:
                        case DeviceType.L:
                        case DeviceType.K:
                        case DeviceType.F:
                            if (!useHexBitIndex && indexText.Any(c => !char.IsDigit(c))
                                || !uint.TryParse(indexText, useHexBitIndex && dataType == DataType.Bit ? NumberStyles.HexNumber : NumberStyles.Number, null, out index))
                            {
                                deviceVariable = new DeviceVariable();
                                return new FormatException();
                            }
                            indices.Add(index);
                            break;
                        case DeviceType.U:
                            if (!uint.TryParse(indexText, NumberStyles.HexNumber, null, out index))
                            {
                                deviceVariable = new DeviceVariable();
                                return new FormatException();
                            }
                            indices.Add(index);
                            break;
                        default:
                            if (!useHexBitIndex && indexText.Any(c => !char.IsDigit(c))
                                || !uint.TryParse(indexText, i == indexTexts.Length - 1 && useHexBitIndex && dataType == DataType.Bit ? NumberStyles.HexNumber : NumberStyles.Number, null, out index))
                            {
                                deviceVariable = new DeviceVariable();
                                return new FormatException();
                            }
                            indices.Add(index);
                            break;
                    }
                }

                if (indices.Count == 1)
                    deviceVariable = new DeviceVariable(deviceType, dataType, indices[0]);
                else if (indices.Count > 1)
                    deviceVariable = new DeviceVariable(deviceType, dataType, indices[0], indices.Skip(1).Select(i => (byte)i).ToArray());
                else
                {
                    deviceVariable = new DeviceVariable();
                    return new FormatException();
                }
                return null;
            }
        }

        public static implicit operator DeviceVariable(string text) => Parse(text);

        public static bool operator ==(DeviceVariable variable1, DeviceVariable variable2) => variable1.Equals(variable2);

        public static bool operator !=(DeviceVariable variable1, DeviceVariable variable2) => !variable1.Equals(variable2);

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        internal DeviceVariable Increase()
        {
            if (SubIndices.Count == 0)
            {
                return new DeviceVariable(DeviceType, DataType, Index + 1);
            }
            else
            {
                var subAddresses = SubIndices.ToArray();
                subAddresses[subAddresses.Length - 1] += 1;
                return new DeviceVariable(DeviceType, DataType, Index, subAddresses);
            }
        }
    }
}

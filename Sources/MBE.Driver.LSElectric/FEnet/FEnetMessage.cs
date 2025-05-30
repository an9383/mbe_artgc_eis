using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBE.Driver.Common;
using MBE.Driver.Common.Channels;

namespace MBE.Driver.LSElectric.FEnet
{
    public abstract class FEnetMessage : IProtocolMessage
    {
        internal static readonly byte[] zero = new byte[] { 0, 0 };
        internal static readonly byte[] one = new byte[] { 1, 0 };

        internal FEnetMessage(FEnetCommand command, FEnetDataType dataType)
        {
            Command = command;
            DataType = dataType;
        }

        private ushort? invokeID;

        private byte[] frameData;

        public ushort? InvokeID { get => invokeID; set => SetProperty(ref invokeID, value); }

        public abstract byte SourceOfFrame { get; }

        public FEnetCommand Command { get; }

        public FEnetDataType DataType { get; }

        protected void InvalidateFrameData()
        {
            lock (this)
            {
                frameData = null;
            }
        }

        protected bool SetProperty<TProperty>(ref TProperty target, TProperty value)
        {
            lock (this)
            {
                if (!EqualityComparer<TProperty>.Default.Equals(target, value))
                {
                    target = value;
                    frameData = null;
                    return true;
                }
                return false;
            }
        }

        internal static IEnumerable<byte> WordToLittleEndianBytes(ushort value) => BitConverter.IsLittleEndian ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse();
        internal static IEnumerable<byte> ValueToLittleEndianBytes(short value) => BitConverter.IsLittleEndian ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse();
        internal static IEnumerable<byte> ValueToLittleEndianBytes(int value) => BitConverter.IsLittleEndian ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse();
        internal static IEnumerable<byte> ValueToLittleEndianBytes(long value) => BitConverter.IsLittleEndian ? BitConverter.GetBytes(value) : BitConverter.GetBytes(value).Reverse();
        internal static ushort ReadWord(Channel channel, List<byte> buffer)
        {
            var result = channel.Read(2, 0).ToArray();
            buffer.AddRange(result);
            return BitConverter.IsLittleEndian ? BitConverter.ToUInt16(result, 0) : BitConverter.ToUInt16(result.Reverse().ToArray(), 0);
        }

        internal static FEnetDataType ToFEnetDataType(DataType dataType)
        {
            switch (dataType)
            {
                case LSElectric.DataType.Bit:
                    return FEnetDataType.Bit;
                case LSElectric.DataType.Byte:
                    return FEnetDataType.Byte;
                case LSElectric.DataType.Word:
                    return FEnetDataType.Word;
                case LSElectric.DataType.DoubleWord:
                    return FEnetDataType.DoubleWord;
                case LSElectric.DataType.LongWord:
                    return FEnetDataType.LongWord;
                default:
                    return FEnetDataType.Continuous;
            }
        }

        internal static DataType ToDataType(FEnetDataType dataType)
        {
            switch (dataType)
            {
                case FEnetDataType.Bit:
                    return LSElectric.DataType.Bit;
                case FEnetDataType.Byte:
                    return LSElectric.DataType.Byte;
                case FEnetDataType.Word:
                    return LSElectric.DataType.Word;
                case FEnetDataType.DoubleWord:
                    return LSElectric.DataType.DoubleWord;
                case FEnetDataType.LongWord:
                    return LSElectric.DataType.LongWord;
                default:
                    return LSElectric.DataType.Unknown;
            }
        }

        /// <summary>
        /// 데이터 프레임 생성
        /// </summary>
        /// <returns>데이터의 직렬화 된 바이트 열거</returns>
        protected virtual IEnumerable<byte> OnCreateDataFrame() { yield break; }

        /// <summary>
        /// 직렬화
        /// </summary>
        /// <param name="companyID">Company ID, 메시지의 헤더. LSIS-XGT나 LGIS-GLOFA를 사용함.</param>
        /// <param name="useChecksum">Application Header에 체크섬 사용 여부</param>
        /// <returns>직렬화 된 바이트 열거</returns>
        public virtual IEnumerable<byte> Serialize(string companyID, bool useChecksum)
        {
            lock (this)
            {
                if (frameData == null)
                {
                    var dataFrame = OnCreateDataFrame().ToArray();

                    List<byte> byteList = new List<byte>(Encoding.ASCII.GetBytes(companyID).Take(10));
                    if (byteList.Count < 10)
                        byteList.AddRange(Enumerable.Repeat((byte)0, 10 - byteList.Count));
                    byteList.Add(0);
                    byteList.Add(0);
                    byteList.Add(0);
                    byteList.Add(SourceOfFrame);
                    byteList.AddRange(WordToLittleEndianBytes(invokeID ?? 0));
                    byteList.AddRange(WordToLittleEndianBytes((ushort)dataFrame.Length));
                    byteList.Add(0);
                    byteList.Add((byte)(useChecksum ? byteList.Sum(b => b) % 256 : 0x00));
                    byteList.AddRange(dataFrame);
                    frameData = byteList.ToArray();
                }
                return frameData;
            }
        }
    }
}

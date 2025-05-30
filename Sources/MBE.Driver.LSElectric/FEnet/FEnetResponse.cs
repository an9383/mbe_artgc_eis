using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MBE.Driver.Common;

namespace MBE.Driver.LSElectric.FEnet
{
    public abstract class FEnetResponse : FEnetMessage, IResponse
    {
        internal FEnetResponse(FEnetRequest request, ushort plcInfo, byte ethernetModuleInfo) : base(request.Command, request.DataType)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            InvokeID = request.InvokeID;
            PlcInfo = plcInfo;
            EthernetModuleSlot = (byte)(ethernetModuleInfo & 0xF);
            EthernetModuleBase = (byte)(ethernetModuleInfo >> 4);
        }

        public FEnetRequest Request { get; private set; }

        public override byte SourceOfFrame => 0x11;

        public ushort PlcInfo { get; }

        public byte EthernetModuleSlot { get; }

        public byte EthernetModuleBase { get; }


        protected override IEnumerable<byte> OnCreateDataFrame()
            => WordToLittleEndianBytes((ushort)((int)Command + 1)).Concat(WordToLittleEndianBytes((ushort)DataType)).Concat(zero);
    }

    public abstract class FEnetACKResponse : FEnetResponse
    {
        internal FEnetACKResponse(FEnetRequest request, ushort plcInfo, byte ethernetModuleInfo) : base(request, plcInfo, ethernetModuleInfo) { }
    }

    public class FEnetReadIndividualResponse : FEnetACKResponse, IReadOnlyDictionary<DeviceVariable, DeviceValue>
    {
        internal FEnetReadIndividualResponse(IEnumerable<DeviceValue> deviceValues, FEnetReadIndividualRequest request, ushort plcInfo, byte ethernetModuleInfo) : base(request, plcInfo, ethernetModuleInfo)
        {
            SetData(deviceValues, request);
        }

        private void SetData(IEnumerable<DeviceValue> deviceValues, IEnumerable<DeviceVariable> deviceVariables)
        {
            var valueArray = deviceValues.ToArray();
            var deviceVariableArray = deviceVariables.ToArray();

            if (valueArray.Length != deviceVariableArray.Length) throw new ArgumentOutOfRangeException(nameof(deviceValues));

            deviceValueList = valueArray.Zip(deviceVariableArray, (v, a) => new KeyValuePair<DeviceVariable, DeviceValue>(a, v)).ToList();

            foreach (var item in deviceValueList)
                deviceValueDictionary[item.Key] = item.Value;
        }


        internal List<KeyValuePair<DeviceVariable, DeviceValue>> deviceValueList;
        private readonly Dictionary<DeviceVariable, DeviceValue> deviceValueDictionary = new Dictionary<DeviceVariable, DeviceValue>();

        public IEnumerable<DeviceVariable> Keys => deviceValueDictionary.Keys;

        public IEnumerable<DeviceValue> Values => deviceValueDictionary.Values;

        public int Count => deviceValueDictionary.Count;

        public DeviceValue this[DeviceVariable deviceVariable] => deviceValueDictionary[deviceVariable];

        public bool ContainsKey(DeviceVariable deviceVariable) => deviceValueDictionary.ContainsKey(deviceVariable);

        public bool TryGetValue(DeviceVariable deviceVariable, out DeviceValue deviceValue) => deviceValueDictionary.TryGetValue(deviceVariable, out deviceValue);

        public IEnumerator<KeyValuePair<DeviceVariable, DeviceValue>> GetEnumerator() => deviceValueList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



        protected override IEnumerable<byte> OnCreateDataFrame()
        {
            foreach (var b in base.OnCreateDataFrame()) yield return b;
            yield return 0x00;  //에러 상태
            yield return 0x00;  //없음

            foreach (var b in WordToLittleEndianBytes((ushort)deviceValueList.Count)) yield return b;

            foreach (var item in deviceValueList)
            {
                switch (item.Key.DataType)
                {
                    case LSElectric.DataType.Bit:
                        foreach (var b in one) yield return b;
                        yield return (byte)(item.Value.BitValue ? 1 : 0);
                        break;
                    case LSElectric.DataType.Byte:
                        foreach (var b in one) yield return b;
                        yield return item.Value.ByteValue;
                        break;
                    case LSElectric.DataType.Word:
                        foreach (var b in WordToLittleEndianBytes(2)) yield return b;
                        foreach (var b in ValueToLittleEndianBytes(item.Value.WordValue)) yield return b;
                        break;
                    case LSElectric.DataType.DoubleWord:
                        foreach (var b in WordToLittleEndianBytes(4)) yield return b;
                        foreach (var b in ValueToLittleEndianBytes(item.Value.DoubleWordValue)) yield return b;
                        break;
                    case LSElectric.DataType.LongWord:
                        foreach (var b in WordToLittleEndianBytes(8)) yield return b;
                        foreach (var b in ValueToLittleEndianBytes(item.Value.LongWordValue)) yield return b;
                        break;
                }
            }
        }
    }

    public class FEnetReadContinuousResponse : FEnetACKResponse, IDeviceDataBlock
    {
        internal FEnetReadContinuousResponse(IEnumerable<byte> bytes, FEnetReadContinuousRequest request, ushort plcInfo, byte ethernetModuleInfo) : base(request, plcInfo, ethernetModuleInfo)
        {
            StartDeviceVariable = (Request as FEnetReadContinuousRequest).StartDeviceVariable;
            DeviceValueCount = (Request as FEnetReadContinuousRequest).Count;

            var startDeviceVariable = StartDeviceVariable;
            switch (startDeviceVariable.DataType)
            {
                case LSElectric.DataType.Bit:
                    startByteIndex = startDeviceVariable.Index / 8;
                    startBitIndex = startDeviceVariable.Index;
                    bitLength = (uint)DeviceValueCount;
                    break;
                case LSElectric.DataType.Byte:
                    startByteIndex = startDeviceVariable.Index;
                    startBitIndex = startDeviceVariable.Index * 8;
                    bitLength = (uint)DeviceValueCount * 8;
                    break;
                case LSElectric.DataType.Word:
                    startByteIndex = startDeviceVariable.Index * 2;
                    startBitIndex = startDeviceVariable.Index * 16;
                    bitLength = (uint)DeviceValueCount * 16;
                    break;
                case LSElectric.DataType.DoubleWord:
                    startByteIndex = startDeviceVariable.Index * 4;
                    startBitIndex = startDeviceVariable.Index * 32;
                    bitLength = (uint)DeviceValueCount * 32;
                    break;
                case LSElectric.DataType.LongWord:
                    startByteIndex = startDeviceVariable.Index * 8;
                    startBitIndex = startDeviceVariable.Index * 64;
                    bitLength = (uint)DeviceValueCount * 64;
                    break;
            }

            this.bytes = bytes.ToArray();
        }

        private readonly byte[] bytes;
        private readonly uint startByteIndex;
        private readonly uint startBitIndex;
        private readonly uint bitLength;

        public int Count => bytes.Length;

        public byte this[int index] => bytes[index];

        public IEnumerator<byte> GetEnumerator() => bytes.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected override IEnumerable<byte> OnCreateDataFrame()
        {
            foreach (var b in base.OnCreateDataFrame()) yield return b;
            yield return 0x00;  //에러 상태
            yield return 0x00;  //없음

            foreach (var b in one) yield return b;

            foreach (var b in WordToLittleEndianBytes((ushort)bytes.Length)) yield return b;
            foreach (var b in bytes)
                yield return b;
        }

        public DeviceVariable StartDeviceVariable { get; }

        public int DeviceValueCount { get; }

        public DeviceValue this[DataType dataType, uint index]
        {
            get
            {
                switch (dataType)
                {
                    case LSElectric.DataType.Bit:
                        if (startBitIndex > index || index >= startBitIndex + bitLength)
                            throw new ArgumentOutOfRangeException(nameof(index));
                        var bitIndex = index - startBitIndex;
                        return ((bytes[bitIndex / 8] >> (int)(bitIndex % 8)) & 1) == 1;
                    case LSElectric.DataType.Byte:
                        uint byteIndex = index - startByteIndex;
                        if (byteIndex >= bytes.Length)
                            throw new ArgumentOutOfRangeException(nameof(index));
                        return bytes[index - startByteIndex];
                    case LSElectric.DataType.Word:
                        byteIndex = index * 2 - startByteIndex;
                        if (byteIndex + 2 > bytes.Length)
                            throw new ArgumentOutOfRangeException(nameof(index));
                        return BitConverter.ToInt16(bytes, (int)byteIndex);
                    case LSElectric.DataType.DoubleWord:
                        byteIndex = index * 4 - startByteIndex;
                        if (byteIndex + 4 > bytes.Length)
                            throw new ArgumentOutOfRangeException(nameof(index));
                        return BitConverter.ToInt32(bytes, (int)byteIndex);
                    case LSElectric.DataType.LongWord:
                        byteIndex = index * 8 - startByteIndex;
                        if (byteIndex + 8 > bytes.Length)
                            throw new ArgumentOutOfRangeException(nameof(index));
                        return BitConverter.ToInt64(bytes, (int)byteIndex);
                    default:
                        throw new ArgumentException(nameof(dataType));
                }
            }
        }
    }

    public class FEnetWriteResponse : FEnetACKResponse
    {
        internal FEnetWriteResponse(FEnetWriteRequest request, ushort plcInfo, byte ethernetModuleInfo) : base(request, plcInfo, ethernetModuleInfo) { }

        protected override IEnumerable<byte> OnCreateDataFrame()
        {
            foreach (var b in base.OnCreateDataFrame()) yield return b;
            yield return 0x00;
            yield return 0x00;
            foreach (var b in WordToLittleEndianBytes(Request.BlockCount)) yield return b;
        }
    }

    public class FEnetNAKResponse : FEnetResponse
    {
        internal FEnetNAKResponse(ushort nakCode, FEnetRequest request, ushort plcInfo, byte ethernetModuleInfo) : base(request, plcInfo, ethernetModuleInfo)
        {
            NAKCodeValue = nakCode;
            if (Enum.IsDefined(typeof(FEnetNAKCode), nakCode))
                NAKCode = (FEnetNAKCode)nakCode;
        }

        internal FEnetNAKResponse(FEnetNAKCode nakCode, FEnetRequest request, ushort plcInfo, byte ethernetModuleInfo) : base(request, plcInfo, ethernetModuleInfo)
        {
            NAKCode = nakCode;
            NAKCodeValue = (ushort)nakCode;
        }

        internal FEnetNAKResponse(FEnetNAKCode nakCode, FEnetCommand command, FEnetDataType dataType, ushort plcInfo, byte ethernetModuleInfo)
            : base(new FEnetRequestError(command, dataType), plcInfo, ethernetModuleInfo)
        {
            NAKCode = nakCode;
            NAKCodeValue = (ushort)nakCode;
        }

        public FEnetNAKCode NAKCode { get; } = FEnetNAKCode.Unknown;

        public ushort NAKCodeValue { get; }

        protected override IEnumerable<byte> OnCreateDataFrame()
        {
            foreach (var b in base.OnCreateDataFrame()) yield return b;
            yield return 0xff;
            yield return 0xff;
            foreach (var b in WordToLittleEndianBytes(NAKCodeValue)) yield return b;
        }

        class FEnetRequestError : FEnetRequest
        {
            public FEnetRequestError(FEnetCommand command, FEnetDataType dataType) : base(command, dataType) { }

            public override ushort BlockCount => 0;

            public override object Clone() => new FEnetRequestError(Command, DataType);
        }
    }

}

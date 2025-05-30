using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MBE.Driver.Common;

namespace MBE.Driver.LSElectric.FEnet
{
    public abstract class FEnetRequest : FEnetMessage, IRequest<FEnetCommErrorCode>, ICloneable
    {
        protected FEnetRequest(FEnetCommand command, FEnetDataType dataType) : base(command, dataType) { }

        private bool? useHexBitIndex;

        public override byte SourceOfFrame => 0x33;

        public abstract ushort BlockCount { get; }

        /// <summary>
        /// 비트 변수의 인덱스를 16진수로 통신할지 여부를 결정.
        /// P, M, L, K, F 이면서 Bit일 경우 16진수로 전송.
        /// 그 외에는 인덱스가 .으로 나누어져있고 Bit일 경우 마지막 자리만 16진수로 전송.
        /// 이 속성을 null로 설정하면 FEnetClient의 UseHexBitIndex 값을 따름.
        /// XGB PLC에서 비트를 읽거나 쓸 때 엉뚱한 비트가 읽히거나 쓰인다면 true로 설정해서 테스트 필요.
        /// </summary>
        public bool? UseHexBitIndex { get => useHexBitIndex; set => SetProperty(ref useHexBitIndex, value); }

        public abstract object Clone();

        protected override IEnumerable<byte> OnCreateDataFrame()
            => WordToLittleEndianBytes((ushort)Command).Concat(WordToLittleEndianBytes((ushort)DataType)).Concat(zero).Concat(WordToLittleEndianBytes(BlockCount));
    }

    public abstract class FEnetReadRequest : FEnetRequest
    {
        protected FEnetReadRequest(FEnetDataType dataType) : base(FEnetCommand.Read, dataType) { }
    }

    public class FEnetReadIndividualRequest : FEnetReadRequest, IList<DeviceVariable>
    {
        public FEnetReadIndividualRequest(DataType dataType) : this(dataType, null as IEnumerable<DeviceVariable>) { }

        public FEnetReadIndividualRequest(DataType dataType, IEnumerable<DeviceVariable> deviceVariables) : base(ToFEnetDataType(dataType))
        {
            if (deviceVariables == null)
                this.deviceVariables = new List<DeviceVariable>();
            else
                this.deviceVariables = new List<DeviceVariable>(deviceVariables);
        }

        public FEnetReadIndividualRequest(DataType dataType, DeviceVariable deviceVariable, params DeviceVariable[] moreDeviceVariables)
            : this(dataType, new DeviceVariable[] { deviceVariable }.Concat(moreDeviceVariables)) { }

        public FEnetReadIndividualRequest(DeviceVariable deviceVariable, params DeviceVariable[] moreDeviceVariables)
            : this(deviceVariable.DataType, new DeviceVariable[] { deviceVariable }.Concat(moreDeviceVariables)) { }

        public override ushort BlockCount => (ushort)deviceVariables.Count;

        public override object Clone() => new FEnetReadIndividualRequest(ToDataType(DataType), deviceVariables) { InvokeID = InvokeID, UseHexBitIndex = UseHexBitIndex };

        private readonly List<DeviceVariable> deviceVariables;

        public int Count => deviceVariables.Count;

        public bool IsReadOnly => ((ICollection<DeviceVariable>)deviceVariables).IsReadOnly;

        public bool Contains(DeviceVariable deviceVariable) => deviceVariables.Contains(deviceVariable);

        public int IndexOf(DeviceVariable deviceVariable) => deviceVariables.IndexOf(deviceVariable);

        public IEnumerator<DeviceVariable> GetEnumerator() => deviceVariables.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(DeviceVariable[] array, int arrayIndex) => deviceVariables.CopyTo(array, arrayIndex);

        public void Clear()
        {
            deviceVariables.Clear();
            InvalidateFrameData();
        }

        public DeviceVariable this[int index]
        {
            get => deviceVariables[index];
            set
            {
                deviceVariables[index] = value;
                InvalidateFrameData();
            }
        }

        public void Add(DeviceVariable deviceVariable)
        {
            deviceVariables.Add(deviceVariable);
            InvalidateFrameData();
        }

        public void Insert(int index, DeviceVariable deviceVariable)
        {
            deviceVariables.Insert(index, deviceVariable);
            InvalidateFrameData();
        }

        public bool Remove(DeviceVariable deviceVariable)
        {
            var result = deviceVariables.Remove(deviceVariable);
            if (result) InvalidateFrameData();
            return result;
        }

        public void RemoveAt(int index)
        {
            deviceVariables.RemoveAt(index);
            InvalidateFrameData();
        }

        protected override IEnumerable<byte> OnCreateDataFrame()
        {
            return base.OnCreateDataFrame()
                .Concat(deviceVariables.SelectMany(deviceVariable =>
                {
                    var deviceVariableBytes = deviceVariable.ToBytes(UseHexBitIndex ?? false);
                    return WordToLittleEndianBytes((ushort)deviceVariableBytes.Length).Concat(deviceVariableBytes);
                }));
        }
    }

    public class FEnetReadContinuousRequest : FEnetReadRequest, IContinuousAccessRequest
    {
        public FEnetReadContinuousRequest(DeviceType deviceType, uint index, int count) : base(FEnetDataType.Continuous)
        {
            startDeviceVariable = new DeviceVariable(deviceType, LSElectric.DataType.Byte, index);
            this.count = count;
        }

        public FEnetReadContinuousRequest(DeviceVariable deviceVariable, int count) : base(FEnetDataType.Continuous)
        {
            if (deviceVariable.DataType == LSElectric.DataType.Unknown)
                throw new ArgumentException(nameof(deviceVariable));

            if (deviceVariable.DataType == LSElectric.DataType.Bit)
            {
                if (deviceVariable.Index % 8 != 0) throw new ArgumentException($"{nameof(deviceVariable)}.{nameof(deviceVariable.Index)}");
                if (count % 8 != 0) throw new ArgumentException(nameof(count));
            }

            startDeviceVariable = deviceVariable;
            this.count = count;
        }

        public override ushort BlockCount => 1;

        public override object Clone() => new FEnetReadContinuousRequest(startDeviceVariable, count) { InvokeID = InvokeID, UseHexBitIndex = UseHexBitIndex };

        private DeviceVariable startDeviceVariable;

        private int count;

        public DeviceVariable StartDeviceVariable{ get => startDeviceVariable; set => SetProperty(ref startDeviceVariable, value); }

        public int Count { get => count; set => SetProperty(ref count, value); }

        protected override IEnumerable<byte> OnCreateDataFrame()
        {
            DeviceVariable deviceVariable;
            int count;
            switch (startDeviceVariable.DataType)
            {
                case LSElectric.DataType.Bit:
                    deviceVariable = new DeviceVariable(startDeviceVariable.DeviceType, LSElectric.DataType.Byte, startDeviceVariable.Index / 8);
                    count = this.count / 8;
                    break;
                case LSElectric.DataType.Byte:
                    deviceVariable = startDeviceVariable;
                    count = this.count;
                    break;
                case LSElectric.DataType.Word:
                    deviceVariable = new DeviceVariable(startDeviceVariable.DeviceType, LSElectric.DataType.Byte, startDeviceVariable.Index * 2);
                    count = this.count * 2;
                    break;
                case LSElectric.DataType.DoubleWord:
                    deviceVariable = new DeviceVariable(startDeviceVariable.DeviceType, LSElectric.DataType.Byte, startDeviceVariable.Index * 4);
                    count = this.count * 4;
                    break;
                case LSElectric.DataType.LongWord:
                    deviceVariable = new DeviceVariable(startDeviceVariable.DeviceType, LSElectric.DataType.Byte, startDeviceVariable.Index * 8);
                    count = this.count * 8;
                    break;
                default:
                    throw new ArgumentException(nameof(startDeviceVariable));
            }

            var deviceVariableBytes = deviceVariable.ToBytes(UseHexBitIndex ?? false);

            return base.OnCreateDataFrame()
                .Concat(WordToLittleEndianBytes((ushort)deviceVariableBytes.Length))
                .Concat(deviceVariableBytes)
                .Concat(WordToLittleEndianBytes((ushort)count));
        }
    }

    public abstract class FEnetWriteRequest : FEnetRequest
    {
        protected FEnetWriteRequest(FEnetDataType dataType) : base(FEnetCommand.Write, dataType) { }
    }

    public class FEnetWriteSingleRequest : FEnetWriteRequest, IDictionary<DeviceVariable, DeviceValue>
    {
        public FEnetWriteSingleRequest(DataType dataType) : this(dataType, null) { }

        public FEnetWriteSingleRequest(DataType dataType, IEnumerable<KeyValuePair<DeviceVariable, DeviceValue>> values) : base(ToFEnetDataType(dataType))
        {
            if (values != null)
                foreach (var value in values)
                    valueDictionary[value.Key] = value.Value;
        }

        public FEnetWriteSingleRequest(DataType dataType, (DeviceVariable, DeviceValue) valueTuple, params (DeviceVariable, DeviceValue)[] moreValueTuples)
            : this(dataType, new (DeviceVariable, DeviceValue)[] { valueTuple }.Concat(moreValueTuples).Select(item => new KeyValuePair<DeviceVariable, DeviceValue>(item.Item1, item.Item2))) { }

        public FEnetWriteSingleRequest((DeviceVariable, DeviceValue) valueTuple, params (DeviceVariable, DeviceValue)[] moreValueTuples)
            : this(valueTuple.Item1.DataType, new (DeviceVariable, DeviceValue)[] { valueTuple }.Concat(moreValueTuples).Select(item => new KeyValuePair<DeviceVariable, DeviceValue>(item.Item1, item.Item2))) { }

        public FEnetWriteSingleRequest(DeviceVariable deviceVariable, DeviceValue deviceValue)
            : this(deviceVariable.DataType, new KeyValuePair<DeviceVariable, DeviceValue>[] { new KeyValuePair<DeviceVariable, DeviceValue>(deviceVariable, deviceValue) }) { }

        public override ushort BlockCount => (ushort)valueDictionary.Count;

        public override object Clone() => new FEnetWriteSingleRequest(ToDataType(DataType), valueDictionary) { InvokeID = InvokeID, UseHexBitIndex = UseHexBitIndex };

        private readonly Dictionary<DeviceVariable, DeviceValue> valueDictionary = new Dictionary<DeviceVariable, DeviceValue>();

        public ICollection<DeviceVariable> Keys => valueDictionary.Keys;

        public ICollection<DeviceValue> Values => valueDictionary.Values;

        public int Count => valueDictionary.Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<DeviceVariable, DeviceValue>>)valueDictionary).IsReadOnly;

        public bool Contains(KeyValuePair<DeviceVariable, DeviceValue> item) => ((ICollection<KeyValuePair<DeviceVariable, DeviceValue>>)valueDictionary).Contains(item);

        public bool ContainsKey(DeviceVariable deviceVariable) => valueDictionary.ContainsKey(deviceVariable);

        public void CopyTo(KeyValuePair<DeviceVariable, DeviceValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<DeviceVariable, DeviceValue>>)valueDictionary).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<DeviceVariable, DeviceValue>> GetEnumerator() => valueDictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool TryGetValue(DeviceVariable deviceVariable, out DeviceValue deviceValue) => valueDictionary.TryGetValue(deviceVariable, out deviceValue);

        public void Clear()
        {
            valueDictionary.Clear();
            InvalidateFrameData();
        }

        public DeviceValue this[DeviceVariable deviceVariable]
        {
            get => valueDictionary[deviceVariable];
            set
            {
                valueDictionary[deviceVariable] = value;
                InvalidateFrameData();
            }
        }

        public void Add(DeviceVariable deviceVariable, DeviceValue deviceValue)
        {
            valueDictionary.Add(deviceVariable, deviceValue);
            InvalidateFrameData();
        }

        public void Add(KeyValuePair<DeviceVariable, DeviceValue> item)
        {
            ((ICollection<KeyValuePair<DeviceVariable, DeviceValue>>)valueDictionary).Add(item);
            InvalidateFrameData();
        }

        public bool Remove(DeviceVariable deviceVariable)
        {
            var result = valueDictionary.Remove(deviceVariable);
            if (result) InvalidateFrameData();
            return result;
        }

        public bool Remove(KeyValuePair<DeviceVariable, DeviceValue> item)
        {
            var result = ((ICollection<KeyValuePair<DeviceVariable, DeviceValue>>)valueDictionary).Remove(item);
            if (result) InvalidateFrameData();
            return result;
        }

        protected override IEnumerable<byte> OnCreateDataFrame()
        {
            foreach (var b in base.OnCreateDataFrame()
                .Concat(valueDictionary.SelectMany(keyValuePair =>
                {
                    var deviceVariableBytes = keyValuePair.Key.ToBytes(UseHexBitIndex ?? false);
                    return WordToLittleEndianBytes((ushort)deviceVariableBytes.Length).Concat(deviceVariableBytes);
                })))
                yield return b;

            foreach (var keyValuePair in valueDictionary)
            {
                switch (keyValuePair.Key.DataType)
                {
                    case LSElectric.DataType.Bit:
                        foreach (var b in WordToLittleEndianBytes(1)) yield return b;
                        yield return (byte)(keyValuePair.Value.BitValue ? 1 : 0);
                        break;
                    case LSElectric.DataType.Byte:
                        foreach (var b in WordToLittleEndianBytes(1)) yield return b;
                        yield return keyValuePair.Value.ByteValue;
                        break;
                    case LSElectric.DataType.Word:
                        foreach (var b in WordToLittleEndianBytes(2)) yield return b;
                        foreach (var b in ValueToLittleEndianBytes(keyValuePair.Value.WordValue)) yield return b;
                        break;
                    case LSElectric.DataType.DoubleWord:
                        foreach (var b in WordToLittleEndianBytes(4)) yield return b;
                        foreach (var b in ValueToLittleEndianBytes(keyValuePair.Value.DoubleWordValue)) yield return b;
                        break;
                    case LSElectric.DataType.LongWord:
                        foreach (var b in WordToLittleEndianBytes(8)) yield return b;
                        foreach (var b in ValueToLittleEndianBytes(keyValuePair.Value.LongWordValue)) yield return b;
                        break;
                }
            }
        }
    }

    public class FEnetWriteContinuousRequest : FEnetWriteRequest, IList<byte>, IContinuousAccessRequest
    {
        public FEnetWriteContinuousRequest(DeviceType deviceType, uint index) : this(deviceType, index, null) { }

        public FEnetWriteContinuousRequest(DeviceType deviceType, uint index, IEnumerable<byte> deviceValues) : base(FEnetDataType.Continuous)
        {
            startDeviceVariable = new DeviceVariable(deviceType, LSElectric.DataType.Byte, index);

            if (deviceValues == null)
                this.deviceValues = new List<byte>();
            else
                this.deviceValues = new List<byte>(deviceValues);
        }

        public FEnetWriteContinuousRequest(DeviceType deviceType, uint index, byte value, params byte[] moreValues)
            : this(deviceType, index, new byte[] { value }.Concat(moreValues)) { }

        public override ushort BlockCount => 1;

        public override object Clone() => new FEnetWriteContinuousRequest(startDeviceVariable.DeviceType, startDeviceVariable.Index, deviceValues) { InvokeID = InvokeID, UseHexBitIndex = UseHexBitIndex };

        private readonly List<byte> deviceValues;

        private DeviceVariable startDeviceVariable;

        public DeviceVariable StartDeviceVariable { get => startDeviceVariable; }

        public DeviceType StartDeviceType
        {
            get => startDeviceVariable.DeviceType;
            set
            {
                if (startDeviceVariable.DeviceType != value)
                {
                    startDeviceVariable = new DeviceVariable(value, startDeviceVariable.DataType, startDeviceVariable.Index, startDeviceVariable.SubIndices.ToArray());
                    InvalidateFrameData();
                }
            }
        }

        public uint StartDeviceIndex
        {
            get => startDeviceVariable.Index;
            set
            {
                if (startDeviceVariable.Index != value)
                {
                    startDeviceVariable = new DeviceVariable(startDeviceVariable.DeviceType, startDeviceVariable.DataType, value, startDeviceVariable.SubIndices.ToArray());
                    InvalidateFrameData();
                }
            }
        }

        public int Count => deviceValues.Count;

        public bool IsReadOnly => ((ICollection<byte>)deviceValues).IsReadOnly;

        public bool Contains(byte deviceValue) => deviceValues.Contains(deviceValue);

        public int IndexOf(byte deviceValue) => deviceValues.IndexOf(deviceValue);

        public IEnumerator<byte> GetEnumerator() => deviceValues.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(byte[] array, int arrayIndex) => deviceValues.CopyTo(array, arrayIndex);


        public void Clear()
        {
            deviceValues.Clear();
            InvalidateFrameData();
        }

        public byte this[int index]
        {
            get => deviceValues[index];
            set
            {
                deviceValues[index] = value;
                InvalidateFrameData();
            }
        }

        public void Add(byte deviceValue)
        {
            deviceValues.Add(deviceValue);
            InvalidateFrameData();
        }

        public void Insert(int index, byte deviceValue)
        {
            deviceValues.Insert(index, deviceValue);
            InvalidateFrameData();
        }

        public bool Remove(byte deviceValue)
        {
            var result = deviceValues.Remove(deviceValue);
            if (result) InvalidateFrameData();
            return result;
        }

        public void RemoveAt(int index)
        {
            deviceValues.RemoveAt(index);
            InvalidateFrameData();
        }

        protected override IEnumerable<byte> OnCreateDataFrame()
        {
            var deviceVariableBytes = startDeviceVariable.ToBytes(UseHexBitIndex ?? false);

            foreach (var b in base.OnCreateDataFrame()
                .Concat(WordToLittleEndianBytes((ushort)deviceVariableBytes.Length))
                .Concat(deviceVariableBytes)
                .Concat(WordToLittleEndianBytes((ushort)(Count))))
                yield return b;

            foreach (var value in deviceValues)
            {
                yield return value;
            }
        }
    }


}

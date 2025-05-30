using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MBE.Driver.LSElectric
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct DeviceValue
    {
        public DeviceValue(bool bitValue) : this() => BitValue = bitValue;
        public DeviceValue(byte byteValue) : this() => this.byteValue = byteValue;
        public DeviceValue(short wordValue) : this() => this.wordValue = wordValue;
        public DeviceValue(int doubleWordValue) : this() => this.doubleWordValue = doubleWordValue;
        public DeviceValue(long longWordValue) : this() => this.longWordValue = longWordValue;
        public DeviceValue(sbyte signedByteValue) : this() => this.signedByteValue = signedByteValue;
        public DeviceValue(ushort unsignedWordValue) : this() => this.unsignedWordValue = unsignedWordValue;
        public DeviceValue(uint unsignedDoubleWordValue) : this() => this.unsignedDoubleWordValue = unsignedDoubleWordValue;
        public DeviceValue(ulong unsignedLongWordValue) : this() => this.unsignedLongWordValue = unsignedLongWordValue;
        public DeviceValue(float singleFloatingPointValue) : this() => this.singleFloatingPointValue = singleFloatingPointValue;
        public DeviceValue(double doubleFloatingPointValue) : this() => this.doubleFloatingPointValue = doubleFloatingPointValue;

        [FieldOffset(0)] private byte byteValue;
        [FieldOffset(0)] private short wordValue;
        [FieldOffset(0)] private int doubleWordValue;
        [FieldOffset(0)] private long longWordValue;
        [FieldOffset(0)] private sbyte signedByteValue;
        [FieldOffset(0)] private ushort unsignedWordValue;
        [FieldOffset(0)] private uint unsignedDoubleWordValue;
        [FieldOffset(0)] private ulong unsignedLongWordValue;
        [FieldOffset(0)] private float singleFloatingPointValue;
        [FieldOffset(0)] private double doubleFloatingPointValue;

        public bool BitValue
        {
            get => (doubleWordValue & 1) == 1;
            set
            {
                unsignedLongWordValue = 0;
                doubleWordValue = value ? 1 : 0;
            }
        }

        public byte ByteValue
        {
            get => byteValue;
            set { unsignedLongWordValue = 0; byteValue = value; }
        }

        public short WordValue
        {
            get => wordValue;
            set { unsignedLongWordValue = 0; wordValue = value; }
        }

        public int DoubleWordValue
        {
            get => doubleWordValue;
            set { unsignedLongWordValue = 0; doubleWordValue = value; }
        }

        public long LongWordValue
        {
            get => longWordValue;
            set { longWordValue = value; }
        }

        public sbyte SignedByteValue
        {
            get => signedByteValue;
            set { unsignedLongWordValue = 0; signedByteValue = value; }
        }

        public ushort UnsignedWordValue
        {
            get => unsignedWordValue;
            set { unsignedLongWordValue = 0; unsignedWordValue = value; }
        }

        public uint UnsignedDoubleWordValue
        {
            get => unsignedDoubleWordValue;
            set { unsignedLongWordValue = 0; unsignedDoubleWordValue = value; }
        }

        public ulong UnsignedLongWordValue
        {
            get => unsignedLongWordValue;
            set { unsignedLongWordValue = value; }
        }

        public float SingleFloatingPointValue
        {
            get => singleFloatingPointValue;
            set { unsignedLongWordValue = 0; singleFloatingPointValue = value; }
        }

        public double DoubleFloatingPointValue
        {
            get => doubleFloatingPointValue;
            set { doubleFloatingPointValue = value; }
        }

        public static implicit operator bool(DeviceValue deviceValue) => deviceValue.BitValue;

        public static implicit operator byte(DeviceValue deviceValue) => deviceValue.byteValue;

        public static implicit operator short(DeviceValue deviceValue) => deviceValue.wordValue;

        public static implicit operator int(DeviceValue deviceValue) => deviceValue.doubleWordValue;

        public static implicit operator long(DeviceValue deviceValue) => deviceValue.longWordValue;

        public static implicit operator sbyte(DeviceValue deviceValue) => deviceValue.signedByteValue;

        public static implicit operator ushort(DeviceValue deviceValue) => deviceValue.unsignedWordValue;

        public static implicit operator uint(DeviceValue deviceValue) => deviceValue.unsignedDoubleWordValue;

        public static implicit operator ulong(DeviceValue deviceValue) => deviceValue.unsignedLongWordValue;

        public static implicit operator float(DeviceValue deviceValue) => deviceValue.singleFloatingPointValue;

        public static implicit operator double(DeviceValue deviceValue) => deviceValue.doubleFloatingPointValue;

        public static implicit operator DeviceValue(bool value) => new DeviceValue(value);

        public static implicit operator DeviceValue(byte value) => new DeviceValue(value);

        public static implicit operator DeviceValue(short value) => new DeviceValue(value);

        public static implicit operator DeviceValue(int value) => new DeviceValue(value);

        public static implicit operator DeviceValue(long value) => new DeviceValue(value);

        public static implicit operator DeviceValue(sbyte value) => new DeviceValue(value);

        public static implicit operator DeviceValue(ushort value) => new DeviceValue(value);

        public static implicit operator DeviceValue(uint value) => new DeviceValue(value);

        public static implicit operator DeviceValue(ulong value) => new DeviceValue(value);

        public static implicit operator DeviceValue(float value) => new DeviceValue(value);

        public static implicit operator DeviceValue(double value) => new DeviceValue(value);

        public static bool operator ==(DeviceValue value1, DeviceValue value2) => value1.Equals(value2);

        public static bool operator !=(DeviceValue value1, DeviceValue value2) => !value1.Equals(value2);

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        public byte[] GetBytes(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Bit:
                    return BitConverter.GetBytes(BitValue);
                case DataType.Byte:
                    return new byte[] { ByteValue };
                case DataType.Word:
                    return BitConverter.GetBytes(WordValue);
                case DataType.DoubleWord:
                    return BitConverter.GetBytes(DoubleWordValue);
                case DataType.LongWord:
                    return BitConverter.GetBytes(LongWordValue);
                default:
                    return null;
            }
        }
    }
}

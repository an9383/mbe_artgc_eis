using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MBE.Driver.LSElectric
{
    public interface IDeviceDataBlock : IReadOnlyList<byte>
    {
        DeviceVariable StartDeviceVariable { get; }

        int DeviceValueCount { get; }

        DeviceValue this[DataType dataType, uint index] { get; }
    }

    public static class IDeviceDataBlockExtensions
    {
        public static IEnumerable<DeviceValue> Cast(this IDeviceDataBlock deviceDataBlock, DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Bit:
                    foreach (var b in deviceDataBlock)
                        for (int i = 0; i < 8; i++)
                            yield return ((b >> i) & 1) == 1;
                    break;
                case DataType.Byte:
                    foreach (var b in deviceDataBlock)
                        yield return b;
                    break;
                case DataType.Word:
                    var bytes = deviceDataBlock.ToArray();
                    for (int i = 0; i < deviceDataBlock.Count; i += 2)
                        yield return BitConverter.ToInt16(bytes, i);
                    break;
                case DataType.DoubleWord:
                    bytes = deviceDataBlock.ToArray();
                    for (int i = 0; i < deviceDataBlock.Count; i += 4)
                        yield return BitConverter.ToInt32(bytes, i);
                    break;
                case DataType.LongWord:
                    bytes = deviceDataBlock.ToArray();
                    for (int i = 0; i < deviceDataBlock.Count; i += 8)
                        yield return BitConverter.ToInt64(bytes, i);
                    break;
                default:
                    throw new ArgumentException(nameof(dataType));
            }
        }
    }
}

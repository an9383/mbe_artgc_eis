using System;
using System.Collections.Generic;
using System.Text;

namespace MBE.Driver.LSElectric.FEnet
{
    public enum FEnetCommand : ushort
    {
        Read = 0x0054,
        Write = 0x0058,
    }

    public enum FEnetDataType : ushort
    {
        Bit = 0x0000,
        Byte = 0x0001,
        Word = 0x0002,
        DoubleWord = 0x0003,
        LongWord = 0x0004,
        Continuous = 0x0014,
    }

    public enum FEnetNAKCode : ushort
    {
        Unknown = 0x0000,
        OverRequestReadBlockCount = 0x0001,
        DeviceVariableTypeError = 0x0002,
        IlegalDeviceMemory = 0x0003,
        OutOfRangeDeviceVariable = 0x0004,
        OverDataLengthIndividual = 0x0005,
        OverDataLengthTotal = 0x0006,
        IlegalCompanyID = 0x0075,
        IlegalLength = 0x0076,
        ErrorChacksum = 0x0076,
        IlegalCommand = 0x0077,
    }

    public enum FEnetCommErrorCode
    {
        NotDefined,
        ResponseLengthDoNotMatch,
        ResponseCommandDoNotMatch,
        ResponseDataTypeDoNotMatch,
        ResponseDataBlockCountDoNotMatch,
        ResponseDataCountNotMatch,
        ErrorDataCount,
        ErrorChecksum,
        ResponseTimeout
    }
}

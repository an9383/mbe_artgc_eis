namespace MBE.Driver.LSElectric
{
    public enum DataType : byte
    {
        Unknown = 0,
        Bit = 0x58,
        Byte = 0x42,
        Word = 0x57,
        DoubleWord = 0x44,
        LongWord = 0x4c
    }
}

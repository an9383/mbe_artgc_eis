using MBE.Driver.Common;

namespace MBE.Driver.LSElectric.FEnet
{
    public class FEnetNAKException : ErrorCodeException<FEnetNAKCode>
    {
        public FEnetNAKException(FEnetNAKCode nakCode) : base(nakCode)
        {
            NAKCodeValue = (ushort)nakCode;
        }

        public FEnetNAKException(FEnetNAKCode nakCode, ushort nakCodeValue) : base(nakCode)
        {
            NAKCodeValue = nakCodeValue;
        }

        public ushort NAKCodeValue { get; }
    }
}

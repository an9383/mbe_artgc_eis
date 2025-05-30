using System;
using System.Collections.Generic;
using System.Text;

namespace MBE.Driver.LSElectric
{
    public class DeviceVariableValue
    {
        public DeviceVariableValue(DeviceVariable deviceVariable)
        {
            DeviceVariable = deviceVariable;
        }

        public DeviceVariable DeviceVariable { get; }

        public DeviceValue DeviceValue { get; set; }

        public byte[] DeviceValueBytes => DeviceValue.GetBytes(DeviceVariable.DataType);
    }
}

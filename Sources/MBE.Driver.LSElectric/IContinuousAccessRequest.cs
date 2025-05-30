using System;
using System.Collections.Generic;
using System.Text;

namespace MBE.Driver.LSElectric
{
    public interface IContinuousAccessRequest
    {
        DeviceVariable StartDeviceVariable { get; }

        int Count { get; }
    }


    public static class ContinuousRequestExtensions
    {
        public static IEnumerable<DeviceVariable> ToDeviceVariables(this IContinuousAccessRequest request)
        {
            var deviceVariable = request.StartDeviceVariable;
            for (int i = 0; i < request.Count; i++)
            {
                yield return deviceVariable;
                deviceVariable = deviceVariable.Increase();
            }
        }
    }
}

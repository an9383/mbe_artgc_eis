using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MBE.Driver.Common
{
    public class ErrorCodeException<TErrorCode> : Exception where TErrorCode : Enum
    {
        public ErrorCodeException(TErrorCode code)
        {
            Code = code;
        }

        public ErrorCodeException(TErrorCode code, Exception innerException) : base(null, innerException)
        {
            Code = code;
        }

        public TErrorCode Code { get; }

        public override string Message
        {
            get
            {
                var codeName = Code.ToString();
                return (typeof(TErrorCode).GetMember(codeName, BindingFlags.Static | BindingFlags.Public)
                    ?.FirstOrDefault()?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? codeName;
            }
        }
    }
}

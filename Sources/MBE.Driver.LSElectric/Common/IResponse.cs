using System;
using System.Collections.Generic;
using System.Text;

namespace MBE.Driver.Common
{
    public interface IResponse : IProtocolMessage
    {
    }

    public interface IResponse<TErrorCode> : IResponse where TErrorCode : Enum
    {
    }
}

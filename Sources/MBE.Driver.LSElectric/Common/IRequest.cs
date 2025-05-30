using System;
using System.Collections.Generic;
using System.Text;

namespace MBE.Driver.Common
{
    public interface IRequest : IProtocolMessage
    {
    }

    public interface IRequest<TErrorCode> : IRequest where TErrorCode : Enum
    {
    }
}

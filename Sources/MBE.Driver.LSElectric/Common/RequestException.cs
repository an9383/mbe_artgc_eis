using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MBE.Driver.Common
{
    public class RequestException<TErrorCode> : ErrorCodeException<TErrorCode> where TErrorCode : Enum
    {
        public RequestException(TErrorCode errorCode, Exception innerException) : base(errorCode, innerException)
        {
        }

        public RequestException(Exception innerException, IRequest<TErrorCode> request) : base(default(TErrorCode), innerException)
        {
            ReceivedBytes = new byte[0];
            Request = request;
        }

        public RequestException(IEnumerable<byte> receivedMessage, Exception innerException, IRequest<TErrorCode> request) : base(default(TErrorCode), innerException)
        {
            ReceivedBytes = receivedMessage?.ToArray() ?? new byte[0];
            Request = request;
        }

        public RequestException(TErrorCode errorCode, IEnumerable<byte> receivedMessage, IRequest<TErrorCode> request) : base(errorCode)
        {
            ReceivedBytes = receivedMessage?.ToArray() ?? new byte[0];
            Request = request;
        }

        public RequestException(TErrorCode errorCode, IEnumerable<byte> receivedMessage, Exception innerException, IRequest<TErrorCode> request) : base(errorCode, innerException)
        {
            ReceivedBytes = receivedMessage?.ToArray() ?? new byte[0];
            Request = request;
        }

        public IReadOnlyList<byte> ReceivedBytes { get; }
        
        public IRequest<TErrorCode> Request { get; }
    }
}

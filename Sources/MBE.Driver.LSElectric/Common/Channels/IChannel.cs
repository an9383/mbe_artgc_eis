using System;
using System.Collections.Generic;
using MBE.Driver.Common.Logging;

namespace MBE.Driver.Common.Channels
{
    public interface IChannel : IDisposable
    {
        bool IsDisposed { get; }

        IChannelLogger Logger { get; set; }

        string Description { get; }

        bool Connected { get; }

        void Open();

        void Write(byte[] bytes);

        byte Read(int timeout);

        IEnumerable<byte> Read(uint count, int timeout);

        IEnumerable<byte> ReadAllRemain();

        uint BytesToRead { get; }
    }
}

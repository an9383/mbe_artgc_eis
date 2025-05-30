using System.Collections.Generic;
using MBE.Driver.Common.Logging;

namespace MBE.Driver.Common.Channels
{
    public abstract class Channel : IChannel
    {
        public bool IsDisposed { get; protected set; }
        
        public abstract void Dispose();

        public abstract void Write(byte[] bytes);
        
        public abstract byte Read(int timeout);

        public abstract void Open();

        public abstract IEnumerable<byte> Read(uint count, int timeout);

        public abstract IEnumerable<byte> ReadAllRemain();

        public abstract bool Connected { get; }

        public IChannelLogger Logger { get; set; }
        
        public abstract string Description { get; }

        public abstract uint BytesToRead { get; }
    }
}

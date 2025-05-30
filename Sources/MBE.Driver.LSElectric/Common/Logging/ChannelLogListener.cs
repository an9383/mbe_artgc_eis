using System;

namespace MBE.Driver.Common.Logging
{
    public class ChannelLogListener : IChannelLogger
    {
        public event EventHandler<ChannelLoggedEventArgs> Logged;

        public void Log(ChannelLog log)
        {
            Logged?.Invoke(this, new ChannelLoggedEventArgs(log));
        }
    }

    public class ChannelLoggedEventArgs : EventArgs
    {
        internal ChannelLoggedEventArgs(ChannelLog log)
        {
            Log = log;
        }

        public ChannelLog Log { get; }
    }
}

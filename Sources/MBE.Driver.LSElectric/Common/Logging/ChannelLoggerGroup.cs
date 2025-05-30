using System;
using System.Collections;
using System.Collections.Generic;

namespace MBE.Driver.Common.Logging
{
    public class ChannelLoggerGroup : IEnumerable<IChannelLogger>, IChannelLogger
    {
        public ChannelLoggerGroup()
        {
        }

        public ChannelLoggerGroup(IEnumerable<IChannelLogger> channelLoggers)
        {
            foreach (var channelLogger in channelLoggers)
            {
                this.channelLoggers.Add(channelLogger);
            }
        }

        private readonly HashSet<IChannelLogger> channelLoggers = new HashSet<IChannelLogger>();

        public void AddChannelLogger(IChannelLogger channelLogger)
        {
            lock (channelLoggers)
            {
                channelLoggers.Add(channelLogger);
            }
        }

        public bool RemoveChannelLogger(IChannelLogger channelLogger)
        {
            lock (channelLoggers)
            {
                return channelLoggers.Remove(channelLogger);
            }
        }

        public void Log(ChannelLog log)
        {
            lock (channelLoggers)
            {
                foreach (var channelLogger in channelLoggers)
                {
                    channelLogger.Log(log);
                }
            }
        }

        public IEnumerator<IChannelLogger> GetEnumerator()
        {
            lock (channelLoggers)
            {
                foreach (var channelLogger in channelLoggers)
                    yield return channelLogger;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

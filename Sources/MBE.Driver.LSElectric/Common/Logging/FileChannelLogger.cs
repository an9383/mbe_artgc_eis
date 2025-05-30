using System;
using KR.MBE.CommonLibrary.Manager;

namespace MBE.Driver.Common.Logging
{
    public class FileChannelLogger : IChannelLogger
    {
        public void Log(ChannelLog log)
        {
            LogManager.Instance.Channel(log.ChannelDescription, log.ToString());
            //Console.WriteLine($"({log.ChannelDescription}) {log}");
        }
    }
}

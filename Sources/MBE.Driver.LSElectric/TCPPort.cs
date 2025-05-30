using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System.Windows.Forms;
using MBE.Driver.LSElectric.FEnet;
using MBE.Driver.LSEletric.Channels;
using MBE.Driver.Common.Logging;
using log4net;


namespace MBE.Driver.LSElectric
{
    public partial class Station
    {
        public int timeOut { get; set; } = 100;

        public FEnetClient _clientSocket;

        public bool ConnectSocket()
        {
            try
            {
                if (_clientSocket?.Channel != null && _clientSocket?.Channel.Connected == false)
                {
                    CloseSocket();
                }

                if (_clientSocket == null)
                {
                    var logger = new FileChannelLogger();

                    // 소켓을 생성한다.
                    _clientSocket = new FEnetClient(new TcpChannel(REMOTEIP, REMOTEPORT) { Logger = logger });

                    _clientSocket.Timeout = timeOut;

                    try
                    {
                        _clientSocket.Open();
                    }catch (Exception ex)
                    {
                        KR.MBE.CommonLibrary.Manager.LogManager.Instance.Debug($"[{REMOTEIP},{REMOTEPORT}:ConnectSocket] Exception: {ex.Message}");
                        return false;
                    }
                    commStatus = true;
                }

                return true;
            }
            catch(Exception ex)
            {
                commStatus = false;

                return false;
            }
        }

        public void CloseSocket()
        {
            lock (this)
            {
                try
                {
                    if (_clientSocket != null)
                    {
                        _clientSocket.Dispose();
                    }
                }
                finally
                {
                    _clientSocket = null;
                }
            }
        }


    }
}

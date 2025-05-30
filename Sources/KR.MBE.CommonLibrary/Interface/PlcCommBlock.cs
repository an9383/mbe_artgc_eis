using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR.MBE.CommonLibrary.Interface
{
    public interface PlcCommBlock
    {
        public string EQUIPMENTID { get; set; }
        public string STATIONID { get; set; }
        public int BLOCKNO { get; set; }
        public string DEVCODE { get; set; }              // I, Q, M, L, N, K, U, R, A, W, F
        public int BLOCKTYPE { get; set; }            // I=0, Q=1, M=2, L=3, N=4, K=5, U=6, R=7, A=8, W=9, F=10
        public int STARTADDRESS { get; set; }            // Function Code가 없는 Address
        public int READDATANUMBER { get; set; }          // Read Data Numbers
        public int COMMINTERVAL { get; set; }

        public int d_BlockIdx { get; set; }
        public int d_txSize { get; set; }               // Tx Byte Size
        public int d_Interval { get; set; }

        public int nRxSize { get; set; }
        public int nTxSize { get; set; }

        public byte[] d_pRxBuff { get; set; }
        public byte[] d_pTxBuff { get; set; }
        public string[] DevCodeArray { get; set; }
    }
}

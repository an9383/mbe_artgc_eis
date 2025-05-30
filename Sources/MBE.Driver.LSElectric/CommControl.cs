using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apache.NMS.ActiveMQ.Util;
using KR.MBE.CommonLibrary.Handler;
using KR.MBE.CommonLibrary.Interface;
using KR.MBE.CommonLibrary.Manager;
using KR.MBE.CommonLibrary.Struct;
using MBE.Driver.Common;
using MBE.Driver.LSElectric.FEnet;
using Microsoft.AspNetCore.Server.HttpSys;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using BitArray = System.Collections.BitArray;

namespace MBE.Driver.LSElectric
{
    public partial class Station
    {
        public int nRxCnt;
        public int nRxErr;
        public int nTxCnt;
        public int nTxErr;
        public int nErrCnt;

        public int nTaskID;

        CancellationTokenSource ctsRead;
        CancellationTokenSource ctsWrite;
        CancellationTokenSource ctsConnectCheck;
        CancellationTokenSource ctsHeartBit;
        CancellationTokenSource ctsFromRCSConnectionRequest;
        CancellationTokenSource ctsRCSDisconnectRequest;
        CancellationTokenSource ctsVMSScenarioAutoChangeEvent;
        CancellationTokenSource ctsVMSScenarioManualChangeEvent;

        public void VMSScenarioManualChangeEvent(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            var sRCSStatus = m_TagList.FindByDescription("RCS Status");
            var sConnectedCraneNumber = m_TagList.FindByDescription("Connected Crane Number"); ;
            var dCCTVSetScenarioNumber =  m_TagList.FindByDescription("CCTV Set Scenario Number"); ;
            var eCCTVScenarioChangeRequest = m_TagList.FindByDescription("CCTV Scenario Change Request Reply"); ;
            var eCCTVScenarioChangeRequestReply = m_TagList.FindByDescription("CCTV Scenario Change Request"); ;
            var dCCTVScenarioChangeErrorCode = m_TagList.FindByDescription("CCTV Scenario Change Error Code");

            CancellationToken token = (CancellationToken)obj;

            while (commEnable)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (_clientSocket == null)
                {
                    continue;
                }

                if (_clientSocket.Channel == null)
                {
                    continue;
                }

                if (!_clientSocket.Channel.Connected)
                {
                    continue;
                }

                if (sRCSStatus.sTagVal != "0" && sConnectedCraneNumber.sTagVal != "0")
                {
                    if (eCCTVScenarioChangeRequest.sTagVal == "1" && eCCTVScenarioChangeRequestReply.sTagVal == "0")
                    {
                        bool isSuccess = true;
                        PlcInterface craneStation = null;

                        if (int.TryParse(sConnectedCraneNumber.sTagVal, out int craneNumber))
                        {
                            craneStation = CraneManager.m_CraneDoc.m_pStation.FindByEquipmentId($"CR{string.Format("{0:000}", craneNumber)}");

                            if (craneStation == null)
                            {
                                dCCTVScenarioChangeErrorCode._TagVal = 8;
                                isSuccess = false;
                            }
                        }
                        else
                        {
                            isSuccess = false;
                        }

                        if (isSuccess)
                        {
                            eCCTVScenarioChangeRequest._TagVal = 1;
                        }
                        else
                        {
                            eCCTVScenarioChangeRequest._TagVal = 2;
                        }
                    }
                    else if (eCCTVScenarioChangeRequest.sTagVal == "0" && eCCTVScenarioChangeRequestReply.sTagVal != "0")
                    {
                        dCCTVScenarioChangeErrorCode._TagVal = 0;
                        eCCTVScenarioChangeRequestReply._TagVal = 0;

                    }
                    else
                    {
                        continue;
                    }

                    // CCTV 전환명령 진행
                }
                else
                {
                    eCCTVScenarioChangeRequestReply._TagVal = 8;
                    eCCTVScenarioChangeRequestReply._TagVal = 2;
                }

                Thread.Sleep(1000);
            }
        }

        public void VMSScenarioAutoChangeEvent(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            var sRCSStatus = m_TagList.FindByDescription("RCS Status");
            var sConnectedCraneNumber = m_TagList.FindByDescription("Connected Crane Number");
            var sTrolleyForwardStep1 = m_TagList.FindByDescription("MICRO MOTION/TROLLEY FORWARD 1 STEP (저속 / 1단)");
            var sTrolleyBackwardStep1 = m_TagList.FindByDescription("MICRO MOTION/TROLLEY BACKWARD 1 STEP (저속 / 1단)");
            var sGantryLeftStep1 = m_TagList.FindByDescription("MICRO MOTION/GANTRY LEFT 1 STEP (저속 / 1단)");
            var sGantryRightStep1 = m_TagList.FindByDescription("MICRO MOTION/GANTRY RIGHT 1 STEP (저속 / 1단)");

            CancellationToken token = (CancellationToken)obj;

            while (commEnable)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                lock (this)
                {
                    if (_clientSocket == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (_clientSocket.Channel == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (!_clientSocket.Channel.Connected)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }

                if (sRCSStatus.sTagVal == "2" && sConnectedCraneNumber.sTagVal != "0")
                {
                    if (sTrolleyForwardStep1.sTagVal == "1" || sTrolleyBackwardStep1.sTagVal == "1")
                    {
                        //트롤리뷰 변경
                        continue;
                    }

                    if (sGantryLeftStep1.sTagVal == "1" || sGantryRightStep1.sTagVal == "1")
                    {
                        //트럭오버뷰 변경
                        continue;
                    }

                    // CCTV 전환명령 진행
                }

                Thread.Sleep(1000);
            }
        }

        public void RCSDisconnectRequest(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            var eRemoteControlDisconnectionRequest = m_TagList.FindByDescription("Remote Control Disconnection Request");
            var sRCSStatus = m_TagList.FindByDescription("RCS Status");
            var sConnectedCraneNumber = m_TagList.FindByDescription("Connected Crane Number");

            CancellationToken token = (CancellationToken)obj;

            while (commEnable)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                lock (this)
                {
                    if (_clientSocket == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (_clientSocket.Channel == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (!_clientSocket.Channel.Connected)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }

                if (eRemoteControlDisconnectionRequest.sTagVal == "1" || sRCSStatus.sTagVal == "0" || sConnectedCraneNumber.sTagVal == "0")
                {
                    // CCTV 꺼버려야함
                }

                Thread.Sleep(1000);
            }
        }

        public void RCSConnectionRequest(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            var eRemoteControlRequestReplyFromRCS = m_TagList.FindByDescription("Remote Control Request Reply From RCS");
            var eRemoteControlRequestFromRCS = m_TagList.FindByDescription("Remote Control Request From RCS");
            var dRemoteControlRequestFromRCSErrorCode = m_TagList.FindByDescription("Remote Control Request From RCS Error Code");
            var dRCSConnectionRequestCraneNumber = m_TagList.FindByDescription("RCS Connection Request Crane Number");
            var dRCSConnectionRequestMode = m_TagList.FindByDescription("RCS Connection Request Mode");

            CancellationToken token = (CancellationToken)obj;

            while (commEnable)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                lock (this)
                {
                    if (_clientSocket == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (_clientSocket.Channel == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (!_clientSocket.Channel.Connected)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }

                if (eRemoteControlRequestFromRCS.sTagVal == "1" && eRemoteControlRequestReplyFromRCS.sTagVal == "0")
                {
                    bool isSuccess = true;
                    var requestMode = dRCSConnectionRequestMode.sTagVal;
                    PlcInterface craneStation = null;

                    if (int.TryParse(dRCSConnectionRequestCraneNumber.sTagVal, out int craneNumber))
                    {
                        craneStation = CraneManager.m_CraneDoc.m_pStation.FindByEquipmentId($"CR{string.Format("{0:000}", craneNumber)}");

                        if (craneStation == null)
                        {
                            dRemoteControlRequestFromRCSErrorCode._TagVal = 8;
                            isSuccess = false;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                    }

                    if (requestMode == "2" && isSuccess)
                    {
                        // 접속 가능여부 체크
                    }

                    if (isSuccess)
                    {
                        eRemoteControlRequestReplyFromRCS._TagVal = 1;
                    }
                    else
                    {
                        eRemoteControlRequestReplyFromRCS._TagVal = 2;
                    }
                }
                else if (eRemoteControlRequestFromRCS.sTagVal == "0" && eRemoteControlRequestReplyFromRCS.sTagVal != "0")
                {
                    dRemoteControlRequestFromRCSErrorCode._TagVal = 0;
                    eRemoteControlRequestReplyFromRCS._TagVal = 0;
                }
                else
                {
                    continue;
                }

                Thread.Sleep(1000);
            }
        }

        public void ConnectionCheck(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            CancellationToken token = (CancellationToken)obj;

            while (commEnable)
            {
                if (token.IsCancellationRequested)
                {
                    commStatus = false;
                    CloseSocket();
                    break;
                }

                ConnectSocket();

                Thread.Sleep(1000);
            }
        }

        public void HeartBit(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            CancellationToken token = (CancellationToken)obj;

            List<CTagBase> heartBitTags = new List<CTagBase>();

            foreach (var tag in m_TagList)
            {
                if (tag.IsHeartbeat && tag.IsWriteable)
                {
                    heartBitTags.Add(tag);
                }
            }

            while (commEnable)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                lock (this)
                {
                    if (_clientSocket == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (_clientSocket.Channel == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (!_clientSocket.Channel.Connected)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }

                Parallel.ForEach(heartBitTags, tag =>
                {
                    if (tag.IsWriteable)
                    {
                        if (tag.AsBool.HasValue)
                        {
                            tag.AsBool = tag.AsBool.Value ? false : true;
                        }
                    }
                    else
                    {

                    }
                });

                Thread.Sleep(1000);
            }
        }

        public void WriteBlockProcess(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            byte[] rcvBuff = new byte[1500];
            int sndSize = 0;
            int rcvSize = 0;
            bool[] fRtn = new bool[nBlkNum];
            bool result = true;
            int i = 0;
            CancellationToken token = (CancellationToken)obj;

            while (commEnable)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }


                lock (this)
                {
                    if (_clientSocket == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (_clientSocket.Channel == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (!_clientSocket.Channel.Connected)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }

                try
                {
                    while (writeQueue.Count > 0)
                    {
                        var writeData = writeQueue.Dequeue();

                        var requestData = new FEnetWriteSingleRequest(DataType.Word)
                        {
                            [$"%{writeData.Key}"] = int.Parse(writeData.Value)
                        };

                        var nakResponse = _clientSocket.Request(requestData) as FEnetNAKResponse;

                        if (nakResponse != null)
                        {
                            LogManager.Instance.Channel("Exception", $"쓰기 오류 발생:{nakResponse.NAKCode}, {nakResponse.NAKCodeValue}");
                        }

                        Thread.Sleep(50);
                    }
                }
                catch
                {
                    nRxErr++;
                    if (nRxErr > 999999) nRxErr = 0;

                    //_connectReadSocket = false;
                    commStatus = false;
                }

                Thread.Sleep(p_ScanTime);
            }
        }

        public void ReadBlockProcess(object? obj)
        {
            if (obj == null)
            {
                return;
            }

            bool[] fRtn = new bool[nBlkNum];
            bool result = true;
            int i = 0;
            CancellationToken token = (CancellationToken)obj;

            while (commEnable)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                lock (this)
                {
                    if (_clientSocket == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (_clientSocket.Channel == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (!_clientSocket.Channel.Connected)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }

                try
                {
                    for (i = 0; i < nBlkNum; i++)
                    {
                        nTxCnt++;
                        if (nTxCnt > 999999) nTxCnt = 0;

                        var receivedBytes = _clientSocket.Read(DeviceType.M, ((uint)m_Commblock[i].STARTADDRESS * 2), m_Commblock[i].READDATANUMBER * 2).ToArray();

                        //var receivedBytes = (_clientSocket.Request(requestData) as FEnetReadContinuousResponse).ToArray();

                        LogManager.Instance.PLCRead(receivedBytes, receivedBytes.Length);

                        if (receivedBytes.Length == (m_Commblock[i].READDATANUMBER * 2))
                        {
                            nRxCnt++;
                            if (nRxCnt > 999999) nRxCnt = 0;
                            commStatus = true;

                            fRtn[i] = SetCommTagVal(receivedBytes, i);

                            nTaskID++;
                            if (nTaskID > 32760) nTaskID = 1;

                            //SaveBuff(rcvBuff, rcvSize, p_ReadCnt);
                        }
                        else // if (rcvSize < 10)
                        {
                            nRxErr++;
                            if (nRxErr > 999999) nRxErr = 0;
                            commStatus = false;
                            //SaveBuff(rcvBuff, rcvSize, p_ReadCnt);
                        }
                    }

                    for (i = 0; i < nBlkNum; i++)
                    {
                        result = result && fRtn[i];
                    }

                    if (result)
                    {
                        result = SendTagInfo();

                    }
                }
                catch (Exception ex)
                {
                    nRxErr++;
                    if (nRxErr > 999999) nRxErr = 0;
                }

                Thread.Sleep(p_ScanTime);
            }
        }

        public bool SetCommTagVal(byte[] tmpBuff, int blkIdx)
        {
            string dType;
            string tdValue;

            DateTime dt = DateTime.Now;
            
            long CurTick = dt.Ticks;

            for (int i = 0; i < m_TagList.Count; i++)
            {
                if (m_TagList[i].d_CommBlkIdx == blkIdx)
                {
                    // Digital Tag
                    dType = m_TagList[i].DATATYPE;
                    switch (dType)
                    {
                        case "BOOL":
                        case "UINT8":
                        case "UBCD8":
                        case "USINT":
                        case "UINT16":
                        case "UBCD16":
                        case "UINT":
                        case "USHORT":
                            m_TagList[i]._RawVal = tmpBuff.Skip(m_TagList[i].d_DataPos).Take(2).ToArray();
                            
                            if(m_TagList[i].TAGID.Contains("."))
                            {
                                System.Collections.BitArray bitArr = new BitArray(m_TagList[i]._RawVal);

                                var index = int.Parse(m_TagList[i].TAGID.Split(".")[1]);

                                m_TagList[i]._ReadTagVal = bitArr[index] ? 1 : 0;
                            }
                            else
                            {
                                m_TagList[i]._ReadTagVal = BitConverter.ToUInt16(m_TagList[i]._RawVal, 0);
                            }
                            break;
                        case "BCD8":
                        case "INT8":
                        case "SINT":
                        case "INT16":
                        case "BCD16":
                        case "INT":
                        case "INTEGER":
                        case "SHORT":
                            m_TagList[i]._RawVal = tmpBuff.Skip(m_TagList[i].d_DataPos).Take(2).ToArray();
                            m_TagList[i]._ReadTagVal = BitConverter.ToInt16(m_TagList[i]._RawVal, 0);
                            break;
                        case "INT32":
                        case "BCD32":
                        case "DINT":
                            m_TagList[i]._RawVal = tmpBuff.Skip(m_TagList[i].d_DataPos).Take(4).ToArray();
                            m_TagList[i]._ReadTagVal = BitConverter.ToInt32(m_TagList[i]._RawVal, 0);
                            break;
                        case "UINT32":
                        case "UBCD32":
                        case "UDINT":
                            m_TagList[i]._RawVal = tmpBuff.Skip(m_TagList[i].d_DataPos).Take(4).ToArray();
                            m_TagList[i]._ReadTagVal = BitConverter.ToUInt32(m_TagList[i]._RawVal, 0);
                            break;
                        case "LONG":
                        case "INT64":
                        case "LINT":
                            m_TagList[i]._RawVal = tmpBuff.Skip(m_TagList[i].d_DataPos).Take(8).ToArray();
                            m_TagList[i]._ReadTagVal = BitConverter.ToInt64(m_TagList[i]._RawVal, 0);
                            break;
                        case "ULONG":
                        case "UINT64":
                        case "ULINT":
                            m_TagList[i]._RawVal = tmpBuff.Skip(m_TagList[i].d_DataPos).Take(8).ToArray();
                            m_TagList[i]._ReadTagVal = BitConverter.ToUInt64(m_TagList[i]._RawVal, 0);
                            break;
                        case "FLOAT":
                        case "REAL":
                            m_TagList[i]._RawVal = tmpBuff.Skip(m_TagList[i].d_DataPos).Take(4).ToArray();
                            m_TagList[i]._ReadTagVal = BitConverter.ToSingle(m_TagList[i]._RawVal, 0);
                            break;
                        case "DOUBLE":
                        case "LREAL":
                            m_TagList[i]._RawVal = tmpBuff.Skip(m_TagList[i].d_DataPos).Take(8).ToArray();
                            m_TagList[i]._ReadTagVal = BitConverter.ToDouble(m_TagList[i]._RawVal, 0);
                            break;
                        case "STRING":
                            m_TagList[i]._RawVal = tmpBuff.Skip(m_TagList[i].d_DataPos).Take(m_TagList[i].STRINGLEN).ToArray();
                            m_TagList[i]._ReadTagVal = Encoding.UTF8.GetString(m_TagList[i]._RawVal).Replace("\0", string.Empty);
                            break;
                        default:
                            continue;
                    }

                    m_TagList[i].GetScaleValue();

                    if (m_TagList[i]._TagScaleVal == null)
                        m_TagList[i].sTagScaleVal = string.Empty;
                    else 
                        m_TagList[i].sTagScaleVal = m_TagList[i]._TagScaleVal.ToString();

                    TagMasterData tdUpdate = (TagMasterData)m_htTagDef[m_TagList[i].TAGID];

                    if (tdUpdate != null)
                    {
                        switch (m_TagList[i].DATATYPE)
                        {
                            case "FLOAT":
                            case "REAL":
                                tdValue = string.Format("{0:F5}", m_TagList[i].AsFloat);
                                tdUpdate.sCurrentValue = tdValue;
                                break;
                            case "DOUBLE":
                            case "LREAL":
                                tdValue = string.Format("{0:F5}", m_TagList[i].AsDouble);
                                tdUpdate.sCurrentValue = tdValue;
                                break;
                            default:
                                tdUpdate.sCurrentValue = m_TagList[i].sTagVal;
                                break;
                        }
                    }

                    m_TagList[i].ReadTick = CurTick;

                    if (m_TagList[i]._BeforeTagVal.ToString() != m_TagList[i]._TagVal.ToString())
                    {
                        PLCUpdateValueToMonitoring(m_TagList[i]);
                    }
                }
            }
            return true;
        }

        public void PLCUpdateValueToMonitoring(CTagBase item)
        {
            Dictionary<string, string> sendData = new Dictionary<string, string>();

            sendData.Add("EQUIPMENTID", item.EQUIPMENTID);
            sendData.Add("STATIONID", item.STATIONID);
            sendData.Add("PARAMETERID", item.PARAMETERID);
            sendData.Add("TAGID", item.TAGID);
            sendData.Add("BEFOREVALUE", item._BeforeTagVal.ToString());
            sendData.Add("VALUE", item.sTagVal);
            sendData.Add("SCALEVALUE", item.sTagScaleVal);

            if (string.IsNullOrEmpty(item.PARAMETERID) == false && item.PARAMETERID.Substring(0,6).Contains("common"))
            {
                MessageHandler.SendMessageAsyncToMonitoring("PLCUpdateData", Newtonsoft.Json.JsonConvert.SerializeObject(sendData));
            }

            MessageHandler.SendMessageAsyncToMonitoringECS("PLCUpdataDataToECS", Newtonsoft.Json.JsonConvert.SerializeObject(sendData));

            item.MonitoringSendTick = DateTime.Now.Ticks;
        }

        public void CommStart() 
        {
            commEnable = true;

            MakeHashTagList();

            if (ctsConnectCheck == null)
            {
                ctsConnectCheck = new CancellationTokenSource();
                ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectionCheck), ctsConnectCheck.Token);
            }

            if (ctsHeartBit == null)
            {
                ctsHeartBit = new CancellationTokenSource();
                ThreadPool.QueueUserWorkItem(new WaitCallback(HeartBit), ctsHeartBit.Token);
            }

            if (ctsRead == null)
            {
                ctsRead = new CancellationTokenSource();
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReadBlockProcess), ctsRead.Token);
            }

            if (EQUIPMENTID.Contains("RCS"))
            {
                if (ctsFromRCSConnectionRequest == null)
                {
                    ctsFromRCSConnectionRequest = new CancellationTokenSource();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(RCSConnectionRequest), ctsFromRCSConnectionRequest.Token);
                }

                if (ctsRCSDisconnectRequest == null)
                {
                    ctsRCSDisconnectRequest = new CancellationTokenSource();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(RCSDisconnectRequest), ctsRCSDisconnectRequest.Token);
                }

                if (ctsVMSScenarioAutoChangeEvent == null)
                {
                    ctsVMSScenarioAutoChangeEvent = new CancellationTokenSource();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(VMSScenarioAutoChangeEvent), ctsVMSScenarioAutoChangeEvent.Token);
                }
            }
            /*if (ctsWrite == null)
            {
                ctsWrite = new CancellationTokenSource();
                ThreadPool.QueueUserWorkItem(new WaitCallback(WriteBlockProcess), ctsWrite.Token);
            }*/
        }

        public void CommEnd()
        {
            commEnable = false;

            if(ctsRead != null)
                ctsRead.Cancel();

            if(ctsWrite != null)
                ctsWrite.Cancel();

            if (ctsHeartBit != null)
                ctsHeartBit.Cancel();

            if(ctsConnectCheck != null)
                ctsConnectCheck.Cancel();

            if (EQUIPMENTID.Contains("RCS"))
            {
                if (ctsFromRCSConnectionRequest != null)
                    ctsFromRCSConnectionRequest.Cancel();
                if (ctsVMSScenarioAutoChangeEvent != null)
                    ctsVMSScenarioAutoChangeEvent.Cancel();
                if (ctsRCSDisconnectRequest != null)
                    ctsRCSDisconnectRequest.Cancel();
            }
                

            Thread.Sleep(2500);
            
            ctsRead.Dispose();
            ctsRead = null;
            /*ctsWrite.Dispose();
            ctsWrite = null;*/
            ctsHeartBit.Dispose();
            ctsHeartBit = null;
            ctsConnectCheck.Dispose();
            ctsConnectCheck = null;

            if (EQUIPMENTID.Contains("RCS"))
            {
                ctsFromRCSConnectionRequest.Dispose();
                ctsFromRCSConnectionRequest = null;
                ctsVMSScenarioAutoChangeEvent.Dispose();
                ctsVMSScenarioAutoChangeEvent = null;
                ctsRCSDisconnectRequest.Dispose();
                ctsRCSDisconnectRequest = null;
            }
        }
    }
}

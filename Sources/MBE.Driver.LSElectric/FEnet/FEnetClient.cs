using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MBE.Driver.Common;
using MBE.Driver.Common.Channels;
using MBE.Driver.Common.Logging;
using MBE.Driver.LSElectric.FeNet.Logging;

namespace MBE.Driver.LSElectric.FEnet
{
    public class FEnetClient : IDisposable
    {
        private ushort invokeID = 0;
        private readonly Dictionary<ushort, ResponseWaitHandle> responseWaitHandles = new Dictionary<ushort, ResponseWaitHandle>();
        private bool isReceiving = false;
        private readonly List<byte> errorBuffer = new List<byte>();
        private readonly object lockSerialize = new object();
        private readonly object lockReceive = new object();

        class ResponseWaitHandle : EventWaitHandle
        {
            public ResponseWaitHandle(List<byte> buffer, FEnetRequest request, int timeout) : base(false, EventResetMode.ManualReset)
            {
                ResponseBuffer = buffer;
                Request = request;
                Timeout = timeout;
            }

            public List<byte> ResponseBuffer { get; }

            public FEnetRequest Request { get; }

            public int Timeout { get; }

            public FEnetResponse Response { get; set; }
        }
        public FEnetClient() { }

        public FEnetClient(IChannel channel)
        {
            this.channel = channel;
        }

        public void Dispose()
        {
            channel?.Dispose();
        }

        private IChannel channel;

        public IChannel Channel
        {
            get => channel;
            set
            {
                if (channel != value)
                {
                    channel = value;
                }
            }
        }

        public string CompanyID { get; set; } = "LSIS-XGT";

        public bool UseChecksum { get; set; } = true;

        public int Timeout { get; set; } = 1000;

        public bool ThrowsExceptionFromNAK { get; set; } = true;

        /// <summary>
        /// 비트 변수의 인덱스를 16진수로 통신할지 여부를 결정.
        /// P, M, L, K, F 이면서 Bit일 경우 16진수로 전송.
        /// 그 외에는 인덱스가 .으로 나누어져있고 Bit일 경우 마지막 자리만 16진수로 전송.
        /// XGB PLC에서 비트를 읽거나 쓸 때 엉뚱한 비트가 읽히거나 쓰인다면 true로 설정해서 테스트
        /// </summary>
        public bool UseHexBitIndex { get; set; }

        public void Open()
        {
            if (channel == null)
                throw new ArgumentNullException(nameof(Channel));
            channel.Open();
        }

        public FEnetResponse Request(FEnetRequest request) => Request(Timeout, request);

        public FEnetResponse Request(int timeout, FEnetRequest request)
        {
            //Debug.WriteLine($"FEnetClient.Request({Thread.CurrentThread.ManagedThreadId}:+)");

            Channel channel = (Channel as Channel); //?? (Channel as ChannelProvider)?.PrimaryChannel;

            if (channel == null)
                throw new ArgumentNullException(nameof(Channel));

            byte[] requestMessage;
            FEnetRequestLog requestLog;
            FEnetResponse result;
            List<byte> buffer = new List<byte>();

            lock (lockSerialize)
            {
                request = (FEnetRequest)request.Clone();
                if (request.InvokeID == null)
                    request.InvokeID = invokeID++;
                if (request.UseHexBitIndex == null)
                    request.UseHexBitIndex = UseHexBitIndex;
                requestMessage = request.Serialize(CompanyID, UseChecksum).ToArray();
            }

            try
            {
                if (responseWaitHandles.TryGetValue(request.InvokeID.Value, out var oldHandle))
                    oldHandle.WaitOne(timeout);

                ResponseWaitHandle responseWaitHandle;
                lock (responseWaitHandles)
                    responseWaitHandle = responseWaitHandles[request.InvokeID.Value] = new ResponseWaitHandle(buffer, request, timeout);

                channel.Write(requestMessage);
                requestLog = new FEnetRequestLog(channel, request, requestMessage);
                channel?.Logger?.Log(requestLog);

                RunReceive(channel);

                responseWaitHandle.WaitOne(timeout);

                result = responseWaitHandle.Response;
                lock (responseWaitHandles)
                    responseWaitHandles.Remove(request.InvokeID.Value);
                if (result == null)
                    result = new FEnetCommErrorResponse(FEnetCommErrorCode.ResponseTimeout, new byte[0], request, 0, 0);
            }
            catch (Exception ex)
            {
                channel?.Logger?.Log(new ChannelErrorLog(channel, ex.InnerException ?? ex));
                throw ex.InnerException ?? ex;
            }

            if (result is FEnetCommErrorResponse commErrorResponse)
            {
                var ex = new RequestException<FEnetCommErrorCode>(commErrorResponse.ErrorCode, commErrorResponse.ReceivedBytes, commErrorResponse.Request);
                channel?.Logger?.Log(new ChannelErrorLog(channel, ex));
                throw ex;
            }

            if (result is FEnetNAKResponse exceptionResponse)
            {
                channel?.Logger?.Log(new FEnetNAKLog(channel, exceptionResponse, buffer.ToArray(), requestLog));
                if (ThrowsExceptionFromNAK)
                    throw new FEnetNAKException(exceptionResponse.NAKCode, exceptionResponse.NAKCodeValue);
            }
            else
                channel?.Logger?.Log(new FEnetResponseLog(channel, result, result is FEnetNAKResponse ? null : buffer.ToArray(), requestLog));


            //Debug.WriteLine($"FEnetClient.Request({Thread.CurrentThread.ManagedThreadId}:-)");

            return result;
        }

        #region  Read Fucntion

        public IReadOnlyDictionary<DeviceVariable, DeviceValue> Read(DeviceVariable deviceVariable, params DeviceVariable[] moreDeviceVariables)
            => Read(Timeout, deviceVariable, moreDeviceVariables);

        public IReadOnlyDictionary<DeviceVariable, DeviceValue> Read(int timeout, DeviceVariable deviceVariable, params DeviceVariable[] moreDeviceVariables)
            => Read(timeout, new DeviceVariable[] { deviceVariable }.Concat(moreDeviceVariables));

        public IReadOnlyDictionary<DeviceVariable, DeviceValue> Read(IEnumerable<DeviceVariable> deviceVariables)
            => Read(Timeout, deviceVariables);

        public IReadOnlyDictionary<DeviceVariable, DeviceValue> Read(int timeout, IEnumerable<DeviceVariable> deviceVariables)
        {
            if (deviceVariables == null) throw new ArgumentNullException(nameof(deviceVariables));
            if (deviceVariables.Count() == 0) throw new ArgumentException(nameof(deviceVariables));

            var response = Request(timeout, new FEnetReadIndividualRequest(deviceVariables.First().DataType, deviceVariables));
            if (response is FEnetReadIndividualResponse readResponse)
                return readResponse;
            else if (response is FEnetNAKResponse exceptionResponse)
                throw new FEnetNAKException(exceptionResponse.NAKCode, exceptionResponse.NAKCodeValue);
            return null;
        }

        public IDeviceDataBlock Read(DeviceType deviceType, uint index, int count) => Read(Timeout, deviceType, index, count);

        public IDeviceDataBlock Read(int timeout, DeviceType deviceType, uint index, int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));

            var response = Request(timeout, new FEnetReadContinuousRequest(deviceType, index, count));
            if (response is FEnetReadContinuousResponse readResponse)
                return readResponse;
            else if (response is FEnetNAKResponse exceptionResponse)
                throw new FEnetNAKException(exceptionResponse.NAKCode, exceptionResponse.NAKCodeValue);
            return null;
        }

        public IDeviceDataBlock Read(DeviceVariable deviceVariable, int count) => Read(Timeout, deviceVariable, count);

        public IDeviceDataBlock Read(int timeout, DeviceVariable deviceVariable, int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));

            var response = Request(timeout, new FEnetReadContinuousRequest(deviceVariable, count));
            if (response is FEnetReadContinuousResponse readResponse)
                return readResponse;
            else if (response is FEnetNAKResponse exceptionResponse)
                throw new FEnetNAKException(exceptionResponse.NAKCode, exceptionResponse.NAKCodeValue);
            return null;
        }


        #endregion

        #region Write Function
        public void Write(DeviceVariable deviceVariable, DeviceValue deviceValue) => Write(Timeout, deviceVariable, deviceValue);

        public void Write(int timeout, DeviceVariable deviceVariable, DeviceValue deviceValue)
            => Write(timeout, new KeyValuePair<DeviceVariable, DeviceValue>[] { new KeyValuePair<DeviceVariable, DeviceValue>(deviceVariable, deviceValue) });

        public void Write((DeviceVariable, DeviceValue) valueTuple, params (DeviceVariable, DeviceValue)[] moreValueTuples)
            => Write(Timeout, new (DeviceVariable, DeviceValue)[] { valueTuple }.Concat(moreValueTuples).Select(item => new KeyValuePair<DeviceVariable, DeviceValue>(item.Item1, item.Item2)));

        public void Write(int timeout, (DeviceVariable, DeviceValue) valueTuple, params (DeviceVariable, DeviceValue)[] moreValueTuples)
            => Write(timeout, new (DeviceVariable, DeviceValue)[] { valueTuple }.Concat(moreValueTuples).Select(item => new KeyValuePair<DeviceVariable, DeviceValue>(item.Item1, item.Item2)));

        public void Write(IEnumerable<KeyValuePair<DeviceVariable, DeviceValue>> values) => Write(Timeout, values);

        public void Write(int timeout, IEnumerable<KeyValuePair<DeviceVariable, DeviceValue>> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Count() == 0) throw new ArgumentOutOfRangeException(nameof(values));

            var response = Request(timeout, new FEnetWriteSingleRequest(values.First().Key.DataType, values));
            if (response is FEnetNAKResponse exceptionResponse)
                throw new FEnetNAKException(exceptionResponse.NAKCode, exceptionResponse.NAKCodeValue);
        }

        public void Write(DeviceType deviceType, uint index, byte deviceValue, params byte[] moreDeviceValues)
            => Write(Timeout, deviceType, index, deviceValue, moreDeviceValues);

        public void Write(int timeout, DeviceType deviceType, uint index, byte deviceValue, params byte[] moreDeviceValues)
            => Write(timeout, deviceType, index, new byte[] { deviceValue }.Concat(moreDeviceValues));

        public void Write(DeviceType deviceType, uint index, IEnumerable<byte> deviceValues)
            => Write(Timeout, deviceType, index, deviceValues);

        public void Write(int timeout, DeviceType deviceType, uint index, IEnumerable<byte> deviceValues)
        {
            var response = Request(timeout, new FEnetWriteContinuousRequest(deviceType, index, deviceValues));
            if (response is FEnetNAKResponse exceptionResponse)
                throw new FEnetNAKException(exceptionResponse.NAKCode, exceptionResponse.NAKCodeValue);
        }


        #endregion

        private void RunReceive(Channel channel)
        {
            lock (lockReceive)
            {
                if (isReceiving) return;
                    isReceiving = true;

                Task.Factory.StartNew(() =>
                {
                    //Debug.WriteLine($"FEnetClient.RunReceive({Thread.CurrentThread.ManagedThreadId}:+)");
                    try
                    {
                        var buffer = new List<byte>();

                        while (true)
                        {
                            lock (responseWaitHandles)
                                if (responseWaitHandles.Count == 0)
                                {
                                    errorBuffer.AddRange(buffer);
                                    if (errorBuffer.Count > 0)
                                        channel?.Logger?.Log(new UnrecognizedErrorLog(channel, errorBuffer.ToArray()));
                                    errorBuffer.Clear();
                                    break;
                                }

                            if (errorBuffer.Count >= 256)
                            {
                                channel?.Logger?.Log(new UnrecognizedErrorLog(channel, errorBuffer.ToArray()));
                                errorBuffer.Clear();
                            }

                            if (buffer.Count < 20)
                                buffer.AddRange(channel.Read((uint)(20 - buffer.Count), 0));

                            if (Encoding.ASCII.GetString(buffer.Take(10).ToArray()).TrimEnd('\0') != CompanyID?.TrimEnd('\0'))
                            {
                                errorBuffer.Add(buffer[0]);
                                buffer.RemoveAt(0);
                                continue;
                            }

                            if (buffer[13] != 0x11)
                            {
                                errorBuffer.AddRange(buffer.Take(14));
                                buffer.RemoveRange(0, 14);
                                continue;
                            }
                            ushort invokeID;
                            ResponseWaitHandle responseWaitHandle;

                            invokeID = (ushort)(buffer[14] | (buffer[15] << 8));
                            lock (responseWaitHandles)
                                responseWaitHandles.TryGetValue(invokeID, out responseWaitHandle);

                            if (responseWaitHandle == null)
                            {
                                errorBuffer.AddRange(buffer.Take(15));
                                buffer.RemoveRange(0, 15);
                                continue;
                            }

                            var plcInfo = (ushort)(buffer[10] | (buffer[11] << 8));
                            var ethernetModuleInfo = buffer[18];

                            var request = responseWaitHandle.Request;

                            if (UseChecksum && buffer[19] != 0 && buffer[19] != buffer.Take(19).Sum(b => b) % 256)
                            {
                                responseWaitHandle.Response = new FEnetCommErrorResponse(FEnetCommErrorCode.ErrorChecksum, buffer, request, plcInfo, ethernetModuleInfo);
                                buffer.Clear();
                                continue;
                            }


                            FEnetResponse result;

                            buffer.AddRange(channel.Read(2, 0));

                            var commandValue = (ushort)((buffer[20] | (buffer[21] << 8)) - 1);
                            if (!Enum.IsDefined(typeof(FEnetCommand), commandValue))
                            {
                                responseWaitHandle.Response = new FEnetCommErrorResponse(FEnetCommErrorCode.ResponseCommandDoNotMatch, buffer, request, plcInfo, ethernetModuleInfo);
                                buffer.Clear();
                                continue;
                            }
                            var command = (FEnetCommand)commandValue;
                            if (request.Command != command)
                            {
                                responseWaitHandle.Response = new FEnetCommErrorResponse(FEnetCommErrorCode.ResponseCommandDoNotMatch, buffer, request, plcInfo, ethernetModuleInfo);
                                buffer.Clear();
                                continue;
                            }

                            buffer.AddRange(channel.Read(2, 0));

                            var dataTypeValue = (ushort)(buffer[22] | (buffer[23] << 8));
                            if (!Enum.IsDefined(typeof(FEnetDataType), dataTypeValue))
                            {
                                responseWaitHandle.Response = new FEnetCommErrorResponse(FEnetCommErrorCode.ResponseDataTypeDoNotMatch, buffer, request, plcInfo, ethernetModuleInfo);
                                buffer.Clear();
                                continue;
                            }
                            var dataType = (FEnetDataType)dataTypeValue;
                            if (request.DataType != dataType)
                            {
                                responseWaitHandle.Response = new FEnetCommErrorResponse(FEnetCommErrorCode.ResponseDataTypeDoNotMatch, buffer, request, plcInfo, ethernetModuleInfo);
                                buffer.Clear();
                                continue;
                            }

                            buffer.AddRange(channel.Read(4, 0));

                            if (buffer[26] != 0 || buffer[27] != 0)
                            {
                                buffer.AddRange(channel.Read(2, 0));
                                result = new FEnetNAKResponse((ushort)(buffer[28] | (buffer[29] << 8)), request, plcInfo, ethernetModuleInfo);
                            }
                            else
                            {
                                buffer.AddRange(channel.Read(2, 0));
                                if (request.BlockCount != (buffer[28] | (buffer[29] << 8)))
                                    result = new FEnetCommErrorResponse(FEnetCommErrorCode.ResponseDataBlockCountDoNotMatch, buffer, request, plcInfo, ethernetModuleInfo);
                                else if (request.Command == FEnetCommand.Write)
                                {
                                    result = new FEnetWriteResponse(request as FEnetWriteRequest, plcInfo, ethernetModuleInfo);
                                }
                                else
                                {
                                    switch (request.DataType)
                                    {
                                        case FEnetDataType.Continuous:
                                            result = DeserializeContinuousDataResponse(channel, buffer, request, plcInfo, ethernetModuleInfo, out var bytes) ?? new FEnetReadContinuousResponse(bytes, request as FEnetReadContinuousRequest, plcInfo, ethernetModuleInfo);
                                            break;
                                        default:
                                            result = DeserializeIndividualDataResponse(channel, buffer, request, plcInfo, ethernetModuleInfo, out var deviceValues) ?? new FEnetReadIndividualResponse(deviceValues, request as FEnetReadIndividualRequest, plcInfo, ethernetModuleInfo);
                                            break;
                                    }
                                }
                            }

                            if (result is FEnetCommErrorResponse responseCommErrorMessage)
                            {
                                buffer.Clear();
                                responseWaitHandle.Response = result;
                                continue;
                            }

                            if (buffer.Count - 20 != (buffer[16] | (buffer[17] << 8)))
                            {
                                responseWaitHandle.Response = new FEnetCommErrorResponse(FEnetCommErrorCode.ResponseLengthDoNotMatch, buffer, request, plcInfo, ethernetModuleInfo);
                                buffer.Clear();
                                continue;
                            }

                            if (errorBuffer.Count > 0)
                            {
                                channel?.Logger?.Log(new UnrecognizedErrorLog(channel, errorBuffer.ToArray()));
                                errorBuffer.Clear();
                            }

                            responseWaitHandle.ResponseBuffer.AddRange(buffer);

                            buffer.Clear();
                            responseWaitHandle.Response = result;
                            responseWaitHandle.Set();
                        }
                    }
                    catch
                    {
                    }
                    isReceiving = false;

                    //Debug.WriteLine($"FEnetClient.RunReceive({Thread.CurrentThread.ManagedThreadId}:-)");
                }, TaskCreationOptions.LongRunning);
            }
        }

        private FEnetResponse DeserializeIndividualDataResponse(Channel channel, List<byte> buffer, FEnetRequest request, ushort plcInfo, byte ethernetModuleInfo, out DeviceValue[] deviceValues)
        {
            var blockCount = request.BlockCount;
            deviceValues = new DeviceValue[blockCount];

            for (int i = 0; i < blockCount; i++)
            {
                var dataCount = FEnetMessage.ReadWord(channel, buffer);

                if (dataCount > 8)
                    return new FEnetCommErrorResponse(FEnetCommErrorCode.ErrorDataCount, buffer, request, plcInfo, ethernetModuleInfo);

                ulong value = 0;
                for (int j = 0; j < dataCount; j++)
                {
                    byte b = channel.Read(0);
                    buffer.Add(b);

                    value |= (ulong)b << (8 * j);
                }

                deviceValues[i] = new DeviceValue(value);
            }

            return null;
        }

        private FEnetResponse DeserializeContinuousDataResponse(Channel channel, List<byte> buffer, FEnetRequest request, ushort plcInfo, byte ethernetModuleInfo, out byte[] bytes)
        {
            var continuousAccessRequest = (IContinuousAccessRequest)request;
            bytes = null;

            double dataUnit;
            switch (continuousAccessRequest.StartDeviceVariable.DataType)
            {
                case DataType.Bit:
                    dataUnit = 1d / 8;
                    break;
                case DataType.Word:
                    dataUnit = 2;
                    break;
                case DataType.DoubleWord:
                    dataUnit = 4;
                    break;
                case DataType.LongWord:
                    dataUnit = 8;
                    break;
                default:
                    dataUnit = 1;
                    break;
            }

            var dataCount = FEnetMessage.ReadWord(channel, buffer);
            if (dataCount != dataUnit * continuousAccessRequest.Count)
                return new FEnetCommErrorResponse(FEnetCommErrorCode.ResponseDataCountNotMatch, buffer, request, plcInfo, ethernetModuleInfo);

            bytes = channel.Read(dataCount, 0).ToArray();
            buffer.AddRange(bytes);

            return null;
        }

        class FEnetCommErrorResponse : FEnetResponse
        {
            public FEnetCommErrorResponse(FEnetCommErrorCode errorCode, IEnumerable<byte> receivedMessage, FEnetRequest request, ushort plcInfo, byte ethernetModuleInfo) : base(request, plcInfo, ethernetModuleInfo)
            {
                ErrorCode = errorCode;
                ReceivedBytes = receivedMessage?.ToArray() ?? new byte[0];
            }

            public FEnetCommErrorCode ErrorCode { get; }
            public IReadOnlyList<byte> ReceivedBytes { get; }

            public override IEnumerable<byte> Serialize(string companyID, bool useChecksum)
            {
                return ReceivedBytes;
            }

            protected override IEnumerable<byte> OnCreateDataFrame()
            {
                yield break;
            }

            public override string ToString()
            {
                string errorName = ErrorCode.ToString();

                if (ReceivedBytes != null && ReceivedBytes.Count > 0)
                    return $"{errorName}: {BitConverter.ToString(ReceivedBytes as byte[])}";
                else
                    return errorName;
            }
        }
    }
}

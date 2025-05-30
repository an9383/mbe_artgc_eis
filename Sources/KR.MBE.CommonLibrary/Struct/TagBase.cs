using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using KR.MBE.CommonLibrary.Interface;

namespace KR.MBE.CommonLibrary.Struct
{
    public class CTagBase
    {
        public int    d_TagIdx;
        public int    d_ByteSize;
        public int    d_Address;          // Function Code가 없는 Address
        public int    d_DataPos;
        public int    d_BitPos;
        public string d_DevCode;          // P, M, L, K, F, T, C, D, N, R
        public string d_DevType;          // X : Bit, B : Byte, W : Word, D : Double Word
        //public int    d_BlockType;        // P=0, M=1, L=2, K=3, F=4, T=5, C=6, D=7, N=8, R=9
        public int    d_CommBlkIdx;       //  통신블럭 Index


        public string EQUIPMENTID;
        public string TAGID;
        public string TAGNAME;
        public string DESCRIPTION;
        public string USEFLAG;         // Yes, No
        public string TAGKIND;         // Real, Virtual
        public string TAGTYPE;         // Digital, Analog, String

        public string DRIVERID;
        public string DRIVERNAME;
        public string PARAMETERID;

        public string STATIONID;
        //public string STATIONNAME;

        public string ADDRESS;
        public string DATATYPE;          // ANALOG TAG인 경우만 적용 (INTEGER,FLOAT 등)
                                         // INT8, UINT8, INT16, UINT16, INT32, UINT32, FLOAT, INT64, UINT64, DOUBLE
        public string EVENTFLAG;         // YES/NO
        public string PARAMETERFLAG;     // YES/NO

        public int    STRINGLEN;
        public string WRITEUSE;          // Yes, No
        public string SCALEUSE;          // Yes, No
        public double SCALEVALUE;        // ReadValue * ScaleValue = TagValue
        public double OFFSET;            // ANALOG TAG인 경우만 적용, DEFAULT = 0
        public double PLCMIN;
        public double PLCMAX;
        public double TAGMIN;
        public double TAGMAX;
        public object TAGVALUE;
        public object SETVALUE;
        public object MINVALUE;
        public object MAXVALUE;

        public string sTagVal;            // To ECS value
        public string sTagScaleVal;            // To ECS value

        public int    INTERVAL;
        public string UNIT;
        public string ALARMFLAG;          // YES/NO
        public string HEARTBEATFLAG;     // YES/NO
        public string MONITORINGFLAG;     // YES/NO
        public string COSFLAG;
        public string COSPARAMETERNAME;

        public bool IsSet
        { 
            get { return sTagVal != "0"; }
        }

        public void Set()
        {
            _TagVal = "1";
        }

        public void Unset()
        {
            _TagVal = "0";
        }



        public bool IsHeartbeat
        {
            get { return HEARTBEATFLAG == "YES" ? true : false; }
        }

        public bool IsMonitoring
        {
            get { return MONITORINGFLAG == "YES" ? true : false; }
        }
        public bool IsAlarm
        {
            get { return ALARMFLAG == "YES" ? true : false; }
        }

        public bool IsCos
        {
            get { return COSFLAG == "YES" ? true : false; }
        }
        public bool IsWriteable
        {
            get { return WRITEUSE == "YES" ? true : false; }
        }
        public bool IsScaleUse
        {
            get { return SCALEUSE == "YES" ? true : false; }
        }

        public double _SetVal;
        private object tagVal;            // real value
        public object _TagScaleVal;
        public object _BeforeTagVal;
        public string BeforeStringVal;

        private byte[] rawVal;
        public byte[] _BeforeRawVal;

        public double _LowVal;
        public double _HighVal;

        public long _Interval;         // 100 ns 단위 시간 1 sec = 10,000,000 ns
        public long _CurTick;          // 100 ns 단위 시간
        public long _LastTick;         // 100 ns 단위 시간

        public string d_ConfirmType;

        public long MonitoringInterval;         // 100 ns 단위 시간 1 sec = 10,000,000 ns
        public long MonitoringCurTick;          // 100 ns 단위 시간
        public long MonitoringLastTick;         // 100 ns 단위 시간
        public long MonitoringSendTick;

        public long ReadTick;                   // 100 ns 단위 시간
        public long SendTick;                   // 100 ns 단위 시간
        public long UpdateTick;                   // 100 ns 단위 시간
        

        public PlcInterface station;


        public void RawValueChanged(byte[] changeValue)
        {
            try
            {
                _BeforeRawVal = rawVal;

                rawVal = changeValue;

                if (_BeforeRawVal == null)
                    _BeforeRawVal = rawVal;
            }
            catch (Exception e)
            {

            }
        }

        public void ValueChanged(object changeValue)
        {
            try
            {
                _BeforeTagVal = tagVal;

                tagVal = changeValue;

                if (tagVal != null)
                {
                    sTagVal = tagVal.ToString();
                    _RawVal = Encoding.UTF8.GetBytes(sTagVal);
                }

                if (_BeforeTagVal == null)
                    _BeforeTagVal = tagVal;

                if (IsWriteable)
                {
                    if(ADDRESS != "MW2050")
                    {
                        int test = 0;
                    }

                    station.WriteTag(TAGID, sTagVal);
                }

                UpdateTick = DateTime.Now.Ticks;
            }
            catch (Exception e)
            {

            }
        }

        public void ReadValueChanged(object changeValue)
        {
            try
            {
                _BeforeTagVal = tagVal;

                tagVal = changeValue;

                if (tagVal != null)
                {
                    sTagVal = tagVal.ToString();
                    _RawVal = Encoding.UTF8.GetBytes(sTagVal);
                }

                if (_BeforeTagVal == null)
                    _BeforeTagVal = tagVal;
                
                UpdateTick = DateTime.Now.Ticks;
            }
            catch (Exception e)
            {

            }
        }

        public byte[] _RawVal
        {
            get
            {
                return rawVal;
            }
            set
            {
                if (rawVal != value)
                {
                    RawValueChanged(value);
                }
            }
        }

        public object _TagVal
        {
            get
            {
                return tagVal;
            }
            set
            {
                if (tagVal != value)
                {
                    ValueChanged(value);
                }
            }
        }

        public object _ReadTagVal
        {
            get
            {
                return tagVal;
            }
            set
            {
                if (tagVal != value)
                {
                    ReadValueChanged(value);
                }
            }
        }

        public ushort? AsUshort
        {
            get
            {
                try
                {
                    return (ushort)Convert.ChangeType(tagVal, typeof(ushort));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagVal = value;
                }
                catch
                {

                }
            }
        }

        public ushort? AsUshortScale
        {
            get
            {
                try
                {
                    return (ushort)Convert.ChangeType(_TagScaleVal, typeof(ushort));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagScaleVal = value;

                    if (_TagScaleVal != null)
                    {
                        sTagScaleVal = _TagScaleVal.ToString();
                    }
                }
                catch
                {

                }
            }
        }

        public short? AsShort
        {
            get
            {
                try
                {
                    return (short)Convert.ChangeType(tagVal, typeof(short));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagVal = value;
                }
                catch
                {

                }
            }
        }

        public short? AsShortScale
        {
            get
            {
                try
                {
                    return (short)Convert.ChangeType(_TagScaleVal, typeof(short));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagScaleVal = value;

                    if (_TagScaleVal != null)
                    {
                        sTagScaleVal = _TagScaleVal.ToString();
                    }
                }
                catch
                {

                }
            }
        }

        public int? AsInt
        {
            get
            {
                try
                {
                    return (int)Convert.ChangeType(tagVal, typeof(int));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagVal = value;
                }
                catch
                {

                }
            }
        }
        public int? AsIntScale
        {
            get
            {
                try
                {
                    return (int)Convert.ChangeType(_TagScaleVal, typeof(int));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagScaleVal = value;

                    if (_TagScaleVal != null)
                    {
                        sTagScaleVal = _TagScaleVal.ToString();
                    }
                }
                catch
                {

                }
            }
        }

        public uint? AsUint
        {
            get
            {
                try
                {
                    return (uint)Convert.ChangeType(tagVal, typeof(uint));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagVal = value;
                }
                catch
                {

                }
            }
        }
        public uint? AsUintScale
        {
            get
            {
                try
                {
                    return (uint)Convert.ChangeType(_TagScaleVal, typeof(uint));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagScaleVal = value;

                    if (_TagScaleVal != null)
                    {
                        sTagScaleVal = _TagScaleVal.ToString();
                    }
                }
                catch
                {

                }
            }
        }

        public long? AsLong
        {
            get
            {
                try
                {
                    return (long)Convert.ChangeType(tagVal, typeof(long));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagVal = value;
                }
                catch
                {

                }
            }
        }
        public long? AsLongScale
        {
            get
            {
                try
                {
                    return (long)Convert.ChangeType(_TagScaleVal, typeof(long));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagScaleVal = value;

                    if (_TagScaleVal != null)
                    {
                        sTagScaleVal = _TagScaleVal.ToString();
                    }
                }
                catch
                {

                }
            }
        }

        public ulong? AsUlong
        {
            get
            {
                try
                {
                    return (ulong)Convert.ChangeType(tagVal, typeof(ulong));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagVal = value;
                }
                catch
                {

                }
            }
        }

        public ulong? AsUlongScale
        {
            get
            {
                try
                {
                    return (ulong)Convert.ChangeType(_TagScaleVal, typeof(ulong));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagScaleVal = value;

                    if (_TagScaleVal != null)
                    {
                        sTagScaleVal = _TagScaleVal.ToString();
                    }
                }
                catch
                {

                }
            }
        }

        public float? AsFloat
        {
            get
            {
                try
                {
                    return (float)Convert.ChangeType(tagVal, typeof(float));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagVal = value;
                }
                catch
                {

                }
            }
        }
        public float? AsFloatScale
        {
            get
            {
                try
                {
                    return (float)Convert.ChangeType(_TagScaleVal, typeof(float));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagScaleVal = value;

                    if (_TagScaleVal != null)
                    {
                        sTagScaleVal = _TagScaleVal.ToString();
                    }
                }
                catch
                {

                }
            }
        }

        public double? AsDouble
        {
            get
            {
                try
                {
                    return (double)Convert.ChangeType(tagVal, typeof(double));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagVal = value;
                }
                catch
                {

                }
            }
        }
        public double? AsDoubleScale
        {
            get
            {
                try
                {
                    return (double)Convert.ChangeType(_TagScaleVal, typeof(double));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _TagScaleVal = value;

                    if (_TagScaleVal != null)
                    {
                        sTagScaleVal = _TagScaleVal.ToString();
                    }
                }
                catch
                {

                }
            }
        }

        public bool? AsBool
        {
            get
            {
                if(tagVal == null)
                {
                    return null;
                }

                try
                {
                    return (bool)Convert.ChangeType(tagVal, typeof(bool));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    if (value == true)
                    {
                        _TagVal = 1;
                    }
                    else
                    {
                        _TagVal = 0;
                    }
                }
                catch
                {

                }
            }
        }

        public string AsString
        {
            get
            {
                try
                {
                    return (string)Convert.ChangeType(tagVal, typeof(string));
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    _TagVal = value;
                }
                catch
                {

                }
            }
        }

        public void GetScaleValue()
        {
            

            if (IsScaleUse)
            {
                switch (DATATYPE)
                {
                    case "UINT8":
                    case "UBCD8":
                    case "UINT16":
                    case "UBCD16":
                    case "USINT":
                    case "UINT":
                    case "USHORT":
                        _TagScaleVal = (AsUshort * SCALEVALUE) + OFFSET;
                        break;
                    case "BCD8":
                    case "INT8":
                    case "INT16":
                    case "BCD16":
                    case "INT":
                    case "SINT":
                    case "INTEGER":
                        _TagScaleVal = (AsShort * SCALEVALUE) + OFFSET;
                        break;
                    case "INT32":
                    case "BCD32":
                    case "DINT":
                        _TagScaleVal = (AsInt * SCALEVALUE) + OFFSET;
                        break;
                    case "UINT32":
                    case "UBCD32":
                    case "UDINT":
                        _TagScaleVal = (AsUint * SCALEVALUE) + OFFSET;
                        break;
                    case "INT64":
                    case "LINT":
                        _TagScaleVal = (AsLong * SCALEVALUE) + OFFSET;
                        break;
                    case "UINT64":
                    case "ULINT":
                        _TagScaleVal = (AsUlong * SCALEVALUE) + OFFSET;
                        break;
                    case "FLOAT":
                    case "REAL":
                        _TagScaleVal = (AsFloat * SCALEVALUE) + OFFSET;
                        break;
                    case "DOUBLE":
                    case "LREAL":
                        _TagScaleVal = (AsDouble * SCALEVALUE) + OFFSET;
                        break;
                }
            }
            else
            {
                switch (DATATYPE)
                {
                    case "UINT8":
                    case "UBCD8":
                    case "UINT16":
                    case "UBCD16":
                    case "USINT":
                    case "UINT":
                    case "USHORT":
                        if (TAGMIN != PLCMIN || TAGMAX != PLCMAX)
                        {
                            var fScale = (TAGMAX - TAGMIN) / (PLCMAX - PLCMIN);

                            _TagScaleVal = ((AsUshort - PLCMIN) * fScale) + TAGMIN;
                        }
                        else
                        {
                            _TagScaleVal = AsUshort;
                        }
                        break;
                    case "BCD8":
                    case "INT8":
                    case "INT16":
                    case "BCD16":
                    case "INT":
                    case "SINT":
                    case "INTEGER":
                        if (TAGMIN != PLCMIN || TAGMAX != PLCMAX)
                        {
                            var fScale = (TAGMAX - TAGMIN) / (PLCMAX - PLCMIN);

                            _TagScaleVal = ((AsShort - PLCMIN) * fScale) + TAGMIN;
                        }
                        else
                        {
                            _TagScaleVal = AsShort;
                        }
                        break;
                    case "INT32":
                    case "BCD32":
                    case "DINT":
                        if (TAGMIN != PLCMIN || TAGMAX != PLCMAX)
                        {
                            var fScale = (TAGMAX - TAGMIN) / (PLCMAX - PLCMIN);

                            _TagScaleVal = ((AsInt - PLCMIN) * fScale) + TAGMIN;
                        }
                        else
                        {
                            _TagScaleVal = AsInt;
                        }
                        break;
                    case "UINT32":
                    case "UBCD32":
                    case "UDINT":
                        if (TAGMIN != PLCMIN || TAGMAX != PLCMAX)
                        {
                            var fScale = (TAGMAX - TAGMIN) / (PLCMAX - PLCMIN);

                            _TagScaleVal = ((AsUint - PLCMIN) * fScale) + TAGMIN;
                        }
                        else
                        {
                            _TagScaleVal = AsUint;
                        }
                        break;
                    case "INT64":
                    case "LINT":
                        if (TAGMIN != PLCMIN || TAGMAX != PLCMAX)
                        {
                            var fScale = (TAGMAX - TAGMIN) / (PLCMAX - PLCMIN);

                            _TagScaleVal = ((AsLong - PLCMIN) * fScale) + TAGMIN;
                        }
                        else
                        {
                            _TagScaleVal = AsLong;
                        }
                        break;
                    case "UINT64":
                    case "ULINT":
                        if (TAGMIN != PLCMIN || TAGMAX != PLCMAX)
                        {
                            var fScale = (TAGMAX - TAGMIN) / (PLCMAX - PLCMIN);

                            _TagScaleVal = ((AsUlong - PLCMIN) * fScale) + TAGMIN;
                        }
                        else
                        {
                            _TagScaleVal = AsUlong;
                        }
                        break;
                    case "FLOAT":
                    case "REAL":
                        if (TAGMIN != PLCMIN || TAGMAX != PLCMAX)
                        {
                            var fScale = (TAGMAX - TAGMIN) / (PLCMAX - PLCMIN);

                            _TagScaleVal = ((AsFloat - PLCMIN) * fScale) + TAGMIN;
                        }
                        else
                        {
                            _TagScaleVal = AsFloat;
                        }
                        break;
                    case "DOUBLE":
                    case "LREAL":
                        if (TAGMIN != PLCMIN || TAGMAX != PLCMAX)
                        {
                            var fScale = (TAGMAX - TAGMIN) / (PLCMAX - PLCMIN);

                            _TagScaleVal = ((AsDouble - PLCMIN) * fScale) + TAGMIN;
                        }
                        else
                        {
                            _TagScaleVal = AsDouble;
                        }
                        break;
                }
            }
        }


        public void SetBlockIndex(PlcCommBlock[] commBlk)
        {
            int StartAddr;
            int EndAddr;
            int byteAddr, wordAddr;
            string dType;
            int varMulti;

            for (int i = 0; i < commBlk.GetLength(0); i++)
            {
                if (commBlk[i].DEVCODE.Equals(d_DevCode) == false)
                {
                    continue;
                }

                //Debug용
                /*switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                }*/

                StartAddr = commBlk[i].STARTADDRESS;
                EndAddr = StartAddr + (commBlk[i].READDATANUMBER - 1);

                // Byte, Word, Double Word 구분
                dType = ADDRESS.Substring(1, 1);

                // Bit
                if (dType.Equals("X"))
                {
                    varMulti = 2;
                }
                // Byte
                else if (dType.Equals("B"))
                {
                    varMulti = 2;
                }
                // Word
                else if (dType.Equals("W"))
                {
                    varMulti = 2;
                }
                // Double Word
                else if (dType.Equals("D"))
                {
                    varMulti = 4;
                }
                else if (dType.Equals("L"))
                {
                    varMulti = 8;
                }
                else
                {
                    varMulti = 2;
                }

                byteAddr = d_Address * varMulti;
                wordAddr = byteAddr / 2;

                if (StartAddr <= wordAddr && wordAddr <= EndAddr)
                {
                    d_CommBlkIdx = commBlk[i].d_BlockIdx;
                    d_DataPos = (wordAddr - StartAddr) * 2;
                    break;
                }
            }
        }

        public void SetCommTagInfo()
        {
            int aPos;
            int strLen;
            string WordAddr;
            string BitAddr;

            d_DevCode = ADDRESS.Substring(0, 1);
            d_DevType = ADDRESS.Substring(1, 1);

            string addr = ADDRESS.Substring(2, ADDRESS.Length - 2);    // 1234.1
            aPos = addr.IndexOf('.', 0);

            if (aPos > 0)
            {
                WordAddr = addr.Substring(0, aPos);
                BitAddr = addr.Substring(aPos + 1, (addr.Length - aPos - 1));
            }
            else
            {
                WordAddr = addr;
                BitAddr = "0";
            }

            // Digital Tag
            if (DATATYPE.Equals("BOOL"))
            {
                d_ByteSize = 1;

                if (d_DevType.Equals("B"))
                {
                    d_Address = Convert.ToInt32(WordAddr) / 2;
                    d_BitPos = Convert.ToInt32(BitAddr);
                }
                else if (d_DevType.Equals("W"))
                {
                    d_Address = Convert.ToInt32(WordAddr);
                    d_BitPos = Convert.ToInt32(BitAddr);
                }
                else if (d_DevType.Equals("D"))
                {
                    d_Address = Convert.ToInt32(WordAddr) * 2;
                    d_BitPos = Convert.ToInt32(BitAddr);
                } 
                else if (d_DevType.Equals("X"))
                {
                    d_Address = Convert.ToInt32(WordAddr) / 16;
                    d_BitPos = Convert.ToInt32(WordAddr) % 16;
                }
                // '.' 으로 Bit 어드레스 구분을 할 때
                //d_Address = Convert.ToInt32(addr.Substring(0, aPos));
                //d_BitPos = Convert.ToInt32(addr.Substring(aPos + 1, strLen - aPos - 1));

            }
            // Analog Tag
            else if (!DATATYPE.Equals("BOOL") )
            {
                d_Address = Convert.ToInt32(WordAddr);
                d_BitPos = 0;

                if (DATATYPE == "INT8" ||
                    DATATYPE == "UINT8" ||
                    DATATYPE == "BCD8" ||
                    DATATYPE == "SINT" ||
                    DATATYPE == "USINT" ||
                    DATATYPE == "UBCD8" ||
                    DATATYPE == "USHORT" || 
                    DATATYPE == "SHORT")
                {
                    d_ByteSize = 1;
                }
                else if (DATATYPE == "INT16" ||
                    DATATYPE == "UINT16" ||
                    DATATYPE == "BCD16" ||
                    DATATYPE == "UBCD16" ||
                    DATATYPE == "INT" ||
                    DATATYPE == "UINT" || 
                    DATATYPE == "INTEGER")
                {
                    d_ByteSize = 2;
                }
                else if (DATATYPE == "INT32" ||
                    DATATYPE == "UINT32" ||
                    DATATYPE == "BCD32" ||
                    DATATYPE == "UBCD32" ||
                    DATATYPE == "DINT" ||
                    DATATYPE == "UDINT" ||
                    DATATYPE == "FLOAT" ||
                    DATATYPE == "REAL")
                {
                    d_ByteSize = 4;
                }
                else if (DATATYPE == "INT64" ||
                         DATATYPE == "UINT64" ||
                         DATATYPE == "LINT" ||
                         DATATYPE == "ULINT" ||
                         DATATYPE == "DOUBLE" ||
                         DATATYPE == "LREAL")
                {
                    d_ByteSize = 8;
                }
                else if (DATATYPE.ToUpper() == "STRING")
                {
                    d_ByteSize = STRINGLEN;
                }
            }
        }


    }
}

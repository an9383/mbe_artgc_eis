using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.Logging;
using System.Timers;

namespace EventLogger
{
    public class EventLogger : Object
    {
        private readonly object _lock = new object();
        private readonly string iniFilename = "SystemInfo.ini";
        public string CurrentDirectory { get; set; } = Directory.GetCurrentDirectory();
        public string CurrentLogFolderName { get; set; } = "EIS";
        public string CurrentExceptionLogFolderName { get; set; } = "EIS_Exception";
        private readonly System.Timers.Timer _timer = new(1000 * 60 * 60 * 24);


        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        
        private static EventLogger _instance;

        public static EventLogger Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new EventLogger();

                return _instance;
            }
        }

        private enum LogType
        {
            Information,
            Error,
            Exception,
            Debug,
            PLCWrite,
            PLCRead,
            TOSReceived,
            TOSSend
        }


        public EventLogger()
        {
        }

        public void Initialize()
        {
            DeleteLog(CurrentLogFolderName);
            DeleteLog(CurrentExceptionLogFolderName);

            _timer.Elapsed += OnTimer;
            _timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            DeleteLog(CurrentLogFolderName);
            DeleteLog(CurrentExceptionLogFolderName);
        }

        private void DeleteLog(string logDirectory)
        {
            try
            {
                var directory = $"{CurrentDirectory}\\Log\\{logDirectory}\\";

                if (false == Directory.Exists(directory))
                    return;

                var dirInfo = new DirectoryInfo(directory);

                var listDirectory = dirInfo.GetDirectories();

                foreach (var dir in listDirectory)
                {
                    var name = dir.Name;

                    if (name.Split('-').Length < 2)
                        continue;

                    var endTime = DateTime.ParseExact(name, "yyyy-MM-dd", null);
                    var startTime = DateTime.Now;

                    var timeSpan = startTime - endTime;

                    if (timeSpan.TotalDays <= 90)
                        continue;

                    Directory.Delete(dir.FullName, true);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Instance.Exception(ex);
            }
        }

        private void ExceptionLog(string value)
        {
            lock (_lock)
            {
                try
                {
                    var dateTime = DateTime.Now;
                    //------------------------------
                    string logType = "EXCEPTION";
                    //------------------------------
                    var filePath = $"{GetExceptionLogDirectory()}\\{dateTime:yyyy-MM-dd-HH}.log";
                    var content = $"{dateTime:yyyy-MM-dd HH:mm:ss:fff}[{logType}]{value}";
                    //------------------------------
                    var fileStream = false == File.Exists(filePath) ? new StreamWriter(filePath) : File.AppendText(filePath);

                    fileStream.WriteLine(content);
                    fileStream.Close();
                }
                catch (Exception ex)
                {
                    EventLogger.Instance.Exception(ex);
                }
            }
        }
        

        private void Log(LogType type, string value)
        {
            lock (_lock)
            {
                try
                {
                    var dateTime = DateTime.Now;
                    //------------------------------
                    string logType = string.Empty;

                    switch (type)
                    {
                        case LogType.Debug: logType = "D E B U G"; break;
                        case LogType.Information: logType = " I N F O "; break;
                        case LogType.Error: logType = "E R R O R"; break;
                        case LogType.PLCRead: logType = "EIS < PLC"; break;
                        case LogType.PLCWrite: logType = "EIS > PLC"; break;
                        case LogType.TOSReceived: logType = "EIS < TOS"; break;
                        case LogType.TOSSend: logType = "EIS > TOS"; break;

                    }

                    if (string.IsNullOrEmpty(logType))
                        logType.PadRight(9);

                    //------------------------------
                    var filePath = $"{GetLogDirectory()}\\{dateTime:yyyy-MM-dd-HH}.log";
                    var content = $"{dateTime:yyyy-MM-dd HH:mm:ss:fff}[{logType}]{value}";
                    //------------------------------
                    var fileStream = false == File.Exists(filePath) ? new StreamWriter(filePath) : File.AppendText(filePath);

                    fileStream.WriteLine(content);
                    fileStream.Close();
                }
                catch (Exception ex)
                {
                    EventLogger.Instance.Exception(ex);
                }
            }
                
        }

        public void PLCRead(byte[] byteBuff, int cnt)
        {
            var sb = new StringBuilder();

            sb.Append($"[RX{DateTime.Now.ToString("yyyyMMddHHmmss")}]");
            sb.Append($"[BYTE={cnt}]");

            foreach (var buff in byteBuff)
            {
                sb.AppendFormat(" {00:X}", Convert.ToInt16(buff.ToString()));
            }


            Log(LogType.PLCRead, $"{sb.ToString()}");
        }

        public void PLCWrite(byte[] byteBuff, int cnt)
        {
            var sb = new StringBuilder();

            sb.Append($"[TX{DateTime.Now.ToString("yyyyMMddHHmmss")}]");
            sb.Append($"[BYTE={cnt}]");

            foreach(var buff in byteBuff)
            {
                sb.AppendFormat(" {00:X}", Convert.ToInt16(buff.ToString()));
            }

            Log(LogType.PLCWrite, $"{sb.ToString()}");
        }

        public void TOSSend(string value)
        {
            Log(LogType.TOSSend, $"{value}");
        }

        public void TOSReceived(string value)
        {
            Log(LogType.TOSReceived, $"{value}");
        }

        public void Information(string value)
        {
            Log(LogType.Information, $"{value}");
        }
        
        public void Error(string value)
        {
            Log(LogType.Error, $"{value}");
        }

        public void Debug(string value)
        {
            Log(LogType.Debug, $"{value}");
        }

        public void Exception(Exception ex)
        {
            var str = string.Format("{0}\n\n{1}\n\n{2}\n\n{3}\n\n{4}\n\n{5}",
                MethodBase.GetCurrentMethod().Name
                , ex.StackTrace
                , ex.TargetSite
                , ex.Data
                , ex.InnerException
                , ex.Source
            );

            ExceptionLog($"[{str}] EXCEPTION MESSAGE={ex.Message}");
        }

        public void Exception(MethodBase method, Exception ex)
        {
            var str = string.Format("{0}\n\n{1}\n\n{2}\n\n{3}\n\n{4}\n\n{5}",
                MethodBase.GetCurrentMethod().Name
                , ex.StackTrace
                , ex.TargetSite
                , ex.Data
                , ex.InnerException
                , ex.Source
            );

            ExceptionLog($"[{str}] EXCEPTION MESSAGE={ex.Message}");
        }

        public void Exception(MethodBase method, Exception ex, string optionMessage)
        {
            var str = string.Format("{0}\n\n{1}\n\n{2}\n\n{3}\n\n{4}\n\n{5}",
                MethodBase.GetCurrentMethod().Name
                , ex.StackTrace
                , ex.TargetSite
                , ex.Data
                , ex.InnerException
                , ex.Source
            );

            ExceptionLog($"[{str}] EXCEPTION MESSAGE={ex.Message}, OPTION MESSAGE={optionMessage}");
        }

        private string GetLogDirectory()
        {
            var dateTime = DateTime.Now;
            //------------------------------
            var directory = $"{CurrentDirectory}\\Log\\{CurrentLogFolderName}\\{dateTime:yyyy-MM-dd}";

            if (false == Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        private string GetExceptionLogDirectory()
        {
            var dateTime = DateTime.Now;
            //------------------------------
            var directory = $"{CurrentDirectory}\\Log\\{CurrentExceptionLogFolderName}\\{dateTime:yyyy-MM-dd}";

            if (false == Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        /*/// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public void setIniFilePath(string INIFilePath)
        {
            if (!INIFilePath.Contains(".ini"))
                INIFilePath = INIFilePath + ".ini";

            iniFilename = new FileInfo(INIFilePath).FullName.ToString();
        }

        /// <summary>
        /// 지정된 위치에 Data를 등록한다..
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void setINIValue(string Section, string Key, string Value)
        {
            //if (this.path == null || this.path.Equals(string.Empty))
            //    path = defaultPath;

            WritePrivateProfileString(Section, Key, Value, this.iniFilename);
        }

        /// <summary>
        /// 지정한 위치에서 값을 가져온다..
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string getINIValue(string Section, string Key)
        {
            //if (this.path == null || this.path.Equals(string.Empty))
            //    path = defaultPath;

            if (Section == null)
                return null;

            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.iniFilename);

            return temp.ToString();

        }

        /// <summary>
        /// Key값을 삭제한다..
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        public void DeleteKey(string Section, string Key)
        {
            //if (this.iniFileName == null || this.iniFileName.Equals(string.Empty))
            //    iniFileName = defaultPath;

            if (Section == null)
                return;

            setINIValue(Key, null, Section);
        }

        /// <summary>
        /// Section을 삭제한다..
        /// </summary>
        /// <param name="Section"></param>
        public void DeleteSection(string Section)
        {
            //if (this.iniFileName == null || this.iniFileName.Equals(string.Empty))
            //    iniFileName = defaultPath;

            if (Section == null)
                return;

            setINIValue(null, null, Section);
        }

        /// <summary>
        /// 해당 Key값이 있는지 확인한다..
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool KeyExists(string Section, string Key)
        {
            //if (this.iniFileName == null || this.iniFileName.Equals(string.Empty))
            //    iniFileName = defaultPath;

            return getINIValue(Key, Section).Length > 0 ? true : false;
        }*/


        public void SaveErrLog(string strMsg)
        {
            lock (this)
            {
                try
                {
                    string strFileName, strPath;

                    strMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + strMsg;

                    strPath = @"C:\ErrLog";
                    Directory.CreateDirectory(strPath);

                    strFileName = strPath + "\\" + "ErrLog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                    FileStream fs = new FileStream(strFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

                    sw.WriteLine(strMsg);
                    sw.Dispose();
                    fs.Dispose();
                }
                catch (Exception err)
                { }
            }
        }

        public void SaveFrameLog(byte[] rcvBuff, int size, int cnt, string gubun)
        {
            try
            {
                string strFileName, strPath, strMsg;

                strMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : ";

                strPath = @"C:\FrameLog";
                Directory.CreateDirectory(strPath);

                strFileName = strPath + "\\" + "FrameLog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                FileStream fs = new FileStream(strFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

                string buff = strMsg + "[" + gubun + "] : ";
                for (int i = 0; i < size; i++)
                {
                    buff = buff + " " + string.Format("{00:X}", Convert.ToInt16(rcvBuff[i].ToString()));
                }
                buff = "[" + cnt.ToString() + "]" + buff;
                sw.WriteLine(buff);
                sw.Dispose();
                fs.Dispose();
            }
            catch (Exception err)
            { }
        }

        /// <summary>
        /// 오류 발생시 "ERROR : 오류내용" 반환..
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public string DoWorkResult(string SQLResult, bool Result)
        {
            string sReturn = string.Empty;

            if (Result)
                sReturn = ToString(SQLResult);
            else
                sReturn = "[ERROR]" + Environment.NewLine + ToString(SQLResult);

            return sReturn;
        }


        /// <summary>
        /// 지정한 Object객체가 null이라면 공백값을 가진 string으로 반환한다..
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ToString(Object value)
        {
            if (value == null)
                return string.Empty;
            else
                return Convert.ToString(value);
        }

        /// <summary>
        /// 지정한 Object객체가 공백인지 여부를 나타낸다..
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool isNullOrWhiteSpace(object value)
        {
            string sValue = this.ToString(value).Trim();

            if (string.IsNullOrEmpty(sValue))
                return true;
            else
                return false;
        }

    }
}

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace KR.MBE.CommonLibrary.Utils
{

    public class Util
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(nint hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, nuint dwExtraInfo);


        public static void KillProcess(nint hWnd)
        {
            int id = 0;
            System.Diagnostics.Process p = null;
            try
            {
                GetWindowThreadProcessId(hWnd, out id);
                p = System.Diagnostics.Process.GetProcessById(id);
                if (p != null)
                {
                    p.Kill();
                    p.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("KillExcel:" + ex.Message);
            }
        }

        public static void TurnOFFCapsLockKey()
        {
            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                const int KEYEVENTF_EXTENDEDKEY = 0x1;
                const int KEYEVENTF_KEYUP = 0x2;

                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }

        public static void TurnONCapsLockKey()
        {
            if (Control.IsKeyLocked(Keys.CapsLock) == false)
            {
                const int KEYEVENTF_EXTENDEDKEY = 0x1;
                const int KEYEVENTF_KEYUP = 0x2;

                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }

        /// <summary>
        /// 현재 네트웍이 연결된 Client IP 를 가져온다.
        /// </summary>
        /// <returns>인터넷이 연결된 상태인경우 해당 IP를 리턴한다. 인터넷이 연결되지 않은 상태인 경우 "" 를 리턴한다. </returns>
        public static string FindActiveIPAddress()
        {
            string clientIP = string.Empty;

            string WMI_QUERY = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'true'  ";
            ObjectQuery oQry = new ObjectQuery(WMI_QUERY);
            ManagementObjectSearcher oSearCher = new ManagementObjectSearcher(oQry);

            try
            {
                foreach (ManagementObject obj in oSearCher.Get())
                {
                    clientIP = ((string[])obj["IPAddress"])[0];
                    break;
                }
            }
            catch
            {

            }
            return clientIP;

        }


        /// <summary>
        /// xml파일을 읽어서 해쉬테이블에 저장한다.
        /// </summary>
        /// <returns></returns>
        public static Hashtable ReadXml(string sFullPathWriteFile)
        {
            try
            {
                string sKey, sValue;
                Hashtable ht = new Hashtable();
                XmlTextReader xtr = new XmlTextReader(sFullPathWriteFile);
                while (xtr.Read())
                {
                    if (xtr.NodeType == XmlNodeType.Element)
                    {
                        sKey = xtr.LocalName;
                        xtr.Read();
                        if (xtr.NodeType == XmlNodeType.Text)
                        {
                            sValue = xtr.Value;
                            ht.Add(sKey, sValue);
                        }
                        else
                            continue;
                    }

                }//while
                xtr.Close();
                ht.Add("REPLY", "OK");
                return ht;
            }
            catch (FileNotFoundException e)
            {
                // xml파일을 찾지 못할경우 default값이 들어있는 해쉬테이블을 리턴한다.
                Hashtable ht = new Hashtable();
                ht.Add("REPLY", e.ToString());
                //ht.Add("formHeight", "400");
                //ht.Add("txtContentHeight", "296");
                //ht.Add("formWidth", "363");
                //ht.Add("formTop", "122");
                //ht.Add("formLeft", "214");
                //ht.Add("txtInputTop", "300");
                return ht;
            }
        }


        /// <summary>
        ///해쉬테이블을 읽어서 xml파일에 저장한다.
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static int WriteXml(Hashtable ht, string sFullPathWriteFile)
        {
            IDictionaryEnumerator htEnum = ht.GetEnumerator();
            XmlTextWriter tw = tw = new XmlTextWriter(sFullPathWriteFile, Encoding.UTF8);
            tw.Formatting = Formatting.Indented;
            tw.WriteStartDocument();
            tw.WriteStartElement("Configuration");
            while (htEnum.MoveNext())
            {
                tw.WriteElementString(htEnum.Key.ToString(), htEnum.Value.ToString());
            }
            tw.WriteEndElement();
            tw.WriteEndDocument();
            tw.Flush();
            tw.Close();
            return 0;
        }



        /// <summary>
        /// XML Format string에서 특정 Element의 Value를 리턴해 주는 함수
        /// </summary>
        /// <param name="sMsg">XML Format String</param>
        /// <param name="sPattern">sPattern == Element </param>
        /// <returns>Element를 가진 tag 내의 value</returns>
        /// <example>GetXMLRecord("<ITEM>data</ITEM>", "ITEM") ==> "data" 반환</example>
        public static string GetXMLRecord(string sMsg, string sPattern)
        {
            try
            {

                string rsData;
                string sFirstPattern;
                string sLastPattern;
                int iFirst;
                int iLast;

                if (sMsg == null || sMsg.Length <= 0)
                    return "";

                sFirstPattern = "<" + sPattern.Trim() + ">";
                sLastPattern = "</" + sPattern.Trim() + ">";

                iFirst = sMsg.IndexOf(sFirstPattern, 0);
                iLast = sMsg.IndexOf(sLastPattern, 0);

                if (iFirst < 0 || iLast < 0)
                {
                    return "";
                }
                else
                {
                    iFirst += sFirstPattern.Length;
                }

                rsData = sMsg.Substring(iFirst, iLast - iFirst);
                return rsData;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        /*
        public class RVUtil
        {
            private const int MAX_BYTE = 1024;
            private static string strline;
            private static int splitIndex;
            private static Dictionary<string, string> dic = new Dictionary<string, string>();

            private static string outValue;
            private static string[] outValues;

            private static string getPath { get; set; }
            private static string getEnv { get; set; }


            public RVUtil()
            {
            
            }

            public RVUtil(string Path)   
            {
                getPath = Path;
            }


            public string readValue(string inKey)
            {
                FileStream fileStreamInput = File.OpenRead(getPath);
                StreamReader sr = new StreamReader(fileStreamInput);


                while (sr.Peek() > -1)
                {
                    strline = sr.ReadLine();
                    splitIndex = strline.IndexOf('=');
                    if (splitIndex == 0 || splitIndex == -1 || strline.StartsWith("#"))
                        continue;
                    dic[strline.Substring(0, splitIndex)] = strline.Substring(splitIndex + 1);

                }
                sr.Close();

                foreach (KeyValuePair<string, string> keyval in dic)
                {
                    if (inKey == keyval.Key)
                    {
                        outValue = keyval.Value;
                        break;
                    }
                }
                fileStreamInput.Close();

                return outValue;
            }

            public string[] readValues(string inKey)
            {
                FileStream fileStreamInput = File.OpenRead(getPath);
                StreamReader sr = new StreamReader(fileStreamInput);

                string[] delimiter = {"|"};

                while (sr.Peek() > -1)
                {
                    strline = sr.ReadLine();
                    splitIndex = strline.IndexOf('=');
                    if (splitIndex == 0 || splitIndex == -1 || strline.StartsWith("#"))
                        continue;
                    dic[strline.Substring(0, splitIndex)] = strline.Substring(splitIndex + 1);

                }
                sr.Close();

                foreach (KeyValuePair<string, string> keyval in dic)
                {
                    if (inKey == keyval.Key)
                    {
                        outValue = keyval.Value;
                        break;
                    }
                }
                fileStreamInput.Close();

                outValues = outValue.Split(delimiter,StringSplitOptions.RemoveEmptyEntries);

                return outValues;
            }

            public string getServerType(string vServerName)
            {
                string serverType = "";
                string ENV = readValue("tibco.environment"); // TEST인지 PROD 인지 구별

                string[] mesList = readValues(ENV + "mes.serverlist");
                string[] eisList = readValues(ENV + "eis.serverlist");
            
                for (int i = 0; i < mesList.Length; i++)
                {
                    if(mesList[i].Equals(vServerName))
                    {
                        serverType = "mes";
                    }
                }

                for (int i = 0; i < eisList.Length; i++)
                {
                    if (eisList[i].Equals(vServerName))
                    {
                        serverType = "eis";
                    }
                }

                return serverType;
            }

            public void Log(String info,String strMsg)
            {
                DateTime dtNow = DateTime.Now;
                string strPath = string.Format(@"C:\KCC_RVLog\KCC_RV_{0}.log", dtNow.ToString("yyyyMMdd"));

                string strDir = Path.GetDirectoryName(strPath);
                DirectoryInfo diDir = new DirectoryInfo(strDir);

                if (!diDir.Exists)
                {
                    diDir.Create();
                    diDir = new DirectoryInfo(strDir);
                }

                if (diDir.Exists)
                {
                     System.IO.StreamWriter swStream = File.AppendText(strPath);
                    string strLog = String.Format("{0} {1} [{2}] {3}", dtNow.ToString("yyyy-MM-dd"), dtNow.ToString("hh:mm:ss"), info,strMsg);
                    swStream.WriteLine(strLog);
                    swStream.Close();
                }
            }

            public string createGUID()
            {
                Guid guid = Guid.NewGuid();
                string getGuid = guid.ToString();

                return getGuid;
            }

            public void Usage()
            {
                Console.Out.Write("Usage: RendezvousListener [-service service] [-network network]");
                Console.Out.Write("                          [-daemon daemon] <subject-list>");
                System.Environment.Exit(1);
            }

            public byte[] StringToByte(string getString)
            {
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                byte[] getData = encoding.GetBytes(getString);

                return getData;
            }

            public string ByteToString(Opaque getByte)
            {
                Opaque opaque = new Opaque();
                opaque.Value = getByte.Value;
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                string getData = encoding.GetString(opaque.Value);
                Log("INFO", "getDATA : " + getData);
                return getData;
            }

            public Opaque StringToOpaque(string getString)
            {
                Opaque opaque = new Opaque();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                byte[] getData = encoding.GetBytes(getString);

                opaque.Value = getData;

                return opaque;
            }

        }

         */


        /*public static void SendSMS(string number, string text)
        {
            SmsApi api = new SmsApi(new SmsApiOptions
            {
                ApiKey = "NCSAXWJO2V19FBBO", // 발급받은 ApiKey
                ApiSecret = "ZF2ZMN7RSZMVOLEAVGRMBKUJNTTWCFVF", // 발급받은 Secret Key
                DefaultSenderId = "1111111111" // 문자 보내는 사람 번호, coolsms 홈페이지에서 발신자 등록한 번호 필수
            });

            var result = api.SendMessageAsync(number, text);
            //Console.WriteLine( result );
        }*/


        public static string changePhoneNumber(string tel)
        {
            string tmpTel = tel.Replace("-", "").Trim();
            string tel1 = string.Empty;
            string tel2 = string.Empty;
            string tel3 = string.Empty;
            string tel_total = string.Empty;


            if (tmpTel.Length >= 2 && tmpTel.Length < 8)
            {
                if (tmpTel.Substring(0, 2) != "02")
                {
                    if (tmpTel.Length == 3)
                    {
                        //tel_total = tmpTel + "-";
                        tel_total = tmpTel;
                    }
                    else if (tmpTel.Length > 3 && tmpTel.Length < 6)
                    {
                        tel1 = tmpTel.Substring(0, 3);
                        tel2 = tmpTel.Substring(3, tmpTel.Length - 3);
                        tel_total = tel1 + "-" + tel2;
                    }
                    else if (tmpTel.Length > 3 && tmpTel.Length > 6)
                    {
                        tel1 = tmpTel.Substring(0, 3);
                        tel2 = tmpTel.Substring(3, 3);
                        tel3 = tmpTel.Substring(6, tmpTel.Length - 6);
                        tel_total = tel1 + "-" + tel2 + "-" + tel3;
                    }
                    else
                    {
                        tel_total = tel;
                    }
                }
                else
                {
                    if (tmpTel.Length == 2)
                    {
                        //tel_total = tmpTel + "-";
                        tel_total = tmpTel;
                    }
                    else if (tmpTel.Length > 2 && tmpTel.Length < 6)
                    {
                        tel1 = tmpTel.Substring(0, 2);
                        tel2 = tmpTel.Substring(2, tmpTel.Length - 2);
                        tel_total = tel1 + "-" + tel2;
                    }
                    else if (tmpTel.Length > 2 && tmpTel.Length > 5)
                    {
                        tel1 = tmpTel.Substring(0, 2);
                        tel2 = tmpTel.Substring(2, 3);
                        tel3 = tmpTel.Substring(5, tmpTel.Length - 5);
                        tel_total = tel1 + "-" + tel2 + "-" + tel3;
                    }


                }
            }
            else if (tmpTel.Length == 8 && tmpTel.Substring(0, 2) == "02")
            {
                tel1 = tmpTel.Substring(0, 2);
                tel2 = tmpTel.Substring(2, 3);
                tel3 = tmpTel.Substring(5, 3);
                tel_total = tel1 + "-" + tel2 + "-" + tel3;
            }
            else if (tmpTel.Length == 8 && tmpTel.Substring(0, 2) != "02")
            {
                tel1 = tmpTel.Substring(0, 4);
                tel2 = tmpTel.Substring(4, 4);
                tel_total = tel1 + "-" + tel2;
            }
            else if (tmpTel.Length == 9 && tmpTel.Substring(0, 2) == "02")
            {
                tel1 = tmpTel.Substring(0, 2);
                tel2 = tmpTel.Substring(2, 3);
                tel3 = tmpTel.Substring(5, 4);
                tel_total = tel1 + "-" + tel2 + "-" + tel3;
            }
            else if (tmpTel.Length == 9 && tmpTel.Substring(0, 2) != "02")
            {
                tel1 = tmpTel.Substring(0, 3);
                tel2 = tmpTel.Substring(3, 4);
                tel3 = tmpTel.Substring(7, 2);
                tel_total = tel1 + "-" + tel2 + "-" + tel3;
            }
            else if (tmpTel.Length == 10 && tmpTel.Substring(0, 2) == "02")
            {
                tel1 = tmpTel.Substring(0, 2);
                tel2 = tmpTel.Substring(2, 4);
                tel3 = tmpTel.Substring(6, 4);
                tel_total = tel1 + "-" + tel2 + "-" + tel3;
            }
            else if (tmpTel.Length == 10 && tmpTel.Substring(0, 2) != "02")
            {
                tel1 = tmpTel.Substring(0, 3);
                tel2 = tmpTel.Substring(3, 3);
                tel3 = tmpTel.Substring(6, 4);
                tel_total = tel1 + "-" + tel2 + "-" + tel3;
            }
            else if (tmpTel.Length == 11)
            {
                tel1 = tmpTel.Substring(0, 3);
                tel2 = tmpTel.Substring(3, 4);
                tel3 = tmpTel.Substring(7, 4);
                tel_total = tel1 + "-" + tel2 + "-" + tel3;
            }
            else
            {
                tel_total = tmpTel;
            }
            return tel_total;

        }


        public class GMail
        {
            private MailAddress sendAddress = null;
            private MailAddress toAddress = null;
            private string sendPassword = "";

            public GMail(string sendMail)
            {
                sendAddress = new MailAddress(sendMail);
            }

            public void SetToAddress(string toMail)
            {
                toAddress = new MailAddress(toMail);
            }

            public void SetPassword(string password)
            {
                sendPassword = password;
            }

            public string SendEmail(string subject, string body)
            {
                SmtpClient smtp = null;
                MailMessage message = null;
                try
                {
                    smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(sendAddress.Address, sendPassword),
                        Timeout = 20000
                    };
                    message = new MailMessage(sendAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    };
                    smtp.Send(message);
                    return "메일 보내기 완료";
                }
                catch (Exception ex)
                {
                    return "메일 보내기 실패" + ex.StackTrace;
                }
                finally
                {
                    if (smtp != null)
                    {
                        smtp.Dispose();
                    }
                    if (message != null)
                    {
                        message.Dispose();
                    }
                }
            }
        }


        #region WaitCursor                 using (Util.BeginWaitCursorBlock()) {    }
        public static IDisposable BeginWaitCursorBlock()
        {
            return !_waitCursorIsActive ? (IDisposable)new waitCursor() : null;
        }
        private static bool _waitCursorIsActive;
        private class waitCursor : IDisposable
        {
            private Cursor oldCur;
            public waitCursor()
            {
                _waitCursorIsActive = true;
                oldCur = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
            }
            public void Dispose()
            {
                Cursor.Current = oldCur;
                _waitCursorIsActive = false;
            }
        }
        #endregion

        #region BeginBusyBlock                 using (Util.BeginBusyBlock()) {    }
        public static IDisposable BeginBusyBlock()
        {
            return !_isBusy ? (IDisposable)new BusyBlock() : null;
        }
        private static bool _isBusy;
        private class BusyBlock : IDisposable
        {
            private Cursor oldCur;
            public BusyBlock()
            {
                _isBusy = true;
                oldCur = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
            }
            public void Dispose()
            {
                Cursor.Current = oldCur;
                _isBusy = false;
            }
        }
        #endregion


        public static bool AutoFontSize(Control button)
        {
            if (button == null) return false;

            Graphics gp;
            SizeF sz;
            float Faktor, FaktorX, FaktorY;

            gp = button.CreateGraphics();
            sz = gp.MeasureString(button.Text, button.Font);
            gp.Dispose();

            int btnSpaceW = 15, btnSpaceH = 5;
            FaktorX = (button.Width - btnSpaceW) / sz.Width;
            FaktorY = (button.Height - btnSpaceH) / sz.Height;

            if (FaktorX > FaktorY) Faktor = FaktorY;
            else Faktor = FaktorX;

            float newFontSize = button.Font.SizeInPoints * Faktor - 1;
            button.Font = new Font(button.Font.Name, newFontSize);

            return true;
        }

    }
}

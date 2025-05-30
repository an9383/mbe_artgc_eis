using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Middleware.ActiveMQ
{

    public class Util
    {

        public class KeyGenerator
        {
            static HashSet<string> set = new HashSet<string>();
            private const int uniqueKeySize = 16;
            private static readonly char[] chars = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
                                                            , 'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
                                                            , 'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
                                                            , '-'//, '!','#','$','%','*','-','?'
        };

            public static string GetUniqueKey( int iUniquekeysize )
            {
                RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
                byte[] data = new byte[iUniquekeysize];
                crypto.GetNonZeroBytes( data );
                StringBuilder result = new StringBuilder( iUniquekeysize );
                foreach( byte b in data )
                {
                    result.Append( chars[b % ( chars.Length - 1 )] );
                }
                return result.ToString();
            }

            public static string GetUniqueKey( String sPreFix, int iUniquekeysize )
            {
                RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
                byte[] data = new byte[iUniquekeysize];
                crypto.GetNonZeroBytes( data );
                StringBuilder result = new StringBuilder( iUniquekeysize );
                foreach( byte b in data )
                {
                    result.Append( chars[b % ( chars.Length - 1 )] );
                }
                String sUniqueKey = sPreFix + "-" + result.ToString();

                return sUniqueKey;
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
            ObjectQuery oQry = new ObjectQuery( WMI_QUERY );
            ManagementObjectSearcher oSearCher = new ManagementObjectSearcher( oQry );

            try
            {
                foreach( ManagementObject obj in oSearCher.Get() )
                {
                    clientIP = ( ( String[] )obj["IPAddress"] )[0];
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
        public static Hashtable ReadXml( String sFullPathWriteFile )
        {
            try
            {
                string sKey, sValue;
                Hashtable ht = new Hashtable();
                XmlTextReader xtr = new XmlTextReader( sFullPathWriteFile );
                while( xtr.Read() )
                {
                    if( xtr.NodeType == XmlNodeType.Element )
                    {
                        sKey = xtr.LocalName;
                        xtr.Read();
                        if( xtr.NodeType == XmlNodeType.Text )
                        {
                            sValue = xtr.Value;
                            ht.Add( sKey, sValue );
                        }
                        else
                            continue;
                    }

                }//while
                xtr.Close();
                ht.Add( "REPLY", "OK" );
                return ht;
            }
            catch( FileNotFoundException e )
            {
                // xml파일을 찾지 못할경우 default값이 들어있는 해쉬테이블을 리턴한다.
                Hashtable ht = new Hashtable();
                ht.Add( "REPLY", e.ToString() );
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
        public int WriteXml( Hashtable ht, String sFullPathWriteFile )
        {
            IDictionaryEnumerator htEnum = ht.GetEnumerator();
            XmlTextWriter tw = tw = new XmlTextWriter( sFullPathWriteFile, Encoding.UTF8 );
            tw.Formatting = Formatting.Indented;
            tw.WriteStartDocument();
            tw.WriteStartElement( "Configuration" );
            while( htEnum.MoveNext() )
            {
                tw.WriteElementString( htEnum.Key.ToString(), htEnum.Value.ToString() );
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
        public static string GetXMLRecord( string sMsg, string sPattern )
        {
            try
            {

                string rsData;
                string sFirstPattern;
                string sLastPattern;
                int iFirst;
                int iLast;

                if( sMsg == null || sMsg.Length <= 0 )
                    return "";

                sFirstPattern = "<" + sPattern.Trim() + ">";
                sLastPattern = "</" + sPattern.Trim() + ">";

                iFirst = sMsg.IndexOf( sFirstPattern, 0 );
                iLast = sMsg.IndexOf( sLastPattern, 0 );

                if( iFirst < 0 || iLast < 0 )
                {
                    return "";
                }
                else
                {
                    iFirst += sFirstPattern.Length;
                }

                rsData = sMsg.Substring( iFirst, iLast - iFirst );
                return rsData;
            }
            catch( Exception ex )
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
    }




}

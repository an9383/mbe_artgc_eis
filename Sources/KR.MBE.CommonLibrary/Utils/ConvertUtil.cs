using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace KR.MBE.CommonLibrary.Utils
{
    public class ConvertUtil
    {
        public static int ObjectToInt(object obj)
        {
            return ObjectToInt(obj, 0);
        }

        public static int ObjectToInt(object obj, int iDefaultValue)
        {
            if (obj == null)
            {
                return iDefaultValue;
            }

            if (!int.TryParse(obj.ToString(), out int iReturn))
            {
                iReturn = iDefaultValue;
            }

            return iReturn;
        }

        public static double ObjectToDouble(object obj)
        {
            return ObjectToDouble(obj, 0.0f);
        }

        public static double ObjectToDouble(object obj, double dDefaultValue)
        {
            if (obj == null)
            {
                return dDefaultValue;
            }

            if (!double.TryParse(obj.ToString(), out double dReturn))
            {
                dReturn = dDefaultValue;
            }

            return dReturn;
        }

        public static long ObjectToLong(object obj)
        {
            return ObjectToLong(obj, 0);
        }

        public static long ObjectToLong(object obj, long lDefaultValue)
        {
            string sValue = obj.ToString();
            long lReturn;

            if (sValue.Contains("."))
            {
                if (double.TryParse(sValue, out double value))
                {
                    lReturn = (long)Math.Truncate(value);
                }
                else
                {
                    lReturn = lDefaultValue;
                }
            }
            else
            {
                if (!long.TryParse(sValue, out lReturn))
                {
                    lReturn = lDefaultValue;
                }

            }

            return lReturn;
        }

        public static int GetStringLength(string str)
        {
            string s = str;
            byte[] temp = Encoding.Default.GetBytes(s);
            return temp.Length;
        }

        /// <summary>
        /// object가 null일 경우 error안나게 하기위해. string 변환
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetStringValue(string Value)
        {
            string rtnValue = "";
            if (Value == null || Value.ToString() == "")
            {
                rtnValue = "";
            }
            else
            {
                rtnValue = Value.ToString();
            }
            return rtnValue;
        }

        public static DateTime ObjectToDateTime(object value)
        {
            DateTime rtnValue = DateTime.Now;
            if (value != null)
            {
                try
                {
                    rtnValue = Convert.ToDateTime(value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return rtnValue;
        }

        public static string ObjectToString(object value)
        {
            string rtnValue = string.Empty;
            if (value != null)
            {
                rtnValue = value.ToString();
            }

            return rtnValue;
        }

        //public static DateTime? ObjectToDateTime( object value )
        //{
        //DateTime? rtnValue = null;
        //try
        //{
        //    rtnValue = Convert.ToDateTime( value );
        //}
        //catch( Exception ex )
        //{

        //}

        //return rtnValue;
        //}

        // 2022.03.17 smkang
        /// <summary>
        /// Convert ARGB hex string to color struct
        /// </summary>
        /// <param name="strColor">RGB string</param>
        /// <returns>color struct</returns>
        public static Color StringToColor(string strColor)
        {
            Color colorReturn = Color.Transparent;
            if (!string.IsNullOrEmpty(strColor))
            {
                colorReturn = ColorTranslator.FromOle(Convert.ToInt32(strColor));
            }

            return colorReturn;
        }

        /// <summary>
        /// Convert color struct to ARGB hex string
        /// </summary>
        /// <param name="color">color struct</param>
        /// <returns>ARGB hex string</returns>
        public static string colorToString(Color color)
        {
            return Convert.ToString(ColorTranslator.ToOle(color));
        }

        #region xml데이터중에서 해당 데이터만  구한다 [ private method ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sRecvMsg"></param>
        /// <param name="Param"></param>
        /// <param name="Param2"></param>
        /// <returns></returns>
        public static string getXMLResult(string sRecvMsg, string Param, string Param2)
        {
            string[] sTemp = new string[2];
            StringBuilder strEmptyXml = new StringBuilder();
            string returnMsg = "";

            try
            {

                sTemp[0] = Param;
                sTemp[1] = Param2;


                // 각문자열 인덱스 위치 
                int[] iTemp = new int[2];
                iTemp[0] = sRecvMsg.IndexOf(sTemp[0]);  // Param의 시작위치
                iTemp[1] = sRecvMsg.IndexOf(sTemp[1]);  // Param2의 시작위치

                iTemp[0] = iTemp[0] + sTemp[0].Length;

                int iReturnCodeLen = iTemp[1] - iTemp[0];       // Return Message 길이  


                //조회 데이터 0건시 에러.. 빈 xml생성~!! 2012.02.01 박성신

                if (iReturnCodeLen < 0)
                {
                    //strEmptyXml.Append("<DATALIST>");
                    //strEmptyXml.Append(" <DATA>");
                    //strEmptyXml.Append("</DATA>");
                    //strEmptyXml.Append(" </DATALIST>");

                    returnMsg = "";
                }
                else
                {
                    returnMsg = sRecvMsg.Substring(iTemp[0], iReturnCodeLen);
                }

                return returnMsg;
            }
            catch
            {

                return null;
            }
        }

        public static Dictionary<string, string> getXmlResultDictionary(string sRecvMsg, string Param, string Param2)
        {
            string[] sTemp = new string[2];
            StringBuilder strEmptyXml = new StringBuilder();
            string sTempStr;
            Dictionary<string, string> returnMsg = new Dictionary<string, string>();

            try
            {

                sTemp[0] = Param;
                sTemp[1] = Param2;

                // 각문자열 인덱스 위치 
                int[] iTemp = new int[2];
                iTemp[0] = sRecvMsg.IndexOf(sTemp[0]);  // Param의 시작위치
                iTemp[1] = sRecvMsg.IndexOf(sTemp[1]);  // Param2의 시작위치

                iTemp[0] = iTemp[0] + sTemp[0].Length;

                int iReturnCodeLen = iTemp[1] - iTemp[0];       // Return Message 길이  

                sTempStr = sRecvMsg.Substring(iTemp[0], iReturnCodeLen);
                sTempStr = sTempStr.Replace("\r", string.Empty);
                sTempStr = sTempStr.Replace("\n", string.Empty);

                //조회 데이터 0건시 에러.. 빈 xml생성~!! 2012.02.01 박성신

                if (!string.IsNullOrEmpty(sTempStr))
                {
                    int cursor = 0;

                    try
                    {

                        while (cursor < sTempStr.Length)
                        {
                            int iStart = sTempStr.IndexOf("<", cursor);

                            if (iStart < 0)
                            {
                                break;
                            }

                            int iEnd = sTempStr.IndexOf(">", iStart);

                            if (iEnd < 0)
                            {
                                break;
                            }


                            string key = sTempStr.Substring(iStart + 1, iEnd - iStart - 1);

                            cursor = iEnd + 1;

                            iStart = cursor;
                            iEnd = sTempStr.IndexOf("<", iStart);

                            if (iStart < 0 || iEnd < 0)
                            {
                                break;
                            }

                            if(iEnd - iStart - 1 <= 0)
                            {
                                break;
                            }

                            string value = sTempStr.Substring(iStart + 1, iEnd - iStart - 1);

                            returnMsg.Add(key, value);

                            cursor = iEnd + 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        return returnMsg;
                    }
                }

                return returnMsg;
            }
            catch
            {

                return null;
            }
        }

        #endregion

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

                if (sMsg == null || sMsg.Length <= 0) return "";

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

        /// <summary>
        /// XML Format string에서 특정 Element의 Values를 리턴해 주는 함수
        /// </summary>
        /// <param name="sMsg">XML Format String</param>
        /// <param name="sPattern">sPatter == Element </param>
        /// <returns>Element를 가진 tag 내의 values</returns>
        /// <example>GetXMLRecords("<ITEMS><ITEM>data1</ITEM><ITEM>data2</ITEM></ITEMS>", "ITEM") ==> "data1", "data2"를 가진 Hashtable 반환</example>
        public static Hashtable GetXMLRecords(string sMsg, string sPattern)
        {
            string rsData;
            string sFirstPattern;
            string sLastPattern;
            int iFirst;
            int iLast;
            int iCurrent = 0;
            int iCnt = 0;
            Hashtable ht = new Hashtable();

            try
            {

                if (sMsg == null || sMsg.Length <= 0) return ht;

                sFirstPattern = "<" + sPattern.Trim() + ">";
                sLastPattern = "</" + sPattern.Trim() + ">";

                while (true)
                {
                    iFirst = sMsg.IndexOf(sFirstPattern, iCurrent);
                    iLast = sMsg.IndexOf(sLastPattern, iCurrent);
                    iCurrent = iLast + sLastPattern.Length;

                    //					if(iCurrent > sMsg.Length)
                    //						return ht;

                    if (iFirst < 0 || iLast < 0)
                    {
                        return ht;
                    }
                    else
                    {
                        iFirst += sFirstPattern.Length;
                    }
                    rsData = sMsg.Substring(iFirst, iLast - iFirst);
                    ht.Add(iCnt++, rsData);
                }
            }
            catch (Exception ex)
            {
                ht.Clear();
                return ht;
            }

        }

        /// <summary>
        /// mobile server Application을 위한 INI 파일의 정보를 얻기 위한 함수
        /// </summary>
        /// <returns>XML Dom의 Root node</returns>
        public static XmlNode GetIniFile()
        {
            lock (typeof(Utils.ConvertUtil))
            {
                //string iniFilePath = "..\\..\\..\\inifile.xml";
                string iniFilePath = "Config\\inifile.xml";

                FileInfo file = new FileInfo(iniFilePath);
                if (!file.Exists)
                {
                    MessageBox.Show(file.DirectoryName + "에 INIF 정보화일(IniFile.xml)이 없습니다.\r\n" + file.Name + "을 복사 또는 이동 후 다시 실행시켜 주세요.");
                    Application.Exit();
                    return null;
                }
                else
                {
                    XmlTextReader reader = new XmlTextReader(iniFilePath);
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(reader);
                    reader.Close();
                    return xDoc.DocumentElement;
                }
            }
        }
    }
}

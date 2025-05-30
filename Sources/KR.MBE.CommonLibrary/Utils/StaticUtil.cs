using System;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Data;
//using System.EnterpriseServices;
using System.IO;
using System.Management;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;

namespace KR.MBE.CommonLibrary.Utils
{
    /// <summary>
    /// Static Library�� ���� ��� �����Դϴ�.
    /// </summary>
    /// 
    //[Transaction(TransactionOption.Supported)]
    //[JustInTimeActivation(true)]
    public class StaticUtil
    {
        public StaticUtil()
		{

        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        #region ���� ���� 

        /// <summary>
        /// ���� ���� ���� 
        /// </summary>
        //6�Ͽ�Ģ�� ���� Log ������ ����			
        public static int WHO = 0;
		public static int WHEN = 1;
		public static int WHERE = 2;
		public static int WHAT = 3;
		public static int WHY = 4;
		public static int HOW = 5;
		public static int MESSAGE = 6;

		public static string[] LOGSEQENCE = {"WHO","WHEN", "WHERE", "WHAT","WHY","HOW","MESSAGE"};
		public static string[] LOGDATA = new string[7];
		public static string[] LOGDATA_BACKUP = new String[7];
		//�������� message header �κ�
		public static string[] MSGHEADER = {"MSGNAME","RECEIVER","SENDER","MSGTYPE","WITHREPLY","STATUS","ERRMSG","TID","MSGBODY"};
		// Primary Msg(Request) or Secondary Msg(Reply)
		public static string PRIMARY = "P";
		public static string SECONDARY = "S";
		// PASS(Success) or FAIL(Error)
		public static string PASS = "PASS";
		public static string FAIL = "FAIL";
        public static string m_sPlantID = "ITIER";
        public static string m_sQueueName = ".\\private$\\eis_queue$";

        #endregion    

		/// <summary>
		/// DB Ÿ���� ������ ������ Ÿ��
		/// </summary>
		public enum DBTYPE //DB Type�� ���� Enumerate Type
		{
			OLEDB = 1, MSSQL, ORACLE 
		}


        public static string getINIValue(string iniFileName, string Section, string Key)
        {
            //if (this.path == null || this.path.Equals(string.Empty))
            //    path = defaultPath;

            if (Section == null)
                return null;

            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniFileName);

            return temp.ToString();
        }

        /// <summary>
        /// Change variable value to Host Variable value type 
        /// usage) DB condition check�� ���
        /// ex) mobile ==> 'mobile'
        /// </summary>
        /// <param name="generalType"></param>
        /// <returns>single quote�� �ѷ����� ����</returns>
        /// <example>HVType("mobile") ==> "'mobile'"�� ��ȯ </example>
        public static string HVType(string generalType) 
		{
			if(generalType == null) 
				return "";
			else
				return "'" + generalType + "'";
		}

		/// <summary>
		/// mobile server Application�� ���� INI ������ ������ ��� ���� �Լ�
		/// </summary>
		/// <returns>XML Dom�� Root node</returns>
		public static XmlNode GetIniFile()
		{
			lock(typeof(StaticUtil))
			{
				//string iniFilePath = "..\\..\\..\\inifile.xml";
                string iniFilePath = "Config\\inifile.xml";
				
				FileInfo file = new FileInfo(iniFilePath);
				if(!file.Exists)
				{
					MessageBox.Show( file.DirectoryName + "�� INIF ����ȭ��(IniFile.xml)�� �����ϴ�.\r\n" + file.Name + "�� ���� �Ǵ� �̵� �� �ٽ� ������� �ּ���.");
					Application.Exit();
					return null;
				}
				else
				{
					XmlTextReader reader = new XmlTextReader(iniFilePath );
					XmlDocument xDoc = new XmlDocument();
					xDoc.Load(reader);
					reader.Close();
					return xDoc.DocumentElement;
				}
			}
		}

		/// <summary>
		/// XML Format string���� Ư�� Element�� Value�� ������ �ִ� �Լ�
		/// </summary>
		/// <param name="sMsg">XML Format String</param>
		/// <param name="sPattern">sPattern == Element </param>
		/// <returns>Element�� ���� tag ���� value</returns>
		/// <example>GetXMLRecord("<ITEM>data</ITEM>", "ITEM") ==> "data" ��ȯ</example>
		public static string GetXMLRecord(string sMsg, string sPattern)
		{
			try
			{
		
				string rsData;
				string sFirstPattern;
				string sLastPattern;
				int iFirst;
				int iLast;

				if (sMsg == null || sMsg.Length <= 0 ) return "";
		 
				sFirstPattern = "<" +  sPattern.Trim() + ">";
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
			catch(Exception ex)
			{
				return "";
			}
		}
				
		/// <summary>
		/// XML Format string���� Ư�� Element�� n��° Value�� ������ �ִ� �Լ�
		/// </summary>
		/// <param name="sMsg">XML Format String</param>
		/// <param name="sPattern">sPatter == Element </param>
		/// <param name="iStData">n��°</param>
		/// <returns>Element�� ���� tag ���� value</returns>
		/// <example>GetXMLRecord("<ITEM>data1</ITEM><ITEM>data2</ITEM>", "ITEM", 2) ==> "data2" ��ȯ </example>
		public static string GetXMLRecord(string sMsg, string sPattern, int iStData)
		{
			try
			{
				string rsData;
				string sFirstPattern;
				string sLastPattern;
				int iFirst;
				int iLast;
				int iCurrent =0;
				int iCnt=0;

				if ( sMsg == null || sMsg.Length <= 0) return "";
		 
				sFirstPattern = "<" +  sPattern.Trim() + ">";
				sLastPattern = "</" + sPattern.Trim() + ">";

				while( true)
				{
					iFirst = sMsg.IndexOf(sFirstPattern, iCurrent);
					iLast = sMsg.IndexOf(sLastPattern, iCurrent);		
					iCurrent = iLast + sLastPattern.Length;
					
					if ( iFirst < 0 || iLast < 0 )
					{
						return "";
					}
					else
					{
						iFirst += sFirstPattern.Length;
					}
					rsData = sMsg.Substring(iFirst, iLast - iFirst);
					if(++iCnt == iStData)
						return rsData;
				}
			}
			catch(Exception ex)
			{
				return "";
			}
		}
	
		/// <summary>
		/// XML Format string���� Ư�� Element�� Values�� ������ �ִ� �Լ�
		/// </summary>
		/// <param name="sMsg">XML Format String</param>
		/// <param name="sPattern">sPatter == Element </param>
		/// <returns>Element�� ���� tag ���� values</returns>
		/// <example>GetXMLRecords("<ITEMS><ITEM>data1</ITEM><ITEM>data2</ITEM></ITEMS>", "ITEM") ==> "data1", "data2"�� ���� Hashtable ��ȯ</example>
		public static Hashtable GetXMLRecords(string sMsg, string sPattern)
		{
			string rsData;
			string sFirstPattern;
			string sLastPattern;
			int iFirst;
			int iLast;
			int iCurrent =0;
			int iCnt=0;
			Hashtable ht = new Hashtable();

			try
			{

				if (sMsg == null || sMsg.Length <= 0 ) return ht;
		 
				sFirstPattern = "<" +  sPattern.Trim() + ">";
				sLastPattern = "</" + sPattern.Trim() + ">";

				while( true)
				{
					iFirst = sMsg.IndexOf(sFirstPattern, iCurrent);
					iLast = sMsg.IndexOf(sLastPattern, iCurrent);		
					iCurrent = iLast + sLastPattern.Length;

//					if(iCurrent > sMsg.Length)
//						return ht;

					if ( iFirst < 0 || iLast < 0 )
					{
						return ht;
					}
					else
					{
						iFirst += sFirstPattern.Length;
					}
					rsData = sMsg.Substring(iFirst, iLast - iFirst);
					ht.Add( iCnt++, rsData);                    
				}
			}
			catch(Exception ex)
			{
				ht.Clear();
				return ht;
			}

		}

		/// <summary>
		/// XML Format string���� Ư�� Element�� Values�� replace�� �Ŀ� string Return �Լ�
		/// </summary>
		/// <param name="sMsg">XML Format String</param>
		/// <param name="sPattern">sPatter == Element </param>
		/// <param name="sReplace">Replace �� Value</param>
		/// <returns>����� XML Format string </returns>
		/// <example>ReplaceXMLRecord("<ITEM>data1</ITEM>", "ITEM", "data2") ==> "<ITEM>data2</ITEM>" ��ȯ </example>
		public static string ReplaceXMLRecord(string sMsg, string sPattern,string sReplace)
		{
			try
			{

				string rsData;
				string sFirstPattern;
				string sLastPattern;
				int iFirst;
				int iLast;
				int iCurrent =0;

				if (sMsg == null || sMsg.Length <= 0 ) 
				{
					return MakeXmlData("MESSAGE ERROR", "ERROR") + sMsg;
				}
		 
				sFirstPattern = "<" +  sPattern.Trim() + ">";
				sLastPattern = "</" + sPattern.Trim() + ">";

				iFirst = sMsg.IndexOf(sFirstPattern, iCurrent);
				iLast = sMsg.IndexOf(sLastPattern, iCurrent);		

				if ( iFirst < 0 || iLast < 0 )
				{
					return  InsertXMLRecord( sMsg, "", sPattern, sReplace);
				}
				else
				{
					iFirst += sFirstPattern.Length;
				}
				rsData = sMsg.Substring(iFirst, iLast - iFirst);
				sMsg = sMsg.Remove( iFirst, rsData.Length);
				sMsg = sMsg.Insert(iFirst, sReplace);
				return sMsg;
			}
			catch(Exception ex)
			{
				return "";
			}
			
		}

		/// <summary>
		/// XML Format string�� Ư�� Element(����element)�� �������� ���ο� element(Sub Element)�� �����ϴ� �Լ�
		/// </summary>
		/// <param name="sMsg">XML Format String</param>
		/// <param name="sUpperPattern">sUpperPattern == Upper Element </param>
		/// <param name="sSubPattern">sSubPattern == Sub Element </param>
		/// <param name="sValue">Value for Sub Element </param>
		/// <returns>���ο� �������� ���Ե� XML Format string </returns>
		/// <example>InsertXMLRecord("<ITEMS><ITEM1>data1</ITEM1></ITEMS>", "ITEM","ITEM2", "data2") ==> "<ITEMS><ITEM1>data1</ITEM1><ITEM2>data2</ITEM2></ITEMS>" ��ȯ </example>
		public static string InsertXMLRecord(string sMsg, string sUpperPattern, string sSubPattern, string sValue)
		{
			try
			{
				string sFirstPattern;
				string sLastPattern;
				int iFirst;
				int iLast;
				int iCurrent =0;
		
				if (sMsg == null || sMsg.Length <= 0) return sMsg;

				if(sUpperPattern == "" && sUpperPattern == null)
				{
					sFirstPattern = "<" +  sSubPattern.Trim() + ">";
					sLastPattern = "</" + sSubPattern.Trim() + ">";
					return sMsg = sMsg + sFirstPattern + sValue.Trim() + sLastPattern;
				}
		 
				sFirstPattern = "<" +  sUpperPattern.Trim() + ">";
				sLastPattern = "</" + sUpperPattern.Trim() + ">";

				iFirst = sMsg.IndexOf(sFirstPattern, iCurrent);
				iLast = sMsg.IndexOf(sLastPattern, iCurrent);		

				if ( iFirst < 0 || iLast < 0 )
				{
					sMsg += MakeXmlData( sValue, sSubPattern);
					return sMsg;
				}
				sMsg =  sMsg.Insert(iLast, MakeXmlData(sValue, sSubPattern));
				return sMsg;
			}
			catch(Exception ex)
			{
				return "";
			}
		}

		/// <summary>
		/// XML Format string�� Ư�� Element(����element)�� �������� ���ο� element(Sub Element)�� �����ϴ� �Լ�
		/// </summary>
		/// <param name="sMsg">XML Format String</param>
		/// <param name="sPattern">sPattern == Element name </param>
		/// <param name="sValue">Value for Element </param>
		/// <returns>���ο� �������� ���Ե� XML Format string </returns>
		/// <example>InsertXMLRecord("<ITEM1>data1</ITEM1>", "ITEM2", "data2") ==> "<ITEM1>data1</ITEM1><ITEM2>data2</ITEM2>" ��ȯ </example>
		public static string InsertXMLRecord(string sMsg,  string sPattern, string sValue)
		{
			return InsertXMLRecord(sMsg, "",sPattern, sValue);
		}

		/// <summary>
		/// �־��� element�� Tag�� ����� Ư�� value�� XML Format���� ��ȯ
		/// </summary>
		/// <param name="sValue">Value</param>
		/// <param name="sTag">Element ���</param>
		/// <returns>Tag�� ���� Value</returns>
		/// <example>MakeXmlData("data1",  "ITEM") ==> "<ITEM>data1</ITEM>" ��ȯ </example>
		public static string MakeXmlData(string sValue, string sTag)
		{
			return "<" + sTag + ">" + sValue + "</" + sTag + ">";
		}

		/// <summary>
		/// �α׿��� Ư�� �и��ڸ� ������ �� �ִ� �Լ�
		/// </summary>
		/// <param name="sLogArr"></param>
		/// <returns>formal log message</returns>
		/// <example>MakeFormalLog(StaticLib.LOGDATA)</example> 
		public static string MakeFormalLog(string[] sLogArr)
		{
			try
			{
				string sResult = null;

				if(sLogArr[MESSAGE] != null && sLogArr[MESSAGE].IndexOf("SEPERATOR") >=0) // �и��� �����ڰ� ���ǵǾ� �ִٸ�
				{
					StringBuilder sBuilder = new StringBuilder();
					string[] sArrr = sLogArr[MESSAGE].Split(','); // 0:SEPERATOR, 1:�и��ڷ� ���� ����, 2:Line��, 3:��,��
					for( int i=0; i< int.Parse(sArrr[2]);i++)
					{
						for(int j=0; j<100;j++)
						{
							sBuilder.Append( sArrr[1].Trim() );
						}
						sBuilder.Append( "\r\n" );
					}
					if( sArrr[3].Trim() == "PREVIOUS" )//	Message ���� �и��� ����
					{
						sResult = sBuilder.ToString();
						for(int i=0; i< sLogArr.Length -1; i++)
						{
							if(sLogArr[i] == null || sLogArr[i] == "")	
								break;
							else
								sResult = sResult + MakeXmlData( sLogArr[i], LOGSEQENCE[i] )+ "\r\n" ;
						}
					}
					else	//		Message �ڿ� �и��� ����
					{
						for(int i=0; i< sLogArr.Length -1; i++)
						{
							if(sLogArr[i] == null || sLogArr[i] == "")	
								break;
							else
								sResult = sResult + MakeXmlData( sLogArr[i], LOGSEQENCE[i] )+ "\r\n" ;
						}
						sResult = sResult + sBuilder.ToString();
					}
				}
				else // �Ϲ����� Log Message
				{
					for(int i=0; i< sLogArr.Length; i++)
					{
						if(sLogArr[i] != null && sLogArr[i] != "")	
							sResult = sResult + MakeXmlData( sLogArr[i], LOGSEQENCE[i] )+ "\r\n" ;
					}
				}			
				return sResult + "\r\n\r\n";
			}
			catch(Exception ex)
			{
				return "";
			}
		}

		/// <summary>
		///  Register Key�� ������ �Լ�
		/// </summary>
		/// <param name="sSubkey">Sub Key </param>
		/// <param name="sItem">Item</param>
		/// <returns>Item's value</returns>
		public static string GetRegisterKeyValue(string sSubkey, string sItem )
		{
            sSubkey = "Software\\MasterEquipmentInterfaceService\\" + sSubkey;
			RegistryKey RK = Registry.LocalMachine;
			RK = RK.CreateSubKey( sSubkey);
			return (string)RK.GetValue(sItem);
		}

		/// <summary>
		///  RegistryKey Value �� �����ϴ� �Լ�
		/// </summary>
		/// <param name="sSubkey">Sub Key</param>
		/// <param name="sItem">Item</param>
		/// <param name="sValue">Item's value</param>
		public static void SetRegisterKeyValue(string sSubkey, string sItem, string sValue)
		{
			sSubkey = "Software\\MasterEquipmentInterfaceService\\" + sSubkey;
			RegistryKey RK = Registry.LocalMachine;
			RK = RK.CreateSubKey(sSubkey);
			RK.SetValue( sItem, sValue );
		}
		
		/// <summary>
		/// DataSet�� ��� �ִ� TableNames�� ��� �Լ�
		/// </summary>
		/// <param name="ds"></param>
		/// <returns>Dataset�� table name</returns>
		public static string GetTablesNameFromDataSet(DataSet ds)
		{
			string  sResult = null;
			if(ds == null)
				return "ERROR" + "Error Message :  DataSet is NULL" ;
			foreach(DataTable dt in ds.Tables)
			{
				sResult += MakeXmlData( dt.ToString(), "TABLE");
			}
			return sResult;
		}

		/// <summary>
		/// Message���� Item�� �̸��� ���� data�� Item�� �̸����� ������ ����  XML �������� ��ȯ�ϴ� �Լ�
		/// </summary>
		/// <param name="theMsg">��ü Message</param>
		/// <param name="item">XML�� ���� Item </param>
		/// <param name="firstPatten">���� ����</param>
		/// <param name="secondPatten">������ ����</param>
		/// <returns>Tag�� �ѷ��� string item</returns>
		/// /// <example>MakeXmlData(" first-Patternsdatasecond-Patterns,"ITEM", "first-Patterns","second-Patterns") ==> return "<ITEM>data</ITEM>" </example>
		public static string MakeXmlData(string theMsg, string MsgTag,string firstPatten, string secondPatten)
		{
			if(theMsg == null)
				theMsg = "";

			string strTemp="";
			
			strTemp	=	"<" + MsgTag + ">";
			strTemp +=  GetParmValue(theMsg,firstPatten,secondPatten); 
			strTemp	+=	"</" + MsgTag + ">";
	
			return strTemp;
		}
		
		/// <summary>
		///  Get Data between Patterns that were given( first pattern + data + second pattern )
		/// </summary>
		/// <param name="theMsg">Parsing�� Message</param>
		/// <param name="FirstPatten">������ ����</param>
		/// <param name="SecondPatten">���� ��ġ</param>
		/// <returns>string data</returns>
		/// <example>GetParmValue("first-Patternsdatasecond-Patterns, "first-Patterns","second-Patterns") ==> return "data" </example>
		public static string GetParmValue(string theMsg, string FirstPatten, string SecondPatten )
		{
			string strMsg="";
			if(theMsg == null)
				theMsg = "";
			string sTemp = theMsg;
			
			try
			{
				if(sTemp=="")
				{
					strMsg=sTemp;					
				}
				else
				{
					int firstPos=sTemp.IndexOf(FirstPatten);

					if (firstPos==-1)
					{	
						strMsg=sTemp;
					}						 

					int secondPos=sTemp.IndexOf(SecondPatten,firstPos);
					if (secondPos==-1)
					{
						strMsg=sTemp.Substring(firstPos+ FirstPatten.Length , sTemp.Length - (firstPos+ FirstPatten.Length));
					}
					else
					{
						strMsg=sTemp.Substring(firstPos + FirstPatten.Length, secondPos-(firstPos + FirstPatten.Length));	
					}
				}
				return (strMsg.Trim());
			}
			catch(Exception e)
			{
				string errMsg=e.Message +" MSG=<" + theMsg+">,  WHERE=<" + e.StackTrace +">";
				return ("");
			}
		}

		/// <summary>
		/// Get Local(this) Computer Name�� ������ �Լ�
		/// </summary>
		/// <returns>Local Computer Name</returns>
		public static string GetLocalHostName_old()
		{
			StringBuilder strHostName = new StringBuilder();
			ManagementObjectSearcher SystemInfoquery = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem") ;
			ManagementObjectCollection queryCollection1 = SystemInfoquery.Get();
			foreach( ManagementObject mo in queryCollection1)
			{
				strHostName.Append( mo["csname"].ToString() );
			}
			return strHostName.ToString();
		}	

		public static string GetLocalHostName()
		{
			return System.Net.Dns.GetHostName();
		}

        /// <summary>
        ///  Middleware�� ���� ���� Message Header Function
        /// </summary>
        /// <param name="sMessageName">Messaage Name</param>
        /// <returns>string data</returns>
        public static string makeMessageHeader(string sSendSubject, string sReceiveSubject, string sMessageName)
        {
            string sHeader = "";

            sHeader = MakeXmlData(sMessageName, "messagename");
            sHeader += MakeXmlData(sSendSubject, "sourcesubject");
            sHeader += MakeXmlData(sReceiveSubject, "targetsubject");
            sHeader += MakeXmlData(sSendSubject, "replysubject");
            sHeader += MakeXmlData("", "transactionid");

            sHeader = MakeXmlData(sHeader, "header");

            return sHeader;
        }

        /// <summary>
        ///  Middleware�� ���� ���� Message Header Function
        /// </summary>
        /// <param name="sMessageName">Messaage Name</param>
        /// <returns>string data</returns>
        public static string makeMessageBody(string sOrderEqpID, string sWorkOrder, string sGubun)
        {
            string sBody = "";

            if (sGubun == "C")
            {
                sBody = MakeXmlData(sOrderEqpID, "PREORDEREQPID");
            }
            else
            {
                sBody = MakeXmlData(sOrderEqpID, "ORDEREQPID");
            }

            sBody += MakeXmlData(sWorkOrder, "WORKORDER");

            sBody = MakeXmlData(sBody, "BODY");

            return sBody;
        }
    }
}

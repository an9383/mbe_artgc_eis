using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
using KR.MBE.CommonLibrary.Manager;

namespace EISDataFilter.Utils
{
    public class DBHandler
    {
        private Constant _Constant = new Constant();

        //public SqlConnection _Connection = new SqlConnection();
        //public SqlCommand _Command = new SqlCommand();
        public SqlConnection _Connection = null;
        public SqlCommand _Command = null;

        public Boolean m_ConnectionCheck = false;

        private string _ConnectionString;


        public bool OpenDB()
        {
            lock (this)
            {
                if (_Connection == null)
                    _Connection = new SqlConnection();

                if (_Command == null)
                    _Command = new SqlCommand();

                //_ConnectionString = _eventLog.getINIValue("DBCONNECTION", "DataFilterConnection");
                _ConnectionString = "";
                try
                {
                    if (_Connection.State.ToString() == System.Data.ConnectionState.Closed.ToString())
                    {
                        _Connection.ConnectionString = _ConnectionString;
                        _Connection.Open();

                        _Command.Connection = _Connection;
                    }
                    return true;

                }
                catch (Exception ex)
                {
                    LogManager.Instance.Exception(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// MSSQL DataBase에 연결된 접속을 종료한다..
        /// </summary>
        public bool CloseDB()
        {
            try
            {
                if (_Connection != null && _Connection.State == ConnectionState.Open)
                {
                    if (_Command != null)
                    {
                        _Command.Dispose();
                        _Command = null;
                    }

                    _Connection.Close();
                    _Connection.Dispose();
                    _Connection = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Exception(ex);
                return false;
            }
        }


        public bool ExecuteInsert(Dictionary<string, object> dicSendSQL)
        {
            bool sReturn = false;

            //Connection Check후 연결 재시도..
            if (_Connection == null || _Connection.State.ToString() != System.Data.ConnectionState.Open.ToString())
            {
                m_ConnectionCheck = false;

                sReturn = this.OpenDB();

                if (!sReturn)
                    return sReturn;
            }

            DataSet dsResult = new DataSet();
            object dicParam = new object();

            string sQuery = string.Empty;

            if (dicSendSQL.ContainsKey(Constant.QUERY))
            {
                if (string.IsNullOrEmpty(dicSendSQL[Constant.QUERY].ToString()) == false)
                    sQuery = dicSendSQL[Constant.QUERY].ToString();
            }

            if (dicSendSQL.ContainsKey(Constant.PARAMETER))
                dicParam = dicSendSQL[Constant.PARAMETER];

            try
            {
                if (!sQuery.Contains(";"))
                    sQuery = sQuery.Trim() + ";";

                //Parameter 적용..
                if (dicSendSQL.ContainsKey(Constant.PARAMETER))
                    _Command = setCommendParameter(_Command, dicParam);
                else
                    _Command = setCommendParameter(_Command);

                _Command.CommandText = sQuery;
                _Command.CommandType = CommandType.Text;

                _Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 조회할 Query의 Parameter값을 받아 Command에 넣어준다..
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="dicParameter"></param>
        /// <returns></returns>
        public SqlCommand setCommendParameter(SqlCommand command, params object[] dicParameter)
        {
            string sKeyName = string.Empty;

            command.Parameters.Clear();

            foreach (object obj in dicParameter)
            {
                Dictionary<string, object> dicParam = obj as Dictionary<string, object>;


                foreach (KeyValuePair<string, object> pair in dicParam)
                {
                    if(string.IsNullOrEmpty((string?)pair.Value) == false)
                        command.Parameters.AddWithValue(pair.Key, pair.Value.ToString());
                    else
                        command.Parameters.AddWithValue(pair.Key, "");
                }
            }

            return command;
        }

        public bool insertIFXMLMSGSET(String xml, String EVENTTYPE)
        {
            bool sReturn ;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            Dictionary<string, object> dicSendSql = new Dictionary<string, object>();
            Dictionary<string, object> dicSendSqlParam = new Dictionary<string, object>();

            string sSQL = string.Empty;
            sSQL = "INSERT INTO [DBO].[IFXMLMSGSET] (EVENTTYPE, XMLMSGSET, INSERTID) VALUES(@EVENTTYPE, @XML, NEWID())";

            dicSendSql.Add("QUERY", sSQL);

            dicSendSqlParam.Add("@EVENTTYPE", EVENTTYPE);
            dicSendSqlParam.Add("@XML", xml);

            dicSendSql.Add("PARAMETER", dicSendSqlParam);

            sReturn = ExecuteInsert(dicSendSql);

            return sReturn;

        }

        public bool insertIFXMLMSGSET(XmlDocument xml)
        {
            bool sReturn ;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            Dictionary<string, object> dicSendSql = new Dictionary<string, object>();
            Dictionary<string, object> dicSendSqlParam = new Dictionary<string, object>();

            string sSQL = string.Empty;
            sSQL = "INSERT INTO [DBO].[IFXMLMSGSET] (XMLMSGSET, INSERTID) VALUES(@XML, NEWID())";

            dicSendSql.Add("QUERY", sSQL);

            dicSendSqlParam.Add("@XML", xml);

            dicSendSql.Add("PARAMETER", dicSendSqlParam);

            sReturn = ExecuteInsert(dicSendSql);

            return sReturn;
 
        }

    }
}

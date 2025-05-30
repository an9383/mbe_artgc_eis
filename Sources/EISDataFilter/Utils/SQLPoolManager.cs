using System;
using System.Collections;
using System.Linq;
using System.Text;
using KR.MBE.CommonLibrary.Manager;


namespace EISDataFilter.Utils
{
    public class SQLPoolManager
    {
        public static SQLPoolManager m_PoolMgr = null;
        private ArrayList m_SQLManagerList = null;
        private string m_ConnectionString = null;
        private int m_PoolCount = 0;

        private StringBuilder _Log = new StringBuilder();

        public SQLPoolManager()
        {
        }

        public static SQLPoolManager This()
        {
            if (m_PoolMgr == null)
                m_PoolMgr = new SQLPoolManager();
            return m_PoolMgr;
        }

        public SQLPoolManager(string sConnectionString)
		{
            this.ConnectionString = sConnectionString;

			InitializePoolCount();
		}

        public static SQLPoolManager This(string sConnectionString)
        {
            if (m_PoolMgr == null)
                m_PoolMgr = new SQLPoolManager(sConnectionString);
            return m_PoolMgr;
        }

        /// <summary>
        /// Pool에서 관리하는 SQLManager의 갯수를 가져오거나 설정합니다.
        /// </summary>		
        public int PoolCount
        {
            get
            {
                if (m_SQLManagerList != null)
                    return m_SQLManagerList.Count;
                else
                    return 0;
            }
            set
            {
                m_PoolCount = value;
            }
        }

        /// <summary>
        /// Pool에서 관리하는 SQLManager의 Connection String을 가져오거나 설정합니다.
        /// </summary>		
        public string ConnectionString
        {
            get
            {
                return m_ConnectionString;
            }
            set
            {
                m_ConnectionString = value;
            }
        }

        /// <summary>
        /// OleDbAgent Pool 관리를 위한 최초 생성 Count를 설정해 주는 함수.
        /// </summary>
        /// <remarks>
        ///  e.g:
        ///		m_PoolCount = 5
        /// </remarks>		
        private void InitializePoolCount()
        {
            m_PoolCount = 3;
            m_SQLManagerList = new ArrayList();
        }

        /// <summary>
        /// Default Count 만큼 SQLConnection을 생성하여 ArrayListCollection에 저장한다.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  CreatePoolList()
        /// </remarks>
        public void CreatePoolList()
        {
            if (m_ConnectionString == "" || m_ConnectionString == null)
                this.ConnectionString = ConnectionString;

            if (m_PoolCount <= 0)
                this.InitializePoolCount();

            for (int iCnt = 0; iCnt < m_PoolCount; iCnt++)
            {
                DBHandler tmpConnection = new DBHandler();
                bool bResult = tmpConnection.OpenDB();
                if (!bResult)
                {
                    string sErrMsg = string.Empty;
                    sErrMsg += "[ERROR SQLPoolManager] DBConnection : LocalEIS" + System.Environment.NewLine;

                    LogManager.Instance.Error(sErrMsg);
                }

                m_SQLManagerList.Add(tmpConnection);
            }
        }

        /// <summary>
        /// ArrayListCollection에서 사용중이 아닌 OleDbAgent를 반환해 준다.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OleDbAgent oraAgent = getUnuseCollection()
        /// </remarks>
        /// <returns>OleDbAgent</returns>		
        public DBHandler getUnuseCollection()
        {
            DBHandler SQLConnection = null;

            lock (this)
            {
                if (m_SQLManagerList.Count > 0)
                {
                    SQLConnection = (DBHandler)m_SQLManagerList[0];
                    m_SQLManagerList.RemoveAt(0);
                    return SQLConnection;
                }
                else
                {
                    SQLConnection = new DBHandler();
                    bool bResult = SQLConnection.OpenDB();
                    if (!bResult)
                    {
                        string sErrMsg = string.Empty;
                        sErrMsg += "[ERROR SQLPoolManager] DBConnection : LocalEIS" + System.Environment.NewLine;

                        LogManager.Instance.Error(sErrMsg);
                    }
                }
            }
            return SQLConnection;
        }

        /// <summary>
        /// 사용이 끝난 MSSQLManager를 ArrayListCollection에 추가한다.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="oraAgent">사용완료된 MSSQLManager</param>
        public void setUnuseCollection(DBHandler conn)
        {
            lock (this)
            {
                m_SQLManagerList.Add(conn);
            }
        }

        /// <summary>
        /// Connection 및 ArrayList를 해제한다.
        /// </summary>
        public void Release()
        {
            DBHandler dbConnection = new DBHandler();
            IEnumerator iEnum = m_SQLManagerList.GetEnumerator();

            while (iEnum.MoveNext())
            {
                dbConnection = (DBHandler)iEnum.Current;
                dbConnection.CloseDB();
            }
            m_SQLManagerList.Clear();
        }


    }
}

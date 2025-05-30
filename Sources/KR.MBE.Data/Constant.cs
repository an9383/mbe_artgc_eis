
namespace KR.MBE.Data
{
    public class Constant
    {
        #region MessageName
        /// <summary>
        /// Alarm 상태
        /// </summary>
        public class MessgeName
        {
            /// <summary>
            /// GetQueryResult
            /// </summary>
            public const string GetQueryResult = "GetQueryResult";
            /// <summary>
            /// GetQueryResultExcute
            /// </summary>
            public const string GetQueryResultExcute = "GetQueryResultExcute";
        }
        #endregion

        #region LanguageCode
        /// <summary>
        /// 언어코드
        /// </summary>
        public class LanguageCode
        {
            public const string LC_KOREAN = "ko";
            public const string LC_ENGLISH = "en";
            public const string LC_NATIVE1 = "n1";
            public const string LC_NATIVE2 = "n1";
            public const string LC_VIETNAM = "vt";
        }
        #endregion

        public const string DEVMODE = "DEV";  // 개발 모드
        public const string PRODMODE = "PROD";  // 운영 모드

        #region FlagTrueFalse
        /// <summary>
        /// Flag [ True | False ]
        /// </summary>
        public class FlagTrueFalse
        {
            public const string True = "True";
            public const string False = "False";
        }
        #endregion

        #region FlagYesOrNo
        /// <summary>
        /// Flag [ Yes | No ]
        /// </summary>
        public class FlagYesOrNo
        {
            public const string Yes = "Yes";
            public const string No = "No";
        }
        #endregion

        #region FlagYN
        /// <summary>
        /// Flag [ Y | N ]
        /// </summary>
        public class FlagYN
        {
            public const string Y = "Y";
            public const string N = "N";
        }
        #endregion

        #region ActiveState
        /// <summary>
        /// ActiveState [ Active | Inactive ]
        /// </summary>
        public class ActiveState
        {
            public const string Active = "Active";
            public const string Inactive = "Inactive";
        }
        #endregion

        #region BayDirection
        public class BayDirection
        {
            public const string Horizontal = "Horizontal";
            public const string Vertical = "Vertical";
        }
        #endregion


        #region DateString
        public class DateString
        {
            public const string STARTDATE = "19010101";
            public const string ENDDATE = "20991231";
            public const string STARTDATETIME = "19010101000000";
            public const string ENDDATETIME = "20991231235959";
        }
        #endregion


        #region CreateDataMethod
        public class CreateDataMethod
        {
            public const string FixedValue = "FixedValue";
            public const string InReceived = "InReceived";
            public const string MakeFunction = "MakeFunction";
        }
        #endregion

        #region JobStatus
        public class JobStatus
        {
            public const string Wait = "Wait";
            public const string Start = "Start";
            public const string Deleted = "Deleted";
            public const string Rejected = "Rejected";
            public const string Completed = "Completed";
        }
        #endregion


        #region StepJobStatus
        public class StepJobStatus
        {
            public const string Wait = "Wait";
            public const string StartRequest = "StartRequest";
            public const string Start = "Start";
            public const string Completed = "Completed";
        }
        #endregion

        #region StepJobType
        public class StepJobType
        {
            public const string StartEnd = "StartEnd";
            public const string Execute = "Execute";
        }
        #endregion

        #region YardData
        public class YardData
        {
            public const string BaySize20ft = "20ft";
            public const string BaySize40ft = "40ft";
            public const string BaySize45ft = "45ft";
        }
        #endregion

        /// <summary>
        /// Query형식 지정.."Value = 'SELECT', 'INSERT', 'UPDATE', 'DELETE'"
        /// </summary>
        public const string QUERYTYPE = "QUERYTYPE";

        public const string QUERY = "QUERY";

        public const string TABLENAME = "TABLENAME";

        /// <summary>
        /// DB Parameter 지정..
        /// </summary>
        public const string PARAMETER = "PARAMETER";

        /// <summary>
        /// QueryType 정의..SELECT Type
        /// </summary>
        public const string SELECT = "SELECT";

        /// <summary>
        /// QueryType 정의..INSERT Type
        /// </summary>
        public const string INSERT = "INSERT";

        /// <summary>
        /// QueryType 정의..UPDATE Type
        /// </summary>
        public const string UPDATE = "UPDATE";

        /// <summary>
        /// QueryType 정의..DELETE Type
        /// </summary>
        public const string DELETE = "DELETE";
    }
}

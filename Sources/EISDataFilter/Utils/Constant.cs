using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EISDataFilter.Utils
{
    class Constant
    {
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

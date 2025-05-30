using System.Data;
using System.Data.SqlClient;

namespace ARTGC.EIS
{
    public static class GlobalVar
    {
        public const string g_OPMode = "OFFLINE";

        public static string appPath;
        public static string iniFileName;

        public static int g_DrvNo;
        public static int g_CraneNo;

        public static string g_SiteID;
        public static string g_SiteName;

        public static SqlConnection g_cnnMainDB;

        public static SqlCommand g_cmdMain;

        public static DataTable g_dtGridList = new DataTable();
    }
}

using System.Data;

namespace KR.MBE.CommonLibrary.Struct
{

    public struct ReturnData
    {
        public string returncode
        {
            get; set;
        }
        public string returntype
        {
            get; set;
        }
        public string returnmessage
        {
            get; set;
        }
        public string returndetailmessage
        {
            get; set;
        }
        public DataSet returndataset
        {
            get; set;
        }
    }


}

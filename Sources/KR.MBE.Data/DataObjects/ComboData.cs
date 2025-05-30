using System;

namespace KR.MBE.Data.DataObjects
{

    public class ComboData
    {
        public ComboData( String sValue, String sName )
        {
            this.Value = sValue;
            this.Name = sName;
        }
        public String Value
        {
            get; set;
        }
        public String Name
        {
            get; set;
        }
    }


}

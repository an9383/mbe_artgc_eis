using System;
namespace KR.ITIER.Data.TableObjects
{
    public class INVENTORY
    {
        public class KeyData
        {
            public String SITEID { get; set; }
            public String BLOCKID { get; set; }
            public String BAYID { get; set; }
            public String ROWID { get; set; }
            public String TIERID { get; set; }
        }
        public KeyData Key = new KeyData();
        public String CONTAINERID { get; set; }
        public String CONTAINERISO { get; set; }
        public String CONTAINERTYPE { get; set; }
        public String CONTAINEROPR { get; set; }
        public String CONTAINERCLASS { get; set; }
        public String CONTAINERCARGOTYPE { get; set; }
        public String CONTAINERFULLMTY { get; set; }
        public String REEFERWORKTEMP { get; set; }
        public String REEFERTEMP { get; set; }
        public String REEFERTEMPUNIT { get; set; }
        public String REEFERPLUGSTATUS { get; set; }
        public String SENDSEQNUMBER { get; set; }
        public String SENDTIMEKEY { get; set; }
        public String SENDINOUTFLAG { get; set; }
        public String LASTUPDATETIME { get; set; }
        public String LASTUPDATEUSERID { get; set; }
    }
}

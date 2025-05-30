using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public interface PlcInterface
    {
        public void CommStart();
        public void CommEnd();

        public void SetStation(DataRow dr);

        public void MakeReadFrame();

        public bool WriteTag(DataRow dr);
        public string WriteTag(DataTable dt);

        public string EQUIPMENTID { get; set; }

        public string PROTOCOLNAME { get; set; }
        public bool commStatus { get; }
        public bool commEnable { get; set; }

        public string EquipmentType { get; set; }

        public string STATIONID { get; set; }
    }
}

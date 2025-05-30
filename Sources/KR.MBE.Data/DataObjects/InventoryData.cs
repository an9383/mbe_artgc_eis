using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace KR.MBE.Data.DataObjects
{
    public class InventoryData
    {
        public String SITEID = string.Empty;
        public String BLOCKID = string.Empty;
        public String BLOCKNAME = string.Empty;
        public String BAYID = string.Empty;
        public String BAYNAME = string.Empty;
        public String ROWID = string.Empty;
        public String ROWNAME = string.Empty;
        public String WORKINGLANEFLAG = string.Empty;
        public String TIERID = string.Empty;
        public String TIERNAME = string.Empty;
        public int TIERINDEX = 0;
        public String CONTAINERID = string.Empty;
        public String CONTAINERISO = string.Empty;
        public String CONTAINERTYPE = string.Empty;
        public String CONTAINEROPR = string.Empty;
        public String CONTAINERCLASS = string.Empty;
        public String CONTAINERCARGOTYPE = string.Empty;
        public String CONTAINERFULLMTY = string.Empty;
        public String REEFERWORKTEMP = string.Empty;
        public String REEFERTEMP = string.Empty;
        public String REEFERTEMPUNIT = string.Empty;
        public String REEFERPLUGSTATUS = string.Empty;
        public String SENDSEQNUMBER = string.Empty;
        public String SENDTIMEKEY = string.Empty;
        public String SENDINOUTFLAG = string.Empty;
        public String LASTUPDATETIME = string.Empty;
        public String LASTUPDATEUSERID = string.Empty;

        public void SetInventoryInfo(DataRow dr)
        {
            FieldInfo[] infoArr = this.GetType().GetFields();
            string colName = string.Empty;
            string value = string.Empty;

            for (int i = 0; i < infoArr.Length; i++)
            {
                for (int col = 0; col < dr.Table.Columns.Count; col++)
                {
                    colName = dr.Table.Columns[col].ToString().ToUpper();
                    if (infoArr[i].Name == colName && (infoArr[i].FieldType.Name == "String"))
                    {
                        value = dr[col].ToString();
                        FieldInfo info = infoArr[i];
                        info.SetValue(this, value);
                        break;
                    }
                    if (infoArr[i].Name == colName && (infoArr[i].FieldType.Name == "Int32"))
                    {
                        value = dr[col].ToString();
                        FieldInfo info = infoArr[i];
                        info.SetValue(this, Convert.ToInt32(value));
                        break;
                    }
                }
            }
        }

        public Hashtable ToHashTable()
        {
            FieldInfo[] infoArr = this.GetType().GetFields();
            Hashtable htReturn = new Hashtable();
            for (int i = 0; i < infoArr.Length; i++)
            {
                FieldInfo info = infoArr[i];
                htReturn.Add(infoArr[i].Name, info.GetValue(this));
            }
            return htReturn;
        }


    }




}


using KR.ITIER.Data;
using KR.ITIER.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR.ITIER.UI.ControlUtil.CustomControl
{
    public partial class itierBayInventory : Panel
    {
        [Category("CustomData"), Description("SiteID")]
        public String SiteID { set; get; } = String.Empty;
        [Category("CustomData"), Description("Yard BlockID")]
        public String BlockID { set; get; } = String.Empty;
        [Category("CustomData"), Description("Yard BayID")]
        public String BayID { set; get; } = String.Empty;

        private itierContainer selectInventory = null;

        public event EventHandler OnSelectInventoryChanged;
        public class SelectInventoryChangedEventArgs : EventArgs
        {
            public itierContainer InventoryInfo;
        }
        public itierBayInventory()
        {
            InitializeComponent();
        }

        public itierBayInventory(string sSiteID, string sBlockID, string sBayID)
        {
            InitializeComponent();
            this.SiteID = sSiteID;
            this.BlockID = sBlockID;
            this.BayID = sBayID;
        }

        public void InitInventory()
        {
            if ((String.IsNullOrEmpty(this.SiteID)) || (String.IsNullOrEmpty(this.BlockID)) || (String.IsNullOrEmpty(this.BayID)))
            {
                return;
            }

            // Clear
            this.Controls.Clear();
            this.SuspendLayout();

            int cellINTERVAL = 31;
            // ADD Cell
            int xCellPos = 10;
            List<Control> cellControls = new List<Control>();

            DataTable dtRowInfo = getRowInfo();
            DataTable dtTierInfo = getTierInfo();

            for (int i = 0; i < dtRowInfo.Rows.Count; i++)
            {
                int yCellPos = 190;
                string sRowID = dtRowInfo.Rows[i]["ROWID"].ToString();
                string sRowName = dtRowInfo.Rows[i]["ROWNAME"].ToString();
                string sWorkingLane = dtRowInfo.Rows[i]["WORKINGLANEFLAG"].ToString();

                if ("Yes".Equals(sWorkingLane))
                {
                    if ("2".Equals(sRowName))
                    {
                        xCellPos += 7;
                    }
                }

                ucContainer_30 rowDisplayControl = new ucContainer_30(sRowName, "", xCellPos, yCellPos, false, true);

                cellControls.Add(rowDisplayControl);

                if ("Yes".Equals(sWorkingLane))
                {
                    yCellPos -= cellINTERVAL;

                    ucContainer_30 vehicle = new ucContainer_30(sRowName + "V", sWorkingLane, xCellPos, yCellPos, false, false);

                    cellControls.Add(vehicle);

                    if ("1".Equals(sRowName))
                    {
                        xCellPos += 7;
                    }
                }

                if (!"Yes".Equals(sWorkingLane))
                {
                    DataRow[] drTierList = dtTierInfo.Select("ROWID = '" + sRowID + "'", "TIERINDEX");

                    if (drTierList.Length > 0)
                    {
                        for (int y = 0; y < drTierList.Length; y++)
                        {
                            DataRow drTier = drTierList[y];

                            yCellPos -= cellINTERVAL;
                            itierContainer container = new itierContainer(drTier, xCellPos, yCellPos, false, false);
                            container.MouseEnter += Container_MouseEnter;
                            cellControls.Add(container);
                        }
                    }
                }
                xCellPos += cellINTERVAL;
            }

            this.Controls.AddRange(cellControls.ToArray());
            this.ResumeLayout();
            this.Invalidate();
        }

        private void Container_MouseEnter(object sender, EventArgs e)
        {
            selectInventory = (itierContainer)sender;
            SelectInventoryChangedEventArgs arg = new SelectInventoryChangedEventArgs();
            arg.InventoryInfo = selectInventory;
            this.OnSelectInventoryChanged(this, arg);

        }

        public itierContainer getSelectInventory()
        {
            return this.selectInventory;
        }

        private DataTable getRowInfo()
        {
            Hashtable htParameter = new Hashtable();
            htParameter.Add("SITEID", this.SiteID);
            htParameter.Add("BLOCKID", this.BlockID);
            htParameter.Add("BAYID", this.BayID);

            DataSet dsReturn = MessageHandler.getCustomQuery(this.SiteID, "getMonRowInfo", "00001", Constant.LanguageCode.LC_KOREAN, htParameter);
            DataTable dtReturn = null;

            if (dsReturn.Tables.Contains("_REPLYDATA"))
            {
                dtReturn = dsReturn.Tables["_REPLYDATA"];
            }

            return dtReturn;
        }

        private DataTable getTierInfo()
        {
            Hashtable htParameter = new Hashtable();
            htParameter.Add("SITEID", this.SiteID);
            htParameter.Add("BLOCKID", this.BlockID);
            htParameter.Add("BAYID", this.BayID);

            DataSet dsReturn = MessageHandler.getCustomQuery(this.SiteID, "getBayInventory", "00001", Constant.LanguageCode.LC_KOREAN, htParameter);
            DataTable dtReturn = null;

            if (dsReturn.Tables.Contains("_REPLYDATA"))
            {
                dtReturn = dsReturn.Tables["_REPLYDATA"];
            }

            return dtReturn;
        }


    }
}

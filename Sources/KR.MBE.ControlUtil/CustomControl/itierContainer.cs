using KR.ITIER.CommonLibrary;
using KR.ITIER.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR.ITIER.UI.ControlUtil.CustomControl
{
    public partial class itierContainer : Button
    {
        [Category("CustomData"), Description("SiteID")]
        public String SiteID { set; get; } = String.Empty;

        [Category("CustomData"), Description("Yard BlockID")]
        public String BlockID { set; get; } = String.Empty;

        [Category("CustomData"), Description("Yard BayID")]
        public String BayID { set; get; } = String.Empty;

        [Category("CustomData"), Description("Yard RowID")]
        public String RowID { set; get; } = String.Empty;

        [Category("CustomData"), Description("Yard TierID")]
        public String TierID { set; get; } = String.Empty;

        [Category("CustomData"), Description("Vehicle Flag")]
        public bool VehicleFlag { set; get; } = false;

        #region variable
        public ContainerData ContainerInfo = new ContainerData();
        #endregion

        public itierContainer()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public itierContainer(DataRow drInventory, int xPos, int yPos, bool labelFlag, bool vehicleFlag)
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            this.SiteID = drInventory["SITEID"].ToString();
            this.BlockID = drInventory["BLOCKID"].ToString(); 
            this.BayID = drInventory["BAYID"].ToString();
            this.RowID = drInventory["ROWID"].ToString();
            this.TierID = drInventory["TIERID"].ToString();

            this.ContainerInfo.SetContainerInfo(drInventory);

            this.Location = new Point(xPos, yPos);
            this.VehicleFlag = vehicleFlag;

            initContainer();


        }

        public void initContainer()
        {
            this.Width = 30;
            this.Height = 30;

            if (this.ContainerInfo.CONTAINERID.Length > 0)
            {
                this.BackColor = Color.SandyBrown;
            }

            if (this.VehicleFlag)
            {
                this.BackgroundImage = imgList.Images["vehicle.png"];
            }

        }

        



    }
}

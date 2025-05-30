using KR.MBE.Data.DataObjects;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace KR.MBE.UI.ControlUtil.CustomControl
{
    public partial class itierInventory : Button
    {
        #region variable
        public InventoryData InventoryInfo = new InventoryData();
        #endregion

        public itierInventory()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public itierInventory(DataRow drInventory, int xPos, int yPos, bool labelFlag, bool vehicleFlag)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.InventoryInfo.SetInventoryInfo(drInventory);
            this.Location = new Point(xPos, yPos);
            initInventory();
        }

        public void initInventory()
        {
            this.Width = 30;
            this.Height = 30;
            this.Text = "";
            this.ForeColor = Color.White;
            if (this.InventoryInfo.CONTAINERID.Length > 0)
            {
                this.BackColor = Color.SandyBrown;
            }
            else
            {
                //this.BackColor = SystemColors.Control;
                this.BackColor = Color.White;

            }
        }

        public void SetBackColor(Color c)
        {
            this.BackColor = c;
        }



    }
}

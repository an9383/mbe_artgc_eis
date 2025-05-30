using System;
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
    public partial class ucBayInfo : UserControl
    {
        #region var/const
        public string mCraneID, mStatus, mMode;
        //private int mCellX, mCellY;
        private int mXPos, mYPos;
        private Graphics mGraphic;
        private SolidBrush mFillBrush, mPenBrush;
        private Font mDrawFont;

        private const int XSTART = 11, YSTART = 11, INTERVAL = 360;
        private const int BlockXInterval = 348, BlockYInterVal = 303;
        private const int XCellSTART = 42, YCellSTART = 6, cellINTERVAL = 25;

        private Dictionary<string, ucTrolley> mTrolleyDic = new Dictionary<string, ucTrolley>();
        private Dictionary<string, ucHoist> mHoistDic = new Dictionary<string, ucHoist>();
        public Dictionary<string, ucSpreader> mSpreaderDic = new Dictionary<string, ucSpreader>();
        public Dictionary<string, ucContainer> mCellDic = new Dictionary<string, ucContainer>();
        private Dictionary<string, ucBayInfo> mBayDic = new Dictionary<string, ucBayInfo>();
        private Dictionary<string, ucInventory> mInventoryDic = new Dictionary<string, ucInventory>();

        public event EventHandler buttonClick_Event;
        private bool bClickFlag = false;
        delegate void TimerEventFiredDelegate();

        public System.Threading.Timer timer1;
        #endregion

        public ucBayInfo()
        {
            InitializeComponent();
        }

        public ucBayInfo(string sCraneID, string sStatus, string sMode, int xPos, int yPos)
        {            
            InitializeComponent();

            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            mCraneID = sCraneID;
            mStatus = sStatus;
            mMode = sMode;

            mXPos = xPos;
            mYPos = yPos;

            // GDI 변수 초기화
            mGraphic = this.CreateGraphics();
            mFillBrush = new SolidBrush(Color.Blue);
            mPenBrush = new SolidBrush(Color.Black);
            mDrawFont = new Font("Arial", 8, FontStyle.Bold);
            this.Location = new Point(mXPos, mYPos);

            txt1.Text = sCraneID;

            //txtStatus.Text = mStatus;

            txtMode.Text = mMode;
            txtMode.TextAlign = HorizontalAlignment.Center;
            txtMode.BorderStyle = BorderStyle.None;

            setBayStatus(mStatus);

            btn1.Click += btn1_Click;
        }

        /// <summary>
        /// Bay 
        /// </summary>
        /// <param name="sBayInfo"></param>
        public void setLabel(string sBayInfo)
        {
            this.lbBayInfo.Text = sBayInfo;
        }

        public void btn1_Click(object sender, EventArgs e)
        {
            if (this.buttonClick_Event != null)
            {
                //buttonClick_Event(sender, e);
                if (timer1 == null)
                {
                    timer1 = new System.Threading.Timer(Callback);
                    timer1.Change(0, 10);
                }

                bClickFlag = !bClickFlag;
                this.customPanel.AnimateBorder = bClickFlag;
                this.setVehicleDisplay(true);
            }
        }

        public void setBorderAnimate(bool bAnimateFlag)
        {
            this.customPanel.AnimateBorder = bAnimateFlag;
        }

        public void setVehicleDisplay(bool vehicleFlag)
        {
            if (mCellDic.ContainsKey("Vehicle"))
            {
                mCellDic["Vehicle"].BackgroundImage = null;

                if (vehicleFlag)
                {
                    mCellDic["Vehicle"].BackgroundImage = imgList.Images["vehicle_225.png"];
                    mCellDic["Vehicle"].BorderStyle = BorderStyle.None;
                }
            }
        }

        void Callback(object status)
        {
            // UI 에서 사용할 경우는 Cross-Thread 문제가 발생하므로 Invoke 또는 BeginInvoke 를 사용해서 마샬링을 통한 호출을 처리하여야 한다.
            if (this.InvokeRequired)
            {
                this.BeginInvoke((System.Action)(() =>
                {
                    BeginInvoke(new TimerEventFiredDelegate(Work));
                }));
            }
        }

        private void clearDictionary()
        {
            mInventoryDic.Clear();
            mCellDic.Clear();
            mTrolleyDic.Clear();
            mHoistDic.Clear();
            mSpreaderDic.Clear();
        }

        public void drawInventory(string sCraneID, string sBayID, string sEquipmentStatus, DataTable dtRowInfo, DataTable dtTierInfo)
        {
            // 초기화
            clearDictionary();

            ucInventory inventory = this.inventory;

            inventory.setBayInfo(sCraneID, sBayID, sEquipmentStatus);

            //inventory.Click += Inventory_Click;

            if (!mInventoryDic.ContainsKey(sCraneID))
            {
                mInventoryDic.Add(sCraneID, inventory);
            }

            // ADD Trolley
            List<Control> trolleyControls = new List<Control>();
            ucTrolley trolley = new ucTrolley(sCraneID, inventory.Width - 78, 20);

            if (!mTrolleyDic.ContainsKey(sCraneID))
            {
                mTrolleyDic.Add(sCraneID, trolley);
            }

            trolleyControls.Add(trolley);
            inventory.Controls.AddRange(trolleyControls.ToArray());

            // ADD Hoist
            List<Control> hoistControls = new List<Control>();
            ucHoist hoist = new ucHoist(sCraneID, inventory.Width - 68, 35, 30);

            if (!mHoistDic.ContainsKey(sCraneID))
            {
                mHoistDic.Add(sCraneID, hoist);
            }

            hoistControls.Add(hoist);
            inventory.Controls.AddRange(hoistControls.ToArray());

            // ADD Spreader
            List<Control> spreaderControls = new List<Control>();
            ucSpreader spreader = new ucSpreader(sCraneID, inventory.Width - 78, 65);

            if (!mSpreaderDic.ContainsKey(sCraneID))
            {
                mSpreaderDic.Add(sCraneID, spreader);
            }

            spreaderControls.Add(spreader);
            inventory.Controls.AddRange(spreaderControls.ToArray());

            // ADD Cell
            int xCellPos = XCellSTART;
            List<Control> cellControls = new List<Control>();

            for (int i = 0; i < dtRowInfo.Rows.Count; i++)
            {
                int yCellPos = inventory.Height - cellINTERVAL - 5;
                string sRowID = dtRowInfo.Rows[i]["ROWID"].ToString();
                string sRowName = dtRowInfo.Rows[i]["ROWNAME"].ToString();
                string sWorkingLane = dtRowInfo.Rows[i]["WORKINGLANEFLAG"].ToString();

                if ("Yes".Equals(sWorkingLane))
                {
                    if ("1".Equals(sRowName))
                    {
                        xCellPos += 7;
                    }
                }

                ucContainer rowDisplayControl = new ucContainer(sCraneID, sRowName, "", xCellPos, yCellPos, true);

                cellControls.Add(rowDisplayControl);

                if ("Yes".Equals(sWorkingLane))
                {
                    yCellPos -= cellINTERVAL;

                    string sVehicle = "Vehicle";
                    ucContainer container = new ucContainer(sCraneID, sVehicle, sWorkingLane, xCellPos, yCellPos, false);

                    if (!mCellDic.ContainsKey(sVehicle))
                    {
                        mCellDic.Add(sVehicle, container);
                    }

                    //setVehicleDisplay(true);

                    cellControls.Add(container);

                    yCellPos -= cellINTERVAL;

                    string sVContainer = "VCON";
                    ucContainer vContainer = new ucContainer(sCraneID, sVContainer, sWorkingLane, xCellPos, yCellPos, false);

                    if (!mCellDic.ContainsKey(sVContainer))
                    {
                        mCellDic.Add(sVContainer, vContainer);
                    }

                    //setVehicleDisplay(true);

                    vContainer.Click += Container_Click;

                    cellControls.Add(vContainer);

                    if ("2".Equals(sRowName))
                    {
                        xCellPos += 7;
                    }
                }

                DataRow[] drTierList = dtTierInfo.Select("ROWID = '" + sRowID + "'", "TIERINDEX");

                if (drTierList.Length > 0)
                {
                    for (int y = 0; y < drTierList.Length; y++)
                    {
                        DataRow drTier = drTierList[y];

                        string equipmentID = sRowID + drTier["TIERNAME"].ToString();
                        string eqpState = "";// getEquipmentState();
                        string sContainer = drTier["CONTAINER"].ToString();

                        yCellPos -= cellINTERVAL;

                        if (!"".Equals(sContainer))
                        {
                            ucContainer container = new ucContainer(sCraneID, equipmentID, "", xCellPos, yCellPos, false);

                            //ShapeControl.CustomControl container = new ShapeControl.CustomControl(equipmentID, xCellPos, yCellPos);

                            //container.Shape = ShapeControl.ShapeType.Rectangle;
                            //container.Size = new Size(24, 24);
                            //container.Location = new Point(xCellPos, yCellPos);
                            //container.BorderWidth = 1;
                            //container.BackColor = Color.Blue;
                            //container.BorderColor = Color.Black;

                            container.Click += Container_Click;

                            if (!mCellDic.ContainsKey(equipmentID))
                            {
                                mCellDic.Add(equipmentID, container);
                            }

                            cellControls.Add(container);
                        }
                    }
                }
                xCellPos += cellINTERVAL;
            }
            inventory.Controls.AddRange(cellControls.ToArray());
        }

        public void drawInventory_Resize(string sCraneID, string sBayID, string sEquipmentStatus, DataTable dtRowInfo, DataTable dtTierInfo)
        {
            // 초기화
            clearDictionary();

            this.panel1.Width = this.Width;
            this.panel1.Height = this.Height - this.panel1.Location.Y;
            this.customPanel.Width = this.panel1.Width;
            this.customPanel.Height = this.panel1.Height;

            ucInventory inventory = this.inventory;
            inventory.Width = this.Width - inventory.Location.X - customPanel.BorderWidth;
            inventory.Height = this.panel1.Height - inventory.Location.Y - customPanel.BorderWidth;

            inventory.setBayInfo(sCraneID, sBayID, sEquipmentStatus);

            //inventory.Click += Inventory_Click;

            if (!mInventoryDic.ContainsKey(sCraneID))
            {
                mInventoryDic.Add(sCraneID, inventory);
            }

            // ADD Trolley
            List<Control> trolleyControls = new List<Control>();
            ucTrolley trolley = new ucTrolley(sCraneID, inventory.Width - 78, 20);

            if (!mTrolleyDic.ContainsKey(sCraneID))
            {
                mTrolleyDic.Add(sCraneID, trolley);
            }

            trolleyControls.Add(trolley);
            inventory.Controls.AddRange(trolleyControls.ToArray());

            // ADD Hoist
            List<Control> hoistControls = new List<Control>();
            ucHoist hoist = new ucHoist(sCraneID, inventory.Width - 68, 35, 30);

            if (!mHoistDic.ContainsKey(sCraneID))
            {
                mHoistDic.Add(sCraneID, hoist);
            }

            hoistControls.Add(hoist);
            inventory.Controls.AddRange(hoistControls.ToArray());

            // ADD Spreader
            List<Control> spreaderControls = new List<Control>();
            ucSpreader spreader = new ucSpreader(sCraneID, inventory.Width - 78, 65);

            if (!mSpreaderDic.ContainsKey(sCraneID))
            {
                mSpreaderDic.Add(sCraneID, spreader);
            }

            spreaderControls.Add(spreader);
            inventory.Controls.AddRange(spreaderControls.ToArray());

            if (dtRowInfo != null && dtRowInfo.Rows.Count > 0)
            { 

                // ADD Cell
                int workingBaseX = (int)(inventory.Width / 6);
                int workingBaseY = (int)(inventory.Height / 6);

                int rowWidth = (workingBaseX * 4) / dtRowInfo.Rows.Count;

                int xCellPos = workingBaseX;

                DataRow drTierIndex = dtTierInfo.Select("TIERINDEX > 0 ", "TIERINDEX DESC").FirstOrDefault();
                string maxTier = drTierIndex["TIERNAME"].ToString();

                int tierHeight = (workingBaseY * 4) / (int.Parse(maxTier) + 1);

                List<Control> cellControls = new List<Control>();

                for (int i = 0; i < dtRowInfo.Rows.Count; i++)
                {
                    int yCellPos = inventory.Height - tierHeight - 5;
                    string sRowID = dtRowInfo.Rows[i]["ROWID"].ToString();
                    string sRowName = dtRowInfo.Rows[i]["ROWNAME"].ToString();
                    string sWorkingLane = dtRowInfo.Rows[i]["WORKINGLANEFLAG"].ToString();

                    if ("Yes".Equals(sWorkingLane))
                    {
                        if ("1".Equals(sRowName))
                        {
                            xCellPos += 7;
                        }
                    }

                    ucContainer rowDisplayControl = new ucContainer(sCraneID, sRowName, "", xCellPos, yCellPos, true);
                    rowDisplayControl.Width = rowWidth;
                    rowDisplayControl.Height = tierHeight;

                    cellControls.Add(rowDisplayControl);

                    if ("Yes".Equals(sWorkingLane))
                    {
                        yCellPos -= tierHeight;

                        string sVehicle = "Vehicle";
                        ucContainer container = new ucContainer(sCraneID, sVehicle, sWorkingLane, xCellPos, yCellPos, false);
                        container.Width = rowWidth;
                        container.Height = tierHeight;

                        if (!mCellDic.ContainsKey(sVehicle))
                        {
                            mCellDic.Add(sVehicle, container);
                        }

                        //setVehicleDisplay(true);

                        cellControls.Add(container);

                        if ("2".Equals(sRowName))
                        {
                            xCellPos += 7;
                        }
                    }

                    DataRow[] drTierList = dtTierInfo.Select("ROWID = '" + sRowID + "'", "TIERINDEX");

                    if (drTierList.Length > 0)
                    {
                        for (int y = 0; y < drTierList.Length; y++)
                        {
                            DataRow drTier = drTierList[y];

                            string equipmentID = sRowID + drTier["TIERNAME"].ToString();
                            string eqpState = "";// getEquipmentState();
                            string sContainer = drTier["CONTAINER"].ToString();

                            yCellPos -= tierHeight;

                            if (!"".Equals(sContainer))
                            {
                                ucContainer container = new ucContainer(sCraneID, equipmentID, eqpState, xCellPos, yCellPos, false);
                                container.Width = rowWidth;
                                container.Height = tierHeight;
                                //ShapeControl.CustomControl container = new ShapeControl.CustomControl(equipmentID, xCellPos, yCellPos);

                                //container.Shape = ShapeControl.ShapeType.Rectangle;
                                //container.Size = new Size(24, 24);
                                //container.Location = new Point(xCellPos, yCellPos);
                                //container.BorderWidth = 1;
                                //container.BackColor = Color.Blue;
                                //container.BorderColor = Color.Black;

                                container.Click += Container_Click;

                                if (!mCellDic.ContainsKey(equipmentID))
                                {
                                    mCellDic.Add(equipmentID, container);
                                }

                                cellControls.Add(container);
                            }
                        }
                    }
                    xCellPos += rowWidth;
                }
            inventory.Controls.AddRange(cellControls.ToArray());
            }
        }

        private void Container_Click(object sender, EventArgs e)
        {
            ucContainer container = (ucContainer)sender;

            if (mCellDic.ContainsKey(container.mContainerID))
            {
                if ("VCON".Equals(container.mContainerID))
                {
                    txt3.Text = container.mContainerID;
                }
                else
                {
                    txt2.Text = container.mContainerID;
                }
            }
        }

        private void Work()
        {
            string sJobType = "";
            bool bPickFlag = false;
            bool bDefaultFlag = false;
            //int xDefaultPos = inventory.Width - 80;
            int yDefaultPos = 60;
            bool xPickPosFlag = false;
            bool bCompleteFlag = false;

            // Container
            if (txt1.Text == null || "".Equals(txt1.Text))
            {
                return;
            }

            if (txt2.Text == null || "".Equals(txt2.Text))
            {
                return;
            }

            string conID = txt2.Text;
            int xConPos = 0;
            int yConPos = 0;

            // Trolley
            string sCraneID = txt1.Text;
            int xTrolleyPos = 0;
            int yTrolleyPos = 0;
            int xSpreaderPos = 0;
            int ySpreaderPos = 0;
            int xVehiclePos = 0;
            int yVehiclePos = 0;
            int safetyPosition = 60;

            string sVehicle = "Vehicle";

            if (!"".Equals(txt3.Text))
            {
                conID = "VCON";
                sVehicle = txt2.Text;
            }

            if (mCellDic.ContainsKey(conID))
            {
                xConPos = mCellDic[conID].mXPos;
                yConPos = mCellDic[conID].mYPos;
            }

            if (mCellDic.ContainsKey(sVehicle))
            {
                xVehiclePos = mCellDic[sVehicle].mXPos;
                yVehiclePos = mCellDic[sVehicle].mYPos;
            }

            if (mTrolleyDic.ContainsKey(sCraneID))
            {
                xTrolleyPos = mTrolleyDic[sCraneID].mXPos;
                yTrolleyPos = mTrolleyDic[sCraneID].mYPos;

                bDefaultFlag = mTrolleyDic[sCraneID].mDefaultPositionFlag;
                xPickPosFlag = mTrolleyDic[sCraneID].mPickPositionFlag;
            }

            if (mSpreaderDic.ContainsKey(sCraneID))
            {
                xSpreaderPos = mSpreaderDic[sCraneID].mXPos;
                ySpreaderPos = mSpreaderDic[sCraneID].mYPos;

                bPickFlag = mSpreaderDic[sCraneID].mPickFlag;
                bCompleteFlag = mSpreaderDic[sCraneID].mCompleteFlag;
            }

            if (bCompleteFlag)
            {
                if (safetyPosition < ySpreaderPos)
                {
                    sJobType = "UP";
                }
                else
                {

                    if ("".Equals(txt3.Text))
                    {
                        mCellDic[conID].Dispose();
                        mCellDic.Remove(conID);
                    }

                    mSpreaderDic[sCraneID].mCompleteFlag = false;

                    //txt1.Text = "";
                    txt2.Text = "";
                    txt3.Text = "";

                    this.setBorderAnimate(false);
                    this.setVehicleDisplay(false);

                }
            }
            else if (!bPickFlag)
            {
                if (!xPickPosFlag && yDefaultPos < ySpreaderPos)
                {
                    sJobType = "UP";
                }
                else if (!bDefaultFlag && yDefaultPos == ySpreaderPos)
                {
                    mTrolleyDic[sCraneID].mDefaultPositionFlag = true;
                }
                else if (bDefaultFlag && !xPickPosFlag)
                {
                    if (xConPos < xTrolleyPos)
                    {
                        sJobType = "MCL";
                    }
                    else if (xConPos > xTrolleyPos)
                    {
                        sJobType = "MCR";
                    }
                    else if (xConPos == xTrolleyPos)
                    {
                        mTrolleyDic[sCraneID].mPickPositionFlag = true;
                    }
                }
                else if (xPickPosFlag)
                {
                    if (ySpreaderPos == yConPos - 9)
                    {
                        mSpreaderDic[sCraneID].mPickFlag = true;
                        mTrolleyDic[sCraneID].mDefaultPositionFlag = false;
                        mTrolleyDic[sCraneID].mPickPositionFlag = false;
                    }
                    else
                    {
                        sJobType = "DOWN";
                    }
                }
            }
            else
            {
                if (!bDefaultFlag && yDefaultPos < ySpreaderPos)
                {
                    sJobType = "PICKUP";
                }
                else if (!bDefaultFlag && yDefaultPos == ySpreaderPos)
                {
                    mTrolleyDic[sCraneID].mDefaultPositionFlag = true;
                }
                else if (bDefaultFlag && !xPickPosFlag)
                {
                    if (xVehiclePos < xTrolleyPos)
                    {
                        sJobType = "PICKMCL";
                    }
                    else if (xVehiclePos > xTrolleyPos)
                    {
                        sJobType = "PICKMCR";
                    }
                    else if (xVehiclePos == xTrolleyPos)
                    {
                        mTrolleyDic[sCraneID].mPickPositionFlag = true;
                    }
                }
                else if (xPickPosFlag)
                {
                    if (ySpreaderPos == yVehiclePos - 9 - 25)
                    {
                        mSpreaderDic[sCraneID].mPickFlag = false;
                        mSpreaderDic[sCraneID].mCompleteFlag = true;

                        mTrolleyDic[sCraneID].mDefaultPositionFlag = false;
                        mTrolleyDic[sCraneID].mPickPositionFlag = false;
                    }
                    else
                    {
                        sJobType = "PICKDOWN";
                    }
                }

            }

            RefreshTrolley(sJobType);
        }

        private void RefreshTrolley(string message)
        {
            if ("".Equals(txt1.Text) || "".Equals(txt2.Text))
            {
                return;
            }

            string sCraneID = txt1.Text;
            string sContainerID = txt2.Text;

            if (!"".Equals(txt3.Text))
            {
                sContainerID = "VCON";
            }

            int xPos = 0;
            int yPos = 0;
            int height = 0;

            if (message.Equals("MCL") || message.Equals("PICKMCL"))
            {
                xPos = -1;
            }
            else if (message.Equals("MCR") || message.Equals("PICKMCR"))
            {
                xPos = +1;
            }
            else if (message.Equals("DOWN") || message.Equals("PICKDOWN"))
            {
                height = 1;
            }
            else if (message.Equals("UP") || message.Equals("PICKUP"))
            {
                height = -1;
            }
            else if (message.Equals("MV"))
            {
                xPos = 1;
            }

            if (mTrolleyDic.ContainsKey(sCraneID))
            {
                mTrolleyDic[sCraneID].SetTrolleyLocation(xPos, yPos);
                mTrolleyDic[sCraneID].RefreshTrolley();
            }

            if (mHoistDic.ContainsKey(sCraneID))
            {
                mHoistDic[sCraneID].SetHoistLocation(xPos, yPos, height);
                mHoistDic[sCraneID].RefreshHoist();
            }

            if (mSpreaderDic.ContainsKey(sCraneID))
            {
                mSpreaderDic[sCraneID].SetSpreaderLocation(xPos, height);
                mSpreaderDic[sCraneID].RefreshSpreader();
            }

            if (message.Equals("PICKUP") || message.Equals("PICKDOWN"))
            {
                mCellDic[sContainerID].setContainerLocation(xPos, height);
            }

            if (message.Equals("PICKMCL") || message.Equals("PICKMCR"))
            {
                mCellDic[sContainerID].setContainerLocation(xPos, yPos);
            }
        }

        public void setBayStatus(string sStatus)
        {
            this.SuspendLayout();

            switch (sStatus)
            {
                case "Idle":
                    mFillBrush.Color = Common.m_cIdle;
                    break;
                case "Run":
                    mFillBrush.Color = Common.m_cRun;
                    break;
                case "Hold":
                    mFillBrush.Color = Common.m_cHold;
                    break;
                case "Down":
                    mFillBrush.Color = Common.m_cDown;
                    break;
                case "PM":
                    mFillBrush.Color = Common.m_cPM;
                    break;
                case "Maint":
                    mFillBrush.Color = Common.m_cRun;
                    break;
                default:
                    mFillBrush.Color = Color.Transparent;
                    break;
            }

            //txtState.Text = sState;
            //txtState.SelectionAlignment = HorizontalAlignment.Center;
            //txtState.BorderStyle = BorderStyle.None;

            //Pen outLinePen = new Pen(mFillBrush, 10);
            //Rectangle recPanel = panel1.Bounds;
            //mGraphic.DrawRectangle(outLinePen, recPanel);

            this.customPanel.BorderColor = mFillBrush.Color;

            this.ResumeLayout(false);
        }

        public void setRCSStatus(string sRCSID, string sRCSStatus, string sMode)
        {
            this.SuspendLayout();

            txtStatus.Text = sRCSID;
            txtStatus.SelectionAlignment = HorizontalAlignment.Center;
            txtStatus.BorderStyle = BorderStyle.None;

            txtMode.Text = sMode;

            this.ResumeLayout(false);
        }

        public void setGIContainer()
        {
            if (mCellDic.ContainsKey("VCON"))
            {
                ucContainer container = mCellDic["VCON"];

                container.BorderStyle = BorderStyle.FixedSingle;
                container.BackColor = Color.SkyBlue;
            }
        }

        public void settxt1(string txt)
        {
            this.txt1.Text = txt;
        }

        public void settxt2(string txt)
        {
            this.txt2.Text = txt;
        }

        public void RefreshInvetory()
        {
            if (string.IsNullOrEmpty(mCraneID))
            {
                return;
            }
            //setBayStatus();
        }
    }
}

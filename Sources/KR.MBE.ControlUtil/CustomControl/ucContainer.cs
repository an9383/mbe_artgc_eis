using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR.ITIER.UI.ControlUtil
{
    public partial class ucContainer : UserControl
    {
        
        #region var/const
        public string mContainerID, mCraneID;
        public int mXPos, mYPos;

        private bool mDrawFlag;
        #endregion

        //public event EventHandler ButtonClick;
        public ucContainer()
        {
            InitializeComponent();
        }
         
        public ucContainer(string craneID, string containerID, string workingLane, int xPos, int yPos, bool labelFlag )
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            mCraneID = craneID;
            mContainerID = containerID;

            mXPos = xPos;
            mYPos = yPos;

            this.Location = new Point( mXPos, mYPos );

            if (labelFlag)
            {
                lbRow.Text = containerID;
            }
            else if (!"Yes".Equals(workingLane))
            {
                this.BorderStyle = BorderStyle.FixedSingle;
                this.BackColor = Color.SkyBlue;

                //if ("".Equals(color))
                //{
                //    this.BackColor = Color.SkyBlue;
                //}
                //else
                //{
                //    System.Drawing.Color cTemp = new System.Drawing.Color();
                //    string[] sTempColor = color.Split(',');

                //    if (sTempColor.Length > 2)
                //    {
                //        cTemp = System.Drawing.Color.FromArgb(int.Parse(sTempColor[0]), int.Parse(sTempColor[1]), int.Parse(sTempColor[2]), int.Parse(sTempColor[3]));
                //        this.BackColor = cTemp;
                //    }
                //}
            }
        }

        #region func
        /// <summary>
        /// Container 위치 변경
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        public void setContainerLocation( int xPos, int yPos )
        {
            mXPos = mXPos + xPos;
            mYPos = mYPos + yPos;

            this.Location = new Point( mXPos, mYPos );
        }

        /// <summary>
        /// 작업 중인 Container를 Spreder 위치로 이동
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        public void pickContainer(int xPos, int yPos)
        {
            mXPos = xPos;
            mYPos = yPos;

            this.Location = new Point(mXPos, mYPos);
        }

        /// <summary>
        /// Container 색상 변경
        /// </summary>
        /// <param name="sARGB"></param>
        public void setContainerColor(string sARGB)
        {
            System.Drawing.Color cTemp = new System.Drawing.Color();
            string[] sTempColor = sARGB.Split(',');

            if (sTempColor.Length > 2)
            {
                cTemp = System.Drawing.Color.FromArgb(int.Parse(sTempColor[0]), int.Parse(sTempColor[1]), int.Parse(sTempColor[2]), int.Parse(sTempColor[3]));
                this.BackColor = cTemp;
            }
        }
        #endregion
    }
}

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
    public partial class ucContainer_30 : UserControl
    {
        
        #region var/const
        public string mTierName, mContainer;
        private int mXPos, mYPos;
        private Graphics mGraphic;
        private SolidBrush mFillBrush, mPenBrush;
        private Font mDrawFont;
        private Color m_cRun = Color.Blue, m_cIdle = Color.Orange, m_cPM = Color.AliceBlue, m_cDown = Color.Red;
        private bool mDrawFlag;
        #endregion

        //public event EventHandler ButtonClick;
        public ucContainer_30()
        {
            InitializeComponent();
        }

            #region ctor/dtor
        public ucContainer_30( string tierName, string container, int xPos, int yPos, bool drawFlag, bool labelFlag )
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            mTierName = tierName;
            mContainer = container;
            mXPos = xPos;
            mYPos = yPos;
            mDrawFlag = drawFlag;

            this.Location = new Point( mXPos, mYPos );

            if (!"".Equals(container))
            {
                this.BackColor = Color.SandyBrown;
            }

            if (labelFlag)
            {
                this.BorderStyle = BorderStyle.None;
                this.label1.Text = tierName;
            }

            if ("Yes".Equals(drawFlag))
            {
                this.BackgroundImage = imgList.Images["vehicle.png"];
            }

            
        }
        #endregion

        #region func
        public void SetEquipmentState( string equipmentState )
        {
            //mEquipmentState = equipmentState;
        }

        public void SetEquipmentLocation( int xPos, int yPos )
        {
            mXPos = mXPos + xPos;
            mYPos = mYPos + yPos;

            this.Location = new Point( mXPos, mYPos );
        }

        public void RefreshEquipment()
        {
            //if( string.IsNullOrEmpty( mEquipmentID ) )
            //{
            //    return;
            //}
            //DrawCell();
        }
        #endregion
    }
}

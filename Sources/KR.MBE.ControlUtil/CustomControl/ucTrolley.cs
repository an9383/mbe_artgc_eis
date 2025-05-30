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
    public partial class ucTrolley : UserControl
    {
        #region var/const
        public string mCraneID;
        public int mXPos, mYPos;
        public bool mDefaultPositionFlag = false, mPickPositionFlag = false;

        private Graphics mGraphic;
        private SolidBrush mFillBrush, mPenBrush;
        private Font mDrawFont;
        #endregion

        public ucTrolley()
        {
            InitializeComponent();
        }
            
        public ucTrolley( string craneID, int xPos, int yPos )
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            mCraneID = craneID;
            mXPos = xPos;
            mYPos = yPos;

            // GDI 변수 초기화
            mGraphic = this.CreateGraphics();
            //mFillBrush = new SolidBrush( Color.LightGray );
            //mPenBrush = new SolidBrush( Color.Black );
            //mDrawFont = new Font( "Arial", 8, FontStyle.Bold );
            this.Location = new Point( mXPos, mYPos );

            DrawTrolley();
        }

        #region func

        private void DrawTrolley()
        {
            this.SuspendLayout();

            //mGraphic.FillRectangle( mFillBrush, this.ClientRectangle );

            this.ResumeLayout( false );
        }

        public void SetTrolleyLocation( int xPos, int yPos )
        {
            mXPos = mXPos + xPos;
            mYPos = mYPos + yPos;

            this.Location = new Point( mXPos, mYPos );
        }

        public void SetSpreaderStatus(bool defaultPostionFlag, bool pickPositionFlag)
        {
            mDefaultPositionFlag = defaultPostionFlag;
            mPickPositionFlag = pickPositionFlag;
        }

        public void RefreshTrolley()
        {
            DrawTrolley();
        }
        #endregion
    }
}

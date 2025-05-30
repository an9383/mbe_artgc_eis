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
    public partial class ucSpreader : UserControl
    {
        #region var/const
        private string mCraneID;
        public int mXPos, mYPos;
        public bool mPickFlag = false, mCompleteFlag = false;
        private Graphics mGraphic;
        private SolidBrush mFillBrush, mPenBrush;
        private Font mDrawFont;
        #endregion

        public ucSpreader()
        {
            InitializeComponent();
        }

            #region ctor/dtor
        public ucSpreader( string craneID, int xPos, int yPos )
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
            this.Location = new Point( mXPos, mYPos );

            //DrawSpreader();
        }
        #endregion

        #region getter/setter

        #endregion

        #region func

        private void DrawSpreader()
        {
            this.SuspendLayout();

            //mGraphic.FillRectangle( mFillBrush, this.ClientRectangle );

            this.ResumeLayout( false );
        }

        public void SetSpreaderLocation( int xPos, int yPos )
        {
            mXPos = mXPos + xPos;
            mYPos = mYPos + yPos;

            this.Location = new Point( mXPos, mYPos );
        }

        public void SetSpreaderStatus(bool pickFlag)
        {
            mPickFlag = pickFlag;
        }

        public void RefreshSpreader()
        {
            DrawSpreader();
        }
        #endregion
    }
}

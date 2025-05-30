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
    public partial class ucHoist : UserControl
    {
        #region var/const
        private string mCraneID;
        public int mXPos, mYPos, mHeight;
        private Graphics mGraphic;
        private SolidBrush mFillBrush, mPenBrush;
        private Font mDrawFont;
        #endregion

        public ucHoist()
        {
            InitializeComponent();
        }

            #region ctor/dtor
        public ucHoist( string craneID, int xPos, int yPos, int height )
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
            mHeight = height;

            // GDI 변수 초기화
            mGraphic = this.CreateGraphics();
            //mFillBrush = new SolidBrush( Color.LightGray );
            //mPenBrush = new SolidBrush( Color.Black );
            //mDrawFont = new Font( "Arial", 8, FontStyle.Bold );
            this.Location = new Point( mXPos, mYPos );
            this.Height = mHeight;

            DrawHoist();
        }
        #endregion

        #region getter/setter

        #endregion

        #region func

        private void DrawHoist()
        {
            this.SuspendLayout();            

            //mGraphic.FillRectangle( mFillBrush, this.ClientRectangle );

            this.ResumeLayout( false );
        }

        public void SetHoistLocation( int xPos, int yPos, int height )
        {
            mXPos = mXPos + xPos;
            mYPos = mYPos + yPos;
            mHeight = mHeight + height;

            this.Location = new Point( mXPos, mYPos );
            this.Height = mHeight;
        }

        public void RefreshHoist()
        {
            DrawHoist();
        }
        #endregion


    }
}

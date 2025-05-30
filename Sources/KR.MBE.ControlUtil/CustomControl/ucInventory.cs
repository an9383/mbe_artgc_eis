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
    public partial class ucInventory : UserControl
    {
        public string mCraneID, mBayID, mBayState;
        private Graphics mGraphic;
        private SolidBrush mFillBrush, mPenBrush;
        private Font mDrawFont;

        public ucInventory()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            // GDI 변수 초기화
            mGraphic = this.CreateGraphics();
            mFillBrush = new SolidBrush(Color.Cyan);
            mPenBrush = new SolidBrush(Color.Black);
            mDrawFont = new Font("Arial", 8, FontStyle.Bold);
        }

        private void ucInventory_Paint(object sender, PaintEventArgs e)
        {
            //setBayStatus();
        }

        public void setBayInfo(string sCraneID, string sBayID, String sBayState)
        {
            mCraneID = sCraneID;
            mBayID = sBayID;
            mBayState = sBayState;
        }

        private void setBayStatus()
        {
            this.SuspendLayout();

            switch(mBayState)
            {
                case "Idle":
                    mFillBrush.Color = Color.Lime;
                    break;
                case "Run":
                    mFillBrush.Color = SystemColors.Highlight;
                    break;
                case "Hold":
                    mFillBrush.Color = Color.Red;
                    break;
                case "Down":
                    mFillBrush.Color = Color.Red;
                    break;
                case "PM":
                    mFillBrush.Color = Color.Yellow;
                    break;
                default:
                    mFillBrush.Color = Color.Transparent;
                    break;
            }

            Pen outLinePen = new Pen(mFillBrush, 10);
            mGraphic.DrawRectangle(outLinePen, this.ClientRectangle);

            this.ResumeLayout( false );
        }

        public void RefreshInvetory()
        {
            if( string.IsNullOrEmpty(mBayID) )
            {
                return;
            }
            setBayStatus();
        }
    }
}

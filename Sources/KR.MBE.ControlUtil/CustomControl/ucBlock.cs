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
    public partial class ucBlock : UserControl
    {
        public string mBlockID, mBlockName;
        public int mGantyMaxPosition;
        private int mXPos, mYPos;
        private Graphics mGraphic;
        private SolidBrush mPenBrush;
        private Font mDrawFont;

        public ucBlock()
        {
            InitializeComponent();
        }

        public ucBlock(string blockID, string blockName, int xPos, int yPos, int gantryMaxPosition)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.DoubleBuffer, true);

            mBlockID = blockID;
            mBlockName = blockName;
            mXPos = xPos;
            mYPos = yPos;
            mGantyMaxPosition = gantryMaxPosition;

            // GDI 변수 초기화
            mGraphic = this.CreateGraphics();
            mPenBrush = new SolidBrush(Color.Black);
            mDrawFont = new Font("Arial", 8, FontStyle.Bold);

            this.Location = new Point(mXPos, mYPos);
        }
        #region func

        private void DrawInventory()
        {
            this.SuspendLayout();

            if (!string.IsNullOrEmpty(mBlockName))
            {
                string drawString = mBlockName.Length > 1 ? mBlockName.Substring(0, 2) : mBlockName;
                mGraphic.DrawString(drawString, mDrawFont, mPenBrush, 1.0f, 1.0f);
            }

            this.ResumeLayout(false);
        }

        private void ucBlock_Paint(object sender, PaintEventArgs e)
        {
            DrawInventory();
        }

        public void RefreshInvetory()
        {
            if (string.IsNullOrEmpty(mBlockID))
            {
                return;
            }
            DrawInventory();
        }
        #endregion
    }
}

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
    public partial class ucBay : UserControl
    {
        public string mBlockID, mBayID;
        public int mXPos, mYPos;

        public ucBay()
        {
            InitializeComponent();
        }

        public ucBay(string bayID, int xPos, int yPos)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            mBayID = bayID;

            mXPos = xPos;
            mYPos = yPos;

            this.Location = new Point(mXPos, mYPos);
        }

        public ucBay(string blockID, string bayID, int xPos, int yPos)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            mBlockID = blockID;
            mBayID = bayID;

            mXPos = xPos;
            mYPos = yPos;

            this.Location = new Point(mXPos, mYPos);
        }
    }
}

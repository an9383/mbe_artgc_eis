using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR.MBE.UI.ControlUtil.CustomControl
{
    public partial class itierStatusLabel : UserControl
    {


        private string sTagID = string.Empty;
        [Category("CUSTOM"), Description("Custom")]
        public string TAGID
        {
            get { return sTagID; }
            set { sTagID = value; }
        }

        public enum StatusList { Gray, Run, Hold }
        private StatusList m_Status = StatusList.Gray;
        [Category("CUSTOM"), Description("Custom")]
        public StatusList STATUS
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
                this.pbStatus.Image = imgList.Images[m_Status.ToString()];
            }
        }

        private Boolean m_AutoFontSize = true;
        [Category("CUSTOM"), Description("Custom")]
        public Boolean AutoFontSize
        {
            get { return m_AutoFontSize; }
            set { m_AutoFontSize = value; }
        }

        [Category("CUSTOM"), Description("Custom")]
        public string StatusDescription
        {
            get 
            { 
                return txtDescription.Text; 
            }
            set 
            { 
                txtDescription.Text = value; AdjustFontSize();
            }
        }


        public itierStatusLabel()
        {
            InitializeComponent();
            this.STATUS = StatusList.Gray; // Gray
        }

        public void setControl(string sDescription, string sTagID)
        {
            this.BorderStyle = BorderStyle.None;
            this.STATUS = StatusList.Gray; // Gray

            this.StatusDescription = sDescription;
            this.TAGID = sTagID;
        }

        private void AdjustFontSize()
        {
            if (this.txtDescription.Text.Length > 0)
            {
                Graphics gp;
                SizeF sz;
                Single Faktor, FaktorX, FaktorY;

                gp = this.txtDescription.CreateGraphics();
                sz = gp.MeasureString(this.txtDescription.Text, this.txtDescription.Font);
                gp.Dispose();

                FaktorX = (this.txtDescription.Width) / sz.Width;
                FaktorY = (this.Height) / sz.Height;

                if (FaktorX > FaktorY)
                {
                    Faktor = FaktorY;
                }
                else
                {
                    Faktor = FaktorX;
                }

                this.txtDescription.Font = new Font(this.txtDescription.Font.Name, this.txtDescription.Font.SizeInPoints * (Faktor) - 1);
            }

        }

        protected override void OnResize(EventArgs e)
        {
            if (m_AutoFontSize) AdjustFontSize();
            base.OnResize(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (m_AutoFontSize) AdjustFontSize();
            base.OnTextChanged(e);
        }

        private void ucStatusLabel_Resize(object sender, EventArgs e)
        {
            pbStatus.Top = (this.Height - pbStatus.Height) / 2;
        }

        private void ucStatusLabel_BackColorChanged(object sender, EventArgs e)
        {
            if (this.BackColor.Equals(Color.Transparent)) return;
            txtDescription.BackColor = this.BackColor;
        }

        private void ucStatusLabel_ForeColorChanged(object sender, EventArgs e)
        {
            txtDescription.ForeColor = this.ForeColor;
        }

        private void ucStatusLabel_FontChanged(object sender, EventArgs e)
        {
            txtDescription.Font = this.Font;
        }
    }
}

using KR.MBE.Data.DataObjects;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace KR.MBE.UI.ControlUtil.CustomControl
{
    public partial class itierStatus : Label
    {
        private string sTagID = string.Empty;
        public enum StatusList { Gray, Run, Hold }

        private StatusList m_Status = StatusList.Gray;

        [Category("CUSTOM"), Description("Custom")]
        public string TAGID
        {
            get { return sTagID; }
            set { sTagID = value; }
        }

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
                this.Image = imgList.Images[m_Status.ToString()]; 
            }
        }


        public itierStatus()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.ImageAlign = ContentAlignment.MiddleLeft;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.STATUS = StatusList.Gray;
        }

        protected override void OnCreateControl()
        {
            AutoSize = false;
        }

        private Image _image;
        public new Image Image
        {
            get { return _image; }
            set
            {
                const int spacing = 4;
                if (_image != null)
                {
                    Padding = new Padding(Padding.Left - spacing - _image.Width, Padding.Top, Padding.Right, Padding.Bottom);
                }
                if (value != null)
                {
                    Padding = new Padding(Padding.Left + spacing + value.Width, Padding.Top, Padding.Right, Padding.Bottom);
                }
                _image = value;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image != null)
            {
                Rectangle r = CalcImageRenderBounds(Image, ClientRectangle, ImageAlign);
                e.Graphics.DrawImage(Image, r);
            }

            base.OnPaint(e); // Paint text
        }

        protected override void OnResize(EventArgs e)
        {
            AdjustFontSize();
            base.OnResize(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            AdjustFontSize();
            base.OnTextChanged(e);
        }

        private void AdjustFontSize()
        {
            if (this.Text.Length > 0)
            {
                Graphics gp;
                SizeF sz;
                Single Faktor, FaktorX, FaktorY;

                gp = this.CreateGraphics();
                sz = gp.MeasureString(this.Text, this.Font);
                gp.Dispose();

                FaktorX = (this.Width) / sz.Width;
                FaktorY = (this.Height) / sz.Height;

                if (FaktorX > FaktorY)
                {
                    Faktor = FaktorY;
                }
                else
                {
                    Faktor = FaktorX;
                }

                this.Font = new Font(this.Font.Name, this.Font.SizeInPoints * (Faktor) - 1);

            }

        }


    }
}

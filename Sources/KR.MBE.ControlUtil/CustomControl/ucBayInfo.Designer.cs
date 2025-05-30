
namespace KR.ITIER.UI.ControlUtil.CustomControl
{
    partial class ucBayInfo
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucBayInfo));
            this.lbBayInfo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtMode = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txt2 = new KR.ITIER.UI.ControlUtil.CustomControl.itierTextBox();
            this.txt1 = new KR.ITIER.UI.ControlUtil.CustomControl.itierTextBox();
            this.btn1 = new KR.ITIER.UI.ControlUtil.CustomControl.itierButton();
            this.inventory = new KR.ITIER.UI.ControlUtil.CustomControl.ucInventory();
            this.customPanel = new KR.ITIER.UI.ControlUtil.CustomControl.CustomControl();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.txt3 = new KR.ITIER.UI.ControlUtil.CustomControl.itierTextBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbBayInfo
            // 
            this.lbBayInfo.AutoSize = true;
            this.lbBayInfo.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbBayInfo.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbBayInfo.Location = new System.Drawing.Point(124, 7);
            this.lbBayInfo.Name = "lbBayInfo";
            this.lbBayInfo.Size = new System.Drawing.Size(50, 19);
            this.lbBayInfo.TabIndex = 2;
            this.lbBayInfo.Text = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtMode);
            this.groupBox1.Controls.Add(this.txtStatus);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox1.Size = new System.Drawing.Size(64, 106);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // txtMode
            // 
            this.txtMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMode.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtMode.Location = new System.Drawing.Point(6, 72);
            this.txtMode.Name = "txtMode";
            this.txtMode.Size = new System.Drawing.Size(52, 25);
            this.txtMode.TabIndex = 4;
            this.txtMode.Text = "Manual";
            // 
            // txtStatus
            // 
            this.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStatus.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtStatus.ForeColor = System.Drawing.Color.Red;
            this.txtStatus.Location = new System.Drawing.Point(6, 20);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(52, 37);
            this.txtStatus.TabIndex = 4;
            this.txtStatus.Text = "";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.txt3);
            this.panel1.Controls.Add(this.txt2);
            this.panel1.Controls.Add(this.txt1);
            this.panel1.Controls.Add(this.btn1);
            this.panel1.Controls.Add(this.inventory);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.customPanel);
            this.panel1.Location = new System.Drawing.Point(0, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(345, 265);
            this.panel1.TabIndex = 4;
            // 
            // txt2
            // 
            this.txt2.DecimalPoint = 2;
            this.txt2.Location = new System.Drawing.Point(9, 165);
            this.txt2.MaskType = KR.ITIER.UI.ControlUtil.CustomControl.Mask.None;
            this.txt2.MaxLength = 18;
            this.txt2.MaxValue = ((long)(9223372036854775807));
            this.txt2.Name = "txt2";
            this.txt2.Size = new System.Drawing.Size(48, 21);
            this.txt2.TabIndex = 7;
            // 
            // txt1
            // 
            this.txt1.DecimalPoint = 2;
            this.txt1.Location = new System.Drawing.Point(9, 138);
            this.txt1.MaskType = KR.ITIER.UI.ControlUtil.CustomControl.Mask.None;
            this.txt1.MaxLength = 18;
            this.txt1.MaxValue = ((long)(9223372036854775807));
            this.txt1.Name = "txt1";
            this.txt1.Size = new System.Drawing.Size(48, 21);
            this.txt1.TabIndex = 6;
            // 
            // btn1
            // 
            this.btn1.ActionType = KR.ITIER.UI.ControlUtil.CustomControl.itierButton.ActionTypeList.None;
            this.btn1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn1.DefaultScheme = true;
            this.btn1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btn1.Hint = "";
            this.btn1.Location = new System.Drawing.Point(20, 119);
            this.btn1.Name = "btn1";
            this.btn1.Scheme = PinkieControls.ButtonXP.Schemes.Blue;
            this.btn1.Size = new System.Drawing.Size(26, 13);
            this.btn1.TabIndex = 5;
            this.btn1.Text = "itierButton1";
            // 
            // inventory
            // 
            this.inventory.BackColor = System.Drawing.Color.LightCyan;
            this.inventory.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("inventory.BackgroundImage")));
            this.inventory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.inventory.Location = new System.Drawing.Point(78, 8);
            this.inventory.Name = "inventory";
            this.inventory.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.inventory.Size = new System.Drawing.Size(260, 250);
            this.inventory.TabIndex = 4;
            // 
            // customPanel
            // 
            this.customPanel.AnimateBorder = false;
            this.customPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.customPanel.Blink = false;
            this.customPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.customPanel.BorderStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this.customPanel.BorderWidth = 4;
            this.customPanel.CenterColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.customPanel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.customPanel.Location = new System.Drawing.Point(0, 0);
            this.customPanel.Margin = new System.Windows.Forms.Padding(0);
            this.customPanel.Name = "customPanel";
            this.customPanel.Shape = KR.ITIER.UI.ControlUtil.CustomControl.ShapeType.Rectangle;
            this.customPanel.ShapeImage = null;
            this.customPanel.Size = new System.Drawing.Size(345, 265);
            this.customPanel.SurroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.customPanel.TabIndex = 5;
            this.customPanel.Tag2 = "";
            this.customPanel.UseGradient = false;
            this.customPanel.Vibrate = false;
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "vehicle_225.png");
            // 
            // txt3
            // 
            this.txt3.DecimalPoint = 2;
            this.txt3.Location = new System.Drawing.Point(9, 212);
            this.txt3.MaskType = KR.ITIER.UI.ControlUtil.CustomControl.Mask.None;
            this.txt3.MaxLength = 18;
            this.txt3.MaxValue = ((long)(9223372036854775807));
            this.txt3.Name = "txt3";
            this.txt3.Size = new System.Drawing.Size(48, 21);
            this.txt3.TabIndex = 8;
            // 
            // ucBayInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lbBayInfo);
            this.Name = "ucBayInfo";
            this.Size = new System.Drawing.Size(345, 300);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label lbBayInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtMode;
        private System.Windows.Forms.RichTextBox txtStatus;
        private System.Windows.Forms.Panel panel1;
        public ucInventory inventory;
        private itierButton btn1;
        public itierTextBox txt1;
        public itierTextBox txt2;
        private CustomControl customPanel;
        private System.Windows.Forms.ImageList imgList;
        public itierTextBox txt3;
    }
}

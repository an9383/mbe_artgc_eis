namespace KR.MBE.UI.ControlUtil.CustomControl
{
    partial class itierEnumComboBox
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.LbLCmbUC = new System.Windows.Forms.Label();
            this.CmbUC = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LbLCmbUC);
            this.panel1.Controls.Add(this.CmbUC);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(350, 30);
            this.panel1.TabIndex = 0;
            // 
            // LbLCmbUC
            // 
            this.LbLCmbUC.AutoSize = true;
            this.LbLCmbUC.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LbLCmbUC.Location = new System.Drawing.Point(12, 6);
            this.LbLCmbUC.Name = "LbLCmbUC";
            this.LbLCmbUC.Size = new System.Drawing.Size(47, 16);
            this.LbLCmbUC.TabIndex = 0;
            this.LbLCmbUC.Text = "Label";
            // 
            // CmbUC
            // 
            this.CmbUC.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.CmbUC.FormattingEnabled = true;
            this.CmbUC.Location = new System.Drawing.Point(178, 3);
            this.CmbUC.Name = "CmbUC";
            this.CmbUC.Size = new System.Drawing.Size(158, 24);
            this.CmbUC.TabIndex = 1;
            // 
            // ucComboBox_smkang
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panel1);
            this.Name = "ucComboBox_smkang";
            this.Size = new System.Drawing.Size(350, 30);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox CmbUC;
        private System.Windows.Forms.Label LbLCmbUC;
    }
}

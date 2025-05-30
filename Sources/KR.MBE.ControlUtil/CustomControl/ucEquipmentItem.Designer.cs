
namespace KR.MBE.UI.ControlUtil.CustomControl
{
    partial class ucEquipmentItem
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
            this.lblEquipment = new System.Windows.Forms.Label();
            this.lblColor = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblEquipment
            // 
            this.lblEquipment.AutoSize = true;
            this.lblEquipment.Location = new System.Drawing.Point(13, 6);
            this.lblEquipment.Name = "lblEquipment";
            this.lblEquipment.Size = new System.Drawing.Size(99, 12);
            this.lblEquipment.TabIndex = 0;
            this.lblEquipment.Text = "설비명(설비코드)";
            // 
            // lblColor
            // 
            this.lblColor.Location = new System.Drawing.Point(173, 4);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(15, 15);
            this.lblColor.TabIndex = 1;
            this.lblColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ucEquipmentItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.lblEquipment);
            this.Name = "ucEquipmentItem";
            this.Size = new System.Drawing.Size(200, 23);
            this.Load += new System.EventHandler(this.ucEquipmentItem_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEquipment;
        private System.Windows.Forms.Label lblColor;
    }
}

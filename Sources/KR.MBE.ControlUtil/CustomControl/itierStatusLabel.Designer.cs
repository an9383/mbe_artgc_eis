
namespace KR.MBE.UI.ControlUtil.CustomControl
{
    partial class itierStatusLabel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(itierStatusLabel));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.splMain = new System.Windows.Forms.SplitContainer();
            this.pbStatus = new System.Windows.Forms.PictureBox();
            this.txtDescription = new KR.MBE.UI.ControlUtil.CustomControl.VerticalTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splMain)).BeginInit();
            this.splMain.Panel1.SuspendLayout();
            this.splMain.Panel2.SuspendLayout();
            this.splMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "Gray");
            this.imgList.Images.SetKeyName(1, "Run");
            this.imgList.Images.SetKeyName(2, "Hold");
            // 
            // splMain
            // 
            this.splMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splMain.IsSplitterFixed = true;
            this.splMain.Location = new System.Drawing.Point(0, 0);
            this.splMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splMain.Name = "splMain";
            // 
            // splMain.Panel1
            // 
            this.splMain.Panel1.Controls.Add(this.pbStatus);
            this.splMain.Panel1MinSize = 20;
            // 
            // splMain.Panel2
            // 
            this.splMain.Panel2.Controls.Add(this.txtDescription);
            this.splMain.Panel2MinSize = 20;
            this.splMain.Size = new System.Drawing.Size(217, 20);
            this.splMain.SplitterDistance = 25;
            this.splMain.SplitterWidth = 1;
            this.splMain.TabIndex = 0;
            // 
            // pbStatus
            // 
            this.pbStatus.Location = new System.Drawing.Point(2, 2);
            this.pbStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(16, 16);
            this.pbStatus.TabIndex = 9;
            this.pbStatus.TabStop = false;
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.SystemColors.Window;
            this.txtDescription.BorderColor = System.Drawing.Color.Empty;
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtDescription.LeftRightPadding = ((uint)(2u));
            this.txtDescription.Location = new System.Drawing.Point(0, 0);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(191, 20);
            this.txtDescription.TabIndex = 0;
            this.txtDescription.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // itierStatusLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splMain);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "itierStatusLabel";
            this.Size = new System.Drawing.Size(217, 20);
            this.BackColorChanged += new System.EventHandler(this.ucStatusLabel_BackColorChanged);
            this.FontChanged += new System.EventHandler(this.ucStatusLabel_FontChanged);
            this.ForeColorChanged += new System.EventHandler(this.ucStatusLabel_ForeColorChanged);
            this.Resize += new System.EventHandler(this.ucStatusLabel_Resize);
            this.splMain.Panel1.ResumeLayout(false);
            this.splMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splMain)).EndInit();
            this.splMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.SplitContainer splMain;
        private System.Windows.Forms.PictureBox pbStatus;
        private VerticalTextBox txtDescription;
    }
}

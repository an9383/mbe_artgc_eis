
namespace KR.ITIER.UI.ControlUtil
{
    partial class ucContainer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucContainer));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.lbRow = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "vehicle.png");
            this.imgList.Images.SetKeyName(1, "vehicle_26.png");
            this.imgList.Images.SetKeyName(2, "vehicle_225.png");
            this.imgList.Images.SetKeyName(3, "vehicle_clear.png");
            this.imgList.Images.SetKeyName(4, "gray.png");
            // 
            // lbRow
            // 
            this.lbRow.AutoSize = true;
            this.lbRow.Location = new System.Drawing.Point(4, 6);
            this.lbRow.Name = "lbRow";
            this.lbRow.Size = new System.Drawing.Size(0, 12);
            this.lbRow.TabIndex = 0;
            // 
            // ucContainer
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.lbRow);
            this.Name = "ucContainer";
            this.Size = new System.Drawing.Size(24, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Label lbRow;
    }
}

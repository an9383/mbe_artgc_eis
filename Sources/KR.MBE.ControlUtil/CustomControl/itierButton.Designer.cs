
namespace KR.MBE.UI.ControlUtil.CustomControl
{
    partial class itierButton
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(itierButton));
            this.imgList24 = new System.Windows.Forms.ImageList();
            this.imgList20 = new System.Windows.Forms.ImageList();
            this.SuspendLayout();
            // 
            // imgList24
            // 
            this.imgList24.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList24.ImageStream")));
            this.imgList24.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList24.Images.SetKeyName(0, "IconSearch.png");
            this.imgList24.Images.SetKeyName(1, "IconSave.png");
            // 
            // imgList20
            // 
            this.imgList20.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList20.ImageStream")));
            this.imgList20.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList20.Images.SetKeyName(0, "IconDelete.png");
            this.imgList20.Images.SetKeyName(1, "IconExcel.png");
            this.imgList20.Images.SetKeyName(2, "IconInsert.png");
            this.imgList20.Images.SetKeyName(3, "IconPrinter.png");
            this.imgList20.Images.SetKeyName(4, "IconUndo.png");
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imgList24;
        private System.Windows.Forms.ImageList imgList20;
    }
}

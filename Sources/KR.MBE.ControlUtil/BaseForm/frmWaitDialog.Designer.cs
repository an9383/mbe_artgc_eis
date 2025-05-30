namespace KR.MBE.UI.ControlUtil
{
    partial class frmWaitDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.picLoading = new System.Windows.Forms.PictureBox();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.AliceBlue;
            this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMain.Controls.Add(this.lblMessage);
            this.pnlMain.Controls.Add(this.picLoading);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(567, 93);
            this.pnlMain.TabIndex = 5;
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMessage.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblMessage.Location = new System.Drawing.Point(0, 51);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(565, 40);
            this.lblMessage.TabIndex = 9;
            this.lblMessage.Text = "Loading....";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picLoading
            // 
            this.picLoading.BackColor = System.Drawing.Color.Transparent;
            this.picLoading.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picLoading.Dock = System.Windows.Forms.DockStyle.Top;
            this.picLoading.Image = global::KR.MBE.UI.ControlUtil.Properties.Resources.wait;
            this.picLoading.InitialImage = global::KR.MBE.UI.ControlUtil.Properties.Resources.wait;
            this.picLoading.Location = new System.Drawing.Point(0, 0);
            this.picLoading.Name = "picLoading";
            this.picLoading.Size = new System.Drawing.Size(565, 51);
            this.picLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picLoading.TabIndex = 8;
            this.picLoading.TabStop = false;
            // 
            // frmWaitDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 93);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmWaitDialog";
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.PictureBox picLoading;
        private System.Windows.Forms.Label lblMessage;



    }
}
namespace KR.MBE.UI.ControlUtil
{
    partial class ErrorMessageBox
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lblErrorCode = new System.Windows.Forms.Label();
            this.txtErrorMessage = new System.Windows.Forms.TextBox();
            this.btnDetail = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtErrorDetailMessage = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtErrorDetailMessage);
            this.splitContainer1.Size = new System.Drawing.Size(424, 276);
            this.splitContainer1.SplitterDistance = 144;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.btnDetail);
            this.splitContainer2.Panel2.Controls.Add(this.btnClose);
            this.splitContainer2.Size = new System.Drawing.Size(424, 144);
            this.splitContainer2.SplitterDistance = 114;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lblErrorCode);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.txtErrorMessage);
            this.splitContainer3.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer3.Size = new System.Drawing.Size(424, 114);
            this.splitContainer3.SplitterDistance = 30;
            this.splitContainer3.TabIndex = 3;
            // 
            // lblErrorCode
            // 
            this.lblErrorCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblErrorCode.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblErrorCode.ForeColor = System.Drawing.Color.Red;
            this.lblErrorCode.Location = new System.Drawing.Point(0, 0);
            this.lblErrorCode.Name = "lblErrorCode";
            this.lblErrorCode.Size = new System.Drawing.Size(424, 30);
            this.lblErrorCode.TabIndex = 1;
            this.lblErrorCode.Text = "ErrorCode";
            this.lblErrorCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtErrorMessage
            // 
            this.txtErrorMessage.BackColor = System.Drawing.SystemColors.Control;
            this.txtErrorMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorMessage.Location = new System.Drawing.Point(3, 3);
            this.txtErrorMessage.Multiline = true;
            this.txtErrorMessage.Name = "txtErrorMessage";
            this.txtErrorMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErrorMessage.Size = new System.Drawing.Size(418, 74);
            this.txtErrorMessage.TabIndex = 2;
            // 
            // btnDetail
            // 
            this.btnDetail.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDetail.Location = new System.Drawing.Point(274, 0);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(75, 26);
            this.btnDetail.TabIndex = 10;
            this.btnDetail.Text = "자세히보기";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.Location = new System.Drawing.Point(349, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 26);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtErrorDetailMessage
            // 
            this.txtErrorDetailMessage.BackColor = System.Drawing.Color.White;
            this.txtErrorDetailMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorDetailMessage.Location = new System.Drawing.Point(0, 0);
            this.txtErrorDetailMessage.Multiline = true;
            this.txtErrorDetailMessage.Name = "txtErrorDetailMessage";
            this.txtErrorDetailMessage.ReadOnly = true;
            this.txtErrorDetailMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErrorDetailMessage.Size = new System.Drawing.Size(424, 128);
            this.txtErrorDetailMessage.TabIndex = 3;
            // 
            // ErrorMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 276);
            this.Controls.Add(this.splitContainer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorMessageBox";
            this.Text = "ErrorMessageBox";
            this.Load += new System.EventHandler(this.ErrorMessageBox_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblErrorCode;
        private System.Windows.Forms.TextBox txtErrorMessage;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TextBox txtErrorDetailMessage;
    }
}
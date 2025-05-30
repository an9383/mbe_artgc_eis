
namespace ARTGC.EIS
{
    partial class frmMain
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            MainMenu = new System.Windows.Forms.MenuStrip();
            MenuGrp1 = new System.Windows.Forms.ToolStripMenuItem();
            mnuCommStatus = new System.Windows.Forms.ToolStripMenuItem();
            MainTools = new System.Windows.Forms.ToolStrip();
            MainStatus = new System.Windows.Forms.StatusStrip();
            groupBox2 = new System.Windows.Forms.GroupBox();
            dgvTagList = new KR.MBE.UI.ControlUtil.CustomControl.itierGrid();
            cmdTagRefresh = new System.Windows.Forms.Button();
            tvStation = new KR.MBE.UI.ControlUtil.TreeViewBound();
            btnJobStart = new System.Windows.Forms.Button();
            btnTagEdit = new System.Windows.Forms.Button();
            cbJobEnd = new System.Windows.Forms.CheckBox();
            btnJobEnd = new System.Windows.Forms.Button();
            btnTestAcceptJob = new System.Windows.Forms.Button();
            timerEventChecker = new System.Windows.Forms.Timer(components);
            btnTestStepJob = new System.Windows.Forms.Button();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            panel1 = new System.Windows.Forms.Panel();
            button1 = new System.Windows.Forms.Button();
            MainMenu.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTagList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // MainMenu
            // 
            MainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { MenuGrp1 });
            MainMenu.Location = new System.Drawing.Point(0, 0);
            MainMenu.Name = "MainMenu";
            MainMenu.Size = new System.Drawing.Size(1430, 24);
            MainMenu.TabIndex = 0;
            MainMenu.Text = "menuStrip1";
            // 
            // MenuGrp1
            // 
            MenuGrp1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuCommStatus });
            MenuGrp1.Name = "MenuGrp1";
            MenuGrp1.Size = new System.Drawing.Size(50, 20);
            MenuGrp1.Text = "Menu";
            // 
            // mnuCommStatus
            // 
            mnuCommStatus.Name = "mnuCommStatus";
            mnuCommStatus.Size = new System.Drawing.Size(148, 22);
            mnuCommStatus.Text = "Comm Status";
            mnuCommStatus.Click += mnuCommStatus_Click;
            // 
            // MainTools
            // 
            MainTools.ImageScalingSize = new System.Drawing.Size(20, 20);
            MainTools.Location = new System.Drawing.Point(0, 24);
            MainTools.Name = "MainTools";
            MainTools.Size = new System.Drawing.Size(1430, 25);
            MainTools.TabIndex = 1;
            MainTools.Text = "toolStrip1";
            // 
            // MainStatus
            // 
            MainStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            MainStatus.Location = new System.Drawing.Point(0, 922);
            MainStatus.Name = "MainStatus";
            MainStatus.Size = new System.Drawing.Size(1430, 22);
            MainStatus.TabIndex = 2;
            MainStatus.Text = "statusStrip1";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(dgvTagList);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
            groupBox2.Location = new System.Drawing.Point(5, 5);
            groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox2.Size = new System.Drawing.Size(1155, 807);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Tag Infomation";
            // 
            // dgvTagList
            // 
            dgvTagList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTagList.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvTagList.EnterKeyNextCell = false;
            dgvTagList.IsExcelSaveFlag = false;
            dgvTagList.Location = new System.Drawing.Point(3, 19);
            dgvTagList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            dgvTagList.Name = "dgvTagList";
            dgvTagList.RowHeadersWidth = 51;
            dgvTagList.RowTemplate.Height = 23;
            dgvTagList.Size = new System.Drawing.Size(1149, 784);
            dgvTagList.TabIndex = 0;
            dgvTagList.CellClick += dgvTagList_CellClick;
            // 
            // cmdTagRefresh
            // 
            cmdTagRefresh.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
            cmdTagRefresh.Location = new System.Drawing.Point(94, 9);
            cmdTagRefresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmdTagRefresh.Name = "cmdTagRefresh";
            cmdTagRefresh.Size = new System.Drawing.Size(114, 38);
            cmdTagRefresh.TabIndex = 8;
            cmdTagRefresh.Text = "Tag Refresh";
            cmdTagRefresh.UseVisualStyleBackColor = true;
            cmdTagRefresh.Click += cmdTagRefresh_Click;
            // 
            // tvStation
            // 
            tvStation.DisplayMember = null;
            tvStation.Dock = System.Windows.Forms.DockStyle.Fill;
            tvStation.LevelMember = null;
            tvStation.Location = new System.Drawing.Point(5, 5);
            tvStation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tvStation.Name = "tvStation";
            tvStation.ParentMember = null;
            tvStation.RootParentValue = resources.GetObject("tvStation.RootParentValue");
            tvStation.Size = new System.Drawing.Size(251, 807);
            tvStation.SortMember = null;
            tvStation.TabIndex = 9;
            tvStation.ValueMember = null;
            // 
            // btnJobStart
            // 
            btnJobStart.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
            btnJobStart.Location = new System.Drawing.Point(479, 9);
            btnJobStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnJobStart.Name = "btnJobStart";
            btnJobStart.Size = new System.Drawing.Size(80, 38);
            btnJobStart.TabIndex = 17;
            btnJobStart.Text = "Job Start";
            btnJobStart.UseVisualStyleBackColor = true;
            // 
            // btnTagEdit
            // 
            btnTagEdit.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
            btnTagEdit.Location = new System.Drawing.Point(359, 9);
            btnTagEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnTagEdit.Name = "btnTagEdit";
            btnTagEdit.Size = new System.Drawing.Size(114, 38);
            btnTagEdit.TabIndex = 16;
            btnTagEdit.Text = "Job Process";
            btnTagEdit.UseVisualStyleBackColor = true;
            // 
            // cbJobEnd
            // 
            cbJobEnd.AutoSize = true;
            cbJobEnd.Location = new System.Drawing.Point(214, 19);
            cbJobEnd.Margin = new System.Windows.Forms.Padding(3, 14, 3, 4);
            cbJobEnd.Name = "cbJobEnd";
            cbJobEnd.Size = new System.Drawing.Size(90, 19);
            cbJobEnd.TabIndex = 15;
            cbJobEnd.Text = "AutoJobEnd";
            cbJobEnd.UseVisualStyleBackColor = true;
            // 
            // btnJobEnd
            // 
            btnJobEnd.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
            btnJobEnd.Location = new System.Drawing.Point(8, 9);
            btnJobEnd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnJobEnd.Name = "btnJobEnd";
            btnJobEnd.Size = new System.Drawing.Size(80, 38);
            btnJobEnd.TabIndex = 14;
            btnJobEnd.Text = "Job End";
            btnJobEnd.UseVisualStyleBackColor = true;
            // 
            // btnTestAcceptJob
            // 
            btnTestAcceptJob.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
            btnTestAcceptJob.Location = new System.Drawing.Point(565, 9);
            btnTestAcceptJob.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnTestAcceptJob.Name = "btnTestAcceptJob";
            btnTestAcceptJob.Size = new System.Drawing.Size(114, 38);
            btnTestAcceptJob.TabIndex = 18;
            btnTestAcceptJob.Text = "TEST AcceptJob";
            btnTestAcceptJob.UseVisualStyleBackColor = true;
            btnTestAcceptJob.Click += btnTestAcceptJob_Click;
            // 
            // timerEventChecker
            // 
            timerEventChecker.Interval = 1000;
            timerEventChecker.Tick += timerEventChecker_Tick;
            // 
            // btnTestStepJob
            // 
            btnTestStepJob.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
            btnTestStepJob.Location = new System.Drawing.Point(685, 9);
            btnTestStepJob.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnTestStepJob.Name = "btnTestStepJob";
            btnTestStepJob.Size = new System.Drawing.Size(114, 38);
            btnTestStepJob.TabIndex = 19;
            btnTestStepJob.Text = "TEST StepJob";
            btnTestStepJob.UseVisualStyleBackColor = true;
            btnTestStepJob.Click += btnTestStepJob_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer1.Location = new System.Drawing.Point(0, 105);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tvStation);
            splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(5);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(groupBox2);
            splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(5);
            splitContainer1.Size = new System.Drawing.Size(1430, 817);
            splitContainer1.SplitterDistance = 261;
            splitContainer1.TabIndex = 20;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnJobEnd);
            flowLayoutPanel1.Controls.Add(cmdTagRefresh);
            flowLayoutPanel1.Controls.Add(cbJobEnd);
            flowLayoutPanel1.Controls.Add(panel1);
            flowLayoutPanel1.Controls.Add(btnTagEdit);
            flowLayoutPanel1.Controls.Add(btnJobStart);
            flowLayoutPanel1.Controls.Add(btnTestAcceptJob);
            flowLayoutPanel1.Controls.Add(btnTestStepJob);
            flowLayoutPanel1.Controls.Add(button1);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 49);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(10);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            flowLayoutPanel1.Size = new System.Drawing.Size(1430, 56);
            flowLayoutPanel1.TabIndex = 21;
            // 
            // panel1
            // 
            panel1.Location = new System.Drawing.Point(310, 8);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(43, 39);
            panel1.TabIndex = 20;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(805, 8);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(109, 39);
            button1.TabIndex = 21;
            button1.Text = "TestMoveEvent";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1430, 944);
            Controls.Add(splitContainer1);
            Controls.Add(MainStatus);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(MainTools);
            Controls.Add(MainMenu);
            MainMenuStrip = MainMenu;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "frmMain";
            Text = "ARTGC EIS";
            FormClosing += frmMain_FormClosing;
            Load += frmMain_Load;
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTagList).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStrip MainTools;
        private System.Windows.Forms.StatusStrip MainStatus;
        private System.Windows.Forms.ToolStripMenuItem MenuGrp1;
        private System.Windows.Forms.ToolStripMenuItem mnuCommStatus;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdTagRefresh;
        private KR.MBE.UI.ControlUtil.CustomControl.itierGrid dgvTagList;
        private KR.MBE.UI.ControlUtil.TreeViewBound tvStation;
        private System.Windows.Forms.Button btnJobStart;
        private System.Windows.Forms.Button btnTagEdit;
        private System.Windows.Forms.CheckBox cbJobEnd;
        private System.Windows.Forms.Button btnJobEnd;
        private System.Windows.Forms.Button btnTestAcceptJob;
        private System.Windows.Forms.Timer timerEventChecker;
        private System.Windows.Forms.Button btnTestStepJob;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
    }
}


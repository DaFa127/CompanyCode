
namespace Order_Create
{
    partial class Order_Create
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Order_Create));
            this.dgv = new System.Windows.Forms.DataGridView();
            this.cbb_OrderType = new System.Windows.Forms.ComboBox();
            this.btn_OrderCreated = new System.Windows.Forms.Button();
            this.btn_OrderUpdate = new System.Windows.Forms.Button();
            this.DTP = new System.Windows.Forms.DateTimePicker();
            this.btn_status = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.Menu_Plan = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Plan_Main = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Plan_Temporary = new System.Windows.Forms.ToolStripMenuItem();
            this.Meun_Query = new System.Windows.Forms.ToolStripMenuItem();
            this.Posting_difference = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Login = new System.Windows.Forms.ToolStripMenuItem();
            this.Lonin = new System.Windows.Forms.ToolStripMenuItem();
            this.Logout = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_SAP_Update = new System.Windows.Forms.Button();
            this.bgWorker_Created = new System.ComponentModel.BackgroundWorker();
            this.bgWorker_Return = new System.ComponentModel.BackgroundWorker();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.bgWorker_Update = new System.ComponentModel.BackgroundWorker();
            this.Order_bar = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(12, 85);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowTemplate.Height = 23;
            this.dgv.Size = new System.Drawing.Size(1305, 567);
            this.dgv.TabIndex = 0;
            // 
            // cbb_OrderType
            // 
            this.cbb_OrderType.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbb_OrderType.FormattingEnabled = true;
            this.cbb_OrderType.Location = new System.Drawing.Point(214, 38);
            this.cbb_OrderType.Name = "cbb_OrderType";
            this.cbb_OrderType.Size = new System.Drawing.Size(131, 35);
            this.cbb_OrderType.TabIndex = 1;
            this.cbb_OrderType.SelectedIndexChanged += new System.EventHandler(this.cbb_OrderType_SelectedIndexChanged);
            // 
            // btn_OrderCreated
            // 
            this.btn_OrderCreated.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_OrderCreated.Location = new System.Drawing.Point(827, 32);
            this.btn_OrderCreated.Name = "btn_OrderCreated";
            this.btn_OrderCreated.Size = new System.Drawing.Size(115, 45);
            this.btn_OrderCreated.TabIndex = 2;
            this.btn_OrderCreated.Text = "工单资料建立";
            this.btn_OrderCreated.UseVisualStyleBackColor = true;
            this.btn_OrderCreated.Click += new System.EventHandler(this.btn_OrderCreated_Click);
            // 
            // btn_OrderUpdate
            // 
            this.btn_OrderUpdate.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_OrderUpdate.Location = new System.Drawing.Point(1014, 32);
            this.btn_OrderUpdate.Name = "btn_OrderUpdate";
            this.btn_OrderUpdate.Size = new System.Drawing.Size(84, 45);
            this.btn_OrderUpdate.TabIndex = 3;
            this.btn_OrderUpdate.Text = "工单上传";
            this.btn_OrderUpdate.UseVisualStyleBackColor = true;
            this.btn_OrderUpdate.Click += new System.EventHandler(this.btn_OrderUpdate_Click);
            // 
            // DTP
            // 
            this.DTP.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DTP.Location = new System.Drawing.Point(12, 38);
            this.DTP.Name = "DTP";
            this.DTP.Size = new System.Drawing.Size(180, 35);
            this.DTP.TabIndex = 4;
            this.DTP.Value = new System.DateTime(2021, 4, 19, 0, 0, 0, 0);
            // 
            // btn_status
            // 
            this.btn_status.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_status.Location = new System.Drawing.Point(639, 32);
            this.btn_status.Name = "btn_status";
            this.btn_status.Size = new System.Drawing.Size(116, 45);
            this.btn_status.TabIndex = 5;
            this.btn_status.Text = "工单状态查询";
            this.btn_status.UseVisualStyleBackColor = true;
            this.btn_status.Click += new System.EventHandler(this.btn_status_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 669);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "label1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(193, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Plan,
            this.Meun_Query,
            this.Menu_Login});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1329, 25);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Menu_Plan
            // 
            this.Menu_Plan.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Plan_Main,
            this.Menu_Plan_Temporary});
            this.Menu_Plan.Name = "Menu_Plan";
            this.Menu_Plan.Size = new System.Drawing.Size(68, 21);
            this.Menu_Plan.Text = "排程相关";
            // 
            // Menu_Plan_Main
            // 
            this.Menu_Plan_Main.Name = "Menu_Plan_Main";
            this.Menu_Plan_Main.Size = new System.Drawing.Size(124, 22);
            this.Menu_Plan_Main.Text = "加硫排程";
            this.Menu_Plan_Main.Click += new System.EventHandler(this.Menu_Plan_Main_Click);
            // 
            // Menu_Plan_Temporary
            // 
            this.Menu_Plan_Temporary.Name = "Menu_Plan_Temporary";
            this.Menu_Plan_Temporary.Size = new System.Drawing.Size(124, 22);
            this.Menu_Plan_Temporary.Text = "插单维护";
            this.Menu_Plan_Temporary.Click += new System.EventHandler(this.Menu_Plan_Temporary_Click);
            // 
            // Meun_Query
            // 
            this.Meun_Query.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Posting_difference,
            this.Order_bar});
            this.Meun_Query.Name = "Meun_Query";
            this.Meun_Query.Size = new System.Drawing.Size(68, 21);
            this.Meun_Query.Text = "各类查询";
            // 
            // Posting_difference
            // 
            this.Posting_difference.Name = "Posting_difference";
            this.Posting_difference.Size = new System.Drawing.Size(180, 22);
            this.Posting_difference.Text = "报工数据比对";
            this.Posting_difference.Click += new System.EventHandler(this.Posting_difference_Click);
            // 
            // Menu_Login
            // 
            this.Menu_Login.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Lonin,
            this.Logout});
            this.Menu_Login.Name = "Menu_Login";
            this.Menu_Login.Size = new System.Drawing.Size(68, 21);
            this.Menu_Login.Text = "账号登录";
            // 
            // Lonin
            // 
            this.Lonin.Name = "Lonin";
            this.Lonin.Size = new System.Drawing.Size(180, 22);
            this.Lonin.Text = "登入";
            this.Lonin.Click += new System.EventHandler(this.Lonin_Click);
            // 
            // Logout
            // 
            this.Logout.Name = "Logout";
            this.Logout.Size = new System.Drawing.Size(180, 22);
            this.Logout.Text = "登出";
            this.Logout.Click += new System.EventHandler(this.Logout_Click);
            // 
            // btn_SAP_Update
            // 
            this.btn_SAP_Update.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_SAP_Update.Location = new System.Drawing.Point(1170, 32);
            this.btn_SAP_Update.Name = "btn_SAP_Update";
            this.btn_SAP_Update.Size = new System.Drawing.Size(147, 45);
            this.btn_SAP_Update.TabIndex = 10;
            this.btn_SAP_Update.Text = "更新SAP处理结果";
            this.btn_SAP_Update.UseVisualStyleBackColor = true;
            this.btn_SAP_Update.Click += new System.EventHandler(this.btn_SAP_Update_Click);
            // 
            // bgWorker_Created
            // 
            this.bgWorker_Created.WorkerReportsProgress = true;
            this.bgWorker_Created.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_Created_DoWork);
            this.bgWorker_Created.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_Created_ProgressChanged);
            this.bgWorker_Created.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_Created_RunWorkerCompleted);
            // 
            // bgWorker_Return
            // 
            this.bgWorker_Return.WorkerReportsProgress = true;
            this.bgWorker_Return.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_Return_DoWork);
            this.bgWorker_Return.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_Return_ProgressChanged);
            this.bgWorker_Return.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_Return_RunWorkerCompleted);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(560, 660);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(757, 42);
            this.progressBar.TabIndex = 12;
            this.progressBar.Visible = false;
            // 
            // bgWorker_Update
            // 
            this.bgWorker_Update.WorkerReportsProgress = true;
            this.bgWorker_Update.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_Update_DoWork);
            this.bgWorker_Update.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_Update_ProgressChanged);
            this.bgWorker_Update.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_Update_RunWorkerCompleted);
            // 
            // Order_bar
            // 
            this.Order_bar.Name = "Order_bar";
            this.Order_bar.Size = new System.Drawing.Size(180, 22);
            this.Order_bar.Text = "工单条码查询";
            this.Order_bar.Click += new System.EventHandler(this.Order_bar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(573, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 19);
            this.label2.TabIndex = 13;
            this.label2.Text = "步骤1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(761, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 19);
            this.label3.TabIndex = 14;
            this.label3.Text = "步骤2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(948, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 19);
            this.label4.TabIndex = 15;
            this.label4.Text = "步骤3";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(1104, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 19);
            this.label5.TabIndex = 16;
            this.label5.Text = "步骤4";
            // 
            // Order_Create
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1329, 714);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btn_SAP_Update);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_status);
            this.Controls.Add(this.DTP);
            this.Controls.Add(this.btn_OrderUpdate);
            this.Controls.Add(this.btn_OrderCreated);
            this.Controls.Add(this.cbb_OrderType);
            this.Controls.Add(this.dgv);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Order_Create";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "加硫工单";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.ComboBox cbb_OrderType;
        private System.Windows.Forms.Button btn_OrderCreated;
        private System.Windows.Forms.Button btn_OrderUpdate;
        private System.Windows.Forms.DateTimePicker DTP;
        private System.Windows.Forms.Button btn_status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Menu_Plan;
        private System.Windows.Forms.ToolStripMenuItem Menu_Plan_Main;
        private System.Windows.Forms.ToolStripMenuItem Menu_Plan_Temporary;
        private System.Windows.Forms.Button btn_SAP_Update;
        private System.ComponentModel.BackgroundWorker bgWorker_Created;
        private System.ComponentModel.BackgroundWorker bgWorker_Return;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker bgWorker_Update;
        private System.Windows.Forms.ToolStripMenuItem Meun_Query;
        private System.Windows.Forms.ToolStripMenuItem Posting_difference;
        private System.Windows.Forms.ToolStripMenuItem Menu_Login;
        private System.Windows.Forms.ToolStripMenuItem Lonin;
        private System.Windows.Forms.ToolStripMenuItem Logout;
        private System.Windows.Forms.ToolStripMenuItem Order_bar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}



namespace Order_Create
{
    partial class Posting
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
            this.DTP = new System.Windows.Forms.DateTimePicker();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btn_query = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bgWorker_Query = new System.ComponentModel.BackgroundWorker();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btn_update_diff = new System.Windows.Forms.Button();
            this.btn_back = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // DTP
            // 
            this.DTP.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DTP.Location = new System.Drawing.Point(110, 12);
            this.DTP.Name = "DTP";
            this.DTP.Size = new System.Drawing.Size(189, 34);
            this.DTP.TabIndex = 0;
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(12, 52);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowTemplate.Height = 23;
            this.dgv.Size = new System.Drawing.Size(1339, 474);
            this.dgv.TabIndex = 1;
            // 
            // btn_query
            // 
            this.btn_query.Location = new System.Drawing.Point(1267, 12);
            this.btn_query.Name = "btn_query";
            this.btn_query.Size = new System.Drawing.Size(84, 34);
            this.btn_query.TabIndex = 2;
            this.btn_query.Text = "查询";
            this.btn_query.UseVisualStyleBackColor = true;
            this.btn_query.Click += new System.EventHandler(this.btn_query_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 27);
            this.label1.TabIndex = 3;
            this.label1.Text = "报工日期";
            // 
            // bgWorker_Query
            // 
            this.bgWorker_Query.WorkerReportsProgress = true;
            this.bgWorker_Query.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_Query_DoWork);
            this.bgWorker_Query.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_Query_ProgressChanged);
            this.bgWorker_Query.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_Query_RunWorkerCompleted);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(338, 12);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(889, 34);
            this.progressBar.TabIndex = 4;
            // 
            // btn_update_diff
            // 
            this.btn_update_diff.Location = new System.Drawing.Point(1255, 532);
            this.btn_update_diff.Name = "btn_update_diff";
            this.btn_update_diff.Size = new System.Drawing.Size(96, 42);
            this.btn_update_diff.TabIndex = 5;
            this.btn_update_diff.Text = "差异上传";
            this.btn_update_diff.UseVisualStyleBackColor = true;
            this.btn_update_diff.Click += new System.EventHandler(this.btn_update_diff_Click);
            // 
            // btn_back
            // 
            this.btn_back.Location = new System.Drawing.Point(12, 536);
            this.btn_back.Name = "btn_back";
            this.btn_back.Size = new System.Drawing.Size(84, 34);
            this.btn_back.TabIndex = 6;
            this.btn_back.Text = "返回";
            this.btn_back.UseVisualStyleBackColor = true;
            this.btn_back.Click += new System.EventHandler(this.btn_back_Click);
            // 
            // Posting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1363, 578);
            this.Controls.Add(this.btn_back);
            this.Controls.Add(this.btn_update_diff);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_query);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.DTP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Posting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "报工数据比对";
            this.Load += new System.EventHandler(this.Posting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker DTP;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btn_query;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker bgWorker_Query;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btn_update_diff;
        private System.Windows.Forms.Button btn_back;
    }
}
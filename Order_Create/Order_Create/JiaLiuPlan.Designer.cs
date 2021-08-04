
namespace Order_Create
{
    partial class JiaLiuPlan
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.cbb_week = new System.Windows.Forms.ComboBox();
            this.lab_Title = new System.Windows.Forms.Label();
            this.dgv_now = new System.Windows.Forms.DataGridView();
            this.dgv_next = new System.Windows.Forms.DataGridView();
            this.lab_next = new System.Windows.Forms.Label();
            this.lab_now = new System.Windows.Forms.Label();
            this.lab_machine = new System.Windows.Forms.Label();
            this.cbb_place = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lab_day = new System.Windows.Forms.Label();
            this.btn_update = new System.Windows.Forms.Button();
            this.btn_Plan_Day = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_now)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_next)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(1, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(760, 717);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox2.Location = new System.Drawing.Point(758, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(733, 717);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // cbb_week
            // 
            this.cbb_week.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_week.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbb_week.FormattingEnabled = true;
            this.cbb_week.Location = new System.Drawing.Point(767, 40);
            this.cbb_week.Name = "cbb_week";
            this.cbb_week.Size = new System.Drawing.Size(112, 29);
            this.cbb_week.TabIndex = 3;
            this.cbb_week.SelectedIndexChanged += new System.EventHandler(this.cbb_week_SelectedIndexChanged);
            // 
            // lab_Title
            // 
            this.lab_Title.AutoSize = true;
            this.lab_Title.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Title.Location = new System.Drawing.Point(1358, 42);
            this.lab_Title.Name = "lab_Title";
            this.lab_Title.Size = new System.Drawing.Size(65, 27);
            this.lab_Title.TabIndex = 4;
            this.lab_Title.Text = "week";
            // 
            // dgv_now
            // 
            this.dgv_now.AllowUserToAddRows = false;
            this.dgv_now.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_now.Location = new System.Drawing.Point(767, 122);
            this.dgv_now.Name = "dgv_now";
            this.dgv_now.RowTemplate.Height = 23;
            this.dgv_now.Size = new System.Drawing.Size(713, 250);
            this.dgv_now.TabIndex = 5;
            this.dgv_now.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_now_CellClick);
            this.dgv_now.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgv_now_ColumnWidthChanged);
            this.dgv_now.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgv_now_Scroll);
            // 
            // dgv_next
            // 
            this.dgv_next.AllowUserToAddRows = false;
            this.dgv_next.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_next.Location = new System.Drawing.Point(767, 412);
            this.dgv_next.Name = "dgv_next";
            this.dgv_next.RowTemplate.Height = 23;
            this.dgv_next.Size = new System.Drawing.Size(713, 250);
            this.dgv_next.TabIndex = 6;
            this.dgv_next.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_next_CellClick);
            this.dgv_next.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_next_CellValueChanged);
            this.dgv_next.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgv_next_ColumnWidthChanged);
            this.dgv_next.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgv_next_EditingControlShowing);
            this.dgv_next.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgv_next_Scroll);
            // 
            // lab_next
            // 
            this.lab_next.AutoSize = true;
            this.lab_next.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_next.Location = new System.Drawing.Point(1065, 382);
            this.lab_next.Name = "lab_next";
            this.lab_next.Size = new System.Drawing.Size(52, 27);
            this.lab_next.TabIndex = 7;
            this.lab_next.Text = "下周";
            // 
            // lab_now
            // 
            this.lab_now.AutoSize = true;
            this.lab_now.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_now.Location = new System.Drawing.Point(1065, 92);
            this.lab_now.Name = "lab_now";
            this.lab_now.Size = new System.Drawing.Size(52, 27);
            this.lab_now.TabIndex = 8;
            this.lab_now.Text = "本周";
            // 
            // lab_machine
            // 
            this.lab_machine.AutoSize = true;
            this.lab_machine.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_machine.Location = new System.Drawing.Point(1065, 65);
            this.lab_machine.Name = "lab_machine";
            this.lab_machine.Size = new System.Drawing.Size(52, 27);
            this.lab_machine.TabIndex = 9;
            this.lab_machine.Text = "机台";
            // 
            // cbb_place
            // 
            this.cbb_place.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_place.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbb_place.FormattingEnabled = true;
            this.cbb_place.Location = new System.Drawing.Point(905, 40);
            this.cbb_place.Name = "cbb_place";
            this.cbb_place.Size = new System.Drawing.Size(108, 29);
            this.cbb_place.TabIndex = 10;
            this.cbb_place.SelectedIndexChanged += new System.EventHandler(this.cbb_place_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(932, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 27);
            this.label1.TabIndex = 11;
            this.label1.Text = "车间";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(797, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 27);
            this.label2.TabIndex = 12;
            this.label2.Text = "周数";
            // 
            // lab_day
            // 
            this.lab_day.AutoSize = true;
            this.lab_day.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_day.Location = new System.Drawing.Point(1325, 9);
            this.lab_day.Name = "lab_day";
            this.lab_day.Size = new System.Drawing.Size(48, 27);
            this.lab_day.TabIndex = 13;
            this.lab_day.Text = "day";
            // 
            // btn_update
            // 
            this.btn_update.Location = new System.Drawing.Point(1396, 670);
            this.btn_update.Name = "btn_update";
            this.btn_update.Size = new System.Drawing.Size(84, 31);
            this.btn_update.TabIndex = 14;
            this.btn_update.Text = "上传排程";
            this.btn_update.UseVisualStyleBackColor = true;
            this.btn_update.Click += new System.EventHandler(this.btn_update_Click);
            // 
            // btn_Plan_Day
            // 
            this.btn_Plan_Day.Location = new System.Drawing.Point(768, 669);
            this.btn_Plan_Day.Name = "btn_Plan_Day";
            this.btn_Plan_Day.Size = new System.Drawing.Size(93, 32);
            this.btn_Plan_Day.TabIndex = 15;
            this.btn_Plan_Day.Text = "机台日排程";
            this.btn_Plan_Day.UseVisualStyleBackColor = true;
            this.btn_Plan_Day.Click += new System.EventHandler(this.btn_Plan_Day_Click_1);
            // 
            // JiaLiuPlan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1492, 713);
            this.Controls.Add(this.btn_Plan_Day);
            this.Controls.Add(this.btn_update);
            this.Controls.Add(this.dgv_next);
            this.Controls.Add(this.lab_day);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbb_place);
            this.Controls.Add(this.lab_machine);
            this.Controls.Add(this.lab_now);
            this.Controls.Add(this.lab_next);
            this.Controls.Add(this.dgv_now);
            this.Controls.Add(this.lab_Title);
            this.Controls.Add(this.cbb_week);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "JiaLiuPlan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "加硫排程";
            this.Load += new System.EventHandler(this.JiaLiuPlan_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_now)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_next)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ComboBox cbb_week;
        private System.Windows.Forms.Label lab_Title;
        private System.Windows.Forms.DataGridView dgv_now;
        private System.Windows.Forms.DataGridView dgv_next;
        private System.Windows.Forms.Label lab_next;
        private System.Windows.Forms.Label lab_now;
        private System.Windows.Forms.Label lab_machine;
        private System.Windows.Forms.ComboBox cbb_place;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lab_day;
        private System.Windows.Forms.Button btn_update;
        private System.Windows.Forms.Button btn_Plan_Day;
    }
}
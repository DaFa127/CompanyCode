
namespace Order_Create
{
    partial class Plan_Day
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
            this.dgv_Day = new System.Windows.Forms.DataGridView();
            this.btn_Change = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Day)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_Day
            // 
            this.dgv_Day.AllowUserToAddRows = false;
            this.dgv_Day.AllowUserToDeleteRows = false;
            this.dgv_Day.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Day.Location = new System.Drawing.Point(2, 3);
            this.dgv_Day.Name = "dgv_Day";
            this.dgv_Day.RowTemplate.Height = 23;
            this.dgv_Day.Size = new System.Drawing.Size(582, 269);
            this.dgv_Day.TabIndex = 0;
            this.dgv_Day.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Day_CellDoubleClick);
            // 
            // btn_Change
            // 
            this.btn_Change.Location = new System.Drawing.Point(497, 278);
            this.btn_Change.Name = "btn_Change";
            this.btn_Change.Size = new System.Drawing.Size(87, 33);
            this.btn_Change.TabIndex = 1;
            this.btn_Change.Text = "确认修改";
            this.btn_Change.UseVisualStyleBackColor = true;
            this.btn_Change.Click += new System.EventHandler(this.btn_Change_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(2, 278);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(87, 33);
            this.btn_Close.TabIndex = 2;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // Plan_Day
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 314);
            this.ControlBox = false;
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Change);
            this.Controls.Add(this.dgv_Day);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Plan_Day";
            this.Text = "Plan_Day";
            this.Load += new System.EventHandler(this.Plan_Day_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Day)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_Day;
        private System.Windows.Forms.Button btn_Change;
        private System.Windows.Forms.Button btn_Close;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonFunc;


namespace Order_Create
{
    public partial class Plan_Day : Form
    {
        private string loadweek = "";
        private string loadmachine = "";

        public Plan_Day(string week,string machine)
        {
            InitializeComponent();
            loadweek = week;
            loadmachine = machine;
            this.Text = machine + "#日排程";
        }

        private void Plan_Day_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DBName db = new DBName();

            string sqlstr = "select MACHINE as 机台,ITEM as 物料代号, QTY as 数量, PLAN_TIME as 排程日期, ";
            sqlstr += "(CASE PLAN_STOP WHEN 1 THEN '否' WHEN 0 THEN '是' END) as 停止生产";
            sqlstr +=", PLAN_STOP_REASON as 停产原因 FROM[CSTCKMACH].[dbo].[JiaLiu_Plan_Day] ";
            sqlstr += "where RIGHT(PLAN_ID,6)='" + loadweek + "' and MACHINE='" + loadmachine + "'" + "order by PLAN_TIME ";

            dt = ConnDB.QueryDB(sqlstr, db.DB_CSTS5);

            dgv_Day.DataSource = dt;

            dgv_Day.Columns["机台"].ReadOnly = true;
            dgv_Day.Columns["物料代号"].ReadOnly = true;
            dgv_Day.Columns["数量"].ReadOnly = true;
            dgv_Day.Columns["排程日期"].ReadOnly = true;
            dgv_Day.Columns["停止生产"].ReadOnly = true;

            dgv_Day.Columns["机台"].Width = 60;
            dgv_Day.Columns["物料代号"].Width = 80;
            dgv_Day.Columns["数量"].Width = 60;
            dgv_Day.Columns["排程日期"].Width = 80;
            dgv_Day.Columns["停止生产"].Width = 80;
            dgv_Day.Columns["停产原因"].Width = 150;

            for (int i = 0; i < dgv_Day.Rows.Count; i++)
            {
                if (dgv_Day.Rows[i].Cells["停止生产"].Value.ToString() == "是")
                {
                    dgv_Day.Rows[i].Cells["停止生产"].Style.BackColor = Color.Red;
                }
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_Day_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (col == 4 && row >= 0)
            {
                if (dgv_Day.Rows[row].Cells["停止生产"].Value.ToString() == "是")
                {
                    dgv_Day.Rows[row].Cells["停止生产"].Value = "否";
                    dgv_Day.Rows[row].Cells["停止生产"].Style.BackColor = Color.White;
                }
                else
                {
                    dgv_Day.Rows[row].Cells["停止生产"].Value = "是";
                    dgv_Day.Rows[row].Cells["停止生产"].Style.BackColor = Color.Red;
                }
            }
        }

        private void btn_Change_Click(object sender, EventArgs e)
        {
            int plan_stop = 1;
            string plan_stop_reason;
            string plan_time;
            string machine = loadmachine;
            DateTime plan_stop_createdtime;
            DBName db = new DBName();
            

            for (int i = 0; i < dgv_Day.Rows.Count; i++)
            {
                plan_time = dgv_Day.Rows[i].Cells["排程日期"].Value.ToString();
                plan_stop_reason = dgv_Day.Rows[i].Cells["停产原因"].Value.ToString();
                plan_stop_createdtime=DateTime.Now;

                if (dgv_Day.Rows[i].Cells["停止生产"].Value.ToString() == "是")
                {
                    plan_stop = 0;
                }
                else
                {
                    plan_stop = 1;
                }

                SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@MACHINE",machine),
                    new SqlParameter("@PLAN_STOP",plan_stop),
                    new SqlParameter("@PLAN_STOP_REASON",plan_stop_reason),
                    new SqlParameter("@PLAN_STOP_CREATEDTIME",plan_stop_createdtime),
                    new SqlParameter("@PLAN_TIME",plan_time)
                };

                string sqlstr = "Update [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] Set PLAN_STOP=" + "@PLAN_STOP" + ",";
                sqlstr += "PLAN_STOP_REASON=" + "@PLAN_STOP_REASON" + ",PLAN_STOP_CREATEDTIME=" + "@PLAN_STOP_CREATEDTIME" + " ";
                sqlstr += "where MACHINE=" + "@MACHINE" + " and PLAN_TIME=" + "@PLAN_TIME";
                ConnDB.ExcuteSQL(sqlstr, db.DB_CSTS5,paras);
            }
        }
    }
}

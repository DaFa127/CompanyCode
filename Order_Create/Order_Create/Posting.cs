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
using NLog;
using NLog.Layouts;
using Oracle.ManagedDataAccess.Client;

namespace Order_Create
{
    public partial class Posting : Form
    {
        public static DataTable dt_posting = new DataTable();
        public Logger log = LogManager.GetCurrentClassLogger();
        public Posting()
        {
            InitializeComponent();
        }

        private void btn_query_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;

            btn_query.Enabled = false;

            btn_update_diff.Enabled = false;

            btn_back.Enabled = false;

            bgWorker_Query.RunWorkerAsync();
        }

        private void btn_update_diff_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count > 0)
            {
                DialogResult checkResult =  MessageBox.Show("是否执行差异上传", "提示", MessageBoxButtons.YesNo);

                if (checkResult == DialogResult.Yes)
                {
                    Update_Posting();

                    dt_posting.Clear();

                    dgv.DataSource = dt_posting;

                    btn_update_diff.Enabled = false;
                }
            }
        }
        private void btn_back_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Order_Create().ShowDialog();
            this.Close();
        }

        private static DataTable Load_Order_Posting(string loadDay)
        {
            DBName db = new DBName();
            DataTable dt = new DataTable();
            StringBuilder strsb = new StringBuilder();

            strsb.Append("SELECT SITE, ITEM, SAP_SO_NUM, QTY, '' as NewQty, UNIT, SCRAP_QTY, WORK_CENTER, MACHINE_TIME, HUMAN_TIME, ");
            strsb.Append("STG_LOCATION, POSTING_DATE, CREATED_TIME, STATUS, MSG, SAP_PROCESS_TIME, COMP_NO, COMP_COUNT, ");
            strsb.Append("DOC_NO, DOC_YEAR, (CASE UPDATE_TO_ORACLE WHEN 1 THEN '否' WHEN 0 THEN '是' END) as UPDATE_TO_ORACLE, ");
            strsb.Append("UPDATE_TO_ORACLE_TIME FROM [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Posting] ");
            strsb.Append("where POSTING_DATE ='" + loadDay + "'" + "order by SAP_SO_NUM ");
            dt = ConnDB.QueryDB(strsb, db.DB_CSTS5);

            return dt;
        }

        private void Update_Posting()
        {
            DBName db = new DBName();
            int update_qty = 0;
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                int dgvQty = 0;
                dgvQty = Convert.ToInt32(dgv.Rows[i].Cells["QTY"].Value.ToString());
                int dgvQty_new = 0;
                dgvQty_new = Convert.ToInt32(dgv.Rows[i].Cells["NewQty"].Value.ToString());

                if (dgvQty == 0 && dgvQty_new != 0 && dgv.Rows[i].Cells["STATUS"].Value.ToString() == "I")
                {
                    update_qty += 1;
                    string site = dgv.Rows[i].Cells["SITE"].Value.ToString();
                    string item = dgv.Rows[i].Cells["ITEM"].Value.ToString();
                    string sap_so_num = dgv.Rows[i].Cells["SAP_SO_NUM"].Value.ToString();
                    string qty = dgv.Rows[i].Cells["NewQty"].Value.ToString();
                    string unit = dgv.Rows[i].Cells["UNIT"].Value.ToString();
                    string scrap_qty = dgv.Rows[i].Cells["SCRAP_QTY"].Value.ToString();
                    string work_center = dgv.Rows[i].Cells["WORK_CENTER"].Value.ToString();
                    string machine_time = dgv.Rows[i].Cells["MACHINE_TIME"].Value.ToString();
                    string human_time = dgv.Rows[i].Cells["HUMAN_TIME"].Value.ToString();
                    string stg_location = dgv.Rows[i].Cells["STG_LOCATION"].Value.ToString();
                    string posting_date = dgv.Rows[i].Cells["POSTING_DATE"].Value.ToString();
                    string mes_process_time = dgv.Rows[i].Cells["CREATED_TIME"].Value.ToString();
                    string status = dgv.Rows[i].Cells["STATUS"].Value.ToString();

                    OracleParameter[] paras_Oracle = new OracleParameter[]
                    {
                        new OracleParameter(":SITE",OracleDbType.Char,4),
                        new OracleParameter(":ITEM",OracleDbType.Char,18),
                        new OracleParameter(":SAP_SO_NUM",OracleDbType.Char,11),
                        new OracleParameter(":QTY",OracleDbType.Decimal,10),
                        new OracleParameter(":UNIT",OracleDbType.Char,3),
                        new OracleParameter(":SCRAP_QTY",OracleDbType.Decimal,10),
                        new OracleParameter(":WORK_CENTER",OracleDbType.Char,8),
                        new OracleParameter(":MACHINE_TIME",OracleDbType.Decimal,9),
                        new OracleParameter(":HUMAN_TIME",OracleDbType.Decimal,9),
                        new OracleParameter(":STG_LOCATION",OracleDbType.Char,4),
                        new OracleParameter(":POSTING_DATE",OracleDbType.Char,8),
                        new OracleParameter(":MES_PROCESS_TIME",OracleDbType.Char,14),
                        new OracleParameter(":STATUS",OracleDbType.Char,1)
                    };

                    paras_Oracle[0].Value = site;
                    paras_Oracle[1].Value = item;
                    paras_Oracle[2].Value = sap_so_num;
                    paras_Oracle[3].Value = qty;
                    paras_Oracle[4].Value = unit;
                    paras_Oracle[5].Value = scrap_qty;
                    paras_Oracle[6].Value = work_center;
                    paras_Oracle[7].Value = machine_time;
                    paras_Oracle[8].Value = human_time;
                    paras_Oracle[9].Value = stg_location;
                    paras_Oracle[10].Value = posting_date;
                    paras_Oracle[11].Value = mes_process_time;
                    paras_Oracle[12].Value = status; 

                    StringBuilder sqlsb_oracle = new StringBuilder(400);
                    sqlsb_oracle.Append("Insert into MES_ERP.ERP_SHOP_ORDER_POSTING_HEAD (SITE, ITEM, SAP_SO_NUM, QTY, UNIT, ");
                    sqlsb_oracle.Append("SCRAP_QTY, WORK_CENTER, MACHINE_TIME, HUMAN_TIME, STG_LOCATION, POSTING_DATE, MES_PROCESS_TIME, STATUS)");
                    sqlsb_oracle.Append(" values ");
                    sqlsb_oracle.Append("(" + ":SITE" + "," + ":ITEM" + "," + ":SAP_SO_NUM" + "," + ":QTY" + "," + ":UNIT" + ",");
                    sqlsb_oracle.Append(":SCRAP_QTY" + "," + ":WORK_CENTER" + "," + ":MACHINE_TIME" + "," + ":HUMAN_TIME" + "," + ":STG_LOCATION" + ",");
                    sqlsb_oracle.Append(":POSTING_DATE" + "," + ":MES_PROCESS_TIME" + "," + ":STATUS" + ")");

                    ConnDB.ExcuteSQL(sqlsb_oracle, db.DB_Oracle, paras_Oracle);

                    string created_time = dgv.Rows[i].Cells["CREATED_TIME"].Value.ToString();
                    string update_to_oracle_time = ConnDB.GetDBTime().ToString("yyyy-MM-dd HH:mm:ss");

                    SqlParameter[] paras = new SqlParameter[]
                    {
                        new  SqlParameter("@QTY",qty),
                        new  SqlParameter("@CREATED_TIME",created_time),
                        new  SqlParameter("@UPDATE_TO_ORACLE_TIME",update_to_oracle_time),
                        new  SqlParameter("@SAP_SO_NUM",sap_so_num),
                        new  SqlParameter("@POSTING_DATE",posting_date)
                    };
                    StringBuilder sqlsb = new StringBuilder();

                    sqlsb.Append("Update [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Posting] Set QTY=" + "@QTY" + ",");
                    sqlsb.Append("CREATED_TIME=" + "@CREATED_TIME" + ",UPDATE_TO_ORACLE=0,");
                    sqlsb.Append("UPDATE_TO_ORACLE_TIME=" + "@UPDATE_TO_ORACLE_TIME" + " ");
                    sqlsb.Append("Where SAP_SO_NUM=" + "@SAP_SO_NUM" + " and ");
                    sqlsb.Append("POSTING_DATE=" + "@POSTING_DATE" + "");
                    ConnDB.ExcuteSQL(sqlsb, db.DB_CSTS5,paras);
                }
                else if (dgvQty < dgvQty_new && dgv.Rows[i].Cells["STATUS"].Value.ToString() == "S")
                {
                    update_qty += 1;
                    string qty_New = dgv.Rows[i].Cells["NewQty"].Value.ToString();
                    string sap_so_num_New = dgv.Rows[i].Cells["SAP_SO_NUM"].Value.ToString();
                    string posting_date_New = dgv.Rows[i].Cells["POSTING_DATE"].Value.ToString();

                    SqlParameter[] paras2 = new SqlParameter[]
                    {
                        new  SqlParameter("@QTY",qty_New),
                        new  SqlParameter("@SAP_SO_NUM",sap_so_num_New),
                        new  SqlParameter("@POSTING_DATE",posting_date_New)
                    };
                    StringBuilder sqlsb2 = new StringBuilder();

                    sqlsb2.Append("Update [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Posting] Set QTY=" + "@QTY" + " ");
                    sqlsb2.Append("Where SAP_SO_NUM=" + "@SAP_SO_NUM" + " and ");
                    sqlsb2.Append("POSTING_DATE=" + "@POSTING_DATE" + "");
                    ConnDB.ExcuteSQL(sqlsb2, db.DB_CSTS5, paras2);

                    string logstr = "更新:" + sap_so_num_New + "  日期:" + posting_date_New + "   更新前数量:";
                    logstr += dgv.Rows[i].Cells["QTY"].Value.ToString() + "   更新后数量:" + qty_New;

                    log.Trace(logstr);
                }
            }

            MessageBox.Show("上传完成 笔数:" + update_qty + "  请返回主页面更新该日SAP状态", "提示");

            log.Trace("上传笔数:" + update_qty + "  日期:" + DTP.Value.ToString("yyyyMMdd"));
        }

        private static int Check_JiaLiuPosting(string sap_Num, string sap_Item, string sap_WorkCenter)
        {
            DBName db = new DBName();
            DataTable dt = new DataTable();

            string qty_new = "";
            string posting_date = dt_posting.Rows[0]["POSTING_DATE"].ToString(); ;
            //string created_time = ConnDB.GetDBTime().ToString("yyyyMMddHHmmss");

            string sqlstr = "select count(SAP_SO_NUM) as QTY FROM [CSTCLOT" + posting_date.Substring(0, 4) + "].[dbo].[V_Xbar] ";
            sqlstr += "Where SAP_SO_NUM ='" + sap_Num + "' and jiaMach is Not Null and JiaTime is Not Null and JiaTime<>0 and ";
            sqlstr += "(IsADD <> 'C' or IsADD is null) ";

            dt = ConnDB.QueryDB(sqlstr, db.DB_CSTS5);

            qty_new = dt.Rows[0]["QTY"].ToString();

            return Convert.ToInt32(qty_new);
        }

        private void bgWorker_Query_DoWork(object sender, DoWorkEventArgs e)
        {
            string load_day = DTP.Value.ToString("yyyyMMdd");
            DBName db = new DBName();
            double sttime = DTP.Value.AddDays(-20).ToOADate();
            double edtime = DTP.Value.AddDays(+20).ToOADate();
            int sttime_year = DTP.Value.AddDays(-20).Year;
            int edtime_year = DTP.Value.AddDays(+20).Year;

            DataTable dt_lot = new DataTable();

            dt_posting = Load_Order_Posting(load_day);//先查询是否有建立加硫报工资料

            if (dt_posting.Rows.Count > 0)
            {
                StringBuilder sqlsb = new StringBuilder();
                if (sttime_year == edtime_year)
                {
                    sqlsb.Append("select SAP_SO_NUM FROM [CSTCLOT" + sttime_year +"].[dbo].[V_Xbar] ");
                    sqlsb.Append("where Sttime between " + sttime + " and " + edtime + " and jiaMach is Not Null and JiaTime is Not Null and JiaTime<>0 ");
                    sqlsb.Append("and (IsADD <> 'C' or IsADD is null) ");
                }
                else
                {
                    sqlsb.Append("select SAP_SO_NUM FROM [CSTCLOT" + sttime_year + "].[dbo].[V_Xbar] ");
                    sqlsb.Append("where Sttime between " + sttime + " and " + edtime + " and jiaMach is Not Null and JiaTime is Not Null and JiaTime<>0 ");
                    sqlsb.Append("and (IsADD <> 'C' or IsADD is null) ");
                    sqlsb.Append("union ");
                    sqlsb.Append("select SAP_SO_NUM FROM [CSTCLOT" + edtime_year + "].[dbo].[V_Xbar] ");
                    sqlsb.Append("where Sttime between " + sttime + " and " + edtime + " and jiaMach is Not Null and JiaTime is Not Null and JiaTime<>0 ");
                    sqlsb.Append("and (IsADD <> 'C' or IsADD is null) ");
                }

                dt_lot = ConnDB.QueryDB(sqlsb, db.DB_CSTS5);

                for (int i = 0; i < dt_posting.Rows.Count; i++)
                {
                    int pgb = Convert.ToInt32((double)i / (double)dt_posting.Rows.Count * 100);
                    bgWorker_Query.ReportProgress(pgb);

                    string num = dt_posting.Rows[i]["SAP_SO_NUM"].ToString();
                    //string item = dt_posting.Rows[i]["ITEM"].ToString();
                    //string wc = dt_posting.Rows[i]["WORK_CENTER"].ToString();

                    DataRow[] dr = dt_lot.Select("SAP_SO_NUM='" + num + "'");

                    if (dr.Length > 0)
                    {
                        dt_posting.Rows[i]["NewQty"] = dr.Length;
                    }
                    else
                    {
                        dt_posting.Rows[i]["NewQty"] = 0;
                    }

                    if (dt_posting.Rows[i]["STATUS"].ToString() == "I" && dt_posting.Rows[i]["QTY"].ToString() == "0" && dt_posting.Rows[i]["NewQty"].ToString() != "0")
                    {
                        dt_posting.Rows[i]["CREATED_TIME"] = ConnDB.GetDBTime().ToString("yyyyMMddHHmmss");
                    }
                }
            }

            log.Trace("查询报工数据比对 笔数:" + dt_posting.Rows.Count + load_day);
        }

        private void bgWorker_Query_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int p = e.ProgressPercentage;

            progressBar.Value = p;
        }

        private void bgWorker_Query_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Value = 0;

            progressBar.Visible = false;

            btn_query.Enabled = true;

            btn_update_diff.Enabled = true;

            btn_back.Enabled = true;

            dgv.DataSource = dt_posting;

            SetDataGridViewCol();
        }

        private void SetDataGridViewCol()
        {
            dgv.Columns["SITE"].HeaderText = "工厂";
            dgv.Columns["ITEM"].HeaderText = "料号";
            dgv.Columns["SAP_SO_NUM"].HeaderText = "SAP工单号";
            dgv.Columns["QTY"].HeaderText = "数量";
            dgv.Columns["NewQty"].HeaderText = "本日数量";
            dgv.Columns["UNIT"].HeaderText = "料号单位";
            dgv.Columns["SCRAP_QTY"].HeaderText = "报废数量";
            dgv.Columns["WORK_CENTER"].HeaderText = "工作中心";
            dgv.Columns["MACHINE_TIME"].HeaderText = "机器工时";
            dgv.Columns["HUMAN_TIME"].HeaderText = "人工工时";
            dgv.Columns["STG_LOCATION"].HeaderText = "储存地点";
            dgv.Columns["POSTING_DATE"].HeaderText = "过账时间";
            dgv.Columns["CREATED_TIME"].HeaderText = "ME创建时间";
            dgv.Columns["STATUS"].HeaderText = "SAP处理状态";
            dgv.Columns["MSG"].HeaderText = "SAP异常回报";
            dgv.Columns["SAP_PROCESS_TIME"].HeaderText = "SAP处理时间";
            dgv.Columns["COMP_NO"].HeaderText = "作业完成确认码";
            dgv.Columns["COMP_COUNT"].HeaderText = "确认码计数";
            dgv.Columns["DOC_NO"].HeaderText = "物料文件号";
            dgv.Columns["DOC_YEAR"].HeaderText = "物料文件年";
            dgv.Columns["UPDATE_TO_ORACLE"].HeaderText = "是否上传SAP";
            dgv.Columns["UPDATE_TO_ORACLE_TIME"].HeaderText = "上传SAP时间";

            dgv.Columns["SITE"].Width = 40;
            dgv.Columns["ITEM"].Width = 80;
            dgv.Columns["SAP_SO_NUM"].Width = 90;
            dgv.Columns["QTY"].Width = 40;
            dgv.Columns["NewQty"].Width = 40;
            dgv.Columns["UNIT"].Width = 40;
            dgv.Columns["SCRAP_QTY"].Width = 40;
            dgv.Columns["WORK_CENTER"].Width = 40;
            dgv.Columns["MACHINE_TIME"].Width = 30;
            dgv.Columns["HUMAN_TIME"].Width = 30;
            dgv.Columns["STG_LOCATION"].Width = 40;
            dgv.Columns["POSTING_DATE"].Width = 60;
            dgv.Columns["CREATED_TIME"].Width = 100;
            dgv.Columns["STATUS"].Width = 30;
            dgv.Columns["MSG"].Width = 100;
            dgv.Columns["SAP_PROCESS_TIME"].Width = 100;
            dgv.Columns["COMP_NO"].Width = 80;
            dgv.Columns["COMP_COUNT"].Width = 60;
            dgv.Columns["DOC_NO"].Width = 60;
            dgv.Columns["DOC_YEAR"].Width = 40;
            dgv.Columns["UPDATE_TO_ORACLE"].Width = 40;
            dgv.Columns["UPDATE_TO_ORACLE_TIME"].Width = 100;

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                if (dgv.Rows[i].Cells["QTY"].Value.ToString() != dgv.Rows[i].Cells["NewQty"].Value.ToString())
                {
                    dgv.Rows[i].Cells["NewQty"].Style.BackColor=Color.Tomato;
                }
            }
        }

        private void Posting_Load(object sender, EventArgs e)
        {
            DTP.Value = DateTime.Now.AddDays(-1);

            progressBar.Visible = false;

            btn_update_diff.Enabled = false;

            log.Trace("开启报工数据比对");
        }

    }
}

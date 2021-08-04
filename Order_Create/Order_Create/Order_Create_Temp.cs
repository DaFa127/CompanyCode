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
using NLog.LayoutRenderers.Wrappers;
using C_Dll_Login;
using NLog;
using Oracle.ManagedDataAccess.Client;

namespace Order_Create
{
    public partial class Order_Create_Temp : Form
    {
        private static DataTable dt_Spec = new DataTable();
        private static DataTable dt_CT = new DataTable();
        private DataTable dt_dgv = new DataTable();
        public Logger log = LogManager.GetCurrentClassLogger();

        public Order_Create_Temp()
        {
            InitializeComponent();
        }

        private void Order_Create_Temp_Load(object sender, EventArgs e)
        {
            GetSpec();
            GetJialiuCT();
            SetDataTable();
            dgv.DataSource = dt_dgv;
            //SetDataGridView();
            comboBox1.Items.Add("CR1");
            comboBox1.Items.Add("CR2");
            comboBox2.Items.Add("A");
            comboBox2.Items.Add("B");
            comboBox2.Items.Add("C");
            DTP.Value=DateTime.Now;
            log.Trace("开启加硫插单维护");
        }
        private void btn_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 11 && textBox1.Text.Substring(0, 1) == "E" && comboBox1.Text != "" && textBox2.Text != "")
            {
                CheckSpec(textBox1.Text, comboBox2.Text);
            }
            else
            {
                MessageBox.Show("输入错误", "提示");
            }
        }
        private void btn_update_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count > 0)
            {
                UpdateOracle();
            }
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Order_Create().ShowDialog();
            this.Close();
        }

        private void UpdateOracle()
        {
            DBName db = new DBName();
            //先写入.5数据库
            StringBuilder sqlsb = new StringBuilder();
            sqlsb.Append("Insert into [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Create] (SITE, SHOP_ORDER, ITEM, SO_TYPE, SPEC_NO, ");
            sqlsb.Append("SPEC_VERSION, PRD_PHASE, START_DATE, COMP_DATE, QTY, ");
            sqlsb.Append("UNIT, LOCATION, WORK_CENTER, CREATED_TIME, STATUS) values ");

            //再写入Oracle数据库
            StringBuilder sqlsb_Oracle = new StringBuilder();
            sqlsb_Oracle.Append("Insert into MES_ERP.ERP_SHOP_ORDER_CREATE_HEAD (SITE, SHOP_ORDER, ITEM, SO_TYPE, SPEC_NO, ");
            sqlsb_Oracle.Append("SPEC_VERSION, PRD_PHASE, START_DATE, COMP_DATE, QTY, ");
            sqlsb_Oracle.Append("UNIT, STG_LOCATION, WORK_CENTER, MES_PROCESS_TIME, STATUS) values ");


            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                string site = "2061";
                string shop_order = dgv.Rows[i].Cells["SHOP_ORDER"].Value.ToString();
                string item = dgv.Rows[i].Cells["ITEM"].Value.ToString(); 
                string so_type = dgv.Rows[i].Cells["SO_TYPE"].Value.ToString();
                string spec_no = dgv.Rows[i].Cells["SPEC_NO"].Value.ToString(); 
                string spec_version = dgv.Rows[i].Cells["SPEC_VERSION"].Value.ToString();
                string prd_phase = dgv.Rows[i].Cells["PRD_PHASE"].Value.ToString();
                string start_date = dgv.Rows[i].Cells["START_DATE"].Value.ToString();
                string comp_date = dgv.Rows[i].Cells["COMP_DATE"].Value.ToString();
                string qty = dgv.Rows[i].Cells["QTY"].Value.ToString();
                string unit = "PCS";
                string location = dgv.Rows[i].Cells["LOCATION"].Value.ToString();
                string work_center = dgv.Rows[i].Cells["WORK_CENTER"].Value.ToString();
                string created_time = ConnDB.GetDBTime().ToString("yyyyMMddHHmmss");
                string status = dgv.Rows[i].Cells["STATUS"].Value.ToString();

                SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@SITE",site),
                    new SqlParameter("@SHOP_ORDER",shop_order),
                    new SqlParameter("@ITEM",item),
                    new SqlParameter("@SO_TYPE",so_type),
                    new SqlParameter("@SPEC_NO",spec_no),
                    new SqlParameter("@SPEC_VERSION",spec_version),
                    new SqlParameter("@PRD_PHASE",prd_phase),
                    new SqlParameter("@START_DATE",start_date),
                    new SqlParameter("@COMP_DATE",comp_date),
                    new SqlParameter("@QTY",qty),
                    new SqlParameter("@UNIT",unit),
                    new SqlParameter("@LOCATION",location),
                    new SqlParameter("@WORK_CENTER",work_center),
                    new SqlParameter("@CREATED_TIME",created_time),
                    new SqlParameter("@STATUS",status)
                };

                StringBuilder sqlsb2 = new StringBuilder();
                sqlsb2.Append(sqlsb + "(" + "@SITE" + ",");
                sqlsb2.Append("@SHOP_ORDER" + "," + "@ITEM" + "," + "@SO_TYPE" + "," + "@SPEC_NO" + "," + "@SPEC_VERSION" + ",");
                sqlsb2.Append("@PRD_PHASE" + "," + "@START_DATE" + "," + "@COMP_DATE" + "," + "@QTY" + "," + "@UNIT" + ",");
                sqlsb2.Append("@LOCATION" + "," + "@WORK_CENTER" + "," + "@CREATED_TIME" + "," + "@STATUS" + ")");

                ConnDB.ExcuteSQL(sqlsb2, db.DB_CSTS5, paras);


                OracleParameter[] paras_Oracle = new OracleParameter[]
                {
                new OracleParameter(":SITE",OracleDbType.Char,4),
                new OracleParameter(":SHOP_ORDER",OracleDbType.Char,20),
                new OracleParameter(":ITEM",OracleDbType.Char,18),
                new OracleParameter(":SO_TYPE",OracleDbType.Char,4),
                new OracleParameter(":SPEC_NO",OracleDbType.Char,12),
                new OracleParameter(":SPEC_VERSION",OracleDbType.Char,4),
                new OracleParameter(":PRD_PHASE",OracleDbType.Char,1),
                new OracleParameter(":START_DATE",OracleDbType.Char,8),
                new OracleParameter(":COMP_DATE",OracleDbType.Char,8),
                new OracleParameter(":QTY",OracleDbType.Decimal,10),
                new OracleParameter(":UNIT",OracleDbType.Char,3),
                new OracleParameter(":STG_LOCATION",OracleDbType.Char,4),
                new OracleParameter(":WORK_CENTER",OracleDbType.Char,8),
                new OracleParameter(":MES_PROCESS_TIME",OracleDbType.Char,14),
                new OracleParameter(":STATUS",OracleDbType.Char,1)
                };

                paras_Oracle[0].Value = site;
                paras_Oracle[1].Value = shop_order;
                paras_Oracle[2].Value = item;
                paras_Oracle[3].Value = so_type;
                paras_Oracle[4].Value = spec_no;
                paras_Oracle[5].Value = spec_version;
                paras_Oracle[6].Value = prd_phase;
                paras_Oracle[7].Value = start_date;
                paras_Oracle[8].Value = comp_date;
                paras_Oracle[9].Value = qty;
                paras_Oracle[10].Value = unit;
                paras_Oracle[11].Value = location;
                paras_Oracle[12].Value = work_center;
                paras_Oracle[13].Value = ConnDB.GetDBTime().ToString("yyyyMMddHHmmss");
                paras_Oracle[14].Value = status;

                StringBuilder sqlsb_Oracle2 = new StringBuilder(400);

                sqlsb_Oracle2.Append(sqlsb_Oracle + "(" + ":SITE" + "," + ":SHOP_ORDER" + "," + ":ITEM" + "," + ":SO_TYPE" + "," + ":SPEC_NO" + ",");
                sqlsb_Oracle2.Append(":SPEC_VERSION" + "," + ":PRD_PHASE" + "," + ":START_DATE" + "," + ":COMP_DATE" + "," + ":QTY" + ",");
                sqlsb_Oracle2.Append(":UNIT" + "," + ":STG_LOCATION" + "," + ":WORK_CENTER" + "," + ":MES_PROCESS_TIME" + "," + ":STATUS" + ")");
                ConnDB.ExcuteSQL(sqlsb_Oracle2, db.DB_Oracle, paras_Oracle);

                log.Trace("上传SAP ITEM:" + item + "  SHOP_ORDER:" + shop_order);

                StringBuilder sqlsb_Update= new StringBuilder(200);
                sqlsb_Update.Append("update [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Create] Set UPDATE_TO_ORACLE=0,");
                sqlsb_Update.Append("UPDATE_TO_ORACLE_TIME='" + ConnDB.GetDBTime().ToString("yyyy-MM-dd HH:mm:ss") + "' ");
                sqlsb_Update.Append("Where SHOP_ORDER='" + shop_order + "' and START_DATE='" + start_date + "'");
                ConnDB.ExcuteSQL(sqlsb_Update, db.DB_CSTS5);

                log.Trace("更新MSSQL SHOP_ORDER:" + shop_order + "  START_DATE:" + start_date);

            }
            MessageBox.Show("上传完成,返回主页面查询该日期工单并更新SAP状态直到I变化(S为成功 A处理中 F失败)","提示说明");
        }

        private void CheckSpec(string etno,string stge)
        {
            string spec_ds;
            string spec;
            string ver;
            string ct;
            int qty;

            //先找规格版次
            DataRow[] dr = dt_Spec.Select("CST_PRT_NO='" + etno + "' and CST_SPEC_STAGE='" + stge + "'");
            if (dr.Length > 0)
            {
                //这边注意新SPEC时有没有问题
                spec = dr[0]["CST_SPEC_AS400NO"].ToString();
                if (spec.Length > 8)
                {
                    spec_ds = dr[0]["CST_SPEC_AS400NO"].ToString();
                }
                else
                {
                    spec_ds = dr[0]["CST_SPEC_AS400NO"].ToString().Replace("S", "");
                }

                ver = dr[0]["CST_SPEC_AS400VER"].ToString();

                stge = dr[0]["CST_SPEC_STAGE"].ToString();

                log.Trace("spec:" + spec + "  spec_ds:" + spec_ds + "  " + stge + ver);
                //单模加硫时间
                DataRow[] drs = dt_CT.Select("NAME like '%" + spec_ds + "%'");
                if (drs.Length > 0)
                {
                    ct = drs[0]["CST_TIREVULCANIZATION_SUMMER"].ToString();
                    qty = 86400 / (Convert.ToInt32(ct) + 105);

                    if (comboBox1.Text == "CR1") qty = qty * Convert.ToInt32(textBox2.Text);

                    //先找插单资料中是否有相同的工作中心工单与相同规格的资料
                    DataRow[] dr_dgv_spec = dt_dgv.Select("WORK_CENTER='" + comboBox1.Text + "' and ITEM='" + etno + "'", "SHOP_ORDER DESC");
                    if (dr_dgv_spec.Length > 0)
                    {
                        MessageBox.Show("已维护过该料号");
                        return;
                    }
                        
                    //先找插单资料中是否有相同的工作中心工单
                    DataRow[] dr_dgv = dt_dgv.Select("WORK_CENTER='" + comboBox1.Text + "'", "SHOP_ORDER DESC");

                    if (dr_dgv.Length > 0)
                    {
                        string dgvOrder_last = dr_dgv[0]["SHOP_ORDER"].ToString();
                        int dgvOrder_num = Convert.ToInt32(dgvOrder_last.Substring(11, 3)) + 1;
                        string dgvOrder_create = dgvOrder_last.Substring(0, 11) + dgvOrder_num.ToString("000");

                        DataRow drdgv = dt_dgv.NewRow();
                        drdgv["SITE"] = "2061";
                        drdgv["SHOP_ORDER"] = dgvOrder_create;
                        drdgv["ITEM"] = etno;

                        if (stge == "A")
                        {
                            drdgv["SO_TYPE"] = "ZP02";
                            drdgv["LOCATION"] = "50R1";
                        }
                        else
                        {
                            drdgv["SO_TYPE"] = "ZP01";
                            drdgv["LOCATION"] = "50F2";
                        }
                        drdgv["SPEC_NO"] = spec;
                        drdgv["SPEC_VERSION"] = ver;
                        drdgv["PRD_PHASE"] = stge;
                        drdgv["START_DATE"] = DTP.Value.ToString("yyyMMdd");
                        drdgv["COMP_DATE"] = DTP.Value.ToString("yyyMMdd");
                        drdgv["QTY"] = qty;
                        drdgv["UNIT"] = "PCS";
                        drdgv["WORK_CENTER"] = comboBox1.Text;
                        drdgv["CREATED_TIME"] = ConnDB.GetDBTime().ToString("yyyyMMddHHmmss");
                        drdgv["STATUS"] = "I";
                        dt_dgv.Rows.Add(drdgv);
                        log.Trace("ITEM:" + etno + "  SHOP_ORDER:" + dgvOrder_create + "  START_DATE:" + DTP.Value.ToString("yyyMMdd"));
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        DBName db = new DBName();
                        StringBuilder sqlsb = new StringBuilder(100);
                        sqlsb.Append("select * FROM [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Create] where ");
                        sqlsb.Append("START_DATE='" + DTP.Value.ToString("yyyyMMdd") + "' and ");
                        sqlsb.Append("WORK_CENTER='" + comboBox1.Text + "' order by SHOP_ORDER DESC ");
                        dt = ConnDB.QueryDB(sqlsb, db.DB_CSTS5);

                        //选择的工作中心是否有开加硫工单
                        DataRow[] drs_order = dt.Select("ITEM='" + etno + "' and PRD_PHASE='" + stge + "' and SPEC_VERSION='" + ver + "'");

                        if (drs_order.Length > 0)
                        {
                            MessageBox.Show("该日已有此规格工单", "提示");
                            return;
                        }

                        //插单的MES工单号
                        string order_last = dt.Rows[0]["SHOP_ORDER"].ToString();
                        int order_num = Convert.ToInt32(order_last.Substring(11, 3)) + 1;
                        string order_create = order_last.Substring(0, 11) + order_num.ToString("000");

                        DataRow drdt = dt_dgv.NewRow();
                        drdt["SITE"] = "2061";
                        drdt["SHOP_ORDER"] = order_create;
                        drdt["ITEM"] = etno;

                        if (stge == "A")
                        {
                            drdt["SO_TYPE"] = "ZP02";
                            drdt["LOCATION"] = "50R1";
                        }
                        else
                        {
                            drdt["SO_TYPE"] = "ZP01";
                            drdt["LOCATION"] = "50F2";
                        }
                        drdt["SPEC_NO"] = spec;
                        drdt["SPEC_VERSION"] = ver;
                        drdt["PRD_PHASE"] = stge;
                        drdt["START_DATE"] = DTP.Value.ToString("yyyMMdd");
                        drdt["COMP_DATE"] = DTP.Value.ToString("yyyMMdd");
                        drdt["QTY"] = qty * Convert.ToInt32(textBox2.Text);
                        drdt["UNIT"] = "PCS";
                        drdt["WORK_CENTER"] = comboBox1.Text;
                        drdt["CREATED_TIME"] = ConnDB.GetDBTime().ToString("yyyyMMddHHmmss");
                        drdt["STATUS"] = "I";
                        dt_dgv.Rows.Add(drdt);

                        log.Trace("ITEM:" + etno + "  SHOP_ORDER:" + order_create + "  START_DATE:" + DTP.Value.ToString("yyyMMdd"));
                    }
                    dgv.DataSource = dt_dgv;
                    SetDataGridView();
                }
            }
            else
            {
                MessageBox.Show("找不到该料号,请确认！", "提示");
                log.Trace("找不到该料号,请确认！" + etno );
            }
        }

        private static DataTable GetSpec()
        {
            DBName db = new DBName();

            StringBuilder sqlsb = new StringBuilder(200);

            sqlsb.Append("select * FROM OPENQUERY(MIDCSTC_PRD,'select CST_SPEC_AS400NO,a.CST_PRT_NO,a.CST_PAINTLINE_CONTENT,");
            sqlsb.Append("a.CST_SPEC_AS400VER,b.CST_PRT_DESC,a.CST_SPEC_STAGE FROM S_PLM_MES.PLM_MES_TIRE_PAINTLINE a inner join ");
            sqlsb.Append("(select CST_PRT_ETNO,CST_PRT_DESC,MAX(CST_PRT_EFFECTIVEDATE) as max_date ");
            sqlsb.Append("FROM S_PLM_MES.PLM_MES_TIRESPEC GROUP BY CST_PRT_ETNO,CST_PRT_DESC) b on a.CST_PRT_NO=b.CST_PRT_ETNO ");
            sqlsb.Append("order by a.CST_PAINTLINE_CONTENT,a.CST_SPEC_AS400VER desc')");

            return dt_Spec = ConnDB.QueryDB(sqlsb, db.DB_CSTS5);
        }

        private static DataTable GetJialiuCT()
        {
            DBName db = new DBName();

            StringBuilder sqlsb = new StringBuilder(200);

            sqlsb.Append("select * FROM OPENQUERY(MIDCSTC_PRD,'select NAME,CST_TIREVULCANIZATION_SUMMER ");
            sqlsb.Append("FROM S_PLM_MES.PLM_MES_TIREVULCANIZATIONEI WHERE CST_TIREVULCANIZATION_IP=305')");

            return dt_CT = ConnDB.QueryDB(sqlsb, db.DB_CSTS5);
        }
        private void SetDataTable()
        {
            dt_dgv.Columns.Add("SITE");
            dt_dgv.Columns.Add("SHOP_ORDER");
            dt_dgv.Columns.Add("ITEM");
            dt_dgv.Columns.Add("SO_TYPE");
            dt_dgv.Columns.Add("SPEC_NO");
            dt_dgv.Columns.Add("SPEC_VERSION");
            dt_dgv.Columns.Add("PRD_PHASE");
            dt_dgv.Columns.Add("START_DATE");
            dt_dgv.Columns.Add("COMP_DATE");
            dt_dgv.Columns.Add("QTY");
            dt_dgv.Columns.Add("UNIT");
            dt_dgv.Columns.Add("LOCATION");
            dt_dgv.Columns.Add("WORK_CENTER");
            dt_dgv.Columns.Add("CREATED_TIME");
            dt_dgv.Columns.Add("SAP_SO_NUM");
            dt_dgv.Columns.Add("STATUS");
            dt_dgv.Columns.Add("MSG");
            dt_dgv.Columns.Add("SAP_PROCESS_TIME");
            dt_dgv.Columns.Add("UPDATE_TO_ORACLE");
            dt_dgv.Columns.Add("UPDATE_TO_ORACLE_TIME");
        }

        private void SetDataGridView()
        {
            dgv.Columns["SITE"].HeaderText = "工厂";
            dgv.Columns["SHOP_ORDER"].HeaderText = "ME生成key值";
            dgv.Columns["ITEM"].HeaderText = "料号";
            dgv.Columns["SO_TYPE"].HeaderText = "工单类型";
            dgv.Columns["SPEC_NO"].HeaderText = "SPEC代号";
            dgv.Columns["SPEC_VERSION"].HeaderText = "SPEC版次";
            dgv.Columns["PRD_PHASE"].HeaderText = "SPEC阶段";
            dgv.Columns["START_DATE"].HeaderText = "工单开始";
            dgv.Columns["COMP_DATE"].HeaderText = "工单结束";
            dgv.Columns["QTY"].HeaderText = "数量";
            dgv.Columns["UNIT"].HeaderText = "料号单位";
            dgv.Columns["LOCATION"].HeaderText = "储存地点";
            dgv.Columns["WORK_CENTER"].HeaderText = "工作中心";
            dgv.Columns["CREATED_TIME"].HeaderText = "ME创建时间";
            dgv.Columns["SAP_SO_NUM"].HeaderText = "SAP工单号";
            dgv.Columns["STATUS"].HeaderText = "SAP处理状态";
            dgv.Columns["MSG"].HeaderText = "SAP异常回报";
            dgv.Columns["SAP_PROCESS_TIME"].HeaderText = "SAP处理时间";
            dgv.Columns["UPDATE_TO_ORACLE"].HeaderText = "是否上传SAP";
            dgv.Columns["UPDATE_TO_ORACLE_TIME"].HeaderText = "上传SAP时间";

            dgv.Columns["SITE"].Width = 40;
            dgv.Columns["SHOP_ORDER"].Width = 100;
            dgv.Columns["ITEM"].Width = 80;
            dgv.Columns["SO_TYPE"].Width = 40;
            dgv.Columns["SPEC_NO"].Width = 60;
            dgv.Columns["SPEC_VERSION"].Width = 30;
            dgv.Columns["PRD_PHASE"].Width = 30;
            dgv.Columns["START_DATE"].Width = 60;
            dgv.Columns["COMP_DATE"].Width = 60;
            dgv.Columns["QTY"].Width = 40;
            dgv.Columns["UNIT"].Width = 40;
            dgv.Columns["LOCATION"].Width = 40;
            dgv.Columns["WORK_CENTER"].Width = 40;
            dgv.Columns["CREATED_TIME"].Width = 100;
            dgv.Columns["SAP_SO_NUM"].Width = 90;
            dgv.Columns["STATUS"].Width = 30;
            dgv.Columns["MSG"].Width = 130;
            dgv.Columns["SAP_PROCESS_TIME"].Width = 90;
            dgv.Columns["UPDATE_TO_ORACLE"].Width = 40;
            dgv.Columns["UPDATE_TO_ORACLE_TIME"].Width = 100;
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonFunc;
using NLog;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using NLog.Layouts;
using Oracle.ManagedDataAccess.Client;
using C_Dll_Login;

namespace Order_Create
{
    public partial class Order_Create : Form
    {
        public DataTable dt_use = new DataTable();
        public string order_Type = "";
        Logger log = LogManager.GetCurrentClassLogger();
        public static C_Dll_Login.ClassMain userLog;
        public bool limitday = false;
        public Order_Create()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DBName db = new DBName();
            DataTable dt_limit = new DataTable();
            DTP.Value= System.DateTime.Now; ;
            //cbb_OrderType.Items.Add("工单入库");
            label1.Text = "需先将工单资料建立后,才能执行工单上传";
            label1.ForeColor = Color.FromArgb(139,0,0);

            userLog = new C_Dll_Login.ClassMain();

            if (userLog.GetLogRight() is null)
            {
                userLog.SetProg("C_Order_Created");
                userLog.AddRight("B 报工权限");
                userLog.AddRight("P 排程权限");
                userLog.AddRight("T 通用");
                userLog.AddRight("S 管理员");
            }

            string sqlstr = "select StTime,EdTime FROM [CSTS_ERP_ORDER].[dbo].[Report_forbidden_Control] where On_Off=1 order by id desc";
            dt_limit = ConnDB.QueryDB(sqlstr, db.DB_CSTS5);

            if (dt_limit.Rows.Count > 0)
            {
                DateTime limitsttime =Convert.ToDateTime(dt_limit.Rows[0]["StTime"].ToString());
                DateTime limitedtime = Convert.ToDateTime(dt_limit.Rows[0]["EdTime"].ToString());
                DateTime dbtime = ConnDB.GetDBTime();

                if (limitsttime < dbtime && limitedtime > dbtime)
                {
                    limitday = true;
                }
            }

            SetComboBox(userLog.GetLogRight());

            SetButton_Close(userLog.GetLogRight());

            SetButton_Open(userLog.GetLogRight(), limitday);

            log.Trace("程序开启");
        }

        private void btn_OrderCreated_Click(object sender, EventArgs e)
        {
            if (order_Type != "")
            {
                SetButton_Close(userLog.GetLogRight());
                bgWorker_Created.RunWorkerAsync();
                progressBar.Visible = true;
            }
            else
            {
                MessageBox.Show("未选择工单类型","错误");
            }
        }
        private void btn_SAP_Update_Click(object sender, EventArgs e)
        {
            if (order_Type != "")
            {
                SetButton_Close(userLog.GetLogRight());
                bgWorker_Return.RunWorkerAsync();
                progressBar.Visible = true;
            }
            else
            {
                MessageBox.Show("未选择工单类型", "错误");
            }
        }

        private void btn_OrderUpdate_Click(object sender, EventArgs e)
        {
            if (order_Type != "")
            {
                SetButton_Close(userLog.GetLogRight());
                bgWorker_Update.RunWorkerAsync();
                progressBar.Visible = true;
            }
            else
            {
                MessageBox.Show("未选择工单类型", "错误");
            }

            btn_OrderUpdate.Enabled = false;
        }
        /// <summary>
        /// 工单状态查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_status_Click(object sender, EventArgs e)
        {
            string status_day = DTP.Value.ToString("yyyyMMdd");

            string sqlstr="";

            DBName db = new DBName();

            dgv.DataSource = null;

            SetButton_Close(userLog.GetLogRight());;

            switch (cbb_OrderType.Text)
            {
                case "工单开立":
                    dt_use = Load_Order_Create(status_day);
                    log.Trace("查询工单开立  选择日期:" + status_day);
                    break;
                case "工单报工":
                    dt_use = Load_Order_Posting(status_day);
                    log.Trace("查询工单报工  选择日期:" + status_day);
                    break;
                case "工单入库":
                    sqlstr = "select ITEM,SAP_SO_NUM,QTY,UNIT,STG_LOCATION,STATUS,MSG";
                    sqlstr += "FROM MES_ERP.ERP_SHOP_ORDER_INSTOCK ";
                    sqlstr += "where POSTING_DATE=" + status_day + "and SITE=2061 order by SAP_SO_NUM";
                    break;
            }

            if (dt_use.Rows.Count == 0) MessageBox.Show("该日尚未有相对应的工单", "提示");

            dgv.DataSource = dt_use;
            
            SetDataGridViewCol(cbb_OrderType.Text);

            SetButton_Open(userLog.GetLogRight(), limitday);
        }

        private static DataTable Load_Order_Create(string loadDay)
        {
            DBName db = new DBName();
            DataTable dt = new DataTable();
            StringBuilder strsb = new StringBuilder();

            strsb.Append("SELECT SITE, SHOP_ORDER, ITEM, SO_TYPE, SPEC_NO, SPEC_VERSION, PRD_PHASE,");
            strsb.Append("START_DATE, COMP_DATE, QTY, UNIT, LOCATION, WORK_CENTER, CREATED_TIME, SAP_SO_NUM,");
            strsb.Append("STATUS, MSG, SAP_PROCESS_TIME, (CASE UPDATE_TO_ORACLE WHEN 1 THEN '否' WHEN 0 THEN '是' END) as UPDATE_TO_ORACLE,");
            strsb.Append("UPDATE_TO_ORACLE_TIME FROM [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Create] ");
            strsb.Append("where START_DATE ='" + loadDay + "'" + " order by SHOP_ORDER ");
            //strsb.Append("where START_DATE ='" + loadDay + "'" + " and STATUS='I' order by SHOP_ORDER ");
            dt = ConnDB.QueryDB(strsb, db.DB_CSTS5);

            return dt;
        }
        private static DataTable Load_Order_Posting(string loadDay)
        {
            DBName db = new DBName();
            DataTable dt = new DataTable();
            StringBuilder strsb = new StringBuilder();

            strsb.Append("SELECT SITE, ITEM, SAP_SO_NUM, QTY, UNIT, SCRAP_QTY, WORK_CENTER, MACHINE_TIME, HUMAN_TIME, ");
            strsb.Append("STG_LOCATION, POSTING_DATE, CREATED_TIME, STATUS, MSG, SAP_PROCESS_TIME, COMP_NO, COMP_COUNT, ");
            strsb.Append("DOC_NO, DOC_YEAR, (CASE UPDATE_TO_ORACLE WHEN 1 THEN '否' WHEN 0 THEN '是' END) as UPDATE_TO_ORACLE,");
            strsb.Append(" UPDATE_TO_ORACLE_TIME FROM [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Posting] ");
            strsb.Append("where POSTING_DATE ='" + loadDay + "'" + "order by SAP_SO_NUM ");
            dt = ConnDB.QueryDB(strsb, db.DB_CSTS5);

            return dt;
        }

        private void Create_JiaLiuPlan(string createDay)
        {
            DBName db = new DBName();
            DataTable dt_cr1 = new DataTable();
            DataTable dt_cr2 = new DataTable();
            StringBuilder sqlsb_cr1 = new StringBuilder();
            StringBuilder sqlsb_cr2 = new StringBuilder();
            bool cr1_enter = false;
            bool cr2_enter = false;

            //查询[CSTCKMACH].[dbo].[JiaLiu_Plan_Day]表中是否已有该日排程,区分加硫1课与加硫2课
            //sqlsb_cr1.Append("select a.SPEC_NO, b.ITEM, a.PRD_PHASE, a.SPEC_VERSION ");
            //sqlsb_cr1.Append("FROM [CSTCKMACH].[dbo].[JiaLiu_Plan_Week] as a left join [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] as b ");
            //sqlsb_cr1.Append("on a.PLAN_ID = b.PLAN_ID where b.PLAN_TIME='" + createDay + "' and b.MACHINE between 101 and 252 ");
            //sqlsb_cr1.Append("and b.PLAN_STOP=1 GROUP BY a.SPEC_NO,b.ITEM,a.PRD_PHASE,a.SPEC_VERSION ");
            //dt_cr1 = ConnDB.QueryDB(sqlsb_cr1, db.DB_CSTS5);
            //if (dt_cr1.Rows.Count > 0) cr1_enter = true;

            sqlsb_cr1.Append("select distinct ITEM FROM [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] ");
            sqlsb_cr1.Append("where PLAN_TIME='" + createDay + "' and PLAN_STOP=1 and MACHINE between 101 and 252 ");
            dt_cr1 = ConnDB.QueryDB(sqlsb_cr1, db.DB_CSTS5);
            if (dt_cr1.Rows.Count > 0) cr1_enter = true;

            //sqlsb_cr2.Append("select a.SPEC_NO, b.ITEM, a.PRD_PHASE, a.SPEC_VERSION ");
            //sqlsb_cr2.Append("FROM [CSTCKMACH].[dbo].[JiaLiu_Plan_Week] as a left join [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] as b ");
            //sqlsb_cr2.Append("on a.PLAN_ID = b.PLAN_ID where b.PLAN_TIME='" + createDay + "' and b.MACHINE between 301 and 430 ");
            //sqlsb_cr2.Append("and b.PLAN_STOP=1 GROUP BY a.SPEC_NO,b.ITEM,a.PRD_PHASE,a.SPEC_VERSION ");
            //dt_cr2 = ConnDB.QueryDB(sqlsb_cr2, db.DB_CSTS5);
            //if (dt_cr2.Rows.Count > 0) cr2_enter = true;

            sqlsb_cr2.Append("select distinct ITEM FROM [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] ");
            sqlsb_cr2.Append("where PLAN_TIME='" + createDay + "' and PLAN_STOP=1 and MACHINE between 301 and 430 ");
            dt_cr2 = ConnDB.QueryDB(sqlsb_cr2, db.DB_CSTS5);
            if (dt_cr2.Rows.Count > 0) cr2_enter = true;


            //如果有排程且未开立工单,在[CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Create]表中写入开立工单资料,等待传入ORACLE
            if (cr1_enter == true || cr2_enter == true)
            {
                StringBuilder sqlsb = new StringBuilder();
                sqlsb.Append("INSERT INTO [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Create] (SITE, SHOP_ORDER, ITEM, SO_TYPE,");
                sqlsb.Append("SPEC_NO, SPEC_VERSION, PRD_PHASE, START_DATE, COMP_DATE, QTY, UNIT, LOCATION, WORK_CENTER,");
                sqlsb.Append("CREATED_TIME, STATUS) values ");

                string site = "2061";//工厂
                string shop_order = "";//MES工单号(自定义)
                string item = "";//SAP料号
                string so_type = "";//工单类型
                string spec_no = "";//SPEC代号
                string spec_version = "";//SPEC版次
                string prd_phase = "";//阶段
                string start_date = "";//MES排程日期
                string comp_date = "";//MES排程日期
                int qty = 0;//数量
                string unit = "PCS";//料号单位
                string location = "50F2";//存储地点(加硫50F2)(研发50R2)
                string work_center = "";//工作中心
                string created_time = "";//资料建立时间
                string status = "I";//SAP处理状态

                if (cr1_enter == true)//加硫1课的
                {
                    for (int i = 0; i < dt_cr1.Rows.Count; i++)
                    {
                        item = Convert.ToString(dt_cr1.Rows[i]["ITEM"]);
                        //规格版次需要修改抓取来源
                        StringBuilder sqlsb_Ver = new StringBuilder(100);
                        DataTable dt_cr1_ver = new DataTable();
                        sqlsb_Ver.Append("select TOP 1 SPEC_NO,PRD_PHASE,SPEC_VERSION FROM [CSTCKMACH].[dbo].[JiaLiu_Plan_Week] ");
                        sqlsb_Ver.Append("where ITEM='" + item + "' and PLAN_STARTTIME <= '" + createDay + "' and PLAN_ENDTIME >= '" + createDay + "' ");
                        sqlsb_Ver.Append("and MACHINE between 101 and 252 order by id desc");
                        dt_cr1_ver = ConnDB.QueryDB(sqlsb_Ver, db.DB_CSTS5);

                        if (dt_cr1_ver.Rows.Count == 0 ) return;

                        qty = 0;
                        work_center = "CR1";
                        shop_order = work_center + DTP.Value.ToString("yyyyMMdd") + (i + 1).ToString("000");
                        prd_phase = Convert.ToString(dt_cr1_ver.Rows[0]["PRD_PHASE"]);
                        switch (prd_phase)
                        {
                            case "A":
                                so_type = "ZP02";
                                location = "50R1";
                                break;
                            case "B":
                                so_type = "ZP01";
                                location = "50F2";
                                break;
                            case "C":
                                so_type = "ZP01";
                                location = "50F2";
                                break;
                        }
                        spec_no = Convert.ToString(dt_cr1_ver.Rows[0]["SPEC_NO"]);
                        spec_version = Convert.ToString(dt_cr1_ver.Rows[0]["SPEC_VERSION"]);
                        start_date = createDay;
                        comp_date = createDay;
                        //qty = Convert.ToInt32(dt_cr1.Rows[i]["TOTAL"]);
                        qty = GetSpecQty(item, createDay, "CR1");
                        created_time = DateTime.Now.ToString("yyyyMMddHHmmss");

                        SqlParameter[] paras_cr1 = new SqlParameter[]
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

                        StringBuilder strsb_cr1_insert = new StringBuilder();
                        strsb_cr1_insert.Append(sqlsb + "(" + "@SITE" + ",");
                        strsb_cr1_insert.Append("@SHOP_ORDER" + "," + "@ITEM" + "," + "@SO_TYPE" + "," + "@SPEC_NO" + "," + "@SPEC_VERSION" + ",");
                        strsb_cr1_insert.Append("@PRD_PHASE" + "," + "@START_DATE" + "," + "@COMP_DATE" + "," + "@QTY" + "," + "@UNIT" + ",");
                        strsb_cr1_insert.Append("@LOCATION" + "," + "@WORK_CENTER" + "," + "@CREATED_TIME" + "," + "@STATUS" + ")");
                        ConnDB.ExcuteSQL(strsb_cr1_insert, db.DB_CSTS5, paras_cr1);
                        
                    }
                }

                if (cr2_enter == true)//加硫2课的
                {
                    for (int i = 0; i < dt_cr2.Rows.Count; i++)
                    {
                        item = Convert.ToString(dt_cr2.Rows[i]["ITEM"]);

                        StringBuilder sqlsb2_Ver = new StringBuilder(100);
                        DataTable dt_cr2_ver = new DataTable();
                        sqlsb2_Ver.Append("select TOP 1 SPEC_NO,PRD_PHASE,SPEC_VERSION FROM [CSTCKMACH].[dbo].[JiaLiu_Plan_Week] ");
                        sqlsb2_Ver.Append("where ITEM='" + item + "' and PLAN_STARTTIME <= '" + createDay + "' and PLAN_ENDTIME >= '" + createDay + "' ");
                        sqlsb2_Ver.Append("and MACHINE between 301 and 430 order by id desc");
                        dt_cr2_ver = ConnDB.QueryDB(sqlsb2_Ver, db.DB_CSTS5);

                        if (dt_cr2_ver.Rows.Count == 0) return;

                        qty = 0;
                        work_center = "CR2";
                        shop_order = work_center + DTP.Value.ToString("yyyyMMdd") + (i + 1).ToString("000");
                        
                        prd_phase = Convert.ToString(dt_cr2_ver.Rows[0]["PRD_PHASE"]);
                        switch (prd_phase)
                        {
                            case "A":
                                so_type = "ZP02";
                                location = "50R2";
                                break;
                            case "B":
                                so_type = "ZP01";
                                location = "50F2";
                                break;
                            case "C":
                                so_type = "ZP01";
                                location = "50F2";
                                break;
                        }
                        spec_no = Convert.ToString(dt_cr2_ver.Rows[0]["SPEC_NO"]);
                        spec_version = Convert.ToString(dt_cr2_ver.Rows[0]["SPEC_VERSION"]);
                        start_date = createDay;
                        comp_date = createDay;
                        //qty = Convert.ToInt32(dt_cr2.Rows[i]["TOTAL"]);
                        qty = GetSpecQty(item, createDay, "CR2");
                        created_time = DateTime.Now.ToString("yyyyMMddHHmmss");

                        SqlParameter[] paras_cr2 = new SqlParameter[]
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

                        StringBuilder strsb_cr2_insert = new StringBuilder();
                        strsb_cr2_insert.Append(sqlsb + "(" + "@SITE" + ",");
                        strsb_cr2_insert.Append("@SHOP_ORDER" + "," + "@ITEM" + "," + "@SO_TYPE" + "," + "@SPEC_NO" + "," + "@SPEC_VERSION" + ",");
                        strsb_cr2_insert.Append("@PRD_PHASE" + "," + "@START_DATE" + "," + "@COMP_DATE" + "," + "@QTY" + "," + "@UNIT" + ",");
                        strsb_cr2_insert.Append("@LOCATION" + "," + "@WORK_CENTER" + "," + "@CREATED_TIME" + "," + "@STATUS" + ")");
                        ConnDB.ExcuteSQL(strsb_cr2_insert, db.DB_CSTS5, paras_cr2);
                    }
                }
            }
            else
            {
                MessageBox.Show("该日未有加硫排程","提示");
            }
        }
        /// <summary>
        /// 开立工单(上传Oracle)
        /// </summary>
        /// <param name="update_Day"></param>
        private void Update_Oracle_Order(int dr)
        {
            DBName db = new DBName();
            string site ;
            string shop_order;
            string item;
            string so_type;
            string spec_no;
            string spec_ver;
            string prd_phase;
            string start_date;
            string comp_date ;
            string qty;
            string unit;
            string location;
            string work_center;
            string mes_process_time;
            string status;

            site = dgv.Rows[dr].Cells["SITE"].Value.ToString();
            shop_order = dgv.Rows[dr].Cells["SHOP_ORDER"].Value.ToString();
            item = dgv.Rows[dr].Cells["ITEM"].Value.ToString();
            so_type = dgv.Rows[dr].Cells["SO_TYPE"].Value.ToString();
            spec_no = dgv.Rows[dr].Cells["SPEC_NO"].Value.ToString();
            spec_ver = dgv.Rows[dr].Cells["SPEC_VERSION"].Value.ToString();
            prd_phase = dgv.Rows[dr].Cells["PRD_PHASE"].Value.ToString();
            start_date = dgv.Rows[dr].Cells["START_DATE"].Value.ToString();
            comp_date = dgv.Rows[dr].Cells["COMP_DATE"].Value.ToString();
            qty = dgv.Rows[dr].Cells["QTY"].Value.ToString();
            unit = dgv.Rows[dr].Cells["UNIT"].Value.ToString();
            location = dgv.Rows[dr].Cells["LOCATION"].Value.ToString();
            work_center = dgv.Rows[dr].Cells["WORK_CENTER"].Value.ToString();
            mes_process_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            status = dgv.Rows[dr].Cells["STATUS"].Value.ToString();

            OracleParameter[] paras = new OracleParameter[]
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

            paras[0].Value = site;
            paras[1].Value = shop_order;
            paras[2].Value = item;
            paras[3].Value = so_type;
            paras[4].Value = spec_no;
            paras[5].Value = spec_ver;
            paras[6].Value = prd_phase;
            paras[7].Value = start_date;
            paras[8].Value = comp_date;
            paras[9].Value = qty;
            paras[10].Value = unit;
            paras[11].Value = location;
            paras[12].Value = work_center;
            paras[13].Value = mes_process_time;
            paras[14].Value = status;

            StringBuilder sqlsb = new StringBuilder(400);
            sqlsb.Append("Insert into MES_ERP.ERP_SHOP_ORDER_CREATE_HEAD (SITE, SHOP_ORDER, ITEM, SO_TYPE, SPEC_NO, ");
            sqlsb.Append("SPEC_VERSION, PRD_PHASE, START_DATE, COMP_DATE, QTY, UNIT, STG_LOCATION, WORK_CENTER, MES_PROCESS_TIME, ");
            sqlsb.Append("STATUS) values ");
            sqlsb.Append("(" + ":SITE" + "," + ":SHOP_ORDER" + "," + ":ITEM" + "," + ":SO_TYPE" + "," + ":SPEC_NO" + ",");
            sqlsb.Append(":SPEC_VERSION" + "," + ":PRD_PHASE" + "," + ":START_DATE" + "," + ":COMP_DATE" + "," + ":QTY" + ",");
            sqlsb.Append(":UNIT" + "," + ":STG_LOCATION" + "," + ":WORK_CENTER" + "," + ":MES_PROCESS_TIME" + "," + ":STATUS" + ")");
            ConnDB.ExcuteSQL(sqlsb, db.DB_Oracle, paras);

            string update_to_oracle_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            SqlParameter[] paras_ms = new SqlParameter[]
            {
                new SqlParameter("@UPDATE_TO_ORACLE_TIME",update_to_oracle_time),
                new SqlParameter("@SHOP_ORDER",shop_order)
            };

            //更新MSSQL资料
            string sqlstr = "Update [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Create] Set UPDATE_TO_ORACLE=0,";
            sqlstr += "UPDATE_TO_ORACLE_TIME=" + "@UPDATE_TO_ORACLE_TIME" + " where SHOP_ORDER=" + "@SHOP_ORDER" + "";

            ConnDB.ExcuteSQL(sqlstr, db.DB_CSTS5,paras_ms);
        }
        /// <summary>
        /// 报工(上传Oracle)
        /// </summary>
        /// <param name="update_Day"></param>
        private void Update_Oracle_Posting(int dr)
        {
            DBName db = new DBName();
            string site;
            string item;
            string sap_so_num;
            string qty;
            string unit;
            string scrap_qty;
            string work_center;
            string machine_time;
            string human_time;
            string stg_location;
            string posting_date;
            string mes_process_time;
            string status;

            site = dgv.Rows[dr].Cells["SITE"].Value.ToString();
            item = dgv.Rows[dr].Cells["ITEM"].Value.ToString();
            sap_so_num = dgv.Rows[dr].Cells["SAP_SO_NUM"].Value.ToString();
            qty = dgv.Rows[dr].Cells["QTY"].Value.ToString();
            unit = dgv.Rows[dr].Cells["UNIT"].Value.ToString();
            scrap_qty = dgv.Rows[dr].Cells["SCRAP_QTY"].Value.ToString();
            work_center = dgv.Rows[dr].Cells["WORK_CENTER"].Value.ToString();
            machine_time = dgv.Rows[dr].Cells["MACHINE_TIME"].Value.ToString();
            human_time = dgv.Rows[dr].Cells["HUMAN_TIME"].Value.ToString();
            stg_location = dgv.Rows[dr].Cells["STG_LOCATION"].Value.ToString();
            posting_date = dgv.Rows[dr].Cells["POSTING_DATE"].Value.ToString();
            mes_process_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            status = dgv.Rows[dr].Cells["STATUS"].Value.ToString();

            if (qty != "0")
            {
                OracleParameter[] paras = new OracleParameter[]
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

                paras[0].Value = site;
                paras[1].Value = item;
                paras[2].Value = sap_so_num;
                paras[3].Value = qty;
                paras[4].Value = unit;
                paras[5].Value = scrap_qty;
                paras[6].Value = work_center;
                paras[7].Value = machine_time;
                paras[8].Value = human_time;
                paras[9].Value = stg_location;
                paras[10].Value = posting_date;
                paras[11].Value = mes_process_time;
                paras[12].Value = status;

                StringBuilder sqlsb = new StringBuilder(400);
                sqlsb.Append("Insert into MES_ERP.ERP_SHOP_ORDER_POSTING_HEAD (SITE, ITEM, SAP_SO_NUM, QTY, UNIT, ");
                sqlsb.Append("SCRAP_QTY, WORK_CENTER, MACHINE_TIME, HUMAN_TIME, STG_LOCATION, POSTING_DATE, MES_PROCESS_TIME, STATUS)");
                sqlsb.Append(" values ");
                sqlsb.Append("(" + ":SITE" + "," + ":ITEM" + "," + ":SAP_SO_NUM" + "," + ":QTY" + "," + ":UNIT" + ",");
                sqlsb.Append(":SCRAP_QTY" + "," + ":WORK_CENTER" + "," + ":MACHINE_TIME" + "," + ":HUMAN_TIME" + "," + ":STG_LOCATION" + ",");
                sqlsb.Append(":POSTING_DATE" + "," + ":MES_PROCESS_TIME" + "," + ":STATUS" + ")");
                ConnDB.ExcuteSQL(sqlsb, db.DB_Oracle, paras);

                string update_to_oracle_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlParameter[] paras_ms = new SqlParameter[]
                {
                new SqlParameter("@UPDATE_TO_ORACLE_TIME",update_to_oracle_time),
                new SqlParameter("@SAP_SO_NUM",sap_so_num)
                };

                //更新MSSQL资料
                string sqlstr = "Update [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Posting] Set UPDATE_TO_ORACLE=0,";
                sqlstr += "UPDATE_TO_ORACLE_TIME=" + "@UPDATE_TO_ORACLE_TIME" + " where SAP_SO_NUM=" + "@SAP_SO_NUM" + "";

                ConnDB.ExcuteSQL(sqlstr, db.DB_CSTS5, paras_ms);
            }
        }
        /// <summary>
        /// 加硫报工数据建立(MSSQL)
        /// </summary>
        /// <param name="createDay"></param>
        private void Create_JiaLiuPosting(string sap_Num,string sap_Item,string sap_WorkCenter)
        {
            DBName db = new DBName();
            DataTable dt = new DataTable();

            string site = "2061";
            string item = sap_Item;
            string sap_so_num = sap_Num;
            string qty = "";
            string unit = "PCS";
            string scrap_qty = "0";//报废数量
            string work_center = sap_WorkCenter;//工作中心
            string machine_time = "0";//机器工时
            string human_time = "0";//人工工时
            string stg_location = "50G1";//扣账地点
            string posting_date = DTP.Value.ToString("yyyyMMdd");//过账日期
            string created_time = ConnDB.GetDBTime().ToString("yyyyMMddHHmmss");
            string status = "I";

            string sqlstr = "select count(SAP_SO_NUM) as QTY FROM [CSTCLOT" + posting_date.Substring(0,4)+ "].[dbo].[V_Xbar] ";
            sqlstr += "Where SAP_SO_NUM ='" + sap_Num + "' and jiaMach is Not Null and JiaTime is Not Null and JiaTime<>0 and ";
            sqlstr += "(IsADD <> 'C' or IsADD is null) ";
            
            dt = ConnDB.QueryDB(sqlstr, db.DB_CSTS5);

            qty = dt.Rows[0]["QTY"].ToString();

            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@SITE",site),
                new SqlParameter("@ITEM",item),
                new SqlParameter("@SAP_SO_NUM",sap_so_num),
                new SqlParameter("@QTY",qty),
                new SqlParameter("@UNIT",unit),
                new SqlParameter("@SCRAP_QTY",scrap_qty),
                new SqlParameter("@WORK_CENTER",work_center),
                new SqlParameter("@MACHINE_TIME",machine_time),
                new SqlParameter("@HUMAN_TIME",human_time),
                new SqlParameter("@STG_LOCATION",stg_location),
                new SqlParameter("@POSTING_DATE",posting_date),
                new SqlParameter("@CREATED_TIME",created_time),
                new SqlParameter("@STATUS",status)
            };

            StringBuilder sqlsb = new StringBuilder(300);
            sqlsb.Append("Insert into [CSTS_ERP_ORDER].[dbo].[jiaLiu_Order_Posting] (SITE, ITEM, SAP_SO_NUM, ");
            sqlsb.Append("QTY, UNIT, SCRAP_QTY, WORK_CENTER, MACHINE_TIME, HUMAN_TIME, STG_LOCATION, POSTING_DATE, ");
            sqlsb.Append("CREATED_TIME, STATUS) values ");
            sqlsb.Append("(" + "@SITE" + "," + "@ITEM" + "," + "@SAP_SO_NUM" + "," + "@QTY" + "," + "@UNIT" + ",");
            sqlsb.Append("@SCRAP_QTY" + "," + "@WORK_CENTER" + "," + "@MACHINE_TIME" + "," + "@HUMAN_TIME" + "," + "@STG_LOCATION" + ",");
            sqlsb.Append("@POSTING_DATE" + "," + "@CREATED_TIME" + "," + "@STATUS" + ")");
            ConnDB.ExcuteSQL(sqlsb, db.DB_CSTS5, paras);
        }

        private void SetDataGridViewCol(string tableType)
        {
            switch (tableType)
            {
                case "工单开立":
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
                    dgv.Columns["WORK_CENTER"].Width =40;
                    dgv.Columns["CREATED_TIME"].Width = 100;
                    dgv.Columns["SAP_SO_NUM"].Width = 90;
                    dgv.Columns["STATUS"].Width = 30;
                    dgv.Columns["MSG"].Width = 130;
                    dgv.Columns["SAP_PROCESS_TIME"].Width = 90;
                    dgv.Columns["UPDATE_TO_ORACLE"].Width = 40;
                    dgv.Columns["UPDATE_TO_ORACLE_TIME"].Width = 100;
                    break;
                case "工单报工":
                    dgv.Columns["SITE"].HeaderText = "工厂";
                    dgv.Columns["ITEM"].HeaderText = "料号";
                    dgv.Columns["SAP_SO_NUM"].HeaderText = "SAP工单号";
                    dgv.Columns["QTY"].HeaderText = "数量";
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
                    dgv.Columns["QTY"].Width = 60;
                    dgv.Columns["UNIT"].Width = 40;
                    dgv.Columns["SCRAP_QTY"].Width = 40;
                    dgv.Columns["WORK_CENTER"].Width = 40;
                    dgv.Columns["MACHINE_TIME"].Width = 30;
                    dgv.Columns["HUMAN_TIME"].Width = 30;
                    dgv.Columns["STG_LOCATION"].Width = 40;
                    dgv.Columns["POSTING_DATE"].Width = 60;
                    dgv.Columns["CREATED_TIME"].Width =100;
                    dgv.Columns["STATUS"].Width = 30;
                    dgv.Columns["MSG"].Width = 100;
                    dgv.Columns["SAP_PROCESS_TIME"].Width = 90;
                    dgv.Columns["COMP_NO"].Width = 80;
                    dgv.Columns["COMP_COUNT"].Width = 60;
                    dgv.Columns["DOC_NO"].Width = 50;
                    dgv.Columns["DOC_YEAR"].Width = 40;
                    dgv.Columns["UPDATE_TO_ORACLE"].Width = 40;
                    dgv.Columns["UPDATE_TO_ORACLE_TIME"].Width = 100;
                    break;
                case "工单入库":
                    break;

            }

            int sum = 0;

            if (tableType == "工单报工")
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    sum += Convert.ToInt32(dgv.Rows[i].Cells["QTY"].Value.ToString());
                    dgv.Columns["QTY"].HeaderText = "数量(" + sum + ")";
                }
            }
        }

        private void SetButton_Open(string logPermission,bool limit)
        {
            switch (logPermission)
            {
                case "B":
                    Meun_Query.Enabled = true;
                    if (limit == false) btn_OrderUpdate.Enabled = true;
                    btn_SAP_Update.Enabled = true;
                    btn_OrderCreated.Enabled = true;
                    break;
                case "P":
                    Menu_Plan.Enabled = true;
                    btn_OrderUpdate.Enabled = true;
                    btn_SAP_Update.Enabled = true;
                    btn_OrderCreated.Enabled = true;
                    break;
                case "T":
                    Menu_Plan.Enabled = true;
                    Meun_Query.Enabled = true;
                    if (limit == false) btn_OrderUpdate.Enabled = true; ;
                    btn_SAP_Update.Enabled = true;
                    btn_OrderCreated.Enabled = true;
                    break;
                case "S":
                    Menu_Plan.Enabled = true;
                    Meun_Query.Enabled = true;
                    btn_OrderUpdate.Enabled = true;
                    btn_SAP_Update.Enabled = true;
                    btn_OrderCreated.Enabled = true;
                    break;
                default:
                    btn_status.Enabled = true;
                    break;
            }
        }

        private void SetButton_Close(string logPermission)
        {
            switch (logPermission)
            {
                case "B":
                    Menu_Plan.Enabled = false;
                    btn_OrderUpdate.Enabled = false;
                    btn_SAP_Update.Enabled = false;
                    btn_OrderCreated.Enabled = false;
                    break;
                case "P":
                    Meun_Query.Enabled = false;
                    btn_OrderUpdate.Enabled = false;
                    btn_SAP_Update.Enabled = false;
                    btn_OrderCreated.Enabled = false;
                    break;
                case "T":
                    btn_OrderUpdate.Enabled = false;
                    btn_SAP_Update.Enabled = false;
                    btn_OrderCreated.Enabled = false;
                    break;
                case "S":
                    btn_OrderUpdate.Enabled = false;
                    btn_SAP_Update.Enabled = false;
                    btn_OrderCreated.Enabled = false;
                    break;
                default:
                    Menu_Plan.Enabled = false;
                    Meun_Query.Enabled = false;
                    btn_OrderUpdate.Enabled = false;
                    btn_SAP_Update.Enabled = false;
                    btn_OrderCreated.Enabled = false;
                    break;
            }
        }
        private void SetComboBox(string logPermission)
        {
            cbb_OrderType.Items.Clear();
            switch (logPermission)
            {
                case "B":
                    cbb_OrderType.Items.Add("工单报工");
                    break;
                case "P":
                    cbb_OrderType.Items.Add("工单开立");
                    break;
                case "T":
                    cbb_OrderType.Items.Add("工单开立");
                    cbb_OrderType.Items.Add("工单报工");
                    break;
                case "S":
                    cbb_OrderType.Items.Add("工单开立");
                    cbb_OrderType.Items.Add("工单报工");
                    break;
                default:
                    cbb_OrderType.Items.Add("工单开立");
                    cbb_OrderType.Items.Add("工单报工");
                    break;
            }
        }
        private void Menu_Plan_Main_Click(object sender, EventArgs e)
        {
            this.Hide();
            new JiaLiuPlan().ShowDialog();
            this.Close();
        }
        private void Menu_Plan_Temporary_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Order_Create_Temp().ShowDialog();
            this.Close();
        }
        private void Posting_difference_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Posting().ShowDialog();
            this.Close();
        }

        private void Order_bar_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Order_BarCode().ShowDialog();
            this.Close();
        }

        private void cbb_OrderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            order_Type = cbb_OrderType.Text;
        }

        private static int GetSpecQty(string item,string dtime,string workcenter)
        {
            DBName db = new DBName();
            DataTable dt = new DataTable();
            StringBuilder sqlsb = new StringBuilder(100);
            int qty;

            sqlsb.Append("select sum(qty) as Total FROM [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] where PLAN_TIME='" + dtime + "' ");
            if (workcenter == "CR1")
            {
                sqlsb.Append("and ITEM = '" + item + "' and MACHINE between 101 and 252 ");
            }
            else
            {
                sqlsb.Append("and ITEM = '" + item + "' and MACHINE between 301 and 430 ");
            }

            dt = ConnDB.QueryDB(sqlsb, db.DB_CSTS5);

            if (dt.Rows.Count > 0)
            {
                qty = Convert.ToInt32(dt.Rows[0]["Total"]);
            }
            else
            {
                qty = 0;
            }

            return qty;
        }
        /// <summary>
        /// 更新Oracle加硫工单状态到MSSQL
        /// </summary>
        /// <param name="shop_Order"></param>
        private void Updata_SAP_Order(string shop_Order)
        {
            DataTable dt = new DataTable() ;
            DBName db = new DBName();

            string sap_so_num = "";
            string status = "";
            string msg = "";
            string sap_process_time = "";

            string Oracle_sqlstr = "select SAP_SO_NUM, STATUS, MSG, SAP_PROCESS_TIME FROM MES_ERP.ERP_SHOP_ORDER_CREATE_HEAD ";
            Oracle_sqlstr += "Where SHOP_ORDER ='" + shop_Order + "' and SITE='2061' " ;

            dt = ConnDB.QueryDB(Oracle_sqlstr, db.DB_Oracle);

            if (dt.Rows.Count > 0)
            {
                sap_so_num = dt.Rows[0]["SAP_SO_NUM"].ToString();
                status = dt.Rows[0]["STATUS"].ToString();
                msg = dt.Rows[0]["MSG"].ToString();
                sap_process_time = dt.Rows[0]["SAP_PROCESS_TIME"].ToString();

                SqlParameter[] paras_ms = new SqlParameter[]
                {
                    new SqlParameter("@SAP_SO_NUM",sap_so_num),
                    new SqlParameter("@STATUS",status),
                    new SqlParameter("@MSG",msg),
                    new SqlParameter("@SAP_PROCESS_TIME",sap_process_time),
                    new SqlParameter("@SHOP_ORDER",shop_Order)
                };

                string sqlstr = "Update [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Create] Set SAP_SO_NUM =" + "@SAP_SO_NUM" + ",";
                sqlstr += "STATUS=" + "@STATUS" + ",MSG=" + "@MSG" + ",SAP_PROCESS_TIME=" + "@SAP_PROCESS_TIME" + " ";
                sqlstr += "WHERE SHOP_ORDER=" + "@SHOP_ORDER" + "";
                ConnDB.ExcuteSQL(sqlstr, db.DB_CSTS5,paras_ms);
            }
        }
        private void Updata_SAP_Posting(string sap_Num,string qty)
        {
            DataTable dt = new DataTable();
            DBName db = new DBName();

            string sap_so_num = "";
            string status = "";
            string msg = "";
            string sap_process_time = "";
            string comp_no = "";
            string comp_count = "";
            string doc_no = "";
            string doc_year = "";


            string Oracle_sqlstr = "select SAP_SO_NUM, STATUS, MSG, SAP_PROCESS_TIME, COMP_NO, COMP_COUNT, DOC_NO,";
            Oracle_sqlstr += " DOC_YEAR FROM MES_ERP.ERP_SHOP_ORDER_POSTING_HEAD ";
            Oracle_sqlstr += "Where SAP_SO_NUM ='" + sap_Num + "' and QTY='" + qty + "' and SITE='2061' ";

            dt = ConnDB.QueryDB(Oracle_sqlstr, db.DB_Oracle);

            if (dt.Rows.Count > 0)
            {
                sap_so_num = dt.Rows[0]["SAP_SO_NUM"].ToString();
                status = dt.Rows[0]["STATUS"].ToString();
                msg = dt.Rows[0]["MSG"].ToString();
                sap_process_time = dt.Rows[0]["SAP_PROCESS_TIME"].ToString();
                comp_no = dt.Rows[0]["COMP_NO"].ToString();
                comp_count = dt.Rows[0]["COMP_COUNT"].ToString();
                doc_no = dt.Rows[0]["DOC_NO"].ToString();
                doc_year = dt.Rows[0]["DOC_YEAR"].ToString();

                SqlParameter[] paras_ms = new SqlParameter[]
                {
                    new SqlParameter("@SAP_SO_NUM",sap_so_num),
                    new SqlParameter("@QTY",qty),
                    new SqlParameter("@STATUS",status),
                    new SqlParameter("@MSG",msg),
                    new SqlParameter("@SAP_PROCESS_TIME",sap_process_time),
                    new SqlParameter("@COMP_NO",comp_no),
                    new SqlParameter("@COMP_COUNT",comp_count),
                    new SqlParameter("@DOC_NO",doc_no),
                    new SqlParameter("@DOC_YEAR",doc_year)
                };

                string sqlstr = "Update [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Posting] Set STATUS =" + "@STATUS" + ",";
                sqlstr += "MSG=" + "@MSG" + ",SAP_PROCESS_TIME=" + "@SAP_PROCESS_TIME" + ",COMP_NO=" + "@COMP_NO" + ",";
                sqlstr += "COMP_COUNT=" + "@COMP_COUNT" + ",DOC_NO=" + "@DOC_NO" + ",DOC_YEAR=" + "@DOC_YEAR" + " ";
                sqlstr += "WHERE SAP_SO_NUM=" + "@SAP_SO_NUM" + " and QTY=" + "@QTY" + " ";
                ConnDB.ExcuteSQL(sqlstr, db.DB_CSTS5, paras_ms);
            }
        }

        private void bgWorker_Created_DoWork(object sender, DoWorkEventArgs e)
        {
            string load_day = DTP.Value.ToString("yyyyMMdd"); ;

            //查询是否执行过工单上传
            switch (order_Type)
            {
                case "工单开立":
                    dt_use = Load_Order_Create(load_day);
                    if (dt_use.Rows.Count > 0)
                    {
                        MessageBox.Show("MSSQL已有该日" + order_Type + "资料", "提示");
                    }
                    else
                    {
                        Create_JiaLiuPlan(load_day);
                        dt_use = Load_Order_Create(load_day);
                        log.Trace("工单开立  选择日期:" + load_day);
                    }
                    break;
                case "工单报工":
                    dt_use = Load_Order_Posting(load_day);
                    if (dt_use.Rows.Count > 0)
                    {
                        MessageBox.Show("MSSQL已有该日" + order_Type + "资料", "提示");
                    }
                    else
                    {
                        dt_use = Load_Order_Create(load_day);//先查询是否有开加硫工单,没有开加硫工单无法报工
                        if (dt_use.Rows.Count > 0)
                        {

                            for (int i = 0; i < dt_use.Rows.Count; i++)
                            {
                                int pgb = Convert.ToInt32((double)i / (double)dt_use.Rows.Count * 100);
                                bgWorker_Created.ReportProgress(pgb);

                                string num = dt_use.Rows[i]["SAP_SO_NUM"].ToString();
                                string item = dt_use.Rows[i]["ITEM"].ToString();
                                string wc = dt_use.Rows[i]["WORK_CENTER"].ToString();
                                if (num != "")
                                {
                                    Create_JiaLiuPosting(num, item, wc);
                                }
                            }
                            dt_use = Load_Order_Posting(load_day);
                            log.Trace("工单报工  选择日期:" + load_day);
                        }
                        else
                        {
                            MessageBox.Show("该日未开立加硫工单无法建立报工资料", "提示");
                        }
                    }
                    break;
                case "工单入库":
                    break;
            }
        }

        private void bgWorker_Created_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int p = e.ProgressPercentage;

            progressBar.Value = p;
        }
        private void bgWorker_Created_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetButton_Open(userLog.GetLogRight(), limitday);

            progressBar.Value = 0;

            progressBar.Visible = false;

            dgv.DataSource = dt_use;

            SetDataGridViewCol(order_Type);
        }
        /// <summary>
        /// 回报SAP处理状态与错误信息(Oracle)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_Return_DoWork(object sender, DoWorkEventArgs e)
        {
            string status_day = DTP.Value.ToString("yyyyMMdd");
            switch (order_Type)
            {
                case "工单开立":
                    if (dgv.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv.Rows.Count; i++)
                        {
                            int pgb = Convert.ToInt32((double)i / (double)dgv.Rows.Count * 100);
                            bgWorker_Return.ReportProgress(pgb);
                            //只更新STATUS不等于S的工单
                            if (dgv.Rows[i].Cells["STATUS"].Value.ToString() != "S")
                            {
                                Updata_SAP_Order(dgv.Rows[i].Cells["SHOP_ORDER"].Value.ToString());
                            }
                        }
                        dt_use = Load_Order_Create(status_day);
                        log.Trace("更新SAP工单开立状态  选择日期:" + status_day);
                    }
                    break;
                case "工单报工":
                    if (dgv.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv.Rows.Count; i++)
                        {
                            int pgb = Convert.ToInt32((double)i / (double)dgv.Rows.Count * 100);
                            bgWorker_Return.ReportProgress(pgb);
                            //只更新STATUS不等于S的工单
                            string dgv_qty = dgv.Rows[i].Cells["QTY"].Value.ToString();
                            if (dgv_qty != "0" && dgv.Rows[i].Cells["STATUS"].Value.ToString() != "S")
                            {
                                Updata_SAP_Posting(dgv.Rows[i].Cells["SAP_SO_NUM"].Value.ToString(), dgv_qty);
                            }
                        }
                        dt_use = Load_Order_Posting(status_day);
                        log.Trace("更新SAP工单报工状态  选择日期:" + status_day);
                    }
                    break;
            }
        }

        private void bgWorker_Return_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int p = e.ProgressPercentage;

            this.progressBar.Value = p;
        }

        private void bgWorker_Return_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetButton_Open(userLog.GetLogRight(), limitday);

            this.progressBar.Value = 0;

            this.progressBar.Visible = false;

            dgv.DataSource = dt_use;

            SetDataGridViewCol(order_Type);
        }

        private void bgWorker_Update_DoWork(object sender, DoWorkEventArgs e)
        {
            string load_day = DTP.Value.ToString("yyyyMMdd");
            switch (order_Type)
            {
                case "工单开立":
                    if (dgv.Rows.Count > 0)
                    {
                        DataRow[] dr = dt_use.Select("UPDATE_TO_ORACLE='是'");//有上传过的记录

                        if (dr.Length == 0)
                        {
                            for (int i = 0; i < dgv.Rows.Count; i++)
                            {
                                int pgb = Convert.ToInt32((double)i / (double)dgv.Rows.Count * 100);
                                bgWorker_Update.ReportProgress(pgb);
                                Update_Oracle_Order(i);
                            }
                            dt_use = Load_Order_Create(load_day);
                            log.Trace("上传SAP工单开立状态  选择日期:" + load_day);
                        }
                        else
                        {
                            MessageBox.Show("该日" + order_Type + "已上传过Oracle", "提示");
                        }
                    }
                    break;
                case "工单报工":
                    if (dgv.Rows.Count > 0)
                    {
                        DataRow[] dr = dt_use.Select("UPDATE_TO_ORACLE='是'");//有上传过的记录
                        if (dr.Length == 0)
                        {
                            for (int i = 0; i < dgv.Rows.Count; i++)
                            {
                                int pgb = Convert.ToInt32((double)i / (double)dgv.Rows.Count * 100);
                                bgWorker_Update.ReportProgress(pgb);
                                Update_Oracle_Posting(i);
                            }
                            dt_use = Load_Order_Posting(load_day);
                            log.Trace("上传SAP工单报工状态  选择日期:" + load_day);
                        }
                        else
                        {
                            MessageBox.Show("该日" + order_Type + "已上传过Oracle", "提示");
                        }
                    }
                    break;
            }
        }

        private void bgWorker_Update_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int p = e.ProgressPercentage;

            this.progressBar.Value = p;
        }

        private void bgWorker_Update_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetButton_Open(userLog.GetLogRight(), limitday);

            progressBar.Value = 0;

            progressBar.Visible = false;

            dgv.DataSource = dt_use;

            SetDataGridViewCol(order_Type);
        }

        private void Lonin_Click(object sender, EventArgs e)
        {
            userLog.Login();

            SetButton_Close(userLog.GetLogRight());

            SetButton_Open(userLog.GetLogRight(), limitday);

            SetComboBox(userLog.GetLogRight());

            log.Trace("登入人员:" + userLog.GetLogUserName());
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            userLog.LogOut();

            SetButton_Close(userLog.GetLogRight());

            SetButton_Open(userLog.GetLogRight(), limitday);

            SetComboBox(userLog.GetLogRight());

            log.Trace("人员登出:" + userLog.GetLogUserName());
        }

    }
}

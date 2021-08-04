using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonFunc;

namespace Order_Create
{
    public partial class Order_BarCode : Form
    {
        public Order_BarCode()
        {
            InitializeComponent();
        }

        private void Order_BarCode_Load(object sender, EventArgs e)
        {
            DTP_Start.Value = DateTime.Now.AddDays(-1);
            DTP_End.Value = DateTime.Now;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DBName db = new DBName();
            DataTable dt_Order = new DataTable();
            DataTable dt_Lot = new DataTable();
            DataTable dt_Bad = new DataTable();
            DataTable dt_asrs = new DataTable();
            DataTable dt_pcfnwg = new DataTable();
            DataTable dt_All = new DataTable();

            dt_All.Columns.Add("条码");
            dt_All.Columns.Add("成品代号");
            dt_All.Columns.Add("大印字");
            dt_All.Columns.Add("加硫");
            dt_All.Columns.Add("外检");
            dt_All.Columns.Add("测定");
            dt_All.Columns.Add("成管扫描");
            dt_All.Columns.Add("轮胎状态");
            dt_All.Columns.Add("SAP_SO_NUM");
            dt_All.Columns.Add("日期");

            string order_sttime = DTP_Start.Value.ToString("yyyyMMdd");
            string order_edtime = DTP_End.Value.ToString("yyyyMMdd");
            int lot_sttime_year = DTP_Start.Value.AddDays(-20).Year;
            int lot_edtime_year = DTP_End.Value.AddDays(+20).Year;
            double lot_sttime = DTP_Start.Value.AddDays(-20).ToOADate();
            double lot_edtime = DTP_End.Value.AddDays(+20).ToOADate();
            
            //选择期间的加硫工单
            StringBuilder sqlsb = new StringBuilder();
            sqlsb.Append("Select SAP_SO_NUM,START_DATE FROM [CSTS_ERP_ORDER].[dbo].[JiaLiu_Order_Create] ");
            sqlsb.Append("where START_DATE between '" + order_sttime + "' and '" + order_edtime + "' order by START_DATE,SHOP_ORDER");
            dt_Order = ConnDB.QueryDB(sqlsb,db.DB_CSTS5);

            //报废胎资料
            StringBuilder sqlsb_bad = new StringBuilder();
            sqlsb_bad.Append("select T9FRSD FROM [CSTCUBUF].[dbo].[PCTX_UP400] where left(T9QUES,1)='C' and ");
            sqlsb_bad.Append("intime between " + lot_sttime + " and " + lot_edtime);
            dt_Bad = ConnDB.QueryDB(sqlsb_bad, db.DB_CSTS5);

            //成管扫描记录
            StringBuilder sqlsb_asrs = new StringBuilder();
            sqlsb_asrs.Append("select BARCODE_NO, SCAN_UPDATE_DATE FROM [ZXMES_ASRS].[dbo].[T_BARCODE] ");
            sqlsb_asrs.Append("where CREATION_DATE between "  + lot_sttime + " and " + lot_edtime + " ");
            sqlsb_asrs.Append("union ");
            sqlsb_asrs.Append("select BARCODE_NO, SCAN_UPDATE_DATE FROM [ZBARCODE].[dbo].[T_BARCODE" + DateTime.Now.AddMonths(-1).ToString("yyyyMM") + "] ");
            sqlsb_asrs.Append("where CREATION_DATE between " + lot_sttime + " and " + lot_edtime + " order by SCAN_UPDATE_DATE desc");
            dt_asrs = ConnDB.QueryDB(sqlsb_asrs, db.DB_CSTS5);

            //pcfnwg
            StringBuilder sqlsb_pcfnwd = new StringBuilder();
            sqlsb_pcfnwd.Append("select WGSPEC, WGITAD FROM [CSTCPUBLIC].[dbo].[pcfnwg] ");
            dt_pcfnwg = ConnDB.QueryDB(sqlsb_pcfnwd, db.DB_CSTS5);

            //选择期间前后20天条码资料
            StringBuilder sqlsb_lot = new StringBuilder();
            if (lot_sttime_year == lot_edtime_year)
            {
                sqlsb_lot.Append("select ChengBar as 条码, PRD1 as 成品代号, '' as 大印字, ");
                sqlsb_lot.Append("CONVERT(datetime,JiaTime-2) as 加硫, CONVERT(datetime,WJTime-2) as 外检, CONVERT(datetime,CeTime-2) as 测定, ");
                sqlsb_lot.Append("'' as 成管扫描, IsAdd as 轮胎状态, SAP_SO_NUM FROM [CSTCLOT" + lot_sttime_year + "].[dbo].");
                sqlsb_lot.Append("[V_Xbar] where Sttime between " + lot_sttime + " and " + lot_edtime );
                dt_Lot = ConnDB.QueryDB(sqlsb_lot, db.DB_CSTS5);
            }
            else
            {
                sqlsb_lot.Append("select ChengBar as 条码, PRD1 as 成品代号, '' as 大印字, ");
                sqlsb_lot.Append(" CONVERT(datetime,JiaTime-2) as 加硫, CONVERT(datetime,WJTime-2) as 外检, CONVERT(datetime,CeTime-2) as 测定, ");
                sqlsb_lot.Append("'' as 成管扫描, IsAdd as 轮胎状态, SAP_SO_NUM FROM [CSTCLOT" + lot_sttime_year + "].[dbo].");
                sqlsb_lot.Append("[V_Xbar] where Sttime between " + lot_sttime + " and " + lot_edtime );
                sqlsb_lot.Append("union ");
                sqlsb_lot.Append("select ChengBar as 条码, PRD1 as 成品代号, '' as 大印字, ");
                sqlsb_lot.Append(" CONVERT(datetime,JiaTime-2) as 加硫, CONVERT(datetime,WJTime-2) as 外检, CONVERT(datetime,CeTime-2) as 测定, ");
                sqlsb_lot.Append("'' as 成管扫描, IsAdd as 轮胎状态, SAP_SO_NUM FROM [CSTCLOT" + lot_edtime_year + "].[dbo].");
                sqlsb_lot.Append("[V_Xbar] where Sttime between " + lot_sttime + " and " + lot_edtime );
                dt_Lot = ConnDB.QueryDB(sqlsb_lot, db.DB_CSTS5);
            }

            for (int i = 0; i < dt_Order.Rows.Count; i++)
            {
                string order_day = dt_Order.Rows[i]["START_DATE"].ToString();
                DataRow[] dr = dt_Lot.Select("SAP_SO_NUM='" + dt_Order.Rows[i]["SAP_SO_NUM"].ToString() + "'");
                if (dr.Length > 0)
                {
                    for (int j = 0; j < dr.Length; j++)
                    {
                        string bar = dr[j]["条码"].ToString();
                        string prd1 = dr[j]["成品代号"].ToString();
                        string big = "";
                        string jiatime = dr[j]["加硫"].ToString();
                        string wjtime = dr[j]["外检"].ToString();
                        string cetime = dr[j]["测定"].ToString();
                        string asrs = dr[j]["成管扫描"].ToString();
                        string tire = dr[j]["轮胎状态"].ToString();
                        string sap_so_num = dr[j]["SAP_SO_NUM"].ToString();

                        //大印字
                        DataRow[] dr_big = dt_pcfnwg.Select("WGSPEC='" + prd1 + "'");
                        if (dr_big.Length > 0)
                        {
                            big = dr_big[0]["WGITAD"].ToString();
                        }

                        if (tire == "C") tire = "补条码";
                        //是否为报废胎
                        DataRow[] dr_bad = dt_Bad.Select("T9FRSD='" + bar + "'");
                        if (dr_bad.Length > 0)
                        {
                            tire = "报废";
                        }

                        //成管扫描时间
                        DataRow[] dr_asrs = dt_asrs.Select("BARCODE_NO='" + bar + "'");
                        if (dr_asrs.Length > 0)
                        {
                            asrs = dr_asrs[0]["SCAN_UPDATE_DATE"].ToString();
                        }

                        dt_All.Rows.Add(bar, prd1, big, jiatime, wjtime, cetime, asrs, tire, sap_so_num, order_day);
                    }
                }
            }

            dgv.DataSource = dt_All;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Order_Create().ShowDialog();
            this.Close();
        }
    }
}

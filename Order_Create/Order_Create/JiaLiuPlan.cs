using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Windows.Forms;
using CommonFunc;
using NLog;
using NLog.LayoutRenderers.Wrappers;

namespace Order_Create
{
    public partial class JiaLiuPlan : Form
    {
        DataTable dt_now = new DataTable();//查询排程用
        DataTable dt_next = new DataTable();
        DataTable dt_now_Main = new DataTable();//主要显示用
        DataTable dt_next_Main = new DataTable();
        DataTable dt_big = new DataTable();//大印字对应
        DataTable dt_CT = new DataTable();//单模产能表
        DataTable dt_machine = new DataTable();//机台当前规格
        DataTable dt_production_order = new DataTable();//制造通知单
        DataTable dt_machine_type = new DataTable();//加硫外壳机型

        private DateTimePicker dtp_next_ST = new DateTimePicker();
        private DateTimePicker dtp_next_ED = new DateTimePicker();
        private DateTimePicker dtp_now_ST = new DateTimePicker();
        private DateTimePicker dtp_now_ED = new DateTimePicker();
        private ComboBox cbb_col3 = new ComboBox();
        private ComboBox cbb_col5 = new ComboBox();
        private Rectangle _rectangle;
        private int dgv_next_col = 0;
        private int dgv_next_row = 0;
        private int dgv_now_col = 0;
        private int dgv_now_row = 0;
        private int spec_ct = 0;//单模产能

        DateTime this_Sun;
        DateTime next_Sun;

        public JiaLiuPlan()
        {
            InitializeComponent();

            SetDtp_Next_ST();
            SetDtp_Next_ED();
            SetDtp_Now_ST();
            SetDtp_Now_ED();

            dgv_next.Controls.Add(cbb_col3);
            cbb_col3.Visible = false;
            cbb_col3.TextChanged += new EventHandler(cbb_col3_TextChange);

            dgv_next.Controls.Add(cbb_col5);
            cbb_col5.Visible = false;
            cbb_col5.TextChanged += new EventHandler(cbb_col5_TextChange);
        }

        private void SetDtp_Next_ST()
        {
            dgv_next.Controls.Add(dtp_next_ST);
            dtp_next_ST.Visible = false;
            dtp_next_ST.Format = DateTimePickerFormat.Custom;
            dtp_next_ST.CustomFormat = "yyyyMMdd";
            dtp_next_ST.TextChanged += new EventHandler(dtp_next_ST_TextChange);
        }

        private void SetDtp_Next_ED()
        {
            dgv_next.Controls.Add(dtp_next_ED);
            dtp_next_ED.Visible = false;
            dtp_next_ED.Format = DateTimePickerFormat.Custom;
            dtp_next_ED.CustomFormat = "yyyyMMdd";
            dtp_next_ED.TextChanged += new EventHandler(dtp_next_ED_TextChange);
        }

        private void SetDtp_Now_ST()
        {
            dgv_now.Controls.Add(dtp_now_ST);
            dtp_now_ST.Visible = false;
            dtp_now_ST.Format = DateTimePickerFormat.Custom;
            dtp_now_ST.CustomFormat = "yyyyMMdd";
            dtp_now_ST.TextChanged += new EventHandler(dtp_now_ST_TextChange);
        }

        private void SetDtp_Now_ED()
        {
            dgv_now.Controls.Add(dtp_now_ED);
            dtp_now_ED.Visible = false;
            dtp_now_ED.Format = DateTimePickerFormat.Custom;
            dtp_now_ED.CustomFormat = "yyyyMMdd";
            dtp_now_ED.TextChanged += new EventHandler(dtp_now_ED_TextChange);
        }

        private void dtp_next_ST_TextChange(object sender, EventArgs e)
        {
            dgv_next.CurrentCell.Value = dtp_next_ST.Text.ToString(); //时间控件选择时间时，就把时间赋给所在的单元格\

            if (dgv_next.CurrentCell.Value != null)
            {
                DateTime starttime = DateTime.ParseExact(dtp_next_ST.Text.ToString(), "yyyyMMdd",
                    System.Globalization.CultureInfo.CurrentCulture);

                if (starttime > next_Sun.AddDays(6) || starttime < next_Sun)
                {
                    MessageBox.Show("选择日期不在范围内", "错误");
                    dgv_next.CurrentCell.Value = null;
                    return;
                }

                string plan_starttime_str = dgv_next.Rows[dgv_next_row].Cells["开始时间"].Value.ToString();
                string plan_endtime_str = dgv_next.Rows[dgv_next_row].Cells["结束时间"].Value.ToString();
                int days = 0;
                if (plan_starttime_str != "" && plan_endtime_str != "")
                {
                    DateTime plan_starttime = DateTime.ParseExact(plan_starttime_str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime plan_endtime = DateTime.ParseExact(plan_endtime_str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    TimeSpan ts = plan_endtime - plan_starttime;
                    days = ts.Days;
                }
                else
                {
                    return;
                }

                string spec = dgv_next.Rows[dgv_next_row].Cells["物料代号"].Value.ToString();

                if (spec != "")
                {
                    GetSpec_CT(spec);//获取单模产能

                    int qtyday = 86400 / (spec_ct + 105);//一天能做的数量
                    int totalqty = 0;
                    if (dgv_next.Rows[dgv_next_row].Cells["状态"].Value.ToString() == "3")
                    {
                        totalqty = qtyday * (days + 1) * 2;//双模
                    }
                    else if (dgv_next.Rows[dgv_next_row].Cells["状态"].Value.ToString() == "0")
                    {
                        totalqty = 0;
                    }
                    else
                    {
                        totalqty = qtyday * (days + 1);//单模
                    }
                    dgv_next.Rows[dgv_next_row].Cells["数量"].Value = totalqty;
                }
            }
        }
        private void dtp_next_ED_TextChange(object sender, EventArgs e)
        {
            dgv_next.CurrentCell.Value = dtp_next_ED.Text.ToString();

            if (dgv_next.CurrentCell.Value != null)
            {
                DateTime endtime = DateTime.ParseExact(dtp_next_ED.Text.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

                if (endtime > next_Sun.AddDays(6) || endtime < next_Sun)
                {
                    MessageBox.Show("选择日期不在范围内","错误");
                    dgv_next.CurrentCell.Value = null;
                    return;
                }

                string plan_starttime_str = dgv_next.Rows[dgv_next_row].Cells["开始时间"].Value.ToString();
                string plan_endtime_str = dgv_next.Rows[dgv_next_row].Cells["结束时间"].Value.ToString();
                int days = 0;
                if (plan_starttime_str != "" && plan_endtime_str != "")
                {
                    DateTime plan_starttime = DateTime.ParseExact(plan_starttime_str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime plan_endtime = DateTime.ParseExact(plan_endtime_str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    TimeSpan ts = plan_endtime - plan_starttime;
                    days = ts.Days;
                }

                string spec = dgv_next.Rows[dgv_next_row].Cells["物料代号"].Value.ToString();

                if (spec != "")
                {
                    GetSpec_CT(spec);//获取单模产能

                    int qtyday = 86400 / (spec_ct + 105);//一天能做的数量
                    int totalqty = 0;
                    if (dgv_next.Rows[dgv_next_row].Cells["状态"].Value.ToString() == "3")
                    {
                        totalqty = qtyday * (days + 1) * 2;//双模
                    }
                    else if(dgv_next.Rows[dgv_next_row].Cells["状态"].Value.ToString() == "0")
                    {
                        totalqty = 0;
                    }
                    else
                    {
                        totalqty = qtyday * (days + 1);//单模
                    }
                    dgv_next.Rows[dgv_next_row].Cells["数量"].Value = totalqty;
                }
            }
        }

        private void dtp_now_ST_TextChange(object sender, EventArgs e)
        {
            dgv_now.CurrentCell.Value = dtp_now_ST.Text.ToString();
        }

        private void dtp_now_ED_TextChange(object sender, EventArgs e)
        {
            dgv_now.CurrentCell.Value = dtp_now_ED.Text.ToString();
        }

        private void cbb_col3_TextChange(object sender, EventArgs e)
        {
            try
            {
                if (dgv_next.Rows[dgv_next_row].Cells[dgv_next_col].Value.ToString() != cbb_col3.Text)
                {
                    dgv_next.Rows[dgv_next_row].Cells[dgv_next_col].Value = cbb_col3.SelectedItem;
                    cbb_col3.Visible = false;
                }
            }
            catch{}
        }
        private void cbb_col5_TextChange(object sender, EventArgs e)
        {
            try
            {
                if (dgv_next.Rows[dgv_next_row].Cells[dgv_next_col].Value.ToString() != cbb_col5.Text)
                {
                    dgv_next.Rows[dgv_next_row].Cells[dgv_next_col].Value = cbb_col5.SelectedItem;
                    cbb_col5.Visible = false;
                }
            }
            catch { }
        }

        private void JiaLiuPlan_Load(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.DimGray;
            pictureBox2.BackColor = Color.AliceBlue;
            SetLabel();
            SetComboBox();
            SetDataTable_now_Main();
            SetDataTable_next_Main();
            dt_big = Load_Big();
            dt_machine = Load_Machine_Spec();
            dt_CT = Load_Spec_CT();
            dt_machine_type = Load_Machine_Type();
            btn_update.Enabled = true;
            cbb_place.Enabled = false;
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            string nextweek = cbb_week.Items[cbb_week.SelectedIndex + 1].ToString();
            string mach = lab_machine.Text.Substring(0, 3);
            if (dgv_next.Rows[0].Cells[2].Value.ToString() != "")
            {
                UpdatePlan(nextweek, mach);
                dt_next = Load_Week_Plan(Convert.ToInt32(nextweek));
                pictureBox1.Refresh();
                GetMachinePlan(mach);
                //dtp_next_ST.Text = null;
                dtp_now_ED.TextChanged -= new EventHandler(dtp_now_ED_TextChange);
                SetDtp_Next_ST();
            }
        }
        private void btn_Plan_Day_Click_1(object sender, EventArgs e)
        {
            string nextweek = cbb_week.Items[cbb_week.SelectedIndex + 1].ToString();
            string mach = lab_machine.Text.Substring(0, 3);

            if (dgv_next.Rows[0].Cells[2].Value.ToString() != "" && mach != "")
            {
                Plan_Day pd = new Plan_Day(nextweek, mach);
                pd.ShowDialog();
            }

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (cbb_place.Text != "")
            {
                SetMachine_PictrueBox(cbb_place.Text,e);
            }
        }
        /// <summary>
        /// 画机台图形
        /// </summary>
        /// <param name="place"></param>
        /// <param name="f"></param>
        private void SetMachine_PictrueBox(string place, PaintEventArgs f)
        {
            Graphics dc = f.Graphics;

            if (place == "1510")
            {
                Pen[] penra = new Pen[90];
                Pen[] penline = new Pen[90];
                Font[] myFont = new Font[90];
                Brush[] bush = new SolidBrush[90];//填充的颜色
                Font[] myFont_big = new Font[90];//大印字
                Brush[] bush_big = new SolidBrush[90];

                int ra_h = 45;//矩形高
                int ra_w = 70;//矩形宽
                int ra_x = 0;//矩形X
                int ra_y = 0;//矩形Y
                int l_x1 = 0;//lineX1
                int l_x2 = 0;//lineX2
                int l_y = 0;//lineY
                int str_x = 0;//机台名称X
                int str_y = 0;//机台名称Y
                int strbig_x = 0;//大印字X
                int strbig_y = 0;//大印字Y

                for (int i = 0; i < 90; i++)
                {

                    myFont[i] = new Font("微软雅黑", 10, FontStyle.Bold);
                    bush[i] = new SolidBrush(Color.SkyBlue);

                    myFont_big[i] = new Font("微软雅黑", 10, FontStyle.Bold);
                    bush_big[i] = new SolidBrush(Color.MintCream);

                    if (i <= 9)
                    {
                        ra_x = ra_w * i + 5 * i;
                        ra_y = ra_h * 0;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i + 1) + 5 * i;
                        l_y = 17;
                        str_x = ra_w / 3 + ra_w * i + i * 5;
                        str_y = 0;
                        strbig_x = ra_w / 6 + ra_w * i + i * 5;
                        strbig_y = 22;
                    }
                    else if (i > 9 && i <= 19)
                    {
                        ra_x = ra_w * (i - 10) + 5 * (i - 10);
                        ra_y = ra_h * 1 + 5 * 1;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 9) + 5 * (i - 10);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 10) + (i - 10) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 6 + ra_w * (i - 10) + (i - 10) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 19 && i <= 29)
                    {
                        ra_x = ra_w * (i - 20) + 5 * (i - 20);
                        ra_y = ra_h * 2 + 5 * 2;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 19) + 5 * (i - 20);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 20) + (i - 20) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 6 + ra_w * (i - 20) + (i - 20) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 29 && i <= 39)
                    {
                        ra_x = ra_w * (i - 30) + 5 * (i - 30);
                        ra_y = ra_h * 3 + 5 * 3;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 29) + 5 * (i - 30);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 30) + (i - 30) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 6 + ra_w * (i - 30) + (i - 30) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 39 && i <= 49)
                    {
                        ra_x = ra_w * (i - 40) + 5 * (i - 40);
                        ra_y = ra_h * 4 + 5 * 4;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 39) + 5 * (i - 40);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 40) + (i - 40) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 6 + ra_w * (i - 40) + (i - 40) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 49 && i <= 59)
                    {
                        ra_x = ra_w * (i - 50) + 5 * (i - 50);
                        ra_y = ra_h * 5 + 5 * 5;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 49) + 5 * (i - 50);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 50) + (i - 50) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 6 + ra_w * (i - 50) + (i - 50) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 59 && i <= 69)//401开始
                    {
                        ra_x = ra_w * (i - 60) + 5 * (i - 60);
                        ra_y = ra_h * 7 + 5 * 7;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 59) + 5 * (i - 60);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 60) + (i - 60) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 6 + ra_w * (i - 60) + (i - 60) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 69 && i <= 79)
                    {
                        ra_x = ra_w * (i - 70) + 5 * (i - 70);
                        ra_y = ra_h * 8 + 5 * 8;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 69) + 5 * (i - 70);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 70) + (i - 70) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 6 + ra_w * (i - 70) + (i - 70) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 79 && i <= 89)
                    {
                        ra_x = ra_w * (i - 80) + 5 * (i - 80);
                        ra_y = ra_h * 9 + 5 * 9;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 79) + 5 * (i - 80);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 80) + (i - 80) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 6 + ra_w * (i - 80) + (i - 80) * 5;
                        strbig_y = ra_y + 22;
                    }

                    string machno = "";
                    if (i > 59)
                    {
                        machno = (401 + i - 60).ToString();
                        dc.DrawString(machno, myFont[i], bush[i], str_x, str_y);
                    }
                    else
                    {
                        machno = (301 + i).ToString();
                        dc.DrawString((301 + i).ToString(), myFont[i], bush[i], str_x, str_y);
                    }
                    //查询该机台是否有排程资料
                    DataRow[] drs_plan = dt_next.Select("MACHINE='" + machno + "'");
                    if (drs_plan.Length > 0)
                    {
                        penra[i] = new Pen(Color.Yellow, 1);
                        penline[i] = new Pen(Color.Yellow, 1);
                    }
                    else
                    {
                        penra[i] = new Pen(Color.White, 1);
                        penline[i] = new Pen(Color.White, 1);
                    }
                    dc.DrawRectangle(penra[i], ra_x, ra_y, ra_w, ra_h);
                    dc.DrawLine(penline[i], l_x1, l_y, l_x2, l_y);

                    string mach = "MachNo='" + machno + "'";
                    DataRow[] drs = dt_machine.Select(mach);
                    string spec = "";
                    if (drs.Length > 0)
                    {
                        spec = drs[0]["LastLprd1"].ToString();
                    }
                    string PRT_NO = "CST_PRT_NO='E" + spec + "'";
                    DataRow[] drs_big = dt_big.Select(PRT_NO);
                    String big = "";
                    if (drs_big.Length > 0)
                    {
                        double intime = Convert.ToDouble(drs[0]["LastIntime"]);
                        if (DateTime.Now.ToOADate() - intime <= 1)
                        {
                            big = drs_big[0]["CST_PAINTLINE_CONTENT"].ToString();
                        }
                    }
                    dc.DrawString(big, myFont_big[i], bush_big[i], strbig_x, strbig_y);
                }
            }
            else
            {
                Pen[] penra = new Pen[104];
                Pen[] penline = new Pen[104];
                Font[] myFont = new Font[104];//机台名称
                Brush[] bush = new SolidBrush[104];//填充的颜色
                Font[] myFont_big = new Font[104];//大印字
                Brush[] bush_big = new SolidBrush[104];

                int ra_h = 45;//矩形高
                int ra_w = 70;//矩形宽
                int ra_x = 0;//矩形X
                int ra_y = 0;//矩形Y
                int l_x1 = 0;//lineX1
                int l_x2 = 0;//lineX2
                int l_y = 0;//lineY
                int str_x = 0;//机台名称X
                int str_y = 0;//机台名称Y
                int strbig_x = 0;//大印字X
                int strbig_y = 0;//大印字Y


                for (int i = 0; i < 104; i++)
                {
                    myFont[i] = new Font("微软雅黑", 10, FontStyle.Bold);
                    bush[i] = new SolidBrush(Color.SkyBlue);

                    myFont_big[i] = new Font("微软雅黑", 10, FontStyle.Bold);
                    bush_big[i] = new SolidBrush(Color.MintCream);

                    if (i <= 9)
                    {
                        ra_x = ra_w * i + 5 * i;
                        ra_y = ra_h * 0;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i + 1) + 5 * i;
                        l_y = 17;
                        str_x = ra_w / 3 + ra_w * i + i * 5;
                        str_y = 0;
                        strbig_x = ra_w / 10 + ra_w * i + i * 5;
                        strbig_y = 22;
                    }
                    else if (i > 9 && i <= 19)
                    {
                        ra_x = ra_w * (i - 10) + 5 * (i - 10);
                        ra_y = ra_h * 1 + 5 * 1;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 9) + 5 * (i - 10);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 10) + (i - 10) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 10) + (i - 10) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 19 && i <= 29)
                    {
                        ra_x = ra_w * (i - 20) + 5 * (i - 20);
                        ra_y = ra_h * 2 + 5 * 2;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 19) + 5 * (i - 20);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 20) + (i - 20) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 20) + (i - 20) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 29 && i <= 39)
                    {
                        ra_x = ra_w * (i - 30) + 5 * (i - 30);
                        ra_y = ra_h * 3 + 5 * 3;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 29) + 5 * (i - 30);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 30) + (i - 30) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 30) + (i - 30) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 39 && i <= 49)
                    {
                        ra_x = ra_w * (i - 40) + 5 * (i - 40);
                        ra_y = ra_h * 4 + 5 * 4;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 39) + 5 * (i - 40);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 40) + (i - 40) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 40) + (i - 40) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 49 && i <= 51)
                    {
                        ra_x = ra_w * (i - 50) + 5 * (i - 50);
                        ra_y = ra_h * 5 + 5 * 5;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 49) + 5 * (i - 50);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 50) + (i - 50) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 50) + (i - 50) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 51 && i <= 61)//201开始
                    {
                        ra_x = ra_w * (i - 52) + 5 * (i - 52);
                        ra_y = ra_h * 7 + 5 * 7;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 51) + 5 * (i - 52);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 52) + (i - 52) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 52) + (i - 52) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 61 && i <= 71)
                    {
                        ra_x = ra_w * (i - 62) + 5 * (i - 62);
                        ra_y = ra_h * 8 + 5 * 8;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 61) + 5 * (i - 62);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 62) + (i - 62) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 62) + (i - 62) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 71 && i <= 81)
                    {
                        ra_x = ra_w * (i - 72) + 5 * (i - 72);
                        ra_y = ra_h * 9 + 5 * 9;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 71) + 5 * (i - 72);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 72) + (i - 72) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 72) + (i - 72) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 81 && i <= 91)
                    {
                        ra_x = ra_w * (i - 82) + 5 * (i - 82);
                        ra_y = ra_h * 10 + 5 * 10;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 81) + 5 * (i - 82);
                        l_y = ra_y + 17;  
                        str_x = ra_w / 3 + ra_w * (i - 82) + (i - 82) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 82) + (i - 82) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 91 && i <= 101)
                    {
                        ra_x = ra_w * (i - 92) + 5 * (i - 92);
                        ra_y = ra_h * 11 + 5 * 11;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 91) + 5 * (i - 92);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 92) + (i - 92) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 92) + (i - 92) * 5;
                        strbig_y = ra_y + 22;
                    }
                    else if (i > 101 && i <= 103)
                    {
                        ra_x = ra_w * (i - 102) + 5 * (i - 102);
                        ra_y = ra_h * 12 + 5 * 12;
                        l_x1 = ra_x;
                        l_x2 = ra_w * (i - 101) + 5 * (i - 102);
                        l_y = ra_y + 17;
                        str_x = ra_w / 3 + ra_w * (i - 102) + (i - 102) * 5;
                        str_y = ra_y;
                        strbig_x = ra_w / 10 + ra_w * (i - 102) + (i - 102) * 5;
                        strbig_y = ra_y + 22;
                    }

                    string machno = "";
                    if (i <= 51)
                    {
                        machno = (101 + i).ToString();
                        dc.DrawString(machno, myFont[i], bush[i], str_x, str_y);
                    }
                    else 
                    {
                        machno = (149 + i).ToString();
                        dc.DrawString((149 + i).ToString(), myFont[i], bush[i], str_x, str_y);
                    }

                    //查询该机台是否有排程资料

                    DataRow[] drs_plan = dt_next.Select("MACHINE='" + machno + "'");
                    if (drs_plan.Length > 0)
                    {
                        penra[i] = new Pen(Color.Yellow, 2);
                        penline[i] = new Pen(Color.Yellow, 2);
                    }
                    else
                    {
                        penra[i] = new Pen(Color.White, 1);
                        penline[i] = new Pen(Color.White, 1);
                    }
                    dc.DrawRectangle(penra[i], ra_x, ra_y, ra_w, ra_h);
                    dc.DrawLine(penline[i], l_x1, l_y, l_x2, l_y);

                    //取当前机台规格
                    string mach = "MachNo='" + machno + "'";
                    DataRow[] drs = dt_machine.Select(mach);
                    string spec = "";
                    string type = "";
                    if (drs.Length > 0)
                    {
                        switch (drs[0]["LastLR"].ToString())
                        {
                            case "1":
                                spec = drs[0]["LastLPrd1"].ToString();
                                type = "左";
                                break;
                            case "2":
                                spec = drs[0]["LastRPrd1"].ToString();
                                type = "右";
                                break;
                            case "3":
                                spec = drs[0]["LastLPrd1"].ToString();
                                type = "双";
                                break;
                        }
                    }
                    string PRT_NO = "CST_PRT_NO='E" + spec + "'";
                    DataRow[] drs_big = dt_big.Select(PRT_NO);
                    string big = "";
                    if (drs_big.Length > 0)
                    {
                        double intime = Convert.ToDouble(drs[0]["LastIntime"]);
                        if (DateTime.Now.ToOADate() - intime <= 1)
                        {
                            big = type + ":" + drs_big[0]["CST_PAINTLINE_CONTENT"].ToString();
                        }
                    }
                    dc.DrawString(big, myFont_big[i], bush_big[i], strbig_x, strbig_y);
                }
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (cbb_week.Text == "") 
                throw new ArgumentException("未选择周数", "cbb_week");
            if (cbb_place.Text == "")
                throw new ArgumentException("未选择车间", "cbb_place");

            lab_now.Text = cbb_week.Text.Substring(4, 2) + "周(" + this_Sun.ToString("MM/dd") 
                           + "-" + this_Sun.AddDays(6).ToString("MM/dd") + ")";
            //lab_now.Text = cbb_week.Text.Substring(0,4) + "年" + cbb_week.Text.Substring(4,2) + "周";
                           
            int c_num = cbb_week.SelectedIndex + 1;

            if (c_num != 5)
            {
                //lab_next.Text = cbb_week.Items[c_num].ToString().Substring(0, 4) + "年" + cbb_week.Items[c_num].ToString().Substring(4, 2) + "周";
                lab_next.Text = cbb_week.Items[c_num].ToString().Substring(4, 2) + "周(" + next_Sun.ToString("MM/dd")
                                + "-" + next_Sun.AddDays(6).ToString("MM/dd") + ")" ;
            }
            else
            {
                lab_next.Text = "";

                DataTable dt = null;

                dgv_next.DataSource = dt;
            }

            string machno = Convert.ToString(GetMachineNum(e.X, e.Y, cbb_place.Text));
            string machytpe = "";
            DataRow[] drs = dt_machine.Select("MachNo='" + machno + "'");
            if (drs.Length > 0) machytpe = drs[0]["TypeName1"].ToString();
            lab_machine.Text = machno + "#" + machytpe ;
            GetMachinePlan(machno);
            //btn_update.Enabled = true;

            //if (Convert.ToInt32(cbb_week.Text) - Convert.ToInt32(cbb_week.Items[2].ToString()) >= 0)
            //{
            //    btn_update.Enabled = true;
            //}
            //else
            //{
            //    btn_update.Enabled = false;
            //}
        }
        private void dgv_next_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //检测是被表示的控件还是DataGridViewTextBoxEditingControl
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                TextBox EditingTB = e.Control as TextBox; // 获取编辑用的文本框的引用
                EditingTB.TextChanged += EditingTB_TextChanged; // 动态注册事件
            }
        }

        private void EditingTB_TextChanged(object sender, EventArgs e)
        {
            String nowstr = (sender as TextBox).Text;
            if (dgv_next_col == 1)
            {
                if (nowstr.Length == 11 && nowstr.Substring(0, 1) == "E")
                {
                    string spec = "CST_PRT_NO='" + nowstr + "'";
                    DataRow[] drs_big = dt_big.Select(spec);
                    if (drs_big.Length > 0)
                    {
                        dgv_next.Rows[dgv_next_row].Cells[dgv_next_col + 1].Value = drs_big[0]["CST_PAINTLINE_CONTENT"].ToString();

                        //查询机台外壳
                        string as400no = drs_big[0]["CST_SPEC_AS400NO"].ToString().Replace("S", "");
                        string name = "CST_TIREVULCANIZATION_NAME ='2061" + as400no + drs_big[0]["CST_SPEC_STAGE"].ToString();
                        name += drs_big[0]["CST_SPEC_AS400VER"].ToString() + lab_machine.Text.Substring(4,3) + "'";
                        string mach_type = "";
                        DataRow[] drs_tire = dt_machine_type.Select(name);
                        if (drs_tire.Length > 0)
                        {
                            if (drs_tire[0]["CST_TIRE_CONTAINERTYPE"].ToString() != "")
                            {
                                mach_type = drs_tire[0]["CST_TIRE_CONTAINERTYPE"].ToString().Substring(0, 2);
                            }
                            dgv_next.Rows[dgv_next_row].Cells[dgv_next_col + 7].Value = mach_type;
                        }
                        else
                        {
                            MessageBox.Show("该规格不适用此机型", "警告");
                        }
                    }
                    
                    //查询制造通知单里该规格是否为限量
                    string spec_p = "ETNO='" + nowstr + "'";
                    DataRow[] drs_mark = dt_production_order.Select(spec_p);
                    string str_ps="";
                    if (drs_mark.Length > 0)
                    {
                        if (drs_mark[0]["SCHEDULE_MARK"].ToString() == "D")
                        {
                            str_ps = "限量:" + drs_mark[0]["SUM(SCHEDULE_LIMITED)OVER(PARTITIONBYETNO)"].ToString();
                        }
                        dgv_next.Rows[dgv_next_row].Cells[dgv_next_col + 8].Value = str_ps;
                    }
                }
                else 
                {
                    dgv_next.Rows[dgv_next_row].Cells[dgv_next_col + 1].Value = "";
                    dgv_next.Rows[dgv_next_row].Cells[dgv_next_col + 7].Value = "";
                    dgv_next.Rows[dgv_next_row].Cells[dgv_next_col + 8].Value = "";

                }
            }
        }

        /// <summary>
        /// 获取点选机台号
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int GetMachineNum(int x, int y,string place)
        {
            int machNum = 0;
            if (place == "1510")
            {
                int h = 45;
                if (y < h)
                {
                    machNum = 301 + x / 75;
                }
                else if (y > h && y <= h * 2)
                {
                    machNum = 311 + x / 75;
                }
                else if (y > (h * 2 + 5 * 1) && y <= (h * 3 + 5 * 1))
                {
                    machNum = 321 + x / 75;
                }
                else if (y > (h * 3 + 5 * 2) && y <= (h * 4 + 5 * 2))
                {
                    machNum = 331 + x / 75;
                }
                else if (y > (h * 4 + 5 * 3) && y <= (h * 5 + 5 * 3))
                {
                    machNum = 341 + x / 75;
                }
                else if (y > (h * 5 + 5 * 4) && y <= (h * 6 + 5 * 4))
                {
                    machNum = 351 + x / 75;
                }
                else if (y > (h * 7 + 5 * 6) && y <= (h * 8 + 5 * 6))
                {
                    machNum = 401 + x / 75;
                }
                else if (y > (h * 8 + 5 * 7) && y <= (h * 9 + 5 * 7))
                {
                    machNum = 411 + x / 75;
                }
                else if (y > (h * 9 + 5 * 8) && y <= (h * 10 + 5 * 8))
                {
                    machNum = 421 + x / 75;
                }
            }
            else
            {
                int h = 45;
                if (y < h)
                {
                    machNum = 101 + x / 75;
                }
                else if (y > h && y <= h * 2)
                {
                    machNum = 111 + x / 75;
                }
                else if (y > (h * 2 + 5 * 1) && y <= (h * 3 + 5 * 1))
                {
                    machNum = 121 + x / 75;
                }
                else if (y > (h * 3 + 5 * 2) && y <= (h * 4 + 5 * 2))
                {
                    machNum = 131 + x / 75;
                }
                else if (y > (h * 4 + 5 * 3) && y <= (h * 5 + 5 * 3))
                {
                    machNum = 141 + x / 75;
                }
                else if (y > (h * 5 + 5 * 4) && y <= (h * 6 + 5 * 4))
                {
                    if (x <= 150)
                        machNum = 151 + x / 75;
                }
                else if (y > (h * 7 + 5 * 6) && y <= (h * 8 + 5 * 6))
                {
                    machNum = 201 + x / 75;
                }
                else if (y > (h * 8 + 5 * 7) && y <= (h * 9 + 5 * 7))
                {
                    machNum = 211 + x / 75;
                }
                else if (y > (h * 9 + 5 * 8) && y <= (h * 10 + 5 * 8))
                {
                    machNum = 221 + x / 75;
                }
                else if (y > (h * 10 + 5 * 9) && y <= (h * 11 + 5 * 9))
                {
                    machNum = 231 + x / 75;
                }
                else if (y > (h * 11 + 5 * 10) && y <= (h * 12 + 5 * 10))
                {
                    machNum = 241 + x / 75;
                }
                else if (y > (h * 12 + 5 * 11) && y <= (h * 13 + 5 * 11))
                {
                    if (x < 150)
                        machNum = 251 + x / 75;
                }
            }
            return machNum;
        }

        /// <summary>
        /// 获取机台排程
        /// </summary>
        /// <param name="mach"></param>
        private void GetMachinePlan(string mach)
        {

            DataRow[] dr_now = dt_now.Select("MACHINE='" + mach + "'");

            DataRow[] dr_next = dt_next.Select("MACHINE='" + mach + "'");

            if (dr_now.Length == 0)
            {
                dt_now_Main.Clear();

                SetDataTable_now_Main();
            }
            else
            {
                for (int i = 0; i < dr_now.Length; i++)
                {
                    dt_now_Main.Rows[i]["物料代号"] = dr_now[i]["ITEM"];
                    dt_now_Main.Rows[i]["大印字"] = dr_now[i]["BIG"];
                    dt_now_Main.Rows[i]["状态"] = dr_now[i]["USE_COUNT"];
                    dt_now_Main.Rows[i]["数量"] = dr_now[i]["QTY"];
                    dt_now_Main.Rows[i]["模式"] = dr_now[i]["MACHINE_WORK"];
                    dt_now_Main.Rows[i]["开始时间"] = dr_now[i]["PLAN_STARTTIME"];
                    dt_now_Main.Rows[i]["结束时间"] = dr_now[i]["PLAN_ENDTIME"];
                    dt_now_Main.Rows[i]["外壳"] = dr_now[i]["MACHINE_MODEL"];
                    dt_now_Main.Rows[i]["备注"] = dr_now[i]["PS"];
                }
            }

            dgv_now.DataSource = dt_now_Main;

            if (dr_next.Length == 0)
            {
                dt_next_Main.Clear();

                SetDataTable_next_Main();
            }
            else
            {
                for (int i = 0; i < dr_next.Length; i++)
                {
                    dt_next_Main.Rows[i]["物料代号"] = dr_next[i]["ITEM"];
                    dt_next_Main.Rows[i]["大印字"] = dr_next[i]["BIG"];
                    dt_next_Main.Rows[i]["状态"] = dr_next[i]["USE_COUNT"];
                    dt_next_Main.Rows[i]["数量"] = dr_next[i]["QTY"];
                    dt_next_Main.Rows[i]["模式"] = dr_next[i]["MACHINE_WORK"];
                    dt_next_Main.Rows[i]["开始时间"] = dr_next[i]["PLAN_STARTTIME"];
                    dt_next_Main.Rows[i]["结束时间"] = dr_next[i]["PLAN_ENDTIME"];
                    dt_next_Main.Rows[i]["外壳"] = dr_next[i]["MACHINE_MODEL"];
                    dt_next_Main.Rows[i]["备注"] = dr_next[i]["PS"];
                }
            }

            dgv_next.DataSource = dt_next_Main;
            SetDataGrid();
        }

        /// <summary>
        /// 设置combobox
        /// </summary>
        private void SetComboBox()
        {
            int now_week = GetWeek(DateTime.Now);

            if (now_week < 3)
            {
                int last_year_max = MaxWeekOfYear(DateTime.Now.AddYears(-1).Year);
                if (now_week == 2)
                {
                    cbb_week.Items.Add(Convert.ToString(DateTime.Now.AddYears(-1).Year) + last_year_max);
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 1).ToString("00")));
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week).ToString("00")));
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week + 1).ToString("00")));
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week + 2).ToString("00")));
                }
                else
                {
                    cbb_week.Items.Add(Convert.ToString(DateTime.Now.AddYears(-1).Year) + (last_year_max - 1));
                    cbb_week.Items.Add(Convert.ToString(DateTime.Now.AddYears(-1).Year) + last_year_max);
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week).ToString("00")));
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week + 1).ToString("00")));
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week + 2).ToString("00")));
                }
            }
            else if (now_week > 50)
            {
                int now_year_max = MaxWeekOfYear(DateTime.Now.Year);
                if (now_week == 51)
                {
                    if (now_year_max > 52)
                    {
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 2).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 1).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week + 1).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week + 2).ToString("00")));
                    }
                    else
                    {
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 2).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 1).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.AddYears(1).Year) + (now_week + 1).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.AddYears(1).Year) + (now_week + 2 - now_year_max).ToString("00")));
                    }
                }
                else if (now_week == 52)
                {
                    if (now_year_max > 52)
                    {
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 2).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 1).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week + 1).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.AddYears(1).Year) + (now_week + 2 - now_year_max).ToString("00")));
                    }
                    else
                    {
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 2).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 1).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.AddYears(1).Year) + (now_week + 1 - now_year_max).ToString("00")));
                        cbb_week.Items.Add((Convert.ToString(DateTime.Now.AddYears(1).Year) + (now_week + 2 - now_year_max).ToString("00")));
                    }
                }
                else if (now_week == 53)
                {
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 2).ToString("00")));
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 1).ToString("00")));
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week).ToString("00")));
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.AddYears(1).Year) + (now_week + 1 - now_year_max).ToString("00")));
                    cbb_week.Items.Add((Convert.ToString(DateTime.Now.AddYears(1).Year) + (now_week + 2 - now_year_max).ToString("00")));
                }
            }
            else
            {
                cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 2).ToString("00")));
                cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week - 1).ToString("00")));
                cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week).ToString("00")));
                cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week + 1).ToString("00")));
                cbb_week.Items.Add((Convert.ToString(DateTime.Now.Year) + (now_week + 2).ToString("00")));
            }

            cbb_place.Items.Add("1508");
            cbb_place.Items.Add("1509");
            cbb_place.Items.Add("1510");
        }
        private void SetLabel()
        {
            lab_day.Text = DateTime.Now.ToString("D");
            lab_day.ForeColor = Color.DarkBlue;
            lab_day.BackColor = Color.AliceBlue;
            lab_Title.Text = "本日周数:" + GetWeek(DateTime.Now);
            lab_Title.ForeColor = Color.Brown;
            lab_Title.BackColor = Color.AliceBlue;
            lab_now.BackColor = Color.AliceBlue;
            lab_next.BackColor = Color.AliceBlue;
            lab_machine.ForeColor = Color.Blue;
            lab_machine.BackColor = Color.AliceBlue;
            label1.BackColor = Color.AliceBlue;
            label2.BackColor = Color.AliceBlue;
            lab_machine.Text = "";
            lab_next.Text = "";
            lab_now.Text = "";
        }
        private void SetDataTable_now_Main()
        {
            if (dt_now_Main.Columns.Count == 0)
            {
                dt_now_Main.Columns.Add("顺序");
                dt_now_Main.Columns.Add("物料代号");
                dt_now_Main.Columns.Add("大印字");
                dt_now_Main.Columns.Add("状态");
                dt_now_Main.Columns.Add("数量");
                dt_now_Main.Columns.Add("模式");
                dt_now_Main.Columns.Add("开始时间");
                dt_now_Main.Columns.Add("结束时间");
                dt_now_Main.Columns.Add("外壳");
                dt_now_Main.Columns.Add("备注");
            }

            for (int i = 1; i < 11; i++)
            {
                DataRow dr = dt_now_Main.NewRow();
                dr["顺序"] = i;
                dt_now_Main.Rows.Add(dr);
            }
        }
        private void SetDataTable_next_Main()
        {
            if (dt_next_Main.Columns.Count == 0)
            {
                dt_next_Main.Columns.Add("顺序");
                dt_next_Main.Columns.Add("物料代号");
                dt_next_Main.Columns.Add("大印字");
                dt_next_Main.Columns.Add("状态");
                dt_next_Main.Columns.Add("数量");
                dt_next_Main.Columns.Add("模式");
                dt_next_Main.Columns.Add("开始时间");
                dt_next_Main.Columns.Add("结束时间");
                dt_next_Main.Columns.Add("外壳");
                dt_next_Main.Columns.Add("备注");
            }

            for (int i = 1; i < 11; i++)
            {
                DataRow dr = dt_next_Main.NewRow();
                dr["顺序"] = i;
                dt_next_Main.Rows.Add(dr);
            }
        }
        private void SetDataGrid()
        {
            dgv_now.Columns[0].Width = 40;
            dgv_now.Columns[1].Width = 90;
            dgv_now.Columns[2].Width = 70;
            dgv_now.Columns[3].Width = 50;
            dgv_now.Columns[4].Width = 50;
            dgv_now.Columns[5].Width = 70;
            dgv_now.Columns[6].Width = 80;
            dgv_now.Columns[7].Width = 80;
            dgv_now.Columns[8].Width = 70;
            dgv_now.Columns[9].Width = 150;
            dgv_now.Columns[0].ReadOnly = true;
            dgv_now.Columns[1].ReadOnly = true;
            dgv_now.Columns[2].ReadOnly = true;
            dgv_now.Columns[3].ReadOnly = true;
            dgv_now.Columns[5].ReadOnly = true;
            dgv_now.Columns[8].ReadOnly = true;
            dgv_now.Columns[9].ReadOnly = true;

            dgv_next.Columns[0].Width = 40;
            dgv_next.Columns[1].Width = 90;
            dgv_next.Columns[2].Width = 70;
            dgv_next.Columns[3].Width = 50;
            dgv_next.Columns[4].Width = 50;
            dgv_next.Columns[5].Width = 70;
            dgv_next.Columns[6].Width = 80;
            dgv_next.Columns[7].Width = 80;
            dgv_next.Columns[8].Width = 70;
            dgv_next.Columns[9].Width = 150;
            dgv_next.Columns[0].ReadOnly = true;
            dgv_next.Columns[2].ReadOnly = true;
            dgv_next.Columns[3].ReadOnly = true;
            dgv_next.Columns[8].ReadOnly = true;
        }

        private void cbb_place_SelectedIndexChanged(object sender, EventArgs e)
        {
            int realnow_Week = GetWeek(DateTime.Now);//当前时间周期
            int select_Week = Convert.ToInt32(cbb_week.Text.Substring(4, 2));//选择的周期
            int diff_Week = realnow_Week - select_Week;
            DateTime datatime_now = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

            switch (diff_Week)
            {
                case 0:
                    this_Sun = GetDateTimeWeekFirstDaySun(datatime_now);
                    next_Sun = this_Sun.AddDays(7);
                    break;
                case 1:
                    this_Sun = GetDateTimeWeekFirstDaySun(datatime_now.AddDays(-7));
                    next_Sun = this_Sun.AddDays(7);
                    break;
                case 2:
                    this_Sun = GetDateTimeWeekFirstDaySun(datatime_now.AddDays(-14));
                    next_Sun = this_Sun.AddDays(7);
                    break;
                case -1:
                    this_Sun = GetDateTimeWeekFirstDaySun(datatime_now.AddDays(7));
                    next_Sun = this_Sun.AddDays(14);
                    break;
                case -2:
                    this_Sun = GetDateTimeWeekFirstDaySun(datatime_now.AddDays(14));
                    next_Sun = this_Sun.AddDays(21);
                    break;
            }
            dt_machine = Load_Machine_Spec();
            pictureBox1.Refresh();
        }

        private void cbb_week_SelectedIndexChanged(object sender, EventArgs e)
        {
            DBName db = new DBName();
            int week = Convert.ToInt32(cbb_week.Text);
            int nextweek = 0;
            int maxweek = 0;

            dt_now= Load_Week_Plan(week);


            if (week.ToString().Substring(4, 2) == "52" || week.ToString().Substring(4, 2).ToString() == "53")
            {
                if (maxweek == 52)
                {
                    nextweek = week + 49;
                }
                else
                {
                    nextweek = week + 48;
                }
            }
            else
            {
                nextweek = week + 1;
            }

            dt_next = Load_Week_Plan(nextweek);

            int now_week =Convert.ToInt32(DateTime.Now.Year.ToString() + GetWeek(DateTime.Now).ToString());

            dt_production_order =Load_Production_Order(nextweek);//获取制造通知单

            cbb_place.Enabled = true;
        }

        private void dgv_next_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgv_next_col = e.ColumnIndex;
            dgv_next_row = e.RowIndex;



            if (dgv_next_col == 6 && dgv_next_row > -1)
            {
                _rectangle = dgv_next.GetCellDisplayRectangle(dgv_next_col, dgv_next_row, true);
                dtp_next_ST.Size = new Size(_rectangle.Width, _rectangle.Height);
                dtp_next_ST.Location = new Point(_rectangle.X, _rectangle.Y);
                dtp_next_ST.Visible = true;
                dtp_next_ST.Value = next_Sun;
            }
            else if(dgv_next_col == 7 && dgv_next_row > -1)
            {
                _rectangle = dgv_next.GetCellDisplayRectangle(dgv_next_col, dgv_next_row, true);
                dtp_next_ED.Size = new Size(_rectangle.Width, _rectangle.Height);
                dtp_next_ED.Location = new Point(_rectangle.X, _rectangle.Y);
                dtp_next_ED.Visible = true;
                dtp_next_ED.Value = next_Sun;
            }
            else if (dgv_next_col == 3 && dgv_next_row > -1)
            {
                _rectangle = dgv_next.GetCellDisplayRectangle(dgv_next_col, dgv_next_row, true);
                cbb_col3.Size = new Size(_rectangle.Width, _rectangle.Height);
                cbb_col3.Location = new Point(_rectangle.X, _rectangle.Y);
                cbb_col3.Items.Clear();
                if (cbb_place.Text == "1510")
                {
                    cbb_col3.Items.Add("");
                    cbb_col3.Items.Add("0");
                    cbb_col3.Items.Add("1");
                    cbb_col3.Text = dgv_next.Rows[dgv_next_row].Cells[dgv_next_col].Value.ToString();
                }
                else
                {
                    cbb_col3.Items.Add("");
                    cbb_col3.Items.Add("0");
                    cbb_col3.Items.Add("1");
                    cbb_col3.Items.Add("2");
                    cbb_col3.Items.Add("3");
                    cbb_col3.Text = dgv_next.Rows[dgv_next_row].Cells[dgv_next_col].Value.ToString();
                }

                cbb_col3.Visible = true;
            }
            else if (dgv_next_col == 5 && dgv_next_row > -1)
            {
                _rectangle = dgv_next.GetCellDisplayRectangle(dgv_next_col, dgv_next_row, true);
                cbb_col5.Size = new Size(_rectangle.Width, _rectangle.Height);
                cbb_col5.Location = new Point(_rectangle.X, _rectangle.Y);
                cbb_col5.Items.Clear();
                cbb_col5.Items.Add("");
                cbb_col5.Items.Add("试作");
                cbb_col5.Items.Add("清纱");
                cbb_col5.Items.Add("调台");
                cbb_col5.Items.Add("验模");
                cbb_col5.Text = dgv_next.Rows[dgv_next_row].Cells[dgv_next_col].Value.ToString();
                cbb_col5.Visible = true;
            }
            else
            {
                cbb_col3.Visible = false;
                cbb_col5.Visible = false;
                dtp_next_ST.Visible = false;
                dtp_next_ED.Visible = false;
            }
        }

        private void dgv_next_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            dtp_next_ST.Visible = false;
            dtp_next_ED.Visible = false;
            cbb_col3.Visible = false;
            cbb_col5.Visible = false;
        }

        private void dgv_next_Scroll(object sender, ScrollEventArgs e)
        {
            dtp_next_ST.Visible = false;
            dtp_next_ED.Visible = false;
            cbb_col3.Visible = false;
            cbb_col5.Visible = false;
        }

        private void dgv_now_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgv_now_col = e.ColumnIndex;
            dgv_now_row = e.RowIndex;
            if (dgv_now_col == 6 && dgv_now_row > -1)
            {
                _rectangle = dgv_now.GetCellDisplayRectangle(dgv_now_col, dgv_now_row, true);
                dtp_now_ST.Size = new Size(_rectangle.Width, _rectangle.Height);
                dtp_now_ST.Location = new Point(_rectangle.X, _rectangle.Y);
                dtp_now_ST.Visible = true;
            }
            else if (dgv_now_col == 7 && dgv_now_row > -1)
            {
                _rectangle = dgv_now.GetCellDisplayRectangle(dgv_now_col, dgv_now_row, true);
                dtp_now_ED.Size = new Size(_rectangle.Width, _rectangle.Height);
                dtp_now_ED.Location = new Point(_rectangle.X, _rectangle.Y);
                dtp_now_ED.Visible = true;
            }
            else
            {
                dtp_now_ST.Visible = false;
                dtp_now_ED.Visible = false;
            }
        }

        private void dgv_now_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            dtp_now_ST.Visible = false;
            dtp_now_ED.Visible = false;
        }

        private void dgv_now_Scroll(object sender, ScrollEventArgs e)
        {
            dtp_now_ST.Visible = false;
            dtp_now_ED.Visible = false;
        }
        /// <summary>
        /// 上传排程资料
        /// </summary>
        /// <param name="week"></param>
        /// <param name="machine"></param>
        private void UpdatePlan(string week,string machine)
        {
            DBName db = new DBName();
            Logger log = LogManager.GetCurrentClassLogger();
            string plan_id = "";//排程id(关联JiaLiu_Plan_Day)

            SqlParameter[] paras_del_week = new SqlParameter[]
            {
                new SqlParameter("@MACHINE",machine),
                new SqlParameter("@WEEK",week)
            };

            string sqlstr_week = "delete [CSTCKMACH].[dbo].[JiaLiu_Plan_Week] where PLAN_WEEK=" + "@WEEK" + " and MACHINE=" + "@MACHINE" ;

            ConnDB.ExcuteSQL(sqlstr_week, db.DB_CSTS5, paras_del_week);

            if (cbb_place.Text == "1510")
            {
                plan_id = "CR2" + machine + week;
            }
            else
            {
                plan_id = "CR1" + machine + week;
            }

            SqlParameter[] paras_del_day = new SqlParameter[]
            {
                new SqlParameter("@PLAN_ID",plan_id),
                new SqlParameter("@MACHINE",machine)
            };

            string sqlstr_day = "delete [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] where PLAN_ID=" + "@PLAN_ID" + " and MACHINE=" + "@MACHINE";

            ConnDB.ExcuteSQL(sqlstr_day, db.DB_CSTS5, paras_del_day);

            string spec_no = "";
            string item = "";//物料代号
            string big = "";//大印字
            string prd_phase = "";//阶段
            string spec_version = "";//版次
            string use_count = "";//模数
            string qty = "";//数量
            string machine_work = "";
            string machine_type = "";//适用机型
            string machine_model = "";//外壳
            string machine_ct = "";//单模产能
            string schedule_mark = "";//排程符号------还没值
            string ps = "";//备注
            string sequence = "";//顺序
            string plan_type = "";//排程类型
            string plan_starttime = "";//开始时间
            string plan_endtime = "";//结束时间
            string createdtime = "";
            int error = 0;//错误数量
            int model_num = 0;//机台开启模数

            for (int i = 0; i < dgv_next.Rows.Count; i++)
            {
                item = dgv_next.Rows[i].Cells[1].Value.ToString();
                if (item != "")
                {
                    use_count = dgv_next.Rows[i].Cells[3].Value.ToString();
                    qty = dgv_next.Rows[i].Cells[4].Value.ToString();
                    machine_work = dgv_next.Rows[i].Cells[5].Value.ToString();
                    ps = dgv_next.Rows[i].Cells[9].Value.ToString();
                    sequence = dgv_next.Rows[i].Cells[0].Value.ToString();
                    plan_type = "Main";
                    plan_starttime= dgv_next.Rows[i].Cells[6].Value.ToString();
                    plan_endtime = dgv_next.Rows[i].Cells[7].Value.ToString();
                    machine_model = dgv_next.Rows[i].Cells[8].Value.ToString();
                    GetSpec_CT(item);
                    machine_ct = (spec_ct).ToString();
                    createdtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    if (cbb_place.Text == "1510")
                    {
                        if (use_count == "1")
                        {
                            model_num = 1;
                        }
                        else
                        {
                            model_num = 0;
                        }
                    }
                    else
                    {
                        if (use_count == "1" || use_count == "2")
                        {
                            model_num = 1;
                        }
                        else if (use_count == "3")
                        {
                            model_num = 2;
                        }
                        else
                        {
                            model_num = 0;
                        }
                    }

                    string prt_no = "CST_PRT_NO='" + item + "'";
                    DataRow[] drs = dt_big.Select(prt_no);
                    if (drs.Length > 0)
                    {
                        spec_no = drs[0]["CST_SPEC_AS400NO"].ToString();
                        big = drs[0]["CST_PAINTLINE_CONTENT"].ToString();
                        prd_phase = drs[0]["CST_SPEC_STAGE"].ToString();
                        spec_version = drs[0]["CST_SPEC_AS400VER"].ToString();

                        SqlParameter[] paras_plan = new SqlParameter[]
                        {
                            new SqlParameter("@MACHINE", machine),
                            new SqlParameter("@SPEC_NO", spec_no),
                            new SqlParameter("@ITEM", item),
                            new SqlParameter("@BIG", big),
                            new SqlParameter("@PRD_PHASE", prd_phase),
                            new SqlParameter("@SPEC_VERSION", spec_version),
                            new SqlParameter("@USE_COUNT", use_count),
                            new SqlParameter("@QTY", qty),
                            new SqlParameter("@MACHINE_WORK", machine_work),
                            new SqlParameter("@MACHINE_TYPE", machine_type),
                            new SqlParameter("@MACHINE_MODEL", machine_model),
                            new SqlParameter("@MACHINE_CT", machine_ct),
                            new SqlParameter("@SCHEDULE_MARK", schedule_mark),
                            new SqlParameter("@PS", ps),
                            new SqlParameter("@SEQUENCE", sequence),
                            new SqlParameter("@PLAN_TYPE", plan_type),
                            new SqlParameter("@PLAN_WEEK", week),
                            new SqlParameter("@PLAN_STARTTIME", plan_starttime),
                            new SqlParameter("@PLAN_ENDTIME", plan_endtime),
                            new SqlParameter("@PLAN_ID", plan_id),
                            new SqlParameter("@CREATEDTIME", createdtime)
                        };

                        StringBuilder sqlsb = new StringBuilder(500);
                        sqlsb.Append("Insert into [CSTCKMACH].[dbo].[JiaLiu_Plan_Week] (MACHINE, SPEC_NO, ITEM, BIG, PRD_PHASE, SPEC_VERSION, ");
                        sqlsb.Append("USE_COUNT, QTY, MACHINE_WORK, MACHINE_TYPE, MACHINE_MODEL, MACHINE_CT, SCHEDULE_MARK, PS, SEQUENCE, ");
                        sqlsb.Append("PLAN_TYPE, PLAN_WEEK, PLAN_STARTTIME, PLAN_ENDTIME, PLAN_ID, CREATEDTIME) ");
                        sqlsb.Append("values (" + "@MACHINE" + "," + "@SPEC_NO" + "," + "@ITEM" + "," + "@BIG" + "," + "@PRD_PHASE" + ",");
                        sqlsb.Append("@SPEC_VERSION" + "," + "@USE_COUNT" + "," + "@QTY" + "," + "@MACHINE_WORK" + "," + "@MACHINE_TYPE" + ",");
                        sqlsb.Append("@MACHINE_MODEL" + "," + "@MACHINE_CT" + "," + "@SCHEDULE_MARK" + "," + "@PS" + "," + "@SEQUENCE" + ",");
                        sqlsb.Append("@PLAN_TYPE" + "," + "@PLAN_WEEK" + "," + "@PLAN_STARTTIME" + "," + "@PLAN_ENDTIME" + "," + "@PLAN_ID" + ",");
                        sqlsb.Append("@CREATEDTIME" + ")");

                        ConnDB.ExcuteSQL(sqlsb, db.DB_CSTS5, paras_plan);

                        //将起讫时间分天记录plan_day
                        string qty_day = "";
                        DateTime st_time = DateTime.Now;
                        string plan_day="";

                        int now_week = Convert.ToInt32(DateTime.Now.Year.ToString() + GetWeek(DateTime.Now).ToString());
                        int diff_week = Convert.ToInt32(week) - now_week;

                        if (diff_week == 1)
                        {
                            st_time = GetDateTimeWeekFirstDaySun(DateTime.Now.AddDays(7));
                        }
                        else if (diff_week == 2)
                        {
                            st_time = GetDateTimeWeekFirstDaySun(DateTime.Now.AddDays(14));
                        }

                        //开始时间 结束时间
                        if (qty == "99999" && plan_endtime == "") //常驻规格
                        {
                            qty_day = (86400 / (Convert.ToInt32(machine_ct) + 105)).ToString();
                            for (int j = 0; j < 7; j++)
                            {
                                DateTime starttime = DateTime.ParseExact(plan_starttime, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                                plan_day = starttime.AddDays(j).ToString("yyyyMMdd");
                                SqlParameter[] paras_plan_day = new SqlParameter[]
                                {
                                    new SqlParameter("@PLAN_ID", plan_id),
                                    new SqlParameter("@MACHINE", machine),
                                    new SqlParameter("@ITEM", item),
                                    new SqlParameter("@QTY", qty_day),
                                    new SqlParameter("@PLAN_TIME", plan_day),
                                    new SqlParameter("@CREATEDTIME", createdtime)
                                };

                                StringBuilder sqlsb_day = new StringBuilder(100);
                                sqlsb_day.Append("Insert into [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] (PLAN_ID, MACHINE, ITEM, QTY, PLAN_TIME, CREATEDTIME) ");
                                sqlsb_day.Append("values (" + "@PLAN_ID" + "," + "@MACHINE" + "," + "@ITEM" + "," + "@QTY" + "," + "@PLAN_TIME" + ",");
                                sqlsb_day.Append("@CREATEDTIME" + ")");

                                ConnDB.ExcuteSQL(sqlsb_day, db.DB_CSTS5, paras_plan_day);
                            }
                        }
                        else
                        {
                            DateTime starttime = DateTime.ParseExact(plan_starttime, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                            DateTime endtime = DateTime.ParseExact(plan_endtime, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                            TimeSpan ts = endtime - starttime;
                            int days = ts.Days;
                            days += 1 ;
                            qty_day = (Convert.ToInt32(qty) / days).ToString() ;

                            for (int j = 0; j < days; j++)
                            {
                                plan_day = starttime.AddDays(j).ToString("yyyyMMdd");
                                SqlParameter[] paras_plan_day = new SqlParameter[]
                                {
                                    new SqlParameter("@PLAN_ID", plan_id),
                                    new SqlParameter("@MACHINE", machine),
                                    new SqlParameter("@ITEM", item),
                                    new SqlParameter("@QTY", qty_day),
                                    new SqlParameter("@PLAN_TIME", plan_day),
                                    new SqlParameter("@CREATEDTIME", createdtime)
                                };

                                StringBuilder sqlsb_day = new StringBuilder(100);
                                sqlsb_day.Append("Insert into [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] (PLAN_ID, MACHINE, ITEM, QTY, PLAN_TIME, CREATEDTIME) ");
                                sqlsb_day.Append("values (" + "@PLAN_ID" + "," + "@MACHINE" + "," + "@ITEM" + "," + "@QTY" + "," + "@PLAN_TIME" + ",");
                                sqlsb_day.Append("@CREATEDTIME" + ")");

                                ConnDB.ExcuteSQL(sqlsb_day, db.DB_CSTS5, paras_plan_day);
                            }
                        }

                        //if (qty == "99999" && plan_endtime == "")//常驻规格
                        //{
                        //    //一周七日 每日的产能数量
                        //    for (int j = 0; j < 7; j++)
                        //    {
                        //        plan_day = st_time.AddDays(j).ToString("yyyyMMdd");
                        //        SqlParameter[] paras_plan_day = new SqlParameter[]
                        //        {
                        //            new SqlParameter("@PLAN_ID", plan_id),
                        //            new SqlParameter("@MACHINE", machine),
                        //            new SqlParameter("@ITEM", item),
                        //            new SqlParameter("@QTY", qty_day),
                        //            new SqlParameter("@PLAN_TIME", plan_day),
                        //            new SqlParameter("@CREATEDTIME", createdtime)
                        //        };

                        //        StringBuilder sqlsb_day = new StringBuilder(100);
                        //        sqlsb_day.Append("Insert into [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] (PLAN_ID, MACHINE, ITEM, QTY, PLAN_TIME, CREATEDTIME) ");
                        //        sqlsb_day.Append("values (" + "@PLAN_ID" + "," + "@MACHINE" + "," + "@ITEM" + "," + "@QTY" + "," + "@PLAN_TIME" + ",");
                        //        sqlsb_day.Append("@CREATEDTIME" + ")");

                        //        ConnDB.ExcuteSQL(sqlsb_day, db.DB_CSTS5, paras_plan_day);
                        //    }
                        //}
                        //else
                        //{
                        //    double time_total;
                        //    int time_day;
                        //    int time_hour;
                        //    //计算总花费时间(单模产能*数量/单模 or 双模) 秒数转成天数
                        //    if (model_num == 1)
                        //    {
                        //        time_total = Convert.ToDouble(spec_ct) * Convert.ToDouble(qty) / (double)86400;
                        //    }
                        //    else if (model_num == 2)
                        //    {
                        //        time_total = Convert.ToDouble(spec_ct) * Convert.ToDouble(qty) / (double)2 / (double)86400 ;
                        //    }
                        //    else
                        //    {
                        //        time_total = 0;
                        //    }

                        //    //分为天数与小时进行计算
                        //    if (time_total < 1)
                        //    {
                        //        time_day = 0;
                        //        time_hour = Convert.ToInt32(time_total * 24);
                        //    }
                        //    else
                        //    {
                        //        string[] str = time_total.ToString().Split('.');
                        //        time_hour = Convert.ToInt32(Convert.ToDouble("0." + str[1]) * 24);
                        //        time_day = Convert.ToInt32(str[0]);
                        //    }

                        //    DateTime last_plan_endtime;
                        //    //判断顺序后根据排产数分配各天数量
                        //    if (sequence == "1")//顺序为1的话开始时间为周日8点
                        //    {
                        //        last_plan_endtime = Convert.ToDateTime(st_time.ToString("yyyy/MM/dd") + " 08:00:00");

                        //        if (time_day < 1)//排产量是否超过一天
                        //        {
                        //            plan_day = last_plan_endtime.ToString("yyyyMMdd");

                        //            //排产的量不到一天的单模产能的话,直接使用排产的数量qty
                        //            SqlParameter[] paras_plan_day = new SqlParameter[]
                        //            {
                        //                new SqlParameter("@PLAN_ID", plan_id),
                        //                new SqlParameter("@MACHINE", machine),
                        //                new SqlParameter("@ITEM", item),
                        //                new SqlParameter("@QTY", qty),
                        //                new SqlParameter("@PLAN_TIME", plan_day),
                        //                new SqlParameter("@CREATEDTIME", createdtime)
                        //            };

                        //            StringBuilder sqlsb_day = new StringBuilder(100);
                        //            sqlsb_day.Append("Insert into [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] (PLAN_ID, MACHINE, ITEM, QTY, PLAN_TIME, CREATEDTIME) ");
                        //            sqlsb_day.Append("values (" + "@PLAN_ID" + "," + "@MACHINE" + "," + "@ITEM" + "," + "@QTY" + "," + "@PLAN_TIME" + ",");
                        //            sqlsb_day.Append("@CREATEDTIME" + ")");

                        //            ConnDB.ExcuteSQL(sqlsb_day, db.DB_CSTS5, paras_plan_day);
                        //        }
                        //        else
                        //        {
                        //            for (int j = 0; j <= time_day; j++)
                        //            {
                        //                string plan_day_qty="";

                        //                if (j == time_day)
                        //                {
                        //                    //最后一天剩余的量(排产量 - 一天的单模产能 * 排产天数)
                        //                    plan_day_qty = (Convert.ToInt32(qty) - (Convert.ToInt32(qty_day) * time_day * model_num)).ToString();
                        //                }
                        //                else
                        //                {
                        //                    plan_day_qty =Convert.ToString(Convert.ToInt16(qty_day) * model_num);//一天的单模产能
                        //                }

                        //                plan_day = last_plan_endtime.AddDays(j).ToString("yyyyMMdd");

                        //                SqlParameter[] paras_plan_day = new SqlParameter[]
                        //                {
                        //                    new SqlParameter("@PLAN_ID", plan_id),
                        //                    new SqlParameter("@MACHINE", machine),
                        //                    new SqlParameter("@ITEM", item),
                        //                    new SqlParameter("@QTY", plan_day_qty),
                        //                    new SqlParameter("@PLAN_TIME", plan_day),
                        //                    new SqlParameter("@CREATEDTIME", createdtime)
                        //                };

                        //                StringBuilder sqlsb_day = new StringBuilder(100);
                        //                sqlsb_day.Append("Insert into [CSTCKMACH].[dbo].[JiaLiu_Plan_Day] (PLAN_ID, MACHINE, ITEM, QTY, PLAN_TIME, CREATEDTIME) ");
                        //                sqlsb_day.Append("values (" + "@PLAN_ID" + "," + "@MACHINE" + "," + "@ITEM" + "," + "@QTY" + "," + "@PLAN_TIME" + ",");
                        //                sqlsb_day.Append("@CREATEDTIME" + ")");

                        //                ConnDB.ExcuteSQL(sqlsb_day, db.DB_CSTS5, paras_plan_day);
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //获取上一个顺序的换模时间
                        //        string last_plan_time_str = dgv_next.Rows[i - 1].Cells[6].Value.ToString();
                        //        last_plan_endtime = DateTime.ParseExact(last_plan_time_str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);

                        //        //已早上8点区分,8点前分在昨天
                        //        if (last_plan_endtime.Hour < 8)
                        //        {
                        //            if (time_day > 0)
                        //            {

                        //            }
                        //            else
                        //            {
                                        
                        //            }
                        //        }
                        //        else
                        //        {
                                    
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        log.Error("PLM_MES_TIRESPEC未找到相应物料代号:" + item);
                        error += 1;
                    }
                }
            }
            MessageBox.Show(machine + "# 上传成功   失败笔数:" + error, "执行结果");
        }

        /// <summary>
        /// 获取规格明细 版本 
        /// </summary>
        /// <returns></returns>
        private static DataTable Load_Big()
        {
            DataTable dt = new DataTable();
            DBName db = new DBName();

            StringBuilder sqlsb = new StringBuilder(400);
            sqlsb.Append("select CST_SPEC_AS400NO,a.CST_PRT_NO,a.CST_PAINTLINE_CONTENT,a.CST_SPEC_AS400VER,b.CST_PRT_DESC,a.CST_SPEC_STAGE ");
            sqlsb.Append("FROM S_PLM_MES.PLM_MES_TIRE_PAINTLINE a inner join (select CST_PRT_ETNO,CST_PRT_DESC,MAX(CST_PRT_EFFECTIVEDATE) as max_date ");
            sqlsb.Append("FROM S_PLM_MES.PLM_MES_TIRESPEC GROUP BY CST_PRT_ETNO,CST_PRT_DESC) b on ");
            sqlsb.Append("a.CST_PRT_NO=b.CST_PRT_ETNO order by a.CST_PAINTLINE_CONTENT");
            dt = ConnDB.QueryDB(sqlsb, db.DB_Oracle);
            return dt;
        }

        /// <summary>
        /// 当前各机台规格
        /// </summary>
        /// <returns></returns>
        private static DataTable Load_Machine_Spec()
        {
            DataTable dt = new DataTable();
            DBName db = new DBName();

            StringBuilder sqlsb = new StringBuilder(100);
            sqlsb.Append("select a.MachNo,a.LastLR,a.LastLprd1,a.LastRprd1,a.LastIntime,b.TypeName1,b.TypeName2 ");
            sqlsb.Append("FROM [CSTCKMACH].[dbo].[PPSet] as a left join [CSTCKMACH].[dbo].[JiaLiu_MachineType] as b on a.MachNo=b.Machine ");
            sqlsb.Append("where a.MachNo <>'J1' order by a.MachNo ");
            dt = ConnDB.QueryDB(sqlsb, db.DB_CSTS5);
            return dt;
        }

        /// <summary>
        /// 获取各规格单模产能
        /// </summary>
        /// <returns></returns>
        private static DataTable Load_Spec_CT()
        {
            DataTable dt = new DataTable();
            DBName db = new DBName();

            StringBuilder sqlsb = new StringBuilder(50);
            sqlsb.Append("select NAME,CST_TIREVULCANIZATION_SUMMER FROM S_PLM_MES.PLM_MES_TIREVULCANIZATIONEI WHERE CST_TIREVULCANIZATION_IP=305");
            dt = ConnDB.QueryDB(sqlsb, db.DB_Oracle);
            return dt;
        }

        /// <summary>
        /// 查询单模产能
        /// </summary>
        /// <param name="spec"></param>
        private void GetSpec_CT(string spec)
        {
            string prt_no = "CST_PRT_NO='" + spec + "'";
            DataRow[] drs_big = dt_big.Select(prt_no);
            string name;
            string as400no;
            spec_ct = 0;
            if (drs_big.Length > 0)
            {
                as400no = drs_big[0]["CST_SPEC_AS400NO"].ToString().Replace("S", "");
                name = "NAME like '%2061" + as400no + drs_big[0]["CST_SPEC_STAGE"].ToString() + drs_big[0]["CST_SPEC_AS400VER"].ToString() + "%'";
                DataRow[] drs_ct = dt_CT.Select(name);
                if (drs_ct.Length > 0)
                {
                    spec_ct = Convert.ToInt32(drs_ct[0]["CST_TIREVULCANIZATION_SUMMER"].ToString());
                }
            }
        }

        /// <summary>
        /// 获取制造通知单
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        private static DataTable Load_Production_Order(int week)
        {
            DBName db = new DBName();
            DataTable dt = new DataTable();
            DateTime st_time = DateTime.Now;
            DateTime end_time = DateTime.Now;

            int now_week = Convert.ToInt32(DateTime.Now.Year.ToString() + GetWeek(DateTime.Now).ToString());
            int diff_week = week - now_week;

            if (diff_week == 1)
            {
                st_time = GetDateTimeWeekFirstDaySun(DateTime.Now.AddDays(7));
                end_time = st_time.AddDays(6);
            }
            else if (diff_week == 2)
            {
                st_time = GetDateTimeWeekFirstDaySun(DateTime.Now.AddDays(14));
                end_time = st_time.AddDays(6);
            }

            StringBuilder sqlsb = new StringBuilder();
            sqlsb.Append("select distinct(ETNO), SCHEDULE_MODULUS, sum(SCHEDULE_AMOUNT)over(PARTITION by ETNO),");
            sqlsb.Append("SCHEDULE_MARK, sum(SCHEDULE_LIMITED)over(PARTITION by ETNO), BIGPRINT, VUL_MODEL_DESC ");
            sqlsb.Append("FROM S_MES.OFFLINE_PP_MANUFACTURING where mark='Y' and SCHEDULE_DATE between '" + st_time.ToString("yyyyMMdd") + "' and '");
            sqlsb.Append(end_time.ToString("yyyyMMdd") + "' order by ETNO");
            dt = ConnDB.QueryDB(sqlsb, db.DB_Oracle);

            return dt;
        }

        /// <summary>
        /// 获取机台类型与外壳
        /// </summary>
        /// <returns></returns>
        private static DataTable Load_Machine_Type()
        {
            DBName db = new DBName();
            DataTable dt = new DataTable();

            StringBuilder sqlsb = new StringBuilder();
            sqlsb.Append("select CST_SPEC_AS400NO, CST_SPEC_STAGE, CST_SPEC_AS400VER, CST_PRT_PRODUCTIONPLANT, CST_EQPMODEL, CST_PRT_MOULDNAME,");
            sqlsb.Append("CST_TIREVULCANIZATION_NAME, CST_TIRE_CONTAINERTYPE, CST_EQPDESC FROM S_PLM_ERP.CST_VULCANIZATIONINFO ");
            dt = ConnDB.QueryDB(sqlsb, db.DB_Oracle);

            return dt;
        }

        private static DataTable Load_Week_Plan(int week)
        {
            DBName db = new DBName();
            DataTable dt = new DataTable();

            StringBuilder strsb = new StringBuilder(300);
            strsb.Append("SELECT MACHINE, SEQUENCE, ITEM, BIG, PRD_PHASE, USE_COUNT, QTY, MACHINE_WORK,");
            strsb.Append("MACHINE_TYPE, MACHINE_MODEL, SCHEDULE_MARK, PS, PLAN_TYPE, PLAN_STARTTIME, PLAN_ENDTIME ");
            strsb.Append("FROM [CSTCKMACH].[dbo].[JiaLiu_Plan_Week] where PLAN_WEEK='" + week + "' ");
            strsb.Append("order by SEQUENCE ");

            dt = ConnDB.QueryDB(strsb, db.DB_CSTS5);

            return dt;
        }


        private void dgv_next_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int x = e.RowIndex;
            int y = e.ColumnIndex;

            //if (e.ColumnIndex == 4)
            //{
            //    GetPlan_Endtime(x,y);
            //}
        }

        /// <summary>
        /// 计算换模时间
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        private void GetPlan_Endtime(int X,int Y)
        {
            string big = dgv_next.Rows[X].Cells[2].Value.ToString();
            string qty = dgv_next.Rows[X].Cells[4].Value.ToString();
            string spec = dgv_next.Rows[X].Cells[1].Value.ToString();
            string num = dgv_next.Rows[X].Cells[3].Value.ToString();
            int qty_total = 0;
            double time_total = 0;
            int time_day = 0;
            int time_hour = 0;
            string daystr = "";

            if (Y == 4 && big != "" && qty == "99999")
            {
                dgv_next.Rows[X].Cells[6].Value = "";//换模时间为空
            }
            else if(Y == 4 && big != "" && qty != "")
            {
                GetSpec_CT(spec);//获取单模产能
                if (spec_ct != 0)
                {
                    switch (num)
                    {
                        case "1":
                            qty_total = Convert.ToInt32(qty) * (spec_ct + 105);
                            break;
                        case "2":
                            qty_total = Convert.ToInt32(qty) * (spec_ct + 105);
                            break;
                        case "3":
                            qty_total = Convert.ToInt32(qty) * (spec_ct + 105) / 2;
                            break;
                        case "0":
                            qty_total = 0;
                            break;
                    }

                    if (qty_total != 0)
                    {
                        time_total = (double)qty_total / (double)86400;
                        time_total = Math.Round(time_total, 9);
                        if (time_total < 1)
                        {
                            time_hour = Convert.ToInt32(time_total * 24);
                        }
                        else
                        {
                            string[] str = time_total.ToString().Split('.');
                            time_hour = Convert.ToInt32(Convert.ToDouble("0." + str[1]) * 24);
                            time_day = Convert.ToInt32(str[0]);
                        }
                    }
                }
               
                DateTime sun = GetDateTimeWeekFirstDaySun(DateTime.Now.AddDays(7));//获取周日
                if (X == 0)
                {
                    //第一笔的话周日八点开始计算
                    time_hour += 8;
                    if (time_hour > 24)
                    {
                        time_hour -= 24;
                        time_day += 1;
                    }
                    else if (time_hour == 24)
                    {
                        time_hour = 0;
                        time_day += 1;
                    }
                    sun = sun.AddDays(time_day);
                    daystr = sun.ToString("yyyyMMdd") + time_hour.ToString("00");
                }
                else
                {
                    string before_time_str =dgv_next.Rows[X - 1].Cells[6].Value.ToString();

                    if (before_time_str == "")
                    {
                        //如果上一笔排程换模时间为空则先从周日八点开始计算
                        time_hour += 8;
                        if (time_hour > 24)
                        {
                            time_hour -= 24;
                            time_day += 1;
                        }
                        else if (time_hour == 24)
                        {
                            time_hour = 0;
                            time_day += 1;
                        }
                        sun = sun.AddDays(time_day);
                        daystr = sun.ToString("yyyyMMdd") + time_hour.ToString("00");
                    }
                    else
                    {
                        DateTime before_time = DateTime.ParseExact(before_time_str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);
                        time_hour += before_time.Hour;
                        if (time_hour >= 24)
                        {
                            time_hour -= 24;
                            time_day += 1;
                        }
                        before_time = before_time.AddDays(time_day);
                        daystr = before_time.ToString("yyyyMMdd") + time_hour.ToString("00");
                    }
                }
                dgv_next.Rows[X].Cells[6].Value = daystr;//换模时间
            }
        }

        /// <summary>
        /// 获取当前周
        /// </summary>
        /// <param name="dTime"></param>
        /// <returns></returns>
        private static int GetWeek(DateTime dTime)
        {
            //int firstWeek = Convert.ToInt32(DateTime.Parse(dTime.Year.ToString() + "-1-1").DayOfWeek);

            //int weekDay = firstWeek == 0 ? 1 : (7 - firstWeek + 1);

            //int currentDay = dTime.DayOfYear;

            //int current_week = Convert.ToInt32(Math.Ceiling((currentDay - weekDay) / 7.0)) + 1;

            //return current_week;

            int first_day = Convert.ToInt32(DateTime.Parse(dTime.Year.ToString() + "-1-1").DayOfWeek);//第一天是星期几

            int first_weekday_num = DateTime.Parse(dTime.Year.ToString() + "-1-" + (7 - first_day)).Day;

            int weekday_num = dTime.DayOfYear;

            int diffday = weekday_num - first_weekday_num;//相差几天

            int current_week = (diffday / 7) + 1;

            return current_week;
        }

        /// <summary>
        /// 获取年最大周数
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int MaxWeekOfYear(int year)
        {
            if (year < 1 || year > 9999)
                throw new ArgumentException("illegal year", "year");

            int maxDays = (DateTime.IsLeapYear(year) ? 366 : 365);

            DayOfWeek firstDayOfWeek = new DateTime(year, 1, 1).DayOfWeek;

            int beforeFirstSunday = (7 - Convert.ToInt32(firstDayOfWeek)) % 7;

            int remainDays = maxDays - beforeFirstSunday;

            int ret = (beforeFirstSunday % 7 == 0 ? 0 : 1);
            ret += (remainDays / 7);
            ret += (remainDays % 7 == 0 ? 0 : 1);

            return ret;
        }

        /// <summary>
        /// 获取该周 周日
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeWeekFirstDaySun(DateTime dateTime) 
        {
            DateTime firstWeekDay = DateTime.Now;

            try
            {
                //得到是星期几，然后从当前日期减去相应天数   
                int weeknow = Convert.ToInt32(dateTime.DayOfWeek);

                int daydiff = (-1) * weeknow;

                firstWeekDay = dateTime.AddDays(daydiff);
            }
            catch { }

            return firstWeekDay;
        }
    }
}

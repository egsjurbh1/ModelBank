﻿using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ModelBank
{
    public partial class MainForm : Form
    {
        //配置参数
        public class Cfg
        {
            public const int nLength = 24;                                 //支持的参数总数     
            public static int nParaNum = 0;                                //参数数量
            public static string sFilepath = "./funcidconfig.xml";         //xml文件路径

            public const string sKey_root = "funcid";                      //xml根节点名
            public const string sKey_funcid = "peercode";                  //xml子节点funcid属性名
            public const string sKey_describe = "describe";                //xml子节点describe属性名
            public const string sKey_avail = "available";                  //xml子节点available属性名
            //xml孙节点属性
            public static readonly string[] sPara = new string[4] { "keyname", "length", "default", "empty" };
            public static string[] sFuncid = new string[64];               //xml配置的功能号列表
        }

        //数组控件
        TextBox[] tblen = new TextBox[Cfg.nLength];
        TextBox[] tbv = new TextBox[Cfg.nLength];
        Label[] lb = new Label[Cfg.nLength];

        public MainForm()
        {
            InitializeComponent();
            bt_GeneBankData.Enabled = false;
            btgetdata.Enabled = false;
            //label初始化不显示
            tssLable.Text = "";
            init();
            tssLable.Text = "启动成功";
            tssLable.ForeColor = Color.ForestGreen;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void init()
        {
            int i = 0;
            string strname = null;
            const string stblen = "tblength";
            const string stbv = "tbvalue";
            const string slbp = "lbpara";

            //数组控件处理
            foreach (Control ctl in gbPara.Controls)
            {
                //获取控件名
                strname = ctl.Name.ToString();
                //按类型和控件名判断
                if (ctl is TextBox && strname.StartsWith(stblen))
                {
                    //取控件名的数字部分
                    i = int.Parse(strname.Remove(0, stblen.Length));
                    //tblen[6]对应tblength6
                    tblen[i-1] = (TextBox)ctl;
                }
                else if (ctl is TextBox && strname.StartsWith(stbv))
                {
                    i = int.Parse(strname.Remove(0, stbv.Length));
                    tbv[i-1] = (TextBox)ctl;
                }
                else if (ctl is Label && strname.StartsWith(slbp))
                {
                    i = int.Parse(strname.Remove(0, slbp.Length));
                    lb[i - 1] = (Label)ctl;
                    lb[i - 1].Text = "";    //不显示
                }
            }
           
            //获取功能号
            readfuncfromxml(Cfg.sFilepath, Cfg.sKey_root, Cfg.sKey_funcid, Cfg.sKey_avail, Cfg.sKey_describe);
        }

        /// <summary>
        /// 读xml获取可用功能
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="skey_root"></param>
        /// <param name="skey_funcid"></param>
        /// <param name="skey_avail"></param>
        /// <param name="skey_des"></param>
        private void readfuncfromxml(string filepath, string skey_root, string skey_funcid, string skey_avail, string skey_des)
        {
            XmlAnalysis xma = new XmlAnalysis();
            string[] sFunc = new string[64];
            string sError = null;          
            int nFuncAvailn = 0;

            //读取xml配置文件中的可用功能
            bool bSuccess = xma.readxml_findfunc(filepath, ref sFunc, ref Cfg.sFuncid, ref sError, skey_des,
                                        skey_root, skey_funcid, skey_avail, ref nFuncAvailn);
            if (!bSuccess)
            {
                MessageBox.Show("Error:" + sError);
                return;
            }
            //更新下拉功能列表
            for (int i = 0; i < nFuncAvailn; ++i)
                cbb_funcid.Items.Add(sFunc[i]);
        }

        /// <summary>
        /// 功能号选择及解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbb_funcid_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmlAnalysis xmla = new XmlAnalysis();
            string sError = null;
            int i = 0;
            string sFuncid = "";
            bt_GeneBankData.Enabled = false;
            btgetdata.Enabled = false;
            tssLable.Text = "";
            tssLable.ForeColor = Color.Black;

            //结果集
            DataTable dtResult = new DataTable();
            for(int k = 0; k < Cfg.sPara.Length; ++k)
                dtResult.Columns.Add(new DataColumn(Cfg.sPara[k], typeof(string)));

            //功能判断
            string sfunc = cbb_funcid.Text;
            foreach (string sf in Cfg.sFuncid)
            {
                if (sf == null)
                    continue;
                if (sfunc.Contains(sf))
                    sFuncid = sf;
            }

            //解析xml
            bool bSuccess = xmla.readxml_funcPara(Cfg.sFilepath, sFuncid, ref sError, ref dtResult,
                            Cfg.sKey_root, Cfg.sKey_funcid, Cfg.sKey_avail, Cfg.sPara);
            if (!bSuccess)
            {
                MessageBox.Show("Error:" + sError);
                return;
            }

            //参数数量
            Cfg.nParaNum = dtResult.Rows.Count;
            if(Cfg.nParaNum > Cfg.nLength)
            {
                MessageBox.Show("Error:" + "该功能的参数超过了当前系统限制！" + Cfg.nLength.ToString());
                return;
            }

            //输出到前端控件
            foreach(DataRow dr in dtResult.Rows)
            {
                try
                {
                    lb[i].Text = dr[Cfg.sPara[0]].ToString();           //keyname
                    tblen[i].Text = dr[Cfg.sPara[1]].ToString();        //length
                    tbv[i].Text = dr[Cfg.sPara[2]].ToString();          //value
                    tbv[i].MaxLength = int.Parse(tblen[i].Text);        //长度控制
                    //是否为空
                    if (dr[Cfg.sPara[3]].ToString() == "no")
                    {
                        lb[i].ForeColor = Color.Red;
                        tbv[i].Enabled = true;
                    }
                    else if (dr[Cfg.sPara[3]].ToString() == "yes")
                    {
                        lb[i].ForeColor = Color.Black;
                        tbv[i].Enabled = false;
                    }
                    else if (dr[Cfg.sPara[3]].ToString() == "solid")
                    {
                        lb[i].ForeColor = Color.Black;
                        tbv[i].Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show(Cfg.sPara[3].ToString() + "字段配置有误（必须为yes、no或solid");
                        return;
                    }
                    //对日期的特殊处理
                    if (lb[i].Text.Contains("operdate"))
                    {
                        tbv[i].Text = DateTime.Now.ToString("yyyyMMdd");
                    }
                    else if (lb[i].Text.Contains("opertime"))
                    {
                        tbv[i].Text = DateTime.Now.ToString("hhmmss");
                    }
                    ++i;
                }
                catch(Exception ed)
                {
                    MessageBox.Show(ed.ToString());        
                }
            }

            //多余的控件关闭
            for(int j = Cfg.nLength - 1; j >= Cfg.nParaNum; --j)
            {
                tblen[j].Enabled = false;
                tbv[j].Enabled = false;
                lb[j].Text = "";
            }
            bt_GeneBankData.Enabled = true;
            btgetdata.Enabled = true;
            tssLable.Text = "从xml文件导入成功";
            tssLable.ForeColor = Color.ForestGreen;     
        }

        /// <summary>
        /// 生成buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_GeneBankData_Click(object sender, EventArgs e)
        {
            int[] length = new int[Cfg.nParaNum];
            string[] keyvalue = new string[Cfg.nParaNum];
            tssLable.Text = "";
            tssLable.ForeColor = Color.Black;

            //获取控件值
            for (int i = 0; i < Cfg.nParaNum; ++i)
            {
                keyvalue[i] = tbv[i].Text;
                length[i] = int.Parse(tblen[i].Text);
            }
            string s = null;
            //拼buffer
            for (int i = 0; i < Cfg.nParaNum; i++)
            {
                //int oldlength = keyvalue[i].Length; //获取字符长度
                int oldlength = Encoding.Default.GetByteCount(keyvalue[i]); //获取字符长度修正20150909
                for (int j = 0; j < (length[i] - oldlength); j++)
                {
                    keyvalue[i] = keyvalue[i] + " ";
                }
                s = s + keyvalue[i];
            }
            rtbresultdata.Text = "";    //清空
            rtbresultdata.Text = s;
            //合计
            tssLable.Text = "生成buffer成功:" + s.Length.ToString() + "字节"; ;
            tssLable.ForeColor = Color.ForestGreen;
            bt_sendbank.Enabled = true;
        }

        /// <summary>
        /// 获取客户数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btgetdata_Click(object sender, EventArgs e)
        {
            //数据库连接描述
            string dbconnect_str = "Data Source=" + tbDataSource.Text + ";" +
                                    "Initial Catalog=" + tbdbname.Text + ";" +
                                    "Persist Security Info=True;" +
                                    "User ID=" + tbdbuserid.Text + ";" +
                                    "Password=" + tbdbpassword.Text + ";";
            //连接数据库
            SqlProcess sp = new SqlProcess();
            //测试数据库连接
            if (!sp.ConnectSQL(dbconnect_str))
            {
                MessageBox.Show("-10001数据库连接失败！请重新配置。");
                return;
            }

            //查找客户信息         
            DataTable dtResult = new DataTable(); //结果集
            DataTable dtResult2 = new DataTable(); //结果集2
            DataTable dtResult3 = new DataTable(); //结果集3
            int custid = 0;
            try
            {
                custid = int.Parse(tbCustid.Text);
            }
            catch
            {
                MessageBox.Show("-10002custid配置有误，必须为数字");
                return;
            }

            bool bSuccess = getuserdata(custid, dbconnect_str, tbdbname.Text, ref dtResult, ref dtResult2, ref dtResult3);
            if (!bSuccess)
            {
                MessageBox.Show("-10010未查到客户数据");
                return;
            }

            //客户数据写入
            for (int i = 0; i < Cfg.nParaNum; ++i)
            {
                if (lb[i].Text == "b_custname")
                {
                    tbv[i].Text = dtResult.Rows[0]["custlname"].ToString().Trim();
                    lb[i].ForeColor = Color.ForestGreen;
                }
                else if (lb[i].Text == "b_idtype")
                {
                    tbv[i].Text = dtResult.Rows[0]["idtype"].ToString().Trim();
                    lb[i].ForeColor = Color.ForestGreen;
                }
                else if (lb[i].Text == "b_idno")
                {
                    tbv[i].Text = dtResult.Rows[0]["idno"].ToString().Trim();
                    lb[i].ForeColor = Color.ForestGreen;
                }
                else if (lb[i].Text == "b_bankid")
                {
                    tbv[i].Text = dtResult.Rows[0]["bankid"].ToString().Trim();
                    lb[i].ForeColor = Color.ForestGreen;
                }
                else if (lb[i].Text == "b_moneytype")
                {
                    tbv[i].Text = dtResult.Rows[0]["moneytype"].ToString().Trim();
                    lb[i].ForeColor = Color.ForestGreen;
                }
                else if (lb[i].Text == "b_fundid")
                {
                    string orgid = dtResult.Rows[0]["orgid"].ToString().Trim();
                    string fundid = dtResult.Rows[0]["fundid"].ToString().Trim();
                    string smid = null;
                    for (int j = 0; j < 10 - fundid.Length; j++)
                    {
                        smid = smid + "0";
                    }
                    tbv[i].Text = orgid + smid + fundid;
                    lb[i].ForeColor = Color.ForestGreen;
                }
                else if (lb[i].Text == "b_custtype")
                {
                    tbv[i].Text = dtResult2.Rows[0]["singleflag"].ToString().Trim();
                    lb[i].ForeColor = Color.ForestGreen;
                }
                else if (lb[i].Text == "b_bksno")
                {
                    Random ran = new Random();
                    int rkey = ran.Next(10000, 99999);
                    tbv[i].Text = dtResult3.Rows[0]["sno"].ToString().Trim() + rkey.ToString();
                    lb[i].ForeColor = Color.ForestGreen;
                }

                tssLable.Text = "导入客户数据成功";
                tssLable.ForeColor = Color.ForestGreen;
            }
            }

        public bool getuserdata(int custid, string dbconnectstr, string dbname, ref DataTable dt1, ref DataTable dt2, ref DataTable dt3)
        {
            SqlProcess sp = new SqlProcess();
            string dbtable1 = "..banktranid ";
            string dbtable2 = "..custbaseinfo ";
            string dbtable3 = "..logbanktran ";

            //查客户数据
            string sql1 = "select custlname,idtype,idno,fundid,moneytype,bankid,orgid from " + dbname + dbtable1 + "where custid=" + custid.ToString();
            sp.ExecSingleSQL(dbconnectstr, sql1, dt1);
            //若未查到信息，返回false
            if (dt1.Rows.Count == 0)
                return false;

            //查客户类型数据
            string sql2 = "select singleflag from " + dbname + dbtable2 + "where custid=" + custid.ToString();
            sp.ExecSingleSQL(dbconnectstr, sql2, dt2);
            //若未查到信息，返回false
            if (dt2.Rows.Count == 0)
                return false;

            //查流水
            string sql3 = "select sno from " + dbname + dbtable3 + "where custid=" + custid.ToString() + " order by sno desc";
            sp.ExecSingleSQL(dbconnectstr, sql3, dt3);
            //若未查到信息，返回false
            if (dt3.Rows.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// 发送银行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_sendbank_Click(object sender, EventArgs e)
        {
            string sIp = tb_bankip.Text;                    //IP
            int nPort = Int32.Parse(tb_bankport.Text);      //Port

            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse(sIp);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //连接服务器
                clientSocket.Connect(new IPEndPoint(ip, nPort));
                tssLable.Text = "连接服务器成功";
                tssLable.ForeColor = Color.ForestGreen;
            }
            catch
            {
                MessageBox.Show("-20001连接服务器失败。");
                tssLable.Text = "连接服务器失败";
                tssLable.ForeColor = Color.Red;
                return;
            }

            //发送数据
            try
            {
                string sendMessage = rtbresultdata.Text;
                clientSocket.Send(Encoding.GetEncoding("GB2312").GetBytes(sendMessage));
                tssLable.Text = "发送消息……";
                rtb_recvbankdata.Text = "";
            }
            catch
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                MessageBox.Show("-20002发送数据失败。");
                tssLable.Text = "发送数据失败";
                tssLable.ForeColor = Color.Red;
                return;
            }

            //通过clientSocket接收数据
            byte[] result = new byte[1024];
            int receiveLength = 0;
            //接收
            clientSocket.ReceiveTimeout = 3000;   //设置超时时间3s
            try
            {
                receiveLength = clientSocket.Receive(result);
                rtb_recvbankdata.Text = Encoding.GetEncoding("GB2312").GetString(result, 0, receiveLength);
                tssLable.Text = "接收消息成功";
                tssLable.ForeColor = Color.ForestGreen;
            }
            catch(Exception)
            {
                tssLable.Text = "接收消息失败：连接超时或其它";
                tssLable.ForeColor = Color.Red;
                clientSocket.Close();
                return;
            }

            //关闭连接
            //clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}

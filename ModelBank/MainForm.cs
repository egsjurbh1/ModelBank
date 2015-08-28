using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Collections;

namespace ModelBank
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            bt_GeneBankData.Enabled = false;
        }

        int[] length;
        string[] keyvalue;
        int n = 0;
        const int nLength = 24;

        /// <summary>
        /// 功能号选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbb_funcid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(string.Compare(cbb_funcid.Text, "160303预指定确认") == 0)
            {
                l1.Text = "b_packlen"; tblength1.Text = "4"; l1.ForeColor = Color.Red; tbvalue1.Text = "0238"; tbvalue1.Enabled = false; 
                l2.Text = "b_transcode"; tblength2.Text = "3"; l2.ForeColor = Color.Red; tbvalue2.Text = "104"; tbvalue2.Enabled = false;
                l3.Text = "b_sourcetype"; tblength3.Text = "1"; l3.ForeColor = Color.Red; tbvalue3.Text = "1"; tbvalue3.Enabled = false;
                l4.Text = "b_bkid"; tblength4.Text = "4"; l4.ForeColor = Color.Red; tbvalue4.Text = " "; tbvalue4.Enabled = false;
                l5.Text = "b_scid"; tblength5.Text = "8"; l5.ForeColor = Color.Red; tbvalue5.Text = " "; tbvalue5.Enabled = false;
                l6.Text = "b_bksno"; tblength6.Text = "20"; l6.ForeColor = Color.ForestGreen; tbvalue6.Text = "12341234123412341234"; 
                l7.Text = "b_packstatus"; tblength7.Text = "1"; l7.ForeColor = Color.Red; tbvalue7.Text = "0"; tbvalue7.Enabled = false;
                l8.Text = "b_operdate"; tblength8.Text = "8"; l8.ForeColor = Color.ForestGreen; tbvalue8.Text = DateTime.Now.ToString("yyyyMMdd");
                l9.Text = "b_opertime"; tblength9.Text = "6"; l9.ForeColor = Color.ForestGreen; tbvalue9.Text = DateTime.Now.ToString("hhmmss");
                l10.Text = "b_custtype"; tblength10.Text = "1"; l10.ForeColor = Color.ForestGreen; tbvalue10.Text = "0";
                l11.Text = "b_custname"; tblength11.Text = "60"; l11.ForeColor = Color.ForestGreen; tbvalue11.Text = "客户姓名";
                l12.Text = "b_idtype"; tblength12.Text = "1"; l12.ForeColor = Color.ForestGreen; tbvalue12.Text = "证件类型";
                l13.Text = "b_idno"; tblength13.Text = "32"; l13.ForeColor = Color.ForestGreen; tbvalue13.Text = "证件号";
                l14.Text = "b_orgid"; tblength14.Text = "4"; l14.ForeColor = Color.ForestGreen; tbvalue14.Text = "0000";
                l15.Text = "b_fundid"; tblength15.Text = "14"; l15.ForeColor = Color.ForestGreen; tbvalue15.Text = "fundid";
                l16.Text = "b_fundpwd"; tblength16.Text = "16"; l16.ForeColor = Color.Red; tbvalue16.Text = "990818"; tbvalue16.Enabled = false;
                l17.Text = "b_bankid"; tblength17.Text = "32"; l17.ForeColor = Color.ForestGreen; tbvalue17.Text = "银行卡";
                l18.Text = "b_newbankid"; tblength18.Text = "32"; l18.ForeColor = Color.Red; tbvalue18.Text = " "; tbvalue18.Enabled = false;
                l19.Text = "moneytype_tmp"; tblength19.Text = "3"; l19.ForeColor = Color.ForestGreen; tbvalue19.Text = "000";
                l20.Text = "b_moneytype"; tblength20.Text = "1"; l20.ForeColor = Color.ForestGreen; tbvalue20.Text = "0";
                l21.Text = "b_moneykind"; tblength21.Text = "1"; l21.ForeColor = Color.Red; tbvalue21.Text = " "; tbvalue21.Enabled = false;
                l22.Text = "b_mac"; tblength22.Text = "8"; l22.ForeColor = Color.Red; tbvalue22.Text = " "; tbvalue22.Enabled = false;
                l23.Text = "null"; tblength23.Text = "0"; l23.ForeColor = Color.Gray; tbvalue23.Text = " "; tbvalue23.Enabled = false;
                l24.Text = "null"; tblength24.Text = "0"; l24.ForeColor = Color.Gray; tbvalue24.Text = " "; tbvalue24.Enabled = false;
                n = 22;
                bt_GeneBankData.Enabled = true;
            }
        }

        /// <summary>
        /// 生成buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_GeneBankData_Click(object sender, EventArgs e)
        {
            length = new int[nLength];
            keyvalue = new string[nLength];
            keyvalue[0] = tbvalue1.Text; length[0] = int.Parse(tblength1.Text);
            keyvalue[1] = tbvalue2.Text; length[1] = int.Parse(tblength2.Text);
            keyvalue[2] = tbvalue3.Text; length[2] = int.Parse(tblength3.Text);
            keyvalue[3] = tbvalue4.Text; length[3] = int.Parse(tblength4.Text);
            keyvalue[4] = tbvalue5.Text; length[4] = int.Parse(tblength5.Text);
            keyvalue[5] = tbvalue6.Text; length[5] = int.Parse(tblength6.Text); 
            keyvalue[6] = tbvalue7.Text; length[6] = int.Parse(tblength7.Text);
            keyvalue[7] = tbvalue8.Text; length[7] = int.Parse(tblength8.Text);
            keyvalue[8] = tbvalue9.Text; length[8] = int.Parse(tblength9.Text);
            keyvalue[9] = tbvalue10.Text; length[9] = int.Parse(tblength10.Text);
            keyvalue[10] = tbvalue11.Text; length[10] = int.Parse(tblength11.Text);
            keyvalue[11] = tbvalue12.Text; length[11] = int.Parse(tblength12.Text);
            keyvalue[12] = tbvalue13.Text; length[12] = int.Parse(tblength13.Text);
            keyvalue[13] = tbvalue14.Text; length[13] = int.Parse(tblength14.Text);
            keyvalue[14] = tbvalue15.Text; length[14] = int.Parse(tblength15.Text);
            keyvalue[15] = tbvalue16.Text; length[15] = int.Parse(tblength16.Text);
            keyvalue[16] = tbvalue17.Text; length[16] = int.Parse(tblength17.Text);
            keyvalue[17] = tbvalue18.Text; length[17] = int.Parse(tblength18.Text);
            keyvalue[18] = tbvalue19.Text; length[18] = int.Parse(tblength19.Text);
            keyvalue[19] = tbvalue20.Text; length[19] = int.Parse(tblength20.Text);
            keyvalue[20] = tbvalue21.Text; length[20] = int.Parse(tblength21.Text);
            keyvalue[21] = tbvalue22.Text; length[21] = int.Parse(tblength22.Text);
            keyvalue[22] = tbvalue22.Text; length[22] = int.Parse(tblength23.Text);
            keyvalue[23] = tbvalue22.Text; length[23] = int.Parse(tblength24.Text);

            string s = null;
            //拼buffer
            for (int i = 0; i < n; i++)
            {
                int oldlength = keyvalue[i].Length;
                for (int j = 0; j < (length[i] - oldlength); j++)
                {
                    keyvalue[i] = keyvalue[i] + " ";
                }
                s = s + keyvalue[i];
            }
            rtbresultdata.Text = "";
            rtbresultdata.Text = s;
        }

        /// <summary>
        /// 读取xml，解析放入DataSet
        /// </summary>
        /// <param name="xmlfilepath"></param>
        /// <returns></returns>
        //DataSet ReadXMLFile(string xmlfilepath, string funcid)
        //{
        //    DataSet ds = new DataSet();
        //    XmlDocument xmd = new XmlDocument();
        //    XmlReaderSettings setting = new XmlReaderSettings();
        //    setting.IgnoreComments = true;  //忽略注释
        //    XmlReader reader = XmlReader.Create(xmlfilepath, setting);

        //    try
        //    {
        //        xmd.Load(xmlfilepath);  //Load Xml
        //        XmlNode xnroot = xmd.SelectSingleNode(Cfg.sanfrootnode);    //根节点
        //        XmlNodeList xnl = xnroot.ChildNodes;                        //获取子节点

        //        foreach (XmlNode xn in xnl)
        //        {
        //            //节点转换为元素
        //            XmlElement xe = (XmlElement)xn;
        //            //获取funcid
        //            string curfuncid = xe.GetAttribute(Cfg.sanffuncid).ToString();
        //            //找到当前功能点
        //            if (string.Compare(funcid, curfuncid) == 0)
        //            {
        //                XmlNodeList xnl_cfg = xe.SelectNodes("./req/dsd");   //xe.GetElementsByTagName("dsd");
        //                //取子节点配置
        //                foreach (XmlNode xn_cfg in xnl_cfg)
        //                {
        //                    XmlElement xe_cfg = (XmlElement)xn;
        //                    string keyname = xe_cfg.Ge
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message);
        //    }

        //    //reader.Close;
        //    return ds;
        //}
    }
}

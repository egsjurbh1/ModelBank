using System;
using System.Xml;
using System.Data;
using System.Windows.Forms;

namespace ModelBank
{
    class XmlAnalysis
    {
        /// <summary>
        /// 解析xml功能号下的参数
        /// </summary>
        /// <param name="sFilepath"></param>
        /// <param name="sFuncid"></param>
        /// <param name="sErrormsg"></param>
        /// <param name="dtResult"></param>
        /// <param name="skey_root"></param>
        /// <param name="skey_funcid"></param>
        /// <param name="skey_avail"></param>
        /// <param name="sPara"></param>
        /// <returns></returns>
        public bool readxml_funcPara(string sFilepath, string sFuncid, ref string sErrormsg, ref DataTable dtResult,
                                    string skey_root, string skey_funcid, string skey_avail, params string[] sPara)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlReaderSettings setting = new XmlReaderSettings();
                setting.IgnoreComments = true;  //忽略注释
                XmlReader reader = XmlReader.Create(sFilepath, setting);

                xmlDoc.Load(reader);
                XmlNode root = xmlDoc.SelectSingleNode(skey_root);  //父节点
                //获取到根节点的子节点
                XmlNodeList nodeList = root.ChildNodes;
                XmlNodeList subList = root.ChildNodes;  //孙节点

                //遍历所有子节点,取当前功能号参数
                foreach (XmlNode xn in nodeList)
                {
                    XmlElement xe = (XmlElement)xn;     //转换为元素
                    string sCurFuncid = xe.GetAttribute(skey_funcid);
                    string sAvailable = xe.GetAttribute(skey_avail);
                    if (string.Compare(sCurFuncid, sFuncid) != 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (string.Compare(sAvailable, "yes") == 0)
                        {
                            subList = xe.ChildNodes;    //取孙节点
                            break;
                        }
                        else if (string.Compare(sAvailable, "no") == 0)
                        {
                            sErrormsg = "当前配置为不可用," + "Funcid=" + sCurFuncid;
                            return false;
                        }
                        else
                        {
                            sErrormsg = "字段配置有误（必须为yes或no），" + "Funcid=" + sCurFuncid;
                            return false;
                        }
                    }
                }

                //遍历孙节点，取参数
                DataRow dr;
                foreach (XmlNode xmlNode in subList)
                {
                    dr = dtResult.NewRow();
                    XmlElement xe_sub = (XmlElement)xmlNode;     //转换为元素
                    //遍历获取参数
                    for(int i = 0; i < sPara.Length; i++)
                        dr[sPara[i]] = xe_sub.GetAttribute(sPara[i]);

                    //加入到结果集
                    dtResult.Rows.Add(dr);
                }
            }
            catch( Exception e)
            {
                MessageBox.Show("Error:" + e);
            }
            return true;
        }

        /// <summary>
        /// 获取xml中可用的功能号
        /// </summary>
        /// <param name="sFilepath"></param>
        /// <param name="sFunc"></param>
        /// <param name="sFuncid"></param>
        /// <param name="sErrormsg"></param>
        /// <param name="skey_des"></param>
        /// <param name="skey_root"></param>
        /// <param name="skey_funcid"></param>
        /// <param name="skey_avail"></param>
        /// <param name="nFuncAvailNum"></param>
        /// <returns></returns>
        public bool readxml_findfunc(string sFilepath, ref string[] sFunc, ref string[] sFuncid, ref string sErrormsg, 
            string skey_des, string skey_root, string skey_funcid, string skey_avail, ref int nFuncAvailNum)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlReaderSettings setting = new XmlReaderSettings();
                setting.IgnoreComments = true;  //忽略注释
                XmlReader reader = XmlReader.Create(sFilepath, setting);

                xmlDoc.Load(reader);
                XmlNode root = xmlDoc.SelectSingleNode(skey_root);  //父节点
                //获取到根节点的子节点
                XmlNodeList nodeList = root.ChildNodes;
                int nfunc = 0;
                nFuncAvailNum = 0;
                //遍历所有子节点,取功能号
                foreach (XmlNode xn in nodeList)
                {
                    XmlElement xe = (XmlElement)xn;     //转换为元素
                    string sAvailable = xe.GetAttribute(skey_avail);
                    //所有功能号
                    string sCurFuncid = xe.GetAttribute(skey_funcid);
                    sFuncid[nfunc] = sCurFuncid;
                    ++nfunc;
                    //可用功能号
                    if (string.Compare(sAvailable, "yes") == 0)
                    {                      
                        string sFuncDes = xe.GetAttribute(skey_des);
                        sFunc[nFuncAvailNum] = sCurFuncid + sFuncDes;
                        ++nFuncAvailNum;
                    }       
                }
                //没有可用的情况
                if(nFuncAvailNum == 0)
                {
                    sErrormsg = "当前没有可用的功能号，请检查配置文件";
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error:" + e);
            }
            return true;
        }
    }
}

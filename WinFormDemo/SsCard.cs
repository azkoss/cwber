using System;
using System.Collections.Generic;
using MedicareComLib;
using System.Xml;
using log4net;

namespace WinFormDemo
{
    public static class SsCard
    {
        #region 数据结构

        private static ILog log = log4net.LogManager.GetLogger("sscardLog");
        private static OutpatientClass m_hObj = new OutpatientClass();  


        /// <summary>
        /// 个人信息
        /// </summary>
        public struct SS_PersonInfo
        {
            public int code; //状态码
            public String msg; //消息
            public String sIC_NO; // 卡号
            public String sPersonName; //姓名
            public String sPactCode; //险种类别，离休是31  公费是32
            public String sFromHos; //转诊医院编码
            public String sFromHosDate; //转诊时限
            public String sHosFlag; //住院标示
            public String sPerType; //参保人员类别
            public String sInRedList; //是否在红名单
            public String sIsFiedHos; //定点医院状态
            public string sSex; //性别
            public string sBirthday; //出生日期
            public string sId_NO; //身份证号
            public string sPersonCount; //身份证号
        }

        /// <summary>
        /// 卡信息
        /// </summary>
        public struct SS_CardInfo
        {
            public int code; //状态码
            public String msg; //消息
            public String sIC_NO; // 卡号
            public String sPersonName; //姓名
            public String sPersonSex; //性别
            public String sPersonID; //身份证
        }

        /// <summary>
        /// 费用分解信息
        /// </summary>
        public struct SS_DivideInfo
        {
            public String strCuretype; //就诊类型
            public String strIlltype; //就诊方式
            public String strFeeno; //收费单据号
            public String strOperator; //收费员
            public String strSectionCode; //就诊科别代码
            public String strSectionName; //就诊科别名称
            public String strDrid; //医师编号
            public String strDrNamr; //医师姓名
            public String strDrlevel; //医师职称
            public String strUnitprice; //诊疗费
            public String strGhFee; //挂号费
            public String strSumprice; //总费用

            //public String strGH_Fee;
            //public String strGH_hiscode;   //挂号信息---hiscode
            //public String strGH_itemname;  //挂号信息---itemname

            //public String strZL_Fee;
            //public String strZL_hiscode;   //诊疗信息---hiscode
            //public String strZL_itemname;  //诊疗信息---itemname
        }

        /// <summary>
        /// 费用分解结果
        /// </summary>
        public struct SS_DivideResult
        {
            public int code; //状态码
            public String msg; //消息

            public String sTradeNO; //交易流水
            public String sFeeNO; //收费单据号
            public String sTradeDate; //交易日期

            public String sFeeall; //费用总金额
            public String sFund; //基金支付
            public String sCash; //现金支付
            public String sPersoncountpay; //个人帐户支付
            public String sPersoncount; //个人账户余额

            //分解费用返回信息
            public String par_tradedate; //  --交易日期
            public String par_feeall; // --费用总金额
            public String par_fund; //--基金支付
            public String par_cash; // --现金支付
            public String par_personcountpay; //  --个人帐户支付
            public String par_personcount; //  --个人账户余额

            public String par_mzfee;
            public String par_mzfeein;
            public String par_mzfeeout;
            public String par_mzpayfirst;
            public String par_mzselfpay2;
            public String par_mzbigpay;
            public String par_mzbigselfpay;
            public String par_mzoutofbig;
            public String par_bcpay;
            public String par_jcbz;

            public String par_recpeno;
            public String par_hiscode;
            public String par_itemcode;
            public String par_itemname;
            public String par_itemtype;
            public String par_unitprice;
            public String par_fee;
            public String par_feein;
            public String par_feeout;
            public String par_selfpay2;
            public String par_state;
            public String par_feetype;
            public String par_preferentialfee;
            public String par_preferentialscale;

            public String par_hiscode2;
            public String par_itemcode2;
            public String par_itemname2;
            public String par_itemtype2;
            public String par_unitprice2;
            public String par_fee2;
            public String par_feein2;
            public String par_feeout2;
            public String par_selfpay22;
            public String par_state2;
            public String par_feetype2;
            public String par_preferentialfee2;
            public String par_preferentialscale2;
        }

        #endregion

        #region 私有变量

        #endregion

        #region 公共方法

        /// <summary>
        /// 初始化读卡器
        /// </summary>
        /// <param name="strOut"></param>
        /// <returns></returns>
        public static int SS_Open(ref String strOut)
        {
            int ret = 0;
            String strInfo = null;
            m_hObj.Open(out strInfo);
            log.Info(strInfo);
            XmlDocument xmlDoc = GetXmlDoc(strInfo);

            bool bRet = CheckOutputState(xmlDoc, ref strInfo);
            xmlDoc = null;

            if (bRet == false)
            {
                strOut = strInfo;
                return -1;
            }
            strOut = "打开成功";

            return ret;
        }

        /// <summary>
        /// 关闭读卡器
        /// </summary>
        /// <param name="strOut"></param>
        /// <returns></returns>
        public static string SS_Close(ref String strOut)
        {
            int ret = 0;

           

            String strInfo = null;  
            m_hObj.Close(out strInfo);

            XmlDocument xmlDoc = GetXmlDoc(strInfo);

            bool bRet = CheckOutputState(xmlDoc, ref strInfo);
            xmlDoc = null;

            if (bRet == false)
            {
                strOut = strInfo;
                return strOut;
            }

            
            strOut = "关闭成功";

            return ret.ToString();
        }

        /// <summary>
        /// 获取用户个人信息
        /// </summary>
        /// <returns></returns>
        public static String  SS_GetPersonInfo()
        {
            String strInfo = null;
            m_hObj.GetPersonInfo(out strInfo);
        
            return strInfo;
        }

        /// <summary>
        /// 读取卡信息
        /// </summary>
        /// <returns></returns>
        public static SS_CardInfo SS_GetCardInfo()
        {
            SS_CardInfo info = new SS_CardInfo();
            String strInfo = null;
            if (WinFormDemo.Properties.Settings.Default.testMode) //不调试
            {//调试不检测
                strInfo = "<?xml version=\"1.0\" encoding=\"UTF-16\" standalone=\"yes\"?>" +
                          "<root version=\"1.00.0260\">" +
                          "     <state success=\"true\">" +
                          "         <error></error>" +
                          "         <warning></warning>" +
                          "     </state>" +
                          "     <output>" +
                          "         <ic>" +
                          "             <card_no>000004788003</card_no>" +
                          "             <ic_no>00000478800S</ic_no>" +
                          "             <id_no>430221198810091119</id_no>" +
                          "             <personname>王五</personname>" +
                          "             <sex>1</sex>" +
                          "             <birthday>19881009</birthday>" +
                          "         </ic>" +
                          "     </output>" +
                          "</root>";
            }
            else
            {
                m_hObj.GetCardInfo(out strInfo);
            }
            log.Info("读取卡信息" + strInfo);

            XmlDocument xmlDoc = GetXmlDoc(strInfo);

            bool bRet = CheckOutputState(xmlDoc, ref strInfo);

            if (bRet)
            {
                XmlNode dataNode = GetNodeFromPath(xmlDoc.DocumentElement, "output/ic");
                info.sIC_NO = dataNode.SelectNodes("ic_no")[0].InnerText;
                info.sPersonName = dataNode.SelectNodes("personname")[0].InnerText;
                info.sPersonSex = dataNode.SelectNodes("sex")[0].InnerText;

                info.code = 0;
                info.msg = "成功读取卡信息";
            }
            else
            {
                info.code = -1;
                info.msg = "读取失败";
            }

            return info;
        }

        /// <summary>
        /// 分解费用
        /// </summary>
        /// <param name="divideInfo"></param>
        /// <returns></returns>
        public static string SS_Divide(string strIn)
        {
                string strOut = null;
                m_hObj.Divide(strIn,out strOut);
                return strOut;
        }

        /// <summary>
        /// 确认交易
        /// </summary>
        /// <param name="strBalance">余额，返回</param>
        /// <param name="strOut"></param>
        /// <returns></returns>
        public static string SS_Trade()
        {
           
                String strInfo = null;
             
                m_hObj.Trade(out strInfo);

                return strInfo;
        }
        public static string YYT_YB_REFUND(string TradeNo,string operator_id)
        {
            String outStr = null;
            try
            {
                m_hObj.RefundmentDivide(TradeNo,operator_id, out outStr);
                return outStr;
            }
            catch(Exception e) {
                return e.ToString();
            }
            
        }
        /// <summary>
        /// 查询交易
        /// </summary>
        /// <param name="sTradeNO">交易流水号</param>
        /// <param name="strRes"></param>
        /// <returns></returns>
        public static int SS_CommitTradeState(String sTradeNO, ref String strState, ref String strRes)
        {
            // OutpatientClass m_hObj = new OutpatientClass();
            String sOut = null;
            m_hObj.CommitTradeState(sTradeNO, out sOut);

            XmlDocument xmlDoc = GetXmlDoc(sOut);

            bool bRet = CheckOutputState(xmlDoc, ref sOut);
            if (bRet)
            {
                XmlNode dataNode = GetNodeFromPath(xmlDoc.DocumentElement, "output");
                strState = dataNode.SelectNodes("tradestate")[0].InnerText;
                strRes = "返回状态：" + strState;
                return 0;
            }
            else
            {
                strRes = sOut;
                return -1;
            }
        }


        public static int SS_RefundmentDivide_Injury(string sTradeNO, string strName, ref string strRes)
        {
            // OutpatientClass m_hObj = new OutpatientClass();
            SS_DivideResult info = new SS_DivideResult();
            String sOut = null;
           
            log.Info(sOut);
            XmlDocument xmlDoc = GetXmlDoc(sOut);

            bool bRet = CheckOutputState(xmlDoc, ref sOut);
            if (bRet)
            {
                XmlNode dataNode = GetNodeFromPath(xmlDoc.DocumentElement, "output/tradeinfo");
                info.sTradeNO = dataNode.SelectNodes("tradeno")[0].InnerText;
                info.sFeeNO = dataNode.SelectNodes("feeno")[0].InnerText;
                info.sTradeDate = dataNode.SelectNodes("tradedate")[0].InnerText;
                // info.par_tradedate = dataNode.SelectNodes("tradedate")[0].InnerText;

                strRes = "成功";
                return 0;
            }
            else
            {
                strRes = sOut;
                log.Info(sOut);
                return -1;
            }
        }

        #endregion

        #region 私有方法

        //读取xml串
        private static XmlDocument GetXmlDoc(string sXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sXML);
            return xmlDoc;
        }

        //读取Node
        private static XmlNode GetNodeFromPath(XmlNode oParentNode, string sPath)
        {
            XmlNode tmpNode = oParentNode.SelectNodes(sPath)[0];
            return tmpNode;
        }

        //读取信息
        private static bool CheckOutputState(XmlDocument xmlDoc, ref String strRet)
        {
            string sState = GetNodeFromPath(xmlDoc.DocumentElement, "state").Attributes["success"].InnerText;

            log.Info("读取信息状态" + sState);

            bool bRet = false;
            if (sState.Equals("true"))
            {
                bRet = true;
                strRet = "调用返回状态：成功";
            }
            else
            {
                bRet = false;
                strRet = "调用返回状态：失败";
            }
            //读取错误信息
            XmlNodeList errNodes = GetNodeFromPath(xmlDoc.DocumentElement, "state").SelectNodes("error");
            for (int i = 0; i < errNodes.Count; i++)
            {
                if (errNodes[i].Attributes.Count > 0)
                {
                    string sErrNO = errNodes[i].Attributes["no"].InnerText;
                    string sErrMsg = errNodes[i].Attributes["info"].InnerText;
                    strRet = "调用返回错误：编号 [" + sErrNO + "] -- 描述 [" + sErrMsg + "]";
                }
            }

            //读取警告信息
            XmlNodeList warNodes = GetNodeFromPath(xmlDoc.DocumentElement, "state").SelectNodes("warning");
            for (int i = 0; i < warNodes.Count; i++)
            {
                if (warNodes[i].Attributes.Count > 0)
                {
                    string sWarNO = warNodes[i].Attributes["no"].InnerText;
                    string sWarMsg = warNodes[i].Attributes["info"].InnerText;
                    strRet = "调用返回警告：编号 [" + sWarNO + "] -- 描述 [" + sWarMsg + "]";
                }
            }

            //读取信息
            XmlNodeList infNodes = GetNodeFromPath(xmlDoc.DocumentElement, "state").SelectNodes("information");
            for (int i = 0; i < infNodes.Count; i++)
            {
                if (infNodes[i].Attributes.Count > 0)
                {
                    string sInfNO = infNodes[i].Attributes["no"].InnerText;
                    string sInfMsg = infNodes[i].Attributes["info"].InnerText;
                    strRet = "调用返回信息：编号 [" + sInfNO + "] -- 描述 [" + sInfMsg + "]";
                }
            }
            return bRet;
        }

        //截取根节点里的所有节点
        private static void genNode(string str)
        {
            List<int> lisint = new List<int>();
            char[] a = str.ToCharArray();
            for (int ii = 0; ii < a.Length; ii++)
            {
                if (a[ii].ToString() == ">")
                    lisint.Add(ii);
            }
            str = str.Substring(lisint[0] + 1);
            log.Info("截取后结果" + str);
        }

        #endregion
    }
}
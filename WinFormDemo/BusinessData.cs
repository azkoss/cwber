namespace WinFormDemo
{
    public static class BusinessData
    {
        static public int ylqd = -1;
        static public string ErrorData = "";
        static public string message = "";
        /*
         * 交易种类：
         * kaika:开卡
         * charge:充值（现金/银行卡）
         * pay:缴费
         * register:挂号
        */
        static public string business_type = "";

        /*
         * 交易金额，包括开卡和充值，不包括缴费业务（缴费业务会根据用户所选的缴费项目自动计算金额）
         * 格式8,2（最大长度10位，包括2为小数位，必须含有2位小数位）
        */
        static public string trans_money = "";

        //开卡
        public struct issue
        {
            static public string mobile = "";//手机号码    
            static public string error = "";//错误信息

            static public string contact_moble = "";//联系人手机号码
            static public string contact_id = "";//联系人身份证号码
        }

        //充值
        public struct charge
        {
            /*充值类型
            0601 现金充值
            0602 转账充值
            0603 退费充值
            0604 充值冲正
            */
            static public string type = "";

            //static public string money = "";
            static public string result = "";
        }
        //缴费
        public struct pay
        {
            static public string item_no_list = "";
            static public float total_money = 0;
            static public string item_money = "";//用户选择的缴费项目及其金额
        }

        public struct his
        {
            static public string send_info = "";
        }

        public struct ID
        {
            static public string name = "";//姓名
            static public string sex = "";//性别
            static public string nation = "";//民族
            static public string Birthday = "";//生日
            static public string adress = "";//地址
            static public string id = "";//身份证号
            static public string organ = "";//发证地址
            static public string start = "";//有效期起始
            static public string end = "";//有效期结束
            static public string bmpfilename = "";
        }

        public struct bank_card
        {
            static public int money;//12位金额
            static public string orgno = "";//6位凭证号(流水号)
        }

        public struct print
        {
            static public bool no_paper = false;
        }
        public struct cash
        {
            static public bool cash_full = false;
        }
        public struct card
        {
            static public bool no_card = false;
        }

        public struct jyt_card_status
        {
            static public bool is_locked = false;//是否锁卡
            static public bool is_losted = false;//是否挂失
        }
    }
}

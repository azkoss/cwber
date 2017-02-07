using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WinFormDemo
{
    public class Struce
    {
        public struct JZITME
        {
            public String strItemAmount;//数量
            public String strItemPrices;//单价
            public String strItemUnits;//单位
            public String strItemCosts;//应收费用
            public String strItemOrderNo;//处方号
            public String strItemDetailName;//明细名称 
        }

        public struct JZINFO : INotifyPropertyChanged
        {
            public String strAdmRowId;//记录编号
            public String strPayMoney;//交易金额
            public String strAdmDept;//就诊科室
            public String strAdmDate;//就诊时间
            public String strTimes;//就诊时间
            public List<JZITME> strAdmItme;//就诊细目
            public bool strAdmCheck;

            public string AdmRowid
            {
                get { return strAdmRowId; }
                set { this.strAdmRowId = value; OnPropertyChanged("AdmRowid"); }
            }
            public string Paymoney
            {
                get { return strPayMoney; }
                set { this.strPayMoney = value; OnPropertyChanged("Paymoney"); }
            }
            public string Admdept
            {
                get { return strAdmDept; }
                set { this.strAdmDept = value; OnPropertyChanged("Admdept"); }
            }
            public string Admdate
            {
                get { return strAdmDate; }
                set { this.strAdmDate = value; OnPropertyChanged("Admdate"); }
            }
            public List<JZITME> Admitme
            {
                get { return strAdmItme; }
                set { this.strAdmItme = value; OnPropertyChanged("Admitme"); }
            }
            public bool Admcheck
            {
                get { return strAdmCheck; }
                set { this.strAdmCheck = value; OnPropertyChanged("Admcheck"); }
            }
            public string Times
            {
                get { return strTimes; }
                set { this.strTimes = value; OnPropertyChanged("Times"); }
            }
            public event PropertyChangedEventHandler PropertyChanged;

            void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }
    }
}

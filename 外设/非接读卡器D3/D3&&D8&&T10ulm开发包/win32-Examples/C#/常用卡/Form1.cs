using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WFTest
{
    public partial class Form1 : Form
    {
        [DllImport("dcrf32.dll")]
        public static extern int dc_init(Int16 port, Int32 baud);  //初试化
        [DllImport("dcrf32.dll")]
        public static extern short dc_exit(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_beep(int icdev,ushort misc);
        [DllImport("dcrf32.dll")]
        public static extern short dc_reset(int icdev, uint sec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_config_card(int icdev, byte cardType);
        [DllImport("dcrf32.dll")]

        public static extern short dc_card(int icdev,byte model,ref ulong snr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_card_double(int icdev, byte model, [Out] byte[] snr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_card_double_hex(int icdev, byte model,[Out]char[] snr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_reset(int icdev,ref byte rlen,[Out] byte[] recvbuff);
        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_command(int icdev, byte slen, byte[] sendbuff, ref byte rlen,
                                                     [Out]byte[] recvbuff, byte timeout);
        [DllImport("dcrf32.dll")]
        public static extern short dc_card_b(int icdev, [Out] byte[] atqb);


        [DllImport("dcrf32.dll")]
        public static extern short dc_setcpu(int icdev, byte address);
        [DllImport("dcrf32.dll")]
        public static extern short dc_cpureset(int icdev, ref byte rlen, byte[] rdata);
        [DllImport("dcrf32.dll")]
        public static extern short dc_cpuapdu(int icdev,byte slen, byte[] sendbuff, ref byte rlen,
                                                     [Out]byte[] recvbuff); 

        [DllImport("dcrf32.dll")]
        public static extern short dc_readpincount_4442(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_read_4442(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_4442(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_verifypin_4442(int icdev,byte[] password);
        [DllImport("dcrf32.dll")]
        public static extern short dc_readpin_4442(int icdev,byte[] password);
        [DllImport("dcrf32.dll")]
        public static extern short dc_changepin_4442(int icdev,byte[] password);

       [DllImport("dcrf32.dll")]
        public static extern short dc_readpincount_4428(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_read_4428(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_4428(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_verifypin_4428(int icdev,byte[] password);
        [DllImport("dcrf32.dll")]
        public static extern short dc_readpin_4428(int icdev,byte[] password);
        [DllImport("dcrf32.dll")]
        public static extern short dc_changepin_4428(int icdev,byte[] password);

        [DllImport("dcrf32.dll")]
        public static extern int dc_authentication(int icdev, int _Mode, int _SecNr);

        [DllImport("dcrf32.dll")]
        public static extern int dc_authentication_pass(int icdev, int _Mode, int _SecNr, byte[] nkey);

        [DllImport("dcrf32.dll")]
        public static extern int dc_authentication_pass_hex(int icdev, int _Mode, int _SecNr, string nkey);

        [DllImport("dcrf32.dll")]
        public static extern int dc_load_key(int icdev, int mode, int secnr, byte[] nkey);  //密码装载到读写模块中


        [DllImport("dcrf32.dll")]
        public static extern int dc_write(int icdev, int adr, [In] byte[] sdata);  //向卡中写入数据

        [DllImport("dcrf32.dll")]
        public static extern int dc_write_hex(int icdev, int adr, [In] string sdata);  //向卡中写入数据
    

        [DllImport("dcrf32.dll")]
        public static extern int dc_read(int icdev, int adr, [Out] byte[] sdata);  //从卡中读数据

        [DllImport("dcrf32.dll")]
        public static extern short dc_read_24c(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_24c(int icdev, Int16 offset, Int16 lenth, byte[] buffer);

        [DllImport("dcrf32.dll")]
        public static extern short dc_read_24c64(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_24c64(int icdev, Int16 offset, Int16 lenth, byte[] buffer);


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
          //  string str = System.Environment.CurrentDirectory;
            int icdev;
            int st;
            char[] ssnr = new char[128];
            byte rlen=0; 
            byte[] rbuff;
            listBox1.Items.Clear();
 
            icdev = dc_init(100, 115200);
            if (icdev < 0)
            {
                listBox1.Items.Add("init error");
                return;
            }
            listBox1.Items.Add("dc init ok");

            dc_config_card(icdev,0x41);
            dc_reset(icdev, 10);

            st = dc_card_double_hex(icdev, 0, ssnr);
            if (st != 0)
            {
                dc_exit(icdev);
                listBox1.Items.Add("dc_card error ");
                return;
            }

            listBox1.Items.Add("dc_card ok");
            listBox1.Items.Add("card id : " + new string(ssnr));

            rbuff = new byte[128];

            st = dc_pro_reset(icdev, ref rlen, rbuff);
            if (st != 0)
            {
                listBox1.Items.Add("pro reset error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("pro reset ok");
            listBox1.Items.Add("reset information " + byteToChar(rlen, rbuff));
                
            StringBuilder temp1 = new StringBuilder(64);
            rbuff = new byte[128];
            st = dc_pro_command(icdev, (byte)5, new byte[] { 0, (byte)0x84, 0, 0, 8 }, ref rlen, rbuff, (byte)7);
            if (st != 0)
            {
                listBox1.Items.Add("pro command error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("get a random number ok");
            listBox1.Items.Add("data is "+(byteToChar(rlen,rbuff)));

            dc_beep(icdev, 10);
            dc_exit(icdev);
        }
        public static String byteToChar(int length,byte[] data)
        {
            StringBuilder stringbuiler = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                String temp = data[i].ToString("x");
                if (temp.Length == 1)
                {
                    stringbuiler.Append("0" + temp);
                }
                else
                {
                    stringbuiler.Append(temp);
                }
            }
            return (stringbuiler.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int icdev;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff;
            listBox1.Items.Clear();
 
            icdev = dc_init(100, 115200);
            if (icdev < 0)
            {
                listBox1.Items.Add("init error");
                return;
            }
            listBox1.Items.Add("dc init ok");

            st = dc_config_card(icdev, 0x42);
            st = dc_reset(icdev, 5);

            st = dc_card_b(icdev,  snr);
            if (st != 0)
            {
                listBox1.Items.Add("dc_card_b error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("dc_card ok");


            rbuff = new byte[128];

            StringBuilder temp1 = new StringBuilder(64);
            rbuff = new byte[128];
            st = dc_pro_command(icdev, (byte)5, new byte[] { 0, (byte)0x84, 0, 0, 8 }, ref rlen, rbuff, (byte)7);
            if (st != 0)
            {
                listBox1.Items.Add("pro command error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("get a random number ok");
            listBox1.Items.Add("data is " + byteToChar(rlen, rbuff));

            dc_beep(icdev, 10);
            dc_exit(icdev);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int icdev;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            listBox1.Items.Clear();
            icdev = dc_init(100, 115200);
            if (icdev < 0)
            {
                listBox1.Items.Add("dc init error");
                return;
            }
            listBox1.Items.Add("dc init ok");

            st = dc_setcpu(icdev, 0x0c); 
            if (st != 0) 
            {
                listBox1.Items.Add("dc set cpu error");
                dc_exit(icdev); 
                return; 
            }
            listBox1.Items.Add("dc set cpu ok");

            st = dc_cpureset(icdev, ref rlen, snr);
            if (st != 0)
            {
                listBox1.Items.Add("dc cpureset error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("dc cpureset ok");
            listBox1.Items.Add("reset information is " + byteToChar(rlen, snr));

            st = dc_cpuapdu(icdev, 5, new byte[] { 0, (byte)0x84, 0, 0, 8 }, ref rlen, rbuff);
            if (st != 0)
            {
                listBox1.Items.Add("dc cpuapdu error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("get a random number ok");
            listBox1.Items.Add("data is " + byteToChar(rlen, rbuff));

            dc_beep(icdev, 10);
            dc_exit(icdev);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int icdev;
            int st;
            byte[] snr = new byte[128];
            byte[] recv_buffer = new byte[128];

            listBox1.Items.Clear();
            listBox1.Items.Add("dc_init ...");
            icdev = dc_init(100, 115200);
            if (icdev < 0)
            {
                listBox1.Items.Add("dc_init error");
                return;
            }
            listBox1.Items.Add("dc_init ok");

            listBox1.Items.Add("get the pin count...");
            st = dc_readpincount_4442(icdev);
            if (st < 0)
            {
                listBox1.Items.Add("get the pin count error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("get the pin count ok");
            listBox1.Items.Add("the pin count is "+st);

            listBox1.Items.Add("check the pin ffffff ...");
            st = dc_verifypin_4442(icdev, new byte[] { (byte)0xff, (byte)0xff, (byte)0xff });
            if (st != 0)
            {
                listBox1.Items.Add("check the pin error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("check  the pin ok");

            listBox1.Items.Add("change the pin to ffffff");
            st = dc_changepin_4442(icdev, new byte[] { (byte)0xff, (byte)0xff, (byte)0xff });
            if (st != 0)
            {
                listBox1.Items.Add("change the pin error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("change the pin ok");

            listBox1.Items.Add("read the pin ...");
            byte[] temp =new byte[3];
            st = dc_readpin_4442(icdev, temp);
            if (st != 0)
            {
                listBox1.Items.Add("read the pin error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("read the pin ok");
            listBox1.Items.Add("the pin is " + byteToChar(3, temp));

                   listBox1.Items.Add("read data from 0xf5 to 0xfA");
        st = dc_read_4442(icdev, (short) 0xf5, (short) 6, recv_buffer);
        if (st != 0) {
            listBox1.Items.Add(" error");
            dc_exit(icdev);
            return;
        }
        listBox1.Items.Add(" ok");
        listBox1.Items.Add("data : " + byteToChar(6, recv_buffer));
       

        listBox1.Items.Add("write data from 0xf5 to 0x1A again as '123456789abc");
        st = dc_write_4442(icdev, (short) 0xf5, (short) 6, new byte[]{0x12, 0x34, 0x56, (byte) 0x78, (byte) 0x9a, (byte) 0xbc});
        if (st != 0) {
            listBox1.Items.Add(" error");
            listBox1.Items.Add(" error code " + st);
            dc_exit(icdev);
            return;
        }
        listBox1.Items.Add(" ok");

        listBox1.Items.Add("read data from 0xf5 to 0xfA again");
        st = dc_read_4442(icdev, (short) 0xf5, (short) 6, recv_buffer);
        if (st != 0) {
            listBox1.Items.Add(" error");
            dc_exit(icdev);
            return;
        }
        listBox1.Items.Add(" ok");
        listBox1.Items.Add("data : "+ byteToChar(6, recv_buffer));

        listBox1.Items.Add("write data from 0xf5 to 0x1A  as 'ffffffffffff");
        st = dc_write_4442(icdev, (short) 0xf5, (short) 6, new byte[]{(byte) 0xff, (byte) 0xff, (byte) 0xff, (byte) 0xff, (byte) 0xff, (byte) 0xff});
        if (st != 0) {
            listBox1.Items.Add(" error");
            listBox1.Items.Add(" error code " + st);
            dc_exit(icdev);
            return;
        }
        listBox1.Items.Add(" ok");

        dc_beep(icdev, 10);
        listBox1.Items.Add("dc_exit ... ");
        st = dc_exit(icdev);
        if (st != 0) {
            listBox1.Items.Add("error!");
            return;
        }
        listBox1.Items.Add("ok!");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int icdev;
            int st;
            byte[] snr = new byte[128];
            byte[] recv_buffer = new byte[128];

            listBox1.Items.Clear();
            listBox1.Items.Add("dc_init ...");
            icdev = dc_init(100, 115200);
            if (icdev < 0)
            {
                listBox1.Items.Add("dc_init error");
                return;
            }
            listBox1.Items.Add("dc_init ok");

            listBox1.Items.Add("get the pin count...");
            st = dc_readpincount_4428(icdev);
            if (st < 0)
            {
                listBox1.Items.Add("get the pin count error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("get the pin count ok");
            listBox1.Items.Add("the pin count is " + st);

            listBox1.Items.Add("check the pin ffffff ...");
            st = dc_verifypin_4428(icdev, new byte[] { (byte)0xff, (byte)0xff});
            if (st != 0)
            {
                listBox1.Items.Add("check the pin error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("check  the pin ok");

            listBox1.Items.Add("change the pin to ffff");
            st = dc_changepin_4428(icdev, new byte[] { (byte)0xff, (byte)0xff });
            if (st != 0)
            {
                listBox1.Items.Add("change the pin error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("change the pin ok");

            listBox1.Items.Add("read the pin ...");
            byte[] temp = new byte[2];
            st = dc_readpin_4428(icdev, temp);
            if (st != 0)
            {
                listBox1.Items.Add("read the pin error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("read the pin ok");
            listBox1.Items.Add("the pin is " + byteToChar(2, temp));

            listBox1.Items.Add("read data from 0xf5 to 0xfA");
            st = dc_read_4428(icdev, (short)0xf5, (short)6, recv_buffer);
            if (st != 0)
            {
                listBox1.Items.Add(" error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add(" ok");
            listBox1.Items.Add("data : " + byteToChar(6, recv_buffer));


            listBox1.Items.Add("write data from 0xf5 to 0x1A again as '123456789abc");
            st = dc_write_4428(icdev, (short)0xf5, (short)6, new byte[] { 0x12, 0x34, 0x56, (byte)0x78, (byte)0x9a, (byte)0xbc });
            if (st != 0)
            {
                listBox1.Items.Add(" error");
                listBox1.Items.Add(" error code " + st);
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add(" ok");

            listBox1.Items.Add("read data from 0xf5 to 0xfA again");
            st = dc_read_4428(icdev, (short)0xf5, (short)6, recv_buffer);
            if (st != 0)
            {
                listBox1.Items.Add(" error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add(" ok");
            listBox1.Items.Add("data : " + byteToChar(6, recv_buffer));

            listBox1.Items.Add("write data from 0xf5 to 0x1A  as 'ffffffffffff");
            st = dc_write_4428(icdev, (short)0xf5, (short)6, new byte[] { (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff });
            if (st != 0)
            {
                listBox1.Items.Add(" error");
                listBox1.Items.Add(" error code " + st);
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add(" ok");

            dc_beep(icdev, 10);

            listBox1.Items.Add("dc_exit ... ");
            st = dc_exit(icdev);
            if (st != 0)
            {
                listBox1.Items.Add("error!");
                return;
            }
            listBox1.Items.Add("ok!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int icdev;
            int st;
            byte[] bsnr = new byte[128];
            char[] ssnr = new char[128];
            byte[] recv_buffer = new byte[128];
            StringBuilder sbuilder = new StringBuilder();

            listBox1.Items.Clear();
            icdev = dc_init(100, 115200);
            if (icdev < 0)
            {
                listBox1.Items.Add("dc_init error");
                return;
            }
            listBox1.Items.Add("dc_init ok");

            dc_config_card(icdev, 0x41);

            st = dc_card_double_hex(icdev, 0,ssnr);
            if (st != 0)
            {
                dc_exit(icdev);
                listBox1.Items.Add("dc_card error ");
                return;
            }

            listBox1.Items.Add("dc_card ok");
            listBox1.Items.Add("card id : " + new string(ssnr));


            st = dc_authentication_pass_hex(icdev, 0, 15, "ffffffffffff");
            if (st != 0)
            {
                listBox1.Items.Add("dc_authentication error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("dc_authentication ok");

            listBox1.Items.Add("dc_read...");
            st = dc_read(icdev, 61, recv_buffer);
            if (st != 0)
            {
                listBox1.Items.Add("dc_read error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("dc_read ok");
            listBox1.Items.Add("the data is "+ byteToChar(16,recv_buffer));

            listBox1.Items.Add("dc_write 0000000000000000");

            st = dc_write_hex(icdev, 61, "00000000000000000000000000000000");

            if (st != 0)
            {
                dc_exit(icdev);
                listBox1.Items.Add("dc_write error");
                return;
            }
            listBox1.Items.Add("dc_write ok");

            st = dc_read(icdev, 61, recv_buffer);
            if (st != 0)
            {
                listBox1.Items.Add("dc_read error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("dc_read ok");
            listBox1.Items.Add("the data is " + byteToChar(16, recv_buffer));

            listBox1.Items.Add("dc_write ffffffffffffffff");
            st = dc_write(icdev, 61, new byte[] {
                (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff,
                (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff,
                (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff,
                (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff });
            if (st != 0)
            {
                dc_exit(icdev);
                listBox1.Items.Add("dc_write error");
                return;
            }
            listBox1.Items.Add("dc_write ok");

            dc_beep(icdev, 10);

            listBox1.Items.Add("dc_exit ... ");
            st = dc_exit(icdev);
            if (st != 0)
            {
                listBox1.Items.Add("error!");
                return;
            }
            listBox1.Items.Add("ok!");

        }

        private void button7_Click(object sender, EventArgs e)
        {
            int icdev;
            int st;
            byte[] snr = new byte[128];
            byte[] recv_buffer = new byte[128];

            listBox1.Items.Clear();
            listBox1.Items.Add("dc_init ...");
            icdev = dc_init(100, 115200);
            if (icdev < 0)
            {
                listBox1.Items.Add("dc_init error");
                return;
            }
            listBox1.Items.Add("dc_init ok");

            listBox1.Items.Add("write data from 0x00 to 0x05 again as '123456789abc");
            st = dc_write_24c(icdev, (short)0x00, (short)6, new byte[] { 0x12, 0x34, 0x56, (byte)0x78, (byte)0x9a, (byte)0xbc });
            if (st != 0)
            {
                listBox1.Items.Add(" error");
                listBox1.Items.Add(" error code " + st);
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add(" ok");

            listBox1.Items.Add("read data from 0x00 to 0x05 again");
            st = dc_read_24c(icdev, (short)0x00, (short)6, recv_buffer);
            if (st != 0)
            {
                listBox1.Items.Add(" error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add(" ok");
            listBox1.Items.Add("data : " + byteToChar(6, recv_buffer));

            dc_beep(icdev, 10);

            listBox1.Items.Add("dc_exit ... ");
            st = dc_exit(icdev);
            if (st != 0)
            {
                listBox1.Items.Add("error!");
                return;
            }
            listBox1.Items.Add("ok!");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int icdev;
            int st;
            byte[] snr = new byte[128];
            byte[] recv_buffer = new byte[128];

            listBox1.Items.Clear();
            listBox1.Items.Add("dc_init ...");
            icdev = dc_init(100, 115200);
            if (icdev < 0)
            {
                listBox1.Items.Add("dc_init error");
                return;
            }
            listBox1.Items.Add("dc_init ok");

            listBox1.Items.Add("write data from 0x00 to 0x05 again as '123456789abc");
            st = dc_write_24c64(icdev, (short)0x0, (short)6, new byte[] { 0x12, 0x34, 0x56, (byte)0x78, (byte)0x9a, (byte)0xbc });
            if (st != 0)
            {
                listBox1.Items.Add(" error");
                listBox1.Items.Add(" error code " + st);
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add(" ok");

            listBox1.Items.Add("read data from 0x00 to 0x05 again");
            st = dc_read_24c64(icdev, (short)0x00, (short)6, recv_buffer);
            if (st != 0)
            {
                listBox1.Items.Add(" error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add(" ok");
            listBox1.Items.Add("data : " + byteToChar(6, recv_buffer));

            dc_beep(icdev, 10);

            listBox1.Items.Add("dc_exit ... ");
            st = dc_exit(icdev);
            if (st != 0)
            {
                listBox1.Items.Add("error!");
                return;
            }
            listBox1.Items.Add("ok!");
        }

        private void button9_Click(object sender, EventArgs e)
        {

            int icdev;
            int st;
            ulong snr = 0;
            listBox1.Items.Clear();

            icdev = dc_init(0, 115200);
            if (icdev < 0)
            {
                listBox1.Items.Add("init error");
                return;
            }
            listBox1.Items.Add("dc init ok");

            dc_config_card(icdev, 0x42);
            dc_reset(icdev, 10);

            st = dc_card(icdev, 0, ref snr);
            if (st != 0)
            {
                listBox1.Items.Add("dc card error");
                dc_exit(icdev);
                return;
            }
            listBox1.Items.Add("dc_card ok");
            listBox1.Items.Add("card id is" + (snr.ToString("x")));
        }
 
    }
}

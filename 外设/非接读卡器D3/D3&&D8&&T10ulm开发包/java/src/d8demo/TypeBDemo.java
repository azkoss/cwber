/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package d8demo;

import com.sun.jna.Native;
import java.io.UnsupportedEncodingException;
import javax.print.DocFlavor;

/**
 * type b ¿¨²Ù×÷demo
 *
 * @author decard
 */
public class TypeBDemo {

    public static void print_bytes(byte[] b, int length) {
        for (int i = 0; i < length; ++i) {
            String hex = Integer.toHexString(b[i] & 0xFF);
            if (hex.length() == 1) {
                hex = '0' + hex;
            }
            System.out.print(hex.toUpperCase());
        }
    }

    public static String gbk_bytes_to_string(byte[] data) {
        int i;
        String s = "";

        for (i = 0; i < data.length; ++i) {
            if (data[i] == 0) {
                break;
            }
        }

        byte[] temp = new byte[i];
        System.arraycopy(data, 0, temp, 0, temp.length);

        try {
            s = new String(temp, "GBK");
        } catch (UnsupportedEncodingException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }

        return s;
    }

    public static byte[] string_to_gbk_bytes(String data) {
        int i = 0;
        byte[] s = null;

        try {
            s = data.getBytes("GBK");
            i = s.length;
        } catch (UnsupportedEncodingException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }

        byte[] temp = new byte[i + 1];
        System.arraycopy(s, 0, temp, 0, i);
        temp[i] = 0;

        return temp;
    }

    public static void main(String[] args) {
        Dcrf32_h dcrf32;
        dcrf32 = (Dcrf32_h) Native.loadLibrary("./dcrf32.dll", Dcrf32_h.class);

        int result;
        int handle;

        int[] snr = new int[1];
        byte[] rlen = new byte[1];
        byte[] temp;
        byte[] send_buffer = new byte[2048];
        byte[] recv_buffer = new byte[2048];



        System.out.print("dc_init ... ");
        result = dcrf32.dc_init((short) 100, 115200);
        if (result < 0) {
            System.out.println("error!");
            return;
        }
        System.out.println("ok!");

        handle = result;

        System.out.print("dc_config_card ... ");
        result = dcrf32.dc_config_card(handle, (byte) 0x42);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok!");

        System.out.print("dc request card b ...");
        temp = new byte[128];
        result = dcrf32.dc_card_b(handle, temp);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok");
        System.out.print("atqb : ");
        print_bytes(temp, 12);
        System.out.println("");

        System.out.print("dc_pro_command ... ");
        send_buffer[0] = (byte) 0x00;
        send_buffer[1] = (byte) 0x84;
        send_buffer[2] = (byte) 0x00;
        send_buffer[3] = (byte) 0x00;
        send_buffer[4] = (byte) 0x08;
        result = dcrf32.dc_pro_command(handle, (byte) 5, send_buffer, rlen, recv_buffer, (byte) 7);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok!");

        System.out.print("DATA: ");
        print_bytes(recv_buffer, rlen[0] & 0xFF);
        System.out.println();

        System.out.print("dc_beep ... ");
        result = dcrf32.dc_beep(handle, (short) 10);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok!");

        System.out.print("dc_exit ... ");
        result = dcrf32.dc_exit(handle);
        if (result != 0) {
            System.out.println("error!");
            return;
        }
        System.out.println("ok!");

    }
}

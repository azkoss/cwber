/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package d8demo;

import com.sun.jna.Native;

/**
 * m1卡操作demo
 * @author decard
 */
public class M1Demo {

    public static void print_bytes(byte[] b, int length) {
        for (int i = 0; i < length; ++i) {
            String hex = Integer.toHexString(b[i] & 0xFF);
            if (hex.length() == 1) {
                hex = '0' + hex;
            }
            System.out.print(hex.toUpperCase());
        }
    }

    public static void main(String[] args) {

        Dcrf32_h dcrf32;
        dcrf32 = (Dcrf32_h) Native.loadLibrary("./dcrf32.dll", Dcrf32_h.class);

        int result;
        int handle;
        int[] snr = new int[1];
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

        System.out.print("dc_card ... ");
        result = dcrf32.dc_config_card(handle, (byte) 0x41);//设置非接卡型为A
        result = dcrf32.dc_card(handle, (byte) 0, snr);
        if (result != 0) {
            System.out.println(result);
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok!");

        recv_buffer[0] = (byte) ((snr[0] >>> 24) & 0xff);
        recv_buffer[1] = (byte) ((snr[0] >>> 16) & 0xff);
        recv_buffer[2] = (byte) ((snr[0] >>> 8) & 0xff);
        recv_buffer[3] = (byte) ((snr[0] >>> 0) & 0xff);
        print_bytes(recv_buffer, 4);
        System.out.println("");

        byte[] pass = new byte[6];
        pass[0] = (byte) 0xff;
        pass[1] = (byte) 0xff;
        pass[2] = (byte) 0xff;
        pass[3] = (byte) 0xff;
        pass[4] = (byte) 0xff;
        pass[5] = (byte) 0xff;

        System.out.print("dc_load_key ... ");
        result = dcrf32.dc_load_key(handle, (byte) 0, (byte) 0, pass);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok!");

        System.out.print("dc_authentication ... ");
        result = dcrf32.dc_authentication(handle, (byte) 0, (byte) 0);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok!");

        System.out.print("dc_write ... ");
        send_buffer[0] = (byte) 0x41;
        send_buffer[1] = (byte) 0x42;
        send_buffer[2] = (byte) 0x43;
        send_buffer[3] = (byte) 0x44;
        send_buffer[4] = (byte) 0x45;
        send_buffer[5] = (byte) 0x46;
        send_buffer[6] = (byte) 0x47;
        send_buffer[7] = (byte) 0x48;
        send_buffer[8] = (byte) 0x49;
        send_buffer[9] = (byte) 0x4A;
        send_buffer[10] = (byte) 0x4B;
        send_buffer[11] = (byte) 0x4C;
        send_buffer[12] = (byte) 0x4D;
        send_buffer[13] = (byte) 0x4E;
        send_buffer[14] = (byte) 0x4F;
        send_buffer[15] = (byte) 0x50;
        result = dcrf32.dc_write(handle, (byte) 2, send_buffer);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok!");

        System.out.print("dc_read ... ");
        result = dcrf32.dc_read(handle, (byte) 2, recv_buffer);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok!");

        print_bytes(recv_buffer, 16);
        System.out.println("");

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

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package d8demo;

import com.sun.jna.Native;

/**
 * 4442¿¨²Ù×÷demo
 * @author Administrator
 */
public class D8_4442_Demo {

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
        byte[] rlen = new byte[1];
        byte[] recv_buffer = new byte[2048];

        System.out.print("dc_init ... ");
        result = dcrf32.dc_init((short) 100, 115200);
        if (result < 0) {
            System.out.println("error!");
            return;
        }
        System.out.println("ok!");

        handle = result;

        System.out.print("get pin count ...");
        result = dcrf32.dc_readpincount_4442(handle);
        if (result < 0) {
            System.out.println(" error");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println(" ok");

        System.out.println("pin count :" + result);

        System.out.print("varifi the pin ");
        result = dcrf32.dc_verifypin_4442(handle, new byte[]{(byte) 0xff, (byte) 0xff, (byte) 0xff});
        if (result != 0) {
            System.out.println(" error");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println(" ok");

        System.out.print("change pin to 0xffffff");
        result = dcrf32.dc_changepin_4442(handle, new byte[]{(byte) 0xff, (byte) 0xff, (byte) 0xff});
        if (result != 0) {
            System.out.println(" error");
        }
        System.out.println(" ok");

        System.out.print(" read the pin ");
        recv_buffer = new byte[1024];
        result = dcrf32.dc_readpin_4442(handle, recv_buffer);
        if (result != 0) {
            System.out.println(" error");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok");

        System.out.print(" the pin is ");
        print_bytes(recv_buffer, 3);
        System.out.println("");

        System.out.print("read data from 0xf5 to 0xfa");
        result = dcrf32.dc_read_4442(handle, (short) 0xf5, (short) 6, recv_buffer);
        if (result != 0) {
            System.out.println(" error");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println(" ok");
        System.out.print("data : ");
        print_bytes(recv_buffer, 6);
        System.out.println("");

        System.out.print("write data from 0xf5 to 0xfa again as '123456789abc");
        result = dcrf32.dc_write_4442(handle, (short) 0xf5, (short) 6, new byte[]{0x12, 0x34, 0x56, (byte) 0x78, (byte) 0x9a, (byte) 0xbc});
        if (result != 0) {
            System.out.println(" error");
            System.out.println(" error code " + result);
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println(" ok");

        System.out.print("read data from 0xf5 to 0xfa again");
        result = dcrf32.dc_read_4442(handle, (short) 0xf5, (short) 6, recv_buffer);
        if (result != 0) {
            System.out.println(" error");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println(" ok");
        System.out.print("data : ");
        print_bytes(recv_buffer, 6);
        System.out.println("");

        System.out.print("write data from 0xf5 to 0xfa  as 'ffffffffffff");
        result = dcrf32.dc_write_4442(handle, (short) 0xf5, (short) 6, new byte[]{(byte) 0xff, (byte) 0xff, (byte) 0xff, (byte) 0xff, (byte) 0xff, (byte) 0xff});
        if (result != 0) {
            System.out.println(" error");
            System.out.println(" error code " + result);
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println(" ok");

        result = dcrf32.dc_beep(handle, (short) 10);
        
        System.out.print("dc_exit ... ");
        result = dcrf32.dc_exit(handle);
        if (result != 0) {
            System.out.println("error!");
            return;
        }
        System.out.println("ok!");
    }
}

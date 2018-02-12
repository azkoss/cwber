/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package d8demo;

import com.sun.jna.Native;

/**
 *  ´ó¿¨×ù²Ù×÷demo
 * @author decard
 */
public class ContactCpuDemo {

    /**
     * @param b
     * @param length
     */
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
        byte[] rlen = new byte[1];
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

        System.out.print("set cpu address ... ");
        result = dcrf32.dc_setcpu(handle, (byte) 0x0c);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok");

        System.out.print("cpu reset ... ");
        result = dcrf32.dc_cpureset(handle, rlen, recv_buffer);
        if (result != 0) {
            System.out.println("error!");
            dcrf32.dc_exit(handle);
            return;
        }
        System.out.println("ok");
        System.out.print("reset info :");
        print_bytes(recv_buffer, rlen[0] & 0xFF);
        System.out.println();

        System.out.print("dc_pro_command ... ");
        send_buffer[0] = (byte) 0x00;
        send_buffer[1] = (byte) 0x84;
        send_buffer[2] = (byte) 0x00;
        send_buffer[3] = (byte) 0x00;
        send_buffer[4] = (byte) 0x08;
        result = dcrf32.dc_cpuapdu(handle, (byte) 5, send_buffer, rlen, recv_buffer);
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

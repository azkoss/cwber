package d8demo;

import com.sun.jna.Native;

public class D8_24C01_Demo {

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

		int st, icdev;
		int status = -1;
		byte[] rdata = new byte[1024];

		icdev = dcrf32.dc_init((short) 100, 115200);
		if (icdev <= 0) {
			System.out.println("dc init error");
			return;
		}
		System.out.println("dc init ok");

		st = dcrf32.dc_setcpu(icdev, (byte) 12);

		st = dcrf32.dc_write_24c(icdev, (short) 0, (short) 8, new byte[] {
				0x12, 0x34, 0x56, 0x78, (byte) 0x90, (byte) 0xab, (byte) 0xcd,
				(byte) 0xef });
		if (st != 0) {
			System.out.println("write data error");
			dcrf32.dc_exit(icdev);
			return;
		}
		System.out
				.println("write data from byte 0 to 7 as 1234567890abcdef ok");
		st = dcrf32.dc_read_24c(icdev, (short) 0, (short) 8, rdata);
		if (st != 0) {
			System.out.println("read data error");
			dcrf32.dc_exit(icdev);
			return;
		}
		System.out.println("read data ok");

		D8_24C01_Demo.print_bytes(rdata, 8);
		
		st = dcrf32.dc_beep(icdev, (short) 10);
		
		dcrf32.dc_exit(icdev);
	}
}

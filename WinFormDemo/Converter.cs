﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormDemo
{
    class Converter
    {
        public byte convert(int i)
        {
            if (i <= 100)
            {
                switch (i)
                {
                    case 0:
                        return 0x00;
                    case 1:
                        return 0x01;
                    case 2:
                        return 0x02;
                    case 3:
                        return 0x03;
                    case 4:
                        return 0x04;
                    case 5:
                        return 0x05;
                    case 6:
                        return 0x06;
                    case 7:
                        return 0x07;
                    case 8:
                        return 0x08;
                    case 9:
                        return 0x09;
                    case 10:
                        return 0x10;
                    case 11:
                        return 0x11;
                    case 12:
                        return 0x12;
                    case 13:
                        return 0x13;
                    case 14:
                        return 0x14;
                    case 15:
                        return 0x15;
                    case 16:
                        return 0x16;
                    case 17:
                        return 0x17;
                    case 18:
                        return 0x18;
                    case 19:
                        return 0x19;
                    case 20:
                        return 0x20;
                    case 21:
                        return 0x21;
                    case 22:
                        return 0x22;
                    case 23:
                        return 0x23;
                    case 24:
                        return 0x24;
                    case 25:
                        return 0x25;
                    case 26:
                        return 0x26;
                    case 27:
                        return 0x27;
                    case 28:
                        return 0x28;
                    case 29:
                        return 0x29;
                    case 30:
                        return 0x30;
                    case 31:
                        return 0x31;
                    case 32:
                        return 0x32;
                    case 33:
                        return 0x33;
                    case 34:
                        return 0x34;
                    case 35:
                        return 0x35;
                    case 36:
                        return 0x36;
                    case 37:
                        return 0x37;
                    case 38:
                        return 0x38;
                    case 39:
                        return 0x39;
                    case 40:
                        return 0x40;
                    case 41:
                        return 0x41;
                    case 42:
                        return 0x42;
                    case 43:
                        return 0x43;
                    case 44:
                        return 0x44;
                    case 45:
                        return 0x45;
                    case 46:
                        return 0x46;
                    case 47:
                        return 0x47;
                    case 48:
                        return 0x48;
                    case 49:
                        return 0x49;
                    case 50:
                        return 0x50;
                    case 51:
                        return 0x51;
                    case 52:
                        return 0x52;
                    case 53:
                        return 0x53;
                    case 54:
                        return 0x54;
                    case 55:
                        return 0x55;
                    case 56:
                        return 0x56;
                    case 57:
                        return 0x57;
                    case 58:
                        return 0x58;
                    case 59:
                        return 0x59;
                    case 60:
                        return 0x60;
                    case 61:
                        return 0x61;
                    case 62:
                        return 0x62;
                    case 63:
                        return 0x63;
                    case 64:
                        return 0x64;
                    case 65:
                        return 0x65;
                    case 66:
                        return 0x66;
                    case 67:
                        return 0x67;
                    case 68:
                        return 0x68;
                    case 69:
                        return 0x69;
                    case 70:
                        return 0x70;
                    case 71:
                        return 0x71;
                    case 72:
                        return 0x72;
                    case 73:
                        return 0x73;
                    case 74:
                        return 0x74;
                    case 75:
                        return 0x75;
                    case 76:
                        return 0x76;
                    case 77:
                        return 0x77;
                    case 78:
                        return 0x78;
                    case 79:
                        return 0x79;
                    case 80:
                        return 0x80;
                    case 81:
                        return 0x81;
                    case 82:
                        return 0x82;
                    case 83:
                        return 0x83;
                    case 84:
                        return 0x84;
                    case 85:
                        return 0x85;
                    case 86:
                        return 0x86;
                    case 87:
                        return 0x87;
                    case 88:
                        return 0x88;
                    case 89:
                        return 0x89;
                    case 90:
                        return 0x90;
                    case 91:
                        return 0x91;
                    case 92:
                        return 0x92;
                    case 93:
                        return 0x93;
                    case 94:
                        return 0x94;
                    case 95:
                        return 0x95;
                    case 96:
                        return 0x96;
                    case 97:
                        return 0x97;
                    case 98:
                        return 0x98;
                    case 99:
                        return 0x99;
                    case 100:
                        return 0x1c;
                }
            }
            return 0xff;
        }
    }
}
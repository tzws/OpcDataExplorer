using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;


namespace ObjectListViewDemo
{
    [ComVisible(true), StructLayout(LayoutKind.Sequential, Pack = 2)]
    class DUMMY_VARIANT
    {
        [DllImport("oleaut32.dll")]
        public static extern void VariantInit(IntPtr addrofvariant);

        [DllImport("oleaut32.dll")]
        public static extern int VariantClear(IntPtr addrofvariant);

        public static string VarEnumToString(VarEnum vevt)
        {
            string strvt = "";
            short vtshort = (short)vevt;
            if (vtshort == VT_ILLEGAL)
                return "VT_ILLEGAL";

            if ((vtshort & VT_ARRAY) != 0)
                strvt += "VT_ARRAY | ";

            if ((vtshort & VT_BYREF) != 0)
                strvt += "VT_BYREF | ";

            if ((vtshort & VT_VECTOR) != 0)
                strvt += "VT_VECTOR | ";

            VarEnum vtbase = (VarEnum)(vtshort & VT_TYPEMASK);
            strvt += vtbase.ToString();
            return strvt;
        }

        public static short VT_TYPEMASK = 0x0fff;
        public static short VT_VECTOR = 0x1000;
        public static short VT_ARRAY = 0x2000;
        public static short VT_BYREF = 0x4000;
        public static short VT_ILLEGAL = unchecked((short)0xffff);


        public static int ConstSize = 16;

        public short vt;
        public short r1;
        public short r2;
        public short r3;
        public int v1;
        public int v2;
    }
}

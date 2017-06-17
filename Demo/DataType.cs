using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectListViewDemo
{
    public enum DataType : byte
    {
        None = 0x00,
        Bool = 0x01,
        Byte = 0x02,
        Int16 = 0x03,
        Int32 = 0x04,
        Int64 = 0x05,
        UInt16 = 0x07,
        UInt32 = 0x08,
        UInt64 = 0x09,
        Real32 = 0x0A,
        Real64 = 0x0B,
        String = 0x0C,
        UDT = 0x0D,
        TorqueEvent = 0x0E,
        TPMEvent = 0x0F,
        BalancerEvent = 0x1,
        PassFailEvent = 0x11,
        JobData = 0x12,
        StationInterlock = 0x13,
        DateTime = 0x14,
        Word = 0x15,
        Double = 0x16, 
        DoubleArray = 0x17,
        StringArray = 0x18,
        BooleanArray = 0x19
    }
}

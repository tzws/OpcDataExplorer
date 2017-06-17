using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


using OPCAutomation;

namespace ObjectListViewDemo
{
    public class OPCProperty
    {
        public int PropertyID;
        public string Description;
        public VarEnum DataType;



        public override string ToString()
        {
            return "ID:" + PropertyID + " '" + Description + "' T:" + DUMMY_VARIANT.VarEnumToString(DataType);
        }
    }
}

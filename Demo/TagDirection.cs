using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectListViewDemo
{
    [Flags]
    public enum TagDirection : ushort
    {
        Input = 1,
        Output = 2,
        InputOutput = 3
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectListViewDemo
{
 
    public delegate void TagEventHandler(object sender, TagEventArgs args);

    public class TagEventArgs : EventArgs
    {
        public OPCTag Tag;
        public TagEventArgs(OPCTag tag)
        {
            Tag = tag;
        }
    }
}

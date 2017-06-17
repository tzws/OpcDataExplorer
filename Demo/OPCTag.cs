using System;
using System.Collections.Generic;

using System.Text;


using System.Runtime.InteropServices;

using OPCAutomation;
using System.Globalization;

namespace ObjectListViewDemo
{
    public class OPCTag
    {

        public int id { get; set; }
        public int parent_id { get; set; }
        public int root_id { get; set; }
        public string parent_fullname { get; set; }
        public string Name { get; set; }


        public OPCGroup OPCGroup;
        public OPCItem OPCItem;



		// 这个其实就是di、do点，或者ai、ao点
        protected TagDirection mDirection;
        public TagDirection Direction
        {
            get { return mDirection; }
            set { mDirection = value; }
        }

		// Datatype 也是需要设置的
        protected DataType mDataType;
        public DataType DataType
        {
            get { return mDataType; }
            set { mDataType = value; }
        }

		// Datatype 的 string 名字
        protected string mDataTypeText;
        public string DataTypeText
        {
            get { if (isRealTag) return mDataTypeText; else return null; }
            set { mDataTypeText = value;  } 
		}

		// Value
        private object mValue;
        public object Value
        {
            get { if (isRealTag) return GetValue(null); else return null; }
            set { SetValue(null, value); }
        }

		protected OPCQuality _quality;
        public OPCQuality Quality 
		{
			get;
			set;
		}

		protected string  _qualityText;
        public string QualityText 
		{
            get { 
				if (isRealTag) 
				{
					
					// 处理 OPCQualityGood 和 OPCQualityMask 的 Value 一样的问题 
					if (Quality == OPCQuality.OPCQualityGood)
						return "GOOD";
						//return Enum.GetName(typeof(OPCQuality), OPCQuality.OPCQualityGood);
					else if (Quality == OPCQuality.OPCQualityBad)
						return "BAD";
					else if (Quality == OPCQuality.OPCQualityUncertain)
						return "Uncertain";

					else 
						return Enum.GetName(typeof(OPCQuality), Quality);

					//else if (Quality == OPCQuality.OPCQualityMask)
						//return "Mask";

				}
				else 
					return null; 
			}
			set  { _qualityText = value; }
		}


		protected DateTime mItemTimeStamp;
        public DateTime ItemTimeStamp  
		{
            get;
			set;
		}

		public int ServerHandle { get; set; }
		public int ClientHandle { get; set; }

		protected string mItemTimeStampText;
        public string ItemTimeStampText  
		{
            get { 
				if (isRealTag) 
				{
					//return mItemTimeStampText;
					return ItemTimeStamp.ToLocalTime().ToString("yyy-MM-dd HH:mm:ss.fff");
					//return ItemTimeStamp.ToString("yyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
				}
				else return null; }
			set  { mItemTimeStampText = value; }
		}


		// 这个就是TimeStamp吗？
		private int _updateTime = 1000;
        public int UpdateTime
        {
            get { return _updateTime; }
			set { _updateTime = value; }
        }

		// 更新次数
		protected long _updateCount = 0;
		public long UpdateCount
        {
            get { return _updateCount; }
			set  { _updateCount = value; }
        }

		protected string _updateCountText;
		public string UpdateCountText
        {
            get { 
				if (isRealTag) 
					return UpdateCount.ToString(); 
				else 
					return null; 
			}
			set  { _updateCountText = value; }
        }

		public bool isRealTag { get; set; }


        #region Events
        public event TagEventHandler TagChanged;
        public event TagEventHandler TagRead;
        public event TagEventHandler TagValueUpdate;
        #endregion

		public bool isLeaf {get; set; }
		public bool isFirstUpdate {get; set; }

        public object Data { get; set; } // the Data (Specify Binding as such {Binding Data.Field})

        public List<OPCTag> Children { get; set; }
        public OPCTag Parent { get; set; }

        public string FullName
        {
            get { return (parent_fullname != null) ? parent_fullname + "." + Name : Name; }
        }

		public string groupName { get; set; }

        private int _level = -1;
        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    _level = (Parent != null) ? Parent.Level + 1 : 0;
                }
                return _level;
            }
        }


        ///// <summary>
        ///// Only used for output tags.  First set OverrideValue, then set this OverrideTagValue to true
        ///// When done overrideing the value, simply set OverrideTagValue to false;
        ///// </summary>
        //public bool UseOverrideValue;
        //public object OverrideValue;

        public object GetValue(object accessor)
        {
            //if (UseOverrideValue == true) {
                //return OverrideValue;
            //} else {

				// 这其实只是处理事件而已，不改变 mValue 
                OnTagRead(accessor);
                return mValue;
            //}

        }

        public void SetValue(object accessor, object value)
        {
            if (mValue != null && mValue.Equals(value)) return;
            mValue = value;
			// 这其实只是处理事件而已，不改变 mValue 
            OnTagChanged(accessor);
            OnTagValueUpdate(accessor);
        }


        #region Event Triggers
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Fires the TagRead event
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void OnTagRead(object sender)
        {
            if (TagRead != null) TagRead(sender, new TagEventArgs(this));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Fires the TagChanged event
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void OnTagChanged(object sender)
        {
            if (TagChanged != null) TagChanged(sender, new TagEventArgs(this));
        }

        protected virtual void OnTagValueUpdate(object sender)
        {
			// sender 在什么情况下是null？
			// 使用 tag.Value = xxxx; 的情况下，而不是 tag.SetValue(); 的情况下
			if (sender != null)
				if (TagValueUpdate != null) TagValueUpdate(sender, new TagEventArgs(this));
        }

        #endregion

        public bool HasChildren { get { return Children.Count > 0; } }

        //public OPCTag() 
		//{ 
			//Children = new List<OPCTag>(); 
		//}

        public void AddChild(OPCTag t)
        {
            t.Parent = this;
            Children.Add(t);
        }

		public OPCTag (int a, int b, int c, string p, string n)
		{
			this.id = a;
			this.parent_id = b;
			this.root_id = c;
			this.parent_fullname = p;
			this.Name = n;
			Children = new List<OPCTag>(); 
			Quality = OPCQuality.OPCQualityBad;
			this.isFirstUpdate = true;
			DataTypeText = "VT_EMPTY";

			this.Direction = TagDirection.Output;
		}

		public OPCTag (string p, string n)
		{
			this.parent_fullname = p;
			this.Name = n;
			Children = new List<OPCTag>(); 
			Quality = OPCQuality.OPCQualityBad;
			this.isFirstUpdate = true;
			DataTypeText = "VT_EMPTY";

			this.Direction = TagDirection.Output;
		}

		public OPCTag (string n)
		{
			this.Name = n;
			Children = new List<OPCTag>(); 
			Quality = OPCQuality.OPCQualityBad;
			this.isFirstUpdate = true;
			DataTypeText = "VT_EMPTY";

			this.Direction = TagDirection.Output;
		}

		public string ToString1()
		{
			return "id is: " + id + ", parent_id is: " + parent_id + ", root_id is: " + root_id  + ", parent_fullname is: " + parent_fullname +  ", name is: " + Name;
		}

    }
}

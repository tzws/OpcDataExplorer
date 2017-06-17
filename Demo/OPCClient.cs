using System;
using System.Collections.Generic;
using System.Text;
using OPCAutomation;

using System.Net;
using System.Threading;
using System.Runtime.InteropServices;


//Interop.OPCAutomation.dll must be a reference in the project 

namespace ObjectListViewDemo
{

    //This class communicates with an OPC server through Tags.
    //public class OPCClient : IDevice, IDynamicTagDevice
    public class OPCClient
    {
        #region Fields

        private string mID = "OPC";

        private OPCAutomation.OPCServer mServer;
		private OPCBrowser oPCBrowser;
		private long _itemCounts = 0;

		public long ItemCounts
		{
			get { return _itemCounts; }
			set { _itemCounts = value; }
		}


        private string mOPCServerName;
        private string mOPCServerIP;
        protected bool mInitialized;

        private Dictionary<int, OPCTag> mAcyncWriteTags = new Dictionary<int, OPCTag>();
        private Dictionary<int, OPCTag> mTagsByClientHandle = new Dictionary<int, OPCTag>();
        //private Dictionary<int, OPCTag> mTagsByServerHandle = new Dictionary<int, OPCTag>();

        private Dictionary<string, OPCTag> mTagsByName = new Dictionary<string, OPCTag>();        
        private Dictionary<string, OPCGroup> mGroups = new Dictionary<string, OPCGroup>();
        private Dictionary<OPCGroup, List<OPCTag> > mGroupTagsMap = new Dictionary<OPCGroup, List<OPCTag>  >();


        public event EventHandler AddTagsComplete;
        public event EventHandler GetAllTagNamesComplete;

		private delegate bool AddTagsDelegate(SortedDictionary<string, OPCTag> opcTags, string groupName = null, bool removeFormer = false);



		/// <summary>
		/// 服务器端句柄
		/// </summary>
		Array itemServerHandles;
		public Array ItemServerHandles 
		{ 
			get { return itemServerHandles; } 
			set { itemServerHandles = value; } 
		}


        private int mLastError;

        private object mAsyncTransIDLock;
        private int mLastTransID = 0;            
        #endregion

        #region Properties

		public string _defaultGroupName = "PowerOpcExpolorerGroup";
		public string DefaultGroupName 
		{ 
            get { return _defaultGroupName; }
			set { _defaultGroupName = value; }
		}

		private string _delimiter = ".";
		public string Delimiter 
		{ 
            get { return _delimiter; }
			set { _delimiter = value; }
		}



		// 可能把 client和server的handle也暴露出来，也比较好
		// remove 也可以批量
		// AddItems，还有这种，可以批量加的函数
		// 可以先测试 quality
		// if (quality == OPCAutomation.OPCQuality.OPCQualityGood)
		// connect 函数，可以 加个IP，这就能连远程计算机了
		//
		//
		// ObjOPCItems.AddItems(NoOfItems, Array_Items,Array_ClientHanlers, _ gpServerHandlers, gsErrors)
		//
		// ObjOPCItem.Read(OPCAutomation.OPCDataSource.OPCDevice)
		// OPCItem bItem = OpcItems.GetOPCItem(itmHandleServer);
		// 这两个函数有啥区别？OpcItems.GetOPCItem 是数据已经拿到本地了？ ObjOPCItem.Read 是再从server端读取？


		// 还可以注册一个事件，每当  OnGroupDataChange 完成之后，通知 opcclient  的调用者
		// 这样的话，grid 就可以自动刷新了，不用每次点击之后再刷新了。

		// 添加多个group有什么用？多个group的控制，是放在OPCClient里好，还是外面好？
		// 这个还需要对OPC 协议 深入的理解
		// 在OPC 服务器来看，OPCItems 是 用 OPCGroup 来管理的。添加Item，也是用Group来添加的。


		// 这个OPCClient，最好单独的封在一个线程里，这样主线程不会卡死。
		// 调用OPCClient 的，需要一个回调函数，用来显示 loading 
		//

		// 需要一个 OPCProperty class，来封装、返回property.

		// Connect 函数，如果已经连接了，应该返回什么？而不是再连一遍 
		
		private OPCGroup defaultGroup;

        public ServerInformation ServerInfo  { get; set; }

        public string ID
        {
            get { return mID; }
            set { mID = value; }
        }

        public bool Connected
        {
            get { 
				
				//return (mServer != null) ? mServer.ServerState == 1 : false; 
				
				try {
					if (mServer != null)
					{
						if (mServer.ServerState == 1)
						{
							return true;
						}
					}
					return false;
					
				} catch (Exception ex)
				{
					return false;
				}
			}
        }

        public bool Initialized
        {
            get { return mInitialized; }
        }

        public OPCTag[] Tags
        {
            get
            {
                List<OPCTag> tags = new List<OPCTag>();
                foreach (KeyValuePair<string, OPCTag> tag in mTagsByName) {
                    tags.Add(tag.Value);
                }
                return tags.ToArray();
            }
            set
            {
                RemoveAllItems();
                foreach (OPCTag tag in value) {
                    AddTag(tag);
                }
            }
        }

        public OPCTag this[string name]
        {
            get
            {
                if (mTagsByName.ContainsKey(name)) {
                    return mTagsByName[name];
                } else {
                    return null;
                }
            }
        }
        #endregion

        #region Constructor/Dispose
        public OPCClient()
        {
            mAsyncTransIDLock = new object();
            mInitialized = false;
        }

        public void Dispose()
        {
            if (mServer != null) {
                Disconnect();
            }
        }
        #endregion
        
        #region Initialization/Connection        
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the OPC Client
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        public void Initialize(string OPCServerName, string serverIp = "127.0.0.1")
        {
            mOPCServerName = OPCServerName;
			mOPCServerIP = serverIp;
            mInitialized = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the OPC Client
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        public void Initialize(string[] args)
        {
            Initialize(args[0]);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The OPCServer Object provides a method called 'Connect' that allows you
        /// to 'connect' with an OPC server.  The 'Connect' method can take two arguments,
        /// a server name and a Node name.  The Node name is optional and does not have to
        // be used to connect to a local server.  When the 'Connect' method is called you
        /// should see the OPC Server application start if it is not aleady running.
        ///
        ///Special Note: When connect remotely to another PC running the KepserverEx make
        ///sure that you have properly configured DCOM on both PC's. You will find documentation
        ///explaining exactly how to do this on your installation CD or at the Kepware web site.
        ///The web site is www.kepware.com.
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        public bool Connect()
        {
            if (!mInitialized) return false;

			// 如果已经Connect了，就直接返回
			if (Connected ) return true;

            try {
                mServer = new OPCServer();
                mServer.Connect(mOPCServerName, mOPCServerIP);
                //mServer.Connect(mOPCServerName, "");

                if (mServer.ServerState != (int)OPCServerState.OPCRunning)
                {
					return false;
                }

				// 创建 OPCBrowser
				oPCBrowser = mServer.CreateBrowser();

				GetDelimiter();


				long DefaultGroupTimeBias = mServer.OPCGroups.DefaultGroupTimeBias;
				Console.WriteLine("DefaultGroupTimeBias is: " + DefaultGroupTimeBias );
				//mServer.OPCGroups.DefaultGroupTimeBias = (8 * 60);
				mServer.OPCGroups.DefaultGroupTimeBias = (int)-TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes;

				DefaultGroupTimeBias = mServer.OPCGroups.DefaultGroupTimeBias;
				Console.WriteLine("DefaultGroupTimeBias is: " + DefaultGroupTimeBias );


				bool result = CreateGroup(DefaultGroupName);
				if (!result)
				{
					return false;
				}

				defaultGroup = mGroups[DefaultGroupName];
				if (defaultGroup == null)
				{
					return false;
				}

				this.ServerInfo = new ServerInformation();
				this.ServerInfo.MajorVersion = mServer.MajorVersion.ToString();
				this.ServerInfo.MinorVersion = mServer.MinorVersion.ToString();
				this.ServerInfo.BuildNumber = mServer.BuildNumber.ToString();



				//LocaleID 
				Console.WriteLine("LocaleID is: " + mServer.LocaleID);
				OPCGroups g = mServer.OPCGroups;

				foreach (OPCGroup x in g )
				{
					Console.WriteLine("group name is: " + x.Name );
				}

				//string ServerName = mServer.ServerName;
				//Console.WriteLine("ServerName  is: " + ServerName  );

				//string ClientName  = mServer.ClientName ;
				//Console.WriteLine("ClientName   is: " + ClientName  );

                return true;
            } catch (Exception ex) {
                mServer = null;
                return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Disconnects the opc client (Removes all items and groups)
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        public void Disconnect()
        {
            try {
                RemoveAllItems();
                RemoveAllGroups();            
                if ((mServer != null)) {
                    mServer.Disconnect();
                }
            } catch (Exception ex) {
                //Exception Event
				Console.WriteLine("in Disconnect, error is: " + ex.Message);
            } finally {
                mServer = null;
				GC.Collect();
            }
        }
        #endregion



        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a tag to the opc server
        /// </summary>
        /// <param name="tag"></param>
        ///////////////////////////////////////////////////////////////////////////////////////////
        public bool AddTag(OPCTag opcTag, bool readProperty = true, string groupName = null)
        {
            //OPCTag opcTag = new OPCTag(tag);
            if (mTagsByName.ContainsKey(opcTag.Name)) return false;


			// 这里要加 事件处理 ？？
			// 这里是处理 input 点、output点的。
			// 如果设置成output点，当外部修改了这个点的value的时候，就通过事件处理，自动写到server端
			// 先注掉吧。
			
			// 这是是处理 往 server 端写数据的。如果把这个注掉，就没有主动写数据的地方了。
			// 如果在其他地方，要写一个tag的值到server，只需要 tag.Value = xxxx，这样就可以了。
			// 因为绑定了事件，最终会绕到这里这个OPCClient 的  OnTagChanged 方法
			// 这个设计真绕
			if ((opcTag.Direction & TagDirection.Output) == TagDirection.Output) {
				opcTag.TagChanged += new TagEventHandler(OnTagChanged);
			}


            //Add an OPC Group if the update time does not alread exist
            //OPCGroup group = GetGroup(opcTag.UpdateTime);            
			// 这里用 Updatetime 来作为 group 的 Name，好，还是不好？好在：同一个 Updatetime 的在同一个group，更新的话，一起更新。
			// 不好在：不够明确。
			// 而且，这么写，添加 group 成了 添加item 驱动的了。 主动控制性不强
			// 我希望的是，主动性更强一点。那设置一个 defaultGroupName


			if (groupName == null || groupName == "" )
			{
				groupName = DefaultGroupName;
			}

            OPCGroup group = GetGroup(groupName);            


			// 这里应该加一个 try  catch，以防 这个Tag 在 server 端不存在
			try {

				// 这里的AddItem 的第二个参数，应该是什么？
				// 一个数字？这个数字传到server，server数据有变化的时候再传回来，作为一个handle？
				OPCItem item = group.OPCItems.AddItem(opcTag.Name, mTagsByName.Count + 1);
				opcTag.OPCItem = item;
				opcTag.OPCGroup = group;

				if (item != null) {
					//Save the opctag in a dictionary for quick lookup by name or by ClientHandle
					mTagsByName.Add(opcTag.Name, opcTag);
					mTagsByClientHandle.Add(item.ClientHandle, opcTag);                

					opcTag.ClientHandle = item.ClientHandle;
					opcTag.ServerHandle = item.ServerHandle;

                    mGroupTagsMap[group].Add(opcTag);

					//ItemServerHandles.Add(opcTag.OPCItem.ServerHandle);
                    //ItemServerHandles.SetValue
                    //ItemServerHandles.SetValue(opcTag.OPCItem.ServerHandle, ItemServerHandles.Length);
					// 控制是不是添加Tag之后，就读一下Value呢
					if (readProperty)
					{
						if (opcTag.isFirstUpdate)
						{
							ReadPropertyValues(ref opcTag);
							opcTag.isFirstUpdate = false;
						}
					}
					return true;
				} else {
					return false;
				}
			}

			catch(Exception err)
			{
				//没有任何权限的项，都是OPC服务器保留的系统项，此处可不做处理。
				//MessageBox.Show("此项为系统保留项:"+err.Message,"提示信息");
				return false;

				// 这里应该写一个 错误处理 的方法，把每一个error code 对应的 错误信息，反馈给用户
			}
        }

		public void GetItemProperties(string tagName)
		{
			int	count = 4; 

			List<int> propIds = new List<int>();
            propIds.Add(0);
			propIds.Add(1);
            propIds.Add(2);
            propIds.Add(3);
            propIds.Add(4);

			Array PropertyIDs = propIds.ToArray();
			Array PropertyValues;
			Array Errors;

			try{

				mServer.GetItemProperties( tagName, count, ref PropertyIDs, out PropertyValues, out Errors ); 
				if( (count == 0) || (count > 10000) ) 
					return; 

				for( int i = 1; i <= count; i++ ) 
				{

                    if (i == 1)
                    {
                        VarEnum foo = (VarEnum)VarEnum.ToObject(typeof(VarEnum), PropertyValues.GetValue(i));

                        var text = foo.ToString(); // "Clubs"
                        // these methods are clunkier but run 25% faster on my machine
                        var text2 = Enum.GetName(typeof(VarEnum), foo); // "Clubs"
                        //var text3 = typeof(VarEnum).GetEnumName(foo); // "Clubs"
                    }     

                    Console.WriteLine("in GetItemProperties, tagName is : " + tagName + ": DataType is: " + PropertyValues.GetValue(i).GetType() + ": " + PropertyIDs.GetValue(i) + ", " + PropertyValues.GetValue(i).ToString() + ", " + Errors.GetValue(i));

				} 
			
			} catch (Exception ex)
			{
				
			}
		}

		public void ReadPropertyValues(ref OPCTag tag)
		{
			// count 必须比 propIds.Length 少一个 
			int	count = 4; 

			List<int> propIds = new List<int>();
            propIds.Add(0);
			propIds.Add(1);
			propIds.Add(2);
			propIds.Add(3);
			propIds.Add(4);


			Array PropertyIDs = propIds.ToArray();
			Array PropertyValues;
			Array Errors;

			try{

				mServer.GetItemProperties( tag.Name, count, ref PropertyIDs, out PropertyValues, out Errors ); 
				if( (count == 0) || (count > 10000) ) 
					return; 

				for( int i = 1; i <= count; i++ ) 
				{
                    if (i == 1)
                    {
                        VarEnum dataType = (VarEnum)VarEnum.ToObject(typeof(VarEnum), PropertyValues.GetValue(i));

                        //tag.DataType = dataType;

                        //var text = dataType.ToString();

                        // these methods are clunkier but run 25% faster on my machine
                        var text2 = Enum.GetName(typeof(VarEnum), dataType);
                        //var text3 = typeof(VarEnum).GetEnumName(dataType);
                        tag.DataTypeText = text2;
                    } else if ( i == 2)
					{
                        tag.SetValue(this, PropertyValues.GetValue(i));

                    } else if ( i == 3)
					{
                        tag.Quality = (OPCQuality)PropertyValues.GetValue(i);
                    } else if ( i == 4)
					{
                        tag.ItemTimeStamp = (DateTime)PropertyValues.GetValue(i);
					}
					break;
				} 
			
			} catch (Exception ex)
			{
				
			}
		}

		public void QueryAvailableProperties(string tagName)
		{
			int	count = 0; 

			Array PropertyIDs;
			Array Descriptions;
			Array DataTypes;

			try{

				mServer.QueryAvailableProperties( tagName, out count, out PropertyIDs, out Descriptions, out DataTypes ); 
				if( (count == 0) || (count > 10000) ) 
					return; 

				for( int i = 1; i <= count; i++ ) 
				{
					Console.WriteLine("in QueryAvailableProperties, tagName is : " + tagName  + ": "+ PropertyIDs.GetValue(i) + ", " + Descriptions.GetValue(i) + ", " + DataTypes.GetValue(i) + " ==>");
				} 
			
			} catch (Exception ex)
			{
				
			}
		}

		private void AddTagsCompleted(IAsyncResult asyncResult)
		{
            Console.WriteLine("in OPCClient, AddTagsCompleted 1, result is: ");
			Console.WriteLine("in OPCClient, AddTagsCompleted, Thread id is: " + System.Threading.Thread.CurrentThread.ManagedThreadId );

			if (asyncResult == null) return;
			string result = (asyncResult.AsyncState as AddTagsDelegate).EndInvoke(asyncResult).ToString();


			Console.WriteLine("in OPCClient, AddTagsCompleted 2, result is: " + result);
			if (AddTagsComplete != null)
			{
				AddTagsComplete	(this, null);
			}
		}


        public bool AddTagsWorker(SortedDictionary<string, OPCTag> opcTags, string groupName = null, bool removeFormer = false)
		{

			Console.WriteLine("in OPCClient, AddTagsWorker 1");
			Console.WriteLine("in OPCClient, AddTagsWorker, Thread id is: " + System.Threading.Thread.CurrentThread.ManagedThreadId );
			if (opcTags.Count == 0 )
				return false;

			if (removeFormer)
			{
				RemoveAllItems();
			}

			if (groupName == null || groupName == "" )
			{
				groupName = DefaultGroupName;
			}

            OPCGroup group = GetGroup(groupName);            


            int[] ClientHandles = new int[opcTags.Count];
            string[] tagNames = new string[opcTags.Count];
            int handle = mTagsByName.Count + 1;

			int counter = 0;
			foreach (KeyValuePair<string, OPCTag> opcTag in opcTags)
			{
				if (mTagsByName.ContainsKey(opcTag.Value.Name))
					continue;

				if ((opcTag.Value.Direction & TagDirection.Output) == TagDirection.Output) {
					opcTag.Value.TagChanged += new TagEventHandler(OnTagChanged);
				}

				ClientHandles[counter] = handle;
				tagNames[counter] = opcTag.Value.Name;

				handle++;
				counter++;
 			}

			try {

				Array ar_itemNames = Array.CreateInstance(typeof(string), tagNames.Length + 1);
                ((Array)tagNames).CopyTo(ar_itemNames, 1);

				Array ar_itemClientHandles = Array.CreateInstance(typeof(int), tagNames.Length + 1);
                ((Array)ClientHandles).CopyTo(ar_itemClientHandles, 1);

				Array ar_Errors;

				group.OPCItems.AddItems(ar_itemClientHandles.Length - 1, ref ar_itemNames, ref ar_itemClientHandles, out itemServerHandles, out ar_Errors);
                foreach (object item in ItemServerHandles)
                {
					// 这里 如果 item 是 0 的话，会直接 crash。为什么 ItemServerHandles 会有大量的 0 呢？
					// 判断一下。或者用  nested try catch 
					if (0 != (int)item)
					{
						OPCItem	opcItem = group.OPCItems.GetOPCItem((int)item);

						if (opcItem != null) {
							//Save the opctag in a dictionary for quick lookup by name or by ClientHandle




							// 得到的 opcItem 的 ItemID  是什么？是传进去的Name吗？ 是！
							OPCTag opcTag = opcTags[opcItem.ItemID];

							opcTag.OPCItem = opcItem;
							opcTag.OPCGroup = group;

							mGroupTagsMap[group].Add(opcTag);

							mTagsByName.Add(opcTag.Name, opcTag);
							mTagsByClientHandle.Add(opcItem.ClientHandle, opcTag);                
						}					
					}
				}

				Console.WriteLine("in OPCClient, AddTagsWorker 2");
				return true;
			}
			catch(Exception err)
			{
				Console.WriteLine("in OPCClient, AddTagsWorker 3");
				//没有任何权限的项，都是OPC服务器保留的系统项，此处可不做处理。
				//MessageBox.Show("此项为系统保留项:"+err.Message,"提示信息");
				

				// 这里应该写一个 错误处理 的方法，把每一个error code 对应的 错误信息，反馈给用户
			}
			Console.WriteLine("in OPCClient, AddTagsWorker 4");
            return false;
		}

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Add a batch of tags to the opc server
        /// </summary>
        /// <param name="tags"></param>
        ///////////////////////////////////////////////////////////////////////////////////////////
        public void AddTags1(SortedDictionary<string, OPCTag> opcTags, string groupName = null, bool removeFormer = false)
		{

			Console.WriteLine("in OPCClient, AddTags1, Thread id is: " + System.Threading.Thread.CurrentThread.ManagedThreadId );
			AddTagsDelegate worker = AddTagsWorker;
            IAsyncResult asyncResult = worker.BeginInvoke(opcTags, groupName, removeFormer,  AddTagsCompleted, worker);
		}

        public bool AddTags(SortedDictionary<string, OPCTag> opcTags, string groupName = null, bool removeFormer = false)
		{
			Console.WriteLine("in OPCClient, AddTags 1");
			Console.WriteLine("in OPCClient, AddTags, Thread id is: " + System.Threading.Thread.CurrentThread.ManagedThreadId );
			if (opcTags.Count == 0 )
				return false;

			if (removeFormer)
			{
				RemoveAllItems();
			}

			if (groupName == null || groupName == "" )
			{
				groupName = DefaultGroupName;
			}

            OPCGroup group = GetGroup(groupName);            


            int[] ClientHandles = new int[opcTags.Count];
            string[] tagNames = new string[opcTags.Count];
            int handle = mTagsByName.Count + 1;

			int counter = 0;
			foreach (KeyValuePair<string, OPCTag> opcTag in opcTags)
			{
				if (mTagsByName.ContainsKey(opcTag.Value.Name))
					continue;

				if ((opcTag.Value.Direction & TagDirection.Output) == TagDirection.Output) {
					opcTag.Value.TagChanged += new TagEventHandler(OnTagChanged);
				}

				ClientHandles[counter] = handle;
				tagNames[counter] = opcTag.Value.Name;

				handle++;
				counter++;
 			}

			try {

				Array ar_itemNames = Array.CreateInstance(typeof(string), tagNames.Length + 1);
                ((Array)tagNames).CopyTo(ar_itemNames, 1);

				Array ar_itemClientHandles = Array.CreateInstance(typeof(int), tagNames.Length + 1);
                ((Array)ClientHandles).CopyTo(ar_itemClientHandles, 1);

				Array ar_Errors;

				group.OPCItems.AddItems(ar_itemClientHandles.Length - 1, ref ar_itemNames, ref ar_itemClientHandles, out itemServerHandles, out ar_Errors);
                foreach (object item in ItemServerHandles)
                {
					// 这里 如果 item 是 0 的话，会直接 crash。为什么 ItemServerHandles 会有大量的 0 呢？
					// 判断一下。或者用  nested try catch 
					if (0 != (int)item)
					{
						OPCItem	opcItem = group.OPCItems.GetOPCItem((int)item);

						if (opcItem != null) {
							//Save the opctag in a dictionary for quick lookup by name or by ClientHandle

							// 得到的 opcItem 的 ItemID  是什么？是传进去的Name吗？ 是！
							OPCTag opcTag = opcTags[opcItem.ItemID];

							opcTag.OPCItem = opcItem;
							opcTag.OPCGroup = group;
							mGroupTagsMap[group].Add(opcTag);

							mTagsByName.Add(opcTag.Name, opcTag);
							mTagsByClientHandle.Add(opcItem.ClientHandle, opcTag);                
						}					
					}
				}

				Console.WriteLine("in OPCClient, AddTags 2");
				return true;
			}
			catch(Exception err)
			{
				Console.WriteLine("in OPCClient, AddTags 3");
				//没有任何权限的项，都是OPC服务器保留的系统项，此处可不做处理。
				//MessageBox.Show("此项为系统保留项:"+err.Message,"提示信息");

				// 这里应该写一个 错误处理 的方法，把每一个error code 对应的 错误信息，反馈给用户
			}
			Console.WriteLine("in OPCClient, AddTags 4");
            return false;
		}

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets the opc group with the update rate passed in.  Creates the group if it does not exist.
        /// </summary>
        /// <param name="updateRate"></param>
        /// <returns></returns>
        ///////////////////////////////////////////////////////////////////////////////////////////
        private OPCGroup GetGroup(string groupName)
        {
            if (mGroups.ContainsKey(groupName)) {

                return mGroups[groupName];

            } else {
                try {

                    //OPCGroup group = mServer.OPCGroups.Add(updateRate.ToString());
                    //group.UpdateRate = updateRate;
                    //group.DeadBand = 0;
                    //group.IsActive = true;
                    //group.IsSubscribed = true;
                    //group.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(OnGroupDataChange);
                    //group.AsyncWriteComplete += new DIOPCGroupEvent_AsyncWriteCompleteEventHandler(OnGroupAsyncWriteComplete);
                    //mGroups.Add(updateRate, group);
                    //return group;


					CreateGroup(groupName, 500, 0, true, true);
					return mGroups[groupName];

                } catch (Exception ex) {
                    return null;
                }
            }
        }

		public bool CreateGroup (string groupName, int updateRate = 500, int deadBand = 0, bool isActive = true, bool isSubscribed = true)
		{
            if (mGroups.ContainsKey(groupName)) {
				return true;
			}

			if (String.IsNullOrEmpty(groupName)) 
			{
				// Log, send feedback to user
				return false;
			}

            try
            {
				OPCGroup group = mServer.OPCGroups.Add(groupName);

				long TimeBias = group.TimeBias;
				Console.WriteLine("TimeBias is: " + TimeBias );


				group.UpdateRate = updateRate;
				group.DeadBand = deadBand;
				group.IsActive = isActive;
				group.IsSubscribed = isSubscribed;
				//IsSubscribed是false，即没有订阅，DataChange回调事件不会发生？？

				group.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(OnGroupDataChange);
				group.AsyncWriteComplete += new DIOPCGroupEvent_AsyncWriteCompleteEventHandler(OnGroupAsyncWriteComplete);

				// 为啥不实现 AsyncReadComplete 的事件处理呢？
                //group.AsyncReadComplete += new DIOPCGroupEvent_AsyncReadCompleteEventHandler(GroupAsyncReadComplete);

				mGroups.Add(groupName, group);

				mGroupTagsMap.Add(group, new List<OPCTag>());

            }
            catch (Exception err)
            {
                //MessageBox.Show("创建组出现错误："+err.Message,"提示信息",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return false;
            }
            return true;
		}

        #region Event Handlers
        void OnTagChanged(object sender, TagEventArgs e)
        {
            //If we changed the tag then do not write the tag.
			if (sender != null)
				if (sender.Equals(this)) return;

            try {
                OPCTag tag = mTagsByName[e.Tag.Name];
                AsyncWrite(tag);
            } catch (Exception ex) {
                //Error event
            }
        }

        void OnGroupAsyncWriteComplete(int TransactionID, int NumItems, ref Array ClientHandles, ref Array Errors)
        {
            try {
                for (int i = 1; i <= ClientHandles.Length; i++) {
                    int handle = (int)ClientHandles.GetValue(i);
                    OPCTag tag = null;

                    if (mTagsByClientHandle.TryGetValue(handle, out tag)) {
                        tag.Quality = ((int)Errors.GetValue(i) > 0) ? OPCQuality.OPCQualityBad : OPCQuality.OPCQualityGood;
                    }
                    mAcyncWriteTags.Remove(TransactionID);

					// 这里应该显示日志，或者其他的，可以展示错误的东西
                }
            } catch (Exception ex) {
                
            }
        }

        void OnGroupDataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            try {


				Console.WriteLine("in OPCClient, OnGroupDataChange, Thread id is: " + System.Threading.Thread.CurrentThread.ManagedThreadId );

                for (int i = 1; i <= NumItems; i++) {
                    int handle = (int)ClientHandles.GetValue(i);

                    if (mTagsByClientHandle.ContainsKey(handle)) {
                        OPCTag tag = mTagsByClientHandle[handle];                        


                        tag.SetValue(this, ItemValues.GetValue(i));
                        tag.ItemTimeStamp = (DateTime)TimeStamps.GetValue(i);
                        tag.Quality = (OPCQuality)Qualities.GetValue(i);

                        tag.UpdateCount++;
                    }
                }
            } catch(Exception ex) {
            
            }
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Writes the opc tag value to the server syncronously
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SyncWrite(OPCTag tag)
        {
            try {
                Array serverErrors = default(Array);
                Array serverHandles = new int[2];
                serverHandles.SetValue((int)tag.OPCItem.ServerHandle, 1);
                Array values = GetValue(tag);
                tag.OPCGroup.SyncWrite(1, ref serverHandles, ref values, out serverErrors);
                HandleServerError(serverErrors);                
            } catch (Exception ex) {
                //Excpetion event                
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Writes the opc tag value to the server asyncronously
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void AsyncWrite(OPCTag tag)
        {
            try {
                Array serverErrors = default(Array);
                Array serverHandles = new int[2];
                serverHandles.SetValue((int)tag.OPCItem.ServerHandle, 1);


				// 它的这个根据自己的DataType 来Write 数据的，我可以不必理会
                //Array values = GetValue(tag);


				// 这是网上的一种写法
				//object[] valueTemp = new object[2] {"",tag.Value };
				//Array values=(Array)valueTemp;

				// 这是那个 OPCClient 的写法
				Array values = new object[2];
				values.SetValue(tag.Value, 1);

                int cancelID;
                int transID = GetNextTransID();
                tag.OPCGroup.AsyncWrite(1, ref serverHandles, ref values, out serverErrors, transID, out cancelID);
                mAcyncWriteTags.Add(transID, tag);

                HandleServerError(serverErrors);                
            } catch (Exception ex) {
                //Exception EVent
            }
        }

        #region Private Helper Methods
        ///////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Increments the transaction id and returns it.
        /// </summary>
        /// <returns>The next transaction ID</returns>
        ///////////////////////////////////////////////////////////////////////////////////////////////
        private int GetNextTransID()
        {
            lock (mAsyncTransIDLock) {
                mLastTransID++;
                return mLastTransID;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Converts the tag value to an array for writing to the OPC Server
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        ///////////////////////////////////////////////////////////////////////////////////////////////
        private Array GetValue(OPCTag tag)
        {
            Array values = new object[2];
            switch (tag.DataType) {
                case DataType.Bool:
                    values.SetValue(Convert.ToBoolean(tag.Value), 1);
                    break;
                case DataType.Byte:
                    values.SetValue(Convert.ToByte(tag.Value), 1);
                    break;
                case DataType.Int16:
                    values.SetValue(Convert.ToInt16(tag.Value), 1);
                    break;
                case DataType.Int32:
                    values.SetValue(Convert.ToInt32(tag.Value), 1);
                    break;
                case DataType.Int64:
                    values.SetValue(Convert.ToInt64(tag.Value), 1);
                    break;
                case DataType.UInt16:
                    values.SetValue(Convert.ToUInt16(tag.Value), 1);
                    break;
                case DataType.UInt32:
                    values.SetValue(Convert.ToUInt32(tag.Value), 1);
                    break;
                case DataType.UInt64:
                    values.SetValue(Convert.ToUInt64(tag.Value), 1);
                    break;
                case DataType.Real32:
                    values.SetValue(Convert.ToSingle(tag.Value), 1);
                    break;
                case DataType.Real64:
                    values.SetValue(Convert.ToDouble(tag.Value), 1);
                    break;
                case DataType.String:
                    values.SetValue(Convert.ToString(tag.Value), 1);
                    break;
            }
            return values;
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a array of local opc servers
        /// </summary>
        /// <returns></returns>
        //////////////////////////////////////////////////////////////////////////////////////////////
        public static string[] ListOPCServers(string nodeName = "127.0.0.1")
        {
            List<string> list = new List<string>();
			if (!String.IsNullOrEmpty(nodeName))
			{
				try {
					OPCServer server = new OPCServer();
					object result = server.GetOPCServers(nodeName);
					if (result != null && result is Array) {
						Array servers = result as Array;
						foreach (string s in servers) {
							list.Add(s);
						}
					}				
				} catch (Exception ex)
				{

				}
			}
            return list.ToArray();
        }

        /// <summary>
        /// 枚举本地OPC服务器
        /// </summary>
        public string[] GetLocalServer()
        {
            List<string> list = new List<string>();

			string strHostIP = "";
			string strHostName = "";

            //获取本地计算机IP,计算机名称
            IPHostEntry IPHost = Dns.Resolve(Environment.MachineName);
            if (IPHost.AddressList.Length > 0)
            {
                strHostIP = IPHost.AddressList[0].ToString();
            }
            else
            {
                return list.ToArray();
            }
            //通过IP来获取计算机名称，可用在局域网内
            IPHostEntry ipHostEntry = Dns.GetHostByAddress(strHostIP);
            strHostName = ipHostEntry.HostName.ToString();

			Console.WriteLine("HostName is: " + strHostName);

            //获取本地计算机上的OPCServerName
            try
            {
                OPCServer OpcServer = new OPCServer();
                object result = OpcServer.GetOPCServers(strHostName);

				

				if (result != null && result is Array) {
					Array servers = result as Array;
					foreach (string s in servers) {
						list.Add(s);
					}
				}

				
            }
            catch(Exception err)
            {
				Console.WriteLine("枚举本地OPC服务器出错："+err.Message);
                //MessageBox.Show("枚举本地OPC服务器出错："+err.Message,"提示信息",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            return list.ToArray();
        }

		private void GetDelimiter()
		{

            //OPCNamespaceTypes Organization = (OPCNamespaceTypes)oPCBrowser.Organization;
			//Console.WriteLine("Organization is: " + Organization);

            //if (Organization == OPCNamespaceTypes.OPCHierarchical)
			//{
                //Console.WriteLine("Organization is: Hierarchical");

            //}
            //else if (Organization == OPCNamespaceTypes.OPCFlat)
			//{
                //Console.WriteLine("Organization is: Flat");
			//}

			//string CurrentPosition = oPCBrowser.CurrentPosition;
			//Console.WriteLine("CurrentPosition is: " + CurrentPosition);


            //oPCBrowser.ShowBranches();
			//oPCBrowser.ShowLeafs(true);
			//for (int i = 1; i <= oPCBrowser.Count; i++)
			//{
				//string name = oPCBrowser.Item(i);
				//Console.WriteLine("name is: " + name);
				//if (name != "")
				//{
					//foreach(char c in name){
						//if (IsCharAlphaNumeric(c))
						//{
							//if (c.IsSymbol())
							//{
								//Delimiter = c.ToString();
								//return;
								////break;
							//}
						//}
					//}
				//}
				
			//}



			string root = "";
			string name;

            oPCBrowser.ShowBranches();
			if (oPCBrowser.Count >= 1)
			{
				root = oPCBrowser.Item(1);
				Console.WriteLine("root is: " + root);

				if (root != "")
				{
					oPCBrowser.ShowLeafs(true);
					for (int i = 1; i <= oPCBrowser.Count; i++)
					{
						name = oPCBrowser.Item(i);
						Console.WriteLine("name is: " + name);
						if (name != "")
						{
							if (root != name)
							{

								// pos 应该是0 
								int pos = name.IndexOf(root);
								string delimier = name.Substring(root.Length, 1);
								Console.WriteLine("delimier is: " + delimier);
                                if (!Char.IsLetterOrDigit(System.Convert.ToChar(delimier)))
								{
									Delimiter = delimier;
									return;
									//break;
								}
							} else {

							}
						} else {
							
						}
					}

					//foreach ( string x in oPCBrowser)
					//{

					//}

				} else {
					
				}
			}
		}

        /// <summary>
        /// 列出OPC服务器中所有节点
        /// </summary>
        public List<string> GetAllTagNames()
        {
			//OPCBrowser oPCBrowser = mServer.CreateBrowser();
            //展开分支
            oPCBrowser.ShowBranches();
            //展开叶子
            oPCBrowser.ShowLeafs(true);
			ItemCounts = oPCBrowser.Count;


			List<string> list = new List<string>();
            foreach (object turn in oPCBrowser)
            {
				list.Add((string)turn);
            }

			if ( GetAllTagNamesComplete != null)
			{
				GetAllTagNamesComplete(this, null);
			}
            return list;
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes all OPC groups from the server
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////
        private void RemoveAllGroups()
        {
            RemoveAllItems();
            try {
				if (mServer != null)
				{
					try {

						//foreach (KeyValuePair<string, OPCTag> tag in mTagsByName) {
						foreach (KeyValuePair<string, OPCGroup> group in mGroups)
						{
							group.Value.DataChange -= new DIOPCGroupEvent_DataChangeEventHandler(OnGroupDataChange);
							group.Value.AsyncWriteComplete -= new DIOPCGroupEvent_AsyncWriteCompleteEventHandler(OnGroupAsyncWriteComplete);
							
						}
						
						mServer.OPCGroups.RemoveAll();
						
					} catch (Exception ex)
					{
						Console.WriteLine("in RemoveAllGroups, error is: " + ex.Message);
					}
				}
            } finally {
                mGroups.Clear();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes all OPC Items
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void RemoveAllItems()
        {            

            //foreach (KeyValuePair<string, OPCTag> tag in mTagsByName) {
                //Array serverHandles = new Int32[2];
                //Array serverErrors = default(Array);
                //try {
                    //serverHandles.SetValue(tag.Value.OPCItem.ServerHandle, 1);
                    //tag.Value.OPCGroup.OPCItems.Remove(1, ref serverHandles, out serverErrors);
                //} catch (Exception ex) {
                    ////Exception event;
					//Console.WriteLine("In RemoveAllItems, error: " + ex..Message);
                //}
                //HandleServerError(serverErrors);
            //}


            //if (ItemServerHandles != null)
            //{
            //    if (ItemServerHandles.Count > 0)
            //    {
            //        Array errors;
            //        //Array serverHandle = temp.ToArray();
            //        //移除上一次选择的项
            //        mServer.OPCGroups.Remove(ItemServerHandles.Count, ref ItemServerHandles, out errors);
            //    }
            //}

			try {
				if (mGroupTagsMap != null)
				{
					foreach (KeyValuePair<OPCGroup, List <OPCTag> > groupTag in mGroupTagsMap) 
					{
						OPCGroup group = groupTag.Key;

						List<int> temp = new List<int>();
						temp.Add(0);

						foreach (OPCTag tag in groupTag.Value)
						{
							temp.Add(tag.ServerHandle);
						}

						//注：OPC中以1为数组的索引起始
						if (temp.Count > 0)
						{
							Array errors;
							Array serverHandle = temp.ToArray();
							//移除上一次选择的项
							group.OPCItems.Remove(groupTag.Value.Count, ref serverHandle, out errors);
						}
					}
				}			
			} catch (Exception ex )
			{
				Console.WriteLine("in RemoveAllItems, error is: " + ex.Message);
				
			} finally {
				mTagsByName.Clear();
				mTagsByClientHandle.Clear();
				mGroupTagsMap.Clear();			
			}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Handles parsing error array and setting last errror member
        /// </summary>
        /// <param name="serverErrors"></param>
        ///////////////////////////////////////////////////////////////////////////////////////////////
        private void HandleServerError(Array serverErrors)
        {
			if (serverErrors != null)
			{
				if (serverErrors.Length >= 2) {
					object error = serverErrors.GetValue(1);
					if (error is int) mLastError = (int)error;
				}
			}
        }
 
    }
}

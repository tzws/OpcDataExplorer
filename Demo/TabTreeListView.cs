using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using BrightIdeasSoftware;



//using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Text;
//using System.Windows.Forms;
using System.Net;
//using System.Collections;
using OPCAutomation;

using System.IO;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;

namespace ObjectListViewDemo
{
    public partial class TabTreeListView : OlvDemoTab {

        private byte[] treeListViewViewState;


		DataSet OpcTagSet = new DataSet();
		DataTable OpcTagTable;
		int seqId = 1;

		List<string> allTagNames;
		SortedDictionary<string, OPCTag> keyTagMap = new SortedDictionary<string, OPCTag>();
		SortedDictionary<string, OPCTag> rootTagMap = new SortedDictionary<string, OPCTag>();

		OPCClient opcClient;
		OPCTag lastTag = null;


        private System.Timers.Timer aTimer;
		private bool treeInitialized = false;

		private delegate bool AddTagsDelegate();

		//define a delegate
        private delegate void ShowTagPropertiesDelegate(OPCTag tag);
        private delegate void OnTagValueUpdateDelegate(object sender, TagEventArgs e);
        private delegate void AddTagsCompletedDeleate();

		public string Delimiter { get; set; }


        public TabTreeListView()
        {

            //Console.WriteLine("VarEnum.VT_ARRAY | VarEnum.VT_EMPTY is: " + (VarEnum.VT_ARRAY | VarEnum.VT_VARIANT));
            //Console.WriteLine("typeof(System.Type).GetFields(BindingFlags.Public | BindingFlags.Static) is : " + (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static));

			CultureInfo culture = new CultureInfo("zh-CN"); // Saudi Arabia
			System.Threading.Thread.CurrentThread.CurrentCulture = culture;


			//CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            DllHelper.RegisterDllAndExe();

            this.ListView = treeListView;
            //this.ListView.CellClick += (sender, args) => Debug.WriteLine("CellClicked: {0}", args);

            this.ListView.SelectionChanged += delegate(object sender, EventArgs args) {
				if (this.ListView.SelectedObject != null)
				{
					lastTag = (OPCTag)(this.ListView.SelectedObject);
					ShowTagProperties((OPCTag)( this.ListView.SelectedObject));
				}
            };

			createTagStructure();

			opcClient = new OPCClient();
			opcClient.AddTagsComplete += new EventHandler(OnAddTagsComplete);
			opcClient.GetAllTagNamesComplete += new EventHandler(OnGetAllTagNamesComplete);
			//opcClient.GetLocalServer();

        }

		private void ShowTagProperties(OPCTag tag)
		{
            //OPCTag tag = listView.SelectedObject as OPCTag;
            //OPCTag focusedTag = listView.FocusedObject as OPCTag;

            if (this.TagValueBox.InvokeRequired)
            {
                //instansiate a delegate with the method
                ShowTagPropertiesDelegate myDelegate = new ShowTagPropertiesDelegate(ShowTagPropertiesWorker);
                //Invoke delegate
                this.TagValueBox.Invoke(myDelegate, tag);
			} else {

				if (tag != null && tag.isRealTag)
				{
					if (tag.Value == null)
					{
						this.TagValueBox.Text = "<null>";
					} else {
						this.TagValueBox.Text = tag.Value.ToString();
					}
					this.TagQualityBox.Text = tag.QualityText;
					this.TagTimestampBox.Text = tag.ItemTimeStampText;
				} else if (tag != null && !tag.isRealTag)
				{
					this.TagValueBox.Text = "";
					this.TagQualityBox.Text = "";
					this.TagTimestampBox.Text = "";
				}

				this.WriteValueInput.Text = "";
			}
		}

		private void ShowTagPropertiesWorker(OPCTag tag)
		{
            //OPCTag tag = listView.SelectedObject as OPCTag;
            //OPCTag focusedTag = listView.FocusedObject as OPCTag;

			if (tag != null && tag.isRealTag)
			{
				if (tag.Value == null)
				{
					this.TagValueBox.Text = "<null>";
				} else {
					this.TagValueBox.Text = tag.Value.ToString();
				}
				this.TagQualityBox.Text = tag.QualityText;
				this.TagTimestampBox.Text = tag.ItemTimeStampText;
			} else if (tag != null && !tag.isRealTag)
			{
				this.TagValueBox.Text = "";
				this.TagQualityBox.Text = "";
				this.TagTimestampBox.Text = "";
			}

			this.WriteValueInput.Text = "";
		}


		private void createTagStructure()
		{

			OpcTagTable = OpcTagSet.Tables.Add("OpcTag");

			OpcTagTable.Columns.Add(new DataColumn("id", typeof(int)));
			OpcTagTable.Columns.Add(new DataColumn("parent_id", typeof(int)));
			OpcTagTable.Columns.Add(new DataColumn("root_id", typeof(int)));

			OpcTagTable.Columns.Add(new DataColumn("parent_fullname", typeof(string)));		
			OpcTagTable.Columns.Add(new DataColumn("Name", typeof(string)));		

		}

		private void loadTagToDataSet ()
		{
			// 添加之后，打印出来看看，
			foreach (KeyValuePair<string, OPCTag> p in keyTagMap)
			{
				//Console.WriteLine("{0} = {1}", p.Key, p.Value.ToString1());
				//OpcTagSet.Tables["OpcTag"].Rows.Add(p.Value.id, p.Value.parent_id, p.Value.root_id, p.Value.parent_fullname, p.Value.Name);
				OpcTagTable.Rows.Add(p.Value.id, p.Value.parent_id, p.Value.root_id, p.Value.parent_fullname, p.Value.Name);
 			}

			// Visualize DataSet.
			//Console.WriteLine(OpcTagSet.GetXml());
			// Write to File
			File.WriteAllText("output.xml", OpcTagSet.GetXml());

			//outputTag();
		}

		private void outputTag()
		{
			foreach (KeyValuePair<string, OPCTag> p in rootTagMap)
			{
				Console.WriteLine("{0} = {1}", p.Key, p.Value.ToString1());
				if (p.Value.HasChildren)
				{
					outputTagChildren(p.Value.Children);
				}
 			}
		}

		private void outputTagChildren(List<OPCTag> tags)
		{
			foreach (OPCTag t in tags)
			{
				if (t.HasChildren)
				{
					outputTagChildren(t.Children);
				}
				Console.WriteLine("{0} = {1}", t.Name, t.ToString1());

			}
		}


        /// <summary>
        /// 【按钮】连接ＯＰＣ服务器
        /// </summary>
        private void btnConnLocalServer_Click(object sender, EventArgs e)
        {
            try
            {

				if (this.btnConnServer.Text == "连接")
				{
					if (String.IsNullOrEmpty(this.txtRemoteServerIP.Text)) 
						this.txtRemoteServerIP.Text = "127.0.0.1";

					opcClient.Initialize(cmbServerName.Text, this.txtRemoteServerIP.Text);

					bool result = opcClient.Connect();
					if (!result)
					{
						// 在这里处理连接错误
						MessageBox.Show("不能连接到OPC Server：" + cmbServerName.Text, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					this.btnConnServer.Enabled = false;

					// 启动Timer，去定时坚持是否连接正常
					aTimer = new System.Timers.Timer();
					aTimer.Interval = 1000;

					// Alternate method: create a Timer with an interval argument to the constructor.
					//aTimer = new System.Timers.Timer(2000);

					// Create a timer with a two second interval.
					//aTimer = new System.Timers.Timer(5000);

					// Hook up the Elapsed event for the timer. 
					aTimer.Elapsed += OnTimedEvent;

					// Have the timer fire repeated events (true is the default)
					aTimer.AutoReset = false;

					// Start the timer
					aTimer.Enabled = true;

                    TextOverlay textOverlay = this.treeListView.OverlayText as TextOverlay;
					textOverlay.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
                    textOverlay.TextColor = Color.Firebrick;
                    textOverlay.BackColor = Color.AntiqueWhite;
                    textOverlay.BorderColor = Color.DarkRed;
                    textOverlay.BorderWidth = 4.0f;
                    textOverlay.Font = new Font("宋体", 36);
                    //textOverlay.Rotation = -5;
                    textOverlay.Text = "正在加载Tag...";
					this.treeListView.RefreshOverlays();


					if( !treeInitialized)
					{
						InitializeTree();
					}


					// 添加Tags，AddTags

					//opcClient.AddTags1(keyTagMap);
					AddTagsDelegate worker = AddTagsWorker;
					IAsyncResult asyncResult = worker.BeginInvoke(AddTagsCompleted, worker);
					//this.treeListView.BeginInvoke(new AddTagsDelegate(AddTagsWorker1));

					this.btnConnServer.Text = "断开";
				} else {

					aTimer.Enabled = false;
					aTimer = null;

					Thread.Sleep(1000);
					if (opcClient.Connected == true)
					{
						opcClient.Disconnect();
					}

					// 这样行不行？ 
					// 显然不够，因为还得加 Column 类的
					//this.treeListView = new BrightIdeasSoftware.TreeListView();
                    //this.treeListView.Clear();
                    this.treeListView.ClearObjects();
                    Thread.Sleep(1000);
                    treeInitialized = false;
					keyTagMap.Clear();
					rootTagMap.Clear();
					allTagNames.Clear();
					seqId = 1;

					lastTag = null;
					this.btnConnServer.Text = "连接";
					this.ExpandAllNodes.Enabled = false;
					this.ExpandAllNodes.Text = "全部展开";
				}
            }

            catch (Exception err)
            {
                MessageBox.Show("初始化出错：" + err.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

		private void InitializeTree()
		{
			if( !treeInitialized)
			{
				// Display Serverinfo 
				ServerInformation info = opcClient.ServerInfo;

				Delimiter = opcClient.Delimiter;

				// 拿到所有的 展开的  tag name
				allTagNames = opcClient.GetAllTagNames();
				foreach (string s in allTagNames)
				{
					processName(s);
				}

				// 这一步 应该 没用了。
				//loadTagToDataSet();

				SetupTree();
				Console.WriteLine("in TabTreeListView, calling AddTags, Thread id is: " + System.Threading.Thread.CurrentThread.ManagedThreadId );

				treeInitialized = true;
			}
		}


		// 这里确实有个多线程的问题，不知道到底用到了几个线程，需要解决多线程的问题吗？
		// http://stackoverflow.com/questions/4876414/how-to-prevent-system-timers-timer-from-queuing-for-execution-on-a-thread-pool#
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
		{
			if (opcClient.Connected == false)
			{
				try
				{
					opcClient.Disconnect();
					bool result = opcClient.Connect();
					if (result)
					{
						if (opcClient.Connected != false)
						{

							// 添加Tags，AddTags

							//opcClient.AddTags1(keyTagMap);
							AddTagsDelegate worker = AddTagsWorker;
							IAsyncResult asyncResult = worker.BeginInvoke(AddTagsCompleted, worker);
							//this.treeListView.BeginInvoke(new AddTagsDelegate(AddTagsWorker1));

						} else {

						}
					}
				}
				catch (Exception ex)
				{
					// 这里Catch 一个  System.Threading.ThreadAbortException，看起来已经catch了。
					aTimer.Enabled = true;
				}
			}
			else {
				try {
					Console.WriteLine();

					if (this.treeListView.InvokeRequired) {
						//this.treeListView.Invoke((MethodInvoker) delegate { DoRereshTreeList(); });

					} else {
						
						//this.treeListView.Refresh();
					}
				} catch (Exception ex)
				{
					
				}

			}

			aTimer.Enabled = true;
		}

		private void DoRereshTreeList()
		{
			this.treeListView.Refresh();
		}

        public bool AddTagsWorker()
		{
			Console.WriteLine("in TabTreeListView, AddTagsWorker, Thread id is: " + System.Threading.Thread.CurrentThread.ManagedThreadId );
			//opcClient.AddTags(keyTagMap);

			// 感觉上，一个一个的添加Tag，应该会解决这个问题。
			foreach (KeyValuePair<string, OPCTag> p in keyTagMap)
			{
				if ( p.Value.isRealTag)
				{
					opcClient.AddTag(p.Value);
				}
 			}

            return true;
		}

        public bool AddTagsWorker1()
		{
			Console.WriteLine("in TabTreeListView, AddTagsWorker1, Thread id is: " + System.Threading.Thread.CurrentThread.ManagedThreadId );
			opcClient.AddTags(keyTagMap);
			//opcClient.AddTags1(keyTagMap);

            this.treeListView.Refresh();
			Console.WriteLine("in TabTreeListView, AddTagsWorker1, result is: ");
            return true;
		}

		private void AddTagsCompleted(IAsyncResult asyncResult)
		{
            Console.WriteLine("in TabTreeListView, AddTagsCompleted 1, result is: ");
			Console.WriteLine("in TabTreeListView, AddTagsCompleted, Thread id is: " + System.Threading.Thread.CurrentThread.ManagedThreadId );

			if (asyncResult == null) return;
			string result = (asyncResult.AsyncState as AddTagsDelegate).EndInvoke(asyncResult).ToString();

			// 试试这样会更新 列 的数据吗
            //this.treeListView.Refresh();

            if (this.btnConnServer.InvokeRequired)
            {
                //instansiate a delegate with the method
                AddTagsCompletedDeleate myDelegate = new AddTagsCompletedDeleate(AddTagsCompletedWorker);
                //Invoke delegate
                this.btnConnServer.Invoke(myDelegate);
			} else {
				
				this.btnConnServer.Enabled = true;
				this.treeListView.Refresh();
                //this.treeListView.ExpandAll();
                //this.treeListView.OverlayText
			}

			Console.WriteLine("in TabTreeListView, AddTagsCompleted 2, result is: " + result);
		}

		private void AddTagsCompletedWorker ()
		{
			this.btnConnServer.Enabled = true;
            this.treeListView.Refresh();
            //this.treeListView.ExpandAll();
            //this.treeListView.Refresh();
            //this.treeListView.OverlayText.Text = null;

            this.treeListView.OverlayText.Text = "";
            //this.treeListView.RemoveOverlay(this.treeListView.OverlayText);
			this.treeListView.RefreshOverlays();
			this.ExpandAllNodes.Enabled = true;
			
		}


		private void processName (string tagName)
		{
			//int i = tagName.LastIndexOf('.');
			int i = tagName.LastIndexOf(System.Convert.ToChar(Delimiter));

			string lhs = i < 0 ? tagName : tagName.Substring(0, i),
				   rhs = i < 0 ? "" : tagName.Substring(i + 1);		

			if ( rhs == "")
			{
				// 这是找到了root节点了，那么这个root节点就是 lhs
				addNode(lhs);

			} else {
				// 处理上一层节点
				processName(lhs);

				// 添加本层节点
				addNode(tagName, lhs);
			}
		}

		private void addNode(string name, string parentName = "")
		{
			// 应该先判断一下，这个 Key 在 map 中存在不存在。不存在，就添加
			if (!keyTagMap.ContainsKey(name))
			{
                OPCTag tag;
				if (parentName != "")
				{
					tag = new OPCTag(parentName, name);
					tag.id = seqId;
					if (keyTagMap.ContainsKey(parentName))
					{
						OPCTag parent = keyTagMap[parentName];
						tag.parent_id = parent.id;
						tag.root_id = parent.root_id;
						tag.isLeaf = true;
						if (allTagNames.Contains(name))
						{
							tag.isRealTag = true;
						}
						parent.AddChild(tag);
					}

				} else {
					tag = new OPCTag(name);
					tag.parent_fullname = "root";
					tag.id = seqId;
					tag.parent_id = 0;
					tag.root_id = tag.id;
					tag.isLeaf = false;
					tag.isRealTag = false;

					rootTagMap.Add(name, tag);
				}

				seqId++;

				tag.TagValueUpdate += new TagEventHandler(OnTagValueUpdate);
				keyTagMap.Add(name, tag);

				//Console.WriteLine("Value added for key = \"" + name  + "\"");
			}
		}

		public void OnAddTagsComplete(object sender, EventArgs e)
		{
            if (sender.Equals(opcClient))
			{
				Console.WriteLine("in TabTreeListView::OnAddTagsComplete, AddTags done");
			}
            this.treeListView.Refresh();
		}

		public void OnGetAllTagNamesComplete(object sender, EventArgs e)
		{
            if (sender.Equals(opcClient))
			{
				Console.WriteLine("in TabTreeListView::OnGetAllTagNamesComplete, done");
			}
		}

        public void OnTagValueUpdate(object sender, TagEventArgs e)
		{
			if (this.ListView.InvokeRequired)
			{
				//instansiate a delegate with the method
				OnTagValueUpdateDelegate myDelegate = new OnTagValueUpdateDelegate(OnTagValueUpdateWorker);
				//Invoke delegate
				this.ListView.Invoke(myDelegate, sender, e);
			} else {

				//Console.WriteLine("in OnTagValueUpdate 1");
				// Server端的Value变了，treelistview 的回调函数
				if (sender.Equals(opcClient))
				{
					// 这里的 sender 到底是不是  opcclient 呢？
					try {
						OPCTag tag = keyTagMap[e.Tag.Name];

						if (this.ListView.SelectedObject != null)
						{
							if (tag == (OPCTag)this.ListView.SelectedObject)
								ShowTagProperties(tag);

						} else if (lastTag != null)
						{
							if (tag == lastTag)
								ShowTagProperties(tag);
						}
                        //this.treeListView.Refresh();
                        //this.treeListView.RefreshOverlays();
                        //this.treeListView.RefreshHotItem();
                        this.treeListView.Invalidate();

						//if (tag.isFirstUpdate)
						//{
						//tag.isFirstUpdate = false;

						////opcClient.QueryAvailableProperties(tag.Name);
						////opcClient.GetItemProperties(tag.Name);
						//}

						//Console.WriteLine("in OnTagValueUpdate, Name is: " + e.Tag.Name + ", Value is: " + e.Tag.Value);

						// 这里，应该让 TreeListView 刷新。 能不能做到单独刷新一个 元素呢？
					} catch (Exception ex) {
						//Error event
					}
				}
				//Console.WriteLine("in OnTagValueUpdate 2");
			}

		}

        public void OnTagValueUpdateWorker(object sender, TagEventArgs e)
		{
			//Console.WriteLine("in OnTagValueUpdate 1");
			// Server端的Value变了，treelistview 的回调函数
            if (sender.Equals(opcClient))
			{
				// 这里的 sender 到底是不是  opcclient 呢？
				try {
					OPCTag tag = keyTagMap[e.Tag.Name];

					if (this.ListView.SelectedObject != null)
					{
						if (tag == (OPCTag)this.ListView.SelectedObject)
							ShowTagProperties(tag);
					
					} else if (lastTag != null)
					{
						if (tag == lastTag)
							ShowTagProperties(tag);
					}

					//this.treeListView.Refresh();
					this.treeListView.Invalidate();

					//if (tag.isFirstUpdate)
					//{
						//tag.isFirstUpdate = false;

						////opcClient.QueryAvailableProperties(tag.Name);
						////opcClient.GetItemProperties(tag.Name);
					//}

					//Console.WriteLine("in OnTagValueUpdate, Name is: " + e.Tag.Name + ", Value is: " + e.Tag.Value);

					// 这里，应该让 TreeListView 刷新。 能不能做到单独刷新一个 元素呢？
				} catch (Exception ex) {
					//Error event
				}
			}
			//Console.WriteLine("in OnTagValueUpdate 2");
		}


        protected override void InitializeTab() {

			// 设置选中的行用边框 高显
			Coordinator.ChangeHotItemStyle(this.ListView, 2);

			//HotItemStyle hotItemStyle = new HotItemStyle();
			//hotItemStyle.ForeColor = Color.AliceBlue;
			//hotItemStyle.BackColor = Color.FromArgb(255, 64, 64, 64);
			//this.ListView.HotItemStyle = hotItemStyle;




            // Setup the controls on the tab
            // On other tabs, we add "Vista" hot item style. But that only works when NOT in
            // owner draw mode, and TreeListViews *requires* OwnerDraw. So we can't use Vista style hot item
            // with TreeListView, so we don't give the option
            //this.comboBoxHotItemStyle.SelectedIndex = 0; // None


            this.treeListView.HierarchicalCheckboxes = this.checkBoxHierarchicalCheckboxes.Checked;
            //this.comboBoxExpanders.SelectedIndex = 2; // triangles

            SetupColumns();
            //SetupDragAndDrop();
            //SetupTree();

            // You can change the way the connection lines are drawn by changing the pen
            TreeListView.TreeRenderer renderer = this.treeListView.TreeColumnRenderer;

			renderer.IsShowGlyphs = true;
			renderer.UseTriangles = false;


            renderer.LinePen = new Pen(Color.Firebrick, 0.5f);
            renderer.LinePen.DashStyle = DashStyle.Dot;
        }

        private void SetupDragAndDrop() {

            // Setup the tree so that it can drop and drop.

            // Dropping doesn't do anything, but it does show how it works

            treeListView.IsSimpleDragSource = true;
            treeListView.IsSimpleDropSink = true;

            treeListView.ModelCanDrop += delegate(object sender, ModelDropEventArgs e) {
                e.Effect = DragDropEffects.None;
                if (e.TargetModel == null)
                    return;

                if (e.TargetModel is DirectoryInfo)
                    e.Effect = e.StandardDropActionFromKeys;
                else
                    e.InfoMessage = "Can only drop on directories";
            };

            treeListView.ModelDropped += delegate(object sender, ModelDropEventArgs e) {
                String msg = String.Format("{2} items were dropped on '{1}' as a {0} operation.",
                    e.Effect, ((DirectoryInfo) e.TargetModel).Name, e.SourceModels.Count);
                Coordinator.ShowMessage(msg);
            };
        }

        private void SetupTree() {

            // TreeListView require two delegates:
            // 1. CanExpandGetter - Can a particular model be expanded?
            // 2. ChildrenGetter - Once the CanExpandGetter returns true, ChildrenGetter should return the list of children

            // CanExpandGetter is called very often! It must be very fast.

            this.treeListView.CanExpandGetter = delegate(object x) {

                //return ((MyFileSystemInfo) x).IsDirectory;
				//return !keyTagMap[(string)x].isLeaf;
				return ((OPCTag)x).HasChildren;

            };

            // We just want to get the children of the given directory.
            // This becomes a little complicated when we can't (for whatever reason). We need to report the error 
            // to the user, but we can't just call MessageBox.Show() directly, since that would stall the UI thread
            // leaving the tree in a potentially undefined state (not good). We also don't want to keep trying to
            // get the contents of the given directory if the tree is refreshed. To get around the first problem,
            // we immediately return an empty list of children and use BeginInvoke to show the MessageBox at the 
            // earliest opportunity. We get around the second problem by collapsing the branch again, so it's children
            // will not be fetched when the tree is refreshed. The user could still explicitly unroll it again --
            // that's their problem :)
            this.treeListView.ChildrenGetter = delegate(object x) {
                try {
                    //return ((MyFileSystemInfo) x).GetFileSystemInfos();

                    return ((OPCTag) x).Children;
                }
                catch (UnauthorizedAccessException ex) {
                    this.BeginInvoke((MethodInvoker)delegate() {
                        this.treeListView.Collapse(x);
                        MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
                    });
                    return new ArrayList();
                }
            };

            // Once those two delegates are in place, the TreeListView starts working
            // after setting the Roots property.

            // List all drives as the roots of the tree
            ArrayList roots = new ArrayList();

            //foreach (DriveInfo di in DriveInfo.GetDrives())
            //{
                //if (di.IsReady)
                    //roots.Add(new MyFileSystemInfo(new DirectoryInfo(di.Name)));
            //}


			foreach (KeyValuePair<string, OPCTag> p in rootTagMap)
			{
				//OpcTagTable.Rows.Add(p.Value.id, p.Value.parent_id, p.Value.root_id, p.Value.parent_fullname, p.Value.name);
				Console.WriteLine("{0} = {1}", p.Key, p.Value.ToString1());
				roots.Add((OPCTag)(p.Value) );
 			}

            this.treeListView.Roots = roots;
        }

        private void SetupColumns() {
            // The column setup here is identical to the File Explorer example tab --
            // nothing specific to the TreeListView. 

            // The only difference is that we don't setup anything to do with grouping,
            // since TreeListViews can't show groups.


            //SysImageListHelper helper = new SysImageListHelper(this.treeListView);

            //this.olvColumnName.ImageGetter = delegate(object x) {
                //return helper.GetImageIndex(((MyFileSystemInfo) x).FullName);
            //};



            // Get the size of the file system entity. 
            // Folders and errors are represented as negative numbers

            //this.olvColumnSize.AspectGetter = delegate(object x) {
                //MyFileSystemInfo myFileSystemInfo = (MyFileSystemInfo) x;

                //if (myFileSystemInfo.IsDirectory)
                    //return (long) -1;

                //try {
                    //return myFileSystemInfo.Length;
                //}
                //catch (System.IO.FileNotFoundException) {
                    //// Mono 1.2.6 throws this for hidden files
                    //return (long) -2;
                //}
            //};

            // Show the size of files as GB, MB and KBs. By returning the actual
            // size in the AspectGetter, and doing the conversion in the 
            // AspectToStringConverter, sorting on this column will work off the
            // actual sizes, rather than the formatted string.

            //this.olvColumnSize.AspectToStringConverter = delegate(object x) {
                //long sizeInBytes = (long) x;
                //if (sizeInBytes < 0) // folder or error
                    //return "";
                //return Coordinator.FormatFileSize(sizeInBytes);
            //};

            // Show the system description for this object

            //this.olvColumnFileType.AspectGetter = delegate(object x) {
                //return ShellUtilities.GetFileType(((MyFileSystemInfo) x).FullName);
            //};

            // Show the file attributes for this object
            // A FlagRenderer masks off various values and draws zero or images based 
            // on the presence of individual bits.

            //this.olvColumnAttributes.AspectGetter = delegate(object x) {
                //return ((MyFileSystemInfo) x).Attributes;
            //};
            //FlagRenderer attributesRenderer = new FlagRenderer();
            //attributesRenderer.ImageList = imageListSmall;
            //attributesRenderer.Add(FileAttributes.Archive, "archive");
            //attributesRenderer.Add(FileAttributes.ReadOnly, "readonly");
            //attributesRenderer.Add(FileAttributes.System, "system");
            //attributesRenderer.Add(FileAttributes.Hidden, "hidden");
            //attributesRenderer.Add(FileAttributes.Temporary, "temporary");
            //this.olvColumnAttributes.Renderer = attributesRenderer;

            // Tell the filtering subsystem that the attributes column is a collection of flags

            //this.olvColumnAttributes.ClusteringStrategy = new FlagClusteringStrategy(typeof (FileAttributes));
        }

        #region UI Event handlers

        private void treeListView_ItemActivate(object sender, EventArgs e)
        {
            Object model = this.treeListView.SelectedObject;
            if (model != null)
                this.treeListView.ToggleExpansion(model);
        }

        private void checkBoxHierarchicalCheckboxes_CheckedChanged(object sender, EventArgs e) {
            this.treeListView.HierarchicalCheckboxes = ((CheckBox) sender).Checked;
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            Coordinator.TimedFilter(this.ListView, ((TextBox)sender).Text);
        }

        //private void comboBoxHotItemStyle_SelectedIndexChanged(object sender, EventArgs e)
        //{
            //Coordinator.ChangeHotItemStyle(this.ListView, (ComboBox)sender);
        //}

        //private void buttonRefresh_Click(object sender, EventArgs e)
        //{
            //this.treeListView.RefreshObjects(this.treeListView.SelectedObjects);
        //}

        //private void buttonCheck_Click(object sender, EventArgs e) {
            //this.treeListView.ToggleSelectedRowCheckBoxes();
        //}

        //private void buttonSaveState_Click(object sender, EventArgs e)
        //{
            //// SaveState() returns a byte array that holds the current state of the columns.
            //// For this demo, we just hold onto that value in an instance variable. For your
            //// application, you should persist it some more permanent fashion than this.
            //this.treeListViewViewState = this.treeListView.SaveState();
            //this.buttonRestoreState.Enabled = true;
        //}

        //private void buttonRestoreState_Click(object sender, EventArgs e)
        //{
            //this.treeListView.RestoreState(this.treeListViewViewState);
        //}

        //private void buttonColumns_Click(object sender, EventArgs e)
        //{
            //ColumnSelectionForm form = new ColumnSelectionForm();
            //form.OpenOn(this.treeListView);
        //}

        //private void buttonDisable_Click(object sender, EventArgs e)
        //{
            //bool isControlKeyDown = ((Control.ModifierKeys & Keys.Control) == Keys.Control);
            //if (isControlKeyDown)
                //this.ListView.EnableObjects(this.ListView.DisabledObjects);
            //else
                //this.ListView.DisableObjects(this.ListView.SelectedObjects);
        //}

        #endregion

        //private void comboBoxExpanders_SelectedIndexChanged(object sender, EventArgs e) {
            //TreeListView.TreeRenderer treeColumnRenderer = this.treeListView.TreeColumnRenderer;
            //ComboBox cb = (ComboBox)sender;
            //switch (cb.SelectedIndex)
            //{
                //case 0:
                    //treeColumnRenderer.IsShowGlyphs = false;
                    //break;
                //case 1:
                    //treeColumnRenderer.IsShowGlyphs = true;
                    //treeColumnRenderer.UseTriangles = false;
                    //break;
                //case 2:
                    //treeColumnRenderer.IsShowGlyphs = true;
                    //treeColumnRenderer.UseTriangles = true;
                    //break;
            //}

            //// Cause a redraw so that the changes to the renderer take effect
            //this.treeListView.Refresh();
        //}

        private void WriteValue_Click(object sender, EventArgs e)
        {
			OPCTag tag = this.ListView.SelectedObject as OPCTag;
			if (tag != null)
			{
				tag.Value = this.WriteValueInput.Text;
			} else if (lastTag != null)
			{
				lastTag.Value = this.WriteValueInput.Text;
			}
        }

		public void OnFormClosing()
		{
			try
			{
				aTimer.Enabled = false;
				aTimer = null;

				Thread.Sleep(1000);

				opcClient.Disconnect();
				// 应该再加上处理timer 的
			} catch (Exception ex)
			{
				
			}
		}

        private void ExpandAllNodes_Click(object sender, EventArgs e)
        {
			if ( this.ExpandAllNodes.Text == "全部展开")
			{
				if (opcClient.ItemCounts > 1000)
				{
					if (MessageBox.Show("已加载的Tag数量超过" + opcClient.ItemCounts + "个，过滤的时候会拖慢速度，确认？", "提示信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						this.ExpandAllNodes.Text = "全部关闭";
						this.treeListView.ExpandAll();
					}
				} else {
					
					this.ExpandAllNodes.Text = "全部关闭";
					this.treeListView.ExpandAll();
				}

			} else {
				
				this.ExpandAllNodes.Text = "全部展开";
                this.treeListView.CollapseAll();
			}
        }

        private void ClearFilterButton_Click(object sender, EventArgs e)
        {
            this.textBoxFilter.Text = "";
        }

        private void GetListServer_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.txtRemoteServerIP.Text))
            {
                string[] serverList = OPCClient.ListOPCServers(this.txtRemoteServerIP.Text);

                if (serverList.Length != 0)
                {
                    cmbServerName.Items.Clear();
                    foreach (string turn in (Array)serverList)
                    {
                        cmbServerName.Items.Add(turn);
                    }

                    cmbServerName.SelectedIndex = 0;
                    btnConnServer.Enabled = true;
                }
                else
                {
					MessageBox.Show("不能获取" + this.txtRemoteServerIP.Text + "上的OPC Server列表", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }
    }
}

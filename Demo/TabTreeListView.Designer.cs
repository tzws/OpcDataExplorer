namespace ObjectListViewDemo
{
    partial class TabTreeListView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabTreeListView));
            this.checkBoxHierarchicalCheckboxes = new System.Windows.Forms.CheckBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.ClearFilterButton = new System.Windows.Forms.Button();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.treeListView = new BrightIdeasSoftware.TreeListView();
            this.olvColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnDataType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnValue = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnQuality = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnTimeStamp = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnUpdateCounts = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmbServerName = new System.Windows.Forms.ComboBox();
            this.btnConnServer = new System.Windows.Forms.Button();
            this.txtRemoteServerIP = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TagTimestampBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TagQualityBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TagValueBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.WriteValue = new System.Windows.Forms.Button();
            this.WriteValueInput = new System.Windows.Forms.TextBox();
            this.ExpandAllNodes = new System.Windows.Forms.Button();
            this.GetListServer = new System.Windows.Forms.Button();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxHierarchicalCheckboxes
            // 
            this.checkBoxHierarchicalCheckboxes.AutoSize = true;
            this.checkBoxHierarchicalCheckboxes.Location = new System.Drawing.Point(531, 26);
            this.checkBoxHierarchicalCheckboxes.Name = "checkBoxHierarchicalCheckboxes";
            this.checkBoxHierarchicalCheckboxes.Size = new System.Drawing.Size(84, 16);
            this.checkBoxHierarchicalCheckboxes.TabIndex = 32;
            this.checkBoxHierarchicalCheckboxes.Text = "Checkboxes";
            this.toolTip1.SetToolTip(this.checkBoxHierarchicalCheckboxes, "Show hierarchical checkboxes");
            this.checkBoxHierarchicalCheckboxes.UseVisualStyleBackColor = true;
            this.checkBoxHierarchicalCheckboxes.CheckedChanged += new System.EventHandler(this.checkBoxHierarchicalCheckboxes_CheckedChanged);
            // 
            // groupBox12
            // 
            this.groupBox12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox12.Controls.Add(this.ClearFilterButton);
            this.groupBox12.Controls.Add(this.textBoxFilter);
            this.groupBox12.Location = new System.Drawing.Point(865, 3);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(175, 41);
            this.groupBox12.TabIndex = 30;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "过滤";
            // 
            // ClearFilterButton
            // 
            this.ClearFilterButton.Location = new System.Drawing.Point(114, 17);
            this.ClearFilterButton.Name = "ClearFilterButton";
            this.ClearFilterButton.Size = new System.Drawing.Size(56, 23);
            this.ClearFilterButton.TabIndex = 1;
            this.ClearFilterButton.Text = "清空";
            this.ClearFilterButton.UseVisualStyleBackColor = true;
            this.ClearFilterButton.Click += new System.EventHandler(this.ClearFilterButton_Click);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Location = new System.Drawing.Point(8, 18);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(100, 21);
            this.textBoxFilter.TabIndex = 0;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            // 
            // treeListView
            // 
            this.treeListView.AllColumns.Add(this.olvColumnName);
            this.treeListView.AllColumns.Add(this.olvColumnDataType);
            this.treeListView.AllColumns.Add(this.olvColumnValue);
            this.treeListView.AllColumns.Add(this.olvColumnQuality);
            this.treeListView.AllColumns.Add(this.olvColumnTimeStamp);
            this.treeListView.AllColumns.Add(this.olvColumnUpdateCounts);
            this.treeListView.AllowColumnReorder = true;
            this.treeListView.AllowDrop = true;
            this.treeListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeListView.CellEditUseWholeCell = false;
            this.treeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnName,
            this.olvColumnDataType,
            this.olvColumnValue,
            this.olvColumnQuality,
            this.olvColumnTimeStamp,
            this.olvColumnUpdateCounts});
            this.treeListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListView.HideSelection = false;
            this.treeListView.HighlightBackgroundColor = System.Drawing.Color.Empty;
            this.treeListView.HighlightForegroundColor = System.Drawing.Color.Empty;
            this.treeListView.IsSimpleDragSource = true;
            this.treeListView.IsSimpleDropSink = true;
            this.treeListView.Location = new System.Drawing.Point(8, 50);
            this.treeListView.Name = "treeListView";
            this.treeListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.treeListView.ShowCommandMenuOnRightClick = true;
            this.treeListView.ShowGroups = false;
            this.treeListView.ShowImagesOnSubItems = true;
            this.treeListView.ShowItemToolTips = true;
            this.treeListView.Size = new System.Drawing.Size(851, 433);
            this.treeListView.SmallImageList = this.imageListSmall;
            this.treeListView.TabIndex = 28;
            this.treeListView.UseCompatibleStateImageBehavior = false;
            this.treeListView.UseFilterIndicator = true;
            this.treeListView.UseFiltering = true;
            this.treeListView.UseHotItem = true;
            this.treeListView.View = System.Windows.Forms.View.Details;
            this.treeListView.VirtualMode = true;
            this.treeListView.ItemActivate += new System.EventHandler(this.treeListView_ItemActivate);
            // 
            // olvColumnName
            // 
            this.olvColumnName.AspectName = "Name";
            this.olvColumnName.IsTileViewColumn = true;
            this.olvColumnName.Text = "Name";
            this.olvColumnName.UseInitialLetterForGroup = true;
            this.olvColumnName.Width = 180;
            this.olvColumnName.WordWrap = true;
            // 
            // olvColumnDataType
            // 
            this.olvColumnDataType.AspectName = "DataTypeText";
            this.olvColumnDataType.Text = "DataType";
            this.olvColumnDataType.Width = 131;
            // 
            // olvColumnValue
            // 
            this.olvColumnValue.AspectName = "Value";
            this.olvColumnValue.IsTileViewColumn = true;
            this.olvColumnValue.Sortable = false;
            this.olvColumnValue.Text = "Value";
            this.olvColumnValue.Width = 145;
            // 
            // olvColumnQuality
            // 
            this.olvColumnQuality.AspectName = "QualityText";
            this.olvColumnQuality.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnQuality.Text = "Quality";
            this.olvColumnQuality.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnQuality.Width = 80;
            // 
            // olvColumnTimeStamp
            // 
            this.olvColumnTimeStamp.AspectName = "ItemTimeStampText";
            this.olvColumnTimeStamp.IsTileViewColumn = true;
            this.olvColumnTimeStamp.Text = "TimeStamp";
            this.olvColumnTimeStamp.Width = 148;
            // 
            // olvColumnUpdateCounts
            // 
            this.olvColumnUpdateCounts.AspectName = "UpdateCountText";
            this.olvColumnUpdateCounts.FillsFreeSpace = true;
            this.olvColumnUpdateCounts.IsEditable = false;
            this.olvColumnUpdateCounts.MinimumWidth = 20;
            this.olvColumnUpdateCounts.Text = "Update Counts";
            // 
            // imageListSmall
            // 
            this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmall.Images.SetKeyName(0, "compass");
            this.imageListSmall.Images.SetKeyName(1, "down");
            this.imageListSmall.Images.SetKeyName(2, "user");
            this.imageListSmall.Images.SetKeyName(3, "find");
            this.imageListSmall.Images.SetKeyName(4, "folder");
            this.imageListSmall.Images.SetKeyName(5, "movie");
            this.imageListSmall.Images.SetKeyName(6, "music");
            this.imageListSmall.Images.SetKeyName(7, "no");
            this.imageListSmall.Images.SetKeyName(8, "readonly");
            this.imageListSmall.Images.SetKeyName(9, "public");
            this.imageListSmall.Images.SetKeyName(10, "recycle");
            this.imageListSmall.Images.SetKeyName(11, "spanner");
            this.imageListSmall.Images.SetKeyName(12, "star");
            this.imageListSmall.Images.SetKeyName(13, "tick");
            this.imageListSmall.Images.SetKeyName(14, "archive");
            this.imageListSmall.Images.SetKeyName(15, "system");
            this.imageListSmall.Images.SetKeyName(16, "hidden");
            this.imageListSmall.Images.SetKeyName(17, "temporary");
            // 
            // cmbServerName
            // 
            this.cmbServerName.FormattingEnabled = true;
            this.cmbServerName.Location = new System.Drawing.Point(214, 21);
            this.cmbServerName.Name = "cmbServerName";
            this.cmbServerName.Size = new System.Drawing.Size(121, 20);
            this.cmbServerName.TabIndex = 39;
            // 
            // btnConnServer
            // 
            this.btnConnServer.Location = new System.Drawing.Point(346, 21);
            this.btnConnServer.Name = "btnConnServer";
            this.btnConnServer.Size = new System.Drawing.Size(75, 23);
            this.btnConnServer.TabIndex = 40;
            this.btnConnServer.Text = "连接";
            this.btnConnServer.UseVisualStyleBackColor = true;
            this.btnConnServer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnConnLocalServer_Click);
            // 
            // txtRemoteServerIP
            // 
            this.txtRemoteServerIP.Location = new System.Drawing.Point(8, 20);
            this.txtRemoteServerIP.Name = "txtRemoteServerIP";
            this.txtRemoteServerIP.Size = new System.Drawing.Size(100, 21);
            this.txtRemoteServerIP.TabIndex = 41;
            this.txtRemoteServerIP.Text = "127.0.0.1";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.TagTimestampBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.TagQualityBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.TagValueBox);
            this.groupBox1.Location = new System.Drawing.Point(865, 52);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.groupBox1.Size = new System.Drawing.Size(166, 123);
            this.groupBox1.TabIndex = 42;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前值";
            // 
            // TagTimestampBox
            // 
            this.TagTimestampBox.Location = new System.Drawing.Point(63, 85);
            this.TagTimestampBox.Name = "TagTimestampBox";
            this.TagTimestampBox.ReadOnly = true;
            this.TagTimestampBox.Size = new System.Drawing.Size(100, 21);
            this.TagTimestampBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "时间戳";
            // 
            // TagQualityBox
            // 
            this.TagQualityBox.Location = new System.Drawing.Point(63, 59);
            this.TagQualityBox.Name = "TagQualityBox";
            this.TagQualityBox.ReadOnly = true;
            this.TagQualityBox.Size = new System.Drawing.Size(100, 21);
            this.TagQualityBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "品质";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Tag值";
            // 
            // TagValueBox
            // 
            this.TagValueBox.Enabled = false;
            this.TagValueBox.Location = new System.Drawing.Point(63, 34);
            this.TagValueBox.Name = "TagValueBox";
            this.TagValueBox.ReadOnly = true;
            this.TagValueBox.Size = new System.Drawing.Size(100, 21);
            this.TagValueBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.WriteValue);
            this.groupBox2.Controls.Add(this.WriteValueInput);
            this.groupBox2.Location = new System.Drawing.Point(865, 196);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(166, 94);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "写入值";
            // 
            // WriteValue
            // 
            this.WriteValue.Location = new System.Drawing.Point(95, 51);
            this.WriteValue.Name = "WriteValue";
            this.WriteValue.Size = new System.Drawing.Size(64, 23);
            this.WriteValue.TabIndex = 1;
            this.WriteValue.Text = "写入值";
            this.WriteValue.UseVisualStyleBackColor = true;
            this.WriteValue.Click += new System.EventHandler(this.WriteValue_Click);
            // 
            // WriteValueInput
            // 
            this.WriteValueInput.Location = new System.Drawing.Point(8, 21);
            this.WriteValueInput.Name = "WriteValueInput";
            this.WriteValueInput.Size = new System.Drawing.Size(152, 21);
            this.WriteValueInput.TabIndex = 0;
            // 
            // ExpandAllNodes
            // 
            this.ExpandAllNodes.Enabled = false;
            this.ExpandAllNodes.Location = new System.Drawing.Point(427, 20);
            this.ExpandAllNodes.Name = "ExpandAllNodes";
            this.ExpandAllNodes.Size = new System.Drawing.Size(75, 23);
            this.ExpandAllNodes.TabIndex = 44;
            this.ExpandAllNodes.Text = "全部展开";
            this.ExpandAllNodes.UseVisualStyleBackColor = true;
            this.ExpandAllNodes.Click += new System.EventHandler(this.ExpandAllNodes_Click);
            // 
            // GetListServer
            // 
            this.GetListServer.Location = new System.Drawing.Point(114, 18);
            this.GetListServer.Name = "GetListServer";
            this.GetListServer.Size = new System.Drawing.Size(94, 23);
            this.GetListServer.TabIndex = 45;
            this.GetListServer.Text = "获取Server列表";
            this.GetListServer.UseVisualStyleBackColor = true;
            this.GetListServer.Click += new System.EventHandler(this.GetListServer_Click);
            // 
            // TabTreeListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GetListServer);
            this.Controls.Add(this.ExpandAllNodes);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtRemoteServerIP);
            this.Controls.Add(this.btnConnServer);
            this.Controls.Add(this.cmbServerName);
            this.Controls.Add(this.checkBoxHierarchicalCheckboxes);
            this.Controls.Add(this.groupBox12);
            this.Controls.Add(this.treeListView);
            this.Name = "TabTreeListView";
            this.Size = new System.Drawing.Size(1042, 526);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxHierarchicalCheckboxes;
        //private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.TextBox textBoxFilter;
        //private System.Windows.Forms.Button buttonRefresh;
        //private System.Windows.Forms.Button buttonSaveState;
        //private System.Windows.Forms.Button buttonRestoreState;
        //private System.Windows.Forms.Button buttonColumns;
        private BrightIdeasSoftware.TreeListView treeListView;

        private BrightIdeasSoftware.OLVColumn olvColumnName;

        private BrightIdeasSoftware.OLVColumn olvColumnDataType;
        private BrightIdeasSoftware.OLVColumn olvColumnValue;
        private BrightIdeasSoftware.OLVColumn olvColumnQuality;
        private BrightIdeasSoftware.OLVColumn olvColumnTimeStamp;
        private BrightIdeasSoftware.OLVColumn olvColumnUpdateCounts;

        private System.Windows.Forms.ToolTip toolTip1;
        //private System.Windows.Forms.Label label37;
        //private System.Windows.Forms.ComboBox comboBoxHotItemStyle;
        private System.Windows.Forms.ImageList imageListSmall;
        //private System.Windows.Forms.Button buttonDisable;
        //private System.Windows.Forms.Label label1;
        //private System.Windows.Forms.ComboBox comboBoxExpanders;
        //private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ComboBox cmbServerName;
        private System.Windows.Forms.Button btnConnServer;
        private System.Windows.Forms.TextBox txtRemoteServerIP;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox TagValueBox;
        private System.Windows.Forms.TextBox TagQualityBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TagTimestampBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button WriteValue;
        private System.Windows.Forms.TextBox WriteValueInput;
        private System.Windows.Forms.Button ExpandAllNodes;
        private System.Windows.Forms.Button ClearFilterButton;
        private System.Windows.Forms.Button GetListServer;



    }
}

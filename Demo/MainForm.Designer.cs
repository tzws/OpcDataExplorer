using BrightIdeasSoftware;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ObjectListViewDemo
{
	partial class MainForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
		private void InitializeComponent()
		{
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageTreeListView = new System.Windows.Forms.TabPage();
            this.tabTreeListView1 = new ObjectListViewDemo.TabTreeListView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabControl1.SuspendLayout();
            this.tabPageTreeListView.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(829, 17);
            this.toolStripStatusLabel3.Spring = true;
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageTreeListView);
            this.tabControl1.Location = new System.Drawing.Point(13, 11);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(819, 488);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPageTreeListView
            // 
            this.tabPageTreeListView.Controls.Add(this.tabTreeListView1);
            this.tabPageTreeListView.Location = new System.Drawing.Point(4, 22);
            this.tabPageTreeListView.Name = "tabPageTreeListView";
            this.tabPageTreeListView.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTreeListView.Size = new System.Drawing.Size(811, 462);
            this.tabPageTreeListView.TabIndex = 14;
            this.tabPageTreeListView.Text = "OpcDataExplorer";
            this.tabPageTreeListView.UseVisualStyleBackColor = true;
            // 
            // tabTreeListView1
            // 
            this.tabTreeListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabTreeListView1.Delimiter = null;
            this.tabTreeListView1.Location = new System.Drawing.Point(4, 4);
            this.tabTreeListView1.Name = "tabTreeListView1";
            this.tabTreeListView1.Size = new System.Drawing.Size(800, 462);
            this.tabTreeListView1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 500);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(844, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 522);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "OPC Data Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPageTreeListView.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		//private System.Windows.Forms.TabPage tabPage2;
		//private System.Windows.Forms.TabPage tabPage1;
        //private TabSimpleExample tabSimple;
        //private TabComplexExample tabComplex;
        //private TabDataSet tabDataSet;
        private System.Windows.Forms.StatusStrip statusStrip1;

        private System.Windows.Forms.TabControl tabControl1;
        //private TabPage tabPage7;
        public ToolStripStatusLabel toolStripStatusLabel3;
        public ToolStripStatusLabel toolStripStatusLabel1;
        //private TabPage tabPage11;
        //private TabPage tabPageFileExplorer;
        //private TabFileExplorer tabFileExplorer1;
        //private TabPage tabPageFastList;
        //private TabFastList tabFastList1;
        private TabPage tabPageTreeListView;
        private TabTreeListView tabTreeListView1;
        //private TabPage tabPageDataTreeListView;
        //private TabDataTreeListView tabDataTreeListView1;
        //private TabPage tabPagePrinting;
        //private TabPrinting tabPrinting1;
        //private TabPage tabPageDragAndDrop;
        //private TabDragAndDrop tabDragAndDrop1;
        //private TabPage tabDescribedTasks;
        //private TabDescribedTask tabDescribedTask1;

	}
}

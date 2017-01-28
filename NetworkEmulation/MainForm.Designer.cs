using NetworkEmulation.Editor;

namespace NetworkEmulation {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clientNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cursorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cableCloudToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkManagmentSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.workspaceGroupBox = new System.Windows.Forms.GroupBox();
            this.editorPanel = new NetworkEmulation.Editor.EditorPanel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.networkHierarchyTreeView = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.parametersListView = new System.Windows.Forms.ListView();
            this.subnetworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.workspaceGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.elementsToolStripMenuItem,
            this.cursorToolStripMenuItem,
            this.simulationToolStripMenuItem,
            this.logToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectMenuItem,
            this.saveProjectMenuItem,
            this.loadProjectMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectMenuItem
            // 
            this.newProjectMenuItem.Name = "newProjectMenuItem";
            this.newProjectMenuItem.Size = new System.Drawing.Size(149, 22);
            this.newProjectMenuItem.Text = "New Project...";
            this.newProjectMenuItem.Click += new System.EventHandler(this.newProjectMenuItem_Click);
            // 
            // saveProjectMenuItem
            // 
            this.saveProjectMenuItem.Name = "saveProjectMenuItem";
            this.saveProjectMenuItem.Size = new System.Drawing.Size(149, 22);
            this.saveProjectMenuItem.Text = "Save Project...";
            this.saveProjectMenuItem.Click += new System.EventHandler(this.saveProjectMenuItem_Click);
            // 
            // loadProjectMenuItem
            // 
            this.loadProjectMenuItem.Name = "loadProjectMenuItem";
            this.loadProjectMenuItem.Size = new System.Drawing.Size(149, 22);
            this.loadProjectMenuItem.Text = "Load Project...";
            this.loadProjectMenuItem.Click += new System.EventHandler(this.loadProjectMenuItem_Click);
            // 
            // elementsToolStripMenuItem
            // 
            this.elementsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clientNodeToolStripMenuItem,
            this.networkNodeToolStripMenuItem,
            this.linkToolStripMenuItem,
            this.subnetworkToolStripMenuItem});
            this.elementsToolStripMenuItem.Name = "elementsToolStripMenuItem";
            this.elementsToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.elementsToolStripMenuItem.Text = "Elements";
            // 
            // clientNodeToolStripMenuItem
            // 
            this.clientNodeToolStripMenuItem.Name = "clientNodeToolStripMenuItem";
            this.clientNodeToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.clientNodeToolStripMenuItem.Text = "Client Node";
            this.clientNodeToolStripMenuItem.Click += new System.EventHandler(this.clientNodeToolStripMenuItem_Click);
            // 
            // networkNodeToolStripMenuItem
            // 
            this.networkNodeToolStripMenuItem.Name = "networkNodeToolStripMenuItem";
            this.networkNodeToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.networkNodeToolStripMenuItem.Text = "Network Node";
            this.networkNodeToolStripMenuItem.Click += new System.EventHandler(this.networkNodeToolStripMenuItem_Click);
            // 
            // linkToolStripMenuItem
            // 
            this.linkToolStripMenuItem.Name = "linkToolStripMenuItem";
            this.linkToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.linkToolStripMenuItem.Text = "Link";
            this.linkToolStripMenuItem.Click += new System.EventHandler(this.linkToolStripMenuItem_Click);
            // 
            // cursorToolStripMenuItem
            // 
            this.cursorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.cursorToolStripMenuItem.Name = "cursorToolStripMenuItem";
            this.cursorToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.cursorToolStripMenuItem.Text = "Cursor";
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.moveToolStripMenuItem.Text = "Move";
            this.moveToolStripMenuItem.Click += new System.EventHandler(this.moveToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Enabled = false;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // simulationToolStripMenuItem
            // 
            this.simulationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.stopToolStripMenuItem});
            this.simulationToolStripMenuItem.Name = "simulationToolStripMenuItem";
            this.simulationToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.simulationToolStripMenuItem.Text = "Simulation";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Enabled = false;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cableCloudToolStripMenuItem,
            this.networkManagmentSystemToolStripMenuItem});
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.logToolStripMenuItem.Text = "Log";
            // 
            // cableCloudToolStripMenuItem
            // 
            this.cableCloudToolStripMenuItem.Name = "cableCloudToolStripMenuItem";
            this.cableCloudToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.cableCloudToolStripMenuItem.Text = "Cable Cloud";
            this.cableCloudToolStripMenuItem.Click += new System.EventHandler(this.cableCloudToolStripMenuItem_Click);
            // 
            // networkManagmentSystemToolStripMenuItem
            // 
            this.networkManagmentSystemToolStripMenuItem.Name = "networkManagmentSystemToolStripMenuItem";
            this.networkManagmentSystemToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.networkManagmentSystemToolStripMenuItem.Text = "Network Managment System";
            this.networkManagmentSystemToolStripMenuItem.Click += new System.EventHandler(this.networkManagmentSystemToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.workspaceGroupBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(784, 537);
            this.splitContainer1.SplitterDistance = 558;
            this.splitContainer1.TabIndex = 1;
            // 
            // workspaceGroupBox
            // 
            this.workspaceGroupBox.Controls.Add(this.editorPanel);
            this.workspaceGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workspaceGroupBox.Location = new System.Drawing.Point(0, 0);
            this.workspaceGroupBox.Name = "workspaceGroupBox";
            this.workspaceGroupBox.Size = new System.Drawing.Size(558, 537);
            this.workspaceGroupBox.TabIndex = 0;
            this.workspaceGroupBox.TabStop = false;
            this.workspaceGroupBox.Text = "Workspace";
            // 
            // editorPanel
            // 
            this.editorPanel.BackColor = System.Drawing.SystemColors.Control;
            this.editorPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorPanel.Location = new System.Drawing.Point(3, 16);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(552, 518);
            this.editorPanel.TabIndex = 3;
            this.editorPanel.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.editorPanel_ControlAdded);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(222, 537);
            this.splitContainer2.SplitterDistance = 305;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.networkHierarchyTreeView);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 305);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Network Hierarchy";
            // 
            // networkHierarchyTreeView
            // 
            this.networkHierarchyTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.networkHierarchyTreeView.Location = new System.Drawing.Point(3, 16);
            this.networkHierarchyTreeView.Name = "networkHierarchyTreeView";
            this.networkHierarchyTreeView.Size = new System.Drawing.Size(216, 286);
            this.networkHierarchyTreeView.TabIndex = 1;
            this.networkHierarchyTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.networkHierarchyTreeView_AfterSelect);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.parametersListView);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 228);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Parameters";
            // 
            // parametersListView
            // 
            this.parametersListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parametersListView.Location = new System.Drawing.Point(3, 16);
            this.parametersListView.Name = "parametersListView";
            this.parametersListView.Size = new System.Drawing.Size(216, 209);
            this.parametersListView.TabIndex = 1;
            this.parametersListView.UseCompatibleStateImageBehavior = false;
            this.parametersListView.View = System.Windows.Forms.View.List;
            // 
            // subnetworkToolStripMenuItem
            // 
            this.subnetworkToolStripMenuItem.Name = "subnetworkToolStripMenuItem";
            this.subnetworkToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.subnetworkToolStripMenuItem.Text = "Subnetwork";
            this.subnetworkToolStripMenuItem.Click += new System.EventHandler(this.subnetworkToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "NetworkEmulation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.workspaceGroupBox.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadProjectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clientNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cursorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cableCloudToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkManagmentSystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView networkHierarchyTreeView;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView parametersListView;
        private System.Windows.Forms.GroupBox workspaceGroupBox;
        private EditorPanel editorPanel;
        private System.Windows.Forms.ToolStripMenuItem subnetworkToolStripMenuItem;
    }
}


using NetworkEmulation.editor;

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
            this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cursorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cableCloudToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkManagmentSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editorPanel = new NetworkEmulation.editor.EditorPanel();
            this.menuStrip1.SuspendLayout();
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
            this.newProjectMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newProjectMenuItem.Text = "New Project...";
            this.newProjectMenuItem.Click += new System.EventHandler(this.newProjectMenuItem_Click);
            // 
            // saveProjectMenuItem
            // 
            this.saveProjectMenuItem.Name = "saveProjectMenuItem";
            this.saveProjectMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveProjectMenuItem.Text = "Save Project...";
            this.saveProjectMenuItem.Click += new System.EventHandler(this.saveProjectMenuItem_Click);
            // 
            // loadProjectMenuItem
            // 
            this.loadProjectMenuItem.Name = "loadProjectMenuItem";
            this.loadProjectMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadProjectMenuItem.Text = "Load Project...";
            this.loadProjectMenuItem.Click += new System.EventHandler(this.loadProjectMenuItem_Click);
            // 
            // elementsToolStripMenuItem
            // 
            this.elementsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clientNodeToolStripMenuItem,
            this.networkNodeToolStripMenuItem,
            this.linkToolStripMenuItem,
            this.connectionToolStripMenuItem});
            this.elementsToolStripMenuItem.Name = "elementsToolStripMenuItem";
            this.elementsToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.elementsToolStripMenuItem.Text = "Elements";
            // 
            // clientNodeToolStripMenuItem
            // 
            this.clientNodeToolStripMenuItem.Name = "clientNodeToolStripMenuItem";
            this.clientNodeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clientNodeToolStripMenuItem.Text = "Client Node";
            this.clientNodeToolStripMenuItem.Click += new System.EventHandler(this.clientNodeToolStripMenuItem_Click);
            // 
            // networkNodeToolStripMenuItem
            // 
            this.networkNodeToolStripMenuItem.Name = "networkNodeToolStripMenuItem";
            this.networkNodeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.networkNodeToolStripMenuItem.Text = "Network Node";
            this.networkNodeToolStripMenuItem.Click += new System.EventHandler(this.networkNodeToolStripMenuItem_Click);
            // 
            // linkToolStripMenuItem
            // 
            this.linkToolStripMenuItem.Name = "linkToolStripMenuItem";
            this.linkToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.linkToolStripMenuItem.Text = "Link";
            this.linkToolStripMenuItem.Click += new System.EventHandler(this.linkToolStripMenuItem_Click);
            // 
            // connectionToolStripMenuItem
            // 
            this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            this.connectionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectionToolStripMenuItem.Text = "Connection";
            this.connectionToolStripMenuItem.Click += new System.EventHandler(this.connectionToolStripMenuItem_Click);
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
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.moveToolStripMenuItem.Text = "Move";
            this.moveToolStripMenuItem.Click += new System.EventHandler(this.moveToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Enabled = false;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            this.runToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Enabled = false;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            // editorPanel
            // 
            this.editorPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.editorPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorPanel.Location = new System.Drawing.Point(0, 24);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(784, 537);
            this.editorPanel.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.editorPanel);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "NetworkEmulation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cursorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cableCloudToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkManagmentSystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private EditorPanel editorPanel;
    }
}


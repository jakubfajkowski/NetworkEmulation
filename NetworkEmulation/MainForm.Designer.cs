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
            this.addClientNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteClientNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNetworkNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNetworkNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLinkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteLinkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addConnectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteConnectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editorPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.elementsToolStripMenuItem});
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
            // 
            // saveProjectMenuItem
            // 
            this.saveProjectMenuItem.Name = "saveProjectMenuItem";
            this.saveProjectMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveProjectMenuItem.Text = "Save Project...";
            // 
            // loadProjectMenuItem
            // 
            this.loadProjectMenuItem.Name = "loadProjectMenuItem";
            this.loadProjectMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadProjectMenuItem.Text = "Load Project...";
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
            this.clientNodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addClientNodeMenuItem,
            this.deleteClientNodeMenuItem});
            this.clientNodeToolStripMenuItem.Name = "clientNodeToolStripMenuItem";
            this.clientNodeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clientNodeToolStripMenuItem.Text = "Client Node";
            // 
            // addClientNodeMenuItem
            // 
            this.addClientNodeMenuItem.Name = "addClientNodeMenuItem";
            this.addClientNodeMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addClientNodeMenuItem.Text = "Add...";
            this.addClientNodeMenuItem.Click += new System.EventHandler(this.addClientNodeMenuItem_Click);
            // 
            // deleteClientNodeMenuItem
            // 
            this.deleteClientNodeMenuItem.Name = "deleteClientNodeMenuItem";
            this.deleteClientNodeMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteClientNodeMenuItem.Text = "Delete...";
            // 
            // networkNodeToolStripMenuItem
            // 
            this.networkNodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNetworkNodeMenuItem,
            this.deleteNetworkNodeMenuItem});
            this.networkNodeToolStripMenuItem.Name = "networkNodeToolStripMenuItem";
            this.networkNodeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.networkNodeToolStripMenuItem.Text = "Network Node";
            // 
            // addNetworkNodeMenuItem
            // 
            this.addNetworkNodeMenuItem.Name = "addNetworkNodeMenuItem";
            this.addNetworkNodeMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addNetworkNodeMenuItem.Text = "Add...";
            this.addNetworkNodeMenuItem.Click += new System.EventHandler(this.addNetworkNodeMenuItem_Click);
            // 
            // deleteNetworkNodeMenuItem
            // 
            this.deleteNetworkNodeMenuItem.Name = "deleteNetworkNodeMenuItem";
            this.deleteNetworkNodeMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteNetworkNodeMenuItem.Text = "Delete...";
            // 
            // linkToolStripMenuItem
            // 
            this.linkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLinkMenuItem,
            this.deleteLinkMenuItem});
            this.linkToolStripMenuItem.Name = "linkToolStripMenuItem";
            this.linkToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.linkToolStripMenuItem.Text = "Link";
            // 
            // addLinkMenuItem
            // 
            this.addLinkMenuItem.Name = "addLinkMenuItem";
            this.addLinkMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addLinkMenuItem.Text = "Add...";
            this.addLinkMenuItem.Click += new System.EventHandler(this.addLinkMenuItem_Click);
            // 
            // deleteLinkMenuItem
            // 
            this.deleteLinkMenuItem.Name = "deleteLinkMenuItem";
            this.deleteLinkMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteLinkMenuItem.Text = "Delete...";
            // 
            // connectionToolStripMenuItem
            // 
            this.connectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addConnectionMenuItem,
            this.deleteConnectionMenuItem});
            this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            this.connectionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectionToolStripMenuItem.Text = "Connection";
            // 
            // addConnectionMenuItem
            // 
            this.addConnectionMenuItem.Name = "addConnectionMenuItem";
            this.addConnectionMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addConnectionMenuItem.Text = "Add...";
            this.addConnectionMenuItem.Click += new System.EventHandler(this.addConnectionMenuItem_Click);
            // 
            // deleteConnectionMenuItem
            // 
            this.deleteConnectionMenuItem.Name = "deleteConnectionMenuItem";
            this.deleteConnectionMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteConnectionMenuItem.Text = "Delete...";
            // 
            // editorPanel
            // 
            this.editorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editorPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorPanel.Location = new System.Drawing.Point(0, 24);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(784, 537);
            this.editorPanel.TabIndex = 1;
            this.editorPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.editorPanel_MouseClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.editorPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "NetworkEmulation";
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
        private System.Windows.Forms.ToolStripMenuItem addClientNodeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteClientNodeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNetworkNodeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteNetworkNodeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLinkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteLinkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addConnectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteConnectionMenuItem;
        private System.Windows.Forms.Panel editorPanel;
    }
}


namespace NetworkEmulation.Editor.Element {
    partial class NetworkNodeForm {
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxIpAddress = new System.Windows.Forms.TextBox();
            this.textBoxCloudPort = new System.Windows.Forms.TextBox();
            this.textBoxNMSPort = new System.Windows.Forms.TextBox();
            this.textBoxNumberOfPorts = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "IP Address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Cloud Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "NMS Port:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Number of Ports:";
            // 
            // textBoxIpAddress
            // 
            this.textBoxIpAddress.Location = new System.Drawing.Point(98, 12);
            this.textBoxIpAddress.Name = "textBoxIpAddress";
            this.textBoxIpAddress.Size = new System.Drawing.Size(144, 20);
            this.textBoxIpAddress.TabIndex = 6;
            // 
            // textBoxCloudPort
            // 
            this.textBoxCloudPort.Location = new System.Drawing.Point(98, 38);
            this.textBoxCloudPort.Name = "textBoxCloudPort";
            this.textBoxCloudPort.Size = new System.Drawing.Size(144, 20);
            this.textBoxCloudPort.TabIndex = 7;
            // 
            // textBoxNMSPort
            // 
            this.textBoxNMSPort.Location = new System.Drawing.Point(98, 66);
            this.textBoxNMSPort.Name = "textBoxNMSPort";
            this.textBoxNMSPort.Size = new System.Drawing.Size(144, 20);
            this.textBoxNMSPort.TabIndex = 8;
            // 
            // textBoxNumberOfPorts
            // 
            this.textBoxNumberOfPorts.Location = new System.Drawing.Point(98, 92);
            this.textBoxNumberOfPorts.Name = "textBoxNumberOfPorts";
            this.textBoxNumberOfPorts.Size = new System.Drawing.Size(144, 20);
            this.textBoxNumberOfPorts.TabIndex = 9;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(86, 118);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(167, 118);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // NetworkNodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 148);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxNumberOfPorts);
            this.Controls.Add(this.textBoxNMSPort);
            this.Controls.Add(this.textBoxCloudPort);
            this.Controls.Add(this.textBoxIpAddress);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NetworkNodeForm";
            this.Text = "NetworkNodeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxIpAddress;
        private System.Windows.Forms.TextBox textBoxCloudPort;
        private System.Windows.Forms.TextBox textBoxNMSPort;
        private System.Windows.Forms.TextBox textBoxNumberOfPorts;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}
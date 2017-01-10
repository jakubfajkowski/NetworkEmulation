namespace NetworkEmulation.Editor.Element {
    partial class ConnectionForm {
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
            this.textBoxInputVpi = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxInputVci = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOutputVpi = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxOutputVci = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxOutputPort = new System.Windows.Forms.TextBox();
            this.textBoxInputPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxInputVpi
            // 
            this.textBoxInputVpi.Location = new System.Drawing.Point(85, 12);
            this.textBoxInputVpi.Name = "textBoxInputVpi";
            this.textBoxInputVpi.Size = new System.Drawing.Size(100, 20);
            this.textBoxInputVpi.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Input VPI: ";
            // 
            // textBoxInputVci
            // 
            this.textBoxInputVci.Location = new System.Drawing.Point(85, 38);
            this.textBoxInputVci.Name = "textBoxInputVci";
            this.textBoxInputVci.Size = new System.Drawing.Size(100, 20);
            this.textBoxInputVci.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Input VCI: ";
            // 
            // textBoxOutputVpi
            // 
            this.textBoxOutputVpi.Location = new System.Drawing.Point(85, 90);
            this.textBoxOutputVpi.Name = "textBoxOutputVpi";
            this.textBoxOutputVpi.Size = new System.Drawing.Size(100, 20);
            this.textBoxOutputVpi.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Output VPI: ";
            // 
            // textBoxOutputVci
            // 
            this.textBoxOutputVci.Location = new System.Drawing.Point(85, 116);
            this.textBoxOutputVci.Name = "textBoxOutputVci";
            this.textBoxOutputVci.Size = new System.Drawing.Size(100, 20);
            this.textBoxOutputVci.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Output VCI: ";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(27, 180);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 14;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(110, 180);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 15;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Output port: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Input port: ";
            // 
            // textBoxOutputPort
            // 
            this.textBoxOutputPort.Location = new System.Drawing.Point(85, 142);
            this.textBoxOutputPort.Name = "textBoxOutputPort";
            this.textBoxOutputPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxOutputPort.TabIndex = 13;
            // 
            // textBoxInputPort
            // 
            this.textBoxInputPort.Location = new System.Drawing.Point(85, 64);
            this.textBoxInputPort.Name = "textBoxInputPort";
            this.textBoxInputPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxInputPort.TabIndex = 7;
            // 
            // ConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(197, 212);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxOutputPort);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxOutputVci);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxOutputVpi);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxInputPort);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxInputVci);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxInputVpi);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ConnectionForm";
            this.Text = "ConnectionSP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxInputVpi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxInputVci;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxOutputVpi;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxOutputVci;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxOutputPort;
        private System.Windows.Forms.TextBox textBoxInputPort;
    }
}
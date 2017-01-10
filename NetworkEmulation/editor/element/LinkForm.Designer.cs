namespace NetworkEmulation.Editor.Element {
    partial class LinkForm {
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxInputPort = new System.Windows.Forms.ComboBox();
            this.comboBoxOutputPort = new System.Windows.Forms.ComboBox();
            this.textBoxCapacity = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.Capacity = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Output port: ";
            // 
            // comboBoxInputPort
            // 
            this.comboBoxInputPort.FormattingEnabled = true;
            this.comboBoxInputPort.Location = new System.Drawing.Point(83, 12);
            this.comboBoxInputPort.Name = "comboBoxInputPort";
            this.comboBoxInputPort.Size = new System.Drawing.Size(121, 21);
            this.comboBoxInputPort.TabIndex = 2;
            // 
            // comboBoxOutputPort
            // 
            this.comboBoxOutputPort.FormattingEnabled = true;
            this.comboBoxOutputPort.Location = new System.Drawing.Point(83, 43);
            this.comboBoxOutputPort.Name = "comboBoxOutputPort";
            this.comboBoxOutputPort.Size = new System.Drawing.Size(121, 21);
            this.comboBoxOutputPort.TabIndex = 3;
            // 
            // textBoxCapacity
            // 
            this.textBoxCapacity.Location = new System.Drawing.Point(83, 74);
            this.textBoxCapacity.Multiline = true;
            this.textBoxCapacity.Name = "textBoxCapacity";
            this.textBoxCapacity.Size = new System.Drawing.Size(121, 20);
            this.textBoxCapacity.TabIndex = 4;
            this.textBoxCapacity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(48, 110);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.OkClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(129, 110);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // Capacity
            // 
            this.Capacity.AutoSize = true;
            this.Capacity.Location = new System.Drawing.Point(23, 77);
            this.Capacity.Name = "Capacity";
            this.Capacity.Size = new System.Drawing.Size(54, 13);
            this.Capacity.TabIndex = 7;
            this.Capacity.Text = "Capacity: ";
            // 
            // LinkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 144);
            this.Controls.Add(this.Capacity);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxCapacity);
            this.Controls.Add(this.comboBoxOutputPort);
            this.Controls.Add(this.comboBoxInputPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LinkForm";
            this.Text = "LinkForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxInputPort;
        private System.Windows.Forms.ComboBox comboBoxOutputPort;
        private System.Windows.Forms.TextBox textBoxCapacity;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label Capacity;
    }
}
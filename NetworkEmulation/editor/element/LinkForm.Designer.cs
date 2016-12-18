namespace NetworkEmulation.editor.element {
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
            this.comboInputPort = new System.Windows.Forms.ComboBox();
            this.comboOutputPort = new System.Windows.Forms.ComboBox();
            this.textBoxCapacity = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.Capacity = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Outrput port: ";
            // 
            // comboInputPort
            // 
            this.comboInputPort.FormattingEnabled = true;
            this.comboInputPort.Location = new System.Drawing.Point(124, 40);
            this.comboInputPort.Name = "comboInputPort";
            this.comboInputPort.Size = new System.Drawing.Size(121, 21);
            this.comboInputPort.TabIndex = 2;
            // 
            // comboOutputPort
            // 
            this.comboOutputPort.FormattingEnabled = true;
            this.comboOutputPort.Location = new System.Drawing.Point(124, 88);
            this.comboOutputPort.Name = "comboOutputPort";
            this.comboOutputPort.Size = new System.Drawing.Size(121, 21);
            this.comboOutputPort.TabIndex = 3;
            // 
            // textBoxCapacity
            // 
            this.textBoxCapacity.Location = new System.Drawing.Point(124, 129);
            this.textBoxCapacity.Multiline = true;
            this.textBoxCapacity.Name = "textBoxCapacity";
            this.textBoxCapacity.Size = new System.Drawing.Size(121, 23);
            this.textBoxCapacity.TabIndex = 4;
            this.textBoxCapacity.Text = "300";
            this.textBoxCapacity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(40, 207);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.OkClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(170, 207);
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
            this.Capacity.Location = new System.Drawing.Point(40, 132);
            this.Capacity.Name = "Capacity";
            this.Capacity.Size = new System.Drawing.Size(54, 13);
            this.Capacity.TabIndex = 7;
            this.Capacity.Text = "Capacity: ";
            // 
            // LinkSP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.Capacity);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxCapacity);
            this.Controls.Add(this.comboOutputPort);
            this.Controls.Add(this.comboInputPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "LinkSP";
            this.Text = "LinkSP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboInputPort;
        private System.Windows.Forms.ComboBox comboOutputPort;
        private System.Windows.Forms.TextBox textBoxCapacity;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label Capacity;
    }
}
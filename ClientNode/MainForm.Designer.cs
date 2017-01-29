﻿namespace ClientNode {
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
            this.buttonSend = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonClear = new System.Windows.Forms.Button();
            this.textBoxReceiver = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxEventLog = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxMessage = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxReceived = new System.Windows.Forms.RichTextBox();
            this.comboBoxConnections = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownCapacity = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonConnection = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCapacity)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(382, 29);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.TabIndex = 0;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Receiver Id:";
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(463, 29);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 12;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // textBoxReceiver
            // 
            this.textBoxReceiver.Location = new System.Drawing.Point(83, 5);
            this.textBoxReceiver.Name = "textBoxReceiver";
            this.textBoxReceiver.Size = new System.Drawing.Size(212, 20);
            this.textBoxReceiver.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxEventLog);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 163);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(550, 133);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Event Log:";
            // 
            // textBoxEventLog
            // 
            this.textBoxEventLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxEventLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxEventLog.Location = new System.Drawing.Point(3, 16);
            this.textBoxEventLog.Name = "textBoxEventLog";
            this.textBoxEventLog.ReadOnly = true;
            this.textBoxEventLog.Size = new System.Drawing.Size(544, 114);
            this.textBoxEventLog.TabIndex = 10;
            this.textBoxEventLog.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxConnections);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.numericUpDownCapacity);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.buttonConnection);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxReceiver);
            this.splitContainer1.Panel2.Controls.Add(this.buttonClear);
            this.splitContainer1.Panel2.Controls.Add(this.buttonSend);
            this.splitContainer1.Size = new System.Drawing.Size(550, 163);
            this.splitContainer1.SplitterDistance = 99;
            this.splitContainer1.TabIndex = 15;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(550, 99);
            this.splitContainer2.SplitterDistance = 252;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxMessage);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(252, 99);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Message to Send:";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMessage.Location = new System.Drawing.Point(3, 16);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(246, 80);
            this.textBoxMessage.TabIndex = 21;
            this.textBoxMessage.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxReceived);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(294, 99);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Received Messages:";
            // 
            // textBoxReceived
            // 
            this.textBoxReceived.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxReceived.Location = new System.Drawing.Point(3, 16);
            this.textBoxReceived.Name = "textBoxReceived";
            this.textBoxReceived.ReadOnly = true;
            this.textBoxReceived.Size = new System.Drawing.Size(288, 80);
            this.textBoxReceived.TabIndex = 18;
            this.textBoxReceived.Text = "";
            // 
            // comboBoxConnections
            // 
            this.comboBoxConnections.FormattingEnabled = true;
            this.comboBoxConnections.Location = new System.Drawing.Point(83, 32);
            this.comboBoxConnections.Name = "comboBoxConnections";
            this.comboBoxConnections.Size = new System.Drawing.Size(211, 21);
            this.comboBoxConnections.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(301, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Demanded Capacity:";
            // 
            // numericUpDownCapacity
            // 
            this.numericUpDownCapacity.Location = new System.Drawing.Point(413, 6);
            this.numericUpDownCapacity.Name = "numericUpDownCapacity";
            this.numericUpDownCapacity.Size = new System.Drawing.Size(125, 20);
            this.numericUpDownCapacity.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Connections:";
            // 
            // buttonConnection
            // 
            this.buttonConnection.BackColor = System.Drawing.Color.Lime;
            this.buttonConnection.Location = new System.Drawing.Point(301, 29);
            this.buttonConnection.Name = "buttonConnection";
            this.buttonConnection.Size = new System.Drawing.Size(75, 23);
            this.buttonConnection.TabIndex = 14;
            this.buttonConnection.Text = "Connect";
            this.buttonConnection.UseVisualStyleBackColor = false;
            this.buttonConnection.Click += new System.EventHandler(this.buttonConnection_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 296);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "ClientNode";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCapacity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.TextBox textBoxReceiver;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox textBoxEventLog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox textBoxMessage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox textBoxReceived;
        private System.Windows.Forms.Button buttonConnection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxConnections;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownCapacity;
    }
}


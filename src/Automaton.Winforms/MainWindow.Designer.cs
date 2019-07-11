namespace Automaton.Winforms
{
    partial class MainWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoadModPackBtn = new System.Windows.Forms.Button();
            this.QueueLabel = new System.Windows.Forms.Label();
            this.QueueStatus = new System.Windows.Forms.TextBox();
            this.LogLbl = new System.Windows.Forms.Label();
            this.LogListBox = new System.Windows.Forms.ListBox();
            this.SetInstallLocationBtn = new System.Windows.Forms.Button();
            this.InstallBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LoadModPackBtn
            // 
            this.LoadModPackBtn.Location = new System.Drawing.Point(12, 12);
            this.LoadModPackBtn.Name = "LoadModPackBtn";
            this.LoadModPackBtn.Size = new System.Drawing.Size(118, 23);
            this.LoadModPackBtn.TabIndex = 0;
            this.LoadModPackBtn.Text = "Load Mod Pack";
            this.LoadModPackBtn.UseVisualStyleBackColor = true;
            this.LoadModPackBtn.Click += new System.EventHandler(this.Button1_Click);
            // 
            // QueueLabel
            // 
            this.QueueLabel.AutoSize = true;
            this.QueueLabel.Location = new System.Drawing.Point(12, 53);
            this.QueueLabel.Name = "QueueLabel";
            this.QueueLabel.Size = new System.Drawing.Size(71, 13);
            this.QueueLabel.TabIndex = 1;
            this.QueueLabel.Text = "Work Queue:";
            // 
            // QueueStatus
            // 
            this.QueueStatus.Enabled = false;
            this.QueueStatus.Location = new System.Drawing.Point(12, 69);
            this.QueueStatus.Multiline = true;
            this.QueueStatus.Name = "QueueStatus";
            this.QueueStatus.Size = new System.Drawing.Size(1216, 261);
            this.QueueStatus.TabIndex = 2;
            // 
            // LogLbl
            // 
            this.LogLbl.AutoSize = true;
            this.LogLbl.Location = new System.Drawing.Point(12, 333);
            this.LogLbl.Name = "LogLbl";
            this.LogLbl.Size = new System.Drawing.Size(28, 13);
            this.LogLbl.TabIndex = 3;
            this.LogLbl.Text = "Log:";
            // 
            // LogListBox
            // 
            this.LogListBox.FormattingEnabled = true;
            this.LogListBox.Location = new System.Drawing.Point(12, 349);
            this.LogListBox.Name = "LogListBox";
            this.LogListBox.ScrollAlwaysVisible = true;
            this.LogListBox.Size = new System.Drawing.Size(1216, 277);
            this.LogListBox.TabIndex = 4;
            // 
            // SetInstallLocationBtn
            // 
            this.SetInstallLocationBtn.Enabled = false;
            this.SetInstallLocationBtn.Location = new System.Drawing.Point(136, 12);
            this.SetInstallLocationBtn.Name = "SetInstallLocationBtn";
            this.SetInstallLocationBtn.Size = new System.Drawing.Size(138, 23);
            this.SetInstallLocationBtn.TabIndex = 5;
            this.SetInstallLocationBtn.Text = "Set Install Location";
            this.SetInstallLocationBtn.UseVisualStyleBackColor = true;
            this.SetInstallLocationBtn.Click += new System.EventHandler(this.SetInstallLocationBtn_Click);
            // 
            // InstallBtn
            // 
            this.InstallBtn.Enabled = false;
            this.InstallBtn.Location = new System.Drawing.Point(280, 12);
            this.InstallBtn.Name = "InstallBtn";
            this.InstallBtn.Size = new System.Drawing.Size(104, 23);
            this.InstallBtn.TabIndex = 6;
            this.InstallBtn.Text = "Install!";
            this.InstallBtn.UseVisualStyleBackColor = true;
            this.InstallBtn.Click += new System.EventHandler(this.InstallBtn_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1240, 634);
            this.Controls.Add(this.InstallBtn);
            this.Controls.Add(this.SetInstallLocationBtn);
            this.Controls.Add(this.LogListBox);
            this.Controls.Add(this.LogLbl);
            this.Controls.Add(this.QueueStatus);
            this.Controls.Add(this.QueueLabel);
            this.Controls.Add(this.LoadModPackBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "Automaton";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadModPackBtn;
        private System.Windows.Forms.Label QueueLabel;
        private System.Windows.Forms.TextBox QueueStatus;
        private System.Windows.Forms.Label LogLbl;
        private System.Windows.Forms.ListBox LogListBox;
        private System.Windows.Forms.Button SetInstallLocationBtn;
        private System.Windows.Forms.Button InstallBtn;
    }
}


namespace GuildfordBoroughCouncil.Corporate.VoiceRecorder.Controller
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ChangeDeviceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.ExtensionList = new System.Windows.Forms.ComboBox();
            this.SetExtension = new System.Windows.Forms.Button();
            this.TrayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = this.TrayMenu;
            this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
            this.TrayIcon.Text = "Voice Recorder Controller";
            this.TrayIcon.Visible = true;
            // 
            // TrayMenu
            // 
            this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChangeDeviceMenuItem,
            this.ExitMenuItem});
            this.TrayMenu.Name = "TrayMenu";
            this.TrayMenu.Size = new System.Drawing.Size(154, 48);
            // 
            // ChangeDeviceMenuItem
            // 
            this.ChangeDeviceMenuItem.Name = "ChangeDeviceMenuItem";
            this.ChangeDeviceMenuItem.Size = new System.Drawing.Size(153, 22);
            this.ChangeDeviceMenuItem.Text = "Change Device";
            this.ChangeDeviceMenuItem.Click += new System.EventHandler(this.ChangeDeviceMenuItem_Click);
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(153, 22);
            this.ExitMenuItem.Text = "Exit";
            this.ExitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please select your name from the list below:";
            // 
            // ExtensionList
            // 
            this.ExtensionList.DisplayMember = "Key";
            this.ExtensionList.FormattingEnabled = true;
            this.ExtensionList.Location = new System.Drawing.Point(15, 35);
            this.ExtensionList.Name = "ExtensionList";
            this.ExtensionList.Size = new System.Drawing.Size(396, 21);
            this.ExtensionList.TabIndex = 3;
            this.ExtensionList.ValueMember = "Value";
            // 
            // SetExtension
            // 
            this.SetExtension.Location = new System.Drawing.Point(343, 62);
            this.SetExtension.Name = "SetExtension";
            this.SetExtension.Size = new System.Drawing.Size(68, 23);
            this.SetExtension.TabIndex = 4;
            this.SetExtension.Text = "Save";
            this.SetExtension.UseVisualStyleBackColor = true;
            this.SetExtension.Click += new System.EventHandler(this.SetExtension_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 96);
            this.ControlBox = false;
            this.Controls.Add(this.SetExtension);
            this.Controls.Add(this.ExtensionList);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Voice Recorder Controller";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.TrayMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.ContextMenuStrip TrayMenu;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ExtensionList;
        private System.Windows.Forms.Button SetExtension;
        private System.Windows.Forms.ToolStripMenuItem ChangeDeviceMenuItem;
    }
}


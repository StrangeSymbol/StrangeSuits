namespace StrangeSuits
{
    partial class NetworkConnector
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
            this.tbxHost = new System.Windows.Forms.TextBox();
            this.tbxClient = new System.Windows.Forms.TextBox();
            this.cbHost = new System.Windows.Forms.CheckBox();
            this.cbClient = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tbxHost
            // 
            this.tbxHost.Location = new System.Drawing.Point(12, 31);
            this.tbxHost.Name = "tbxHost";
            this.tbxHost.ReadOnly = true;
            this.tbxHost.Size = new System.Drawing.Size(100, 20);
            this.tbxHost.TabIndex = 1;
            // 
            // tbxClient
            // 
            this.tbxClient.Location = new System.Drawing.Point(12, 68);
            this.tbxClient.Name = "tbxClient";
            this.tbxClient.ReadOnly = true;
            this.tbxClient.Size = new System.Drawing.Size(100, 20);
            this.tbxClient.TabIndex = 2;
            // 
            // cbHost
            // 
            this.cbHost.AutoSize = true;
            this.cbHost.Location = new System.Drawing.Point(118, 34);
            this.cbHost.Name = "cbHost";
            this.cbHost.Size = new System.Drawing.Size(15, 14);
            this.cbHost.TabIndex = 3;
            this.cbHost.UseVisualStyleBackColor = true;
            this.cbHost.Click += new System.EventHandler(this.cbHost_Click);
            // 
            // cbClient
            // 
            this.cbClient.AutoSize = true;
            this.cbClient.Location = new System.Drawing.Point(118, 71);
            this.cbClient.Name = "cbClient";
            this.cbClient.Size = new System.Drawing.Size(15, 14);
            this.cbClient.TabIndex = 4;
            this.cbClient.UseVisualStyleBackColor = true;
            this.cbClient.Click += new System.EventHandler(this.cbClient_Click);
            // 
            // NetworkConnector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 110);
            this.Controls.Add(this.cbClient);
            this.Controls.Add(this.cbHost);
            this.Controls.Add(this.tbxClient);
            this.Controls.Add(this.tbxHost);
            this.Name = "NetworkConnector";
            this.Text = "Strange Suits";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox tbxHost;
        public System.Windows.Forms.TextBox tbxClient;
        public System.Windows.Forms.CheckBox cbHost;
        public System.Windows.Forms.CheckBox cbClient;

    }
}
namespace IOCheckoutTool
{
    partial class Configure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configure));
            this.NameLabel = new System.Windows.Forms.Label();
            this.ChannelLabel = new System.Windows.Forms.Label();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.ChannelBox = new System.Windows.Forms.ComboBox();
            this.RedundantLabel = new System.Windows.Forms.Label();
            this.RedundantNameBox = new System.Windows.Forms.TextBox();
            this.ConfigButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(6, 12);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(77, 17);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "FBM Name";
            // 
            // ChannelLabel
            // 
            this.ChannelLabel.AutoSize = true;
            this.ChannelLabel.Location = new System.Drawing.Point(6, 45);
            this.ChannelLabel.Name = "ChannelLabel";
            this.ChannelLabel.Size = new System.Drawing.Size(60, 17);
            this.ChannelLabel.TabIndex = 1;
            this.ChannelLabel.Text = "Channel";
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(133, 9);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(202, 22);
            this.NameBox.TabIndex = 2;
            // 
            // ChannelBox
            // 
            this.ChannelBox.FormattingEnabled = true;
            this.ChannelBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.ChannelBox.Location = new System.Drawing.Point(133, 41);
            this.ChannelBox.Name = "ChannelBox";
            this.ChannelBox.Size = new System.Drawing.Size(202, 24);
            this.ChannelBox.TabIndex = 3;
            // 
            // RedundantLabel
            // 
            this.RedundantLabel.AutoSize = true;
            this.RedundantLabel.Location = new System.Drawing.Point(6, 83);
            this.RedundantLabel.Name = "RedundantLabel";
            this.RedundantLabel.Size = new System.Drawing.Size(119, 17);
            this.RedundantLabel.TabIndex = 4;
            this.RedundantLabel.Text = "Redundant Name";
            // 
            // RedundantNameBox
            // 
            this.RedundantNameBox.Location = new System.Drawing.Point(133, 80);
            this.RedundantNameBox.Name = "RedundantNameBox";
            this.RedundantNameBox.Size = new System.Drawing.Size(202, 22);
            this.RedundantNameBox.TabIndex = 5;
            // 
            // ConfigButton
            // 
            this.ConfigButton.Location = new System.Drawing.Point(130, 113);
            this.ConfigButton.Name = "ConfigButton";
            this.ConfigButton.Size = new System.Drawing.Size(86, 34);
            this.ConfigButton.TabIndex = 6;
            this.ConfigButton.Text = "Configure";
            this.ConfigButton.UseVisualStyleBackColor = true;
            this.ConfigButton.Click += new System.EventHandler(this.ConfigButton_Click);
            // 
            // Configure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 153);
            this.Controls.Add(this.ConfigButton);
            this.Controls.Add(this.RedundantNameBox);
            this.Controls.Add(this.RedundantLabel);
            this.Controls.Add(this.ChannelBox);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.ChannelLabel);
            this.Controls.Add(this.NameLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(365, 200);
            this.MinimumSize = new System.Drawing.Size(365, 200);
            this.Name = "Configure";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label ChannelLabel;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.ComboBox ChannelBox;
        private System.Windows.Forms.Label RedundantLabel;
        private System.Windows.Forms.TextBox RedundantNameBox;
        private System.Windows.Forms.Button ConfigButton;
    }
}
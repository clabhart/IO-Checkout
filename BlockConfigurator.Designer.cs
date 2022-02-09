namespace IOCheckoutTool
{
    partial class BlockConfigurator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlockConfigurator));
            this.AllRINs = new System.Windows.Forms.RadioButton();
            this.FBM = new System.Windows.Forms.TreeView();
            this.FBMContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RemoveButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ConfigOptions = new System.Windows.Forms.GroupBox();
            this.Manual = new System.Windows.Forms.RadioButton();
            this.HalfDigital = new System.Windows.Forms.RadioButton();
            this.HalfAnalog = new System.Windows.Forms.RadioButton();
            this.AllBOUTs = new System.Windows.Forms.RadioButton();
            this.AllROUTs = new System.Windows.Forms.RadioButton();
            this.AllBINs = new System.Windows.Forms.RadioButton();
            this.BlockOptions = new System.Windows.Forms.ListBox();
            this.ConfigButton = new System.Windows.Forms.Button();
            this.FBMContextMenu.SuspendLayout();
            this.ConfigOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // AllRINs
            // 
            this.AllRINs.AutoSize = true;
            this.AllRINs.Location = new System.Drawing.Point(6, 21);
            this.AllRINs.Name = "AllRINs";
            this.AllRINs.Size = new System.Drawing.Size(78, 21);
            this.AllRINs.TabIndex = 0;
            this.AllRINs.TabStop = true;
            this.AllRINs.Text = "All RINs";
            this.AllRINs.UseVisualStyleBackColor = true;
            this.AllRINs.Click += new System.EventHandler(this.AllRINs_Click);
            // 
            // FBM
            // 
            this.FBM.AllowDrop = true;
            this.FBM.ContextMenuStrip = this.FBMContextMenu;
            this.FBM.Location = new System.Drawing.Point(11, 12);
            this.FBM.Name = "FBM";
            this.FBM.Size = new System.Drawing.Size(189, 202);
            this.FBM.TabIndex = 1;
            this.FBM.DragDrop += new System.Windows.Forms.DragEventHandler(this.FBM_DragDrop);
            this.FBM.DragEnter += new System.Windows.Forms.DragEventHandler(this.FBM_DragEnter);
            // 
            // FBMContextMenu
            // 
            this.FBMContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.FBMContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RemoveButton});
            this.FBMContextMenu.Name = "FBMContextMenu";
            this.FBMContextMenu.Size = new System.Drawing.Size(133, 28);
            // 
            // RemoveButton
            // 
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(132, 24);
            this.RemoveButton.Text = "Remove";
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // ConfigOptions
            // 
            this.ConfigOptions.Controls.Add(this.Manual);
            this.ConfigOptions.Controls.Add(this.HalfDigital);
            this.ConfigOptions.Controls.Add(this.HalfAnalog);
            this.ConfigOptions.Controls.Add(this.AllBOUTs);
            this.ConfigOptions.Controls.Add(this.AllROUTs);
            this.ConfigOptions.Controls.Add(this.AllBINs);
            this.ConfigOptions.Controls.Add(this.AllRINs);
            this.ConfigOptions.Location = new System.Drawing.Point(368, 12);
            this.ConfigOptions.Name = "ConfigOptions";
            this.ConfigOptions.Size = new System.Drawing.Size(169, 202);
            this.ConfigOptions.TabIndex = 2;
            this.ConfigOptions.TabStop = false;
            this.ConfigOptions.Text = "Configuration Options";
            // 
            // Manual
            // 
            this.Manual.AutoSize = true;
            this.Manual.Location = new System.Drawing.Point(6, 175);
            this.Manual.Name = "Manual";
            this.Manual.Size = new System.Drawing.Size(163, 21);
            this.Manual.TabIndex = 0;
            this.Manual.TabStop = true;
            this.Manual.Text = "Manual Configuration";
            this.Manual.UseVisualStyleBackColor = true;
            this.Manual.Click += new System.EventHandler(this.Manual_Click);
            // 
            // HalfDigital
            // 
            this.HalfDigital.AutoSize = true;
            this.HalfDigital.Location = new System.Drawing.Point(6, 148);
            this.HalfDigital.Name = "HalfDigital";
            this.HalfDigital.Size = new System.Drawing.Size(123, 21);
            this.HalfDigital.TabIndex = 0;
            this.HalfDigital.TabStop = true;
            this.HalfDigital.Text = "Half BIN/BOUT";
            this.HalfDigital.UseVisualStyleBackColor = true;
            this.HalfDigital.Click += new System.EventHandler(this.HalfDigital_Click);
            // 
            // HalfAnalog
            // 
            this.HalfAnalog.AutoSize = true;
            this.HalfAnalog.Location = new System.Drawing.Point(6, 73);
            this.HalfAnalog.Name = "HalfAnalog";
            this.HalfAnalog.Size = new System.Drawing.Size(125, 21);
            this.HalfAnalog.TabIndex = 0;
            this.HalfAnalog.TabStop = true;
            this.HalfAnalog.Text = "Half RIN/ROUT";
            this.HalfAnalog.UseVisualStyleBackColor = true;
            this.HalfAnalog.Click += new System.EventHandler(this.HalfAnalog_Click);
            // 
            // AllBOUTs
            // 
            this.AllBOUTs.AutoSize = true;
            this.AllBOUTs.Location = new System.Drawing.Point(6, 121);
            this.AllBOUTs.Name = "AllBOUTs";
            this.AllBOUTs.Size = new System.Drawing.Size(94, 21);
            this.AllBOUTs.TabIndex = 0;
            this.AllBOUTs.TabStop = true;
            this.AllBOUTs.Text = "All BOUTs";
            this.AllBOUTs.UseVisualStyleBackColor = true;
            this.AllBOUTs.Click += new System.EventHandler(this.AllBOUTs_Click);
            // 
            // AllROUTs
            // 
            this.AllROUTs.AutoSize = true;
            this.AllROUTs.Location = new System.Drawing.Point(6, 48);
            this.AllROUTs.Name = "AllROUTs";
            this.AllROUTs.Size = new System.Drawing.Size(95, 21);
            this.AllROUTs.TabIndex = 0;
            this.AllROUTs.TabStop = true;
            this.AllROUTs.Text = "All ROUTs";
            this.AllROUTs.UseVisualStyleBackColor = true;
            this.AllROUTs.Click += new System.EventHandler(this.AllROUTs_Click);
            // 
            // AllBINs
            // 
            this.AllBINs.AutoSize = true;
            this.AllBINs.Location = new System.Drawing.Point(6, 94);
            this.AllBINs.Name = "AllBINs";
            this.AllBINs.Size = new System.Drawing.Size(77, 21);
            this.AllBINs.TabIndex = 0;
            this.AllBINs.TabStop = true;
            this.AllBINs.Text = "All BINs";
            this.AllBINs.UseVisualStyleBackColor = true;
            this.AllBINs.Click += new System.EventHandler(this.AllBINs_Click);
            // 
            // BlockOptions
            // 
            this.BlockOptions.Enabled = false;
            this.BlockOptions.FormattingEnabled = true;
            this.BlockOptions.ItemHeight = 16;
            this.BlockOptions.Items.AddRange(new object[] {
            "RIN",
            "ROUT",
            "BIN",
            "BOUT"});
            this.BlockOptions.Location = new System.Drawing.Point(206, 12);
            this.BlockOptions.Name = "BlockOptions";
            this.BlockOptions.Size = new System.Drawing.Size(152, 116);
            this.BlockOptions.TabIndex = 1;
            this.BlockOptions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BlockOptions_MouseDown);
            // 
            // ConfigButton
            // 
            this.ConfigButton.Location = new System.Drawing.Point(240, 161);
            this.ConfigButton.Name = "ConfigButton";
            this.ConfigButton.Size = new System.Drawing.Size(85, 29);
            this.ConfigButton.TabIndex = 3;
            this.ConfigButton.Text = "Configure";
            this.ConfigButton.UseVisualStyleBackColor = true;
            this.ConfigButton.Click += new System.EventHandler(this.ConfigButton_Click);
            // 
            // BlockConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 231);
            this.Controls.Add(this.ConfigButton);
            this.Controls.Add(this.BlockOptions);
            this.Controls.Add(this.ConfigOptions);
            this.Controls.Add(this.FBM);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(572, 278);
            this.MinimumSize = new System.Drawing.Size(572, 278);
            this.Name = "BlockConfigurator";
            this.Text = "Configure ";
            this.Load += new System.EventHandler(this.BlockConfigurator_Load);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.BlockConfigurator_DragOver);
            this.FBMContextMenu.ResumeLayout(false);
            this.ConfigOptions.ResumeLayout(false);
            this.ConfigOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton AllRINs;
        private System.Windows.Forms.TreeView FBM;
        private System.Windows.Forms.GroupBox ConfigOptions;
        private System.Windows.Forms.RadioButton HalfAnalog;
        private System.Windows.Forms.RadioButton AllROUTs;
        private System.Windows.Forms.RadioButton AllBINs;
        private System.Windows.Forms.RadioButton AllBOUTs;
        private System.Windows.Forms.RadioButton HalfDigital;
        private System.Windows.Forms.RadioButton Manual;
        private System.Windows.Forms.ListBox BlockOptions;
        private System.Windows.Forms.Button ConfigButton;
        private System.Windows.Forms.ContextMenuStrip FBMContextMenu;
        private System.Windows.Forms.ToolStripMenuItem RemoveButton;
    }
}
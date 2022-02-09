namespace IOCheckoutTool
{
    partial class IOCheckout
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
            if(Handler != null)
            {
                Handler.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IOCheckout));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.IOMenu = new System.Windows.Forms.MenuStrip();
            this.ProjectsTab = new System.Windows.Forms.ToolStripMenuItem();
            this.NewProjectItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveButton = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteTab = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpTab = new System.Windows.Forms.ToolStripMenuItem();
            this.InstructionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReportButton = new System.Windows.Forms.ToolStripMenuItem();
            this.requestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VersionRequest = new System.Windows.Forms.ToolStripMenuItem();
            this.FeatureRequest = new System.Windows.Forms.ToolStripMenuItem();
            this.Current = new System.Windows.Forms.Label();
            this.DatabaseContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddCP = new System.Windows.Forms.ToolStripMenuItem();
            this.BuildAll = new System.Windows.Forms.ToolStripMenuItem();
            this.DatabaseBulkAddButton = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.VersionNumber = new System.Windows.Forms.ToolStripStatusLabel();
            this.BuildProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.BuildStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.DataBase = new System.Windows.Forms.TreeView();
            this.DatabaseImages = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.CurrentProject = new System.Windows.Forms.Label();
            this.FBMView = new System.Windows.Forms.DataGridView();
            this.Devices = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CPContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.BuildCP = new System.Windows.Forms.ToolStripMenuItem();
            this.CPBulkAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.FBMContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ConfigureFBM = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Loader = new System.Windows.Forms.OpenFileDialog();
            this.IOMenu.SuspendLayout();
            this.DatabaseContextMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FBMView)).BeginInit();
            this.CPContextMenu.SuspendLayout();
            this.FBMContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // IOMenu
            // 
            this.IOMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.IOMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProjectsTab,
            this.SaveButton,
            this.DeleteTab,
            this.HelpTab});
            this.IOMenu.Location = new System.Drawing.Point(0, 0);
            this.IOMenu.Name = "IOMenu";
            this.IOMenu.Size = new System.Drawing.Size(823, 28);
            this.IOMenu.TabIndex = 1;
            this.IOMenu.Text = "menuStrip1";
            // 
            // ProjectsTab
            // 
            this.ProjectsTab.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewProjectItem});
            this.ProjectsTab.Name = "ProjectsTab";
            this.ProjectsTab.Size = new System.Drawing.Size(75, 24);
            this.ProjectsTab.Text = "Projects";
            // 
            // NewProjectItem
            // 
            this.NewProjectItem.Name = "NewProjectItem";
            this.NewProjectItem.Size = new System.Drawing.Size(122, 26);
            this.NewProjectItem.Text = "New";
            this.NewProjectItem.Click += new System.EventHandler(this.NewProject_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(54, 24);
            this.SaveButton.Text = "Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DeleteTab
            // 
            this.DeleteTab.Name = "DeleteTab";
            this.DeleteTab.Size = new System.Drawing.Size(67, 24);
            this.DeleteTab.Text = "Delete";
            // 
            // HelpTab
            // 
            this.HelpTab.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.InstructionMenuItem,
            this.ReportButton,
            this.requestToolStripMenuItem});
            this.HelpTab.Name = "HelpTab";
            this.HelpTab.Size = new System.Drawing.Size(55, 24);
            this.HelpTab.Text = "Help";
            // 
            // InstructionMenuItem
            // 
            this.InstructionMenuItem.Name = "InstructionMenuItem";
            this.InstructionMenuItem.Size = new System.Drawing.Size(173, 26);
            this.InstructionMenuItem.Text = "Instructions";
            this.InstructionMenuItem.Click += new System.EventHandler(this.InstructionMenuItem_Click);
            // 
            // ReportButton
            // 
            this.ReportButton.Name = "ReportButton";
            this.ReportButton.Size = new System.Drawing.Size(173, 26);
            this.ReportButton.Text = "Report Issue";
            this.ReportButton.Visible = false;
            this.ReportButton.Click += new System.EventHandler(this.ReportButton_Click);
            // 
            // requestToolStripMenuItem
            // 
            this.requestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VersionRequest,
            this.FeatureRequest});
            this.requestToolStripMenuItem.Name = "requestToolStripMenuItem";
            this.requestToolStripMenuItem.Size = new System.Drawing.Size(173, 26);
            this.requestToolStripMenuItem.Text = "Request";
            // 
            // VersionRequest
            // 
            this.VersionRequest.Name = "VersionRequest";
            this.VersionRequest.Size = new System.Drawing.Size(202, 26);
            this.VersionRequest.Text = "Updated Version";
            this.VersionRequest.Click += new System.EventHandler(this.VersionRequest_Click);
            // 
            // FeatureRequest
            // 
            this.FeatureRequest.Name = "FeatureRequest";
            this.FeatureRequest.Size = new System.Drawing.Size(202, 26);
            this.FeatureRequest.Text = "Feature";
            this.FeatureRequest.Click += new System.EventHandler(this.FeatureRequest_Click);
            // 
            // Current
            // 
            this.Current.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Current.Location = new System.Drawing.Point(3, 0);
            this.Current.Name = "Current";
            this.Current.Size = new System.Drawing.Size(82, 38);
            this.Current.TabIndex = 3;
            this.Current.Text = "Loaded Project:";
            // 
            // DatabaseContextMenu
            // 
            this.DatabaseContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.DatabaseContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddCP,
            this.BuildAll,
            this.DatabaseBulkAddButton});
            this.DatabaseContextMenu.Name = "contextMenuStrip1";
            this.DatabaseContextMenu.Size = new System.Drawing.Size(139, 76);
            // 
            // AddCP
            // 
            this.AddCP.Name = "AddCP";
            this.AddCP.Size = new System.Drawing.Size(138, 24);
            this.AddCP.Text = "Add";
            this.AddCP.Click += new System.EventHandler(this.AddCP_Click);
            // 
            // BuildAll
            // 
            this.BuildAll.Name = "BuildAll";
            this.BuildAll.Size = new System.Drawing.Size(138, 24);
            this.BuildAll.Text = "Build";
            this.BuildAll.Click += new System.EventHandler(this.BuildAll_Click);
            // 
            // DatabaseBulkAddButton
            // 
            this.DatabaseBulkAddButton.Name = "DatabaseBulkAddButton";
            this.DatabaseBulkAddButton.Size = new System.Drawing.Size(138, 24);
            this.DatabaseBulkAddButton.Text = "Bulk Add";
            this.DatabaseBulkAddButton.Click += new System.EventHandler(this.BulkAdd_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(91, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(545, 38);
            this.label1.TabIndex = 5;
            this.label1.Text = "System";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VersionNumber,
            this.BuildProgress,
            this.BuildStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 335);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(823, 26);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // VersionNumber
            // 
            this.VersionNumber.Name = "VersionNumber";
            this.VersionNumber.Size = new System.Drawing.Size(155, 20);
            this.VersionNumber.Text = "IO Checkout Version - ";
            // 
            // BuildProgress
            // 
            this.BuildProgress.Name = "BuildProgress";
            this.BuildProgress.Size = new System.Drawing.Size(100, 18);
            this.BuildProgress.Visible = false;
            // 
            // BuildStatus
            // 
            this.BuildStatus.Name = "BuildStatus";
            this.BuildStatus.Size = new System.Drawing.Size(45, 20);
            this.BuildStatus.Text = "XXXX";
            this.BuildStatus.Visible = false;
            // 
            // DataBase
            // 
            this.DataBase.AllowDrop = true;
            this.DataBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataBase.ImageIndex = 0;
            this.DataBase.ImageList = this.DatabaseImages;
            this.DataBase.Location = new System.Drawing.Point(91, 41);
            this.DataBase.Name = "DataBase";
            this.tableLayoutPanel1.SetRowSpan(this.DataBase, 2);
            this.DataBase.SelectedImageIndex = 0;
            this.DataBase.Size = new System.Drawing.Size(545, 263);
            this.DataBase.TabIndex = 4;
            this.DataBase.DragDrop += new System.Windows.Forms.DragEventHandler(this.DataBase_DragDrop);
            this.DataBase.DragEnter += new System.Windows.Forms.DragEventHandler(this.DataBase_DragEnter);
            this.DataBase.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DataBase_MouseDown);
            // 
            // DatabaseImages
            // 
            this.DatabaseImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("DatabaseImages.ImageStream")));
            this.DatabaseImages.TransparentColor = System.Drawing.Color.Transparent;
            this.DatabaseImages.Images.SetKeyName(0, "Default Image.png");
            this.DatabaseImages.Images.SetKeyName(1, "Database Image.png");
            this.DatabaseImages.Images.SetKeyName(2, "CP_FBM Image.png");
            this.DatabaseImages.Images.SetKeyName(3, "Compound Image.png");
            this.DatabaseImages.Images.SetKeyName(4, "ECB Image.png");
            this.DatabaseImages.Images.SetKeyName(5, "Input Image.png");
            this.DatabaseImages.Images.SetKeyName(6, "Output Image.png");
            this.DatabaseImages.Images.SetKeyName(7, "Multiple Input Image.png");
            this.DatabaseImages.Images.SetKeyName(8, "Multiple Output Image.png");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.DataBase, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.CurrentProject, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Current, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.FBMView, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 28);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 221F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(823, 307);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // CurrentProject
            // 
            this.CurrentProject.AutoSize = true;
            this.CurrentProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CurrentProject.Location = new System.Drawing.Point(3, 38);
            this.CurrentProject.Name = "CurrentProject";
            this.CurrentProject.Size = new System.Drawing.Size(82, 22);
            this.CurrentProject.TabIndex = 9;
            this.CurrentProject.Text = "XXXXXXXXX";
            // 
            // FBMView
            // 
            this.FBMView.AllowDrop = true;
            this.FBMView.AllowUserToAddRows = false;
            this.FBMView.AllowUserToDeleteRows = false;
            this.FBMView.AllowUserToOrderColumns = true;
            this.FBMView.AllowUserToResizeColumns = false;
            this.FBMView.AllowUserToResizeRows = false;
            this.FBMView.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.FBMView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.FBMView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FBMView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Devices,
            this.Number});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.FBMView.DefaultCellStyle = dataGridViewCellStyle2;
            this.FBMView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FBMView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.FBMView.Location = new System.Drawing.Point(642, 41);
            this.FBMView.Name = "FBMView";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.FBMView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.FBMView.RowHeadersVisible = false;
            this.FBMView.RowHeadersWidth = 51;
            this.FBMView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tableLayoutPanel1.SetRowSpan(this.FBMView, 2);
            this.FBMView.RowTemplate.Height = 24;
            this.FBMView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.FBMView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.FBMView.Size = new System.Drawing.Size(178, 263);
            this.FBMView.TabIndex = 10;
            this.FBMView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FBMView_MouseDown);
            // 
            // Devices
            // 
            this.Devices.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Devices.HeaderText = "FBMs";
            this.Devices.MinimumWidth = 6;
            this.Devices.Name = "Devices";
            this.Devices.ReadOnly = true;
            this.Devices.Width = 72;
            // 
            // Number
            // 
            this.Number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Number.HeaderText = "Number";
            this.Number.MinimumWidth = 6;
            this.Number.Name = "Number";
            // 
            // CPContextMenu
            // 
            this.CPContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.CPContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BuildCP,
            this.CPBulkAdd});
            this.CPContextMenu.Name = "CPContextMenu";
            this.CPContextMenu.Size = new System.Drawing.Size(139, 52);
            // 
            // BuildCP
            // 
            this.BuildCP.Name = "BuildCP";
            this.BuildCP.Size = new System.Drawing.Size(138, 24);
            this.BuildCP.Text = "Build";
            this.BuildCP.Click += new System.EventHandler(this.BuildCP_Click);
            // 
            // CPBulkAdd
            // 
            this.CPBulkAdd.Name = "CPBulkAdd";
            this.CPBulkAdd.Size = new System.Drawing.Size(138, 24);
            this.CPBulkAdd.Text = "Bulk Add";
            this.CPBulkAdd.Click += new System.EventHandler(this.BulkAdd_Click);
            // 
            // FBMContextMenu
            // 
            this.FBMContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.FBMContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConfigureFBM,
            this.RemoveItem});
            this.FBMContextMenu.Name = "FBMContextMenu";
            this.FBMContextMenu.Size = new System.Drawing.Size(144, 52);
            // 
            // ConfigureFBM
            // 
            this.ConfigureFBM.Name = "ConfigureFBM";
            this.ConfigureFBM.Size = new System.Drawing.Size(143, 24);
            this.ConfigureFBM.Text = "Configure";
            this.ConfigureFBM.Click += new System.EventHandler(this.ConfigureFBM_Click);
            // 
            // RemoveItem
            // 
            this.RemoveItem.Name = "RemoveItem";
            this.RemoveItem.Size = new System.Drawing.Size(143, 24);
            this.RemoveItem.Text = "Remove";
            this.RemoveItem.Click += new System.EventHandler(this.RemoveItem_Click);
            // 
            // Loader
            // 
            this.Loader.Filter = "CSV files (*.csv)|*.csv";
            this.Loader.Title = "Select the Nest Load to add";
            // 
            // IOCheckout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(823, 361);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.IOMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.IOMenu;
            this.MinimumSize = new System.Drawing.Size(841, 408);
            this.Name = "IOCheckout";
            this.Text = "IO Checkout";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IOCheckout_FormClosing);
            this.Load += new System.EventHandler(this.IOCheckout_Load);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.IOCheckout_DragOver);
            this.IOMenu.ResumeLayout(false);
            this.IOMenu.PerformLayout();
            this.DatabaseContextMenu.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FBMView)).EndInit();
            this.CPContextMenu.ResumeLayout(false);
            this.FBMContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip IOMenu;
        private System.Windows.Forms.ToolStripMenuItem ProjectsTab;
        private System.Windows.Forms.ToolStripMenuItem NewProjectItem;
        private System.Windows.Forms.Label Current;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip DatabaseContextMenu;
        private System.Windows.Forms.ToolStripMenuItem SaveButton;
        private System.Windows.Forms.ToolStripMenuItem AddCP;
        private System.Windows.Forms.ToolStripMenuItem BuildAll;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel VersionNumber;
        private System.Windows.Forms.ToolStripMenuItem HelpTab;
        private System.Windows.Forms.ToolStripMenuItem ReportButton;
        private System.Windows.Forms.ToolStripMenuItem requestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem VersionRequest;
        private System.Windows.Forms.ToolStripMenuItem FeatureRequest;
        private System.Windows.Forms.TreeView DataBase;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem InstructionMenuItem;
        private System.Windows.Forms.Label CurrentProject;
        private System.Windows.Forms.ToolStripProgressBar BuildProgress;
        private System.Windows.Forms.ToolStripStatusLabel BuildStatus;
        private System.Windows.Forms.ToolStripMenuItem DeleteTab;
        private System.Windows.Forms.ImageList DatabaseImages;
        private System.Windows.Forms.DataGridView FBMView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Devices;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.ContextMenuStrip CPContextMenu;
        private System.Windows.Forms.ToolStripMenuItem BuildCP;
        private System.Windows.Forms.ContextMenuStrip FBMContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ConfigureFBM;
        private System.Windows.Forms.ToolStripMenuItem RemoveItem;
        private System.Windows.Forms.ToolStripMenuItem CPBulkAdd;
        private System.Windows.Forms.OpenFileDialog Loader;
        private System.Windows.Forms.ToolStripMenuItem DatabaseBulkAddButton;
    }
}


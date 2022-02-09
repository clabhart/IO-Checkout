using IOCheckoutTool.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace IOCheckoutTool
{
    public partial class IOCheckout : Form
    {
        public IOCheckout()
        {
            InitializeComponent();
        }

        private DirectoryInfo Projects;
        private DirectoryInfo Project;
        private DirectoryInfo SaveLocation;
        private XmlDocument CPFile;
        private ErrorHandler Handler;
        private readonly string ProjectsFolder = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "IO Checkout Files");

        private void IOCheckout_Load(object sender, EventArgs e)
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            //string[] oldpath = new string[] { Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\")) + "Projects",
            //Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\")) + "IO Checkout Projects" };
            if (!Directory.Exists(ProjectsFolder))
            {
                Directory.CreateDirectory(ProjectsFolder);
            }
            //if (Directory.Exists(oldpath[0]) && !Directory.Exists(ProjectsFolder))
            //{
            //    Directory.Move(oldpath[0], ProjectsFolder);
            //}
            //VersionNumber.Text += string.Format(CultureInfo.CurrentCulture, "Verison {0}", Environment.Version.ToString(4));
            Projects = new DirectoryInfo(ProjectsFolder);
            CurrentProject.Text = string.Empty;
            LoadFBMs();
            LoadProjects(ProjectsFolder);
            LoadDeletes(ProjectsFolder);
        }

        private static readonly string[] fbms =
        {
            "FBM 201", "FBM 202", "FBM 203","FBM 204","FBM 206","FBM 206B","FBM 204","FBM 207","FBM 207B","FBM 214","FBM 214B","FBM 215","FBM 216",
            "FBM 217","FBM 217R","FBM 218","FBM 220","FBM 221","FBM 223","FBM 224","FBM 228","FBM 230","FBM 231","FBM 232","FBM 233",
            "FBM 237","FBM 238","FBM 239","FBM 240","FBM 240R","FBM 241","FBM 242","FBM 244","FBM 245", "FBM 247", "FBM 248"
        };

        private void LoadProjects(string path)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                foreach (string directory in Directory.GetDirectories(path))
                {
                    ToolStripMenuItem project = new(Path.GetFileNameWithoutExtension(directory))
                    {
                        Name = Path.GetFileNameWithoutExtension(directory)
                    };
                    if (!ProjectsTab.DropDownItems.Cast<ToolStripMenuItem>().Any(x => x.Text == project.Name))
                    {
                        project.Click += LoadProject;
                        ProjectsTab.DropDownItems.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                Handler.LogError(ex.Message);
            }
            Cursor = Cursors.Default;
        }

        private void LoadDeletes(string path)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                foreach (string directory in Directory.GetDirectories(path))
                {
                    ToolStripMenuItem project = new(Path.GetFileNameWithoutExtension(directory))
                    {
                        Name = Path.GetFileNameWithoutExtension(directory)
                    };
                    if (!DeleteTab.DropDownItems.Cast<ToolStripMenuItem>().Any(x => x.Text == project.Name))
                    {
                        project.Click += DeleteProject;
                        DeleteTab.DropDownItems.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                Handler.LogError(ex.Message);
            }
            Cursor = Cursors.Default;
        }

        private void LoadFBMs()
        {
            foreach (string fbm in fbms)
            {
                FBMView.Rows.Add(fbm, "0");
            }
        }

        private void LoadProject(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in ProjectsTab.DropDownItems)
            {
                item.Checked = false;
            }
            if (Handler != null)
            {
                Handler.Dispose();
            }
            Cursor = Cursors.WaitCursor;
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            Project = new DirectoryInfo(Path.Combine(Projects.FullName, menuitem.Name));
            string errorpath = Path.Combine(ProjectsFolder, Project.Name, "Logs");
            DirectoryInfo info;
            if (Directory.Exists(errorpath))
            {
                info = new DirectoryInfo(errorpath);
            }
            else
            {
                info = Directory.CreateDirectory(errorpath);
            }
            Handler = new(info);
            LoadDatabase(out bool loaded);
            if (!loaded) { return; }
            CurrentProject.Text = Project.Name;
            menuitem.Checked = true;
            Cursor = Cursors.Default;
        }

        private void LoadDatabase(out bool loaded)
        {
            Cursor = Cursors.WaitCursor;
            DataBase.Nodes.Clear();
            StreamReader reader = new(Path.Combine(Project.FullName, "Project", "CPs.txt"));
            TreeNode tree = DataBase.Nodes.Add("Database", Project.Name);
            tree.ImageIndex = 1;
            tree.SelectedImageIndex = 1;
            tree.StateImageIndex = 1;
            string path = Path.Combine(Project.FullName, "Project", string.Concat(Project.Name, ".xml"));
            while (!reader.EndOfStream)
            {
                string cp = reader.ReadLine();
                TreeNode node = tree.Nodes.Add("CP", cp);
                node.StateImageIndex = 1;
                node.ImageIndex = 2;
                node.SelectedImageIndex = 2;
                TreeNode ecbnode = node.Nodes.Add("ECB", string.Concat(node.Text, "_ECB"));
                ecbnode.ImageIndex = 3;
                ecbnode.SelectedImageIndex = 3;
                if (File.Exists(path))
                {
                    XmlReader xreader = XmlReader.Create(path, new XmlReaderSettings() { XmlResolver = null });
                    while (xreader.LocalName != "CPs")
                    {
                        xreader.Read();
                    }
                    XmlDocument databasefile = new()
                    {
                        XmlResolver = null
                    };
                    databasefile.Load(xreader);
                    int i = 0;
                    string cppath = string.Concat("//CP[@Name='", cp, "']");
                    string ecbpath = string.Concat("CPs", cppath, "/ECBs");
                    string fbmpath = string.Concat("CPs", cppath, "/FBMs");
                    foreach (XmlNode xmlnode in databasefile.SelectSingleNode(ecbpath).ChildNodes)
                    {
                        TreeNode ecb = ecbnode.Nodes.Add("ECB", xmlnode.Attributes["Name"].Value);
                        ecb.Nodes.Add("Type", xmlnode.Attributes["Type"].Value);
                        ecb.ImageIndex = 4;
                        ecb.SelectedImageIndex = 4;
                        ++i;
                    }
                    i = 0;
                    foreach (XmlNode xmlnode in databasefile.SelectSingleNode(fbmpath).ChildNodes)
                    {
                        TreeNode fbmnode = node.Nodes.Add("FBM", xmlnode.Attributes["Name"].Value);
                        fbmnode.ImageIndex = 2;
                        fbmnode.SelectedImageIndex = 2;
                        fbmnode.Nodes.Add("Type", xmlnode.Attributes["Type"].Value);
                        fbmnode.Nodes.Add("Channel", xmlnode.Attributes["Channel"].Value);
                        if (xmlnode.Attributes["Redundant"] != null)
                        {
                            fbmnode.Nodes.Add("Redundant", xmlnode.Attributes["Redundant"].Value);
                        }
                        if (xmlnode.SelectSingleNode("Compound") != null)
                        {
                            TreeNode compound = fbmnode.Nodes.Add("Compound", xmlnode.SelectSingleNode("Compound").Attributes["Name"].Value);
                            compound.ImageIndex = 3;
                            compound.SelectedImageIndex = 3;
                            foreach (XmlNode block in xmlnode.SelectSingleNode("Compound").ChildNodes)
                            {
                                TreeNode blocknode = compound.Nodes.Add("Block", block.Attributes["Type"].Value);
                                blocknode.Nodes.Add("Name", block.Attributes["Name"].Value);
                                switch (block.Attributes["Type"].Value)
                                {
                                    case "AIN":
                                    case "BIN":
                                    case "CIN":
                                    case "RIN":
                                    case "RINR":
                                        blocknode.ImageIndex = 5;
                                        blocknode.SelectedImageIndex = 5;
                                        break;

                                    case "AOUT":
                                    case "BOUT":
                                    case "COUT":
                                    case "ROUT":
                                    case "ROUTR":
                                        blocknode.ImageIndex = 6;
                                        blocknode.SelectedImageIndex = 6;
                                        break;

                                    case "MAIN":
                                    case "MCIN":
                                        blocknode.ImageIndex = 7;
                                        blocknode.SelectedImageIndex = 7;
                                        break;

                                    case "MCOUT":
                                        blocknode.ImageIndex = 8;
                                        blocknode.SelectedImageIndex = 8;
                                        break;
                                }
                            }
                        }
                        ++i;
                    }
                    xreader.Close();
                    xreader.Dispose();
                }
            }
            tree.Expand();
            DataBase.Sort();
            reader.Close();
            reader.Dispose();
            loaded = true;
            Cursor = Cursors.Default;
        }

        private void DeleteProject(object sender, EventArgs e)
        {
            DataBase.Nodes.Clear();
            CurrentProject.Text = string.Empty;
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            Directory.Delete(Path.Combine(Projects.FullName, menuitem.Name), true);
            LoadProjects(ProjectsFolder);
            LoadDeletes(ProjectsFolder);
        }

        private void NewProject_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            NewProject project = new(Projects) { StartPosition = FormStartPosition.CenterParent };
            project.ShowDialog();
            Project = project.LoadedProject();
            project.Dispose();
            LoadProjects(Projects.FullName);
            LoadDeletes(Projects.FullName);
            ToolStripMenuItem menuitem;
            foreach (ToolStripMenuItem item in ProjectsTab.DropDownItems)
            {
                if (item.Name == Project.Name)
                {
                    menuitem = item;
                    menuitem.Checked = true;
                    break;
                }
            }
            LoadDatabase(out bool loaded);
            if (!loaded) { return; }
            CurrentProject.Text = Project.Name;
            SaveButton.Visible = true;
            Cursor = Cursors.Default;
        }

        private void DataBase_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode dropnode = DataBase.GetNodeAt(DataBase.PointToClient(new Point(e.X, e.Y)));
            if (dropnode.Name.Contains("CP", StringComparison.InvariantCultureIgnoreCase) && dropnode != null)
            {
                List<(string, int)> devices = (List<(string, int)>)e.Data.GetData(typeof(List<(string, int)>));
                devices.ForEach(delegate ((string, int) fbm)
                {
                    for (int j = 1; j <= fbm.Item2; j++)
                    {
                        dropnode.Nodes.Add("FBM", fbm.Item1);
                        dropnode.ImageIndex = 2;
                        dropnode.SelectedImageIndex = 2;
                    }
                });
                dropnode.Expand();
            }
        }

        private void DataBase_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode selected = DataBase.GetNodeAt(e.X, e.Y);
            DataBase.SelectedNode = selected;
            if (selected != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    string nodetype = Regex.Replace(DataBase.SelectedNode.Name, @"\d", string.Empty);
                    switch (nodetype)
                    {
                        case "Database":
                            DatabaseContextMenu.Show(DataBase, new Point(e.X, e.Y));
                            break;

                        case "CP":
                            CPContextMenu.Show(DataBase, new Point(e.X, e.Y));
                            break;

                        case "FBM":
                            FBMContextMenu.Show(DataBase, new Point(e.X, e.Y));
                            break;
                    }
                }
            }
        }

        private void FBMView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                List<(string, int)> ts = new();
                foreach (DataGridViewRow selectedrow in FBMView.SelectedRows)
                {
                    string device = selectedrow.Cells["Devices"].Value.ToString();
                    int number = Convert.ToInt32(selectedrow.Cells["Number"].Value, CultureInfo.InvariantCulture);
                    ts.Add((device, number));
                }
                FBMView.DoDragDrop(ts, DragDropEffects.Copy);
            }
        }

        private void DataBase_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void IOCheckout_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void RemoveItem_Click(object sender, EventArgs e)
        {
            string filename = Path.Combine(Project.FullName, string.Concat(Project.Name, ".xml"));
            string fbm = DataBase.SelectedNode.Text;
            string xpath = string.Format(CultureInfo.CurrentCulture, "//FBM[@Name='{0}']", fbm);
            XmlNode fbmnode;
            XmlDocument project = new()
            {
                XmlResolver = null
            };
            XmlReader reader = XmlReader.Create(filename, new XmlReaderSettings() { XmlResolver = null });
            while (reader.LocalName != "CPs")
            {
                reader.Read();
            }
            project.Load(reader);
            fbmnode = project.SelectSingleNode(xpath);
            DataBase.SelectedNode.Remove();
            fbmnode.ParentNode.RemoveChild(fbmnode);
            TreeNode cp = DataBase.SelectedNode.Parent;
            List<TreeNode> ecbs = cp.Nodes[0].Nodes.OfType<TreeNode>().Where(n => n.Name == "ECB" && n.Text.Contains(fbm[0..^1]
                , StringComparison.InvariantCultureIgnoreCase)).ToList();
            foreach (TreeNode ecb in ecbs)
            {
                xpath = string.Format(CultureInfo.CurrentCulture, "//ECB[@Name='{0}']", ecb.Text);
                XmlNode ecbnode = project.SelectSingleNode(xpath);
                DataBase.Nodes.Remove(ecb);
                ecbnode.ParentNode.RemoveChild(ecbnode);
            }
            reader.Dispose();
            project.Save(filename);
        }

        private void AddCP_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            StreamWriter writer = new(Path.Combine(Project.FullName, "CPs.txt"), append: true);
            string input;
            InputBox inputbox = new("Input CP Name");
            inputbox.ShowDialog();
            input = inputbox.Output;
            writer.WriteLine(input);
            inputbox.Dispose();
            writer.Close();
            writer.Dispose();
            TreeNode node = DataBase.Nodes["Database"].Nodes.Add("CP", input);
            TreeNode ecb = node.Nodes.Add("ECB", string.Concat(node.Text, "_ECB"));
            node.ImageIndex = 2;
            node.SelectedImageIndex = 2;
            ecb.ImageIndex = 3;
            ecb.SelectedImageIndex = 3;
            Cursor = Cursors.Default;
        }

        private void BuildAll_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                Save();
                foreach (TreeNode cp in DataBase.SelectedNode.Nodes)
                {
                    string savepath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "IO Checkout Files", Project.Name);
                    List<TreeNode> fbms = cp.Nodes.Cast<TreeNode>().Where(n => n.Name == "FBM").ToList();
                    if (Directory.Exists(savepath))
                    {
                        SaveLocation = new DirectoryInfo(savepath);
                    }
                    else
                    {
                        SaveLocation = Directory.CreateDirectory(savepath);
                    }
                    if (FBMCheck(fbms))
                    {
                        string message = string.Format(CultureInfo.CurrentCulture, "Please Configure All FBMs in {0}", cp.Text);
                        _ = MessageBox.Show(message);
                        return;
                    }
                    BuildCPFile(fbms, cp.Text);
                }
                OpenExplorer(SaveLocation.FullName);
            }
            catch (Exception ex)
            {
                Handler.LogError(ex.Message);
            }
            Cursor = Cursors.Default;
        }

        private void ConfigureFBM_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            TreeNode node = DataBase.SelectedNode;
            TreeNode ecbnode = node.Parent.Nodes["ECB"];
            TreeNode compound;
            string fbmname;
            string redundantname;
            string ecbprefix;
            switch (node.Text)
            {
                case "FBM 201":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB1", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    break;

                case "FBM 202":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB1", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    break;

                case "FBM 203":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB1", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    break;

                case "FBM 204":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB1", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 6)
                    {
                        fbmname = fbmname[..6];
                    }
                    for (int i = 5; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "AOUT", compound);
                    }
                    break;

                case "FBM 206":
                case "FBM 206B":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB4", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    break;

                case "FBM 207":
                case "FBM 207B":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCIN", compound);
                    break;

                case "FBM 214":
                case "FBM 214B":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "RIN", compound);
                    }
                    break;

                case "FBM 215":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "ROUT", compound);
                    }
                    break;

                case "FBM 216":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "RIN", compound);
                    }
                    break;

                case "FBM 217":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCIN", compound);
                    break;

                case "FBM 217R":
                    AddFBMNode(node, out fbmname, out redundantname, true);
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddECB(redundantname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    if (fbmname.Length > 5)
                    {
                        fbmname = fbmname[..5];
                    }
                    for (int i = 1; i <= 32; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "CINR", compound);
                    }
                    break;

                case "FBM 218":
                    AddFBMNode(node, out fbmname, out redundantname);
                    AddECB(fbmname, "ECB202", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "ROUT", compound);
                    }
                    break;

                case "FBM 220":
                case "FBM 221":
                case "FBM 223":
                case "FBM 224":
                case "FBM 228":
                case "FBM 230":
                case "FBM 232":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB200", ecbnode);
                    break;

                case "FBM 231":
                case "FBM 233":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB201", ecbnode);
                    break;

                case "FBM 237":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB53", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 6)
                    {
                        fbmname = fbmname[..6];
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "AOUT", compound);
                    }
                    break;

                case "FBM 238":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(string.Concat(fbmname, "_CIN"), "MCIN", compound);
                    AddBlock(string.Concat(fbmname, "_COUT"), "MCOUT", compound);
                    break;

                case "FBM 239":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    if (fbmname.Length > 8)
                    {
                        fbmname = fbmname[..8];
                    }
                    AddBlock(string.Concat(fbmname, "_CIN"), "MCIN", compound);
                    AddBlock(string.Concat(fbmname, "_COUT"), "MCOUT", compound);
                    break;

                case "FBM 240":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    if (fbmname.Length > 6)
                    {
                        fbmname = fbmname[..6];
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "COUT", compound);
                    }
                    break;

                case "FBM 240R":
                    AddFBMNode(node, out fbmname, out redundantname, true);
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCOUT", compound);
                    if (fbmname.Length > 6)
                    {
                        fbmname = fbmname[..6];
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "CINR", compound);
                    }
                    break;

                case "FBM 241":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCOUT", compound);
                    if (fbmname.Length > 5)
                    {
                        fbmname = fbmname[..5];
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "CIN", compound);
                    }
                    for (int i = 9; i <= 16; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "COUT", compound);
                    }
                    break;

                case "FBM 242":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCOUT", compound);
                    break;

                case "FBM 244":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 4; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "RIN", compound);
                    }
                    for (int i = 5; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "ROUT", compound);
                    }
                    break;

                case "FBM 245":
                    AddFBMNode(node, out fbmname, out redundantname, true);
                    AddECB(fbmname, "ECB202", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    string redundantecb = string.Empty;
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    if (redundantname.Length > 10)
                    {
                        redundantecb = redundantname[..10];
                    }
                    else
                    {
                        redundantecb = redundantname;
                    }
                    for (int i = 1; i <= 4; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddECB(string.Concat(redundantecb, "_", i.ToString(CultureInfo.InvariantCulture)), "ECB201", ecbnode);
                        AddBlock(name, "RINR", compound);
                    }
                    for (int i = 5; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddECB(string.Concat(redundantecb, "_", i.ToString(CultureInfo.InvariantCulture)), "ECB201", ecbnode);
                        AddBlock(name, "ROUTR", compound);
                    }
                    break;

                case "FBM 247":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    BlockConfigurator configurator = new(fbmname)
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                    int j = 1;
                    configurator.ShowDialog();
                    if (configurator.Analog)
                    {
                        AddBlock(fbmname, "MAIN", compound);
                    }
                    else
                    {
                        AddBlock(fbmname, "MCIN", compound);
                    }
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    configurator.Blocks.ForEach(delegate (string block)
                    {
                        AddECB(string.Concat(ecbprefix, "_", j.ToString(CultureInfo.InvariantCulture)), "ECB201", ecbnode);
                        AddBlock(string.Concat(ecbprefix, "_", j.ToString(CultureInfo.InvariantCulture)), block, compound);
                        ++j;
                    });
                    configurator.Dispose();
                    break;

                case "FBM 248":
                    AddFBMNode(node, out fbmname, out _);
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    configurator = new BlockConfigurator(fbmname)
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                    j = 1;
                    configurator.ShowDialog();
                    if (configurator.Analog)
                    {
                        AddBlock(fbmname, "MAIN", compound);
                    }
                    else
                    {
                        AddBlock(fbmname, "MCIN", compound);
                    }
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    configurator.Blocks.ForEach(delegate (string block)
                    {
                        AddECB(string.Concat(ecbprefix, "_", j.ToString(CultureInfo.InvariantCulture)), "ECB201", ecbnode);
                        AddBlock(string.Concat(ecbprefix, "_", j.ToString(CultureInfo.InvariantCulture)), block, compound);
                        ++j;
                    });
                    configurator.Dispose();
                    break;
            }
            node.ImageIndex = 2;
            node.SelectedImageIndex = 2;
            Cursor = Cursors.Default;
        }

        private static void AddFBMNode(TreeNode node, out string fbmname, out string redundantname, bool redundant = false)
        {
            Configure configure;
            configure = new Configure(redundant)
            {
                StartPosition = FormStartPosition.CenterParent,
                Text = node.Text
            };
            configure.ShowDialog();
            string name = configure.FBMName;
            node.Text = name;
            node.Nodes.Add("Type", configure.Text);
            node.Nodes.Add("Channel", string.Concat("CH ", configure.Channel));
            if (redundant)
            {
                redundantname = configure.RedundantName;
                node.Nodes.Add("Redundant", configure.RedundantName);
            }
            else
            {
                redundantname = string.Empty;
            }
            node.ImageIndex = 2;
            node.SelectedImageIndex = 2;
            fbmname = name;
            configure.Dispose();
        }

        private static void AddECB(string name, string ECBType, TreeNode ecbnode)
        {
            TreeNode ecb = ecbnode.Nodes.Add("ECB", name);
            ecb.Nodes.Add("Type", ECBType);
            ecb.ImageIndex = 4;
            ecb.SelectedImageIndex = 4;
        }

        private static void AddCompound(string name, TreeNode node, out TreeNode compound)
        {
            compound = node.Nodes.Add("Compound", string.Concat(name, "_IO"));
            compound.ImageIndex = 3;
            compound.SelectedImageIndex = 3;
        }

        private static void AddBlock(string name, string blocktype, TreeNode compound)
        {
            TreeNode block = compound.Nodes.Add("Block", blocktype);
            block.Nodes.Add("Name", name);
            switch (blocktype)
            {
                case "AIN":
                case "BIN":
                case "CIN":
                case "RIN":
                case "RINR":
                    block.ImageIndex = 5;
                    block.SelectedImageIndex = 5;
                    break;

                case "AOUT":
                case "BOUT":
                case "COUT":
                case "ROUT":
                case "ROUTR":
                    block.ImageIndex = 6;
                    block.SelectedImageIndex = 6;
                    break;

                case "MAIN":
                case "MCIN":
                    block.ImageIndex = 7;
                    block.SelectedImageIndex = 7;
                    break;

                case "MCOUT":
                    block.ImageIndex = 8;
                    block.SelectedImageIndex = 8;
                    break;
            }
        }

        private void BuildCP_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Save();
            string savepath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "IO Checkout Files", Project.Name);
            List<TreeNode> fbms = DataBase.SelectedNode.Nodes.Cast<TreeNode>().Where(n => n.Name == "FBM").ToList();
            if (Directory.Exists(savepath))
            {
                SaveLocation = new DirectoryInfo(savepath);
            }
            else
            {
                SaveLocation = Directory.CreateDirectory(savepath);
            }
            if (FBMCheck(fbms))
            {
                string message = string.Format(CultureInfo.CurrentCulture, "Please Configure All FBMs in {0}", DataBase.SelectedNode.Text);
                _ = MessageBox.Show(message);
                return;
            }
            BuildCPFile(fbms, DataBase.SelectedNode.Text);
            //OpenExplorer(SaveLocation.FullName);
            Cursor = Cursors.Default;
        }

        private void BulkAdd_Click(object sender, EventArgs e)
        {
            if (Loader.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in Loader.FileNames)
                {
                    using StreamReader reader = new(file);
                    _ = reader.ReadLine();
                    BulkAdd(reader);
                }
            }
        }

        private void BulkAdd(StreamReader reader)
        {
            using (StreamWriter writer = new(Path.Combine(Project.FullName, "CPs.txt"), append: true))
            {
                TreeNode node;
                while (!reader.EndOfStream)
                {
                    string[] fbm = reader.ReadLine().Split(',');
                    string cp = fbm[0];
                    string ch = fbm[1];
                    string type = fbm[2].ToUpperInvariant();
                    string name = fbm[3];
                    string redundant = fbm[4];
                    if (CPCheck(DataBase.Nodes["Database"].Nodes, cp, out TreeNode cpnode))
                    {
                        node = cpnode.Nodes.Add("FBM", name);
                        node.Nodes.Add("Type", type);
                        node.Nodes.Add("Channel", string.Concat("CH ", ch));
                        if (!string.IsNullOrEmpty(redundant))
                        {
                            node.Nodes.Add("Redundant", redundant);
                        }
                        node.ImageIndex = 2;
                        node.SelectedImageIndex = 2;
                    }
                    else
                    {
                        writer.WriteLine(cp);
                        cpnode = DataBase.Nodes["Database"].Nodes.Add("CP", cp);
                        TreeNode ecb = cpnode.Nodes.Add("ECB", string.Concat(cpnode.Text, "_ECB"));
                        cpnode.ImageIndex = 2;
                        cpnode.SelectedImageIndex = 2;
                        ecb.ImageIndex = 3;
                        ecb.SelectedImageIndex = 3;
                        node = cpnode.Nodes.Add("FBM", name);
                        node.Text = name;
                        node.Nodes.Add("Type", type);
                        node.Nodes.Add("Channel", string.Concat("CH ", ch));
                        if (!string.IsNullOrEmpty(redundant))
                        {
                            node.Nodes.Add("Redundant", redundant);
                        }
                        node.ImageIndex = 2;
                        node.SelectedImageIndex = 2;
                    }
                    BulkAddFBM(cpnode.Nodes["ECB"], node, name, redundant);
                }
            }
            DataBase.Sort();
        }

        private static bool CPCheck(TreeNodeCollection cps, string name, out TreeNode cpnode)
        {
            foreach (TreeNode cp in cps)
            {
                if (cp.Text == name)
                {
                    cpnode = cp;
                    return true;
                }
            }
            cpnode = null;
            return false;
        }

        private static void BulkAddFBM(TreeNode ecbnode, TreeNode node, string fbmname, string redundantname)
        {
            TreeNode compound = null;
            string ecbprefix = string.Empty;
            string fbmtype = node.Nodes["Type"].Text.Replace(" ", "", StringComparison.InvariantCultureIgnoreCase);
            switch (fbmtype)
            {
                case "FBM201":
                    AddECB(fbmname, "ECB1", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    break;

                case "FBM202":
                    AddECB(fbmname, "ECB1", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    break;

                case "FBM203":
                    AddECB(fbmname, "ECB1", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    break;

                case "FBM204":
                    AddECB(fbmname, "ECB1", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 6)
                    {
                        fbmname = fbmname[..6];
                    }
                    for (int i = 5; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "AOUT", compound);
                    }
                    break;

                case "FBM206":
                case "FBM206B":
                    AddECB(fbmname, "ECB4", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    break;

                case "FBM207":
                case "FBM207B":
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCIN", compound);
                    break;

                case "FBM214":
                case "FBM214B":
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "RIN", compound);
                    }
                    break;

                case "FBM215":
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "ROUT", compound);
                    }
                    break;

                case "FBM216":
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "RIN", compound);
                    }
                    break;

                case "FBM217":
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCIN", compound);
                    break;

                case "FBM217R":
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddECB(redundantname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    if (fbmname.Length > 5)
                    {
                        fbmname = fbmname[..5];
                    }
                    for (int i = 1; i <= 32; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "CINR", compound);
                    }
                    break;

                case "FBM218":
                    AddECB(fbmname, "ECB202", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "ROUT", compound);
                    }
                    break;

                case "FBM220":
                case "FBM221":
                case "FBM223":
                case "FBM224":
                case "FBM228":
                case "FBM230":
                case "FBM232":
                    AddECB(fbmname, "ECB200", ecbnode);
                    break;

                case "FBM231":
                case "FBM233":
                    AddECB(fbmname, "ECB201", ecbnode);
                    break;

                case "FBM237":
                    AddECB(fbmname, "ECB53", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 6)
                    {
                        fbmname = fbmname[..6];
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "AOUT", compound);
                    }
                    break;

                case "FBM238":
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(string.Concat(fbmname, "_CIN"), "MCIN", compound);
                    AddBlock(string.Concat(fbmname, "_COUT"), "MCOUT", compound);
                    break;

                case "FBM239":
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    if (fbmname.Length > 8)
                    {
                        fbmname = fbmname[..8];
                    }
                    AddBlock(string.Concat(fbmname, "_CIN"), "MCIN", compound);
                    AddBlock(string.Concat(fbmname, "_COUT"), "MCOUT", compound);
                    break;

                case "FBM240":
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    if (fbmname.Length > 6)
                    {
                        fbmname = fbmname[..6];
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "COUT", compound);
                    }
                    break;

                case "FBM240R":
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCOUT", compound);
                    if (fbmname.Length > 6)
                    {
                        fbmname = fbmname[..6];
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "CINR", compound);
                    }
                    break;

                case "FBM241":
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCOUT", compound);
                    if (fbmname.Length > 5)
                    {
                        fbmname = fbmname[..5];
                    }
                    for (int i = 1; i <= 8; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "CIN", compound);
                    }
                    for (int i = 9; i <= 16; ++i)
                    {
                        AddBlock(string.Concat(fbmname, "_", i.ToString(CultureInfo.InvariantCulture)), "COUT", compound);
                    }
                    break;

                case "FBM242":
                    AddECB(fbmname, "ECB5", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MCOUT", compound);
                    break;

                case "FBM244":
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    for (int i = 1; i <= 4; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "RIN", compound);
                    }
                    for (int i = 5; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddBlock(name, "ROUT", compound);
                    }
                    break;

                case "FBM245":
                    AddECB(fbmname, "ECB202", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    AddBlock(fbmname, "MAIN", compound);
                    string redundantecb = string.Empty;
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    if (redundantname.Length > 10)
                    {
                        redundantecb = redundantname[..10];
                    }
                    else
                    {
                        redundantecb = redundantname;
                    }
                    for (int i = 1; i <= 4; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddECB(string.Concat(redundantecb, "_", i.ToString(CultureInfo.InvariantCulture)), "ECB201", ecbnode);
                        AddBlock(name, "RINR", compound);
                    }
                    for (int i = 5; i <= 8; ++i)
                    {
                        string name = string.Concat(ecbprefix, "_", i.ToString(CultureInfo.InvariantCulture));
                        AddECB(name, "ECB201", ecbnode);
                        AddECB(string.Concat(redundantecb, "_", i.ToString(CultureInfo.InvariantCulture)), "ECB201", ecbnode);
                        AddBlock(name, "ROUTR", compound);
                    }
                    break;

                case "FBM247":
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    BlockConfigurator configurator = new(fbmname)
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                    int j = 1;
                    configurator.ShowDialog();
                    if (configurator.Analog)
                    {
                        AddBlock(fbmname, "MAIN", compound);
                    }
                    else
                    {
                        AddBlock(fbmname, "MCIN", compound);
                    }
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    configurator.Blocks.ForEach(delegate (string block)
                    {
                        AddECB(string.Concat(ecbprefix, "_", j.ToString(CultureInfo.InvariantCulture)), "ECB201", ecbnode);
                        AddBlock(string.Concat(ecbprefix, "_", j.ToString(CultureInfo.InvariantCulture)), block, compound);
                        ++j;
                    });
                    configurator.Dispose();
                    break;

                case "FBM248":
                    AddECB(fbmname, "ECB200", ecbnode);
                    AddCompound(fbmname, node, out compound);
                    configurator = new BlockConfigurator(fbmname)
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                    j = 1;
                    configurator.ShowDialog();
                    if (configurator.Analog)
                    {
                        AddBlock(fbmname, "MAIN", compound);
                    }
                    else
                    {
                        AddBlock(fbmname, "MCIN", compound);
                    }
                    if (fbmname.Length > 10)
                    {
                        ecbprefix = fbmname[..10];
                    }
                    else
                    {
                        ecbprefix = fbmname;
                    }
                    configurator.Blocks.ForEach(delegate (string block)
                    {
                        AddECB(string.Concat(ecbprefix, "_", j.ToString(CultureInfo.InvariantCulture)), "ECB201", ecbnode);
                        AddBlock(string.Concat(ecbprefix, "_", j.ToString(CultureInfo.InvariantCulture)), block, compound);
                        ++j;
                    });
                    configurator.Dispose();
                    break;

                default:
                    break;
            }
        }

        private void BuildCPFile(List<TreeNode> fbms, string cp)
        {
            Cursor = Cursors.WaitCursor;
            BuildProgress.Maximum = fbms.Count * 2;
            BuildProgress.Step = 1;
            BuildStatus.Text = string.Concat("Building ", cp);
            BuildProgress.Visible = true;
            BuildStatus.Visible = true;
            string path = Path.Combine(SaveLocation.FullName, string.Concat(cp, ".i"));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FBM fbm;
            CP controller = new(cp)
            {
                Project = SaveLocation
            };
            foreach (TreeNode node in fbms)
            {
                string nodetype = node.Nodes["Type"].Text.Replace(" ", string.Empty, StringComparison.InvariantCultureIgnoreCase);
                switch (nodetype)
                {
                    case "FBM201":
                        fbm = new FBM201()
                        {
                            FBMName = node.Text,
                            FBMType = "201",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM202":
                        fbm = new FBM202()
                        {
                            FBMName = node.Text,
                            FBMType = "202",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM203":
                        fbm = new FBM203()
                        {
                            FBMName = node.Text,
                            FBMType = "203",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM204":
                        fbm = new FBM204()
                        {
                            FBMName = node.Text,
                            FBMType = "204",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM206":
                        fbm = new FBM206()
                        {
                            FBMName = node.Text,
                            FBMType = "206",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM206B":
                        fbm = new FBM206B()
                        {
                            FBMName = node.Text,
                            FBMType = "206",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM207":
                    case "FBM207B":
                        fbm = new FBM207()
                        {
                            FBMName = node.Text,
                            FBMType = "207",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM214":
                        fbm = new FBM214()
                        {
                            FBMName = node.Text,
                            FBMType = "214",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM214B":
                        fbm = new FBM214B()
                        {
                            FBMName = node.Text,
                            FBMType = "214",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM215":
                        fbm = new FBM215()
                        {
                            FBMName = node.Text,
                            FBMType = "215",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM216":
                        fbm = new FBM216()
                        {
                            FBMName = node.Text,
                            FBMType = "216",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM217":
                        fbm = new FBM217()
                        {
                            FBMName = node.Text,
                            FBMType = "217",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM217R":
                        fbm = new FBM217R(node.Nodes["Redundant"].Text)
                        {
                            FBMName = node.Text,
                            FBMType = "217",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM218":
                        fbm = new FBM218()
                        {
                            FBMName = node.Text,
                            FBMType = "218",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM220":
                        fbm = new FBM220()
                        {
                            FBMName = node.Text,
                            FBMType = "220",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM221":
                        fbm = new FBM221()
                        {
                            FBMName = node.Text,
                            FBMType = "221",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM223":
                        fbm = new FBM223()
                        {
                            FBMName = node.Text,
                            FBMType = "223",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM224":
                        fbm = new FBM224()
                        {
                            FBMName = node.Text,
                            FBMType = "224",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM228":
                        fbm = new FBM228()
                        {
                            FBMName = node.Text,
                            FBMType = "228",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM230":
                        fbm = new FBM230()
                        {
                            FBMName = node.Text,
                            FBMType = "230",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM231":
                        fbm = new FBM231()
                        {
                            FBMName = node.Text,
                            FBMType = "231",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM232":
                        fbm = new FBM232()
                        {
                            FBMName = node.Text,
                            FBMType = "232",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM233":
                        fbm = new FBM233()
                        {
                            FBMName = node.Text,
                            FBMType = "233",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM237":
                        fbm = new FBM237()
                        {
                            FBMName = node.Text,
                            FBMType = "237",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM238":
                        fbm = new FBM238()
                        {
                            FBMName = node.Text,
                            FBMType = "238",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM239":
                        fbm = new FBM239()
                        {
                            FBMName = node.Text,
                            FBMType = "239",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM240":
                        fbm = new FBM240()
                        {
                            FBMName = node.Text,
                            FBMType = "240",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM240R":
                        fbm = new FBM240R(node.Nodes["Redundant"].Text)
                        {
                            FBMName = node.Text,
                            FBMType = "240",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM241":
                        fbm = new FBM241()
                        {
                            FBMName = node.Text,
                            FBMType = "241",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM242":
                        fbm = new FBM242()
                        {
                            FBMName = node.Text,
                            FBMType = "242",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM244":
                        fbm = new FBM244()
                        {
                            FBMName = node.Text,
                            FBMType = "244",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM245":
                        fbm = new FBM245(node.Nodes["Redundant"].Text)
                        {
                            FBMName = node.Text,
                            FBMType = "245",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM247":
                        List<string> blocks = new();
                        foreach (TreeNode block in node.Nodes["Compound"].Nodes)
                        {
                            blocks.Add(block.Text);
                        }
                        fbm = new FBM247(blocks)
                        {
                            FBMName = node.Text,
                            FBMType = "247",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;

                    case "FBM248":
                        blocks = new List<string>();
                        foreach (TreeNode block in node.Nodes["Compound"].Nodes)
                        {
                            blocks.Add(block.Text);
                        }
                        fbm = new FBM248(blocks)
                        {
                            FBMName = node.Text,
                            FBMType = "248",
                            CP = cp,
                            Channel = node.Nodes["Channel"].Text,
                            Project = SaveLocation,
                            DAFile = controller.DAFile,
                            DA = controller.DA
                        };
                        fbm.Build();
                        fbm.Dispose();
                        break;
                }
            }
            fbm = new FBM()
            {
                CP = cp,
                Project = SaveLocation
            };
            fbm.Dispose();
            controller.Dispose();
            BuildProgress.PerformStep();
            BuildProgress.Value = 0;
            BuildStatus.Text = string.Concat(cp, " Built");
            Cursor = Cursors.Default;
        }

        private static bool FBMCheck(List<TreeNode> fbms)
        {
            List<bool> vs = new();
            foreach (TreeNode fbm in fbms)
            {
                if (fbm.Nodes.Count > 0)
                {
                    vs.Add(true);
                }
                else
                {
                    vs.Add(false);
                }
            }
            return vs.Contains(false);
        }

        private static void OpenExplorer(string path)
        {
            ProcessStartInfo info = new()
            {
                FileName = path
            };
            Process.Start(info);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                CPFile = new XmlDocument
                {
                    XmlResolver = null
                };
                XmlElement root = CPFile.DocumentElement;
                XmlElement CPs = CPFile.CreateElement("CPs");
                CPFile.AppendChild(CPs);
                foreach (TreeNode cp in DataBase.Nodes["DataBase"].Nodes)
                {
                    CPs.AppendChild(CPNode(cp.Nodes, cp.Text));
                }
                CPFile.Save(Path.Combine(Project.FullName, "Project", string.Concat(Project.Name, ".xml")));
            }
            catch (Exception ex)
            {
                Handler.LogError(ex.Message);
            }
            Cursor = Cursors.Default;
        }

        private void Save()
        {
            Cursor = Cursors.WaitCursor;
            CPFile = new XmlDocument
            {
                XmlResolver = null
            };
            XmlElement root = CPFile.DocumentElement;
            XmlElement CPs = CPFile.CreateElement("CPs");
            CPFile.AppendChild(CPs);
            foreach (TreeNode cp in DataBase.Nodes["DataBase"].Nodes)
            {
                CPs.AppendChild(CPNode(cp.Nodes, cp.Text));
            }
            CPFile.Save(Path.Combine(Project.FullName, "Project", string.Concat(Project.Name, ".xml")));
        }

        private XmlNode CPNode(TreeNodeCollection nodes, string name)
        {
            XmlElement cp = CPFile.CreateElement("CP");
            XmlElement FBMs = CPFile.CreateElement("FBMs");
            XmlElement ECBs = CPFile.CreateElement("ECBs");
            cp.SetAttribute("Name", name);
            foreach (TreeNode node in nodes)
            {
                if (node.Name == "ECB")
                {
                    foreach (TreeNode ecbnode in node.Nodes)
                    {
                        ECBs.AppendChild(ECBNode(ecbnode));
                    }
                }
                else
                {
                    FBMs.AppendChild(FBMNode(node));
                }
            }
            cp.AppendChild(ECBs);
            cp.AppendChild(FBMs);
            return cp;
        }

        private XmlNode ECBNode(TreeNode node)
        {
            XmlElement ecb = CPFile.CreateElement("ECB");
            ecb.SetAttribute("Name", node.Text);
            ecb.SetAttribute("Type", node.Nodes["Type"].Text);
            return ecb;
        }

        private XmlNode FBMNode(TreeNode node)
        {
            XmlElement fbm = CPFile.CreateElement("FBM");
            fbm.SetAttribute("Name", node.Text);
            fbm.SetAttribute("Type", node.Nodes["Type"].Text);
            fbm.SetAttribute("Channel", node.Nodes["Channel"].Text);
            if (node.Nodes["Redundant"] != null)
            {
                fbm.SetAttribute("Redundant", node.Nodes["Redundant"].Text);
            }
            if (node.Nodes["Compound"] != null)
            {
                fbm.AppendChild(CompoundNode(node.Nodes["Compound"]));
            }
            return fbm;
        }

        private XmlNode CompoundNode(TreeNode node)
        {
            XmlElement compound = CPFile.CreateElement("Compound");
            compound.SetAttribute("Name", node.Text);
            foreach (TreeNode block in node.Nodes)
            {
                compound.AppendChild(BlockNode(block));
            }
            return compound;
        }

        private XmlNode BlockNode(TreeNode node)
        {
            XmlElement block = CPFile.CreateElement("Block");
            block.SetAttribute("Type", node.Text);
            block.SetAttribute("Name", node.Nodes["Name"].Text);
            return block;
        }

        private void VersionRequest_Click(object sender, EventArgs e)
        {
            string version = string.Format(CultureInfo.CurrentCulture, "Verison {0}", Environment.Version.ToString(4));
            SendEmail(Resources.Version_Request_Subject, string.Concat(Resources.Version_Request_Body, version, "."));
        }

        private void FeatureRequest_Click(object sender, EventArgs e)
        {
            Email email = new("Request");
            email.ShowDialog();
            SendEmail(Resources.Feature_Request_Subject, email.EmailBody);
            email.Dispose();
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            Email email = new("Bug");
            email.ShowDialog();
            SendEmail(Resources.Report_Bug_Subject, email.EmailBody);
            email.Dispose();
        }

        public static void SendEmail(string subject, string message, string attachmentpath = "")
        {
            Outlook.Application application = new();
            Outlook.MailItem mail = application.CreateItem(Outlook.OlItemType.olMailItem);
            mail.Subject = subject;
            mail.To = Resources.Email;
            mail.Body = message;
            Attachment attachment = new(attachmentpath);
            mail.Attachments.Add(attachment);
            mail.Send();
            attachment.Dispose();
            MessageBox.Show(Resources.Email_Sent);
        }

        private void InstructionMenuItem_Click(object sender, EventArgs e)
        {
            string locationToSavePdf = Path.Combine(Path.GetTempPath(), "Instructions for Using IO Checkout Tool.pdf");
            File.WriteAllBytes(locationToSavePdf, Resources.Instructions_for_Using_IO_Checkout_Tool);
            Process.Start(locationToSavePdf);
        }

        private void IOCheckout_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                if (DataBase.Nodes.Count > 0)
                {
                    CPFile = new XmlDocument
                    {
                        XmlResolver = null
                    };
                    XmlElement root = CPFile.DocumentElement;
                    XmlElement CPs = CPFile.CreateElement("CPs");
                    CPFile.AppendChild(CPs);
                    foreach (TreeNode cp in DataBase.Nodes["DataBase"].Nodes)
                    {
                        CPs.AppendChild(CPNode(cp.Nodes, cp.Text));
                    }
                    CPFile.Save(Path.Combine(Project.FullName, string.Concat(Project.Name, ".xml")));
                }
            }
            catch (Exception ex)
            {
                Handler.LogError(ex.Message);
            }
            Cursor = Cursors.Default;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace IOCheckoutTool
{
    public partial class BlockConfigurator : Form
    {
        #region Fields
        private string BIN;

        private int BlockCount;

        private string BOUT;

        private string RIN;

        private string ROUT;
        #endregion Fields

        #region Properties
        public bool Analog { get; set; }

        public List<string> Blocks { get; } = new List<string>();
        #endregion Properties

        #region Public Constructors

        public BlockConfigurator(string fbmname)
        {
            InitializeComponent();
            FBM.Nodes.Add("FBM", fbmname);
            BlockCount = 0;
            Text += fbmname;
        }

        #endregion Public Constructors

        #region Private Methods

        private static void RemoveNodes(TreeNode node)
        {
            int count = node.Nodes.Count;
            for (int i = 1; i <= count; ++i)
            {
                node.Nodes[string.Concat("Block", i.ToString(CultureInfo.InvariantCulture))].Remove();
            }
        }

        private void AllBINs_Click(object sender, EventArgs e)
        {
            RemoveNodes(FBM.Nodes["FBM"]);
            BlockOptions.Enabled = false;
            for (int i = 1; i <= 8; ++i)
            {
                FBM.Nodes["FBM"].Nodes.Add(string.Concat("Block", i.ToString(CultureInfo.InvariantCulture)), BIN);
            }
            FBM.Nodes["FBM"].Expand();
        }

        private void AllBOUTs_Click(object sender, EventArgs e)
        {
            RemoveNodes(FBM.Nodes["FBM"]);
            BlockOptions.Enabled = false;
            for (int i = 1; i <= 8; ++i)
            {
                FBM.Nodes["FBM"].Nodes.Add(string.Concat("Block", i.ToString(CultureInfo.InvariantCulture)), BOUT);
            }
            FBM.Nodes["FBM"].Expand();
        }

        private void AllRINs_Click(object sender, EventArgs e)
        {
            RemoveNodes(FBM.Nodes["FBM"]);
            BlockOptions.Enabled = false;
            for (int i = 1; i <= 8; ++i)
            {
                FBM.Nodes["FBM"].Nodes.Add(string.Concat("Block", i.ToString(CultureInfo.InvariantCulture)), RIN);
            }
            FBM.Nodes["FBM"].Expand();
        }

        private void AllROUTs_Click(object sender, EventArgs e)
        {
            RemoveNodes(FBM.Nodes["FBM"]);
            BlockOptions.Enabled = false;
            for (int i = 1; i <= 8; ++i)
            {
                FBM.Nodes["FBM"].Nodes.Add(string.Concat("Block", i.ToString(CultureInfo.InvariantCulture)), ROUT);
            }
            FBM.Nodes["FBM"].Expand();
        }

        private void BlockConfigurator_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void BlockConfigurator_Load(object sender, EventArgs e)
        {
            RIN = "RIN";
            ROUT = "ROUT";
            BIN = "BIN";
            BOUT = "BOUT";
        }

        private void BlockOptions_MouseDown(object sender, MouseEventArgs e)
        {
            string block = BlockOptions.SelectedItem.ToString();
            BlockOptions.DoDragDrop(block, DragDropEffects.Copy);
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in FBM.Nodes["FBM"].Nodes)
            {
                Blocks.Add(node.Text);
            }
            if (Blocks.Contains("RIN") || Blocks.Contains("ROUT"))
            {
                Analog = true;
            }
            else
            {
                Analog = false;
            }
            Close();
        }

        private void FBM_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode dropnode = FBM.GetNodeAt(FBM.PointToClient(new Point(e.X, e.Y)));
            string block = (string)e.Data.GetData(typeof(string));
            dropnode.Nodes.Add(string.Concat("Block", BlockCount.ToString(CultureInfo.InvariantCulture)), block);
            ++BlockCount;
            dropnode.Expand();
        }

        private void FBM_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void HalfAnalog_Click(object sender, EventArgs e)
        {
            RemoveNodes(FBM.Nodes["FBM"]);
            BlockOptions.Enabled = false;
            for (int i = 1; i <= 4; ++i)
            {
                FBM.Nodes["FBM"].Nodes.Add(string.Concat("Block", i.ToString(CultureInfo.InvariantCulture)), RIN);
            }
            for (int i = 5; i <= 8; ++i)
            {
                FBM.Nodes["FBM"].Nodes.Add(string.Concat("Block", i.ToString(CultureInfo.InvariantCulture)), ROUT);
            }
            FBM.Nodes["FBM"].Expand();
        }

        private void HalfDigital_Click(object sender, EventArgs e)
        {
            RemoveNodes(FBM.Nodes["FBM"]);
            BlockOptions.Enabled = false;
            for (int i = 1; i <= 4; ++i)
            {
                FBM.Nodes["FBM"].Nodes.Add(string.Concat("Block", i.ToString(CultureInfo.InvariantCulture)), BIN);
            }
            for (int i = 5; i <= 8; ++i)
            {
                FBM.Nodes["FBM"].Nodes.Add(string.Concat("Block", i.ToString(CultureInfo.InvariantCulture)), BOUT);
            }
            FBM.Nodes["FBM"].Expand();
        }

        private void Manual_Click(object sender, EventArgs e)
        {
            RemoveNodes(FBM.Nodes["FBM"]);
            BlockOptions.Enabled = true;
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (FBM.SelectedNode.Name != "FBM")
            {
                FBM.SelectedNode.Remove();
            }
        }

        #endregion Private Methods
    }
}
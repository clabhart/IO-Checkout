using IOCheckoutTool.Properties;
using System;
using System.Windows.Forms;

namespace IOCheckoutTool
{
    public partial class Configure : Form
    {
        public string FBMName { get; set; }
        public string Channel { get; set; }
        public string RedundantName { get; set; }

        public Configure(bool redundant = false)
        {
            InitializeComponent();
            RedundantLabel.Visible = redundant;
            RedundantNameBox.Visible = redundant;
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(NameBox.Text) && ChannelBox.SelectedIndex != -1)
            {
                Close();
            }
            else
            {
                MessageBox.Show(Resources.EmptyForm);
                return;
            }
            FBMName = NameBox.Text;
            Channel = ChannelBox.SelectedItem.ToString();
            RedundantName = RedundantNameBox.Text;
        }
    }
}
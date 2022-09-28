using System;
using System.Windows.Forms;

using IOCheckoutTool.Properties;

namespace IOCheckoutTool
{
    public partial class Configure : Form
    {
        #region Properties
        public string Channel { get; set; }
        public string FBMName { get; set; }
        public string RedundantName { get; set; }
        #endregion Properties

        #region Public Constructors

        public Configure(bool redundant = false)
        {
            InitializeComponent();
            RedundantLabel.Visible = redundant;
            RedundantNameBox.Visible = redundant;
        }

        #endregion Public Constructors

        #region Private Methods

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

        #endregion Private Methods
    }
}
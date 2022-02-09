using IOCheckoutTool.Properties;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace IOCheckoutTool
{
    public partial class Email : Form
    {
        public Email(string issue)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(issue))
            {
                Text = issue;
                Description.Text += issue.ToLower(CultureInfo.CurrentCulture);
            }
        }

        public string EmailBody { get; set; }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Body.Text))
            {
                MessageBox.Show(Resources.EmptyForm);
                return;
            }
            EmailBody = Body.Text;
            Close();
        }
    }
}
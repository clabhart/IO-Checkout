using System;
using System.Globalization;
using System.Windows.Forms;

using IOCheckoutTool.Properties;

namespace IOCheckoutTool
{
    public partial class Email : Form
    {
        #region Properties
        public string EmailBody { get; set; }
        #endregion Properties

        #region Public Constructors

        public Email(string issue)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(issue))
            {
                Text = issue;
                Description.Text += issue.ToLower(CultureInfo.CurrentCulture);
            }
        }

        #endregion Public Constructors

        #region Private Methods

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

        #endregion Private Methods
    }
}
using System;
using System.Windows.Forms;

namespace IOCheckoutTool
{
    public partial class InputBox : Form
    {
        #region Properties
        public string Output { get; set; }
        #endregion Properties

        #region Public Constructors

        public InputBox(string label)
        {
            InitializeComponent();
            TextLabel.Text = label;
            Input.Focus();
        }

        #endregion Public Constructors

        #region Private Methods

        private void OKButton_Click(object sender, EventArgs e)
        {
            Output = Input.Text;
            Close();
        }

        #endregion Private Methods
    }
}
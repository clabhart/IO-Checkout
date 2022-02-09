using System;
using System.Windows.Forms;

namespace IOCheckoutTool
{
    public partial class InputBox : Form
    {
        public InputBox(string label)
        {
            InitializeComponent();
            TextLabel.Text = label;
            Input.Focus();
        }

        public string Output { get; set; }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Output = Input.Text;
            Close();
        }
    }
}
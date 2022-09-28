using System;
using System.IO;
using System.Windows.Forms;

using IOCheckoutTool.Properties;

namespace IOCheckoutTool
{
    public partial class NewProject : Form
    {
        public NewProject(DirectoryInfo projects)
        {
            InitializeComponent();
            Projects = projects;
        }

        private DirectoryInfo Projects { get; set; }

        private void BuildButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CPs.Text))
            {
                _ = MessageBox.Show(Resources.EmptyForm);
                return;
            }
            DirectoryInfo project = Directory.CreateDirectory(Path.Combine(Projects.FullName, ProjectName.Text, "Project"));
            using (StreamWriter writer = new(Path.Combine(project.FullName, "CPs.txt")))
            {
                foreach (string cp in CPs.Text.Split(Environment.NewLine.ToCharArray()))
                {
                    if (!string.IsNullOrEmpty(cp))
                    {
                        writer.WriteLine(cp);
                    }
                }
                writer.Close();
                writer.Dispose();
            }

            Close();
        }

        public DirectoryInfo LoadedProject()
        {
            DirectoryInfo temp = new(Path.Combine(Projects.FullName, ProjectName.Text));
            return temp;
        }
    }
}
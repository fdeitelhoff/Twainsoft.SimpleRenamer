using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using EnvDTE;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Twainsoft.SolutionRenamer.VSPackage.GUI
{
    public partial class RenameProjectDialog
    {
        private Project CurrentProject { get; set; }

        public RenameProjectDialog(Project project)
        {
            InitializeComponent();

            CurrentProject = project;

            ProjectName.Text = project.Name;
            ProjectName.Focus();
            ProjectName.SelectAll();
        }

        public string GetProjectName()
        {
            return ProjectName.Text.Trim();
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            var directory = new FileInfo(CurrentProject.FullName).Directory;

            if (directory == null)
            {
                throw new InvalidOperationException();
            }

            var parentDirectory = directory.Parent;

            if (parentDirectory == null)
            {
                throw new InvalidOperationException();
            }

            // Projects with the same name cannot be in the same folder due to the same folder names.
            // Within the same solution it is no problem! 
            if (Directory.Exists(Path.Combine(parentDirectory.FullName, GetProjectName())))
            {
                MessageBox.Show(
                    string.Format(
                        "The project '{0}' already exists in the solution respectively the file system. Please choose another project name.",
                        GetProjectName()),
                    "Project already exists", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (CurrentProject.Name != GetProjectName())
            {
                DialogResult = true;

                Close();
            }
            else
            {
                Close();
            }
        }
    }
}

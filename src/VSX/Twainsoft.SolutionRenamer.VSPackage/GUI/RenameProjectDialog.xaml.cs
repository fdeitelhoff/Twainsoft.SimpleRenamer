using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Twainsoft.SolutionRenamer.VSPackage.VSX;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Twainsoft.SolutionRenamer.VSPackage.GUI
{
    public partial class RenameProjectDialog
    {
        private RenameData RenameData { get; set; }
        private Project CurrentProject { get; set; }

        public RenameProjectDialog(RenameData renameData, Project project)
        {
            InitializeComponent();

            RenameData = renameData;
            CurrentProject = project;

            ProjectName.Text = project.Name;
            ProjectName.Focus();
            ProjectName.SelectAll();
        }

        public string GetProjectName()
        {
            return Path.GetFileNameWithoutExtension(ProjectName.Text.Trim());
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            var solutionDirectory = new FileInfo(RenameData.Dte.Solution.FileName).Directory;
            var directory = new FileInfo(CurrentProject.FullName).Directory;

            if (directory == null)
            {
                throw new InvalidOperationException();
            }

            var projectDirectory = directory.ToString().Replace(solutionDirectory + @"\", "");
            var projectFileExtension = Path.GetExtension(CurrentProject.FileName);

            var projectFileName = GetProjectName() + projectFileExtension;
            var uniqueName = Path.Combine(projectDirectory, projectFileName);

            IVsHierarchy currentProjectHierarchy;
            RenameData.Solution.GetProjectOfUniqueName(uniqueName, out currentProjectHierarchy);

            // Projects with the same name cannot be in the same folder due to the same folder names.
            // Within the same solution it is no problem! 
            if (currentProjectHierarchy != null) //Directory.Exists(Path.Combine(parentDirectory.FullName, GetProjectName())))
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

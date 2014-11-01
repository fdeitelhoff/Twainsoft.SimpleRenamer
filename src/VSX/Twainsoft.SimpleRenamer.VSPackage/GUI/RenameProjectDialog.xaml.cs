using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Twainsoft.SolutionRenamer.VSPackage.VSX;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Twainsoft.SolutionRenamer.VSPackage.GUI
{
    public partial class RenameProjectDialog
    {
        private RenameData RenameData { get; set; }
        private Project CurrentProject { get; set; }

        private Dictionary<object, List<string>> ParentToProjects { get; set; }

        public RenameProjectDialog(RenameData renameData, Project project)
        {
            InitializeComponent();

            RenameData = renameData;
            CurrentProject = project;

            ParentToProjects = new Dictionary<object, List<string>>();

            // Set the current project name within the GUI.
            ProjectName.Text = project.Name;
            ProjectName.Focus();
            ProjectName.SelectAll();
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            var newProjectName = GetProjectName();

            string uniqueName;

            var solutionDirectory = new FileInfo(RenameData.Dte.Solution.FileName).Directory;
            var directory = new FileInfo(CurrentProject.FullName).Directory;

            if (directory == null)
            {
                throw new InvalidOperationException();
            }
            
            if (Path.GetFileNameWithoutExtension(CurrentProject.FileName) != directory.Name)
            {
                var projectDirectory = directory.ToString().Replace(solutionDirectory + @"\", "");
                var projectFileExtension = Path.GetExtension(CurrentProject.FileName);

                var projectFileName = newProjectName + projectFileExtension;
                uniqueName = Path.Combine(projectDirectory, projectFileName);
            }
            else
            {
                var projectFileExtension = Path.GetExtension(CurrentProject.FileName);

                var projectFileName = newProjectName + projectFileExtension;
                uniqueName = Path.Combine(newProjectName, projectFileName);
            }

            // Check if the project already exists.
            IVsHierarchy currentProjectHierarchy;
            RenameData.Solution.GetProjectOfUniqueName(uniqueName, out currentProjectHierarchy);

            // Check if there's a project on the same level (solution folder) with the same name.
            var projectExists = CheckProjects(newProjectName);

            // Are there projects with the same name on the same hierarchy level or within the same folder name?
            // => That's not allowed!
            if (projectExists || currentProjectHierarchy != null)
            {
                // ToDo: Change this to the WPF counterpart!
                MessageBox.Show(
                    string.Format(
                        "The project '{0}' already exists in the solution respectively the file system. Please choose a different project name.",
                        newProjectName),
                    string.Format("Project '{0}' already exists", newProjectName), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (CurrentProject.Name != newProjectName)
            {
                DialogResult = true;

                Close();
            }
            else
            {
                Close();
            }
        }

        public string GetProjectName()
        {
            return ProjectName.Text.Trim().Replace(".csproj", "");
        }

        private bool CheckProjects(string newProjectName)
        {
            ParentToProjects.Clear();

            foreach (Project proj in RenameData.Dte.Solution.Projects)
            {
                NavigateProject(proj);
            }

            var parent = GetSolutionFolder(CurrentProject) ?? (object)"no parent";

            return ParentToProjects[parent].Contains(newProjectName);
        }

        private void NavigateProject(Project project)
        {
            var parent = GetSolutionFolder(project) ?? (object) "no parent";

            if (!ParentToProjects.ContainsKey(parent))
            {
                ParentToProjects.Add(parent, new List<string> {project.Name});
            }
            else
            {
                ParentToProjects[parent].Add(project.Name);
            }

            // We need to navigate all found project items. There could be a solution folder with projects within.
            foreach (ProjectItem projectItem in project.ProjectItems)
            {
                if (projectItem.SubProject != null)
                {
                    NavigateProject(projectItem.SubProject);
                }
            }
        }

        private SolutionFolder GetSolutionFolder(Project project)
        {
            if (project.ParentProjectItem == null)
            {
                return null;
            }

            var parentProject = project.ParentProjectItem.Collection.Parent as Project;

            if (parentProject == null)
            {
                throw new InvalidOperationException("The Parent Project Of The Current Selected Project cannot be determined!");
            }

            return parentProject.Object as SolutionFolder;
        }
    }
}

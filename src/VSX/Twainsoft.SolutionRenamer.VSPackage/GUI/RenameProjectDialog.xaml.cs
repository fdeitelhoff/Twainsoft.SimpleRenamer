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
            CheckProjectsForReferences();

            var uniqueName = "";

            var solutionDirectory = new FileInfo(RenameData.Dte.Solution.FileName).Directory;
            var directory = new FileInfo(CurrentProject.FullName).Directory;

            if (directory == null)
            {
                throw new InvalidOperationException();
            }

            // Check if the project already exists.
            if (Path.GetFileNameWithoutExtension(CurrentProject.FileName) != directory.Name)
            {
                var projectDirectory = directory.ToString().Replace(solutionDirectory + @"\", "");
                var projectFileExtension = Path.GetExtension(CurrentProject.FileName);

                var projectFileName = GetProjectName() + projectFileExtension;
                uniqueName = Path.Combine(projectDirectory, projectFileName);
            }
            else
            {
                var projectFileExtension = Path.GetExtension(CurrentProject.FileName);

                var projectFileName = GetProjectName() + projectFileExtension;
                uniqueName = Path.Combine(GetProjectName(), projectFileName);
            }

            IVsHierarchy currentProjectHierarchy;
            RenameData.Solution.GetProjectOfUniqueName(uniqueName, out currentProjectHierarchy);

            // Projects with the same name cannot be in the same folder due to the same folder names.
            // Within the same solution it is no problem! 
            if (currentProjectHierarchy != null) //Directory.Exists(Path.Combine(parentDirectory.FullName, GetProjectName())))
            {
                MessageBox.Show(
                    string.Format(
                        "The project '{0}' already exists in the solution respectively the file system. Please choose a different project name.",
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

        

        private void CheckProjectsForReferences()
        {
            //StatusBarHelper.Update("Checking other projects for references to the renamed one...");

            foreach (Project proj in RenameData.Dte.Solution.Projects)
            {
                NavigateProject(proj);
            }
        }

        private void NavigateProject(Project project)
        {
            System.Diagnostics.Debug.WriteLine("Name {0} - UniqueName {1} - Parent {2}", project.Name, project.UniqueName, project.ParentProjectItem);

            if (project.Name != RenameData.NewProjectName)
            {
                //// The GUID points to a C# project. All other project types are excluded here.
                //if (project.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")
                //{
                    
                //}



                // We need to navigate all found project items. There could be a solution folder with projects within.
                foreach (ProjectItem projectItem in project.ProjectItems)
                {
                    if (projectItem.SubProject != null)
                    {
                        NavigateProject(projectItem.SubProject);
                    }
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

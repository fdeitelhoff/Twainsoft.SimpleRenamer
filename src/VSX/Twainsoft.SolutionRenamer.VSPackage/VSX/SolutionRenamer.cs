using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Twainsoft.SolutionRenamer.VSPackage.GUI;
using VSLangProj110;
using VSLangProj80;

namespace Twainsoft.SolutionRenamer.VSPackage.VSX
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.SolutionRenamerVsPackagePkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class SolutionRenamer : Package
    {
        public IVsSolution Solution { get; set; }
        public DTE2 Dte { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null)
            {
                return;
            }

            var contextMenuCommandId = new CommandID(GuidList.SolutionRenamerVsPackageCmdSet, (int)PkgCmdIdList.ContextMenuCommandId);
            var contextMenu = new MenuCommand(OnRenameProject, contextMenuCommandId);
            mcs.AddCommand(contextMenu);

            GetGlobalServices();

            ProjectsWithReferences = new List<Project>();
        }

        private void OnRenameProject(object sender, EventArgs e)
        {
            try
            {
                // Get the currently selected project within the solution explorer.
                var currentProject = GetSelectedProject();

                // Get the new project name from the user.
                var rename = new RenameProjectDialog(currentProject.Name);
                var result = rename.ShowDialog();
                if (!result.HasValue || !result.Value)
                {
                    return;
                }

                // TODO: Check if there's another project with this name! (Where to check??)
                // This is the new project name the user typed in.
                var newProjectName = rename.GetProjectName();

                // Check if this is necessary when the references check was refactored!
                ProjectsWithReferences.Clear();
                
                // Save all changes that were made before the renaming process. Just for safety!
                SaveSolutionFile();

                // Check if there's an solution folder or return null.
                var solutionFolder = GetSolutionFolder(currentProject);

                // Get the file name and the parent directory of the current project before it gets renamed!
                var projectFileName = currentProject.Name;
                var projectParentDirectory = GetProjectParentDirectory(currentProject);

                // Check if the current project is the startup project before it gets renamed and temporarily deleted.
                var isStartupProject = IsStartupProject(currentProject);

                var oldProjectName = currentProject.Name;
                // This is a little bit scary: I need the old project path, before it gets moved. But this instance will have the new name after it gets renamed.
                // Change this behavior!
                OldProject = currentProject;
                OldProjectName = oldProjectName;

                currentProject.Name = newProjectName;

                // The hierarchy is needed for some of the following actions.
                IVsHierarchy currentProjectHierarchy;
                Solution.GetProjectOfUniqueName(currentProject.UniqueName, out currentProjectHierarchy);

                if (projectFileName == projectParentDirectory)
                {
                    // Check if other projects have references to the currently selected project. These references must be changed too!
                    CheckProjectsForReferences(Dte, currentProject);

                    // We need some data for future actions. Collect them here because the project is ready to get removed from the solution!
                    var newProjectFileName = Path.GetFileName(currentProject.FileName);
                    var fullProjectName = currentProject.FullName;
                    var newProjectDirectory = currentProject.Name;

                    // Remove the project from the solution file!
                    RemoveProjectFromSolution(currentProjectHierarchy);

                    // Move the project folder on the file system within the solution folder!
                    MoveProjectFolder(fullProjectName, newProjectDirectory);

                    // Add the renamed project to the solution. Either directly or within a solution folder.
                    // The return project is the new current project we're using for all other steps.
                    currentProject = AddProjectToSolution(solutionFolder, newProjectFileName, fullProjectName, newProjectDirectory);

                    // Save the solution file after we moved the project.
                    SaveSolutionFile();
                }

                // Now we want to add lost project references due to the name change.
                foreach (var proj in ProjectsWithReferences)
                {
                    var project = proj.Object as VSProject2;

                    var references = project.References as References2;

                    references.AddProject(currentProject);


                }

                // Changing the default namespace and the assembly name.
                var defaultNamespace = currentProject.Properties.Item("DefaultNamespace") as Property;
                var assemblyName = currentProject.Properties.Item("AssemblyName") as Property;

                if (defaultNamespace.Value.ToString().Contains(oldProjectName))
                {
                    defaultNamespace.Value = defaultNamespace.Value.ToString()
                        .Replace(oldProjectName, newProjectName);
                }

                if (assemblyName.Value.ToString().Contains(oldProjectName))
                {
                    assemblyName.Value = assemblyName.Value.ToString().Replace(oldProjectName, newProjectName);
                }

                // Maybe this will work? Should I use this flag whenever I want to save the project?
                // Is there another similar flag for solutions? If it exists should I use it to check if I should save the solution?
                if (currentProject.IsDirty)
                {
                    Solution.SaveSolutionElement((uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, currentProjectHierarchy,
                        0);
                }

                // Change some data in the AssemblyInfo.cs file if those data matches the old project name! (AssemblyTitle and AssemblyProduct)
                ChangeAssemblyData(currentProject, oldProjectName, newProjectName, currentProjectHierarchy);

                // If the renamed project was the startup project, we need to refresh this setting after it was deleted.
                if (isStartupProject)
                {
                    Dte.Solution.Properties.Item("StartupProject").Value = currentProject.Name;
                }

                // Rebuild the complete solution.
                Dte.Solution.SolutionBuild.Build();
                // Better this way?
                //dte.Solution.SolutionBuild.BuildProject();
            }
            catch (COMException comException)
            {
                Debug.WriteLine(comException);
            }
            // Just as a fail safe scenario! Should be remove in future versions.
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private void ChangeAssemblyData(Project currentProject, string oldProjectName, string newProjectName, IVsHierarchy currentProjectHierarchy)
        {
            var properties = currentProject.ProjectItems.Item("Properties");
            var assemblyInfo = properties.ProjectItems.Item("AssemblyInfo.cs");

            var assemblyTitle = assemblyInfo.FileCodeModel.CodeElements.Item("AssemblyTitle") as CodeAttribute2;
            var assemblyProduct = assemblyInfo.FileCodeModel.CodeElements.Item("AssemblyProduct") as CodeAttribute2;

            if (assemblyTitle == null || assemblyProduct == null)
            {
                throw new InvalidOperationException("AssemblyTitle Or AssemblyProduct Attribute Is Null!");
            }

            if (assemblyTitle.Value.Contains(oldProjectName))
            {
                assemblyTitle.Value = assemblyTitle.Value.Replace(oldProjectName, newProjectName);
            }

            if (assemblyProduct.Value.Contains(oldProjectName))
            {
                assemblyProduct.Value = assemblyProduct.Value.Replace(oldProjectName, newProjectName);
            }

            if (assemblyInfo.IsDirty)
            {
                Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, currentProjectHierarchy, 0);
            }
        }

        private void GetGlobalServices()
        {
            Solution = GetGlobalService(typeof(IVsSolution)) as IVsSolution;
            Dte = GetGlobalService(typeof(SDTE)) as DTE2;

            if (Solution == null || Dte == null)
            {
                throw new InvalidOperationException("The Solution Or The Dte Object is null!");
            }
        }

        private void SaveSolutionFile()
        {
            if (Dte.Solution.IsDirty)
            {
                Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);
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
        
        private string GetProjectParentDirectory(Project project)
        {
            var parent = Directory.GetParent(project.FullName);

            if (parent == null)
            {
                throw new InvalidOperationException("The Project Parent Directory Is Null!");
            }

            return parent.Name;
        }

        private bool IsStartupProject(Project project)
        {
            return Dte.Solution.Properties.Item("StartupProject").Value.ToString() == project.Name;
        }

        private Project GetSelectedProject()
        {
            IntPtr hierarchyPointer, selectionContainerPointer;
            object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            var monitorSelection =
                (IVsMonitorSelection)GetGlobalService(
                    typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer,
                out projectItemId,
                out multiItemSelect,
                out selectionContainerPointer);

            var selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                hierarchyPointer, typeof (IVsHierarchy)) as IVsHierarchy;

            if (selectedHierarchy != null)
            {
                ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                    projectItemId,
                    (int)__VSHPROPID.VSHPROPID_ExtObject,
                    out selectedObject));
            }

            return selectedObject as Project;
        }

        // There are events for references added, removed and changed. Maybe this is useful in the future?
        private void CheckProjectsForReferences(DTE2 dte, Project newProject)
        {
            NewProjectName = newProject.Name;

            foreach (Project proj in dte.Solution.Projects)
            {
                // Better way of checking the GUID. Maybe invert the equals check? Just now, we want to exclude solution folders.
                // This is not recusive. We need all projects of any depth. So we need to search projects recursively within solution folders.
                //if (proj.Name != newProject.Name && proj.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")
                //{
                    NavigateProject(proj);
                    
                    //var project = proj.Object as VSProject2;
                    
                    //var references = project.References as References2;

                    //foreach (Reference5 reference in references)
                    //{
                    //    Debug.WriteLine(reference.Name + " " + reference.Path);
                    //}
                //}
            }
        }

        // Maybe this is doable in another way? This private field is used for data just for the navigate project code. Looks a little bit ugly.
        private string NewProjectName;
        private Project OldProject;
        private string OldProjectName;
        private List<Project> ProjectsWithReferences;

        private void NavigateProject(Project project)
        {
            if (project.Name != NewProjectName)
            {
                if (project.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")
                {
                    //Debug.WriteLine("Projekt: " + project.Name);

                    CheckProjectReferences(project);
                }

                // We need to navigate all found project items. There could be a solution folder with some projects within.
                NavigateProjectItems(project.ProjectItems);
            }
        }

        private void NavigateProjectItems(ProjectItems projectItems)
        {
            if (projectItems != null)
            {
                foreach (ProjectItem projectItem in projectItems)
                {
                    if (projectItem.SubProject != null)
                    {
                        NavigateProject(projectItem.SubProject);
                    }
                }
            }
        }

        private void CheckProjectReferences(Project proj)
        {
            var project = proj.Object as VSProject2;

            var references = project.References as References2;

            foreach (Reference5 reference in references)
            {
                // OldProjectName seems to be the new one!
                // The references path is the dll in the debug folder. That cannot be compared easily.
                // Maybe SourceProject.FullName is better? Try it out in the next episode...
                if (reference.SourceProject != null && reference.Name == NewProjectName && reference.SourceProject.FullName == OldProject.FullName)
                {
                    //Debug.WriteLine(reference.Name + " " + reference.Path);

                    ProjectsWithReferences.Add(proj);
                }
            }
        }

        private void RemoveProjectFromSolution(IVsHierarchy projectHierarchy)
        {
            Solution.CloseSolutionElement(
                        (uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave |
                        (uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_DeleteProject, projectHierarchy, 0);
        }
        
        private void MoveProjectFolder(string fullProjectName, string newProjectDirectory)
        {
            var parentProjectDirectory = new DirectoryInfo(fullProjectName).Parent;

            if (parentProjectDirectory == null)
            {
                throw new InvalidOperationException("The Parent Project Directory Is Null!");
            }

            // Yes, my naming is... perfect?
            var parentProjectParentDirectory = parentProjectDirectory.Parent;

            if (parentProjectParentDirectory == null)
            {
                throw new InvalidOperationException("The Parent Project Parent Directory Is Null!");
            }

            // Move the current project folder to a folder with the new project name.
            parentProjectDirectory.MoveTo(Path.Combine(parentProjectParentDirectory.FullName, newProjectDirectory));
        }

        private Project AddProjectToSolution(SolutionFolder solutionFolder, string newProjectFileName, string fullProjectName, string newProjectDirectory)
        {
            var parentProjectDirectory = new DirectoryInfo(fullProjectName).Parent;

            if (parentProjectDirectory == null)
            {
                throw new InvalidOperationException("The Project Parent Directory Is Null!");
            }

            // Yes, my naming is... perfect?
            var parentProjectParentDirectory = parentProjectDirectory.Parent;

            if (parentProjectParentDirectory == null)
            {
                throw new InvalidOperationException("The Parent Project Parent Directory Is Null!");
            }

            // If there's no solution folder, we can add the project directory to the solution.
            if (solutionFolder == null)
            {
                return Dte.Solution.AddFromFile(
                        Path.Combine(Path.Combine(parentProjectParentDirectory.FullName, newProjectDirectory), newProjectFileName));
            }

            // Otherwise we must add the renamed project to the solution folder.
            return solutionFolder.AddFromFile(
                        Path.Combine(Path.Combine(parentProjectParentDirectory.FullName, newProjectDirectory), newProjectFileName));
        }
    }
}

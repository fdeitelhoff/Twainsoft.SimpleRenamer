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
                var selectedProject = GetSelectedProject();

                // Get the new project name from the user.
                var rename = new RenameProjectDialog(selectedProject.Name);
                var result = rename.ShowDialog();
                if (!result.HasValue || !result.Value)
                {
                    return;
                }

                // Check if this is necessary when the references check was refactored!
                ProjectsWithReferences.Clear();
                
                // Save all changes that were made before the renaming process. Just for safety!
                SaveSolutionFile();

                // Check if there's an solution folder or return null.
                var solutionFolder = GetSolutionFolder(selectedProject);

                var projectFileName = Path.GetFileNameWithoutExtension(selectedProject.FileName);
                var parentProjectDirectory = new DirectoryInfo(selectedProject.FullName).Parent.Name;
                //var solutionPath = selectedProject.DTE.Solution.FullName;

                // Get the startup project and set it again after this project gets deleted.
                // this uses the UniqueName of the project!
                // Try this here: Dte.Solution.Properties.Item("StartupProject").Value when this gets refactored
                var startupProjects = Dte.Solution.SolutionBuild.StartupProjects as Array;
                var startupName = startupProjects.GetValue(0).ToString();
                var isStartupProject = startupName == selectedProject.UniqueName;

                // Another try for the mighty AccessViolation:
                // Remove the project first and then rename it? Is this possible or will we get an project is not available then?

                var oldProjectName = selectedProject.Name;
                // This is a little bit scary: I need the old project path, before it gets moved. But this instance will have the new name after it gets renamed.
                // Change this behavior!
                OldProject = selectedProject;
                OldProjectName = oldProjectName;

                var newProjectName = rename.GetProjectName();
                selectedProject.Name = newProjectName;

                var fname = Path.GetFileName(selectedProject.FileName);
                var fullName = selectedProject.FullName;
                var newDirectory = Path.GetFileNameWithoutExtension(selectedProject.FileName);

                IVsHierarchy unloadHierarchy;
                Solution.GetProjectOfUniqueName(selectedProject.UniqueName, out unloadHierarchy);

                // Save the complete solution.
                //Solution.SaveSolutionElement((uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                // Save the renamed project.
                //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, unloadHierarchy, 0);

                //var projectType = Guid.Empty;
                //var projectIid = Guid.Empty;
                //IntPtr proj;
                //Solution.CreateProject(ref projectType, fullName, null, null, (uint)__VSCREATEPROJFLAGS.CPF_OPENFILE, ref projectIid,
                //    out proj);

                // Activate COMExceptions to be thrown?
                var newProject = selectedProject;

                if (projectFileName == parentProjectDirectory)
                {
                    // Maybe here or just in case the directory gets renamed:                   
                    // Check if other projects depend on the renamed one. In this case we must collect those references and change them after the renaming process is finished.
                    // First it's implemented here! If this use case is relevant for the normal project renaming too, I'll move the code outside the if statement.

                    //var bla2 = dte.Solution.Projects.Item("CA1");
                    // Get the references projects ands dlls of the currently renamed project.
                    //var project = newProject.Object as VSProject2;

                    //var references = project.References as References2;

                    //foreach (Reference5 reference in references)
                    //{
                    //    Debug.WriteLine(reference.Name + " " + reference.Path);
                    //}

                    CheckProjects(Dte, newProject);

                    //      If TypeOf objProject.Object Is VSLangProj.VSProject Then

                    //objVSProject = DirectCast(objProject.Object, VSLangProj.VSProject)
                    //newProject.Object as VSLangProj.SVsProjectItem


                    //var solution2 = Solution as IVsSolution2;
                    //var solution3 = Solution as IVsSolution3;
                    //var solution4 = Solution as IVsSolution4;
                    //var workspace = MSBuildWorkspace.Create();
                    //var solution = workspace.OpenSolutionAsync(solutionPath).Result;

                    //var defaultProject = selectedProject.is

                    // Unload the saved project.
                    //Solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, unloadHierarchy, 0);
                    //solution4.UnloadProject(projectGuid, (uint)_VSProjectUnloadStatus.UNLOADSTATUS_LoadPendingIfNeeded);
                    //Solution.re

                    //// Save the renamed project.
                    //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, unloadHierarchy, 0);

                    //// Save the complete solution.
                    //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                    //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, unloadHierarchy, 0);

                    //object isExpanded = false;
                    //ErrorHandler.ThrowOnFailure(unloadHierarchy.GetProperty(projectItemId,
                    //    (int)__VSHPROPID.VSHPROPID_Expanded, out isExpanded));

                    // Remove the project from the solution file!
                    Solution.CloseSolutionElement(
                        (uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave |
                        (uint) __VSSLNCLOSEOPTIONS.SLNCLOSEOPT_DeleteProject, unloadHierarchy, 0);

                    // Use the IsDirty flag when this gets outsorced within a new method.
                    //solution.SaveSolutionElement((uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                    // A numeric comparison was attempted on "$(TargetPlatformVersion)" that evaluates to "" instead of a number, in condition "'$(TargetPlatformVersion)' > '8.0'". 
                    // C:\Program Files (x86)\MSBuild\12.0\bin\Microsoft.Common.CurrentVersion.targets
                    // Where the hell is this gonna come from?

                    var di = new DirectoryInfo(fullName).Parent;
                    di.MoveTo(Path.Combine(di.Parent.FullName, newDirectory));
                    //solution3.UpdateProjectFileLocationForUpgrade(di.FullName, Path.Combine(di.Parent.FullName, newDirectory));
                    //solution2.UpdateProjectFileLocation(unloadHierarchy);

                    //SolutionFolder sf;

                    //foreach (Project proj in dte.Solution.Projects)
                    //{
                    //    //System.Windows.Forms.MessageBox.Show(proj.Kind, " proj.Kind ");

                    //    //System.Windows.Forms.MessageBox.Show(proj.Name, " proj.Name ");



                    //    if (proj.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    //    {

                    //        sf = (SolutionFolder)proj.Object;
                    //        //sf.AddFromTemplate(templateFile, destination, strProjectName);
                    //        break;
                    //    }
                    //}

                    var di2 = new DirectoryInfo(fullName).Parent;

                    if (solutionFolder == null)
                    {
                        newProject =
                            Dte.Solution.AddFromFile(
                                Path.Combine(Path.Combine(di2.Parent.FullName, newDirectory), fname));
                    }
                    else
                    {
                        newProject =
                            solutionFolder.AddFromFile(
                                Path.Combine(Path.Combine(di2.Parent.FullName, newDirectory), fname));
                    }

                    //// Save the complete solution.
                    //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                    //solution3.UpdateProjectFileLocationForUpgrade(di.FullName, Path.Combine(di.Parent.FullName, newDirectory));

                    // Save the renamed project.
                    //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, unloadHierarchy, 0);

                    // Save the complete solution.
                    //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                    //using (var reader = new StreamReader(solutionPath))
                    //{
                    //    var contents = reader.ReadToEnd();

                    //    //var regex = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"ConsoleApplication1\", \"" + directory
                    //    //    + "ConsoleApplication1.csproj\", \"" + projectGuid + "\"";

                    //    //var replace = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"ConsoleApplication1\", \"" + newDirectory
                    //    //    + "ConsoleApplication1.csproj\", \"" + projectGuid + "\"";

                    //    //Regex.Replace(contents, regex, replace);

                    //    //File.WriteAllText(solutionPath, contents);

                    //}

                    //if (Convert.ToBoolean(isExpanded))
                    //{
                    //    ErrorHandler.ThrowOnFailure(unloadHierarchy.SetProperty(VSConstants.VSITEMID_ROOT,
                    //        (int)__VSHPROPID.VSHPROPID_Expanded, true));
                    //}

                    // Use the IsDirty flag when this gets outsorced within a new method.
                    Solution.SaveSolutionElement((uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                    //new DirectoryInfo(selectedProject.FullName).Parent.MoveTo();

                    //var dte = Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;
                    //dte.ExecuteCommand("Project.ReloadProject");
                    //solution4.ReloadProject(projectGuid);

                    // Save the complete solution.
                    // Is this neccessarry?
                    //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);
                }

                //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                //foreach (Property property in newProject.Properties)
                //{
                //    try
                //    {
                //        Debug.WriteLine(property.Name + " " + property.Value);
                //    }
                //    catch (Exception ex3)
                //    {
                //        Debug.WriteLine(ex3);
                //    }

                //}

                //foreach (Property property in solution.GetProperty(__VSPROPID.VSPROPID_IsSolutionDirty))
                //{
                //    try
                //    {
                //        Debug.WriteLine(property.Name + " " + property.Value);
                //    }
                //    catch (Exception ex3)
                //    {
                //        Debug.WriteLine(ex3);
                //    }

                //}

                // Now we want to add lost project references due to the name change.
                foreach (var proj in ProjectsWithReferences)
                {
                    var project = proj.Object as VSProject2;

                    var references = project.References as References2;

                    references.AddProject(newProject);

                    // Better this way?
                    //dte.Solution.SolutionBuild.BuildProject();
                }

                IVsHierarchy newProjectHierarchy;
                Solution.GetProjectOfUniqueName(newProject.UniqueName, out newProjectHierarchy);

                // Changing the default namespace and the assembly name.
                var defaultNamespace = newProject.Properties.Item("DefaultNamespace") as Property;
                var assemblyName = newProject.Properties.Item("AssemblyName") as Property;

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
                if (newProject.IsDirty)
                {
                    Solution.SaveSolutionElement((uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, newProjectHierarchy,
                        0);
                }

                // Change some info in the AssemblyInfo.cs file!
                var bla = newProject.ProjectItems.Item("Properties");
                var ai = bla.ProjectItems.Item("AssemblyInfo.cs");

                var at = ai.FileCodeModel.CodeElements.Item("AssemblyTitle") as CodeAttribute2;
                var assemblyProduct = ai.FileCodeModel.CodeElements.Item("AssemblyProduct") as CodeAttribute2;

                if (at.Value.Contains(oldProjectName))
                {
                    at.Value = at.Value.Replace(oldProjectName, newProjectName);
                }

                if (assemblyProduct.Value.Contains(oldProjectName))
                {
                    assemblyProduct.Value = assemblyProduct.Value.Replace(oldProjectName, newProjectName);
                }

                if (ai.IsDirty)
                {
                    Solution.SaveSolutionElement((uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, newProjectHierarchy,
                        0);
                }

                if (isStartupProject)
                {
                    //var startupProjects = dte.Solution.SolutionBuild.StartupProjects as Array;
                    //startupProjects.SetValue(newProject.UniqueName, 0);
                    //dte.Solution.SolutionBuild.StartupProjects

//// ReSharper disable UseArrayCreationExpression.1
//                        var newStartUpProjects = Array.CreateInstance(typeof (object), 1);
//// ReSharper restore UseArrayCreationExpression.1
//                        newStartUpProjects.SetValue(newProject.UniqueName, 0);

//                        dte.Solution.SolutionBuild.StartupProjects = newStartUpProjects;

                    //if (dte.Solution.IsDirty)
                    //{
                    //    solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);
                    //}

                    Dte.Solution.Properties.Item("StartupProject").Value = newProject.Name;

                    // This should work if I can get the path of the project node within the solution structure.
                    //UIHierarchy UIH = dte.ToolWindows.SolutionExplorer;

                    ////UIH.GetItem(@"ConsoleApplication4\SolutionFolder\ClassLibraryNeu1")
                    //UIHierarchyItem UIHItem = UIH.GetItem(newProject.Name);

                    //UIHItem.Select(vsUISelectionType.vsUISelectionTypeSetCaret);

                    //UIHItem.UIHierarchyItems.Expanded = true;

                    //UIHItem.Select(vsUISelectionType.vsUISelectionTypeSelect);



                }

                Dte.Solution.SolutionBuild.Build();

                //foreach (CodeElement codeElement in ai.FileCodeModel.CodeElements)
                //{
                //    try
                //    {
                //        Debug.WriteLine(codeElement.Name + " " + codeElement.Kind);
                //    }
                //    catch (Exception ex4)
                //    {
                //        Debug.WriteLine(ex4);
                //    }
                //}

                // Get Access to the AssemblyInfo.cs to change some other stuff.
                // Properties {6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}
                //foreach (ProjectItem projectItem in newProject.ProjectItems)
                //{
                //    Debug.WriteLine(projectItem.Name + " " + projectItem.Kind);
                //}

                //Solution.OnAfterRenameProject(selectedProject, selectedProject.Name, "bla", 0);

                //var window = this.FindToolWindow(typeof(MyToolWindow), 0, true);
                //if ((null == window) || (null == window.Frame))
                //{
                //    throw new NotSupportedException(Resources.Resources.CanNotCreateWindow);
                //}
                //var windowFrame = (IVsWindowFrame)window.Frame;
                //Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

                //var Solution = GetGlobalService(typeof(IVsSolution)) as IVsSolution;

                //var hierarchy = SolutionEventsHandler.hier;

                //Solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, hierarchy, 0);
            }
            catch (COMException comException)
            {
                Debug.WriteLine(comException);
            }
            // Just as a fail safe senario! Should be remove in future versions.
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
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

        uint projectItemId;
        private Project GetSelectedProject()
        {
            IntPtr hierarchyPointer, selectionContainerPointer;
            object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            

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
        private void CheckProjects(DTE2 dte, Project newProject)
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

        //private void SolutionEventsOnRenamed(string oldName)
        //{
        //    Debug.WriteLine("SolutionEventsOnRenamed");
        //}

        //public DTE DTE2 { get; set; }

        //private void SolutionEventsOnProjectRenamed(Project project, string oldName)
        //{
        //    Debug.WriteLine("SolutionEventsOnProjectRenamed");
        //}
    }
}

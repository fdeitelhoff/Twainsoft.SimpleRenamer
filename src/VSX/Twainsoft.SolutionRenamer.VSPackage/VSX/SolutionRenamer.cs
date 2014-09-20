using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Twainsoft.SolutionRenamer.VSPackage.GUI;

namespace Twainsoft.SolutionRenamer.VSPackage.VSX
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(MyToolWindow))]
    [Guid(GuidList.guidTwainsoft_SolutionRenamer_VSPackagePkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class SolutionRenamer : Package
    {
        //private SolutionEventsHandler SolutionEventsHandler { get; set; }
        //public DTE Dte2 { get; set; }
        //private IVsSolution Solution { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                var menuCommandID = new CommandID(GuidList.guidTwainsoft_SolutionRenamer_VSPackageCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                //MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                //mcs.AddCommand( menuItem );
                // Create the command for the tool window
                var toolwndCommandID = new CommandID(GuidList.guidTwainsoft_SolutionRenamer_VSPackageCmdSet, (int)PkgCmdIDList.cmdidMyTool);
                var menuToolWin = new MenuCommand(OnShowToolWindow, toolwndCommandID);
                mcs.AddCommand( menuToolWin );
            }

            //var solution = GetGlobalService(typeof(IVsSolution)) as IVsSolution;
            //Solution = solution;
            //SolutionEventsHandler = new SolutionEventsHandler(solution);

            //solution.AdviseSolutionEvents(SolutionEventsHandler, out SolutionEventsHandler.EventsCookie);
            //Dte2 = Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;
            //Dte2.Events.SolutionEvents.ProjectRenamed += SolutionEventsOnProjectRenamed;
            //DTE2.Events.SolutionEvents.Renamed += SolutionEventsOnRenamed;
        }

        //private void SolutionEventsOnProjectRenamed(Project project, string oldname)
        //{
        //    Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

        //    IVsHierarchy selectedHierarchy;
        //    Solution.GetProjectOfUniqueName(project.UniqueName, out selectedHierarchy);

        //    Solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, selectedHierarchy, 0);
        //}

        private void OnShowToolWindow(object sender, EventArgs e)
        {
            // Check if its necessarry to save the solution file and project file that often!
            var rename = new RenameProjectDialog();
            var result = rename.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var Solution = GetGlobalService(typeof (IVsSolution)) as IVsSolution;

                IntPtr hierarchyPointer, selectionContainerPointer;
                Object selectedObject = null;
                IVsMultiItemSelect multiItemSelect;
                uint projectItemId;

                IVsMonitorSelection monitorSelection =
                    (IVsMonitorSelection) Package.GetGlobalService(
                        typeof (SVsShellMonitorSelection));

                monitorSelection.GetCurrentSelection(out hierarchyPointer,
                    out projectItemId,
                    out multiItemSelect,
                    out selectionContainerPointer);

                IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                    hierarchyPointer,
                    typeof (IVsHierarchy)) as IVsHierarchy;

                if (selectedHierarchy != null)
                {
                    ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                        projectItemId,
                        (int) __VSHPROPID.VSHPROPID_ExtObject,
                        out selectedObject));
                }

                Project selectedProject = selectedObject as Project;

                IVsHierarchy projHierarchy;
                Solution.GetProjectOfUniqueName(selectedProject.UniqueName, out projHierarchy);

                var projectGuid = Guid.Empty;
                Solution.GetGuidOfProject(projHierarchy, out projectGuid);

                var fileName = Path.GetFileNameWithoutExtension(selectedProject.FileName);
                var directory = new DirectoryInfo(selectedProject.FullName).Parent.Name;
                var solutionPath = selectedProject.DTE.Solution.FullName;

                SolutionFolder solutionFolder = null;

                if (selectedProject.ParentProjectItem != null)
                {
                    var parentProject = selectedProject.ParentProjectItem.Collection.Parent as Project;
                    solutionFolder = parentProject.Object as SolutionFolder;
                }

                selectedProject.Name = rename.GetProjectName();

                var fname = Path.GetFileName(selectedProject.FileName);
                var fullName = selectedProject.FullName;
                var newDirectory = Path.GetFileNameWithoutExtension(selectedProject.FileName);

                IVsHierarchy unloadHierarchy;
                Solution.GetProjectOfUniqueName(selectedProject.UniqueName, out unloadHierarchy);

                // Save the complete solution.
                Solution.SaveSolutionElement((uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                //// Save the renamed project.
                //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, unloadHierarchy, 0);

                //var projectType = Guid.Empty;
                //var projectIid = Guid.Empty;
                //IntPtr proj;
                //Solution.CreateProject(ref projectType, fullName, null, null, (uint)__VSCREATEPROJFLAGS.CPF_OPENFILE, ref projectIid,
                //    out proj);

                if (fileName == directory)
                {
                    var solution2 = Solution as IVsSolution2;
                    var solution3 = Solution as IVsSolution3;
                    var solution4 = Solution as IVsSolution4;
                    //var workspace = MSBuildWorkspace.Create();
                    //var solution = workspace.OpenSolutionAsync(solutionPath).Result;

                    // Unload the saved project.
                    //Solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, unloadHierarchy, 0);
                    //solution4.UnloadProject(projectGuid, (uint)_VSProjectUnloadStatus.UNLOADSTATUS_LoadPendingIfNeeded);
                    //Solution.re

                    //// Save the renamed project.
                    //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, unloadHierarchy, 0);

                    //// Save the complete solution.
                    //Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                    try
                    {
                        Solution.CloseSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave | (uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_DeleteProject, unloadHierarchy, 0);

                        var di = new DirectoryInfo(fullName).Parent;
                        di.MoveTo(Path.Combine(di.Parent.FullName, newDirectory));
                        //solution3.UpdateProjectFileLocationForUpgrade(di.FullName, Path.Combine(di.Parent.FullName, newDirectory));
                        //solution2.UpdateProjectFileLocation(unloadHierarchy);

                        var dte = Package.GetGlobalService(typeof(SDTE)) as EnvDTE80.DTE2;

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
                            dte.Solution.AddFromFile(Path.Combine(Path.Combine(di2.Parent.FullName, newDirectory), fname));
                        }
                        else
                        {
                            solutionFolder.AddFromFile(Path.Combine(Path.Combine(di2.Parent.FullName, newDirectory), fname));
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
                    }
                    catch (Exception e2)
                    {
                        Debug.WriteLine(e2);
                    }

                    //new DirectoryInfo(selectedProject.FullName).Parent.MoveTo();

                    //var dte = Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;
                    //dte.ExecuteCommand("Project.ReloadProject");
                    solution4.ReloadProject(projectGuid);

                    // Save the complete solution.
                    // Is this neccessarry?
                    Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);
                }

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

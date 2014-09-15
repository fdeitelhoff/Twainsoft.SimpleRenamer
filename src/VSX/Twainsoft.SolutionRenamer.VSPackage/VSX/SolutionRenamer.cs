using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
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
                

                selectedProject.Name = rename.GetProjectName();
                //selectedProject.Save(selectedProject.FileName);

                var fullName = selectedProject.FullName;

                IVsHierarchy unloadHierarchy;
                ////Solution.GetGuidOfProject()
                Solution.GetProjectOfUniqueName(selectedProject.UniqueName, out unloadHierarchy);

                // Save the complete solution.
                Solution.SaveSolutionElement((uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);

                // Save the renamed project.
                Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, unloadHierarchy, 0);

                // Unload the saved project.
                Solution.CloseSolutionElement((uint) __VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, unloadHierarchy, 0);

                var projectType = Guid.Empty;
                var projectIid = Guid.Empty;
                IntPtr proj;
                Solution.CreateProject(ref projectType, fullName, null, null, (uint)__VSCREATEPROJFLAGS.CPF_OPENFILE, ref projectIid,
                    out proj);

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

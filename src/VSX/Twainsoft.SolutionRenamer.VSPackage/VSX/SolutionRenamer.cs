using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
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
    public sealed class SolutionRenamer : Package, IVsSolutionEvents, IVsSolutionEvents4
    {
        private void OnShowToolWindow(object sender, EventArgs e)
        {
            var window = this.FindToolWindow(typeof(MyToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.Resources.CanNotCreateWindow);
            }
            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }


        protected override void Initialize()
        {
            //Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
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

            var service = GetService(typeof(SVsSolution)) as IVsSolution;

            uint cookie = 1;
            service.AdviseSolutionEvents(this, out cookie);
            //DTE2 = Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;
            //DTE2.Events.SolutionEvents.ProjectRenamed += SolutionEventsOnProjectRenamed;
            //DTE2.Events.SolutionEvents.Renamed += SolutionEventsOnRenamed;
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
        public int OnAfterRenameProject(IVsHierarchy hierarchy)
        {
            object project;

            ErrorHandler.ThrowOnFailure
                (hierarchy.GetProperty(
                    VSConstants.VSITEMID_ROOT,
                    (int)__VSHPROPID.VSHPROPID_ExtObject,
                    out project));

            var p = project as Project;


            return VSConstants.S_OK;
            //return VSConstants.E_NOTIMPL;
            //throw new NotImplementedException();
        }

        public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.E_NOTIMPL;
        }
    }
}

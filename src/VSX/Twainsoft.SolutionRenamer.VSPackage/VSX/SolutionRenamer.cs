using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
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
    public sealed class SolutionRenamer : Package
    {
        private void OnShowToolWindow(object sender, EventArgs e)
        {
            ToolWindowPane window = this.FindToolWindow(typeof(MyToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }


        protected override void Initialize()
        {
            //Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidTwainsoft_SolutionRenamer_VSPackageCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                //MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                //mcs.AddCommand( menuItem );
                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidTwainsoft_SolutionRenamer_VSPackageCmdSet, (int)PkgCmdIDList.cmdidMyTool);
                MenuCommand menuToolWin = new MenuCommand(OnShowToolWindow, toolwndCommandID);
                mcs.AddCommand( menuToolWin );
            }

            //var service = GetService(typeof (SVsSolution)) as IVsSolution;

            //uint cookie = 1;
            //service.AdviseSolutionEvents(this, out cookie);
            DTE2 = Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;
            DTE2.Events.SolutionEvents.ProjectRenamed += SolutionEventsOnProjectRenamed;
            DTE2.Events.SolutionEvents.Renamed += SolutionEventsOnRenamed;
        }

        private void SolutionEventsOnRenamed(string oldName)
        {
            Debug.WriteLine("SolutionEventsOnRenamed");
        }

        public DTE DTE2 { get; set; }

        private void SolutionEventsOnProjectRenamed(Project project, string oldName)
        {
            Debug.WriteLine("SolutionEventsOnProjectRenamed");
        }
    }
}

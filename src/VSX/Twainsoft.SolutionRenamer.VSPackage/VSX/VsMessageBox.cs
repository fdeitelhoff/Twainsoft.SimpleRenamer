using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Twainsoft.SolutionRenamer.VSPackage.VSX
{
    public enum VsMessageResult
    {
        Abort,
        Cancel = 2,
        Ignore,
        No = 7,
        Ok,
        Retry,
        Yes = 6
    }

    public static class VsMessageBox
    {
        private static VsMessageResult ShowMessageBox(string title, string message,
            OLEMSGBUTTON buttons, OLEMSGDEFBUTTON defaultButton, OLEMSGICON icon)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;

            if (uiShell == null)
            {
                throw new InvalidOperationException("Cannot Find The Visual Studio UI Shell!");
            }

            int result;
            var clsid = Guid.Empty;

            uiShell.ShowMessageBox(
                0,
                ref clsid,
                title,
                message,
                string.Empty,
                0,
                buttons,
                defaultButton,
                icon,
                0,        // false
                out result);

            return (VsMessageResult)result;
        }

        public static VsMessageResult ShowInfoMessageBox(string title, string message)
        {
            return ShowMessageBox(title, message, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_INFO);
        }

        public static VsMessageResult ShowErrorMessageBox(string title, string message)
        {
            return ShowMessageBox(title, message, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_CRITICAL);
        }

        public static VsMessageResult ShowQuestionMessageBox(string title, string message, OLEMSGBUTTON buttons, OLEMSGDEFBUTTON defaultButton)
        {
            return ShowMessageBox(title, message, buttons, defaultButton, OLEMSGICON.OLEMSGICON_QUERY);
        }
    }
}
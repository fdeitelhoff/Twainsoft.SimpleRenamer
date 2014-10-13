using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;

namespace Twainsoft.SolutionRenamer.VSPackage.VSX
{
    public class RenameData
    {
        public IVsSolution Solution { get; set; }
        public DTE2 Dte { get; set; }

        public string NewProjectName { get; set; }
    }
}

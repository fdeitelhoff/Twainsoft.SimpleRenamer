using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;

namespace Twainsoft.SolutionRenamer.VSPackage.VSX
{
    public class RenameData
    {
        public IVsSolution Solution { get; set; }
        public DTE2 Dte { get; set; }

        public string NewProjectName { get; set; }
        public Project RenamedProject { get; set; }

        public List<Project> ProjectsWithReferences { get; private set; }

        public RenameData()
        {
            ProjectsWithReferences = new List<Project>();
        }
    }
}

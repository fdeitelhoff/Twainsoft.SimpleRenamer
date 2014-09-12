using System;
using EnvDTE;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Twainsoft.SolutionRenamer.VSPackage.VSX
{
    public class SolutionEventsHandler : IDisposable, IVsSolutionEvents, IVsSolutionEvents4
    {
        public uint EventsCookie;

        private IVsSolution Solution { get; set; }
        
        public SolutionEventsHandler(IVsSolution solution)
        {
            if (solution == null)
            {
                throw new ArgumentNullException("solution");
            }

            Solution = solution;

            EventsCookie = 1;
        }

        public int OnAfterRenameProject(IVsHierarchy hierarchy)
        {
            object project;

            ErrorHandler.ThrowOnFailure
                (hierarchy.GetProperty(
                    VSConstants.VSITEMID_ROOT,
                    (int)__VSHPROPID.VSHPROPID_ExtObject,
                    out project));

            var p = project as Project;

            // 1. The solution file (.sln) must reflect the new project name, so we save the solution first.
            SaveSolutionFile();

            // 2. If the project directory contains the old project name, rename it.
            RenameProjectDirectory(p);

            return VSConstants.S_OK;
            //return VSConstants.E_NOTIMPL;
            //throw new NotImplementedException();
        }

        private void RenameProjectDirectory(Project project)
        {
            var solutionPath = project.DTE.Solution.FullName;

            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionPath).Result;

            //IVsHierarchy hierarchy;
            //Solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);

            //object newProject;
            //ErrorHandler.ThrowOnFailure
            //    (hierarchy.GetProperty(
            //        VSConstants.VSITEMID_ROOT,
            //        (int) __VSHPROPID.VSHPROPID_ExtObject,
            //        out newProject));

            //var p = newProject as Project;
            //Solution.GetProjectOfGuid(project.UniqueName)
        }

        private void SaveSolutionFile()
        {
            Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);
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

        public void Dispose()
        {
            // TODO: Release the events cookie!
        }
    }
}

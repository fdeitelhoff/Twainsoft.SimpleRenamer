using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Twainsoft.SimpleRenamer.VSPackage.GUI.Options
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("12D3C984-9150-481F-A0B8-4A37151FBD05")]
    public class OptionsStore : DialogPage
    {
        [Category("Twainsoft SimpleRenamer")]
        [DisplayName("Should the complete solution be compiled after the renaming was successfully?")]
        [Description("Should the complete solution be compiled after the renaming was successfully?")]
        public bool CompileSolutionAfterRenaming { get; set; }

        [Category("Twainsoft SimpleRenamer")]
        [DisplayName("Change Strings in the AssemblyInfo.cs of the renamed project?")]
        [Description("Change Strings in the AssemblyInfo.cs of the renamed project?")]
        public bool ChangeAssemblyInfoAfterRenaming { get; set; }
    }
}

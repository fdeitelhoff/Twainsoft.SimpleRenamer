using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Twainsoft.SimpleRenamer.VSPackage.GUI.Options
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("12D3C984-9150-481F-A0B8-4A37151FBD05")]
    public class OptionsStore : DialogPage
    {
        [Category("Twainsoft SimpleRenamer")]
        [DisplayName("ProjectProperties")]
        [Description("Change Application Properties of the renamed project?")]
        public bool ChangeProjectPropertiesAfterRenaming { get; set; }

        [Category("Twainsoft SimpleRenamer")]
        [DisplayName("AssemblyInfo")]
        [Description("Change Strings in the AssemblyInfo.cs of the renamed project?")]
        public bool ChangeAssemblyInfoAfterRenaming { get; set; }

        [Category("Twainsoft SimpleRenamer")]
        [DisplayName("ProjectReferences")]
        [Description("Change References for the renamed project?")]
        public bool ChangeProjectReferencesAfterRenaming { get; set; }
        
        [Category("Twainsoft SimpleRenamer")]
        [DisplayName("StartupProject")]
        [Description("Restore Startup Project after renaming?")]
        public bool RestoreStartupProjectAfterRenaming { get; set; }

        [Category("Twainsoft SimpleRenamer")]
        [DisplayName("CompileSolution")]
        [Description("Should the complete solution be compiled after the renaming was successfully?")]
        public bool RebuildSolutionAfterRenaming { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window
        {
            get
            {
                return new OptionsView(this);
            }
        }
    }
}

using System.Windows.Forms;

namespace Twainsoft.SimpleRenamer.VSPackage.GUI.Options
{
    public partial class OptionsView : UserControl
    {
        private OptionsStore OptionsStore { get; set; }

        private OptionsView()
        {
            InitializeComponent();
        }

        public OptionsView(OptionsStore optionsStore)
            : this()
        {
            OptionsStore = optionsStore;

            Initialize();
        }

        private void Initialize()
        {
            changeProjectProperties.Checked = OptionsStore.ChangeProjectPropertiesAfterRenaming;
            changeAssemblyInfo.Checked = OptionsStore.ChangeAssemblyInfoAfterRenaming;
            changeProjectReferences.Checked = OptionsStore.ChangeProjectReferencesAfterRenaming;
        }

        private void changeProjectProperties_CheckedChanged(object sender, System.EventArgs e)
        {
            OptionsStore.ChangeProjectPropertiesAfterRenaming = changeProjectProperties.Checked;
        }

        private void changeAssemblyInfo_CheckedChanged(object sender, System.EventArgs e)
        {
            OptionsStore.ChangeAssemblyInfoAfterRenaming = changeAssemblyInfo.Checked;
        }

        private void changeProjectReferences_CheckedChanged(object sender, System.EventArgs e)
        {
            OptionsStore.ChangeProjectReferencesAfterRenaming = changeProjectReferences.Checked;
        }
    }
}

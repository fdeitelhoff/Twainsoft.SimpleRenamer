namespace Twainsoft.SolutionRenamer.VSPackage.GUI
{
    public partial class RenamingProgressDialog
    {
        public RenamingProgressDialog()
        {
            InitializeComponent();
        }

        public void SetMessage(string oldProjectName, string newProjectName)
        {
            StatusMessage.Content = string.Format("Project {0} gets renamed to {1}...", oldProjectName, newProjectName);
        }
    }
}

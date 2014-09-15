using System.Windows;

namespace Twainsoft.SolutionRenamer.VSPackage.GUI
{
    public partial class RenameProjectDialog
    {
        public RenameProjectDialog()
        {
            InitializeComponent();
        }

        public string GetProjectName()
        {
            return ProjectName.Text.Trim();
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Close();
        }
    }
}

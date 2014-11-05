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
            
        }
    }
}

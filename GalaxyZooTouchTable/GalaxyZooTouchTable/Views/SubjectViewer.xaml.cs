using System.Windows.Controls;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// Interaction logic for SubjectViewer.xaml
    /// </summary>
    public partial class SubjectViewer : UserControl
    {
        public SubjectViewer()
        {
            InitializeComponent();
        }

        private void Image_TouchEnter(object sender, System.Windows.Input.TouchEventArgs e)
        {
            System.Console.WriteLine("TOUCH ENTER");
        }

        private void Image_TouchLeave(object sender, System.Windows.Input.TouchEventArgs e)
        {
            System.Console.WriteLine("TOUCH LEAVE");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HealthReporter.Controls
{
    /// <summary>
    /// Interaction logic for ManagePresetsControl.xaml
    /// </summary>
    public partial class ManagePresetsControl : UserControl
    {
        private MainWindow _parent;

        public ManagePresetsControl(MainWindow parent)
        {
            InitializeComponent();
            this._parent = parent;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this._parent.stkTest.Children.Clear();
            TestsUserControl obj = new TestsUserControl(this._parent);
            this._parent.stkTest.Children.Add(obj);
        }

        private void btnNewPreset_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

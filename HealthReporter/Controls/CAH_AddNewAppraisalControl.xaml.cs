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
using HealthReporter.Models;

namespace HealthReporter.Controls
{
    /// <summary>
    /// Interaction logic for CAH_AddNewAppraisal.xaml
    /// </summary>
    public partial class CAH_AddNewAppraisalControl : UserControl
    {
        private Client client;
        private List<string> tests;
        private MainWindow _parent;

        public CAH_AddNewAppraisalControl()
        {
            InitializeComponent();
        }

        public CAH_AddNewAppraisalControl(MainWindow _parent, Client client, List<string> tests)
        {
            InitializeComponent();
            this._parent = _parent;
            this.client = client;
            this.tests = tests;

        }

        private void btn_Back(object sender, RoutedEventArgs e)
        {
            this._parent.stkTest.Children.Clear();
            CAH_SelectTestControl obj = new CAH_SelectTestControl(this._parent, client);
            this._parent.stkTest.Children.Add(obj);
        }
    }
}

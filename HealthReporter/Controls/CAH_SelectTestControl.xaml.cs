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
    /// Interaction logic for CAH_SelectTestControl.xaml
    /// </summary>
    public partial class CAH_SelectTestControl : UserControl
    {
        private Client client;
        private MainWindow _parent;
        private List<String> tests;
     

        public CAH_SelectTestControl(MainWindow _parent, Client client)
        {

            var repo = new TestRepository();
            IList<Test> tests = repo.FindAll();

            InitializeComponent();
            this._parent = _parent;
            this.client = client;

            foreach (Test elem in tests)
            {
                listBox.Items.Add(elem.name);
            }
           
        }

        private void btn_Back(object sender, RoutedEventArgs e)
        {
            this._parent.stkTest.Children.Clear();
            CAH obj = new CAH(this._parent,client);
            this._parent.stkTest.Children.Add(obj);
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            foreach (var item in listBox.SelectedItems)
            {
                tests.Add(item.ToString());
            }
            this._parent.stkTest.Children.Clear();
            CAH_AddNewAppraisalControl obj = new CAH_AddNewAppraisalControl(this._parent, client, tests);
            this._parent.stkTest.Children.Add(obj);
        }
    }
}

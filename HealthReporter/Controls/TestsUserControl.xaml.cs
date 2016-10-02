using HealthReporter.Models;
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
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class TestsUserControl : UserControl
    {
        private MainWindow _parent;

        public TestsUserControl(MainWindow parent)
        {
            InitializeComponent();
            this._parent = parent;

            var repo = new TestRepository();
            IList<Test> tests = repo.FindAll();

            dataGrid.ItemsSource = tests;
        }


        private void btn_AddNewTest(object sender, RoutedEventArgs e)
        {
            AddNewTestControl obj = new AddNewTestControl(this._parent);
            this._parent.stkTest.Children.Clear();
            this._parent.stkTest.Children.Add(obj);

        }
    }
}

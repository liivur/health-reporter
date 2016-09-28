using HealthReporter.Models;
using HealthReporter.Utilities;
using Insight.Database;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ClientUserControl : UserControl
    { 

        public ClientUserControl()
        {
            InitializeComponent();
            DatabaseUtility.checkDb();
            IList<Client> clients = DatabaseUtility.getConnection().QuerySql<Client>(
                "SELECT * FROM clients");

            dataGrid.ItemsSource = clients;
        }


        private void btn_AddNewClient(object sender, RoutedEventArgs e)
        {
            AddNewClientControl obj = new AddNewClientControl();
            MainWindow._window1.stkTest.Children.Clear();
            MainWindow._window1.stkTest.Children.Add(obj);

        }
    }
}

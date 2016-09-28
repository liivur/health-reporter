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
    /// Interaction logic for AddNewClientControl.xaml
    /// </summary>
    public partial class AddNewClientControl : UserControl
    {
        private int _counter = 1;

        public AddNewClientControl()
        {
            InitializeComponent(); 
        }

        private void btn_Back(object sender, RoutedEventArgs e)
        {                        
            MainWindow._window1.stkTest.Children.Clear();
            ClientUserControl obj = new ClientUserControl();
            MainWindow._window1.stkTest.Children.Add(obj);
        }

        private void btn_CreateNewClient(object sender, RoutedEventArgs e)
        {
            //TODO: add validation also.

            var connection = DatabaseUtility.getConnection();
            
            var client = new Client() { id = BitConverter.GetBytes(_counter), firstName = this.firstName.Text, lastName = this.lastName.Text };

            var repo = new ClientRepository();
            repo.InsertClient(client);
            _counter++;

            MainWindow._window1.stkTest.Children.Clear();
            ClientUserControl obj = new ClientUserControl();
            MainWindow._window1.stkTest.Children.Add(obj);

        }


    }
}

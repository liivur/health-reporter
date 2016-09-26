using HealthReporter.Models;
using HealthReporter.Utilities;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace HealthReporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _counter = 1;

        public MainWindow()
        {
            InitializeComponent();
            DatabaseUtility.checkDb();
            IList<Client> clients = DatabaseUtility.getConnection().QuerySql<Client>(
                "SELECT * FROM Client");

            dataGrid.ItemsSource = clients;
        }

        private void buttonClick(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "Vajutasid";
            var connection = DatabaseUtility.getConnection();

            var client = new Client() { id = BitConverter.GetBytes(_counter), firstName = "Troeg's Mad Elf", lastName = "Something" };

            var repo = new ClientRepository();
            repo.InsertClient(client);
            _counter++;

            //IList<Client> beer = DatabaseUtility.getConnection().QuerySql<Client>(
            //    "SELECT * FROM Client WHERE firstName = @Name",
            //    new { Name = "Troeg's Mad Elf" });

            IList<Client> beer = DatabaseUtility.getConnection().QuerySql<Client>(
                "SELECT * FROM Client");

            dataGrid.ItemsSource = beer;
        }
    }
}

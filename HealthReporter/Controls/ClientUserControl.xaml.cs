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
        private MainWindow _parent;
        


        public ClientUserControl(MainWindow parent)
        {
            InitializeComponent();
            this._parent = parent;

            var repo = new ClientRepository();
            IList<Client> clients = repo.FindAll();

            string calTotal = "Total : " + clients.Count;
            total.Text = calTotal;

            dataGrid.ItemsSource = clients;
        }


        private void btn_AddNewClient(object sender, RoutedEventArgs e)
        {
            AddNewClientControl obj = new AddNewClientControl(this._parent);
            this._parent.stkTest.Children.Clear();
            this._parent.stkTest.Children.Add(obj);

        }

        private void btn_Delete(object sender, RoutedEventArgs e)
        {
            if(dataGrid.SelectedItem != null)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Client client = (Client)dataGrid.SelectedItem;
                    var repo = new ClientRepository();
                    repo.Delete(client);

                    //Updating table
                    IList<Client> newTable = repo.FindAll();
                    dataGrid.ItemsSource = newTable;

                    //Updating total
                    string calTotal = "Total : " + newTable.Count;
                    total.Text = calTotal;
                }
               
            }
        }

        private void btn_Update(object sender, RoutedEventArgs e)
        {
           Client client = (Client)dataGrid.SelectedItem;
           
            UpdateClientControl obj = new UpdateClientControl(this._parent);

            this._parent.stkTest.Children.Clear();
            this._parent.stkTest.Children.Add(obj);

            //Initializing fields
            obj.firstName.Text = client.firstName;
            obj.lastName.Text = client.lastName;
            obj.groupId.Text = client.groupId.ToString();
            obj.email.Text = client.email.ToString();
            obj.gender.SelectedIndex = int.Parse(client.gender);

        }

        private void btn_Search(object sender, RoutedEventArgs e)
        {
            string searchBy = search.Text;

            var repo = new ClientRepository();
            IList<Client> clients = repo.FindSearchResult(searchBy);


            dataGrid.ItemsSource = clients;


        }
        

    }
}

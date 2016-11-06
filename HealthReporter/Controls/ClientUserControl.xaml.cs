﻿using HealthReporter.Models;
using HealthReporter.Utilities;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
using System.ComponentModel;

namespace HealthReporter.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ClientUserControl : UserControl
    {
        private MainWindow _parent;
        private Client _client;
        private Models.Group _group;

        private bool allClientsButtonselected;

        public ClientUserControl(MainWindow parent)
        {
            InitializeComponent();
            this._parent = parent;

            var repo = new GroupRepository();
            IList<Models.Group> groups = repo.FindAll();

            groupDataGrid.ItemsSource = groups;


            NoCards.Visibility = Visibility.Visible;
            search.Visibility = Visibility.Hidden;

            btnShowClients.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E0EEEE"));

           

            findClientTotal();

        }

        private void findClientTotal()
        {
            // Total client amount
            var repoC = new ClientRepository();
            IList<Client> allclients = repoC.FindAll();

            clientTotal.Text = allclients.Count.ToString() + " Clients";
        }


        private void btn_AddStuff(object sender, RoutedEventArgs e)
        {
            if (this._client != null && validationCheck() == false)
            {
                MessageBox.Show("Please fill required fields");
                return;
            }
            SaveClientInfo(this._client);
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
            (sender as Button).ContextMenu.IsOpen = true;
        }


        private void btn_AddNewGroup(object sender, RoutedEventArgs e)
        {
            try
            {
                var group = new Models.Group()
                {
                    name = "untitled group",
                };

                var repo2 = new GroupRepository();
                repo2.Insert(group);
                this._group = group;
            }
            catch
            {
                MessageBox.Show("Something went wrong with adding a new group.");
            }

            // Updating Group menu
            var repo = new GroupRepository();
            IList<Models.Group> groups = repo.FindAll();

            // Add focus on new added row, put row into editable mode          
            int row = groups.Count - 1;
            groupDataGrid.Focus();
            groupDataGrid.ItemsSource = groups;
            groupDataGrid.SelectedIndex = row;
            groupDataGrid.CurrentCell = new DataGridCellInfo(groupDataGrid.Items[row], groupDataGrid.Columns[0]);
            groupDataGrid.IsReadOnly = false;
            groupDataGrid.BeginEdit();

            // Making clients grids empty
            clientDataGrid.ItemsSource = null;
            clientDetailDatagrid.DataContext = null;


        }

        private void btn_AddNewClient(object sender, RoutedEventArgs e)
        {
            if (this._group == null)
            {
                MessageBox.Show("Please add or select group first.", "Message");
            }
            else
            {


                //Cheking if there is any groups in the database
                var groupRepo = new GroupRepository();
                IList<Models.Group> groups = groupRepo.FindAll();
                if (groups.Count == 0)
                {
                    MessageBox.Show("Please enter group first.", "Message");
                }
                else
                {


                    try
                    {
                        DateTime enteredDate = Convert.ToDateTime(DateTime.Now);
                        var client = new Client()
                        {
                            firstName = "No Name",
                            lastName = "No Name",
                            groupId = this._group.id,
                            email = "",
                            gender = "1",
                            birthDate = String.Format("{0:yyyy-MM-dd}", enteredDate)
                        };

                        var repo2 = new ClientRepository();
                        repo2.Insert(client);
                        this._client = client;

                    }
                    catch
                    {
                        MessageBox.Show("Something went wrong with adding a new client.");
                    }

                    //updating values in groups menu
                    var repo = new ClientRepository();
                    IList<Client> clients = repo.GetClientsByGroupName(this._group);

                    clientDataGrid.ItemsSource = clients;
                    int row = clients.Count - 1;
                    clientDetailDatagrid.DataContext = this._client;
                    clientDataGrid.SelectedIndex = row;

                    // Setting focus on firstName textbox
                    firstName.Focusable = true;
                    Keyboard.Focus(firstName);
                    firstName.SelectAll();

                    findClientTotal();


                }
            }
        }


        //Adding new group
        private void Add(object sender, RoutedEventArgs e)
        {

            try
            {
                var group = new Models.Group()
                {
                    name = "untitled group",
                };

                var repo2 = new GroupRepository();
                repo2.Insert(group);
            }
            catch
            {
                MessageBox.Show("Something went wrong with adding a new group.");
            }

            //updating values in groups menu
            var repo = new GroupRepository();
            IList<Models.Group> groups = repo.FindAll();

            groupDataGrid.ItemsSource = groups;



        }

        private bool messageShown=false;

        private void groupsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (allClientsButtonselected == true && this._client != null && validationCheck() == false)
            {
                groupDataGrid.IsEnabled = false;
                if (messageShown == false)
                {
                    MessageBox.Show("Please fill required fields");
                    messageShown = true;
                }
                else
                {
                    messageShown = false;
                }

                Dispatcher.BeginInvoke(new Action(delegate {
                    groupDataGrid.SelectedItem = this._group;
                }), System.Windows.Threading.DispatcherPriority.Normal, null);
                groupDataGrid.IsEnabled = false;
                groupDataGrid.IsEnabled = true;
                clientDetailDatagrid.Visibility = Visibility.Visible;
                NoCards.Visibility = Visibility.Hidden;
                search.Visibility = Visibility.Visible;
                search.Visibility = Visibility.Hidden;
                allClientsButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ADD8E6"));

                return;
            }

            allClientsButtonselected = false;
            allClientsButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F8FF"));

            search.Visibility = Visibility.Hidden;
            searchAllClients.Visibility = Visibility.Hidden;
            clientDetailDatagrid.Visibility = Visibility.Hidden;
            delete.Visibility = Visibility.Hidden;
            NoCards.Visibility = Visibility.Visible;

            

            if (this._group!=null && this._client != null && validationCheck() == false)
            {
                groupDataGrid.IsEnabled = false;
                if (messageShown == false)
                {
                    MessageBox.Show("Please fill required fields");
                    messageShown = true;
                }else
                {
                    messageShown = false;
                }
               
                Dispatcher.BeginInvoke(new Action(delegate {
                    groupDataGrid.SelectedItem = this._group;
                }), System.Windows.Threading.DispatcherPriority.Normal, null);
                groupDataGrid.IsEnabled = false;
                groupDataGrid.IsEnabled = true;
                clientDetailDatagrid.Visibility = Visibility.Visible;
                NoCards.Visibility = Visibility.Hidden;
                search.Visibility = Visibility.Visible;

                return;
            }
            SaveClientInfo(this._client);


            Models.Group selectedGroup = (Models.Group)groupDataGrid.SelectedItem;


            this._group = selectedGroup;



            var repo = new ClientRepository();
            if (this._group != null)
            {
                search.Visibility = Visibility.Visible;
                IList<Client> clients = repo.GetClientsByGroupName(this._group);
                search.Text = "";
                clientDataGrid.ItemsSource = clients;
                if (clients.Count > 0)
                {
                    clientDataGrid.SelectedIndex = 0;
                }


            }
        }
        

        private void clientDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._client != null && validationCheck() == false)
            {
                clientDataGrid.IsEnabled = false;
                if (messageShown == false)
                {
                    MessageBox.Show("Please fill required fields");
                    messageShown = true;
                }
                else
                {
                    messageShown = false;
                }
                Dispatcher.BeginInvoke(new Action(delegate {
                    clientDataGrid.SelectedItem = this._client;
                }), System.Windows.Threading.DispatcherPriority.Normal, null);
                clientDataGrid.IsEnabled = false;
                clientDataGrid.IsEnabled = true;

               


                return;
            }

            // When all clients tab is selected
            if (this.allClientsButtonselected == true)
            {

                allClientsButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ADD8E6"));
                Client selClient = (Client)clientDataGrid.SelectedItem;
                searchAllClients.Visibility = Visibility.Visible;
                search.Visibility = Visibility.Hidden;

 
                if (selClient != null)
                {
                    
                    SaveClientInfo(this._client);
                    this._client = selClient;
                    NoCards.Visibility = Visibility.Hidden;
                    clientDetailDatagrid.Visibility = Visibility.Visible;
                    
                    clientDetailDatagrid.DataContext = selClient;
                }

            }
            //Otherwise some group is selected
            else
            {

                var repo = new ClientRepository();
                int clientCount = 0;

                //If we have group
                if (this._group != null)
                {
                    IList<Client> clients = repo.GetClientsByGroupName(this._group);
                    clientCount = clients.Count;
                    
                    SaveClientInfo(this._client);
                }

                //Checking if we have clients under selected group
                if (clientCount <= 0)
                {
                    clientDetailDatagrid.Visibility = Visibility.Hidden;
                    delete.Visibility = Visibility.Hidden;
                    NoCards.Visibility = Visibility.Visible;
                }
                else
                {

                    delete.Visibility = Visibility.Visible;
                    clientDetailDatagrid.Visibility = Visibility.Visible;
                    NoCards.Visibility = Visibility.Hidden;
                   
                    SaveClientInfo(this._client);


                    Client selectedClient = (Client)clientDataGrid.SelectedItem;
                    this._client = selectedClient;


                    clientDetailDatagrid.DataContext = this._client;

                }


            }
        }
        private bool validationCheck()
        {
            if (String.IsNullOrWhiteSpace(this._client.firstName) || String.IsNullOrWhiteSpace(this._client.lastName) ||
              String.IsNullOrWhiteSpace(this._client.gender) || String.IsNullOrWhiteSpace(this._client.birthDate) ||
              (!String.IsNullOrWhiteSpace(this._client.email) && !Regex.IsMatch(this._client.email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")))
            {
                return false;
            }
            return true;
        }


        private void SaveClientInfo(Client client)
        {

            if (client != null)
            {
               
                var repo = new ClientRepository();
                DateTime enteredDate = Convert.ToDateTime(client.birthDate.ToString());
                client.birthDate = String.Format("{0:yyyy-MM-dd}", enteredDate);
                //MessageBox.Show(client.gender, "Message");
               
                repo.Update(client);
            }


        }


        private void btn_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Client client = this._client;
                var repo = new ClientRepository();
                repo.Delete(client);

                
                //updating values in clients menu
                var repoClient = new ClientRepository();
                IList<Client> clients = repoClient.GetClientsByGroupName(this._group);

                clientDataGrid.ItemsSource = clients;
                int row = clients.Count - 1;
                clientDetailDatagrid.DataContext = this._client;
                clientDataGrid.SelectedIndex = row;

                findClientTotal();
            }
        }






        private void btn_Clients(object sender, RoutedEventArgs e)
        {
            if (this._client != null && validationCheck() == false)
            {
                MessageBox.Show("Please fill required fields");
                return;
            }
            SaveClientInfo(this._client);

            ClientUserControl obj = new ClientUserControl(_parent);
            _parent.stkTest.Children.Clear();
            _parent.stkTest.Children.Add(obj);
            //btnShowClients.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
            //btnShowTests.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));

        }

        private void btn_Tests(object sender, RoutedEventArgs e)
        {
            if (this._client != null && validationCheck() == false)
            {
                MessageBox.Show("Please fill required fields");
                return;
            }
            SaveClientInfo(this._client);
            allClientsButtonselected = false;
            allClientsButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F8FF"));

            TestsUserControl obj = new TestsUserControl(_parent);
            _parent.stkTest.Children.Clear();
            _parent.stkTest.Children.Add(obj);
            //btnShowTests.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
            //btnShowClients.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
        }



        private void groupDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (groupDataGrid.SelectedItem != null)
                {
                    if (groupDataGrid.SelectedItem is Models.Group)
                    {
                        var row = (Models.Group)groupDataGrid.SelectedItem;

                        if (row != null)
                        {

                            var group = this._group;
                            group.name = (e.EditingElement as TextBox).Text;


                            var repo = new GroupRepository();
                            repo.Update(group);

                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            //MessageBox.Show((e.EditingElement as TextBox).Text.ToString());

            groupDataGrid.IsReadOnly = true;
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Keyboard.ClearFocus();
            groupDataGrid.IsReadOnly = true;

        }

        private void MenuItem_Delete(object sender, RoutedEventArgs e)
        {
              //Finding the item what we are going to delete.
                var item = (MenuItem)sender;
            var contextMenu = (ContextMenu)item.Parent;
            var item2 = (DataGrid)contextMenu.PlacementTarget;
            var deleteobj = (Models.Group)item2.SelectedCells[0].Item;

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
              

            var repo = new GroupRepository();
            repo.Delete(deleteobj);

            // Updating Group menu
            var repoGroup = new GroupRepository();
            IList<Models.Group> groups = repoGroup.FindAll();

     
            groupDataGrid.ItemsSource = groups;
           

            // Making clients grids empty
            clientDataGrid.ItemsSource = null;
            clientDetailDatagrid.DataContext = null;

            findClientTotal();
            }
        }


        private void MenuItem_Rename(object sender, RoutedEventArgs e)
        {

            //Finding the item what we are going to rename.
            var item = (MenuItem)sender;
            var contextMenu = (ContextMenu)item.Parent;
            var item2 = (DataGrid)contextMenu.PlacementTarget;
            var renameobj = (Models.Group)item2.SelectedCells[0].Item;

           
            // Adding focus on the rename obj row

            groupDataGrid.Focus();
          
            groupDataGrid.SelectedItem = renameobj;          
            groupDataGrid.IsReadOnly = false;
            groupDataGrid.BeginEdit();
           


        }
        

        private void filterSearchBox(object sender, TextChangedEventArgs e)
        {
            if (this._client != null && validationCheck() == false)
            {
                if (messageShown == false)
                {
                    MessageBox.Show("Please fill required fields");
                    messageShown = true;
                }
                else
                {
                    messageShown = false;
                }
                search.Text = "";
                return;
            }
            string searchBy = search.Text;

            var repo = new ClientRepository();
            IList<Client> clients = repo.FindSearchResult(searchBy, this._group);


            clientDataGrid.ItemsSource = clients;
            clientDataGrid.SelectedIndex = 0;
            
            
          }

        private void filterSearchBoxAllClients(object sender, TextChangedEventArgs e)
        {
            if (this._client != null && validationCheck() == false)
            {
                if (messageShown == false)
                {
                    MessageBox.Show("Please fill required fields");
                    messageShown = true;
                }
                else
                {
                    messageShown = false;
                }
                searchAllClients.Text = "";
                return;
            }
            string searchBy = searchAllClients.Text;

            var repo = new ClientRepository();
            IList<Client> clients = repo.FindSearchResultAllClients(searchBy);


            clientDataGrid.ItemsSource = clients;
            clientDataGrid.SelectedIndex = 0;


        }

        private void search_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this._client != null && validationCheck() == false)
            {
               
                return;
            }
            SaveClientInfo(this._client);
           
        }

        private void btn_ShowAllClients(object sender, RoutedEventArgs e)
        {

            if (this._client != null && validationCheck() == false)
            {
                MessageBox.Show("Please fill required fields");
                return;
            }
            SaveClientInfo(this._client);
            var repo = new ClientRepository();
            groupDataGrid.SelectedItem = null;


            allClientsButtonselected = true;

        IList<Client> clients = repo.FindAll();
               
            clientDataGrid.ItemsSource = clients;
            clientDataGrid.SelectedIndex = 0;

            allClientsButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ADD8E6"));


        }
        }
    }


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
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace HealthReporter.Controls
{
    /// <summary>
    /// Interaction logic for EditClientControl.xaml
    /// </summary>
    public partial class UpdateClientControl : UserControl, INotifyPropertyChanged
    {
     
        private MainWindow _parent;
        private Client client;

      

        public UpdateClientControl(MainWindow _parent, Client client) : this(_parent)
        {
            InitializeComponent();
            grid.DataContext = client;
            //gender.SelectedItem = int.Parse(client.gender); 
            // lastName.DataContext = new Client { lastName = " " };
            this._parent = _parent;
            this.client = client;
        }

        public UpdateClientControl(MainWindow _parent)
        {
            
            this._parent = _parent;
        }

        private void btn_Back(object sender, RoutedEventArgs e)
        {
            this._parent.stkTest.Children.Clear();
            ClientUserControl obj = new ClientUserControl(this._parent);
            this._parent.stkTest.Children.Add(obj);
        }

        private void btn_Update(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.firstName.Text) || String.IsNullOrWhiteSpace(this.lastName.Text) ||
                String.IsNullOrWhiteSpace(this.gender.Text) || String.IsNullOrWhiteSpace(this.birthDate.Text)||
               (!String.IsNullOrWhiteSpace(this.email.Text) && !Regex.IsMatch(this.email.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")))
            {
                MessageBox.Show("You forgot to enter required fields or there is something wrong with your input.", "Message");
            }
            else
            {
                try
                {
                    //TODO: add validation also.            
                    var client = this.client;
                    client.firstName = this.firstName.Text;

                    client.lastName = this.lastName.Text;
                    client.groupName = this.group.Text;
                    client.email = this.email.Text;
                    client.gender = ((ComboBoxItem)gender.SelectedValue).Tag.ToString();
                    DateTime enteredDate = Convert.ToDateTime(birthDate.SelectedDate.ToString());


                    client.birthDate = String.Format("{0:yyyy-MM-dd}", enteredDate);
                    var repo = new ClientRepository();
                    repo.Update(client);

                    this._parent.stkTest.Children.Clear();
                    ClientUserControl obj = new ClientUserControl(this._parent);
                    this._parent.stkTest.Children.Add(obj);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message");
                }


            }
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}

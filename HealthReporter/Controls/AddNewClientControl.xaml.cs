using HealthReporter.Models;
using HealthReporter.Utilities;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class AddNewClientControl : UserControl, INotifyPropertyChanged
    {
        private MainWindow _parent;
       

        public AddNewClientControl(MainWindow parent)
        {
            InitializeComponent();
            
            //firstName.DataContext = new Client { firstName = " " };
            // lastName.DataContext = new Client { lastName = " " };
            this._parent = parent;
            

           
        }


        private void btn_Back(object sender, RoutedEventArgs e)
        {
            this._parent.stkTest.Children.Clear();
            ClientUserControl obj = new ClientUserControl(this._parent);
            this._parent.stkTest.Children.Add(obj);
        }

        private void btn_CreateNewClient(object sender, RoutedEventArgs e)
        {
            this.grid.DataContext = new Client
            {
                firstName = this.firstName.Text.ToString(),
                lastName = this.lastName.Text.ToString(),
                groupName = this.group.Text.ToString(),
                email = this.email.Text.ToString(),
                gender=  this.gender.Text.ToString(),
                birthDate =this.birthDate.Text.ToString()

            };



            if (String.IsNullOrWhiteSpace(this.firstName.Text) || String.IsNullOrWhiteSpace(this.lastName.Text) ||
                String.IsNullOrWhiteSpace(this.gender.Text) || String.IsNullOrWhiteSpace(this.birthDate.Text)||
                (!String.IsNullOrWhiteSpace(this.email.Text)&&!Regex.IsMatch(this.email.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")))
            {
                
            }
            else
            {
                //TODO: add validation also.      
                try
                {


                    DateTime enteredDate = Convert.ToDateTime(birthDate.SelectedDate.ToString());


                    // DateTime newDate= new DateTime(enteredDate.Year, enteredDate.Month, enteredDate.Day, enteredDate.Hour, enteredDate.Minute, enteredDate.Second);




                    var client = new Client()
                    {
                        firstName = this.firstName.Text,
                        lastName = this.lastName.Text,
                        groupName = this.group.Text,
                        email = this.email.Text,
                        gender = this.gender.SelectedValue.ToString(),
                        birthDate = String.Format("{0:yyyy-MM-dd}", enteredDate)
                    };


                    if (client.IsValid)
                    {

                        var repo = new ClientRepository();
                        repo.Insert(client);
                        
                        this._parent.stkTest.Children.Clear();
                        ClientUserControl obj = new ClientUserControl(this._parent);
                        this._parent.stkTest.Children.Add(obj);
                    }

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

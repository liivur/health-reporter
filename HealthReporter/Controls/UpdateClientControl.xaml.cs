using HealthReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows;
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
    /// Interaction logic for EditClientControl.xaml
    /// </summary>
    public partial class UpdateClientControl : UserControl
    {
     
        private MainWindow _parent;
        
        public UpdateClientControl(MainWindow _parent) 
        {
            InitializeComponent();
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
            try
            {
                //TODO: add validation also.            
                var client = new Client() { firstName = this.firstName.Text, lastName = this.lastName.Text, groupId = int.Parse(this.groupId.Text), email = this.email.Text, gender = this.gender.SelectedValue.ToString() };

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
}

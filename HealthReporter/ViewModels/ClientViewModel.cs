namespace HealthReporter.ViewModels
{

    using HealthReporter.Models;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Input;
    


    //ViewModel goal is to perform some actions, for example edit profile of customer, display readonly component.
    internal class ClientViewModel : INotifyPropertyChanged
    {

        //private Client _Client;

        public ClientViewModel()
        {
            //Client = new Client("Kristiina", "Kõiv");
        }
        /// <summary>
        /// Gets the client instance
        /// </summary>
        public Client Client
        {
            get;
            set;
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

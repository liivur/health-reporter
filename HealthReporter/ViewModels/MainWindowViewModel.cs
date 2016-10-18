using HealthReporter.Controls;
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
        public MainWindow()
        {
            InitializeComponent();
            DatabaseUtility.checkDb();
            ClientUserControl obj = new ClientUserControl(this);
            stkTest.Children.Add(obj);
            btnShowClients.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
        } 

        private void btn_Clients(object sender, RoutedEventArgs e)
        {
            ClientUserControl obj = new ClientUserControl(this);
            stkTest.Children.Clear();
            stkTest.Children.Add(obj);
            btnShowClients.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
            btnShowTests.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
        }

        private void btn_Tests(object sender, RoutedEventArgs e)
        {
            TestsUserControl obj = new TestsUserControl(this);
            stkTest.Children.Clear();
            stkTest.Children.Add(obj);
            btnShowTests.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
            btnShowClients.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
        }
    }
}

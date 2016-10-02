using HealthReporter.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace HealthReporter.Controls
{
    /// <summary>
    /// Interaction logic for AddNewTestControl.xaml
    /// </summary>
    public partial class AddNewTestControl : UserControl
    {
        public MainWindow _parent;

        public AddNewTestControl(MainWindow parent)
        {
            InitializeComponent();
            this._parent = parent;

            var repo = new TestCategoryRepository();

            //var test = new TestCategory() { name = "temp2", position = 2 };
            //repo.Insert(test);


            IList<TestCategory> tests = repo.FindAll();

            categorySelector.ItemsSource = tests;
        }

        private void btn_Back(object sender, RoutedEventArgs e)
        {
            this._parent.stkTest.Children.Clear();
            TestsUserControl obj = new TestsUserControl(this._parent);
            this._parent.stkTest.Children.Add(obj);
        }

        private void btn_CreateNewTest(object sender, RoutedEventArgs e)
        {
            //TODO: add validation also.
            var test = new Test() { name = this.name.Text, description = this.description.Text, categoryId = ((TestCategory)categorySelector.SelectedItem).id };

            var repo = new TestRepository();
            repo.Insert(test);

            this._parent.stkTest.Children.Clear();
            TestsUserControl obj = new TestsUserControl(this._parent);
            this._parent.stkTest.Children.Add(obj);

        }
    }
}

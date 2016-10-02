using System;
using System.Collections.Generic;
using System.Collections;
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
using HealthReporter.Models;
using HealthReporter.Utilities;
using Insight.Database;
using System.Diagnostics;
using System.Data.SQLite;

namespace HealthReporter.Controls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class TestsUserControl : UserControl
    {
        private MainWindow _parent;

        public TestsUserControl(MainWindow parent)
        {
            InitializeComponent();
            this._parent = parent;

            DatabaseUtility.checkDb();
            IList<TestCategory> categories = DatabaseUtility.getConnection().QuerySql<TestCategory>(
                "SELECT id, name FROM test_categories WHERE parentId IS NULL"); //NULL == main category


            var repo = new TestRepository();
            IList < Test > tests = repo.FindAll();
            noOfTests.Text = tests.Count.ToString();
            catsDataGrid.ItemsSource = categories;
        }


        private void btn_AddNewTest(object sender, RoutedEventArgs e)
        {
            AddNewTestControl obj = new AddNewTestControl(this._parent);
            this._parent.stkTest.Children.Clear();
            this._parent.stkTest.Children.Add(obj);

        }
        private void catsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //is called when a catecory is selected
        {
            var grid = sender as DataGrid;
            var selected = grid.SelectedItems;

            if (selected.Count > 0)
            {
                TestCategory category = (TestCategory)selected[0];

                updateTestsColumn(category);
            }
        }

        private void testsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //is called when a test is selected
        {
            var grid = sender as DataGrid;
            var selected = grid.SelectedItems;

            if (selected.Count > 0)
            {
                Test test = (Test)selected[0];
                updateTest(test);
            }

        }

        private void updateTestsColumn(TestCategory cat)
        {
            DatabaseUtility.checkDb();
            var connection = DatabaseUtility.getConnection();

            //get subcategories
            IList<Test> subCats = connection.QuerySql<Test>("SELECT id, name FROM test_categories WHERE parentId = @id", cat);

            //get tests of subcategories
            List<Test> cat_tests = new List<Test>();
            foreach (var category in subCats)
            {
                IList<Test> tests = connection.QuerySql<Test>("SELECT id, name FROM tests WHERE categoryId = @id", category);
                cat_tests.AddRange(tests);
            }

            //if subCats is empty then the main category is a subcategory
            if (cat_tests.Count == 0)
            {
                cat_tests = (List<Test>)connection.QuerySql<Test>("SELECT id, name FROM tests WHERE categoryId = @id", cat);
            }

            testsDataGrid.ItemsSource = cat_tests;
        }

        private void updateTest(Test test)
        {
            DatabaseUtility.checkDb();
            var connection = DatabaseUtility.getConnection();
            IList<Test> tests = connection.QuerySql<Test>("SELECT * FROM tests WHERE id = @id", test);

            Test t = tests[0];

            testName.Text = t.name;
            units.Text = t.units;
            decimals.Text = t.decimals.ToString();
        }
    }
}

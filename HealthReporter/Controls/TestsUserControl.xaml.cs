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
            IList <Test> tests = repo.FindAll();
            noOfTests.Text = tests.Count.ToString();

            catsDataGrid.ItemsSource = categories;

            decimalsSelector.ItemsSource = new List<int> {-2,-1,0,1,2};
        }


        private void btn_AddNewTest(object sender, RoutedEventArgs e)
        {
            AddNewTestControl obj = new AddNewTestControl(this._parent);
            this._parent.stkTest.Children.Clear();
            this._parent.stkTest.Children.Add(obj);
        }

        private void btn_MenRatings(object sender, RoutedEventArgs e)
        {
        }

        private void btn_WomenRatings(object sender, RoutedEventArgs e)
        {
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


        private void updateTestsColumn(TestCategory cat) //cat is a main category
        {
            var rep = new TestCategoryRepository();

            //get subcategories
            IList<TestCategory> subCats = rep.GetCategoryByParent(cat);

            //get tests of subcategories
            var repo = new TestRepository();

            List<Test> cat_tests = new List<Test>();
            foreach (var category in subCats)
            {
                IList<Test> tests = repo.GetTestsByCategory(category);
                cat_tests.AddRange(tests);
            }

            //if subCats is empty then check if there are any tests where test.parentId == cat.id, for example Questionnaires doesn't have sub categories
            if (subCats.Count == 0)
            {
                cat_tests = (List<Test>)repo.GetTestsByCategory(cat);
            }
            testsDataGrid.ItemsSource = cat_tests;
        }

        private void updateTest(Test test)
        {

            testName.Text = test.name;
            units.Text = test.units;
            decimalsSelector.SelectedItem = test.decimals;

            var repo = new RatingRepository();
            IList<Rating> testRatings = repo.getTestRatings(test);

            List<int> ages = new List<int>();
            foreach (var rating in testRatings)
            {
                if (!ages.Contains(rating.age)) ages.Add(rating.age);
            }
            ages.Sort();

            List<string> ageIntervals = getAgeIntervals(ages);
            
            agesControl.ItemsSource = ageIntervals;

            //TODO: show ratings, rating words, desc, connect buttons etc


        }

        private List<string> getAgeIntervals(IList<int> ages)
        {
            List<string> intervals = new List<string>();
            for (int i=0; i < ages.Count; i++){
                string str = "";
                if (i == ages.Count - 1) str = ages[i] + "+";
               
                else str = ages[i] + "-" + (ages[i + 1]-1);
                
                intervals.Add(str);
            }
            return intervals;
        }
    }
}

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
            IList<Test> tests = repo.FindAll();
            noOfTests.Text = tests.Count.ToString();

            catsDataGrid.ItemsSource = categories;

            decimalsSelector.ItemsSource = new List<int> { -2, -1, 0, 1, 2 };
        }


        private void btn_AddNewTest(object sender, RoutedEventArgs e)
        {
            AddNewTestControl obj = new AddNewTestControl(this._parent);
            this._parent.stkTest.Children.Clear();
            this._parent.stkTest.Children.Add(obj);
        }

        private void btn_AddNewCategory(object sender, RoutedEventArgs e)
        {
            TestCategory category = (TestCategory)catsDataGrid.SelectedItem;
            byte[] parentId = null;
            if (category != null)
            {
                parentId = category.id;
            }
            AddNewTestCategoryControl obj = new AddNewTestCategoryControl(this._parent, parentId);
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

        //updates test fields, adds ages buttons
        private void updateTest(Test test)
        {
            testName.Text = test.name;
            units.Text = test.units;
            decimalsSelector.SelectedItem = test.decimals;
            TestDescriptionText.Text = test.description;
            menFormulaRadio.CommandParameter = test;
            womenFormulaRadio.CommandParameter = test;

            var repo = new RatingRepository();

            IList<Rating> ages = repo.getAges(test);
            agesControl.ItemsSource = ages;
        }

        //updates ratings, ratings word and description columns when an age is clicked
        private void btn_AgeButtons(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Rating rating = (Rating)button.CommandParameter;

            //get other ratings of the same age
            var repo = new RatingRepository();
            IList<Rating> sameAgeRatings = repo.getSameAgeRatings(rating);

            //get rating labels (name, interpretation)
            var rep = new RatingLabelRepository();
            List<RatingLabel> labels = new List<RatingLabel>();

            foreach (var rat in sameAgeRatings)
            {
                labels.AddRange(rep.getLabel(rat));
            }

            //bind score column to men/women scores
            if ((bool)menRadio.IsChecked)
            {
                RatingColumn.Binding = new Binding("normM");
            }
            else if ((bool)womenRadio.IsChecked)
            {
                RatingColumn.Binding = new Binding("normF");
            }
            ratingsDatagrid.ItemsSource = sameAgeRatings;
            LabelandDescDatagrid.ItemsSource = labels;
        }

        private void FormulaRadio_Checked(object sender, RoutedEventArgs e) //is called when a radio button to show man/woman formula is checked
        {
            var button = sender as RadioButton;

            if (button.CommandParameter == null) return;
            Trace.Write("yo");
            var gender = button.Content.ToString();

            if (gender == "Men")
            {
                FormulaText.Text = ((Test)button.CommandParameter).formulaM;
            }
            else if (gender == "Women")
            {
                FormulaText.Text = ((Test)button.CommandParameter).formulaF;
            }
        }

        private void menRadio_Copy1_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

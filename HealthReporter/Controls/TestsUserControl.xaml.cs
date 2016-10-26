using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HealthReporter.Models;
using System.Diagnostics;
using System.Windows.Media;
using System.Linq;

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

            var catRep = new TestCategoryRepository();
            IList<TestCategory> categories = catRep.FindRootCategories();
            catsDataGrid.ItemsSource = categories;

            var repo = new TestRepository();
            IList<Test> tests = repo.FindAll();
            noOfTests.Text = tests.Count.ToString();

            decimalsSelector.ItemsSource = new List<int> { -2, -1, 0, 1, 2 };

            categorySelector.ItemsSource = catRep.FindAll();
            DisableFields();

            btnShowTests.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
        }


        private void btn_AddNewTest(object sender, RoutedEventArgs e)
        {
            Test newTest = new Test() { };
            newTest.id = System.Guid.NewGuid().ToByteArray();

            //bind new test to buttons
            SaveTest.CommandParameter = newTest;
            FormulaText.DataContext = newTest;
            testName.DataContext = newTest;
            units.DataContext = newTest;
            TestDescriptionText.DataContext = newTest;
            addNewRating.CommandParameter = newTest;
            Cancel.CommandParameter = newTest;

            var obsAges = new System.Collections.ObjectModel.ObservableCollection<AgeInterval>();
            obsAges.Add(new AgeInterval() { interval = "0-" });
            agesControl.ItemsSource = obsAges;

            testsDataGrid.SelectedIndex = -1;
            testsDataGrid.IsHitTestVisible = false;
            catsDataGrid.IsHitTestVisible = false;
            ClearFields();
            EnableFields();
            updateButton.Visibility = Visibility.Hidden;
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

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Test test = (Test)button.CommandParameter;

            var rep = new RatingLabelRepository();
            rep.Delete(test);

            var repo = new RatingRepository();
            repo.removeRatingsByTest(test);

            ClearRatingDatagrid();
            ClearFields();
            DisableFields();
            agesControl.ItemsSource = null;
            catsDataGrid.SelectedIndex = -1;
            testsDataGrid.SelectedIndex = -1;
            testsDataGrid.IsHitTestVisible = true;
            catsDataGrid.IsHitTestVisible = true;
        }

        private void catsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //is called when a catecory is selected
        {
            updateButton.Visibility = Visibility.Hidden;
            DisableFields();
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
            updateButton.Visibility = Visibility.Hidden;
            DisableFields();
            var grid = sender as DataGrid;
            var selected = grid.SelectedItems;

            if (selected.Count > 0)
            {
                LastTest ltest = (LastTest)selected[0];
                Test test = ltest.test;
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
                IList<Test> t = repo.GetTestsByCategory(category);
                cat_tests.AddRange(t);
            }
            //get tests of main category (if there are any)
            IList<Test> tsts = repo.GetTestsByCategory(cat);
            cat_tests.AddRange(tsts);

            IList<LastTest> tests = new List<LastTest>();
            for (int i = 0; i < cat_tests.Count; i++)
            {
                if (i + 1 < cat_tests.Count)
                {
                    if (!cat_tests[i].categoryId.SequenceEqual(cat_tests[i + 1].categoryId))
                    {
                        tests.Add(new LastTest() { isLast = true, test = cat_tests[i] });
                    }
                    else tests.Add(new LastTest() { isLast = false, test = cat_tests[i] });
                }
                else tests.Add(new LastTest() { isLast = false, test = cat_tests[i] });
            }

            testsDataGrid.ItemsSource = tests;
        }

        //updates test fields, adds ages buttons
        private void updateTest(Test test)
        {
            FormulaText.DataContext = test;
            testName.DataContext = test;
            units.DataContext = test;
            TestDescriptionText.DataContext = test;
            decimalsSelector.SelectedItem = test.decimals;

            IList<AgeInterval> ageIntervals = getAgeIntervals(test);
            agesControl.ItemsSource = ageIntervals;
            select_first();

            //show subcategory
            var catRep = new TestCategoryRepository();
            foreach (TestCategory cat in categorySelector.Items)
            {
                if (cat.id.SequenceEqual(catRep.GetCategoryByTest(test)[0].id))
                {
                    categorySelector.SelectedItem = cat;
                    break;
                }
            }
        }

        private IList<AgeInterval> getAgeIntervals(Test test) //gets ages connected with the test FROM the db, ageInterval{interval; rating}
        {
            var repo = new RatingRepository();
            IList<Rating> ages = repo.getAges(test);
            IList<AgeInterval> intervals = new List<AgeInterval>();

            for (int i = 0; i < ages.Count; i++)
            {
                Rating rating = (Rating)ages[i];
                int age = rating.age;
                if (i + 1 < ages.Count)
                {
                    int nextAge = ((Rating)ages[i + 1]).age;
                    intervals.Add(new AgeInterval() { interval = age + "-" + (nextAge - 1), rating = rating });
                }
                else intervals.Add(new AgeInterval() { interval = age + "+", rating = rating });
            }
            return intervals;
        }

        //updates ratings, ratings word and description columns when an age is clicked
        private void AgeRadio_checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            AgeInterval interval = (AgeInterval)button.CommandParameter;
            Rating rating = interval.rating;

            //clear columns if no rating is connected with the button
            if (rating == null)
            {
                ClearRatingDatagrid();
                return;
            }
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

            List<RatingDatagridItem> items = new List<RatingDatagridItem>();
            for (int i = 0; i < sameAgeRatings.Count; i++)
            {
                if (labels.Count > i) items.Add(new RatingDatagridItem() { rating = sameAgeRatings[i], ratingLabel = labels[i] });
                else items.Add(new RatingDatagridItem() { rating = sameAgeRatings[i] });
            }
            ratingsDatagrid.ItemsSource = items;
        }

        private void GenderRadio_Checked(object sender, RoutedEventArgs e) //is called when a radio button to show man/woman ratings is checked
        {
            if (RatingColumn != null)
            {
                if ((bool)menRadio.IsChecked)
                {
                    RatingColumn.Binding = new Binding("rating.normM");
                }
                else if ((bool)womenRadio.IsChecked)
                {
                    RatingColumn.Binding = new Binding("rating.normF");
                }
            }
        }

        private void FormulaRadio_Checked(object sender, RoutedEventArgs e) //is called when a radio button to show man/woman formula is checked
        {
            if (FormulaText == null) return;
            if ((bool)menFormulaRadio.IsChecked)
            {
                FormulaText.SetBinding(TextBox.TextProperty, new Binding("formulaM"));
            }
            else if ((bool)womenFormulaRadio.IsChecked)
            {
                FormulaText.SetBinding(TextBox.TextProperty, new Binding("formulaF"));
            }
        }

        void chkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (testName.IsReadOnly == true) return;
            var button = sender as RadioButton;
            Rating rating = ((AgeInterval)button.CommandParameter).rating;
            string ageStr = ((AgeInterval)button.CommandParameter).interval;

            if (rating == null)
            {
                int age = parse_age(ageStr, true, 0);

                rating = new Rating() { age = age };
                Rating rat = SaveRating(rating.age);
                int j = -1;
                for (int i = 0; i < agesControl.Items.Count; i++)
                {
                    RadioButton rb = get_button(i);
                    if (rb == button)
                    {
                        j = i;
                        ((AgeInterval)rb.CommandParameter).rating = rat;
                    }
                }
            }
            else SaveRating(rating.age);
        }

        private void btn_AddNewAge(object sender, RoutedEventArgs e) //adds new radiobutton to ages radiobuttons
        {
            Test newTest = (Test)testName.DataContext;
            string ageStr = ""; //current age
            string lastAgeStr = "";
            var obsAges = new System.Collections.ObjectModel.ObservableCollection<AgeInterval>();
            int j = -1;
            for (int i = 0; i < agesControl.Items.Count; i++)
            {
                RadioButton rb = get_button(i);
                AgeInterval param = (AgeInterval)rb.CommandParameter;

                if ((bool)rb.IsChecked)
                {
                    j = i;
                    ageStr = param.interval;
                }
                else obsAges.Add(param);

                if (i == agesControl.Items.Count - 2) lastAgeStr = param.interval;
            }
            //check if age is in correct format
            int age = parse_age(ageStr, true, 0);
            int nextAge = parse_age(ageStr, false, 1);
            int lastAge = -1;
            if (lastAgeStr != "") Int32.TryParse(lastAgeStr.Split('-')[1], out lastAge);

            if(age == -1 || nextAge == -1)
            {
                MessageBox.Show("Please insert age.", "Confirmation");
                return;
            }
    
            if (nextAge > -1 && nextAge <= age || (lastAge != -1 && lastAge + 1 != age))
            {
                obsAges.Insert(j, (new AgeInterval() { interval = ageStr }));
                EnableLastRadioWriting();
                return;
            }
            Rating rating = SaveRating(age);
            if (rating != null) obsAges.Insert(j, (new AgeInterval() { interval = ageStr, rating = rating }));
            else obsAges.Insert(j, (new AgeInterval() { interval = ageStr }));

            if (nextAge != -1) obsAges.Add(new AgeInterval() { interval = nextAge + 1 + "+" });
            agesControl.ItemsSource = obsAges;
            EnableLastRadioWriting();
        }

        private Rating SaveRating(int age)
        {
            Test newTest = (Test)testName.DataContext;

            //get ratings and ratinglabels connected with that age from the datagrids
            IList items = ratingsDatagrid.Items;

            //add age, testId, labelId to ratings and id to labels
            IList<Rating> ratingList = new List<Rating>();
            IList<RatingLabel> ratingLabelList = new List<RatingLabel>();
            for (int i = 0; i < items.Count; i++)
            {
                byte[] labelId = System.Guid.NewGuid().ToByteArray();

                if (items[i] is RatingDatagridItem)
                {
                    RatingDatagridItem item = (RatingDatagridItem)items[i];
                    item.rating.age = age;
                    item.rating.testId = newTest.id;
                    item.ratingLabel.id = labelId;
                    item.rating.labelId = labelId;
                    if(!((item.rating.normF == 0 || item.rating.normM == 0) && i > 0 && item.ratingLabel.name == "" && item.ratingLabel.interpretation == ""))
                    {
                        ratingLabelList.Add(item.ratingLabel);
                        ratingList.Add(item.rating);
                    }
                }
            }
            //checks if there are any ratings with given testId and age and removes them from db
            if (ratingList.Count != 0) removeOldRatings(newTest, ((Rating)ratingList[0]).age);

            //add data to db
            if (ratingList.Count == 0) ratingList.Add(new Rating() { age = age, testId = newTest.id });
            
            var repo = new RatingRepository();
            foreach (Rating rati in ratingList) repo.Insert(rati);

            var rep = new RatingLabelRepository();
            foreach (RatingLabel lab in ratingLabelList) rep.Insert(lab);

            ClearRatingDatagrid();
            return ratingList[0];
        }

        private void btn_DeleteRating(object sender, RoutedEventArgs e) //deletes last rating
        {
            var obsAges = new System.Collections.ObjectModel.ObservableCollection<AgeInterval>();
            string lastAgeStr = "";
            string ageStr = "";
            Rating rating = null;
            for (int i = 0; i < agesControl.Items.Count; i++)
            {
                RadioButton rb = get_button(i);
                AgeInterval param = (AgeInterval)rb.CommandParameter;

                if (agesControl.Items.Count == 1)
                {
                    obsAges.Add(param);
                    agesControl.ItemsSource = obsAges;
                    EnableLastRadioWriting();
                    removeOldRatings((Test)testName.DataContext, 0);
                    ClearRatingDatagrid();
                    return;
                }
                if (i == agesControl.Items.Count - 2)
                {
                    lastAgeStr = param.interval;
                    rating = param.rating;
                }

                else if (i == agesControl.Items.Count - 1) ageStr = param.interval;
                else obsAges.Add(param); 
            }
            int lastAge = parse_age(lastAgeStr, false, 0);
            obsAges.Add(new AgeInterval() { interval = lastAge + "+", rating = rating });
            agesControl.ItemsSource = obsAges;
            EnableLastRadioWriting();

            int age = parse_age(ageStr, true, 0);
            var rep = new RatingRepository();
            if (age != -1) rep.removeRatingsByAge((Test)testName.DataContext, age);
        }

        private int parse_age(string ageStr, bool last, int i)
        {
            int age = -1;
            if (i < ageStr.Split('-').Length)
            {
                if (!Int32.TryParse(ageStr.Split('-')[i], out age))
                {
                    if (last) Int32.TryParse(ageStr.Split('+')[i], out age);
                }
            }
            return age;
        }

        //save new test into db
        private void btn_SaveTest(object sender, RoutedEventArgs e)
        {
            Test test = (Test)testName.DataContext;

            if (testName.Text == "")
            {
                MessageBox.Show("Test name cannot be empty", "Confirmation");
                return;
            }
            select_first();
            deselect_first();

            test.categoryId = ((TestCategory)this.categorySelector.SelectedItem).id;
            test.decimals = (int)this.decimalsSelector.SelectedItem;

            var repo = new TestRepository();
            repo.Insert(test);

            MessageBox.Show("Saved", "Confirmation");

            IList<Test> tests = repo.FindAll();
            noOfTests.Text = tests.Count.ToString();

            select_first();
            int i = catsDataGrid.SelectedIndex;
            catsDataGrid.SelectedIndex = -1;
            catsDataGrid.SelectedIndex = i;

            ClearFields();
            ClearRatingDatagrid();
            DisableFields();
            testsDataGrid.SelectedIndex = -1;
            testsDataGrid.IsHitTestVisible = true;
            catsDataGrid.IsHitTestVisible = true;
            agesControl.ItemsSource = null;
        }

        private void removeOldRatings(Test test, int age)
        {
            var repo = new RatingRepository();
            var rep = new RatingLabelRepository();
            IList<Rating> ages = repo.getAges(test);
            foreach (Rating a in ages)
            {
                //removes ratings with given testId and age & removes ratingslabels
                if (a.age == age)
                {
                    rep.DeleteByAge(test, age);
                    repo.removeRatingsByAge(test, age);
                    break;
                }
            }
        }

        private void ClearRatingDatagrid()
        {
            var ratings = new System.Collections.ObjectModel.ObservableCollection<RatingDatagridItem>();
            ratingsDatagrid.ItemsSource = ratings;
            ratingsDatagrid.CanUserAddRows = true;
            ratingsDatagrid.IsReadOnly = false;
        }

        private void EnableLastRadioWriting()
        {
            RadioButton rb = get_button(agesControl.Items.Count - 1);
            rb.IsChecked = true;
            TextBox tb = rb.Content as TextBox;
            tb.IsReadOnly = false;
        }

        private void btn_DeleteTest(object sender, RoutedEventArgs e)
        {
            Test test = (Test)testName.DataContext;
            var testRepo = new TestRepository();
            testRepo.Delete(test);

            var labRepo = new RatingLabelRepository();
            labRepo.Delete(test);

            var ratRepo = new RatingRepository();
            ratRepo.Delete(test);

            int i = catsDataGrid.SelectedIndex;
            catsDataGrid.SelectedIndex = -1;
            catsDataGrid.SelectedIndex = i;

            ClearFields();
            DisableFields();
            agesControl.ItemsSource = null;

            IList<Test> tests = testRepo.FindAll();
            noOfTests.Text = tests.Count.ToString();
         
        }
        private void btn_UpdateTest(object sender, RoutedEventArgs e)
        {
            updateButton.Visibility = Visibility.Visible;
            EnableFields();
            EnableLastRadioWriting();
        }

        private void btn_Update(object sender, RoutedEventArgs e)
        {
            Test test = (Test)testName.DataContext;
            test.categoryId = ((TestCategory)this.categorySelector.SelectedItem).id;
            test.decimals = (int)this.decimalsSelector.SelectedItem;

            if (testName.Text == "")
            {
                MessageBox.Show("Test name cannot be empty", "Confirmation");
                return;
            }
            select_first();
            deselect_first();

            var repo = new TestRepository();
            repo.Update(test);

            var result = MessageBox.Show("Updated", "Confirmation");
            if (result == MessageBoxResult.OK) DisableFields();          
            select_first();

            addTestButton.IsEnabled = true;
            int i = catsDataGrid.SelectedIndex;
            catsDataGrid.SelectedIndex = -1;
            catsDataGrid.SelectedIndex = i;
        }

        private void select_first()
        {
            RadioButton rb = get_button(0);
            rb.IsChecked = true;           
        }

        private void deselect_first()
        {
            RadioButton rb = get_button(0);
            rb.IsChecked = false;           
        }

        private RadioButton get_button(int i)
        {
            RadioButton rb = null;
            if (agesControl.Items.Count > i)
            {
                ContentPresenter c = (ContentPresenter)agesControl.ItemContainerGenerator.ContainerFromItem(agesControl.Items[i]);
                c.ApplyTemplate();
                rb = c.ContentTemplate.FindName("AgeRadio", c) as RadioButton;
            }
            return rb;
        }

        private void ClearFields()
        {
            ClearRatingDatagrid();
            EnableLastRadioWriting();
            testName.Text = "";
            testName.IsReadOnly = false;
            units.Text = "";
            units.IsReadOnly = false;
            decimalsSelector.IsEnabled = true;
            categorySelector.IsEnabled = true;
            FormulaText.Text = "";
            FormulaText.IsReadOnly = false;
            TestDescriptionText.Text = "";
            TestDescriptionText.IsReadOnly = false;
        }

        private void EnableFields()
        {
            testName.IsReadOnly = false;
            units.IsReadOnly = false;
            decimalsSelector.IsEnabled = true;
            categorySelector.IsEnabled = true;
            FormulaText.IsReadOnly = false;
            TestDescriptionText.IsReadOnly = false;
            ratingsDatagrid.CanUserAddRows = true;
            ratingsDatagrid.IsReadOnly = false;
            addNewRating.Visibility = Visibility.Visible;
            deleteRating.Visibility = Visibility.Visible;
        }

        private void DisableFields()
        {
            testName.IsReadOnly = true;
            units.IsReadOnly = true;
            decimalsSelector.IsEnabled = false;
            categorySelector.IsEnabled = false;
            TestDescriptionText.IsReadOnly = true;
            FormulaText.IsReadOnly = true;
            ratingsDatagrid.CanUserAddRows = false;
            ratingsDatagrid.IsReadOnly = true;
            addNewRating.Visibility = Visibility.Hidden;
            deleteRating.Visibility = Visibility.Hidden;
            updateButton.Visibility = Visibility.Hidden;
        }

        class AgeInterval
        {
            public string interval { get; set; }
            public Rating rating { get; set; }
        }
        class LastTest
        {
            public bool isLast { get; set; }
            public Test test { get; set; }
        }
        class RatingDatagridItem
        {
            public RatingDatagridItem()
            {
                rating = new Rating();
                ratingLabel = new RatingLabel();
            }
            public Rating rating { get; set; }
            public RatingLabel ratingLabel { get; set; }
        }

        private void btn_Clients(object sender, RoutedEventArgs e)
        {
            ClientUserControl obj = new ClientUserControl(_parent);
            _parent.stkTest.Children.Clear();
            _parent.stkTest.Children.Add(obj);
            btnShowClients.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
            btnShowTests.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
        }

        private void btn_Tests(object sender, RoutedEventArgs e)
        {
            TestsUserControl obj = new TestsUserControl(_parent);
            _parent.stkTest.Children.Clear();
            _parent.stkTest.Children.Add(obj);
            btnShowTests.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
            btnShowClients.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
        }

        private void btn_OpenManagePresets(object sender, RoutedEventArgs e)
        {
            ManagePresetsControl obj = new ManagePresetsControl(this._parent);
            this._parent.stkTest.Children.Clear();
            this._parent.stkTest.Children.Add(obj);
        }
    }
}

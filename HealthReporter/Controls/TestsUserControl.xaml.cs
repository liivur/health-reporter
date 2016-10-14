using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HealthReporter.Models;
using System.Diagnostics;

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
            obsAges.Add(new AgeInterval() { interval = "??" });
            agesControl.ItemsSource = obsAges;

            ClearAndEnableFields();
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

            var repo = new RatingRepository();
            repo.removeRatingsByTest(test);

            ClearRatingAndLabel();
            DisableFields();
            agesControl.ItemsSource = null;
            catsDataGrid.SelectedIndex = -1;
            testsDataGrid.SelectedIndex = -1;
        }

        private void catsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //is called when a catecory is selected
        {
            if (SaveTest.Visibility == System.Windows.Visibility.Visible)
            {
                MessageBox.Show("Your changes will be lost. Please save new test or cancel adding.", "Confirmation");
                return;
            }
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
            if (SaveTest.Visibility == System.Windows.Visibility.Visible)
            {
                MessageBox.Show("Your changes will be lost. Please save new test or cancel adding.", "Confirmation");
                return;
            }
            DisableFields();
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
            //get tests of main category
            IList<Test> tsts = repo.GetTestsByCategory(cat);
            cat_tests.AddRange(tsts);

            testsDataGrid.ItemsSource = cat_tests;
        }

        //updates test fields, adds ages buttons
        private void updateTest(Test test)
        {
            FormulaText.DataContext = test;
            testName.DataContext = test;
            units.DataContext = test;
            TestDescriptionText.DataContext = test;

            decimalsSelector.SelectedItem = test.decimals;
            addNewRating.CommandParameter = test;

            IList<AgeInterval> ageIntervals = getAgeIntervals(test);
            agesControl.ItemsSource = ageIntervals;

            //select first
            ContentPresenter c = (ContentPresenter)agesControl.ItemContainerGenerator.ContainerFromItem(agesControl.Items[0]);
            c.ApplyTemplate();
            RadioButton rb = c.ContentTemplate.FindName("AgeRadio", c) as RadioButton;
            rb.IsChecked = true;

            //show subcategory
            var catRep = new TestCategoryRepository();
            foreach (TestCategory cat in categorySelector.Items)
            {
                if (cat.IdAsString() == (catRep.GetCategoryByTest(test)[0]).IdAsString())
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
                else intervals.Add(new AgeInterval() { interval = age + "-", rating = rating });
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
                ClearRatingAndLabel();
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
            ratingsDatagrid.ItemsSource = sameAgeRatings;
            LabelandDescDatagrid.ItemsSource = labels;
        }

        private void GenderRadio_Checked(object sender, RoutedEventArgs e) //is called when a radio button to show man/woman ratings is checked
        {
            if (RatingColumn != null)
            {
                if ((bool)menRadio.IsChecked)
                {
                    RatingColumn.Binding = new Binding("normM");
                }
                else if ((bool)womenRadio.IsChecked)
                {
                    RatingColumn.Binding = new Binding("normF");
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

        private void btn_AddNewRating(object sender, RoutedEventArgs e) //adds new radiobutton to ages radiobuttons
        {
            var button = sender as Button;
            Test newTest = (Test)button.CommandParameter;

            //saves last ratingtable
            SaveLastRating(newTest);

            //gets all ageintervals as observable
            IList<AgeInterval> ageIntervals = getAgeIntervals(newTest);
            var obsAges = new System.Collections.ObjectModel.ObservableCollection<AgeInterval>(ageIntervals);

            //adds new age
            AgeInterval rat = new AgeInterval() { interval = "??" };
            obsAges.Add(rat);
            agesControl.ItemsSource = obsAges;
            EnableLastRadioWriting();
        }

        //saves current ratings and binds to age radiobutton
        private void SaveLastRating(Test newTest)
        {
            //get button that is selected
            string ageStr = "";
            var obsAges = new System.Collections.ObjectModel.ObservableCollection<AgeInterval>();
            RadioButton checkedRb = null;
            for (int i = 0; i < agesControl.Items.Count; i++)
            {
                ContentPresenter c = (ContentPresenter)agesControl.ItemContainerGenerator.ContainerFromItem(agesControl.Items[i]);
                RadioButton rb = c.ContentTemplate.FindName("AgeRadio", c) as RadioButton;
                if ((bool)rb.IsChecked)
                {
                    ageStr = ((AgeInterval)rb.CommandParameter).interval;
                    checkedRb = rb;
                }
                else
                {
                    obsAges.Add((AgeInterval)rb.CommandParameter);
                }
            }
            //check if age is in correct format
            int age;
                string lowerBoundary = ageStr.Split('-')[0];
                Int32.TryParse(lowerBoundary, out age);
                if(!Int32.TryParse(lowerBoundary, out age))
                {
                MessageBox.Show("Please insert age.", "Confirmation");
                return;
                }

            //get ratings and ratinglabels connected with that age from the datagrids
            IList ratings = ratingsDatagrid.Items;
            IList labels = LabelandDescDatagrid.Items;
            //add age, testId, labelId to ratings and id to labels
            IList<Rating> ratingList = new List<Rating>();
            IList<RatingLabel> ratingLabelList = new List<RatingLabel>();
            for (int i=0; i<ratings.Count; i++)
            {
                byte[] labelId = System.Guid.NewGuid().ToByteArray();

                if (ratings[i] is Rating)
                {
                    ((Rating)ratings[i]).age = age;
                    ((Rating)ratings[i]).testId = newTest.id;
                    if (i < labels.Count && labels[i] is RatingLabel)
                    {
                        ((RatingLabel)labels[i]).id = labelId;
                        ((Rating)ratings[i]).labelId = labelId;
                        ratingLabelList.Add((RatingLabel)labels[i]);
                    }
                    ratingList.Add((Rating)ratings[i]);
                }
            }
            if (ratingList.Count == 0)
            {
                MessageBox.Show("Rating table is empty.", "Confirmation");
                return;
            }
            removeOldRatings(newTest, ((Rating)ratingList[0]).age); //checks if there are any ratings with given testId and age and removes them from db

            //add data to db
            var repo = new RatingRepository();
            foreach(Rating rati in ratingList)
            {
                repo.Insert(rati);
            }
            var rep = new RatingLabelRepository();
            foreach(RatingLabel lab in ratingLabelList)
            {
                rep.Insert(lab);
            }

            ClearRatingAndLabel();

            //bind CHECKED agebutton to inserted ratings
            AgeInterval rat = new AgeInterval() { interval = ((AgeInterval)checkedRb.CommandParameter).interval, rating = ratingList[0]};
            obsAges.Add(rat);
            agesControl.ItemsSource = obsAges;
            }

        //save new test into db
        private void btn_SaveTest(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Test test = (Test)button.CommandParameter;

            SaveLastRating(test);

            test.categoryId = ((TestCategory)this.categorySelector.SelectedItem).id;
            test.decimals = (int)this.decimalsSelector.SelectedItem;
 
            var repo = new TestRepository();
            repo.Insert(test);

            DisableFields();
            MessageBox.Show("Saved", "Confirmation");
        }

        private void removeOldRatings(Test test, int age)
        {
            var repo = new RatingRepository();
            IList<Rating> ages = repo.getAges(test);
            foreach(Rating a in ages)
            {
                //removes ratings with given testId and age
                if (a.age == age)
                {
                    //repo.removeRatings(test, age);
                }
                break;
            }
        }
        private void ClearRatingAndLabel()
        {
            var ratings = new System.Collections.ObjectModel.ObservableCollection<Rating>();
            ratingsDatagrid.ItemsSource = ratings;
            ratingsDatagrid.CanUserAddRows = true;
            ratingsDatagrid.IsReadOnly = false;

            var labels = new System.Collections.ObjectModel.ObservableCollection<RatingLabel>();
            LabelandDescDatagrid.IsReadOnly = false;
            LabelandDescDatagrid.CanUserAddRows = true;
            LabelandDescDatagrid.ItemsSource = labels;
        }

        private void EnableLastRadioWriting()
        {
            int n = agesControl.Items.Count - 1;
            ContentPresenter c = (ContentPresenter)agesControl.ItemContainerGenerator.ContainerFromItem(agesControl.Items[n]);
            c.ApplyTemplate();
            RadioButton rb = c.ContentTemplate.FindName("AgeRadio", c) as RadioButton;
            rb.IsChecked = true;
            TextBox tb = rb.Content as TextBox;
            tb.IsReadOnly = false;
        }

        private void btn_RemoveRating(object sender, RoutedEventArgs e)
        {
        }

        private void ClearAndEnableFields()
        {
            ClearRatingAndLabel();
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
            SaveTest.Visibility = System.Windows.Visibility.Visible;
            addNewRating.Visibility = System.Windows.Visibility.Visible;
            removeRating.Visibility = System.Windows.Visibility.Visible;
            Cancel.Visibility = System.Windows.Visibility.Visible;
        }

        private void DisableFields()
        {
            testName.IsReadOnly = true;
            units.IsReadOnly = true;
            decimalsSelector.IsEnabled = false;
            categorySelector.IsEnabled = false;
            TestDescriptionText.IsReadOnly = true;
            ratingsDatagrid.CanUserAddRows = false;
            ratingsDatagrid.IsReadOnly = true;
            LabelandDescDatagrid.IsReadOnly = true;
            LabelandDescDatagrid.CanUserAddRows = false;
            SaveTest.Visibility = System.Windows.Visibility.Hidden;
            addNewRating.Visibility = System.Windows.Visibility.Hidden;
            removeRating.Visibility = System.Windows.Visibility.Hidden;
            Cancel.Visibility = System.Windows.Visibility.Hidden;
        }

        class AgeInterval
        {
            public string interval { get; set; }
            public Rating rating { get; set; }
        }
    }
}

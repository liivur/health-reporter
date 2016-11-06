﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HealthReporter.Models;
using System.Diagnostics;
using System.Windows.Media;
using System.Linq;
using System.Windows.Input;

namespace HealthReporter.Controls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class TestsUserControl : UserControl
    {
        private MainWindow _parent;
        private static Test selectedTest;
        public TestsUserControl(MainWindow parent)
        {          
            InitializeComponent();
            this._parent = parent;

            var catRep = new TestCategoryRepository();
            IList<TestCategory> categories = catRep.FindRootCategories();
            catsDataGrid.ItemsSource = categories;

            if (categories.Count > 0) catsDataGrid.SelectedIndex = 0;

            var repo = new TestRepository();
            IList<Test> tests = repo.FindAll();
            

            decimalsSelector.ItemsSource = new List<int> { -2, -1, 0, 1, 2 };

            categorySelector.ItemsSource = catRep.FindAll();

            btnShowTests.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E0EEEE"));

            findTestTotal();
        }

        private void findTestTotal()
        {
            // Total client amount
            var repoT = new TestRepository();
            IList<Test> alltests = repoT.FindAll();

            testTotal.Text = alltests.Count.ToString() + " Tests";
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
            this.Opacity = 0.3;
            this._parent.stkTest.Children.Add(obj);
        }

        private void GenderTabsItemssource(System.Collections.ObjectModel.ObservableCollection<TabItem> list)
        {
            for (int i = 0; i < MenAgesTab.Items.Count; i++) //fixes annoying binding warnings
            {
                System.Windows.Controls.TabItem c = (System.Windows.Controls.TabItem)MenAgesTab.ItemContainerGenerator.ContainerFromItem(MenAgesTab.Items[i]);
                if(c != null) c.Template = null;
            }
            for (int i = 0; i < WomenAgesTab.Items.Count; i++)
            {
                System.Windows.Controls.TabItem c = (System.Windows.Controls.TabItem)WomenAgesTab.ItemContainerGenerator.ContainerFromItem(WomenAgesTab.Items[i]);
                if(c != null) c.Template = null;
            }
            MenAgesTab.ItemsSource = list;
            WomenAgesTab.ItemsSource = list;
        }

        private void RatingsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                RowItem item = e.Row.DataContext as RowItem;
                DataGrid grid = sender as DataGrid;

                int age = -1;
                if ((GenderTab.SelectedItem as System.Windows.Controls.TabItem).Header.ToString() == "Men") age = (int)MenAgesTab.SelectedValue;
                else age = (int)WomenAgesTab.SelectedValue;
                Test test = (Test)testName.DataContext;

                int itemIndex = grid.Items.IndexOf(item);

                if (item.normF == 0) item.normF = item.normM;
                else if (item.normM == 0) item.normM = item.normF;

                Rating rat = new Rating() { age = age, normF = item.normF, normM = item.normM, testId = test.id, labelId = item.LabelId };

                var ratingRep = new RatingRepository();
                var labelRep = new RatingLabelRepository();
                IList<Rating> ratings = ratingRep.getSameAgeRatings(new Rating() { testId = test.id, age = age });

                if (item.LabelId == null)
                {
                    byte[] id = System.Guid.NewGuid().ToByteArray();

                    RatingLabel label = new RatingLabel() { id = id, interpretation = item.interpretation, name = item.name };
                    if (item.rating == null) label.rating = 0;
                    else label.rating = colorToInt(item.rating.ToString());
                    rat.labelId = id;
                    labelRep.Insert(label);
                }
                else
                {
                    RatingLabel dbLabel = labelRep.getLabel(rat)[0];

                    int label_rating = 0;
                    if (item.rating != null) label_rating = colorToInt(item.rating.ToString());

                    if (dbLabel.name != item.name || dbLabel.rating != label_rating || dbLabel.interpretation != item.interpretation)
                    {
                        RatingLabel label = new RatingLabel() { id = dbLabel.id, interpretation = item.interpretation, name = item.name, rating=label_rating };
                        labelRep.Update(label);
                    }
                }
                var testRep = new TestRepository();
                if (ratings.Count > itemIndex)
                {
                    Rating dbRating = ratings[itemIndex];
                    dbRating.labelId = rat.labelId; //Not good! Needs to be fixed in the future.
                    ratingRep.Update(dbRating, rat);
                    testRep.Update(test);
                }
                else
                {
                    ratingRep.Insert(rat);
                    testRep.Update(test);
                }
            }
        }

        private int colorToInt(String color)
        {
            int rating = -1;

            switch (color.ToString())
            {
                case "#FFFF0000":
                    rating = 0;
                    break;
                case "#FFFFA500":
                    rating = 1;
                    break;
                case "#FFFFFF00":
                    rating = 2;
                    break;
                case "#FF008000":
                    rating = 3;
                    break;
                case "#FF0000FF":
                    rating = 4;
                    break;
                default: { break; }
            }
            return rating;
        }

        private void KeyDownHandler(object sender, KeyEventArgs e) //removes selected rating and ratingLabel from the database if Delete key is pressed
        {
            int tab = (int)WomenAgesTab.SelectedValue;
            if ((GenderTab.SelectedItem as System.Windows.Controls.TabItem).Header.ToString() == "Men")
            {
               tab = (int)MenAgesTab.SelectedValue;
            }
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                foreach (var row in grid.SelectedItems)
                {
                    if (row is RowItem)
                    {
                        int age = -1;
                        if ((GenderTab.SelectedItem as System.Windows.Controls.TabItem).Header.ToString() == "Men") age = (int)MenAgesTab.SelectedValue;
                        else age = (int)WomenAgesTab.SelectedValue;

                        RowItem item = (RowItem)row;
                        var repo = new RatingRepository();
                        repo.DeleteRating(new Rating() { testId = ((Test)testName.DataContext).id, age = age, labelId = item.LabelId, normF = item.normF, normM = item.normM });
                        var rep = new RatingLabelRepository();
                        rep.DeleteByRating(new Rating() { labelId = item.LabelId });
                    }
                }
                updateTestView((Test)testName.DataContext);

                MenAgesTab.SelectedValue = tab;

                var testRep = new TestRepository();
                testRep.Update((Test)testName.DataContext);
            }
        }

        private void btn_AddNewTest(object sender, RoutedEventArgs e)
        {
            Test newTest = new Test() { };
            newTest.id = System.Guid.NewGuid().ToByteArray();
            newTest.name = "No Name";
            if ((TestCategory)catsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("You have not created any categories yet");
                return;
            }
            newTest.categoryId = ((TestCategory)catsDataGrid.SelectedItem).id;

            //new test is inserted into db
            var repo = new TestRepository();
            repo.Insert(newTest);

            int i = catsDataGrid.SelectedIndex;
            catsDataGrid.SelectedIndex = -1;
            catsDataGrid.SelectedIndex = i;

            foreach(LastTest lTest in testsDataGrid.Items)
            {
                if (lTest.test.id.SequenceEqual(newTest.id)) testsDataGrid.SelectedItem = lTest;
            }
            findTestTotal();
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
                LastTest ltest = (LastTest)selected[0];
                Test test = ltest.test;
                selectedTest = test;
                updateTestView(test);
                
                testDetailDatagrid.Visibility = Visibility.Visible;
            }
            else testDetailDatagrid.Visibility = Visibility.Hidden;
        }

        private void menRatingsDatagrid_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        private void WomenRatingsDatagrid_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        private void categorySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool catChanged = saveChangesToDb((Test)testName.DataContext);
            if (catChanged)
            {
                TestCategory selected = (TestCategory)categorySelector.SelectedItem;
                if (selected != null && selected.parentId == null)
                {
                    catsDataGrid.SelectedValue = selected.name;
                    updateTestsColumn(selected);
                }
                else if (selected != null)
                {
                    var repo = new TestCategoryRepository();
                    TestCategory parent = repo.GetParent(selected)[0];
                    catsDataGrid.SelectedValue = parent.name;
                    updateTestsColumn(parent);
                }
                testsDataGrid.SelectedValue = ((Test)testName.DataContext).name;
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

        //updates test fields
        private void updateTestView(Test test)
        {
            testName.DataContext = test;
            units.DataContext = test;
            TestDescriptionText.DataContext = test;
            decimalsSelector.DataContext = test;
            categorySelector.DataContext = test;
            FormulaTextF.DataContext = test;
            FormulaTextM.DataContext = test;

            IList<TabItem> tabitems = new List<TabItem>();
            IList<AgeInterval> ageIntervals = getAgeIntervals(test);
  
            IList<Rating> rats = new List<Rating>();

            var repo = new RatingRepository();
            foreach (AgeInterval ageint in ageIntervals)
            {
                List<RowItem> items = new List<RowItem>();
                IList<Rating> sameAgeRatings = repo.getSameAgeRatings(ageint.rating);

                var rep = new RatingLabelRepository();
                IList<RatingLabel> labs = new List<RatingLabel>();

                foreach (Rating rat in sameAgeRatings)
                {
                    labs = rep.getLabel(rat);
                    foreach (RatingLabel lab in labs)
                    {

                        Brush brush = ratingToColor(lab.rating);
                        items.Add(new RowItem() {name = lab.name, interpretation=lab.interpretation, LabelId=lab.id, normF=rat.normF, normM=rat.normM, rating=brush});
                    }
                    if (labs.Count == 0)
                    {
                        items.Add(new RowItem() { normF = rat.normF, normM = rat.normM, rating = ratingToColor(0)});
                    }
                }
                          
                tabitems.Add(new TabItem() { interval = ageint, rowitems = items});
            }
            GenderTabsItemssource(new System.Collections.ObjectModel.ObservableCollection<TabItem>(tabitems));

            if (tabitems.Count > 0) MenAgesTab.SelectedIndex = 0;

            //show subcategory
            var catRep = new TestCategoryRepository();
            foreach (TestCategory cat in categorySelector.Items)
            {
                if (cat.id.SequenceEqual(catRep.GetCategoryByTest(test)[0].id))
                {
                    categorySelector.SelectedValue = cat.id;
                    break;
                }
            }
        }

        private Brush ratingToColor(int rating)
        {
            string color = "Red";

            switch (rating)
            {
                case 0:
                    color = "Red";
                    break;
                case 1:
                    color = "Orange";
                    break;
                case 2:
                    color = "Yellow";
                    break;
                case 3:
                    color = "Green";
                    break;
                case 4:
                    color = "Blue";
                    break;
                default: { break; }
            }
            var converter = new BrushConverter();
            var brush = (Brush)converter.ConvertFromString(color);
            return brush;
        }

        private bool saveChangesToDb(Test test) //saves changes made on selected test to db, returns true if category was changed
        {
            if (test == null) return false;

            var repo = new TestRepository();
            IList<Test> tests = repo.Get(test);
            if (tests.Count == 0) return false;
            Test dbTest = tests[0];

            if (!test.categoryId.SequenceEqual(dbTest.categoryId) || test.decimals!=dbTest.decimals || test.description != dbTest.description || test.formulaF != dbTest.formulaF ||
                test.formulaM != dbTest.formulaM || test.name != dbTest.name || test.units != dbTest.units || test.weight != dbTest.weight || test.position != dbTest.position)
            {
                repo.Update(test);
            }
            return (!test.categoryId.SequenceEqual(dbTest.categoryId));
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

        private void btn_AddStuff(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void btn_AddNewAge(object sender, RoutedEventArgs e) //adds new age group
        {
            if (testsDataGrid.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a test.");
                return;
            }

            int lastAge = 0;
            if (MenAgesTab.Items.Count > 0)
            {
                TabItem item = (TabItem)MenAgesTab.Items[MenAgesTab.Items.Count - 1];
                lastAge = item.interval.rating.age;
            }

            InputDialog inputDialog = new InputDialog("Please enter new age group:", lastAge + "-");
            if (inputDialog.ShowDialog() == true)
            {

                Test test = (Test)testName.DataContext;

                int frst = parse_age(inputDialog.Answer, true, 0);
                int last = parse_age(inputDialog.Answer, true, 1);

                if (frst != -1 && last != -1)
                {
                    var repo = new RatingRepository();
                    if (MenAgesTab.Items.Count == 0)
                    {
                        repo.Insert(new Rating() { testId = test.id, age = frst});
                        repo.Insert(new Rating() { testId = test.id, age = last + 1});
                    }
                    else if(((TabItem)MenAgesTab.Items[MenAgesTab.Items.Count-1]).interval.rating.age > frst) 
                    {
                        repo.Insert(new Rating() { testId = test.id, age = frst});
                        repo.Insert(new Rating() { testId = test.id, age = last + 1});
                    }
                    else
                    {
                        repo.Insert(new Rating() { testId = test.id, age = last + 1});
                    }
                }
                else MessageBox.Show("Invalid age group.");
                updateTestView(test);
            }
        }

        private void btn_DeleteRating(object sender, RoutedEventArgs e) //deletes last rating
        {
            if (testsDataGrid.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a test.");
                return;
            }
            if (MenAgesTab.Items.Count == 0) return;

            TabItem tabitem = (TabItem)MenAgesTab.Items[MenAgesTab.Items.Count - 1];
            int last = tabitem.interval.rating.age;

            var repo = new RatingRepository();
            repo.removeRatingsByAge((Test)testName.DataContext, last);

            updateTestView((Test)testName.DataContext);
        }

        private int parse_age(string ageStr, bool last, int i)
        {
            int age = -1;
            if (ageStr.Split('-').Length==2)
            {
                Int32.TryParse(ageStr.Split('-')[i], out age);
            }
            return age;
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

        private void btn_DeleteTest(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the selected test?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if(result == MessageBoxResult.Yes)
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
                GenderTabsItemssource(null);

                IList<Test> tests = testRepo.FindAll();
                findTestTotal();
               
            }
        }

        private void ClearFields()
        {
            testName.DataContext = null;
            units.DataContext = null;
            decimalsSelector.DataContext = null;
            categorySelector.DataContext = null;
            TestDescriptionText.DataContext = null;
            FormulaTextF.DataContext = null;
            FormulaTextM.DataContext = null;           
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
        class TabItem
        {
            public TabItem()
            {
            }
            public AgeInterval interval { get; set; }
            public IList<RowItem> rowitems { get; set; }
        }

        class RowItem { 
            public RowItem()
            {
            }
            public decimal normF { get; set; }
            public decimal normM { get; set; }
            public byte[] LabelId { get; set; }
            public string name { get; set; }
            public string interpretation { get; set; }
            public Brush rating { get; set; }
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

        private void filterSearchBox(object sender, TextChangedEventArgs e)
        {
            string searchBy = search.Text;

            TestCategory selectedcategory = (TestCategory)catsDataGrid.SelectedItem;
            
            var repo = new TestRepository();
            IList<Test> result = repo.FindSearchResult(searchBy, selectedcategory);
            

            IList<LastTest> tests = new List<LastTest>();
            for (int i = 0; i < result.Count; i++)
            {
                if (i + 1 < result.Count)
                {
                    if (!result[i].categoryId.SequenceEqual(result[i + 1].categoryId))
                    {
                        tests.Add(new LastTest() { isLast = true, test = result[i] });
                    }
                    else tests.Add(new LastTest() { isLast = false, test = result[i] });
                }
                else tests.Add(new LastTest() { isLast = false, test = result[i] });
            }
            testsDataGrid.ItemsSource = tests;
            testsDataGrid.SelectedIndex = 0;
        }

        private void TextBox_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            var rep = new TestRepository();
            rep.Update((Test)testName.DataContext);
        }
    }
}
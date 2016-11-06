using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HealthReporter;

using System.Windows.Automation;
using Winium.Cruciatus.Core;
using Winium.Cruciatus.Extensions;
using System.IO;
using TestStack.White;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.MenuItems;
using TestStack.White.UIItems.TreeItems;
using TestStack.White.UIItems.Finders;

// https://msdn.microsoft.com/en-us/library/ms182532.aspx

namespace HealthReporterUnitTests
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        private TestContext _testContext;

        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }

        [TestMethod]
        public void TestMethod1()
        {
            // arrange
            double a = 11.99;
            double b = 12;
            double expectedSum = a+b;

            // act
            double actualSum = a + b;

            // assert
            Assert.AreEqual(expectedSum, actualSum, 0.001, "Wrong sum");
        }

        //[TestMethod]
        //public void TestCalc()
        //{
        //    var calc = new Winium.Cruciatus.Application("C:/windows/system32/calc.exe");
        //    calc.Start();

        //    var winFinder = By.Name("Calculator").AndType(ControlType.Window);
        //    var win = Winium.Cruciatus.CruciatusFactory.Root.FindElement(winFinder);
        //    var menuBar = win.FindElementByUid("MenuBar");
        //    Console.WriteLine(menuBar.ToString());
        //    var menu = menuBar.ToMenu();
        //    Console.WriteLine(menu.ToString());
        //    menu.SelectItem("View$Scientific");
        //    menu.SelectItem("View$History");

        //    double expectedSum = 2 + Math.Pow(4, 8);

        //    win.FindElementByUid("132").Click(); // 2
        //    win.FindElementByUid("93").Click(); // +
        //    win.FindElementByUid("134").Click(); // 4
        //    win.FindElementByUid("97").Click(); // ^
        //    win.FindElementByUid("138").Click(); // 8
        //    win.FindElementByUid("121").Click(); // =

        //    var resultBox = win.FindElementByUid("150");
        //    double actualSum = Double.Parse(resultBox.GetAutomationPropertyValue<String>(AutomationElement.NameProperty));

        //    calc.Close();
        //    Assert.AreEqual(expectedSum, actualSum, 0.001, "Wrong sum");
        //}

        //[TestMethod]
        //public void TestApp()
        //{
        //    string solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestRunDirectory));
        //    Console.WriteLine(solutionDir);

        //    var app = new Winium.Cruciatus.Application(solutionDir + "/HealthReporter/bin/Debug/HealthReporter.exe");
        //    app.Start();

        //    var window = Winium.Cruciatus.CruciatusFactory.Root.FindElement(By.Name("Health Reporter").AndType(ControlType.Window));
        //    Console.WriteLine(window.ToString());
        //    var addButton = window.FindElementByUid("addStuff");
        //    addButton.Click();
        //    //var menu = window.FindElement(By.XPath("//ContextMenu"));
        //    //menu.ToMenu().SelectItem("New client");
        //    window.FindElementByUid("newGroup").Click();
        //    //var menu = addButton.ToMenu();
        //    //Console.WriteLine(menu.ToString());
        //    //menu.SelectItem("New client");
        //    //addButton.Click();
        //    Console.WriteLine(addButton.ToString());
        //    //addButton.Click();
        //    app.Close();

        //}

        [TestMethod]
        public void TestAppWhite()
        {
            string groupName = "Test grupp";
            //string applicationDirectory = TestContext.TestDeploymentDir;
            string solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestRunDirectory));
            string applicationPath = Path.Combine(solutionDir, "HealthReporter/bin/Debug/HealthReporter.exe");
            Application application = Application.Launch(applicationPath);
            Window window = application.GetWindow("Health Reporter", InitializeOption.NoCache);
            //Console.WriteLine(window.Title);

            ListView groups = window.Get<ListView>("groupDataGrid");
            var rows = groups.Rows;
            int groupCount = rows.Count;

            Button button = window.Get<Button>("addStuff");
            //Console.WriteLine(button.ToString());
            button.Click();

            PopUpMenu menu = window.Popup;
            //Console.WriteLine(menu.ToString());
            //var items = menu.Items;
            //foreach (var element in items)
            //{
            //    Console.WriteLine("element");
            //    Console.WriteLine(element.ToString());
            //}
            menu.Item("New group").Click();
            //Console.WriteLine();

            
            groups.SelectedRows[0].Enter(groupName);
            //rows[rows.Count - 1].Enter(groupName);
            window.Click();
            //foreach (var element in groups.Rows)
            //{
            //    Console.WriteLine("row");
            //    Console.WriteLine(element.ToString());
            //    Console.WriteLine(element.IsSelected);
            //    Console.WriteLine(element.IsFocussed);
            //}
            //foreach (var element in groups.SelectedRows)
            //{
            //    Console.WriteLine("selected row");
            //    Console.WriteLine(element.Name);
            //    Console.WriteLine(element.IsFocussed);
            //    element.Enter("Test grupp");
            //    element.Focus();
            //}
            //groups.SelectedRows[0].SetValue("Test grupp");

            rows = groups.Rows;
            int newCount = rows.Count;
            string newName = rows[newCount - 1].Name;
            var test = rows[newCount - 1].GetElement(SearchCriteria.All);
            //foreach (var element in test.GetSupportedProperties())
            //{
            //    Console.WriteLine("property");
            //    Console.WriteLine(element.ToString());
            //    Console.WriteLine(element.ProgrammaticName);
            //    Console.WriteLine(test.GetCurrentPropertyValue(element));
            //}
            //Console.WriteLine(test.GetCurrentPropertyValue(AutomationElement.NameProperty));
            //Console.WriteLine(test.GetCurrentPropertyValue(AutomationElement.AutomationIdProperty));
            //Console.WriteLine(test);
            application.Close();
            //Console.WriteLine(groupName + " " + newName);
            //Console.WriteLine(groupCount + " " + newCount);

            Assert.AreEqual(groupCount+1, newCount, 0, "Wrong number of groups after adding one.");
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HealthReporter;

using System.Windows.Automation;
using Winium.Cruciatus.Core;
using Winium.Cruciatus.Extensions;

// https://msdn.microsoft.com/en-us/library/ms182532.aspx

namespace HealthReporterUnitTests
{
    [TestClass]
    public class MainWindowViewModelTests
    {
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

        [TestMethod]
        public void TestCalc()
        {
            var calc = new Winium.Cruciatus.Application("C:/windows/system32/calc.exe");
            calc.Start();

            var winFinder = By.Name("Calculator").AndType(ControlType.Window);
            var win = Winium.Cruciatus.CruciatusFactory.Root.FindElement(winFinder);
            var menu = win.FindElementByUid("MenuBar").ToMenu();

            menu.SelectItem("View$Scientific");
            menu.SelectItem("View$History");

            double expectedSum = 2 + Math.Pow(4, 8);

            win.FindElementByUid("132").Click(); // 2
            win.FindElementByUid("93").Click(); // +
            win.FindElementByUid("134").Click(); // 4
            win.FindElementByUid("97").Click(); // ^
            win.FindElementByUid("138").Click(); // 8
            win.FindElementByUid("121").Click(); // =

            var resultBox = win.FindElementByUid("150");
            double actualSum = Double.Parse(resultBox.GetAutomationPropertyValue<String>(AutomationElement.NameProperty));

            calc.Close();
            Assert.AreEqual(expectedSum, actualSum, 0.001, "Wrong sum");
        }

        //[TestMethod]
        //public
    }
}

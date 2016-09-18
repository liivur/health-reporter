using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HealthReporter;

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
            double expectedSum = 25.55;

            // act
            double actualSum = a + b;

            // assert
            Assert.AreEqual(expectedSum, actualSum, 0.001, "Wrong sum");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using Winium.Cruciatus.Core;
using Winium.Cruciatus.Extensions;

namespace HealthReporterUITests
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc = new Winium.Cruciatus.Application("C:/windows/system32/calc.exe");
            calc.Start();

            var winFinder = By.Name("Calculator").AndType(ControlType.Window);
            var win = Winium.Cruciatus.CruciatusFactory.Root.FindElement(winFinder);

            var elements = win.FindElements(winFinder);
            Console.WriteLine("WTF");
            Console.WriteLine(elements);
            foreach (var element in elements)
            {
                Console.WriteLine("element");
                Console.WriteLine(element.ToString());
            }

            var menu = win.FindElementByUid("MenuBar").ToMenu();

            menu.SelectItem("View$Scientific");
            menu.SelectItem("View$History");

            win.FindElementByUid("132").Click(); // 2
            var elements2 = win.FindElementByUid("150");
            Console.WriteLine(elements2.GetAutomationPropertyValue<String>(AutomationElement.NameProperty));
            win.FindElementByUid("93").Click(); // +
            Console.WriteLine(elements2.GetAutomationPropertyValue<String>(AutomationElement.NameProperty));
            win.FindElementByUid("134").Click(); // 4
            Console.WriteLine(elements2.GetAutomationPropertyValue<String>(AutomationElement.NameProperty));
            win.FindElementByUid("97").Click(); // ^
            Console.WriteLine(elements2.GetAutomationPropertyValue<String>(AutomationElement.NameProperty));
            win.FindElementByUid("138").Click(); // 8
            Console.WriteLine(elements2.GetAutomationPropertyValue<String>(AutomationElement.NameProperty));
            win.FindElementByUid("121").Click(); // =
            Console.WriteLine(elements2.GetAutomationPropertyValue<String>(AutomationElement.NameProperty));

            calc.Close();
        }
    }
}

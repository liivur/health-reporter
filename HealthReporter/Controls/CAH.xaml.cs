using System;
using System.Collections.Generic;
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

namespace HealthReporter.Controls
{
    /// <summary>
    /// Interaction logic for ClientAppraisalHistoryControl.xaml
    /// </summary>
    public partial class CAH : UserControl
    {
        private Client client;
        private MainWindow _parent;


        public CAH(MainWindow _parent)
        {
       
            this._parent = _parent;
        }

        public CAH(MainWindow _parent, Client client) : this(_parent)
        {
            InitializeComponent();
            this._parent = _parent;
            this.client = client;
            ClientInfo.DataContext = client;


            //Finding all appraisal results of client
            var repo = new AppraisalsRepository();
            IList<HistoryTableItem> history = repo.FindAll(client);


            //Finding all appraisal dates of client
            List<string> dates = new List<string>();

            foreach (HistoryTableItem item in history)
            {
                if (item.date != null && !dates.Contains(item.date.ToString()))
                {
                    dates.Add(item.date.ToString());
                }
            }

            //Finding all tests that client has taken
            List<string> tests = new List<string>();

            foreach (HistoryTableItem item in history)
            {
                if (item.TestName != null && !tests.Contains(item.TestName))
                {
                    tests.Add(item.TestName);
                }
            }

            //Initializing datagrid objects
            List<FullHistoryDatagrid> result = new List<FullHistoryDatagrid>();
           
               foreach(HistoryTableItem item in history)
                {
                   if (!result.Exists(x=>x.TestName == item.TestName)) {
                    FullHistoryDatagrid newOne = new FullHistoryDatagrid();
                    newOne.TestName =item.TestName ;
                    newOne.units = item.Units;
                    newOne.list = new List<Date_Score_Appraiser>();
                    
                    foreach(string date in dates)
                    {
                        if (item.date != date)
                        {
                            Date_Score_Appraiser newOne2 = new Date_Score_Appraiser();
                            newOne2.date = date;
                            newOne2.appraiser = "";
                            newOne2.score = 0;
                            newOne.list.Add(newOne2);
                        }
                        else
                        {
                            Date_Score_Appraiser newOne2 = new Date_Score_Appraiser();
                            newOne2.date = item.date;
                            newOne2.appraiser = item.AppraisersName;
                            newOne2.score =item.Score;
                            newOne.list.Add(newOne2);
                        }
                        
                    }
                    result.Add(newOne);

                }else
                {
                    foreach (string date in dates)
                    {
                        if (item.date != date)
                        {
                            
                        }
                        else
                        {
                            FullHistoryDatagrid getElem = result.Find(x => x.TestName == item.TestName);
                            foreach (Date_Score_Appraiser elem in getElem.list)
                            {
                                if( date == elem.date)
                                {
                                    elem.appraiser = item.date;
                                    elem.date = date;
                                    elem.score = item.Score;
                                }

                            }
                          
                        }

                    }
                }
                }
           



            //dates.Add(client.birthDate);
            //dates.Add(DateTime.Now.ToString());
            //dates.Add(DateTime.Now.AddDays(36).ToString());

            //foreach (HistoryTableItem item in history)
            //{
            //    if (item.date != null && !dates.Contains(item.date.ToString()))
            //    {
            //        //MessageBox.Show(item.date.ToString(), "Message");
            //        dates.Add(item.date.ToString());
            //    }
            //}

            //FullHistoryDatagrid test = new FullHistoryDatagrid();
            //test.TestName = "Triceps";
            //test.units = "mm";
            //test.list = new List<Date_Score_Appraiser>();
            //Date_Score_Appraiser uus = new Date_Score_Appraiser();
            //uus.date = client.birthDate;
            //uus.appraiser = "Kristiina";
            //uus.score = 1;
            //test.list.Add(uus);
            //Date_Score_Appraiser uus2 = new Date_Score_Appraiser();
            //uus2.date = DateTime.Now.ToString();
            //uus2.appraiser = "Kristiina";
            //uus2.score = 2;
            //test.list.Add(uus2);
            ////foreach(Date_Score_Appraiser elem in test.list)
            ////{
            ////    MessageBox.Show(elem.score.ToString(), "Message");
            ////}
            //result.Add(test);

            //FullHistoryDatagrid test2 = new FullHistoryDatagrid();
            //test2.TestName = "Biceps";
            //test2.units = "cm";
            //test2.list = new List<Date_Score_Appraiser>();
            //Date_Score_Appraiser uus3 = new Date_Score_Appraiser();
            //uus3.date = client.birthDate;
            //uus3.appraiser = "Kristiina";
            //uus3.score = 3;
            //test2.list.Add(uus3);
            //Date_Score_Appraiser uus4 = new Date_Score_Appraiser();
            //uus4.date = DateTime.Now.ToString();
            //uus4.appraiser = "Kristiina";
            //uus4.score = 4;
            //test2.list.Add(uus4);
            

            //Date_Score_Appraiser uus5 = new Date_Score_Appraiser();
            //uus5.date = DateTime.Now.AddDays(36).ToString();
            //uus5.appraiser = "Kristiina";
            //uus5.score = 5;
            //test2.list.Add(uus5);
            //result.Add(test2);


            //list (TestName, units, (date, score, appraiser))
            int i = 0;
            //Reading elements into table
            foreach (string elem in dates)
            {
                
                    DataGridTextColumn textColumn = new DataGridTextColumn();
                    textColumn.Header = elem;
                textColumn.Binding = new Binding("list[" + i + "]");
                         
                dataGrid.Columns.Add(textColumn);
                i++;
           
            }


            dataGrid.ItemsSource = result;


        }

        //Classes for holding all HistoryDatagrid Items
        public class FullHistoryDatagrid
        {
            public string TestName { get; set; }
            public string units { get; set; }
            public List<Date_Score_Appraiser> list { get; set; }

          
        }

        public class Date_Score_Appraiser
        {
            public string date { get; set; }
            private decimal _score;
            public string appraiser { get; set; }
            public decimal score
            {
                get
                {
                    return _score;
                }
                set
                {
                    _score = value;
                }
            }
            public override string ToString()
            {
                if (score.ToString() == "0")
                {
                    return "";
                }
                else
                {
                    return score.ToString();
                }
            }
        }

           
        



        private void btn_Back(object sender, RoutedEventArgs e)
        {
            this._parent.stkTest.Children.Clear();
            ClientUserControl obj = new ClientUserControl(this._parent);
            this._parent.stkTest.Children.Add(obj);
        }
        
        private void btn_NewAppraisal(object sender, RoutedEventArgs e)
        {
            this._parent.stkTest.Children.Clear();
            CAH_SelectTestControl obj = new CAH_SelectTestControl(this._parent,client);
            this._parent.stkTest.Children.Add(obj);
        }

        private void btn_AddTest(object sender, RoutedEventArgs e)
        {

           
        }

        private void btn_Report(object sender, RoutedEventArgs e)
        {
       
           
        }
    }
}

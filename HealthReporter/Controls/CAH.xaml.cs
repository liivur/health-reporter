﻿using System;
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

            Client client1 = this.client;

            if (client1.gender == "1"|| client1.gender == "Male")
            {
                client1.gender = "Male";
            }
            else
            {
                client1.gender = "Female";
            }

            ClientInfo.DataContext = client1;


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

            dates.Sort((x, y) => DateTime.Parse( y).CompareTo(DateTime.Parse(x)));

            //Creating list with structure: (TestName, units, (date, score, appraiser))

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
                        if (item.date == date)
                        {
                            FullHistoryDatagrid getElem = result.Find(x => x.TestName == item.TestName);
                            foreach (Date_Score_Appraiser elem in getElem.list)
                            {
                                if (date == elem.date)
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




            //Reading elements into table
            int i = 0;
            foreach (string elem in dates)
            {
                
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = String.Format("{0:dd/MM/yyyy}", DateTime.Parse(elem));
                textColumn.Binding = new Binding("list[" + i + "]");
                Style style = new Style(typeof(DataGridCell))
                {
                    Setters = {
                                new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right)
                }
                };
                textColumn.CellStyle = style;

                dataGrid.Columns.Add(textColumn);
                i++;
           
            }

            dataGrid.ItemsSource = result;


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

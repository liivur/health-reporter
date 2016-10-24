using HealthReporter.Utilities;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthReporter.Models
{

    public class AppraisalsRepository
    {
        public void Insert(Appraisal appraisal, Appraiser appraiser, List<Appraisal_tests> tests)
        {
            var connection = DatabaseUtility.getConnection();

            var res = connection.InsertSql("INSERT INTO appraisals (id, appraiserId, clientId, date ) values(@id, @appraiserId, @clientId, @date)", appraisal);
            var res2 = connection.InsertSql("INSERT INTO appraisers (id, name) values(@id, @name)", appraiser);
            foreach (Appraisal_tests test in tests)
            {
                var res3 = connection.InsertSql("INSERT INTO appraisal_tests (appraisalId, testId, score, note, trial1, trial2, trial3) values(@appraisalId, @testId, @score, @note, @trial1, @trial2, @trial3)", test);
            }


        }

        public IList<HistoryTableItem> FindAll(Client client)
        {
            IList<HistoryTableItem> res = DatabaseUtility.getConnection().QuerySql<HistoryTableItem>("SELECT tests.name as TestName,tests.units,  appraisal_tests.score as score, appraisers.name as AppraisersName FROM appraisers inner JOIN appraisals ON appraisals.appraiserId = appraisers.id inner JOIN appraisal_tests ON appraisal_tests.appraisalId = appraisals.id inner JOIN tests ON tests.id = appraisal_tests.testId WHERE appraisals.clientId=@id ", client);

            //IList< Result > res = DatabaseUtility.getConnection().QuerySql<Result>("SELECT * FROM clients");
            return res;

        }

    }

    public class Appraisal
    {
        public byte[] id { get; set; }
        public byte[] appraiserId { get; set; }
        public byte[] clientId { get; set; }
        public string date { get; set; }
        public string updated { get; set; }

    }

    public class Appraiser 
    {
        public byte[] id { get; set; }
        public string name; 
        public string updated { get; set; }

       

    }
    public class Appraisal_tests
    {
        public byte[] appraisalId { get; set; }
        public byte[] testId { get; set; }
        public decimal score { get; set; }
        public string note { get; set; }
        public decimal trial1 { get; set; }
        public decimal trial2 { get; set; }
        public decimal trial3 { get; set; }
        public string updated { get; set; }


    }
    public class HistoryTableItem
    {

       
        public string TestName { get; set; }
        public string units { get; set; }       
        public decimal score { get; set; }
        public string AppraisersName { get; set; }


    }


}

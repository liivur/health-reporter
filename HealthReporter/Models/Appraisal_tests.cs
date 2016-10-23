using HealthReporter.Utilities;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthReporter.Models
{

    public class Appraisal_testsRepository
    {
        public void Insert(Appraisal_tests test)
        {
            var connection = DatabaseUtility.getConnection();
            //test.id = System.Guid.NewGuid().ToByteArray();
            var res = connection.InsertSql("INSERT INTO appraisal_tests (appraisalId, testId, score, note, trial1, trial2, trial3) values(@appraisalId, @testId, @score, @note, @trial1, @trial2, @trial3)", test);
        }

        public IList<Appraisal_tests> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<Appraisal_tests>("SELECT * FROM appraisal_tests");
        }
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

    }
}

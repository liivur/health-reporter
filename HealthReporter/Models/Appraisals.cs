using HealthReporter.Utilities;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthReporter.Models
{

    public class AppraisalsRepository
    {
        public void Insert(Appraisals appraisal)
        {
            var connection = DatabaseUtility.getConnection();
            //test.id = System.Guid.NewGuid().ToByteArray();
            var res = connection.InsertSql("INSERT INTO appraisals (id, appraiserId, clientId, date ) values(@id, @appraiserId, @clientId, @date)", appraisal);
        }

        public IList<Appraisals> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<Appraisals>("SELECT * FROM appraisals");
        }  

    }

    public class Appraisals
    {
        public byte[] id { get; set; }
        public byte[] appraiserId { get; set; }
        public byte[] clientId { get; set; }
        public string date { get; set; }
        public string uploaded { get; set; }
       
    }
}

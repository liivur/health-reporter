using HealthReporter.Utilities;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthReporter.Models
{

    public class AppraisersRepository
    {
        public void Insert(Appraisers appraiser)
        {
            var connection = DatabaseUtility.getConnection();
            //test.id = System.Guid.NewGuid().ToByteArray();
            var res = connection.InsertSql("INSERT INTO appraisers (id, name) values(@id, @name)", appraiser);
        }

        public void Delete(Appraisers appraiser)
        {
            var res = DatabaseUtility.getConnection().InsertSql("DELETE from appraisers where id=@id", appraiser);
        }

        public IList<Appraisers> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<Appraisers>("SELECT * FROM appraisers");
        }

    }



    public class Appraisers
    {
        public byte[] id { get; set; } 
        public string name { get; set; }
        public string updated { get; set; }
       
    }
}

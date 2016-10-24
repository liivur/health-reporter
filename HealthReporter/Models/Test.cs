using System.Collections.Generic;
using Insight.Database;
using HealthReporter.Utilities;

namespace HealthReporter.Models
{
    class TestRepository
    {
        public void Insert(Test test)
        {
            var connection = DatabaseUtility.getConnection();
            //test.id = System.Guid.NewGuid().ToByteArray();
            var res = connection.InsertSql("INSERT INTO tests (id, categoryId, name, description, units, decimals, weight, formulaF, formulaM, position, uploaded) values(@id, @categoryId, @name, @description, @units, @decimals, @weight, @formulaF, @formulaM, @position, @uploaded)", test);
        }

        public void Delete(Test test)
        {
            var res = DatabaseUtility.getConnection().InsertSql("DELETE from tests where id=@id", test);
        }

        public IList<Test> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<Test>("SELECT * FROM tests");
        }

        public IList<Test> GetTestsByCategory(TestCategory cat)
        {
            return DatabaseUtility.getConnection().QuerySql<Test>("SELECT * FROM tests WHERE categoryId = @id", cat);
        }

        public void Update(Test test)
        {
            DatabaseUtility.getConnection().QuerySql<Test>("UPDATE tests SET categoryId=@categoryId, name=@name, description=@description, units=@units, decimals=@decimals, weight=@weight, formulaF=@formulaF, formulaM=@formulaM, updated=CURRENT_TIMESTAMP WHERE id=@id", test);
        }
    }

   public class Test
    {
        public byte[] id { get; set; }
        public byte[] categoryId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string units { get; set; }
        public int decimals { get; set; }
        public double weight { get; set; }
        public string formulaF { get; set; }
        public string formulaM { get; set; }
        public int position { get; set; }
        public string updated { get; set; }
        public string uploaded { get; set; }
    }


}

using System.Collections.Generic;
using Insight.Database;
using HealthReporter.Utilities;

namespace HealthReporter.Models
{
    class TestCategoryRepository
    {
        public void Insert(TestCategory testCategory)
        {
            var connection = DatabaseUtility.getConnection();
            testCategory.id = System.Guid.NewGuid().ToByteArray();
            var res = connection.InsertSql("INSERT INTO test_categories (id, parentId, name, position, uploaded) values(@id, @parentId, @name, @position, @uploaded)", testCategory);
        }

        public IList<TestCategory> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<TestCategory>("SELECT * FROM test_categories");
        }
    }

    class TestCategory
    {
        public byte[] id { get; set; }
        public byte[] parentId { get; set; }
        public string name { get; set; }
        public int position { get; set; }
        public string updated { get; set; }
        public string uploaded { get; set; }
    }


}

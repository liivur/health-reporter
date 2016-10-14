using System.Collections.Generic;
using Insight.Database;
using HealthReporter.Utilities;
using System;

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

        public IList<TestCategory> GetCategoryByTest(Test test)
        {
            return DatabaseUtility.getConnection().QuerySql<TestCategory>("SELECT * FROM test_categories WHERE id=@categoryId", test);
        }

        public IList<TestCategory> FindRootCategories()
        {
            return DatabaseUtility.getConnection().QuerySql<TestCategory>("SELECT * FROM test_categories WHERE parentId IS NULL");
        }

        public IList<TestCategory> GetCategoryByParent(TestCategory cat)
        {
            return DatabaseUtility.getConnection().QuerySql<TestCategory>("SELECT * FROM test_categories WHERE parentId = @id", cat);
        }
    }

    class TestCategory : IHasPrimaryKey
    {
        public byte[] GetPrimaryKey()
        {
            return this.id;
        }

        public string IdAsString()
        {
            string hex = BitConverter.ToString(this.id);
            return hex.Replace("-", "");
        }

        public byte[] id { get; set; }
        public byte[] parentId { get; set; }
        public string name { get; set; }
        public int position { get; set; }
        public string updated { get; set; }
        public string uploaded { get; set; }
    }


}

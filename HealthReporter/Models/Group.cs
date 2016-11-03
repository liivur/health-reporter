using HealthReporter.Utilities;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthReporter.Models
{
    public class GroupRepository
    {
        public void Insert(Group group)
        {
            var connection = Utilities.DatabaseUtility.getConnection();
            group.id = System.Guid.NewGuid().ToByteArray();
            var res = connection.InsertSql("INSERT INTO groups (id, name, updated) values(@id, @name, @updated)", group);
        }

        public IList<Group> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<Group>("SELECT * FROM groups");
        }

        public void Delete(Group group)
        {
            var connection = DatabaseUtility.getConnection();

            var res = connection.InsertSql("DELETE from groups where id=@id", group);

        }
        public void Update(Group group)
        {
            var connection = DatabaseUtility.getConnection();

            var res = connection.InsertSql("UPDATE groups set name='" + group.name + "', updated = CURRENT_TIMESTAMP WHERE id=@id", group);



        }

    }
    public class Group
    {
        public byte[] id { get; set; }
        public string name { get; set; }
        public string updated { get; set; }



    }
}

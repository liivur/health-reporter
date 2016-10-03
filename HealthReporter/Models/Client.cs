using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Insight.Database;
using HealthReporter.Utilities;
using System.Data.SQLite;

namespace HealthReporter.Models
{
    class ClientRepository
    {
        public void Insert(Client client)
        {
            var connection = DatabaseUtility.getConnection();
            client.id = System.Guid.NewGuid().ToByteArray();
            var res = connection.InsertSql("INSERT INTO clients (id, firstName, lastName, groupId, email, gender) values(@id, @firstName, @lastName, @groupId, @email, @gender)", client);
        }

        public IList<Client> FindAll()
        {
            return DatabaseUtility.getConnection().QuerySql<Client>("SELECT * FROM clients");
        }
    }

    class Client
    {
        public byte[] id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int groupId { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string updated { get; set; }
        public string uploaded { get; set; }
    }

    
}

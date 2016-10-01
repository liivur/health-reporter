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
        public void InsertClient(Client client)
        {
            var connection = DatabaseUtility.getConnection();
            client.id = System.Guid.NewGuid().ToByteArray();
            var res = connection.InsertSql("INSERT INTO clients (id, firstName, lastName) values(@id, @firstName, @lastName)", client);
        }
    }

    class Client
    {
        public byte[] id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    // comment for appveyor
    
}

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

        public void Delete(Client client)
        {
            var connection = DatabaseUtility.getConnection();         

            var res = connection.InsertSql("DELETE from clients where firstName='" + client.firstName + "'", client);
  
        }

        public void Update(Client client)
        {
            var connection = DatabaseUtility.getConnection();
            
            var res = connection.InsertSql("UPDATE clients set firstName='" + client.firstName + "', lastName='"+client.lastName + "' , updated = CURRENT_TIMESTAMP WHERE email='" + client.email + "'", client);


           // firstName, lastName, groupId, email, gender) values(@id, @firstName, @lastName, @groupId, @email, @gender)", client);
        }

        internal IList<Client> FindSearchResult(string searchBy)
        {
            return DatabaseUtility.getConnection().QuerySql<Client>("SELECT * FROM clients WHERE firstname LIKE '%"+searchBy+ "%' OR lastName LIKE'%" + searchBy + "%' OR groupID LIKE '%" + searchBy + "%'");
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

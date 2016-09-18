using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Insight.Database;
using System.Data.SQLite;
using HealthReporter.Models;
using System.IO;

namespace HealthReporter.Utilities
{
    class DatabaseUtility
    {
        static String dbName = "HealthReporter.sqlite";

        static public SQLiteConnectionStringBuilder getConnectionStringBuilder()
        {
            return new SQLiteConnectionStringBuilder() { DataSource = dbName };
        }

        static public void checkDb()
        {
            if (!File.Exists(dbName)) {
                createDatabase();
            }
        }

        private static void createDatabase()
        {
            SQLiteConnection dbConnection;
            SQLiteConnectionStringBuilder builder;
            SQLiteConnection.CreateFile(dbName);
            builder = getConnectionStringBuilder();
            dbConnection = new SQLiteConnection(builder.ConnectionString);

            dbConnection.ExecuteSql(ClientRepository.getInsertSql());
        }

        static public SQLiteConnection getConnection()
        {
            var connection = new SQLiteConnection(getConnectionStringBuilder().ConnectionString).OpenConnection();

            return connection;
        }
    }
}

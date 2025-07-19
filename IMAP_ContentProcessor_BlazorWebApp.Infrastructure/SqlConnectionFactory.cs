using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMAP_ContentProcessor_BlazorWebApp.Infrastructure
{
    public class SqlConnectionFactory
    {
        private readonly string connectionString;
        private IDbConnection? connection;

        public SqlConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }

            return connection;
        }

        public IDbConnection CreateNewConnection()
        {
            var newConnection = new MySqlConnection(connectionString);
            newConnection.Open();
            return newConnection;
        }

        public string GetConnectionString()
        {
            return connectionString;
        }

        public void Dispose()
        {
            if (this.connection != null && this.connection.State == ConnectionState.Open)
            {
                this.connection.Dispose();
            }
        }
    }
}

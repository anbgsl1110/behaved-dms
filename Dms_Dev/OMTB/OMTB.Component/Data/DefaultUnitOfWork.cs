using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace OMTB.Component.Data
{
    public class DefaultUnitOfWork : IUnitOfWork
    {
        public IDbConnection Connection { get; private set; }

        private Stack<IDbTransaction> transactions;

        public string ConnectionName { get; private set; }

        public DefaultUnitOfWork(string connectionString)
        {
            this.Connection = new MySqlConnection(connectionString);
            this.transactions = new Stack<IDbTransaction>();
        }

        public DefaultUnitOfWork(string connectionString, string connectionName)
        {
            this.ConnectionName = connectionName;
            this.Connection = new MySqlConnection(connectionString);
            this.transactions = new Stack<IDbTransaction>();
        }

        private void OpenConnection()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (this.Connection.State != ConnectionState.Closed)
            {
                this.Connection.Close();
            }
        }

        public void BeginTran()
        {
            OpenConnection();
            this.transactions.Push(this.Connection.BeginTransaction());
        }

        public void Commit()
        {
            if (this.transactions.Any())
            {
                OpenConnection();
                this.transactions.Pop().Commit();
                CloseConnection();
            }
        }

        public void Rollback()
        {
            if (this.transactions.Any())
            {
                OpenConnection();
                this.transactions.Pop().Rollback();
                CloseConnection();
            }
        }

        public IDbTransaction GetLastTransaction()
        {
            return this.transactions.LastOrDefault();
        }

        public void Dispose()
        {
            CloseConnection();
            this.Connection.Dispose();
            if (!string.IsNullOrEmpty(ConnectionName))
            {
                UnitOfWorkFactory.RebackUnitOfWork(ConnectionName);
            }
        }
    }
}
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Queued.Adapter.Storage.Context
{
    internal sealed class DbContext : IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction Transaction;

        public IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqliteConnection($"Data Source=database.sqlite;Cache=Shared");
                    _connection.Open();
                }
                return _connection;
            }
        }

        public IDbTransaction BeginTransaction()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            Transaction = Connection.BeginTransaction(IsolationLevel.ReadUncommitted);

            return Transaction;
        }

        #region Wrappers for Dapper extensions

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parms = null)
            => Connection.QueryAsync<T>(sql, parms);

        public Task<int> ExecuteAsync(
            string sql, object parms = null)
            => Connection.ExecuteAsync(sql, parms, Transaction);

        public Task<T> ExecuteScalarAsync<T>(
            string sql, object parms = null)
            => Connection.ExecuteScalarAsync<T>(sql, parms, Transaction);

        #endregion

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}

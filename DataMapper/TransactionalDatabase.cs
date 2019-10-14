using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace programmersdigest.DataMapper {
    public class TransactionalDatabase : IDatabase, IDisposable {
        private readonly ConnectionBuilder _connectionBuilder;
        private readonly DatabaseCore _core;
        private DbConnection _connection;
        private DbTransaction _transaction;

        internal TransactionalDatabase(ConnectionBuilder connectionBuilder, DatabaseCore core) {
            _core = core;
            _connectionBuilder = connectionBuilder;
        }

        internal async Task<TransactionalDatabase> Open() {
            if (_connection == null) {
                _connection = _connectionBuilder();
                await _connection.OpenAsync();

                _transaction = _connection.BeginTransaction();
            }

            return this;
        }
        
        public void Commit() {
            _transaction?.Commit();
            _transaction?.Dispose();
            _connection?.Dispose();

            _transaction = null;
            _connection = null;
        }

        public void Rollback() {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _connection?.Dispose();

            _transaction = null;
            _connection = null;
        }

        public void Dispose() {
            Rollback();
        }


        public async Task<IEnumerable<dynamic>> Select(string query) {
            return await Select(query, null);
        }

        public async Task<IEnumerable<dynamic>> Select(string query, IDictionary<string, object> keys) {
            return await _core.Select(_connection, query, keys);
        }

        public async Task<IEnumerable<T>> Select<T>(string query) {
            return await Select<T>(query, null);
        }

        public async Task<IEnumerable<T>> Select<T>(string query, IDictionary<string, object> keys) {
            return await _core.Select<T>(_connection, query, keys);
        }


        public async Task<dynamic> SelectSingle(string query) {
            return await SelectSingle(query, null);
        }

        public async Task<dynamic> SelectSingle(string query, IDictionary<string, object> keys) {
            var result = await _core.Select(_connection, query, keys);
            return result.SingleOrDefault();
        }

        public async Task<T> SelectSingle<T>(string query) {
            return await SelectSingle<T>(query, null);
        }

        public async Task<T> SelectSingle<T>(string query, IDictionary<string, object> keys) {
            var result = await _core.Select<T>(_connection, query, keys);
            return result.SingleOrDefault();
        }


        public async Task<long> Insert(string table, IDictionary<string, object> data) {
            return await _core.Insert(_connection, table, data);
        }

        public async Task<long> Insert<T>(T item) {
            return await _core.Insert(_connection, item);
        }


        public async Task Update(string table, IDictionary<string, object> data, IDictionary<string, object> keys) {
            await _core.Update(_connection, table, data, keys);
        }

        public async Task Update<T>(T item) {
            await _core.Update(_connection, item);
        }


        public async Task Delete(string table, IDictionary<string, object> keys) {
            await _core.Delete(_connection, table, keys);
        }

        public async Task Delete<T>(T item) {
            await _core.Delete(_connection, item);
        }


        public async Task Execute(string query) {
            await _core.Execute(_connection, query);
        }
    }
}

using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace programmersdigest.DataMapper {
    public class Database : IDatabase {
        private readonly DatabaseCore _core;
        private readonly ConnectionBuilder _connectionBuilder;

        public Database(ConnectionBuilder connectionBuilder, LastInsertIdSelector lastInsertIdSelector) {
            _core = new DatabaseCore(lastInsertIdSelector);
            _connectionBuilder = connectionBuilder;
        }

        protected async Task<DbConnection> OpenConnection() {
            var conn = _connectionBuilder();
            await conn.OpenAsync();
            return conn;
        }

        public async Task<TransactionalDatabase> BeginTransation() {
            return await new TransactionalDatabase(_connectionBuilder, _core).Open();
        }


        public async Task<IEnumerable<dynamic>> Select(string query) {
            return await Select(query, null);
        }

        public async Task<IEnumerable<dynamic>> Select(string query, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection()) {
                return await _core.Select(conn, query, keys);
            }
        }

        public async Task<IEnumerable<T>> Select<T>(string query) {
            return await Select<T>(query, null);
        }

        public async Task<IEnumerable<T>> Select<T>(string query, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection()) {
                return await _core.Select<T>(conn, query, keys);
            }
        }


        public async Task<dynamic> SelectSingle(string query) {
            return await SelectSingle(query, null);
        }

        public async Task<dynamic> SelectSingle(string query, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection()) {
                var result = await _core.Select(conn, query, keys);
                return result.SingleOrDefault();
            }
        }

        public async Task<T> SelectSingle<T>(string query) {
            return await SelectSingle<T>(query, null);
        }

        public async Task<T> SelectSingle<T>(string query, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection()) {
                var result = await _core.Select<T>(conn, query, keys);
                return result.SingleOrDefault();
            }
        }
        

        public async Task<long> Insert(string table, IDictionary<string, object> data) {
            using (var conn = await OpenConnection()) {
                return await _core.Insert(conn, table, data);
            }
        }

        public async Task<long> Insert<T>(T item) {
            using (var conn = await OpenConnection()) {
                return await _core.Insert(conn, item);
            }
        }


        public async Task Update(string table, IDictionary<string, object> data, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection()) {
                await _core.Update(conn, table, data, keys);
            }
        }

        public async Task Update<T>(T item) {
            using (var conn = await OpenConnection()) {
                await _core.Update(conn, item);
            }
        }


        public async Task Delete(string table, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection()) {
                await _core.Delete(conn, table, keys);
            }
        }

        public async Task Delete<T>(T item) {
            using (var conn = await OpenConnection()) {
                await _core.Delete(conn, item);
            }
        }
        

        public async Task Execute(string query) {
            using (var conn = await OpenConnection()) {
                await _core.Execute(conn, query);
            }
        }
    }
}

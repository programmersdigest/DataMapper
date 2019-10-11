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
            await conn.OpenAsync().ConfigureAwait(false);
            return conn;
        }

        public async Task<TransactionalDatabase> BeginTransation() {
            return await new TransactionalDatabase(_connectionBuilder, _core).Open().ConfigureAwait(false);
        }


        public async Task<IEnumerable<dynamic>> Select(string query) {
            return await Select(query, null).ConfigureAwait(false);
        }

        public async Task<IEnumerable<dynamic>> Select(string query, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                return await _core.Select(conn, query, keys).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<T>> Select<T>(string query) {
            return await Select<T>(query, null).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> Select<T>(string query, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                return await _core.Select<T>(conn, query, keys).ConfigureAwait(false);
            }
        }


        public async Task<dynamic> SelectSingle(string query) {
            return await SelectSingle(query, null).ConfigureAwait(false);
        }

        public async Task<dynamic> SelectSingle(string query, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                var result = await _core.Select(conn, query, keys).ConfigureAwait(false);
                return result.SingleOrDefault();
            }
        }

        public async Task<T> SelectSingle<T>(string query) {
            return await SelectSingle<T>(query, null).ConfigureAwait(false);
        }

        public async Task<T> SelectSingle<T>(string query, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                var result = await _core.Select<T>(conn, query, keys).ConfigureAwait(false);
                return result.SingleOrDefault();
            }
        }
        

        public async Task<long> Insert(string table, IDictionary<string, object> data) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                return await _core.Insert(conn, table, data).ConfigureAwait(false);
            }
        }

        public async Task<long> Insert<T>(T item) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                return await _core.Insert(conn, item).ConfigureAwait(false);
            }
        }


        public async Task Update(string table, IDictionary<string, object> data, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                await _core.Update(conn, table, data, keys).ConfigureAwait(false);
            }
        }

        public async Task Update<T>(T item) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                await _core.Update(conn, item).ConfigureAwait(false);
            }
        }


        public async Task Delete(string table, IDictionary<string, object> keys) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                await _core.Delete(conn, table, keys).ConfigureAwait(false);
            }
        }

        public async Task Delete<T>(T item) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                await _core.Delete(conn, item).ConfigureAwait(false);
            }
        }
        

        public async Task Execute(string query) {
            using (var conn = await OpenConnection().ConfigureAwait(false)) {
                await _core.Execute(conn, query).ConfigureAwait(false);
            }
        }
    }
}

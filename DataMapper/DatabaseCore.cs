using programmersdigest.DataMapper.Attributes;
using programmersdigest.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace programmersdigest.DataMapper {
    internal class DatabaseCore {
        private LastInsertIdSelector _lastInsertIdSelector;

        public DatabaseCore(LastInsertIdSelector lastInsertIdSelector) {
            _lastInsertIdSelector = lastInsertIdSelector;
        }

        internal async Task<IEnumerable<dynamic>> Select(DbConnection conn, string query, IDictionary<string, object> keys) {
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = query;
                cmd.AddParameters(keys);

                using (var reader = await cmd.ExecuteReaderAsync()) {
                    var results = new List<dynamic>();

                    while (await reader.ReadAsync()) {
                        IDictionary<string, object> item = new ExpandoObject();

                        for (var i = 0; i < reader.FieldCount; i++) {
                            var columnName = reader.GetName(i);
                            var type = reader.GetFieldType(i);
                            item[columnName] = reader.GetValue(i);
                        }

                        results.Add(item);
                    }

                    return results;
                }
            }
        }

        internal async Task<IEnumerable<T>> Select<T>(DbConnection conn, string query, IDictionary<string, object> keys) {
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = query;
                cmd.AddParameters(keys);

                var properties = typeof(T).GetColumnProperties();

                using (var reader = await cmd.ExecuteReaderAsync()) {
                    var results = new List<T>();

                    while (await reader.ReadAsync()) {
                        var item = Activator.CreateInstance<T>();

                        for (var i = 0; i < reader.FieldCount; i++) {
                            var columnName = reader.GetName(i);
                            var property = properties[columnName];
                            var value = reader.GetFieldValue(property.PropertyType, i);

                            property.SetValue(item, value);
                        }

                        results.Add(item);
                    }

                    return results;
                }
            }
        }

        internal async Task<long> Insert(DbConnection conn, string table, IDictionary<string, object> data) {
            using (var cmd = conn.CreateCommand()) {
                var columnsStr = string.Join(",", data.Keys);
                var paramsStr = string.Join(",", data.Keys.Select(c => $"@{c}"));

                cmd.CommandText = $"INSERT INTO \"{table}\" ({columnsStr}) VALUES ({paramsStr});";
                cmd.AddParameters(data);

                _lastInsertIdSelector?.Invoke(cmd);

                var result = await cmd.ExecuteScalarAsync();
                if (result is long) {
                    return (long)result;
                }
                else {
                    return -1;
                }
            }
        }

        internal async Task<long> Insert<T>(DbConnection conn, T item) {
            IDictionary<string, object> data = item.GetDataColumnValues();
            return await Insert(conn, item.GetName(), data);
        }

        internal async Task Update(DbConnection conn, string table, IDictionary<string, object> data, IDictionary<string, object> keys) {
            using (var cmd = conn.CreateCommand()) {
                var paramsStr = string.Join(",", data.Keys.Select(d => $"{d} = @{d}"));
                var whereStr = string.Join(" and ", keys.Keys.Select(p => $"{p} = @{p}"));

                cmd.CommandText = $"UPDATE \"{table}\" SET {paramsStr} WHERE {whereStr}";
                cmd.AddParameters(data);
                cmd.AddParameters(keys);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        internal async Task Update<T>(DbConnection conn, T item) {
            var data = item.GetDataColumnValues();
            var keys = item.GetPrimaryKeyColumnValues();
            if (!keys.Any()) {
                throw new ArgumentException($"The type {typeof(T).Name} does not define a primary key. Please annotate at least one property with the {typeof(PrimaryKeyAttribute).Name}.");
            }

            await Update(conn, item.GetName(), data, keys);
        }

        internal async Task Delete(DbConnection conn, string table, IDictionary<string, object> keys) {
            using (var cmd = conn.CreateCommand()) {
                var whereStr = string.Join(" and ", keys.Keys.Select(p => $"{p} = @{p}"));

                cmd.CommandText = $"DELETE FROM \"{table}\" WHERE {whereStr}";
                cmd.AddParameters(keys);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        internal async Task Delete<T>(DbConnection conn, T item) {
            IDictionary<string, object> keys = item.GetPrimaryKeyColumnValues();
            if (!keys.Any()) {
                throw new ArgumentException($"The type {typeof(T).Name} does not define a primary key. Please annotate at least one property with the {typeof(PrimaryKeyAttribute).Name}.");
            }

            await Delete(conn, item.GetName(), keys);
        }

        internal async Task Execute(DbConnection conn, string query) {
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = query;

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}

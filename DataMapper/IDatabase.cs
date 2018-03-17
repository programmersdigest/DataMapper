using System.Collections.Generic;
using System.Threading.Tasks;

namespace programmersdigest.DataMapper {
    public interface IDatabase {
        Task<IEnumerable<dynamic>> Select(string query);
        Task<IEnumerable<dynamic>> Select(string query, IDictionary<string, object> keys);
        Task<IEnumerable<T>> Select<T>(string query);
        Task<IEnumerable<T>> Select<T>(string query, IDictionary<string, object> keys);
        Task<dynamic> SelectSingle(string query);
        Task<dynamic> SelectSingle(string query, IDictionary<string, object> keys);
        Task<T> SelectSingle<T>(string query);
        Task<T> SelectSingle<T>(string query, IDictionary<string, object> keys);
        Task<long> Insert(string table, IDictionary<string, object> data);
        Task<long> Insert<T>(T item);
        Task Update(string table, IDictionary<string, object> data, IDictionary<string, object> keys);
        Task Update<T>(T item);
        Task Delete(string table, IDictionary<string, object> keys);
        Task Delete<T>(T item);
        Task Execute(string query);
    }
}

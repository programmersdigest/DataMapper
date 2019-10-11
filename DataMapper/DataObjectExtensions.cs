using programmersdigest.DataMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace programmersdigest.DataMapper {
    internal static class DataObjectExtensions {
        private static Type RetrieveType(object item) {
            if (item is Type type) {
                return type;
            }
            
            return item.GetType();            
        }

        private static IEnumerable<PropertyInfo> GetPrimaryKeys(Type type) {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.IsDefined(typeof(PrimaryKeyAttribute)))
                       .DefaultIfEmpty(
                            type.GetProperty("Id") ?? type.GetProperty($"{type.GetName()}Id")
                        );
        }

        public static string GetName(this object item) {
            var type = RetrieveType(item);
            return type.GetCustomAttribute<NameAttribute>()?.Name ?? type.Name;
        }


        public static IEnumerable<string> GetColumns(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Select(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name);
        }

        public static IEnumerable<string> GetPrimaryKeyColumns(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.IsDefined(typeof(PrimaryKeyAttribute)))
                       .Select(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name);
        }

        public static IEnumerable<string> GetForeignDataColumns(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.IsDefined(typeof(ForeignDataAttribute)))
                       .Select(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name);
        }

        public static IEnumerable<string> GetDataColumns(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => !p.IsDefined(typeof(PrimaryKeyAttribute)) && !p.IsDefined(typeof(ForeignDataAttribute)))
                       .Select(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name);
        }


        public static IDictionary<string, PropertyInfo> GetColumnProperties(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .ToDictionary(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name);
        }

        public static IDictionary<string, PropertyInfo> GetPrimaryKeyColumnProperties(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.IsDefined(typeof(PrimaryKeyAttribute)))
                       .ToDictionary(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name);
        }

        public static IDictionary<string, PropertyInfo> GetForeignDataColumnProperties(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.IsDefined(typeof(ForeignDataAttribute)))
                       .ToDictionary(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name);
        }

        public static IDictionary<string, PropertyInfo> GetDataColumnProperties(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => !p.IsDefined(typeof(PrimaryKeyAttribute)) && !p.IsDefined(typeof(ForeignDataAttribute)))
                       .ToDictionary(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name);
        }


        public static IDictionary<string, object> GetColumnValues(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .ToDictionary(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name,
                                     p => p.GetValue(item));
        }

        public static IDictionary<string, object> GetPrimaryKeyColumnValues(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.IsDefined(typeof(PrimaryKeyAttribute)))
                       .ToDictionary(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name,
                                     p => p.GetValue(item));
        }

        public static IDictionary<string, object> GetForeignDataColumnValues(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.IsDefined(typeof(ForeignDataAttribute)))
                       .ToDictionary(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name,
                                     p => p.GetValue(item));
        }

        public static IDictionary<string, object> GetDataColumnValues(this object item) {
            var type = RetrieveType(item);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => !p.IsDefined(typeof(PrimaryKeyAttribute)) && !p.IsDefined(typeof(ForeignDataAttribute)))
                       .ToDictionary(p => p.GetCustomAttribute<NameAttribute>()?.Name ?? p.Name,
                                     p => p.GetValue(item));
        }
    }
}

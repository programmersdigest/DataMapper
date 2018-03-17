using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace programmersdigest.DataMapper {
    internal static class DataExtensions {
        public static void AddParameters(this DbCommand command, IDictionary<string, object> data) {
            if (command == null || data == null)
                return;

            foreach (var kvp in data) {
                var parameter = command.CreateParameter();
                parameter.ParameterName = kvp.Key.StartsWith("@") ? kvp.Key : $"@{kvp.Key}";
                parameter.Value = kvp.Value;
                command.Parameters.Add(parameter);
            }
        }

        public static object GetFieldValue(this DbDataReader reader, Type targetType, int ordinal) {
            var types = new Dictionary<Type, Func<int, object>> {
                [typeof(bool)] = o => reader.GetBoolean(o),
                [typeof(bool?)] = o => reader.GetValue(o) is DBNull ? (bool?)null : reader.GetBoolean(o),
                [typeof(byte)] = o => reader.GetByte(o),
                [typeof(byte?)] = o => reader.GetValue(o) is DBNull ? (byte?)null : reader.GetByte(o),
                [typeof(char)] = o => reader.GetChar(o),
                [typeof(char?)] = o => reader.GetValue(o) is DBNull ? (char?)null : reader.GetChar(o),
                [typeof(DateTime)] = o => reader.GetDateTime(o),
                [typeof(DateTime?)] = o => reader.GetValue(o) is DBNull ? (DateTime?)null : reader.GetDateTime(o),
                [typeof(decimal)] = o => reader.GetDecimal(o),
                [typeof(decimal?)] = o => reader.GetValue(o) is DBNull ? (decimal?)null : reader.GetDecimal(o),
                [typeof(double)] = o => reader.GetDouble(o),
                [typeof(double?)] = o => reader.GetValue(o) is DBNull ? (double?)null : reader.GetDouble(o),
                [typeof(float)] = o => reader.GetFloat(o),
                [typeof(float?)] = o => reader.GetValue(o) is DBNull ? (float?)null : reader.GetFloat(o),
                [typeof(Guid)] = o => reader.GetGuid(o),
                [typeof(Guid?)] = o => reader.GetValue(o) is DBNull ? (Guid?)null : reader.GetGuid(o),
                [typeof(Int16)] = o => reader.GetInt16(o),
                [typeof(Int16?)] = o => reader.GetValue(o) is DBNull ? (Int16?)null : reader.GetInt16(o),
                [typeof(Int32)] = o => reader.GetInt32(o),
                [typeof(Int32?)] = o => reader.GetValue(o) is DBNull ? (Int32?)null : reader.GetInt32(o),
                [typeof(Int64)] = o => reader.GetInt64(o),
                [typeof(Int64?)] = o => reader.GetValue(o) is DBNull ? (Int64?)null : reader.GetInt64(o),
                [typeof(Stream)] = o => reader.GetValue(o) is DBNull ? null : reader.GetStream(o),
                [typeof(string)] = o => reader.GetValue(o) is DBNull ? null : reader.GetString(o),
                [typeof(TextReader)] = o => reader.GetValue(o) is DBNull ? null : reader.GetTextReader(o),
                [typeof(object)] = o => reader.GetValue(o) is DBNull ? null : reader.GetValue(o)
            };

            if (types.TryGetValue(targetType, out var mapping)) {
                return mapping(ordinal);
            }
            else if (targetType.IsEnum) {
                var value = reader.GetValue(ordinal);
                if (value is byte || value is int || value is long) {
                    return Enum.ToObject(targetType, value);
                }
                else if (value is string) {
                    var names = Enum.GetNames(targetType);
                    var values = Enum.GetValues(targetType);
                    for(var i = 0; i < names.Length; i++) {
                        if (names[i] == (string)value) {
                            return values.GetValue(i);
                        }
                    }
                }
            }

            return reader.GetValue(ordinal);
        }
    }
}

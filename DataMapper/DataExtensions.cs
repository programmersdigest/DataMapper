using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;

namespace programmersdigest.DataMapper {
    internal static class DataExtensions {

        private static Dictionary<Type, Func<int, DbDataReader, object>> _columnValueRetrieverFunctionByType = new Dictionary<Type, Func<int, DbDataReader, object>>
        {
            [typeof(bool)] = (index, reader) => reader.GetBoolean(index),
            [typeof(bool?)] = (index, reader) => reader.GetNullableBool(index),
            [typeof(byte)] = (index, reader) => reader.GetByte(index),
            [typeof(byte?)] = (index, reader) => reader.GetNullableByte(index),
            [typeof(char)] = (index, reader) => reader.GetChar(index),
            [typeof(char?)] = (index, reader) => reader.GetNullableChar(index),
            [typeof(DateTime)] = (index, reader) => reader.GetDateTime(index),
            [typeof(DateTime?)] = (index, reader) => reader.GetNullableDateTime(index),
            [typeof(decimal)] = (index, reader) => reader.GetDecimal(index),
            [typeof(decimal?)] = (index, reader) => reader.GetNullableDecimal(index),
            [typeof(double)] = (index, reader) => reader.GetDouble(index),
            [typeof(double?)] = (index, reader) => reader.GetNullableDouble(index),
            [typeof(float)] = (index, reader) => reader.GetFloat(index),
            [typeof(float?)] = (index, reader) => reader.GetNullableFloat(index),
            [typeof(Guid)] = (index, reader) => reader.GetGuid(index),
            [typeof(Guid?)] = (index, reader) => reader.GetNullableGuid(index),
            [typeof(short)] = (index, reader) => reader.GetInt16(index),
            [typeof(short?)] = (index, reader) => reader.GetNullableInt16(index),
            [typeof(int)] = (index, reader) => reader.GetInt32(index),
            [typeof(int?)] = (index, reader) => reader.GetNullableInt32(index),
            [typeof(long)] = (index, reader) => reader.GetInt64(index),
            [typeof(long?)] = (index, reader) => reader.GetNullableInt64(index),
            [typeof(Stream)] = (index, reader) => reader.GetNullableStream(index),
            [typeof(string)] = (index, reader) => reader.GetNullableString(index),
            [typeof(TextReader)] = (index, reader) => reader.GetNullableTextReader(index),
            [typeof(object)] = (index, reader) => reader.GetNullableValue(index)
        };

        public static void AddParameters(this DbCommand command, IDictionary<string, object> data) {
            if (command == null || data == null)
            {
                return;
            }                

            foreach (var kvp in data) {
                var parameter = command.CreateParameter();
                parameter.ParameterName = kvp.Key.StartsWith("@") ? kvp.Key : $"@{kvp.Key}";
                parameter.Value = kvp.Value;
                command.Parameters.Add(parameter);
            }
        }

        public static object GetFieldValue(this DbDataReader reader, Type targetType, int ordinal)
        {
            if (_columnValueRetrieverFunctionByType.TryGetValue(targetType, out var mapping))
            {
                return mapping(ordinal, reader);
            }

            if (!targetType.IsEnum)
            {
                return reader.GetValue(ordinal);
            }

            var value = reader.GetValue(ordinal);

            if(TryGetEnumValue(value, targetType, out var enumValue))
            {
                return enumValue;
            }

            return value;
        }

        private static bool TryGetEnumValue(object value, Type targetType, out object enumValue)
        {
            if (value is byte || value is int || value is long)
            {
                enumValue = Enum.ToObject(targetType, value);
                return true;
            }

            if (value is string enumMemberName)
            {
                var names = Enum.GetNames(targetType);
                var values = Enum.GetValues(targetType);
                for (var i = 0; i < names.Length; i++)
                {
                    if (names[i] == enumMemberName)
                    {
                        enumValue = values.GetValue(i);
                        return true;
                    }
                }
            }

            enumValue = null;
            return false;
        }
    }
}

using System;
using System.Data.Common;
using System.IO;

namespace programmersdigest.DataMapper
{
    internal static class DataReaderExtensions
    {
        internal static bool? GetNullableBool(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (bool?) null : reader.GetBoolean(index);
        }

        internal static byte? GetNullableByte(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (byte?) null : reader.GetByte(index);
        }

        internal static char? GetNullableChar(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (char?) null : reader.GetChar(index);
        }

        internal static DateTime? GetNullableDateTime(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (DateTime?) null : reader.GetDateTime(index);
        }

        internal static decimal? GetNullableDecimal(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (decimal?) null : reader.GetDecimal(index);
        }

        internal static float? GetNullableFloat(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (float?) null : reader.GetFloat(index);
        }

        internal static double? GetNullableDouble(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (double?) null : reader.GetDouble(index);
        }

        internal static Guid? GetNullableGuid(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (Guid?) null : reader.GetGuid(index);
        }

        internal static short? GetNullableInt16(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (short?) null : reader.GetInt16(index);
        }

        internal static int? GetNullableInt32(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (int?) null : reader.GetInt32(index);
        }

        internal static long? GetNullableInt64(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? (long?) null : reader.GetInt64(index);
        }

        internal static string GetNullableString(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? null : reader.GetString(index);
        }

        internal static Stream GetNullableStream(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? null : reader.GetStream(index);
        }

        internal static TextReader GetNullableTextReader(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? null : reader.GetTextReader(index);
        }

        internal static object GetNullableValue(this DbDataReader reader, int index)
        {
            return reader.IsDBNull(index) ? null : reader.GetValue(index);
        }
    }
}

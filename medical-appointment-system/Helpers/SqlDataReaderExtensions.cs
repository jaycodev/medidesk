using System;
using System.Data.SqlClient;

namespace medical_appointment_system.Helpers
{
    public static class SqlDataReaderExtensions
    {
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static string SafeGetString(this SqlDataReader reader, string column)
        {
            return reader.HasColumn(column) && reader[column] != DBNull.Value
                ? reader[column].ToString()
                : string.Empty;
        }

        public static int SafeGetInt(this SqlDataReader reader, string column)
        {
            return reader.HasColumn(column) && reader[column] != DBNull.Value
                ? Convert.ToInt32(reader[column])
                : 0;
        }

        public static DateTime SafeGetDateTime(this SqlDataReader reader, string column)
        {
            return reader.HasColumn(column) && reader[column] != DBNull.Value
                ? Convert.ToDateTime(reader[column])
                : DateTime.MinValue;
        }

        public static bool SafeGetBool(this SqlDataReader reader, string column)
        {
            return reader.HasColumn(column) && reader[column] != DBNull.Value
                ? Convert.ToBoolean(reader[column])
                : false;
        }

        public static TimeSpan SafeGetTimeSpan(this SqlDataReader reader, string column)
        {
            return reader.HasColumn(column) && reader[column] != DBNull.Value
                ? (TimeSpan)reader[column]
                : TimeSpan.Zero;
        }
    }
}
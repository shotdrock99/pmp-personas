using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ModernizacionPersonas.DAL
{
    public static class SqlReaderUtilities
    {
        public static T SafeGet<T>(this SqlDataReader reader, string columnName)
        {
            var indexOfColumn = reader.GetOrdinal(columnName);
            return reader.IsDBNull(indexOfColumn) ? default(T) : reader.GetFieldValue<T>(indexOfColumn);
        }
    }
}

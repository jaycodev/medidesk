using Api.Attributes;
using Api.Queries;
using Microsoft.Data.SqlClient;

namespace Api.Extensions;

public static class SqlCommandExtensions
{
    public static void AddQueryAsParameters(this SqlParameterCollection collection, BaseQuery query)
    {
        var props = query.GetType().GetProperties();

        foreach (var property in props)
        {
            var sqlFilterParater = (SqlFilterParameter)property.GetCustomAttributes(true)
                .Where(x => x.GetType() == typeof(SqlFilterParameter))
                .First();

            var propValue = property.GetValue(query, null);

            collection.AddWithValue(string.Concat(["@", sqlFilterParater.Name]), propValue);
        }
    }
}

using Api.Attributes;
using Api.Queries;

namespace Api.Tests.Extensions;

public class SqlCommandExtensionsTests
{
    [Fact]
    public void AddQueryAsParameterToSqlCommandSuccessfully_Test()
    {
        var collection = new Dictionary<string, object>();
        var appointementQuery = new AppointmentQuery()
        {
            DoctorId = 1,
            Status = "confirmed"
        };

        ExtensionTestClass.AddQueryAsParameters(collection, appointementQuery);

        Assert.True(collection.ContainsKey("doctor_id"));
        Assert.True(collection.ContainsKey("status"));
    }
}

public static class ExtensionTestClass
{
    public static void AddQueryAsParameters(this Dictionary<string, object> collection, BaseQuery query)
    {
        var props = query.GetType().GetProperties();

        foreach (var property in props)
        {
            var sqlFilterParater = (SqlFilterParameter)property.GetCustomAttributes(true)
                .Where(x => x.GetType() == typeof(SqlFilterParameter))
                .First();

            var propValue = property.GetValue(query, null);

            collection.Add(sqlFilterParater.Name, property);
        }
    }
}

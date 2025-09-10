using Microsoft.AspNetCore.Mvc;

namespace Api.Queries;

public class ListQuery
{
    [FromQuery(Name = Constants.Limit)]
    public int Limit { get; set; }

    [FromQuery(Name = Constants.Offset)]
    public int Offset { get; set; }
}

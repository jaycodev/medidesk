using Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Api.Queries;

public class ListQuery : BaseQuery
{
    [FromQuery(Name = Constants.Limit)]
    [SqlFilterParameter(Constants.Limit)]
    public int Limit { get; set; }

    [FromQuery(Name = Constants.Offset)]
    [SqlFilterParameter(Constants.Offset)]
    public int Offset { get; set; }
}

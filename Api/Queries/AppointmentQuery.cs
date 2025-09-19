using Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Api.Queries;

public class AppointmentQuery : BaseQuery
{
    [FromQuery(Name = "userId")]
    [SqlFilterParameter("user_id")]
    public int? UserId { get; set; }

    [FromQuery(Name = "userRol")]
    [SqlFilterParameter("user_rol")]
    public string? UserRole { get; set; }

    [FromQuery(Name = "status")]
    [SqlFilterParameter("status")]
    public string? Status { get; set; }

    [FromQuery(Name = "doctorId")]
    [SqlFilterParameter("doctor_id")]
    public int? DoctorId { get; set; }

    [FromQuery(Name = "date")]
    [SqlFilterParameter("date")]
    public DateTime? Date { get; set; }
}

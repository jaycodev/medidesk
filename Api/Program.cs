using Api.Data.Contract;
using Api.Data.Repository;
using Api.Domains.Doctors.Repositories;
using Api.Domains.Specialties.Repositories;
using Api.Domains.Users.Repository;
using Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGenericContract<Schedule>, ScheduleRepository>();
builder.Services.AddScoped<IGenericContract<Patient>, PatientRepository>();
builder.Services.AddScoped<IGenericContract<Appointment>, AppointmentRepository>();
builder.Services.AddScoped<IGenericContract<Notification>, NotificationRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

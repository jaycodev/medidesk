using Api.Data.Contract;
using Api.Data.Repository;
using Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGenericContract<Doctor>, DoctorRepository>();
builder.Services.AddScoped<IGenericContract<Specialty>, SpecialtyRepository>();
builder.Services.AddScoped<IGenericContract<Schedule>, ScheduleRepository>();
builder.Services.AddScoped<IGenericContract<Patient>, PatientRepository>();

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

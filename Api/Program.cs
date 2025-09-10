using Api.Repositories.Account;
using Api.Repositories.Appointments;
using Api.Repositories.Cloudinary;
using Api.Repositories.Doctors;
using Api.Repositories.Notifications;
using Api.Repositories.Patients;
using Api.Repositories.Schedules;
using Api.Repositories.Specialties;
using Api.Repositories.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddJsonFile("cloudinary.json", optional: true, reloadOnChange: true);

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                       ?? builder.Configuration.GetConnectionString("DB");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string 'DB' or environment variable 'DATABASE_URL' was not found.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddScoped<IAccountRepository>(sp => new AccountRepository(connectionString));
builder.Services.AddScoped<IAppointmentRepository>(sp => new AppointmentRepository(connectionString));
builder.Services.AddScoped<IDoctorRepository>(sp => new DoctorRepository(connectionString));
builder.Services.AddScoped<INotificationRepository>(sp => new NotificationRepository(connectionString));
builder.Services.AddScoped<IPatientRepository>(sp => new PatientRepository(connectionString));
builder.Services.AddScoped<IScheduleRepository>(sp => new ScheduleRepository(connectionString));
builder.Services.AddScoped<ISpecialtyRepository>(sp => new SpecialtyRepository(connectionString));
builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(connectionString));
builder.Services.AddScoped<ICloudinaryRepository, CloudinaryRepository>();

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

app.Run();

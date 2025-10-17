using Web.Filters;
using Web.Services.Account;
using Web.Services.Appointment;
using Web.Services.Doctor;
using Web.Services.Notification;
using Web.Services.Patient;
using Web.Services.Profile;
using Web.Services.Schedule;
using Web.Services.Specialty;
using Web.Services.User;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuthenticatedAttribute>();
});

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient("ApiClient", client =>
{
    var apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
                     ?? "https://localhost:7276/";

    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();
    if (path != null && path.EndsWith("/index"))
    {
        var newPath = path.Replace("/index", "");
        context.Response.Redirect(newPath);
        return;
    }
    await next();
});

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Appointments}/{action=Home}/{id?}");

app.MapControllerRoute(
    name: "controllerOnly",
    pattern: "{controller}",
    defaults: new { action = "Index" });

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

app.Run();

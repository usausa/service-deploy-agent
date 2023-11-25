using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting.WindowsServices;

using Serilog;

using Service.Deploy.Agent.Managers;
using Service.Deploy.Agent.Settings;

#pragma warning disable CA1852

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

// Configure builder
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
});

// Service
builder.Host
    .UseWindowsService()
    .UseSystemd();

// Config
builder.Configuration.SetBasePath(AppContext.BaseDirectory);
builder.Configuration.AddJsonFile("services.json", optional: false, reloadOnChange: true);

// Log
builder.Logging.ClearProviders();
builder.Services.AddSerilog(option =>
{
    option.ReadFrom.Configuration(builder.Configuration);
});

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<KestrelServerOptions>(static options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<RouteOptions>(static options =>
{
    options.AppendTrailingSlash = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ServiceSetting>(builder.Configuration.GetSection("Service"));

// Manager
if (OperatingSystem.IsWindows())
{
    builder.Services.AddSingleton<IServiceManager, WindowsServiceManager>();
}
else
{
    builder.Services.AddSingleton<IServiceManager, SystemdServiceManager>();
}

// Configure
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/", context => context.Response.WriteAsync("Deploy agent"));

app.Run();

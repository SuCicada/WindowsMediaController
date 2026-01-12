using Microsoft.Win32;
using Serilog;
using WindowsMediaController.Services;
using WindowsMediaController.Middleware;

// Mutex for Single Instance
const string MutexName = "Global\\WindowsMediaController_Mutex";
using var mutex = new Mutex(true, MutexName, out bool createdNew);

if (!createdNew)
{
    // App is already running
    return;
}

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File(builder.Configuration["AppConfig:LogFile"]?.Replace("log-.txt", $"log-{DateTime.Now:yyyyMMdd}.txt") ?? "logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// AutoStart Logic
bool autoStart = builder.Configuration.GetValue<bool>("AppConfig:AutoStart");
SetAutoStart(autoStart);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Windows Media Controller API",
        Version = "v1",
        Description = "Windows 媒体控制器 API - 提供媒体播放控制和音量控制功能",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Windows Media Controller",
            Url = new Uri("https://github.com/yourusername/WindowsMediaController")
        }
    });
    
    // 包含 XML 注释（如果有的话）
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddSingleton<IVolumeService, VolumeService>();
builder.Services.AddSingleton<IMediaService, MediaService>();

// WinForms for System Tray
builder.Services.AddSingleton<NotifyIconService>();

// Configure Kestrel Port
int port = builder.Configuration.GetValue<int>("AppConfig:Port");
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(port);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// 启用 Swagger/OpenAPI 文档（所有环境）
// Swagger 中间件应该在路由之前
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Windows Media Controller API v1");
    options.RoutePrefix = "swagger"; // 设置路径为 /swagger
    options.DisplayRequestDuration(); // 显示请求耗时
});

// 异常处理中间件应该在路由之前
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 路由和端点配置
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Start System Tray in a separate thread
var notifyIconService = app.Services.GetRequiredService<NotifyIconService>();
var thread = new Thread(() => notifyIconService.Initialize());
thread.SetApartmentState(ApartmentState.STA);
thread.Start();

app.Run();


void SetAutoStart(bool enable)
{
    const string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
    const string appName = "WindowsMediaController";
    
    try
    {
        using var key = Registry.CurrentUser.OpenSubKey(runKey, true);
        if (key == null) return;

        if (enable)
        {
            string? exePath = Environment.ProcessPath;
            if (exePath != null)
            {
                 key.SetValue(appName, exePath);
            }
        }
        else
        {
            key.DeleteValue(appName, false);
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to set auto-start registry key.");
    }
}

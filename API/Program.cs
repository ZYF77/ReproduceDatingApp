using API.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

//创建Serilog logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    //替换log
    builder.Host.UseSerilog();

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddCors();
    builder.Services.AddMemoryCache(); // 添加内存缓存服务

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer
        (builder.Configuration.GetConnectionString("DefaultConnection")));

    //强类型配置
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    //第一步全局异常处理中间件

    app.UseCors(policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://localhost:4200", "https://localhost:4200"));


    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "HostAbortedException")
{

    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();// 确保所有日志在应用关闭时被写入
}



using System.Text;
using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddMemoryCache(); // 添加内存缓存服务

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer
        (builder.Configuration.GetConnectionString("DefaultConnection")));
    
    
    //强类型配置
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


    //配置identity
    builder.Services.AddIdentityCore<AppUser>(opt =>
    {
        opt.Password.RequireNonAlphanumeric = false;
        opt.User.RequireUniqueEmail = true;
    }).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

    //注册jwt认证，添加认证服务
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() 
            ?? throw new Exception("Token key not found - Program.cs");
        var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

        //配置如何检查签名
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    context.Response.Headers.Append("Token-Expired", "true");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Information("Token validated for user: {UserName}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
        

    });
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    //第一步全局异常处理中间件
    app.UseGlobalExceptionHandling();

    app.UseCors(policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://localhost:4200", "https://localhost:4200"));

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();



    #region 填充测试数据
    //创建一个新的依赖注入作用域
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        //从作用域中获取 DbContext 实例
        var context = services.GetRequiredService<AppDbContext>();//服务定位器模式
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var logger = services.GetRequiredService<ILogger<Seed>>();
        //异步地执行数据库迁移
        await context.Database.MigrateAsync();
        await Seed.SeedUsers(userManager, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration");
    }

    #endregion
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



using COLLECTION_MANAGEMENT_API.Extensions;
using COLLECTION_MANAGEMENT_API.Filters;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

var env_name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configFilename = $"appsettings.{env_name}.json";
configFilename = configFilename.Replace("..", ".");
var Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(configFilename, optional: false, reloadOnChange: true)
            .Build();

string? logpath = Configuration["FilePaths:LogFileRootDirectory"];
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog((hostContext, services, configuration) =>
{
    configuration.Enrich.FromLogContext()
   .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
   .WriteTo.Console(theme: AnsiConsoleTheme.Literate, outputTemplate: "[{Timestamp:HH:mm:ss:fff}] [{Level:u3}] {request_id} {Message:lj}{NewLine}{Exception}")
   .WriteTo.Debug(outputTemplate: "[{Timestamp:HH:mm:ss:fff}] [{Level:u3}] {request_id} {Message:lj}{NewLine}{Exception}")
   .WriteTo.File(logpath + "logs.log", rollingInterval: RollingInterval.Hour, outputTemplate: "[{Timestamp:HH:mm:ss:fff}] [{Level:u3}] {request_id} {Message:lj}{NewLine}{Exception}", retainedFileCountLimit: null);
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestBody | HttpLoggingFields.ResponseBody;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

bool url_browsing_enabled = Configuration.GetValue<bool>("ApplicationConfiguration:URLBrowsingEnabled");
if (url_browsing_enabled)
{
    builder.WebHost.UseUrls(Configuration.GetValue<string>("ApplicationConfiguration:ApplicationURL") ?? string.Empty);
}

builder.Services.AddRazorPages();
builder.Services.AddCors();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", builder =>
//        builder.WithOrigins("http://localhost:4200")
//               .AllowAnyHeader()
//               .AllowAnyMethod()
//               .AllowCredentials());
//});
builder.Services.AddHttpClient();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
// Add services to the container.

builder.Services.AddDbContext<am_dbcontext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection")), options => options.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null)
));

builder.Services.AddDbContext<identity_dbcontext>((sp, options) =>
    options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection")), (options) =>
        {
            options.EnableStringComparisonTranslations();
        }));


builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<identity_dbcontext>()
    .AddDefaultTokenProviders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AM API", Version = "v1" });
    c.OperationFilter<SwaggerFileUploadOperationFilter>();
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
});

builder.Services.AddMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddAuthentication(options =>
{
    // Disable automatic redirect to login page
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JWT:Key") ?? string.Empty)),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = JwtRegisteredClaimNames.Sub
    };

    //x.Events = new JwtBearerEvents
    //{
    //    OnAuthenticationFailed = context =>
    //    {
    //        // Log the failure to identify any issues with the token
    //        Console.WriteLine("Authentication failed: {0}", context.Exception);
    //        return Task.CompletedTask;
    //    },
    //    OnTokenValidated = context =>
    //    {
    //        // This event is triggered after the token has been validated successfully
    //        var claims = context.Principal?.Claims;
    //        var jwtToken = context.SecurityToken as JwtSecurityToken;

    //        // Log the token's claims (e.g., user info, roles, etc.)
    //        Console.WriteLine("Token validated successfully!");
    //        foreach (var claim in claims)
    //        {
    //            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
    //        }
    //        Console.WriteLine($"JWT Token: {jwtToken?.RawData}");

    //        return Task.CompletedTask;
    //    },
    //    OnChallenge = context =>
    //    {
    //        Console.WriteLine("OnChallenge triggered");
    //        context.HandleResponse(); //Prevents double write
    //        context.Response.StatusCode = 401;
    //        context.Response.ContentType = "application/json";
    //        return context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
    //    }
    //};
});
//builder.Services.AddScoped<MenuAuthorizationFilter>();
builder.Services.AddAuthorization();
builder.Services.AddDataAccessServices();
builder.Services.AddBusinessLogicServices();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var result = new ValidationFailedResult(context.ModelState, context.HttpContext);
        result.ContentTypes.Add(MediaTypeNames.Application.Json);
        result.ContentTypes.Add(MediaTypeNames.Application.Xml);
        return result;
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

// Configure static files with explicit physical file provider
#region enable if deployed SPA: Angular/React/Vue
//var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
//if (Directory.Exists(wwwrootPath))
//{
//    app.UseStaticFiles(new StaticFileOptions
//    {
//        FileProvider = new PhysicalFileProvider(wwwrootPath)
//    });

//    // Configure default files (index.html)
//    app.UseDefaultFiles(new DefaultFilesOptions
//    {
//        DefaultFileNames = new List<string> { "index.html" },
//        FileProvider = new PhysicalFileProvider(wwwrootPath)
//    });

//    // Configure SPA fallback for client-side routing
//    app.MapFallbackToFile("index.html");
//} 
#endregion

app.UseStaticFiles();
app.UseDefaultFiles();

app.UseCors(x => x
//.WithOrigins("http://localhost:4200")
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader()
//.AllowCredentials()
);
//app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseSerilogRequestLogging();
app.MapRazorPages();
app.UseHttpLogging();
//app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();

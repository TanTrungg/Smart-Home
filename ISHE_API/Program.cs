using Hangfire;
using ISHE_API.Configurations;
using ISHE_Data.Entities;
using ISHE_Data.Mapping;
using ISHE_Utility.Settings;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";



builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSetting"));
builder.Services.AddDbContext<SMART_HOME_DBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    }
);
builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.WithOrigins(
                              "http://localhost:8100",
                              "https://localhost",
                                  "http://127.0.0.1:5173",
                                  "http://127.0.0.1:5174",
                                  "http://localhost:5174",
                                  "http://localhost:5173",
                                  "http://192.168.56.1:8100",
                                  "http://192.168.118.2:8100",
                                  "https://phatdat-store.web.app",
                                  "https://phatdat-teller.web.app");
                          policy.AllowCredentials();
                      });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddDependenceInjection();
builder.Services.AddAutoMapper(typeof(GeneralProfile));

//HangFire
builder.Services.AddHangfireServices(builder.Configuration);

var app = builder.Build();

//HangFire
app.AddHangfireDashboard();
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
app.Services.AddHangfireJobs(recurringJobManager);

app.UseCors(MyAllowSpecificOrigins);

app.UseSwagger();
app.UseSwaggerUI();

app.UseJwt();
app.UseExceptionHandling();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

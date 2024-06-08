using Hangfire;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using ISHE_API.Configurations.Middleware;
using ISHE_Data;
using ISHE_Service.Implementations;
using ISHE_Service.Interfaces;
using ISHE_Utility.Settings;
using Microsoft.OpenApi.Models;

namespace ISHE_API.Configurations
{
    public static class AppConfiguration
    {
        public static void AddDependenceInjection(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<ICloudStorageService, CloudStorageService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<ISmartDeviceService, SmartDeviceService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IDevicePackageService, DevicePackageService>();
            services.AddScoped<ISurveyRequestService, SurveyRequestService>();
            services.AddScoped<ITellerService, TellerService>();
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IFeedbackDevicePackageService, FeedbackDevicePackageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDeviceTokenService, DeviceTokenService>();
            services.AddScoped<IContractModificationService, ContractModificationService>();
            services.AddScoped<ISendMailService, SendMailService>();

            services.AddScoped<IPaymentService, PaymentService>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ISHE Service Interface",
                    Description = @"APIs for Application to management system and installing equipment for households at Phat Dat store in Ho Chi Minh City.
                        <br/>
                        <br/>
                        <strong>Web app for customer:</strong> <a href='https://phatdat-store.web.app' target='_blank'>https://phatdat-store.web.app</a>
                        <br/>
                        <strong>Web app for teller:</strong> <a href='https://phatdat-teller.web.app' target='_blank'>https://phatdat-teller.web.app</a>",
                    Version = "v1"
                });
                c.DescribeAllParametersInCamelCase();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Use the JWT Authorization header with the Bearer scheme. Enter 'Bearer' followed by a space, then your token.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                 });
                c.EnableAnnotations();
            });
        }
        public static void UseJwt(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }

        public static void UseExceptionHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }


        //HANGFIRE
        public static void AddHangfireDashboard(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire/job-dashboard", new DashboardOptions
            {
                DashboardTitle = "Hangfire Job of Phat Dat store server",
                DarkModeEnabled = true,
                Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = "admin",
                        Pass = "admin.@.@"
                    }
                }
            });
        }

        public static void AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    });
            });

            services.AddHangfireServer();
        }

        public static void AddHangfireJobs(this IServiceProvider serviceProvider, IRecurringJobManager recurringJobManager)
        {
            // Đăng ký công việc định kỳ với Hangfire sử dụng factory delegate
            recurringJobManager.AddOrUpdate(
                "CheckExpired",
                () => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IPromotionService>().CheckExpiredPromotion(),
                "0 17 * * *"
            );

            recurringJobManager.AddOrUpdate(
                "CheckActive",
                () => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IPromotionService>().CheckActivePromotion(),
                "0 17 * * *"
            );
        }
    }
}

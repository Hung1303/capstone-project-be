using API.Filters;
using Core.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Context;
using Repository;
using Repository.Interfaces;
using Services;
using Services.Interfaces;

namespace API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnectionString");
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ISyllabusService, SyllabusService>();
            services.AddScoped<ILessonPlanService, LessonPlanService>();
            services.AddScoped<IClassScheduleService, ClassScheduleService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ISuspensionService, SuspensionService>();
            services.AddScoped<ITeacherFeedbackService, TeacherFeedbackService>();
            services.AddScoped<ICourseFeedbackService, CourseFeedbackService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IApprovalRequestService, ApprovalRequestService>();
            services.AddScoped<IGeneratedReportService, GeneratedReportService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ICenterVerificationService, CenterVerificationService>();

            return services;
        }

        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // ---- Chính sách riêng cho từng Role ----
                options.AddPolicy("Admin", policy =>
                    policy.RequireClaim("Role", UserRole.Admin.ToString()));

                options.AddPolicy("Center", policy =>
                    policy.RequireClaim("Role", UserRole.Center.ToString()));

                options.AddPolicy("Teacher", policy =>
                    policy.RequireClaim("Role", UserRole.Teacher.ToString()));

                options.AddPolicy("Student", policy =>
                    policy.RequireClaim("Role", UserRole.Student.ToString()));

                options.AddPolicy("Parent", policy =>
                    policy.RequireClaim("Role", UserRole.Parent.ToString()));

                options.AddPolicy("Inspector", policy =>
                    policy.RequireClaim("Role", UserRole.Inspector.ToString()));

                // ---- Chính sách kết hợp (multi-role access) ----

                // Admin và Center có quyền quản lý hệ thống / khóa học
                options.AddPolicy("AdminOrCenterAccess", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var roleClaim = context.User.FindFirst("Role")?.Value;
                        return roleClaim == UserRole.Admin.ToString()
                            || roleClaim == UserRole.Center.ToString();
                    }));

                // Center, Teacher, và Admin có quyền giảng dạy và tạo nội dung
                options.AddPolicy("TeachingAccess", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var roleClaim = context.User.FindFirst("Role")?.Value;
                        return roleClaim == UserRole.Teacher.ToString()
                            || roleClaim == UserRole.Center.ToString()
                            || roleClaim == UserRole.Admin.ToString();
                    }));

                // Student, Parent, và Inspector có thể truy cập thông tin khóa học
                options.AddPolicy("ViewCoursesAccess", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var roleClaim = context.User.FindFirst("Role")?.Value;
                        return roleClaim == UserRole.Student.ToString()
                            || roleClaim == UserRole.Parent.ToString()
                            || roleClaim == UserRole.Inspector.ToString()
                            || roleClaim == UserRole.Admin.ToString();
                    }));

                // Inspector hoặc Admin có thể kiểm tra khóa học
                options.AddPolicy("InspectionAccess", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var roleClaim = context.User.FindFirst("Role")?.Value;
                        return roleClaim == UserRole.Inspector.ToString()
                            || roleClaim == UserRole.Admin.ToString();
                    }));
            });

            return services;
        }

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.  
                                    Enter 'Bearer' [space] and then your token in the text input below.
                                    Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddJwtBearer(item =>
            {
                item.RequireHttpsMetadata = false;
                item.SaveToken = true;
                item.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("c2VydmVwZXJmZWN0bHljaGVlc2VxdWlja2NvYWNoY29sbGVjdHNsb3Bld2lzZWNhbWU=")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                };
            });

            return services;
        }

        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            return services;
        }

        public static IServiceCollection AddControllersWithFilters(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<AuditLogActionFilter>();
            });

            return services;
        }
    }
}



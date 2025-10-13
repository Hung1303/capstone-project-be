using Core.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Context;
using Repository;
using Repository.Interfaces;
using Services;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
// 1. Authorization policies cho từng Role và nhóm quyền
builder.Services.AddAuthorization(options =>
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


// 2. Swagger config (bảo mật JWT Bearer)
builder.Services.AddSwaggerGen(options =>
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

// 3. Authentication (Cookie + Google + JWT Bearer)
builder.Services.AddAuthentication(options =>
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
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ISuspensionService, SuspensionService>();
builder.Services.AddEndpointsApiExplorer();

// CORS policy (tighten AllowedOrigins as needed for your frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

// Bind to Render's dynamic PORT if provided
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
var enableSwagger = app.Environment.IsDevelopment() ||
                    string.Equals(Environment.GetEnvironmentVariable("ENABLE_SWAGGER"), "true", StringComparison.OrdinalIgnoreCase);
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Respect reverse proxy headers (Render)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Enable HTTPS redirection only when an HTTPS port is configured
var httpsPorts = Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORTS");
if (!string.IsNullOrWhiteSpace(httpsPorts))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("Default");

app.MapControllers();

// Auto-apply EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate();
}

app.Run();

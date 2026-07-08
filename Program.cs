using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Services;
using StudentManagement.Repositories;
using StudentManagement.Services.Auth;
using StudentManagement;
using StudentManagement.Domain.Repositories;
using StudentManagement.Services;
using StudentManagement.Domain.Models;
using StudentManagement.Constants;


var builder = WebApplication.CreateBuilder(args);
// --- Identity ---
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// --- JWT Authentication ---
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.Zero  // no grace period after token expires
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();



builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRepository<Student>, Repository<Student>>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();


// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();

app.MapControllers();


// --- Seed roles on startup ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in Roles.All)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

app.UseAuthentication(); 

app.UseAuthorization();

app.Run();
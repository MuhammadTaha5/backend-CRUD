using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Services;
using MyFirstAPI.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddOpenApi();

// builder.Services.AddSingleton<ProductService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();

app.MapControllers();

app.Run();
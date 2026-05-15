using MyFirstAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddOpenApi();

// builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<StudentService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<ILogService, LogService>();
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
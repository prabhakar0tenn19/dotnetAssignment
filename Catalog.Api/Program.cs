var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Swagger endpoints validation aur swagger metadata documentation generator service register kar rahe hain
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// HTTP request pipeline configure ho raha hai
if (app.Environment.IsDevelopment())
{
    // Swagger JSON generate karne ke liye middleware
    app.UseSwagger();
    // Swagger UI server karne ke liye taaki interactive documentation page visualise ho sake (api/swagger)
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

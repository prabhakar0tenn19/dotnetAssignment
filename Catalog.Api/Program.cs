using Catalog.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Swagger endpoints validation aur swagger metadata documentation generator service register kar rahe hain
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In-Memory Repository ko DI (Dependency Injection) container me Singleton ki tarah register kar rahe hain,
// taaki iski state/data unique rahe aur across API requests persist kare.
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

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

using GameOfLife.Core.Interfaces;
using GameOfLife.Core.Services;
using GameOfLife.Infrastructure.Data;
using GameOfLife.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=gameoflife.db";

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite(connectionString));

// Register services
builder.Services.AddScoped<IBoardRepository, SqliteBoardRepository>();
builder.Services.AddScoped<GameService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

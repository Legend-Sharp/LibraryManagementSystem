using FluentValidation;
using LibraryManagementSystem.API.Middleware;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Behaviors;
using LibraryManagementSystem.Application.Features.Books.Commands;
using LibraryManagementSystem.Application.Features.Books.Queries;
using LibraryManagementSystem.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlServer")
    ?? "Server=DESKTOP-3DD07B0;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True";

var autoMigrate = builder.Configuration.GetValue("AutoMigrate", true);
var seedOnStartup = builder.Configuration.GetValue("SeedOnStartup", true);

builder.Services.AddDbContext<LibraryDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<LibraryDbContext>());

builder.Services.AddMediatR(typeof(GetBooksQuery).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateBookValidator).Assembly);
builder.Services.AddMemoryCache();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

    if (autoMigrate)
        await db.Database.MigrateAsync();

    if (seedOnStartup)
        await SeedData.InitializeAsync(db);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
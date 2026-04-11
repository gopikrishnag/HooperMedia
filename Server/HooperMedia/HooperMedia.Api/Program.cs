using System.Data;
using System.Reflection;
using HooperMedia.Infrastructure.Data;
using HooperMedia.Infrastructure.Repositories;
using HooperMedia.Infrastructure.Repositories.Interfaces;
using HooperMedia.Infrastructure.Services;
using HooperMedia.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    BaselineLegacyDatabase(dbContext);
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void BaselineLegacyDatabase(ApplicationDbContext dbContext)
{
    var appliedMigrations = dbContext.Database.GetAppliedMigrations().ToList();
    var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

    if (appliedMigrations.Count > 0 || pendingMigrations.Count == 0 || !PersonsTableExists(dbContext))
    {
        return;
    }

    var productVersion = typeof(DbContext).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion?
        .Split('+')[0]
        ?? "10.0.5";

    dbContext.Database.ExecuteSqlRaw(
        """
        IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
        BEGIN
            CREATE TABLE [__EFMigrationsHistory] (
                [MigrationId] nvarchar(150) NOT NULL,
                [ProductVersion] nvarchar(32) NOT NULL,
                CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
            );
        END;

        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {0})
        BEGIN
            INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
            VALUES ({0}, {1});
        END;
        """,
        pendingMigrations[0],
        productVersion);
}

static bool PersonsTableExists(ApplicationDbContext dbContext)
{
    var connection = dbContext.Database.GetDbConnection();
    var shouldCloseConnection = connection.State != ConnectionState.Open;

    if (shouldCloseConnection)
    {
        connection.Open();
    }

    try
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT CASE WHEN OBJECT_ID(N'[Persons]') IS NOT NULL THEN 1 ELSE 0 END;";

        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }
    finally
    {
        if (shouldCloseConnection)
        {
            connection.Close();
        }
    }
}

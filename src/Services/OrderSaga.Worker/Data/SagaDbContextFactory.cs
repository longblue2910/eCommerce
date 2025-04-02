// src/Services/OrderSaga.Worker/Data/SagaDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace OrderSaga.Worker.Data;

public class SagaDbContextFactory : IDesignTimeDbContextFactory<SagaDbContext>
{
    public SagaDbContext CreateDbContext(string[] args)
    {
        // Đọc cấu hình từ appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("SagaStateDb");

        // Đảm bảo connection string có TrustServerCertificate=True
        if (!connectionString.Contains("TrustServerCertificate=True"))
        {
            connectionString += ";TrustServerCertificate=True;";
        }

        var optionsBuilder = new DbContextOptionsBuilder<SagaDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SagaDbContext(optionsBuilder.Options);
    }
}


using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Infraestrutura.Configuracoes;

namespace API.Saude;

public class MongoHealthCheck : IHealthCheck
{
    private readonly MongoDbConfiguracoes _config;

    public MongoHealthCheck(IOptions<MongoDbConfiguracoes> options)
    {
        _config = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var conn = _config.ConnectionString;
            if (string.IsNullOrWhiteSpace(conn))
                return HealthCheckResult.Unhealthy("MongoDB connection string not configured.");

            var dbName = string.IsNullOrWhiteSpace(_config.DatabaseName) ? "admin" : _config.DatabaseName;

            var client = new MongoClient(conn);
            var database = client.GetDatabase(dbName);

            var command = new BsonDocument("ping", 1);
            await database.RunCommandAsync<BsonDocument>(command, cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy("MongoDB Ok!");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Falha ao conectar no MongoDB:  {ex.Message}");
        }
    }
}
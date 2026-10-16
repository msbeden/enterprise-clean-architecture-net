using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Enterprise.Infrastructure.Logging;

/// <summary>
/// MongoDB implementation of IAuditLogService for audit trail persistence.
/// </summary>
public class MongoAuditLogService : IAuditLogService
{
    private readonly IMongoCollection<AuditLogEntry>? _auditCollection;
    private readonly ILogger<MongoAuditLogService> _logger;

    public MongoAuditLogService(IConfiguration configuration, ILogger<MongoAuditLogService> logger)
    {
        _logger = logger;
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "EnterpriseAuditDb";
        var collectionName = configuration["MongoDB:CollectionName"] ?? "AuditLogs";

        if (!string.IsNullOrEmpty(connectionString))
        {
            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _auditCollection = database.GetCollection<AuditLogEntry>(collectionName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to initialize MongoDB client for audit logging.");
            }
        }
        else
        {
            _logger.LogInformation("MongoDB connection string not provided. MongoAuditLogService running in no-op mode.");
        }
    }

    public async Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        if (_auditCollection == null)
        {
            _logger.LogDebug("Audit log entry created (InMemory/Fallback): {EntityName} ({EntityId}) - {Action}", entry.EntityName, entry.EntityId, entry.Action);
            return;
        }

        try
        {
            await _auditCollection.InsertOneAsync(entry, cancellationToken: cancellationToken);
            _logger.LogInformation("Audit log entry persisted to MongoDB: {EntityName} {EntityId}", entry.EntityName, entry.EntityId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing audit log entry to MongoDB.");
        }
    }

    public async Task<IReadOnlyList<AuditLogEntry>> GetLogsForEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default)
    {
        if (_auditCollection == null)
        {
            return Array.Empty<AuditLogEntry>();
        }

        try
        {
            var filter = Builders<AuditLogEntry>.Filter.And(
                Builders<AuditLogEntry>.Filter.Eq(x => x.EntityName, entityName),
                Builders<AuditLogEntry>.Filter.Eq(x => x.EntityId, entityId)
            );

            var logs = await _auditCollection.Find(filter).ToListAsync(cancellationToken);
            return logs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading audit log entries from MongoDB.");
            return Array.Empty<AuditLogEntry>();
        }
    }
}

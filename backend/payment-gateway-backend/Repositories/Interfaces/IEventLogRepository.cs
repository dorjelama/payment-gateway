using payment_gateway_backend.Entities;

namespace payment_gateway_backend.Repositories.Interfaces;

public interface IEventLogRepository
{
    public Task AddLogAsync(EventLog log);
    public Task<IEnumerable<EventLog>> GetLatestLogsAsync(int count);
}
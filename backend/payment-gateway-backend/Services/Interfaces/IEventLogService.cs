namespace payment_gateway_backend.Services.Interfaces;

public interface IEventLogService
{
    public Task AddLogAsync(string eventType, object eventData);
    public Task<IEnumerable<Models.EventLog.EventLogDto>> GetLatestLogsAsync(int count);
}

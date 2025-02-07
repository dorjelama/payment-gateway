using AutoMapper;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Models.EventLog;
using payment_gateway_backend.Repositories.Interfaces;
using payment_gateway_backend.Services.Interfaces;
using System.Text.Json;

namespace payment_gateway_backend.Services.Implementations;

public class EventLogService : IEventLogService
{
    private readonly IEventLogRepository _eventLogRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EventLogService> _logger;
    public EventLogService(IEventLogRepository eventLogRepository, IMapper mapper, ILogger<EventLogService> logger)
    {
        _eventLogRepository = eventLogRepository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task AddLogAsync(string eventType, object eventData)
    {
        try
        {
            var payload = eventData is string str ? str : JsonSerializer.Serialize(eventData);

            var logEntry = new EventLog
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Payload = payload,
                CreatedAt = DateTime.UtcNow
            };

            await _eventLogRepository.AddLogAsync(logEntry);
            _logger.LogInformation("Logged event: {EventType} at {Timestamp}", eventType, logEntry.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log event: {EventType}", eventType);
        }
    }

    public async Task<IEnumerable<EventLogDto>> GetLatestLogsAsync(int count)
    {
        var logs = await _eventLogRepository.GetLatestLogsAsync(count);
        return _mapper.Map<IEnumerable<EventLogDto>>(logs);
    }
}
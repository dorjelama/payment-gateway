using Microsoft.EntityFrameworkCore;
using payment_gateway_backend.Data;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Repositories.Interfaces;

namespace payment_gateway_backend.Repositories.Implementations;

public class EventLogRepository : IEventLogRepository
{
    private readonly PaymentGatewayDbContext _context;
    public EventLogRepository(PaymentGatewayDbContext context) => _context = context;
    public async Task AddLogAsync(EventLog log)
    {
        await _context.EventLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<EventLog>> GetLatestLogsAsync(int count)
    {
        return await _context.EventLogs.OrderByDescending(log => log.CreatedAt).Take(count).ToListAsync();
    }
}

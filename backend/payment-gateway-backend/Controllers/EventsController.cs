using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_gateway_backend.Services.Interfaces;

namespace payment_gateway_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventLogService _eventLogService;

    public EventsController(IEventLogService eventLogService)
    {
        _eventLogService = eventLogService;
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetEventLogs()
    {
        var logs = await _eventLogService.GetLatestLogsAsync(100);
        return Ok(logs);
    }
}

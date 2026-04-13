using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noticore.Application.Notifications.Commands.CreateNotification;

namespace Noticore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Using Primary Constructor to inject ISender (MediatR)
    public class NotificationsController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationCommand command)
        {
            // The controller only delegates the command to MediatR
            var result = await sender.Send(command);

            // Return 201 Created with the generated ID
            return CreatedAtAction(nameof(Create), new { id = result }, result);
        }
    }
}
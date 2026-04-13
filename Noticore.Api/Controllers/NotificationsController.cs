using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noticore.Application.Notifications.Commands.CreateNotification;
using Noticore.Application.Notifications.Queries.GetNotificationById;
using Noticore.Application.Notifications.Queries.GetNotifications;
using System.ComponentModel.DataAnnotations;

namespace Noticore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Using Primary Constructor to inject ISender (MediatR) and the validator
    public class NotificationsController(
        ISender sender, 
        IValidator<CreateNotificationCommand> validator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationCommand command)
        {
            // Manual validation
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // The controller only delegates the command to MediatR
            var result = await sender.Send(command);

            // Return 201 Created with the generated ID
            return CreatedAtAction(nameof(Create), new { id = result }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await sender.Send(new GetNotificationsQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await sender.Send(new GetNotificationByIdQuery(id));
            
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
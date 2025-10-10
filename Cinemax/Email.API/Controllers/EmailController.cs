using Email.API.Models;
using Email.API.Services;
using EventBus.Messages.Events;
using Microsoft.AspNetCore.Mvc;

namespace Email.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IMessageProducer _messageProducer;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IMessageProducer messageProducer, 
            IEmailService emailService,
            ILogger<EmailController> logger)
        {
            _messageProducer = messageProducer;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Send email asynchronously via RabbitMQ (non-blocking)
        /// </summary>
        /// <param name="emailRequest">Email details</param>
        /// <returns>Confirmation that email was queued</returns>
        [HttpPost("send-async")]
        public async Task<ActionResult<ApiResponse<string>>> SendEmailAsync([FromBody] EmailRequest emailRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Subject))
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse(
                        "To and Subject are required fields"));
                }

                var emailEvent = new SendEmailEvent
                {
                    To = emailRequest.To,
                    Subject = emailRequest.Subject,
                    Body = emailRequest.Body,
                    From = emailRequest.From,
                    IsHtml = emailRequest.IsHtml,
                    Cc = emailRequest.Cc,
                    Bcc = emailRequest.Bcc,
                    Attachments = emailRequest.Attachments,
                    Priority = emailRequest.Priority
                };

                await _messageProducer.PublishEmailAsync(emailEvent);

                _logger.LogInformation("Email queued successfully for {To}", emailRequest.To);

                return Ok(ApiResponse<string>.SuccessResponse(
                    emailEvent.Id.ToString(), 
                    "Email has been queued for sending"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to queue email for {To}", emailRequest.To);
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    "Failed to queue email", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Send email synchronously (blocking - for testing purposes)
        /// </summary>
        /// <param name="emailRequest">Email details</param>
        /// <returns>Result of email sending</returns>
        [HttpPost("send-sync")]
        public async Task<ActionResult<ApiResponse<bool>>> SendEmailSync([FromBody] EmailRequest emailRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Subject))
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(
                        "To and Subject are required fields"));
                }

                var result = await _emailService.SendEmailAsync(emailRequest);

                if (result)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(true, "Email sent successfully"));
                }
                else
                {
                    return StatusCode(500, ApiResponse<bool>.ErrorResponse("Failed to send email"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", emailRequest.To);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(
                    "Failed to send email", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>Service status</returns>
        [HttpGet("health")]
        public ActionResult<ApiResponse<string>> HealthCheck()
        {
            return Ok(ApiResponse<string>.SuccessResponse("healthy", "Email service is running"));
        }
    }
}


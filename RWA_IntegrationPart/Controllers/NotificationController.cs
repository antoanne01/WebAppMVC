using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RWA_IntegrationPart.Model;
using RWA_IntegrationPart.Models;
using System.Net.Mail;

namespace RWA_IntegrationPart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly RwaMoviesContext _rwaDbContext;

        public NotificationController(RwaMoviesContext rwaDbContext)
        {
            _rwaDbContext = rwaDbContext;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<NotificationResponse>> GetAll()
        {
            try
            {
                var allNotifications = _rwaDbContext.Notifications.Select(x => new NotificationResponse
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    ReceiverEmail = x.ReceiverEmail,
                    Subject = x.Subject,
                    Body = x.Body
                });

                return Ok(allNotifications);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<NotificationResponse> Get(int id)
        {
            try
            {
                var result = _rwaDbContext.Notifications.FirstOrDefault(x => x.Id == id);
                if(result == null)
                {
                    return NotFound("Requested notification not found");
                }
                return Ok(new NotificationResponse
                {
                    Id = result.Id,
                    CreatedAt = result.CreatedAt,
                    ReceiverEmail = result.ReceiverEmail,
                    Subject = result.Subject,
                    Body = result.Body
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("[action]")]
        public ActionResult<NotificationResponse> CreateNotification(NotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newNotification = new Notification
                {
                    CreatedAt = DateTime.UtcNow,
                    ReceiverEmail = request.ReceiverEmail,
                    Subject = request.Subject,
                    Body = request.Body
                };

                _rwaDbContext.Notifications.Add(newNotification);
                _rwaDbContext.SaveChanges();

                return Ok(new NotificationResponse
                {
                    Id = newNotification.Id,
                    CreatedAt = newNotification.CreatedAt,
                    ReceiverEmail = newNotification.ReceiverEmail,
                    Subject = newNotification.Subject,
                    Body = newNotification.Body
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<NotificationResponse> EditNotification(int id, [FromBody] NotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return NotFound(ModelState);
                }

                var result = _rwaDbContext.Notifications.FirstOrDefault(x => x.Id == id);
                if (result == null)
                {
                    return NotFound("Requested notification not found");
                }

                result.ReceiverEmail = request.ReceiverEmail;
                result.Subject = request.Subject;
                result.Body = request.Body;

                _rwaDbContext.SaveChanges();

                return Ok(new NotificationResponse
                {
                    Id = result.Id,
                    CreatedAt = result.CreatedAt,
                    ReceiverEmail = result.ReceiverEmail,
                    Subject = result.Subject,
                    Body = result.Body
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<NotificationResponse> RemoveNotification(int id)
        {
            try
            {
                var result = _rwaDbContext.Notifications.FirstOrDefault(x => x.Id == id);
                if(result == null)
                {
                    return NotFound("Requested notification not found");
                }

                _rwaDbContext.Notifications.Remove(result);
                _rwaDbContext.SaveChanges();

                return Ok(new NotificationResponse
                {
                    Id = result.Id,
                    CreatedAt = result.CreatedAt,
                    ReceiverEmail = result.ReceiverEmail,
                    Subject = result.Subject,
                    Body = result.Body
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("failedNotificationCount")]
        public ActionResult<int> GetFailedNotificationCount()
        {
            try
            {
                int failed = _rwaDbContext.Notifications.Count(x => x.SentAt == null);
                return Ok(failed);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpPost("[action]")]
        public ActionResult SendNotification()
        {
            var client = new SmtpClient("127.0.0.1", 9025);
            var sender = "admin@webapi.com";
            try
            {
                var failedNotifications = _rwaDbContext.Notifications.Where(x => x.SentAt == null).ToList();

                foreach (var notification in failedNotifications)
                {
                    try
                    {
                        var mail = new MailMessage(
                            from: new MailAddress(sender),
                            to: new MailAddress(notification.ReceiverEmail));

                        mail.Subject = notification.Subject;
                        mail.Body = notification.Body;

                        client.Send(mail);

                        notification.SentAt = DateTime.UtcNow;

                        _rwaDbContext.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        return NotFound("Error occured");
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while sending notifications");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using AARR_stat.Model.Db;
using AARR_stat.Model.Dto;
using PetaPoco;

namespace AARR_stat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AARRStatController : ControllerBase
    {
        private readonly ILogger<AARRStatController> _logger;
        private readonly IConfiguration _configuration;

        public AARRStatController(ILogger<AARRStatController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            _logger.LogDebug("Ping");
            return Ok(new { result = "pong"});
        }

        [HttpGet]
        public IEnumerable<SessionViewDto> Get(DateTime? start, DateTime? end)
        {
            _logger.LogDebug("Get /");
            var sessionItems = new List<SessionViewDto>();

            try
            {
                using (var db = new PetaPoco.Database(_configuration["ConnectionStrings:AARRStatConnection"], "MariaDb")) {
                    var sql = PetaPoco.Sql.Builder.Append("SELECT s.*,t.name AS type FROM session s JOIN sessiontype t ON s.type_id=t.id");
                    if (start.HasValue) {
                        sql.Append("WHERE s.start>=@0", start);
                    }
                    if (end.HasValue) {
                        sql.Append("WHERE s.start<@0", end);
                    }
                    // Default is to return last 24h
                    if (!start.HasValue && !end.HasValue) {
                        sql.Append("WHERE s.start>@0", DateTime.UtcNow.AddDays(-1));
                    }
                    sql.OrderBy("s.start");
                    sessionItems = db.Query<SessionViewDto>(sql).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return sessionItems.ToArray();
        }

        [HttpPost]
        [Route("StartSession")]
        public ActionResult PostStartSession([FromBody] StartSessionDto sessionDto) 
        {
            _logger.LogDebug("Start session");
            try
            {
                using (var db = new PetaPoco.Database(_configuration["ConnectionStrings:AARRStatConnection"], "MariaDb")) {
                    var user = new User { Id = sessionDto.User };
                    var device = new Device { Id = sessionDto.Device, User = user.Id };
                    var session = new Session { Id = sessionDto.Session, User = user.Id, Device = device.Id, Start = DateTime.UtcNow };
                    if (String.IsNullOrEmpty(db.SingleOrDefault<User>("where id=@0", sessionDto.User)?.Id))
                    {
                        db.Insert("user", "id", false, user);
                    }
                    if (String.IsNullOrEmpty(db.SingleOrDefault<Device>("where id=@0", sessionDto.Device)?.Id))
                    {
                        db.Insert("device", "id", false, device);
                    }
                    var dbSessionType = db.SingleOrDefault<SessionType>("where name like @0", $"%{sessionDto.Type}%");
                    if (dbSessionType == null) 
                    {
                        _logger.LogError($"Request parameter 'type' has an invalid value: '{sessionDto.Type}'.");
                        return BadRequest(new { result = "error", message = $"Request parameter 'type' has an invalid value: '{sessionDto.Type}'."});
                    }
                    session.Type = dbSessionType.Id;
                    db.Insert("session", "id", false, session);
                }
                return Ok(new { result = "ok" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("newdevice")]
        public ActionResult PostNewDevice([FromBody] NewDeviceDto newDeviceDto) 
        {
            _logger.LogDebug("PostNewDevice");
            try
            {
                using (var db = new PetaPoco.Database(_configuration["ConnectionStrings:AARRStatConnection"], "MariaDb")) {
                    var user = new User { Id = newDeviceDto.User };
                    var device = new Device { Id = newDeviceDto.Id, User = user.Id, Description = newDeviceDto.Description };
                    if (String.IsNullOrEmpty(db.SingleOrDefault<User>("where id=@0", newDeviceDto.User)?.Id))
                    {
                        db.Insert("user", "id", false, user);
                    }
                    if (String.IsNullOrEmpty(db.SingleOrDefault<Device>("where id=@0", newDeviceDto.Id)?.Id))
                    {
                        db.Insert("device", "id", false, device);
                    } 
                }
                return Ok(new { result = "ok" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}

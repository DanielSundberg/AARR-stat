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
        public IEnumerable<SessionViewDto> Get()
        {
            var sessionItems = new List<SessionViewDto>();

            try
            {
                using (var db = new PetaPoco.Database(_configuration["ConnectionStrings:AARRStatConnection"], "MariaDb")) {
                    sessionItems = db.Query<SessionViewDto>("SELECT * FROM session").ToList();
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
        public ActionResult PostStartSession([FromBody] StartSessionDto sessionItem) 
        {
            try
            {
                using (var db = new PetaPoco.Database(_configuration["ConnectionStrings:AARRStatConnection"], "MariaDb")) {
                    var user = new User { Id = sessionItem.User };
                    var device = new Device { Id = sessionItem.Device, User = user.Id };
                    var session = new Session { Id = sessionItem.Session, User = user.Id, Device = device.Id, Start = DateTime.UtcNow };
                    if (String.IsNullOrEmpty(db.SingleOrDefault<User>("where id=@0", sessionItem.User)?.Id))
                    {
                        db.Insert("user", "id", false, user);
                    }
                    if (String.IsNullOrEmpty(db.SingleOrDefault<Device>("where id=@0", sessionItem.Device)?.Id))
                    {
                        db.Insert("device", "id", false, device);
                    }
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
    }
}

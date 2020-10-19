using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AARR_stat.Model.Dto;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AARR_stat.Model.Db;
using System.Collections.Generic;

namespace AARR_stat.Controllers
{
    [ApiController]
    [Route("/aarrstat/session")]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<AARRStatController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAmazonDynamoDB _dynamoDb;

        public SessionController(ILogger<AARRStatController> logger, IConfiguration configuration, IAmazonDynamoDB dynamoDb)
        {
            _logger = logger;
            _configuration = configuration;
            _dynamoDb = dynamoDb;
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            _logger.LogDebug("Ping");
            return Ok(new { result = "pong"});
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? start, DateTime? end)
        {
            _logger.LogDebug("Get /");
            var sessionItems = new List<SessionViewDto>();

            try
            {
                // TODO: implement query

                // using (var db = new PetaPoco.Database(_configuration["ConnectionStrings:AARRStatConnection"], "MariaDb")) {
                //     var sql = PetaPoco.Sql.Builder.Append("SELECT s.*,t.name AS type FROM session s JOIN sessiontype t ON s.type_id=t.id");
                //     if (start.HasValue) {
                //         sql.Append("WHERE s.start>=@0", start);
                //     }
                //     if (end.HasValue) {
                //         sql.Append("WHERE s.start<@0", end);
                //     }
                //     // Default is to return last 24h
                //     if (!start.HasValue && !end.HasValue) {
                //         sql.Append("WHERE s.start>@0", DateTime.UtcNow.AddDays(-1));
                //     }
                //     sql.OrderBy("s.start");
                //     sessionItems = db.Query<SessionViewDto>(sql).ToList();
                // }
                // using (var context = new DynamoDBContext(_dynamoDb)) {
                //     var existingDevice = await context.<DynamoDbDevice>();
                //     if (existingDevice != null) {
                //         return Ok(new { 
                //             result = "ok", 
                //             device = existingDevice
                //         });
                //     }
                //     else 
                //     {
                //         return BadRequest(new { result = "error", message = "Device not found." });
                //     }
                // }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return Ok( new {
                result = "ok", 
                sessions = sessionItems.ToArray()
            });
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> PostStartSession([FromBody] StartSessionDto startSessionDto) 
        {
            _logger.LogDebug("Start session");
            try
            {
                using (var context = new DynamoDBContext(_dynamoDb)) {

                    var now = DateTime.UtcNow;

                    // First try to load device
                    var existingDevice = await context.LoadAsync<DynamoDbDevice>(startSessionDto.Device);

                    if (existingDevice == null) {
                        return BadRequest(new { 
                            result = "error",
                            message = "Unknown device."
                        });
                    }
                    // Save session
                    var newSession = new DynamoDbSession {
                        Id = startSessionDto.Session,
                        Type = startSessionDto.Type,
                        User = startSessionDto.User,
                        Device = startSessionDto.Device,
                        Start = DateTime.UtcNow
                    };
                    await context.SaveAsync(newSession);
                    var savedSession = await context.LoadAsync<DynamoDbSession>(startSessionDto.Session);
                    return Ok(new { 
                        result = "ok",
                        message = "Created session.", 
                        session = savedSession 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("end")]
        public async Task<IActionResult> PostEndSession([FromBody] EndSessionDto endSessionDto) 
        {
            _logger.LogDebug("End session");
            try
            {
                using (var context = new DynamoDBContext(_dynamoDb)) {


                    // First try to load device
                    var existingSession = await context.LoadAsync<DynamoDbSession>(endSessionDto.Session);

                    if (existingSession == null) {
                        return BadRequest(new { 
                            result = "error",
                            message = "Unknown session."
                        });
                    }
                    // Save session
                    existingSession.End = DateTime.UtcNow;
                    await context.SaveAsync(existingSession);
                    var savedSession = await context.LoadAsync<DynamoDbSession>(endSessionDto.Session);
                    return Ok(new { 
                        result = "ok",
                        message = "Updated session.", 
                        session = savedSession 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

    }
}

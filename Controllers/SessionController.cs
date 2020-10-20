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
            _logger.LogDebug("Get /sessions");

            try
            {
                if (!start.HasValue) {
                    start = DateTime.UtcNow.AddMonths(-1);
                }
                if (!end.HasValue) {
                    end = DateTime.UtcNow;
                }

                using (var context = new DynamoDBContext(_dynamoDb)) {
                    var conditions = new List<ScanCondition>();
                    conditions.Add(new ScanCondition("Start", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Between, start, end));

                    var sessions = await context.ScanAsync<DynamoDbSession>(conditions).GetRemainingAsync();
                    return Ok(new { 
                        result = "ok", 
                        sessions = sessions.ToArray()
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
                    var now = DateTime.UtcNow;
                    existingSession.DurationMS = Convert.ToInt32((now - existingSession.Start).TotalMilliseconds);
                    await context.SaveAsync(existingSession);
                    var savedSession = await context.LoadAsync<DynamoDbSession>(endSessionDto.Session);

                    // Calculate total and avg time today for this user
                    var key = $"{existingSession.User}-{now.Year}-{now.DayOfYear}";
                    var userTotalDay = await context.LoadAsync<DynamoDbUserTotalDay>(key);
                    if (userTotalDay == null) {
                        userTotalDay = new DynamoDbUserTotalDay {
                            UserYearDay = key
                        };
                    }
                    userTotalDay.NofSessions += 1;
                    userTotalDay.TotalMS += existingSession.DurationMS;
                    userTotalDay.AvgMS = (int)Math.Round(userTotalDay.TotalMS / (double)userTotalDay.NofSessions);

                    // Some extra tracking of longer sessions (over 60s)
                    if (existingSession.DurationMS > 60000) {
                        userTotalDay.NofSessionsOver60s += 1;
                        userTotalDay.TotalOver60sMS += existingSession.DurationMS;
                        userTotalDay.AvgOver60sMS = (int)Math.Round(userTotalDay.TotalOver60sMS / (double)userTotalDay.NofSessionsOver60s);
                    }

                    await context.SaveAsync(userTotalDay);

                    return Ok(new { 
                        result = "ok",
                        message = "Updated session.", 
                        session = savedSession, 
                        userTotalDay = userTotalDay
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

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
using System.Globalization;

namespace AARR_stat.Controllers
{
    [ApiController]
    [Route("/api/session")]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAmazonDynamoDB _dynamoDb;

        public SessionController(ILogger<SessionController> logger, IConfiguration configuration, IAmazonDynamoDB dynamoDb)
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
            return Ok(new { result = "pong from session"});
        }

        [HttpGet]
        [Route("testdb")]
        public async Task<IActionResult> TestDb()
        {
            _logger.LogDebug("Testing db");
            
             
            var sessionDescription = await _dynamoDb.DescribeTableAsync("aarrstat-session");

            return Ok(new { 
                result = "ok", 
                message = $"Describe table http call status: {sessionDescription.HttpStatusCode}",
                details = $"Nof items in session table: {sessionDescription.Table.ItemCount}"
            });
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

                    var sessions = await context.ScanAsync<DbSession>(conditions).GetRemainingAsync();
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
                    var existingDevice = await context.LoadAsync<DbDevice>(startSessionDto.Device);

                    if (existingDevice == null) {
                        return BadRequest(new { 
                            result = "error",
                            message = "Unknown device."
                        });
                    }
                    // Save session
                    var newSession = new DbSession {
                        Id = startSessionDto.Session,
                        Type = startSessionDto.Type,
                        User = startSessionDto.User,
                        Device = startSessionDto.Device,
                        Start = DateTime.UtcNow
                    };
                    await context.SaveAsync(newSession);
                    var savedSession = await context.LoadAsync<DbSession>(startSessionDto.Session);
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
                    var existingSession = await context.LoadAsync<DbSession>(endSessionDto.Session);

                    if (existingSession == null) {
                        return BadRequest(new { 
                            result = "error",
                            message = "Unknown session."
                        });
                    }
                    // Save the current session
                    var now = DateTime.UtcNow;
                    existingSession.DurationMS = Convert.ToInt32((now - existingSession.Start).TotalMilliseconds);
                    await context.SaveAsync(existingSession);
                    var savedSession = await context.LoadAsync<DbSession>(endSessionDto.Session);

                    // Calculate and save statistics
                    var yearForDay = existingSession.Start.Year;
                    var day = existingSession.Start.DayOfYear;
                    var (yearForWeek, week) = GetIso8601WeekOfYear(existingSession.Start);
                    var firstSessionToday = false;
                    var firstSessionThisWeek = false;

                    // User day total
                    var key = $"{existingSession.User}-{yearForDay}-{day}";
                    var userTotalDay = await context.LoadAsync<DbUserTotalDay>(key);
                    if (userTotalDay == null) {
                        userTotalDay = new DbUserTotalDay {
                            UserYearDay = key
                        };
                        firstSessionToday = true;
                    }
                    userTotalDay.Day = day;
                    userTotalDay.Timestamp = now;
                    FillAggregatedStatistics(userTotalDay, existingSession.DurationMS);
                    await context.SaveAsync(userTotalDay);

                    // User week total
                    key = $"{existingSession.User}-{yearForWeek}-{week}";
                    var userTotalWeek = await context.LoadAsync<DbUserTotalWeek>(key);
                    if (userTotalWeek == null) {
                        userTotalWeek = new DbUserTotalWeek {
                            UserYearWeek = key
                        };
                        firstSessionThisWeek = true;
                    }
                    userTotalWeek.Week = week;
                    userTotalWeek.Timestamp = now;
                    FillAggregatedStatistics(userTotalWeek, existingSession.DurationMS);
                    await context.SaveAsync(userTotalWeek);

                    // Grand total day
                    key = $"{yearForDay}-{day}";
                    var grandTotalDay = await context.LoadAsync<DbGrandTotalDay>(key);
                    if (grandTotalDay == null) {
                        grandTotalDay = new DbGrandTotalDay {
                            YearDay = key
                        };
                    }
                    grandTotalDay.Day = day;
                    grandTotalDay.Timestamp = now;
                    if (firstSessionToday) {
                        grandTotalDay.UserCount += 1;
                    }
                    FillAggregatedStatistics(grandTotalDay, existingSession.DurationMS);
                    await context.SaveAsync(grandTotalDay);

                    // Grand total week
                    key = $"{yearForWeek}-{week}";
                    var grandTotalWeek = await context.LoadAsync<DbGrandTotalWeek>(key);
                    if (grandTotalWeek == null) {
                        grandTotalWeek = new DbGrandTotalWeek {
                            YearWeek = key
                        };
                    }
                    grandTotalWeek.Week = week;
                    grandTotalWeek.Timestamp = now;
                    if (firstSessionThisWeek) {
                        grandTotalWeek.UserCount += 1;
                    }
                    FillAggregatedStatistics(grandTotalWeek, existingSession.DurationMS);
                    await context.SaveAsync(grandTotalWeek);

                    return Ok(new { 
                        result = "ok",
                        message = "Updated session.", 
                        session = savedSession, 
                        userTotalDay = userTotalDay, 
                        userTotalWeek = userTotalWeek,
                        grandTotalDay = grandTotalDay, 
                        grandTotalWeek = grandTotalWeek
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        
        private void FillAggregatedStatistics(CommonAggregationFields fields, int sessionDuration) 
        {
            fields.NofSessions += 1;
            fields.TotalMS += sessionDuration;
            fields.AvgMS = (int)Math.Round(fields.TotalMS / (double)fields.NofSessions);
            if (sessionDuration > 60000) {
                fields.NofLongSession += 1;
                fields.TotalLongSessionMS += sessionDuration;
                fields.AvgLongSessionMS = (int)Math.Round(fields.TotalLongSessionMS / (double)fields.NofLongSession);
            }
        }

        private static (int, int) GetIso8601WeekOfYear(DateTime time)
        {
            // https://stackoverflow.com/a/11155102

            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return (time.Year, CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));
        } 
    }
}

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
using Amazon.CognitoIdentityProvider;
using Amazon;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.CognitoIdentity.Model;

namespace AARR_stat.Controllers
{
    [ApiController]
    [Route("/app")]
    public class AppController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAmazonDynamoDB _dynamoDb;

        public AppController(ILogger<SessionController> logger, IConfiguration configuration, IAmazonDynamoDB dynamoDb)
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
            return Ok(new { result = "pong from login"});
        }

        [HttpGet]
        [Route("ping-authenticated")]
        public IActionResult PingAuthenticated()
        {
            _logger.LogDebug("Ping");
            return Ok(new { result = "pong from login (authenticated user)"});
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogDebug("Login");

            try 
            {
                var provider = new AmazonCognitoIdentityProviderClient(
                    new Amazon.Runtime.AnonymousAWSCredentials(), 
                    RegionEndpoint.GetBySystemName(_configuration["AWS:Cognito:Region"]));
                var userPool = new CognitoUserPool(_configuration["AWS:Cognito:PoolId"], _configuration["AWS:Cognito:ClientId"], provider);
                var user = new CognitoUser(loginDto.Username, _configuration["AWS:Cognito:ClientId"], userPool, provider);
                var authRequest = new InitiateSrpAuthRequest() { Password = loginDto.Password };
                var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
                
                return Ok(new { 
                    result = "ok", 
                    identityToken = authResponse.AuthenticationResult.IdToken, 
                    expiresInSeconds = authResponse.AuthenticationResult.ExpiresIn
                });
            }
            catch (Amazon.CognitoIdentityProvider.Model.NotAuthorizedException ex) 
            {
                return Unauthorized(new { 
                    result = "error", 
                    type = ex.GetType().ToString(), 
                    message = ex.Message 
                });
            }
        }

        [HttpGet]
        [Route("dashboard")]
        public async Task<IActionResult> GetDashboardStats() {
            using (var context = new DynamoDBContext(_dynamoDb)) {
                // Per day
                var usersPerDay = new List<object>();
                var totPerDay = new List<object>();
                var avgLongSessionPerDay = new List<object>();
                var now = DateTime.UtcNow;
                var yearForDay = now.Year;
                
                for (int i = 9; i >= 0; i--) { // Last 10 days including today
                    var day = now.AddDays(-i);
                    var key = $"{yearForDay}-{day.DayOfYear}";
                    var dayStat = await context.LoadAsync<DbGrandTotalDay>(key);
                    var dayStr = day.ToString("ddd MMM d");
                    usersPerDay.Add(new {
                        key = dayStr, 
                        value = dayStat?.UserCount ?? 0
                    });
                    totPerDay.Add(new {
                        key = dayStr,
                        value = Math.Round((dayStat?.TotalMS ?? 0) / 60000.0)
                    });
                    avgLongSessionPerDay.Add(new {
                        key = dayStr,
                        value = Math.Round((dayStat?.AvgLongSessionMS ?? 0) / 1000.0)
                    });
                }
                return Ok(new { 
                    result = "ok", 
                    usersPerDay = usersPerDay,
                    totPerDay = totPerDay,
                    avgLongSessionPerDay = avgLongSessionPerDay,
                });
            }
        }
    }
}

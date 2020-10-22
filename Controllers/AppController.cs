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
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            _logger.LogDebug("Login");
            return Ok(new { result = "ok"});
        }

    }
}

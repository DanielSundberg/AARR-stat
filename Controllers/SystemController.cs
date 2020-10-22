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
    [Route("/api/system")]
    public class SystemController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAmazonDynamoDB _dynamoDb;

        public SystemController(ILogger<SessionController> logger, IConfiguration configuration, IAmazonDynamoDB dynamoDb)
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
            return Ok(new { result = "pong from system"});
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
    }
}

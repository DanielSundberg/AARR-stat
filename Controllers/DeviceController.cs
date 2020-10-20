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
    [Route("/aarrstat/device")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAmazonDynamoDB _dynamoDb;

        public DeviceController(ILogger<DeviceController> logger, IConfiguration configuration, IAmazonDynamoDB dynamoDb)
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
        [Route("")]
        public async Task<IActionResult> Get() 
        {
            _logger.LogDebug("Get all devices");
            try
            {
                using (var context = new DynamoDBContext(_dynamoDb)) {

                    var devices = await context.ScanAsync<DynamoDbDevice>(new List<ScanCondition>()).GetRemainingAsync();
                    return Ok(new { 
                        result = "ok", 
                        devices = devices.ToArray()
                    });
                }            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(string id) 
        {
            _logger.LogDebug("Get device");
            try
            {
                using (var context = new DynamoDBContext(_dynamoDb)) {
                    var existingDevice = await context.LoadAsync<DynamoDbDevice>(id);
                    if (existingDevice != null) {
                        return Ok(new { 
                            result = "ok", 
                            device = existingDevice
                        });
                    }
                    else 
                    {
                        return BadRequest(new { result = "error", message = "Device not found." });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("new")]
        public async Task<IActionResult> PostNew([FromBody] NewDeviceDto newDeviceDto) 
        {
            _logger.LogDebug("PostNewDevice");
            try
            {
                using (var context = new DynamoDBContext(_dynamoDb)) {

                    var now = DateTime.UtcNow;

                    // First try to load device
                    var existingDevice = await context.LoadAsync<DynamoDbDevice>(newDeviceDto.Id);

                    if (existingDevice == null) {
                        // Create new device
                        var newDevice = new DynamoDbDevice {
                            Id = newDeviceDto.Id, 
                            User = newDeviceDto.User,
                            Description = newDeviceDto.Description,
                            Enabled = newDeviceDto.Enabled,
                            RegisterDate = now,
                            UpdateDate = now, 
                        };
                        await context.SaveAsync(newDevice);
                        return Ok(new { 
                            result = "ok", 
                            message = "Created device.",
                            device = newDevice
                        });
                    }
                    else 
                    {
                        // Update device
                        existingDevice.Enabled = newDeviceDto.Enabled;
                        existingDevice.UpdateDate = now;
                        existingDevice.Description = newDeviceDto.Description;
                        await context.SaveAsync(existingDevice);
                        return Ok(new { 
                            result = "ok",
                            message = "Updated device.", 
                            device = existingDevice 
                        });
                    }
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

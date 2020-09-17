using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        public IEnumerable<AARRStatSessionItem> Get()
        {
            try
            {
                var conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = _configuration["ConnectionStrings:DefaultConnection"];
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                _logger.LogError(ex, ex.Message);
            }


            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new AARRStatSessionItem
            {
                User = Guid.NewGuid().ToString(),
                Device = Guid.NewGuid().ToString(),
                Session = Guid.NewGuid().ToString(),
                Start = DateTime.Now.AddDays(index).ToUniversalTime(),
                End = DateTime.Now.AddDays(index).AddMinutes(5).ToUniversalTime(),
            })
            .ToArray();
        }
    }
}

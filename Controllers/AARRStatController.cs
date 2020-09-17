using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

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
            var sessionItems = new List<AARRStatSessionItem>();

            try
            {
                var conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = _configuration["ConnectionStrings:AARRStatConnection"];
                conn.Open();

                var cmd = new MySqlCommand();
                cmd.CommandText = "session";
                cmd.Connection = conn;
                cmd.CommandType = CommandType.TableDirect;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sessionItem = new AARRStatSessionItem() {
                            Session = reader[0].ToString(),
                            User = reader[1].ToString(),
                            Device = reader[2].ToString(),
                            Start = DateTime.Parse(reader[3].ToString())
                        };
                        sessionItems.Add(sessionItem);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return sessionItems.ToArray();
        }

        [HttpPost]
        public ActionResult Post(AARRStatSessionItem sessionItem) 
        {
            return Ok(new { result = "ok" });
        }
    }
}

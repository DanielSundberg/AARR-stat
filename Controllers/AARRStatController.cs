using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AARR_stat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AARRStatController : ControllerBase
    {
        private readonly ILogger<AARRStatController> _logger;

        public AARRStatController(ILogger<AARRStatController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<AARRStatSessionItem> Get()
        {
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

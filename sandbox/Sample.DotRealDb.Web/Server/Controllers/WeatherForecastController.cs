using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sample.DotRealDb.Web.Server.Data;
using Sample.DotRealDb.Web.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.DotRealDb.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly SampleDbContext sampleDbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, SampleDbContext sampleDbContext)
        {
            _logger = logger;
            this.sampleDbContext = sampleDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await sampleDbContext.WeatherForecasts.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]WeatherForecast forecast)
        {
            sampleDbContext.WeatherForecasts.Add(forecast);
            await sampleDbContext.SaveChangesAsync();
            return Ok(forecast);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await sampleDbContext.WeatherForecasts.FindAsync(id);
            if (deleted == null)
                return NotFound();

            sampleDbContext.Remove(deleted);
            await sampleDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}

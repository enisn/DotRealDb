using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.DotRealDb.Web.Shared
{
    public class WeatherForecast
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public override bool Equals(object obj)
        {
            if (obj is WeatherForecast other)
            {
                return this.Id == other.Id;
            }
            return base.Equals(obj);
        }
    }
}

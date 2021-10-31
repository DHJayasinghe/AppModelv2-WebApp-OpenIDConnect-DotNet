using System;
using System.Collections.Generic;

namespace Maintenance.API
{
    public sealed class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }

    public sealed class SummaryDto
    {
        public string Name { get; set; }
        public IEnumerable<WeatherForecast> Results { get; set; }
    }
}

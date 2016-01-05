using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherWebApp.Models
{
    public class TableWeatherMessage : TableEntity
    {
        public string DeviceID { get; set; }
        public string Temperature { get; set; }
        public string Altitude { get; set; }
        public string Humidity { get; set; }
        public string Pressure { get; set; }
        public string CurrentDateTime { get; set; }
    }
}

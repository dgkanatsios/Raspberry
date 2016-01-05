using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeatherApp.Shared;
using WeatherWebApp.Models;

namespace WeatherWebApp.Controllers
{
    public class WeatherDataController : ApiController
    {
        // GET: api/WeatherData
        public IEnumerable<WeatherMessage> Get()
        {
            return WeatherDataFactory.GetLatestWeatherData();
        }

       
    }
}

using AutoMapper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Shared;

namespace WeatherWebApp.Models
{
    public static class WeatherDataFactory
    {
        public static IEnumerable<WeatherMessage> GetLatestWeatherData()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
   ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("weatherdata");
            string rowKeyToUse = string.Format("{0:D19}",
                DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);



            TableQuery<TableWeatherMessage> query = new TableQuery<TableWeatherMessage>().Take(5);


            List<WeatherMessage> list = new List<WeatherMessage>();
            Mapper.Initialize(cfg => cfg.CreateMap<TableWeatherMessage, WeatherMessage>());

            foreach (TableWeatherMessage entity in table.ExecuteQuery(query))
            {
                WeatherMessage wm = Mapper.Map<WeatherMessage>(entity);
                list.Add(wm);
            }

            return list;
        }
    }
}

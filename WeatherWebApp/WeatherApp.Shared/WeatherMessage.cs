namespace WeatherApp.Shared
{
    public class WeatherMessage
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string DeviceID { get; set; }
        public string Temperature { get; set; }
        public string Altitude { get; set; }
        public string Humidity { get; set; }
        public string Pressure { get; set; }
        public string CurrentDateTime { get; set; }
    }
}
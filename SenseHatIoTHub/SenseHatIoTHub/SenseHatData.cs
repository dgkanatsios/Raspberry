using System;

namespace SenseHatIoTHub
{
    public class SenseHatData
    {
        public double? Humidity { get; set; }
        public double? Pressure { get; set; }
        public double? Temperature { get; set; }
        public string Location { get; set; }
    }
}
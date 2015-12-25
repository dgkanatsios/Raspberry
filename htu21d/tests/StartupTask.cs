using System;
using Windows.ApplicationModel.Background;
using Microsoft.Maker.Devices.I2C.HTU21D;

namespace HTU21DTest
{
    public sealed class StartupTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            HTU21D sensor = new HTU21D();
            bool result = await sensor.BeginAsync();
            if (result)
            {
                float humidity = sensor.Humidity;
                float temperature = sensor.Temperature;
                float dewPoint = sensor.DewPoint;
                System.Diagnostics.Debug.WriteLine("**************************************************");
                System.Diagnostics.Debug.WriteLine("HTU21D Test Verification");
                System.Diagnostics.Debug.WriteLine("**************************************************");
                System.Diagnostics.Debug.WriteLine("HUMIDITY    = [{0}]", humidity);
                System.Diagnostics.Debug.WriteLine("TEMPERATURE = [{0}]", temperature);
                System.Diagnostics.Debug.WriteLine("DEWPOINT    = [{0}]", dewPoint);
                if (temperature < 10 || humidity < 1)
                {
                    System.Diagnostics.Debug.WriteLine(">>> Failed. Received bad values from sensor");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(">>> Passed.");
                }
            }
        }
    }
}
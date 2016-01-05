using System;
using Windows.ApplicationModel.Background;
using Microsoft.Maker.Devices.I2C.MPL3115A2;

namespace MPL3115A2Test
{
    public sealed class StartupTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            MPL3115A2 sensor = new MPL3115A2();
            bool result = await sensor.BeginAsync();
            if (result)
            {
                float altitude = sensor.Altitude;
                float pressure = sensor.Pressure;
                System.Diagnostics.Debug.WriteLine("**************************************************");
                System.Diagnostics.Debug.WriteLine("MPL3115A2 Test Verification");
                System.Diagnostics.Debug.WriteLine("**************************************************");
                System.Diagnostics.Debug.WriteLine("ALTITUDE = [{0}]", altitude);
                System.Diagnostics.Debug.WriteLine("PRESSURE = [{0}]", pressure);
                if (altitude < 0 || pressure < 100000)
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
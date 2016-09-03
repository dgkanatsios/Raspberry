using Emmellsoft.IoT.Rpi.SenseHat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SenseHatIoTHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;

        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //get a reference to SenseHat
            senseHat = await SenseHatFactory.GetSenseHat();
            //initialize the timer
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
        {
            senseHat.Sensors.HumiditySensor.Update();
            senseHat.Sensors.PressureSensor.Update();
            
            //gather data
            SenseHatData data = new SenseHatData();
            data.Temperature = senseHat.Sensors.Temperature;
            data.Humidity = senseHat.Sensors.Humidity;
            data.Pressure = senseHat.Sensors.Pressure;
            data.Location = "Athens";

            //send them to the cloud
            await AzureIoTHub.SendSenseHatDataToCloudAsync(data);

            //notify UI
            TempText.Text = data.Temperature.ToString();
            TempHumidity.Text = data.Humidity.ToString();
            TempPressure.Text = data.Pressure.ToString();
        }

        ISenseHat senseHat;


    }
}

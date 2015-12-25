using Microsoft.Maker.Devices.I2C.Htu21d;
using Microsoft.Maker.Devices.I2C.Mpl3115a2;
using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;

namespace Microsoft.Maker.Sparkfun.WeatherShield
{
    public sealed class WeatherShield
    {
        /// <summary>
        /// LED Control Pins
        /// </summary>
        private int statusLedBluePin;
        private int statusLedGreenPin;

        /// <summary>
        /// Used to signal that the device is properly initialized and ready to use
        /// </summary>
        private bool available = false;

        /// <summary>
        /// A switch to enable/disable the device;
        /// </summary>
        private bool enable = false;

        // Sensors
        private Htu21d htu21d;  // Humidity and temperature sensor
        private Mpl3115a2 mpl3115a2;  // Altitue, pressure and temperature sensor

        /// <summary>
        /// Constructs WeatherShield with I2C bus and status LEDs identified
        /// </summary>
        /// <param name="i2cBusName"></param>
        /// <param name="ledBluePin"></param>
        /// <param name="ledGreenPin"></param>
        public WeatherShield (string i2cBusName, int ledBluePin, int ledGreenPin)
        {
            statusLedBluePin = ledBluePin;
            statusLedGreenPin = ledGreenPin;
            htu21d = new Htu21d(i2cBusName);
            mpl3115a2 = new Mpl3115a2(i2cBusName);
        }

        /// <summary>
        /// Read altitude data
        /// </summary>
        /// <returns>
        /// Calculates the altitude in meters (m) using the US Standard Atmosphere 1976 (NASA) formula
        /// </returns>
        public float Altitude
        {
            get
            {
                if (!enable) { return 0f; }
                return mpl3115a2.Altitude;
            }
        }

        /// <summary>
        /// Initialize the Sparkfun Weather Shield
        /// </summary>
        /// <returns>
        /// Async operation object
        /// </returns>
        public IAsyncOperation<bool> BeginAsync()
        {
            return this.BeginAsyncHelper().AsAsyncOperation<bool>();
        }

        /// <summary>
        /// Blue status LED on shield
        /// </summary>
        /// <remarks>
        /// This object will be created in InitAsync(). The set method will
        /// be marked private, because the object itself will not change, only
        /// the value it drives to the pin.
        /// </remarks>
        public GpioPin BlueLedPin { get; private set; }

        /// <summary>
        /// Read dew point data
        /// </summary>
        /// <returns>
        /// Returns dew point temperature calculated as
        /// -(235.66 + 1762.39 / (log(RelativeHumidity * PartialPressure / 100) - 8.1332))
        /// </returns>
        public float DewPoint
        {
            get
            {
                if (!enable) { return 0f; }
                return htu21d.DewPoint;
            }
        }

        /// <summary>
        /// The current state of the shield
        /// </summary>
            public bool Enable
        {
            get { return enable; }
            set { enable = (available && value); }
        }

        /// <summary>
        /// Green status LED on shield
        /// </summary>
        /// <remarks>
        /// This object will be created in InitAsync(). The set method will
        /// be marked private, because the object itself will not change, only
        /// the value it drives to the pin.
        /// </remarks>
        public GpioPin GreenLedPin { get; private set; }

        /// <summary>
        /// Calculate relative humidity
        /// </summary>
        /// <returns>
        /// The relative humidity
        /// </returns>
        public float Humidity
        {
            get
            {
                if (!enable) { return 0f; }
                return htu21d.Humidity;
            }
        }

        /// <summary>
        /// Read pressure data
        /// </summary>
        /// <returns>
        /// The pressure in Pascals (Pa)
        /// </returns>
        public float Pressure
        {
            get
            {
                if (!enable) { return 0f; }
                return mpl3115a2.Pressure;
            }
        }

        /// <summary>
        /// Calculate current temperature
        /// </summary>
        /// <returns>
        /// The temperature in Celcius (C)
        /// </returns>
        public float Temperature
        {
            get
            {
                if (!enable) { return 0f; }
                return htu21d.Temperature;
            }
        }

        /// <summary>
        /// Initialize the Sparkfun Weather Shield
        /// </summary>
        /// <remarks>
        /// Setup and instantiate the I2C device objects for the HTU21D and the MPL3115A2
        /// and initialize the blue and green status LEDs.
        /// </remarks>
        private async Task<bool> BeginAsyncHelper()
        {
            /*
                * Acquire the GPIO controller
                * MSDN GPIO Reference: https://msdn.microsoft.com/en-us/library/windows/apps/windows.devices.gpio.aspx
                * 
                * Get the default GpioController
                */
            GpioController gpio = GpioController.GetDefault();

            /*
                * Test to see if the GPIO controller is available.
                *
                * If the GPIO controller is not available, this is
                * a good indicator the app has been deployed to a
                * computing environment that is not capable of
                * controlling the weather shield. Therefore we
                * will disable the weather shield functionality to
                * handle the failure case gracefully. This allows
                * the invoking application to remain deployable
                * across the Universal Windows Platform.
                */
            if (null == gpio)
            {
                available = false;
                enable = false;
                return false;
            }

            /*
                * Initialize the blue LED and set to "off"
                *
                * Instantiate the blue LED pin object
                * Write the GPIO pin value of low on the pin
                * Set the GPIO pin drive mode to output
                */
            BlueLedPin = gpio.OpenPin(statusLedBluePin, GpioSharingMode.Exclusive);
            BlueLedPin.Write(GpioPinValue.Low);
            BlueLedPin.SetDriveMode(GpioPinDriveMode.Output);

            /*
                * Initialize the green LED and set to "off"
                * 
                * Instantiate the green LED pin object
                * Write the GPIO pin value of low on the pin
                * Set the GPIO pin drive mode to output
                */
            GreenLedPin = gpio.OpenPin(statusLedGreenPin, GpioSharingMode.Exclusive);
            GreenLedPin.Write(GpioPinValue.Low);
            GreenLedPin.SetDriveMode(GpioPinDriveMode.Output);

            if (!await htu21d.BeginAsync())
            {
                available = false;
                enable = false;
                return false;
            }

            if (!await mpl3115a2.BeginAsync())
            {
                available = false;
                enable = false;
                return false;
            }

            available = true;
            enable = true;
            return true;
        }
    }
}

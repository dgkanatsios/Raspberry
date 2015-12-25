using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Foundation;

namespace Microsoft.Maker.Devices.I2C.Mpl3115a2
{
    /// <summary>
    /// MPL3115A2 precision altimeter IC
    /// http://cache.freescale.com/files/sensors/doc/data_sheet/MPL3115A2.pdf
    /// </summary>
    public sealed class Mpl3115a2
    {
        /// <summary>
        /// Device I2C Bus
        /// </summary>
        private string i2cBusName;

        /// <summary>
        /// Device I2C Address
        /// </summary>
        private const ushort Mpl3115a2I2cAddress = 0x0060;

        /// <summary>
        /// Control registers
        /// </summary>
        private const byte ControlRegister1 = 0x26;

        /// <summary>
        /// Control registers
        /// </summary>
        private const byte PressureDataOutMSB = 0x01;

        /// <summary>
        /// Control Flags
        /// </summary>
        private bool available = false;

        /// <summary>
        /// I2C Device
        /// </summary>
        private I2cDevice i2c;

        /// <summary>
        /// Constructs Mpl3115a2 with I2C bus identified
        /// </summary>
        /// <param name="i2cBusName">
        /// The bus name to provide to the enumerator
        /// </param>
        public Mpl3115a2 (string i2cBusName)
        {
            this.i2cBusName = i2cBusName;
        }

        /// <summary>
        /// Gets the altitude data
        /// </summary>
        /// <returns>
        /// Calculates the altitude in meters (m) using the US Standard Atmosphere 1976 (NASA) formula
        /// </returns>
        public float Altitude
        {
            get
            {
                if (!this.available)
                {
                    return 0f;
                }

                double pressure = this.Pressure;

                // Calculate using US Standard Atmosphere 1976 (NASA)
                double altitude = 44330.77 * (1 - Math.Pow(pressure / 101326, 0.1902632));

                return Convert.ToSingle(altitude);
            }
        }

        /// <summary>
        /// Initialize the Mpl3115a2 device.
        /// </summary>
        /// <returns>
        /// Async operation object.
        /// </returns>
        public IAsyncOperation<bool> BeginAsync()
        {
            return this.BeginAsyncHelper().AsAsyncOperation<bool>();
        }

        /// <summary>
        /// Gets pressure data
        /// </summary>
        /// <returns>
        /// The pressure in Pascals (Pa)
        /// </returns>
        public float Pressure
        {
            get
            {
                if (!this.available)
                {
                    return 0f;
                }

                uint rawPressureData = this.RawPressure;
                double pressurePascals = (rawPressureData >> 6) + (((rawPressureData >> 4) & 0x03) / 4.0);

                return Convert.ToSingle(pressurePascals);
            }
        }

        /// <summary>
        /// Private helper to initialize the altimeter device.
        /// </summary>
        /// <remarks>
        /// Setup and instantiate the I2C device object for the MPL3115A2.
        /// </remarks>
        /// <returns>
        /// Task object.
        /// </returns>
        private async Task<bool> BeginAsyncHelper()
        {
            // Acquire the I2C device
            // MSDN I2C Reference: https://msdn.microsoft.com/en-us/library/windows/apps/windows.devices.i2c.aspx
            //
            // Use the I2cDevice device selector to create an advanced query syntax string
            // Use the Windows.Devices.Enumeration.DeviceInformation class to create a collection using the advanced query syntax string
            // Take the device id of the first device in the collection
            string advancedQuerySyntax = I2cDevice.GetDeviceSelector(i2cBusName);
            DeviceInformationCollection deviceInformationCollection = await DeviceInformation.FindAllAsync(advancedQuerySyntax);
            string deviceId = deviceInformationCollection[0].Id;

            // Establish an I2C connection to the MPL3115A2
            //
            // Instantiate the I2cConnectionSettings using the device address of the MPL3115A2
            // - Set the I2C bus speed of connection to fast mode
            // - Set the I2C sharing mode of the connection to shared
            //
            // Instantiate the the MPL3115A2 I2C device using the device id and the I2cConnectionSettings
            I2cConnectionSettings mpl3115a2Connection = new I2cConnectionSettings(Mpl3115a2I2cAddress);
            mpl3115a2Connection.BusSpeed = I2cBusSpeed.FastMode;
            mpl3115a2Connection.SharingMode = I2cSharingMode.Shared;

            this.i2c = await I2cDevice.FromIdAsync(deviceId, mpl3115a2Connection);

            // Test to see if the I2C devices are available.
            //
            // If the I2C devices are not available, this is
            // a good indicator the weather shield is either
            // missing or configured incorrectly. Therefore we
            // will disable the weather shield functionality to
            // handle the failure case gracefully. This allows
            // the invoking application to remain deployable
            // across the Universal Windows Platform.
            //
            // NOTE: For a more detailed description of the I2C
            // transactions used for testing below, please
            // refer to the "Raw___" functions provided below.
            if (null == this.i2c)
            {
                this.available = false;
                return this.available;
            }
            else
            {
                byte[] data = new byte[1];

                try
                {
                    this.i2c.WriteRead(new byte[] { Mpl3115a2.ControlRegister1 }, data);

                    // ensure SBYB (bit 0) is set to STANDBY
                    data[0] &= 0xFE;

                    // ensure OST (bit 1) is set to initiate measurement
                    data[0] |= 0x02;
                    this.i2c.Write(new byte[] { Mpl3115a2.ControlRegister1, data[0] });

                    this.available = true;
                }
                catch
                {
                    this.available = false;
                }
            }

            return this.available;
        }

        /// <summary>
        /// Gets the raw pressure value from the IC.
        /// </summary>
        private uint RawPressure
        {
            get
            {
                uint pressure = 0;
                byte[] data = new byte[1];
                byte[] rawPressureData = new byte[3];

                // Request pressure data from the MPL3115A2
                // MPL3115A2 datasheet: http://dlnmh9ip6v2uc.cloudfront.net/datasheets/Sensors/Pressure/MPL3115A2.pdf
                //
                // Update Control Register 1 Flags
                // - Read data at CTRL_REG1 (0x26) on the MPL3115A2
                // - Update the SBYB (bit 0) and OST (bit 1) flags to STANDBY and initiate measurement, respectively.
                // -- SBYB flag (bit 0)
                // --- off = Part is in STANDBY mode
                // --- on = Part is ACTIVE
                // -- OST flag (bit 1)
                // --- off = auto-clear
                // --- on = initiate measurement
                // - Write the resulting value back to Control Register 1
                this.i2c.WriteRead(new byte[] { Mpl3115a2.ControlRegister1 }, data);
                data[0] &= 0xFE;  // ensure SBYB (bit 0) is set to STANDBY
                data[0] |= 0x02;  // ensure OST (bit 1) is set to initiate measurement
                this.i2c.Write(new byte[] { Mpl3115a2.ControlRegister1, data[0] });

                // Wait 10ms to allow MPL3115A2 to process the pressure value
                Task.Delay(10);

                // Write the address of the register of the most significant byte for the pressure value, OUT_P_MSB (0x01)
                // Read the three bytes returned by the MPL3115A2
                // - byte 0 - MSB of the pressure
                // - byte 1 - CSB of the pressure
                // - byte 2 - LSB of the pressure
                this.i2c.WriteRead(new byte[] { Mpl3115a2.PressureDataOutMSB }, rawPressureData);

                // Reconstruct the result using all three bytes returned from the device
                pressure = (uint)(rawPressureData[0] << 16);
                pressure |= (uint)(rawPressureData[1] << 8);
                pressure |= rawPressureData[2];

                return pressure;
            }
        }
    }
}

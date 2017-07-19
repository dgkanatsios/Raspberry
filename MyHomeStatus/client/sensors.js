const GrovePi = require('node-grovepi').GrovePi;

module.exports = {
    GroveLCDRGBDisplay: require('./mysensors/GroveLCDRGBDisplay'),
    leds: require('./mysensors/leds'),
    SoundAnalogSensor: require('./mysensors/sound'),
    DHTDigitalSensor: GrovePi.sensors.DHTDigital,
    LightAnalogSensor: GrovePi.sensors.LightAnalog
}
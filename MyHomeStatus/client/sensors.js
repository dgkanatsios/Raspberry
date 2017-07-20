const GrovePi = require('node-grovepi').GrovePi;

module.exports = {
    GroveLCDRGBDisplay: require('./mysensors/GroveLCDRGBDisplay'),
    Leds: require('./mysensors/leds'),
    LoudnessSensor: require('./mysensors/LoudnessSensor'),
    DHTDigitalSensor: GrovePi.sensors.DHTDigital,
    LightAnalogSensor: GrovePi.sensors.LightAnalog,
    RotaryAngleSensor: require('./mysensors/RotaryAngleSensor'),
    ButtonSensor: require('./mysensors/DigitalButton')
}
const GrovePi = require('node-grovepi').GrovePi;

module.exports = {
    GroveLCDRGBDisplay: require('./mysensors/GroveLCDRGBDisplay'),
    Leds: require('./mysensors/leds'),
    LoudnessSensor: require('./mysensors/LoudnessSensor'),
    DHTDigitalSensor: GrovePi.sensors.DHTDigital,
    CustomLightAnalogSensor: require('./mysensors/CustomLightAnalogSensor'),
    RotaryAngleSensor: require('./mysensors/RotaryAngleSensor'),
    ButtonSensor: require('./mysensors/DigitalButton'),
    DustSensor: require('./mysensors/CustomDustDigitalSensor'),
    MotionSensor: GrovePi.sensors.DigitalInput,
    Buzzer : GrovePi.sensors.DigitalOutput
}
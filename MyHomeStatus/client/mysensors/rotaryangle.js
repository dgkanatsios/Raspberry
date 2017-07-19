const AnalogSensor = require('node-grovepi').GrovePi.sensors.base.Analog;
const helpers = require('../helpers');

function RotaryAngleSensor(pin) {
    AnalogSensor.apply(this, Array.prototype.slice.call(arguments));
}
RotaryAngleSensor.prototype = new AnalogSensor();

RotaryAngleSensor.prototype.read = function () {
    let value = AnalogSensor.prototype.read.call(this);

    //http://wiki.seeed.cc/Grove-Rotary_Angle_Sensor/
    let adc_ref = 5;
    let grove_vcc = 5;
    let full_angle = 300;

    let voltage = helpers.round((value) * adc_ref / 1023, 2);

    //Calculate rotation in degrees(0 to 300)
    let degrees = helpers.round((voltage * full_angle) / grove_vcc, 2)

    //Calculate LED brightess(0 to 100) from degrees(0 to 300)
    let brightness = Math.floor(degrees / full_angle * 100)

    return brightness;
}

module.exports = RotaryAngleSensor;
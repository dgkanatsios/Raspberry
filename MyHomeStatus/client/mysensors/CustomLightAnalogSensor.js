const LightAnalogSensor = require('node-grovepi').GrovePi.sensors.LightAnalog;


function CustomLightAnalogSensor(pin) {
  LightAnalogSensor.apply(this, Array.prototype.slice.call(arguments));
}
CustomLightAnalogSensor.prototype = new LightAnalogSensor();

CustomLightAnalogSensor.prototype.read = function() {
  const res = LightAnalogSensor.prototype.read.call(this);
  //noise values
  if(res == 0 || res == 5105 || res == 10220)
    return null;
  else 
    return res;
}

module.exports = LightAnalogSensor
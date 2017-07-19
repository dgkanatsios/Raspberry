const AnalogSensor = require('node-grovepi').GrovePi.sensors.base.Analog;
const helpers = require('../helpers');

function SoundAnalogSensor(pin, sps) {
  AnalogSensor.apply(this, Array.prototype.slice.call(arguments));
  this.samplespersecond = sps || 5;
  this.results = new Array();
  this.samples = 0;
}
SoundAnalogSensor.prototype = new AnalogSensor();

SoundAnalogSensor.prototype.readCurrent = function () {
  return AnalogSensor.prototype.read.call(this);
}

SoundAnalogSensor.prototype.read = function () {
  if (this.results.length == 0)
    throw new Error('no results. Did you call start()?');

  let sum = this.results.reduce(
    (acc, cur) => acc + cur, 0);

  let result = sum / this.results.length;
  this.results.length = 0;
  return helpers.round(result, 2);
}

SoundAnalogSensor.prototype.start = function () {
  loop.bind(this)();
  setInterval(loop.bind(this), 1000 / this.samplespersecond);
}

SoundAnalogSensor.prototype.stop = function () {
  clearInterval(loop);
}

function loop() {
  let currentResult = this.readCurrent();
  this.results.push(currentResult);
}

module.exports = SoundAnalogSensor;
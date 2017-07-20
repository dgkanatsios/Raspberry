const AnalogSensor = require('node-grovepi').GrovePi.sensors.base.Analog;
const helpers = require('../helpers');

function LoudnessSensor(pin, samplespersecond) {
  AnalogSensor.apply(this, Array.prototype.slice.call(arguments));
  this.samplespersecond = samplespersecond || 5;
  this.results = new Array();
  this.samples = 0;
}
LoudnessSensor.prototype = new AnalogSensor();

LoudnessSensor.prototype.readCurrent = function () {
  return AnalogSensor.prototype.read.call(this);
}

LoudnessSensor.prototype.read = function () {
  if (this.results.length == 0)
    throw new Error('no results. Did you call start()?');

  let sum = this.results.reduce((acc, cur) => acc + cur, 0);

  let result = sum / this.results.length;
  this.results = new Array();
  return helpers.round(result, 2);
}

LoudnessSensor.prototype.start = function () {
  loop.bind(this)();
  setInterval(loop.bind(this), 1000 / this.samplespersecond);
}

LoudnessSensor.prototype.stop = function () {
  clearInterval(loop);
}

function loop() {
  this.readCurrent();
  let currentResult = this.readCurrent();
  this.results.push(currentResult);
}

module.exports = LoudnessSensor;
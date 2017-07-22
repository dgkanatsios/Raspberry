const AnalogSensor = require('node-grovepi').GrovePi.sensors.base.Analog;
const helpers = require('../helpers');

function LoudnessSensor(pin, samplespersecond) {
  AnalogSensor.apply(this, Array.prototype.slice.call(arguments));
  this.samplespersecond = samplespersecond || 5;
  this.results = new Array();
}
LoudnessSensor.prototype = new AnalogSensor();

LoudnessSensor.prototype.readAvgMax = function () {
  if (this.results.length == 0)
    throw new Error('no results. Did you call start()?');

  let sum = this.results.reduce((acc, cur) => acc + cur, 0);
  let avg = sum / this.results.length;

  let max = this.results.reduce(function (a, b) {
    return Math.max(a, b);
  });

  //reset the array
  this.results = new Array();

  return {
    avg: helpers.round(avg, 2),
    max: helpers.round(max, 2)
  };
}

LoudnessSensor.prototype.start = function () {
  loop.bind(this)();
  setInterval(loop.bind(this), 1000 / this.samplespersecond);
}

LoudnessSensor.prototype.stop = function () {
  clearInterval(loop);
}

function loop() {
  let currentResult = this.read();
  this.results.push(currentResult);
}

module.exports = LoudnessSensor;
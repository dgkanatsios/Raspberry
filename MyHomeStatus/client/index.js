'use strict';
const GrovePi = require('node-grovepi').GrovePi;
const helpers = require('./helpers');

var Commands = GrovePi.commands;
var Board = GrovePi.board;

const DHTDigitalSensor = GrovePi.sensors.DHTDigital;

const board = new Board({
    debug: true,
    onError: function (err) {
        console.log('Something wrong just happened');
        console.log(err);
    },
    onInit: function (res) {
        if (res) {
            console.log('GrovePi Version :: ' + board.version())

            var dhtsensor = new DHTDigitalSensor(3, DHTDigitalSensor.VERSION.DHT11, DHTDigitalSensor.CELSIUS);
            console.log('Temperature Sensor (start watch)');
            dhtsensor.on('change', function (res) {
                const result = helpers.parsedht(res);
                console.log('Temperature onChange value=' + JSON.stringify(result));
            })
            dhtsensor.watch();
        }
    }
})

function onExit(err) {
  console.log('ending');
  board.close();
  process.removeAllListeners();
  process.exit();
  if (typeof err != 'undefined')
    console.log(err);
}


board.init();
// catches ctrl+c event
process.on('SIGINT', onExit);
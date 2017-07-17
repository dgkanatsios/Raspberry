'use strict';
require('dotenv').config();
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

            const dhtsensor = new DHTDigitalSensor(3, DHTDigitalSensor.VERSION.DHT11, DHTDigitalSensor.CELSIUS);
            console.log('Temperature Sensor (start watch)');
            dhtsensor.on('change', function (res) {
                console.log('Temperature onChange value (temp,hum,heatindex):' + res);
                const result = helpers.parsedht(res);
                if (result !== null) { //if valid temperature
                    helpers.postData(result).then(x => console.log(x)).catch(err => console.log(err));
                }
            })
            dhtsensor.watch(1000);
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
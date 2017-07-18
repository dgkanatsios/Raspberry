'use strict';
require('dotenv').config();
const GrovePi = require('node-grovepi').GrovePi;
const helpers = require('./helpers');

const GroveLCDRGBDisplay = require('./GroveLCDRGBDisplay');
const DHTDigitalSensor = GrovePi.sensors.DHTDigital;
const LightAnalogSensor = GrovePi.sensors.LightAnalog;

const i2c = require('i2c-bus');
const i2c1 = i2c.openSync(1);
let lcd = null;

let dhtsensor = null;
let lightsensor = null;

const leds = require('./leds');
leds.turnBothLedsOff();

const Commands = GrovePi.commands;
const Board = GrovePi.board;

const loopInterval = 1000 * 60;

const board = new Board({
    debug: true,
    onError: function (err) {
        leds.changeRedStatus(true);
        console.log('Something wrong just happened');
        console.log(err);
    },
    onInit: function (res) {
        if (res) {
            leds.changeGreenStatus(true);
            console.log('GrovePi Version :: ' + board.version())

            dhtsensor = new DHTDigitalSensor(3, DHTDigitalSensor.VERSION.DHT11, DHTDigitalSensor.CELSIUS);
            console.log('Temperature sensor initialized');

            lcd = new GroveLCDRGBDisplay(i2c1);
            lcd.setRGB(123, 98, 45);
            console.log('RBG LCD initialized');

            lightsensor = new LightAnalogSensor(0);
            console.log('Light sensor initialized');

            loop();
            setInterval(loop, loopInterval);
        }
    }
});


function loop() {
    const resTemp = dhtsensor.read();
    const resLight = lightsensor.read();
    console.log('Current light intensity:' + resLight);
    if (resTemp) {
        console.log('Current temperature  value (temp,hum,heatindex):' + resTemp);
        leds.changeGreenStatus(true);
        const result = helpers.parsedht(resTemp);
        if (result !== null) { //if valid temperature
            result.light = resLight; //add light to the object
            helpers.postData(result).then(x => console.log(x)).catch(err => console.log(err));
            lcd.setText(`temp ${result.temperature},hum ${result.humidity},HI ${result.heatIndex},L ${result.light}`);
        }
    } else {
        leds.changeRedStatus(true);
        lcd.setText('error getting temperature');
    }
}


function onExit(err) {
    leds.turnBothLedsOff();
    console.log('ending');
    lcd.turnOffDisplay();
    board.close();
    process.removeAllListeners();
    process.exit();
    if (typeof err != 'undefined')
        console.log(err);
}


board.init();
// catches ctrl+c event
process.on('SIGINT', onExit);
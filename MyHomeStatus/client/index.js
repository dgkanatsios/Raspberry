'use strict';
require('dotenv').config();
const GrovePi = require('node-grovepi').GrovePi;
const helpers = require('./helpers');

const i2c = require('i2c-bus');
const i2c1 = i2c.openSync(1);
let lcd = null;

let dhtsensor = null;
let lightsensor = null;
let soundsensor = null;

const sensors = require('./sensors');


const Commands = GrovePi.commands;
const Board = GrovePi.board;

const loopInterval = 1000 * 60;

const board = new Board({
    debug: true,
    onError: function (err) {
        console.log('Something wrong just happened');
        console.log(err);
    },
    onInit: function (res) {
        if (res) {
            console.log('GrovePi Version :: ' + board.version())
            sensors.leds.changeRedStatus(false);
            dhtsensor = new sensors.DHTDigitalSensor(3, sensors.DHTDigitalSensor.VERSION.DHT11, sensors.DHTDigitalSensor.CELSIUS);
            console.log('Temperature sensor initialized');

            lcd = new sensors.GroveLCDRGBDisplay(i2c1);
            lcd.setRGB(123, 98, 45);
            console.log('RBG LCD initialized');

            lightsensor = new sensors.LightAnalogSensor(0);
            console.log('Light sensor initialized');

            soundsensor = new sensors.SoundAnalogSensor(2,5);
            soundsensor.start();
            console.log('Sound sensor initialized');

            loop();
            setInterval(loop, loopInterval);
        }
    }
});


function loop() {
    const resLight = lightsensor.read();
    console.log('Current light intensity:' + resLight);

    const resSound = soundsensor.read();
    console.log('Current sound value:' + resSound);

    const resTemp = dhtsensor.read();
    if (resTemp) {
        console.log('Current temperature  value (temp,hum,heatindex):' + resTemp);
        const result = helpers.parsedht(resTemp);
        if (result !== null) { //if valid temperature

            //add light and sound to the object
            result.light = resLight; 
            result.sound = resSound;
            
            helpers.postData(result).then(response => console.log(response)).catch(err => handleError(err));
            lcd.setText(`temp ${result.temperature},hum ${result.humidity},HI ${result.heatIndex},L ${result.light}`);
        }
    } else {
        handleError('error getting temperature');
    }
}

function handleError(err) {
    console.log(err);
    sensors.leds.changeRedStatus(true); //notify that there has been an error
    lcd.setText(err);
}

function onExit(err) {
    clearInterval(loop);
    console.log('ending');
    sensors.leds.changeRedStatus(false);
    lcd.turnOffDisplay();
    board.close();
    process.removeAllListeners();
    process.exit();
    if (typeof err != 'undefined')
        handleError(err);
}


board.init();
// catches ctrl+c event
process.on('SIGINT', onExit);
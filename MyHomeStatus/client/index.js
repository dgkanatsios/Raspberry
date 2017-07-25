'use strict';
require('dotenv').config();
const GrovePi = require('node-grovepi').GrovePi;
const helpers = require('./helpers');
const DEBUG = false;

const i2c = require('i2c-bus');
const i2c1 = i2c.openSync(1);

let dhtsensor = null,
    lightsensor = null,
    loudnessSensor = null,
    lcd = null,
    dust = null;

const sensors = require('./sensors');
const huehandler = require('./hue/huehandler');

const deviceID = 1;

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
            sensors.Leds.changeRedStatus(false);
            dhtsensor = new sensors.DHTDigitalSensor(3, sensors.DHTDigitalSensor.VERSION.DHT22, sensors.DHTDigitalSensor.CELSIUS);
            console.log('Temperature sensor initialized');

            lcd = new sensors.GroveLCDRGBDisplay(i2c1);
            lcd.setRGB(123, 98, 45);
            console.log('RBG LCD initialized');

            lightsensor = new sensors.CustomLightAnalogSensor(0);
            console.log('Light sensor initialized');

            loudnessSensor = new sensors.LoudnessSensor(2, 5);
            loudnessSensor.start();
            console.log('Loudness sensor initialized');

            dust = new sensors.DustSensor(2);
            dust.start();
            console.log('Dust sensor initialized');

            huehandler.start();
            console.log('Hue handler initialized');

            loop();
            setInterval(loop, loopInterval);
        }
    }
});


function loop() {
    const resLight = lightsensor.read();
    if (DEBUG) console.log('Current light intensity:' + resLight);

    const resLoudness = loudnessSensor.readAvgMax();
    if (DEBUG) console.log(`Current avg sound value: ${resLoudness.avg}, max: ${resLoudness.max}`);

    const resDust = dust.readAvgMax();
    if(DEBUG) console.log(`Current dust concentration value: ${resDust.avg}, max: ${resDust.max}`);

    const resTemp = dhtsensor.read();
    if (resTemp) {
        if (DEBUG) console.log('Current temperature  value (temp,hum,heatindex):' + resTemp);
        const result = helpers.parsedht(resTemp);
        if (result !== null) { //if valid temperature

            //add rest of the properties
            result.light = resLight || helpers.NOT_AVAILABLE;
            result.soundAvg = resLoudness.avg;
            result.soundMax = resLoudness.max;
            result.dustAvg = resDust.avg;
            result.dustMax = resDust.max;
            result.deviceID = deviceID;

            helpers.postData(result).then(response => console.log(response)).catch(err => handleError(err));
            lcd.setText(`T${helpers.round(result.temperature,1)},H${helpers.round(result.humidity,1)},HI${helpers.round(result.heatIndex,1)},L${helpers.round(result.light,0)},S${helpers.round(result.soundAvg,0)},D${helpers.round(result.dustAvg),1}`);
        }
    } else {
        handleError('error getting temperature');
    }
}

function handleError(err) {
    console.log(err);
    sensors.Leds.changeRedStatus(true); //notify that there has been an error
    lcd.setText(err);
}

function onExit(err) {
    console.log('ending');

    clearInterval(loop);

    sensors.Leds.changeRedStatus(false);
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
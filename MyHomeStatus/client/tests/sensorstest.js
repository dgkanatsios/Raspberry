const GrovePi = require('node-grovepi').GrovePi;
const Board = GrovePi.board;
const helpers = require('./../helpers');
const sensors = require('../sensors');

let board = null,
    loudness = null,
    dhtsensor = null,
    lightAnalog = null,
    rotaryAngle = null;
let button = null;

const options = {
    testRotary: true,
    testLoudness: true,
    testButton: true,
    testDHT: false,
    testLightAnalog: false
}

function start() {
    console.log('starting')

    board = new Board({
        debug: true,
        onError: function (err) {
            console.log('test error:' + err);
        },
        onInit: function (res) {
            if (res) {
                if (options.testLoudness) {
                    loudness = new sensors.LoudnessSensor(2, 5);
                    loudness.start();
                    setInterval(loudnessLoop, 10000);
                }

                if (options.testRotary) {
                    rotaryAngle = new sensors.RotaryAngleSensor(1);
                    rotaryAngle.start();
                    rotaryAngle.on('data', function (res) {
                        console.log('Rotary angle: ' + res);
                    });
                }

                if (options.testButton) {
                    button = new sensors.ButtonSensor(4);
                    button.on('down', function (res) {
                        console.log('Button down: ' + res);
                    });
                    button.watch();
                }

                if(options.testDHT){
                    dhtsensor = new sensors.DHTDigitalSensor(3, sensors.DHTDigitalSensor.VERSION.DHT22, sensors.DHTDigitalSensor.CELSIUS);
                    setInterval(dhtLoop, 200);
                }

                if(options.testLightAnalog){
                    lightAnalog = new sensors.CustomLightAnalogSensor(0);
                    setInterval(lightanalogLoop, 200);
                }

            } else {
                console.log('Error: test cannot start, problem in the board?');
            }
        }
    })
    board.init();
}


function loudnessLoop() {
    if (!loudness) throw Error('you need to initialize the sensor');
    let res = loudness.readAvgMax();
    console.log(`Current avg sound value: ${res.avg}, max: ${res.max}`);
}

function dhtLoop(){
    let res = dhtsensor.read();
    let resTemp = helpers.parsedht(res);
    console.log('Current temperature  value (temp,hum,heatindex):' + JSON.stringify(resTemp));
}

function lightanalogLoop(){
    let res = lightAnalog.read();
    console.log('Current light value:' + res);
}

function onExit(err) {
    console.log('ending');

    clearInterval(dhtLoop);
    clearInterval(loudnessLoop);
    clearInterval(lightanalogLoop);
    if(options.testLoudness) loudness.stop();
    if(options.testRotary) rotaryAngle.stop();

    board.close();
    process.removeAllListeners();
    process.exit();
    if (typeof err != 'undefined')
        console.log(err);
}

// starts the test
start();
// catches ctrl+c event
process.on('SIGINT', onExit);
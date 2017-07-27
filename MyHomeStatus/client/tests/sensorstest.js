const GrovePi = require('node-grovepi').GrovePi;
const Board = GrovePi.board;
const helpers = require('./../helpers');
const sensors = require('../sensors');

let board = null,
    loudness = null,
    dhtsensor = null,
    lightAnalog = null,
    rotaryAngle = null,
    dust = null,
    motion = null,
    buzzer = null;

let button = null;

const options = {
    testRotary: false,
    testLoudness: false,
    testButton: false,
    testDHT: false,
    testLightAnalog: false,
    testDust: true,
    testMotion: true,
    testBuzzerOnMotion: false
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
                    rotaryAngle = new sensors.RotaryAngleSensor(0);
                    rotaryAngle.start();
                    rotaryAngle.on('data', function (res) {
                        console.log('Rotary angle: ' + res);
                    });
                }

                if (options.testButton) {
                    button = new sensors.ButtonSensor(7);
                    button.on('down', function (res) {
                        console.log('Button down: ' + res);
                    });
                    button.watch();
                }

                if (options.testDHT) {
                    dhtsensor = new sensors.DHTDigitalSensor(3, sensors.DHTDigitalSensor.VERSION.DHT22, sensors.DHTDigitalSensor.CELSIUS);
                    setInterval(dhtLoop, 200);
                }

                if (options.testLightAnalog) {
                    lightAnalog = new sensors.CustomLightAnalogSensor(1);
                    setInterval(lightanalogLoop, 200);
                }

                if (options.testDust) {
                    dust = new sensors.DustSensor(2);
                    dust.start();
                    setInterval(dustLoop, 30 * 1000);
                }

                if (options.testBuzzerOnMotion) {
                    buzzer = new sensors.Buzzer(4);
                }

                if (options.testMotion) {
                    motion = new sensors.MotionSensor(8);
                    motion.on('change', function (res) {
                        console.log(res);
                        if (res === 1 && options.testBuzzerOnMotion) {
                            buzzer.turnOn();
                            setTimeout((() => buzzer.turnOff()), 5000);
                        }
                    });
                    motion.watch(200);
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

function dhtLoop() {
    let res = dhtsensor.read();
    let resTemp = helpers.parsedht(res);
    console.log('Current temperature  value (temp,hum,heatindex):' + JSON.stringify(resTemp));
}

function lightanalogLoop() {
    let res = lightAnalog.read();
    console.log('Current light value:' + res);
}

function dustLoop() {
    if (!dust) throw Error('you need to initialize the sensor');

    let res = dust.readAvgMax();
    console.log(`Current avg concentration ${res.avg} and max: ${res.max} pcs/0.01cf`);
}

function onExit(err) {
    console.log('ending');

    clearInterval(dhtLoop);
    clearInterval(loudnessLoop);
    clearInterval(lightanalogLoop);
    clearInterval(dustLoop);

    if (options.testLoudness) loudness.stop();
    if (options.testRotary) rotaryAngle.stop();
    if (options.testDust) dust.stop();

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
const GrovePi = require('node-grovepi').GrovePi;
const Commands = GrovePi.commands
const Board = GrovePi.board

const sensors = require('../sensors');

let board;
let loudness = null;

function start() {
    console.log('starting')

    board = new Board({
        debug: true,
        onError: function (err) {
            console.log('TEST ERROR');
            console.log(err);
        },
        onInit: function (res) {
            if (res) {
                loudness = new sensors.SoundAnalogSensor(2, 5);
                loudness.start();
                setInterval(soundLoop, 1000);
            } else {
                console.log('TEST CANNOT START');
            }
        }
    })
    board.init();
}


function soundLoop() {
    if (!loudness) throw Error('you need to initialize the sensor');
    let res = loudness.read();
    console.log('Sound value=' + res);
}

function onExit(err) {
    console.log('ending');
    loudness.stop();
    clearInterval(soundLoop);
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
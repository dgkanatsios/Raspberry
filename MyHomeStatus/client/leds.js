const GrovePi = require('node-grovepi').GrovePi;

const redled = new GrovePi.sensors.DigitalOutput(5);
const greenled = new GrovePi.sensors.DigitalOutput(6);

function turnBothLedsOff(){
    redled.turnOff();
    greenled.turnOff();
}

function changeGreenStatus(status){
    if(status)
        greenled.turnOn();
    else
        greenled.turnOff();
}

function changeRedStatus(status){
    if(status)
        redled.turnOn();
    else
        redled.turnOff();
}

module.exports = {
    turnBothLedsOff,
    changeGreenStatus,
    changeRedStatus
}

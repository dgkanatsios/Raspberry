const GrovePi = require('node-grovepi').GrovePi;

const redled = new GrovePi.sensors.DigitalOutput(5);


function changeRedStatus(status){
    if(status)
        redled.turnOn();
    else
        redled.turnOff();
}

module.exports = {
    changeRedStatus
}

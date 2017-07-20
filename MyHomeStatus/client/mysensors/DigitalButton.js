'use strict';
const DigitalInput = require('node-grovepi').GrovePi.sensors.DigitalInput;

const buttonMode = {
    UP: 0,
    DOWN: 1
};

let mode = buttonMode.UP; //not pressed

let datetimePressed;

function DigitalButton(pin, watchDelay) {
    DigitalInput.apply(this, Array.prototype.slice.call(arguments));
    let wd = watchDelay || 10;
    this.on('change', function (res) {
        //user presses the button for the first time
        if (res == 1 && mode === buttonMode.UP) {
            mode = buttonMode.DOWN;
            datetimePressed = new Date();
            return;
        }
        //user continues to press the button
        else if (res == 1 && mode === buttonMode.DOWN) {
            //do nothing
            return;
        } else { //res == 0 so user has lifted her finger
            let currentDateTime = new Date();
            let miliseconds = currentDateTime.getTime() - datetimePressed.getTime();
            //if less than 1.1 seconds
            if (miliseconds <= 1100) {
                this.emit('down', 'singlepress');
            } else {
                this.emit('down', 'longpress');
            }
            //reset the mode
            mode = buttonMode.UP;
        }
    });
    this.watch(wd);
}
DigitalButton.prototype = new DigitalInput();

module.exports = DigitalButton;
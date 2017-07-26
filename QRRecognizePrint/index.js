'use strict';
const Zbar = require('zbar');

//thermal printer
//const printer = require('./thermal')('/dev/serial0', 19200);
//zj58 USB printer
const printhelper = require('./printhelper');

function testApiCall() {
    const apihelper = require('./apihelper');
    const api = new apihelper(process.env.SERVER_URL);
    api.getReservation('109841').then(res => {
        //console.log(res.Result);
       
        process.exit();
    }).catch(err => {
        console.log(err);
        process.exit();
    });
}



const zbar = new Zbar('/dev/video0', {
    width: 320,
    height: 240
});

console.log('zbar successfully initialized');

zbar.stdout.on('data', function (result) {
    console.log('data scanned : ' + result);

    var buffer = new Buffer(require('fs').readFileSync('a.txt', 'utf-8'), 'base64');
    printhelper.printBuffer(buffer, 'zj58');

    //check if we have not already scanned this image
    // if (!qrResults.includes(result)) {
    //     qrResults.push(result); //save this result so we don't 'recognize' it again
    //     console.log(`QR success: ${JSON.stringify(result)}`);
    //     printer.printText(JSON.stringify(result));
    // }
});

zbar.stderr.on('data', function (buf) {
    console.log(buf.toString());
});
'use strict';
const qrResults = new Array();
//thermal printer
//const printer = require('./thermal')('/dev/serial0', 19200);
//zj58 USB printer
const printer = require('./usbprinter')('zj58');


function testApiCall() {
    const apihelper = require('./apihelper');
    const api = new apihelper(process.env.SERVER_URL);
    api.getReservation('109841').then(res => {
        //console.log(res.Result);
        let toPrint;
        res.Result.forEach(item => {
            toPrint += item.Text;
        });
        printer.printText(toPrint);
        process.exit();
    }).catch(err => {
        console.log(err);
        process.exit();
    });
}

const Zbar = require('zbar');

const zbar = new Zbar('/dev/video0', {
    width: 320,
    height: 240
});


zbar.stdout.on('data', function (result) {
    console.log('data scanned : ' + result.toString());
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
'use strict';
const NodeWebcam = require('node-webcam');
const uuidv4 = require('uuid/v4');
const QrCode = require('qrcode-reader');
const fs = require('fs');
const ImageParser = require('image-parser');
const interval = 1000;
const thermal = require('./thermal')('/dev/serial0', 19200);
const qrResults = new Array();
const deleteOldImages = false;

const opts = {
    //width: 1280,
    //height: 720,
    width: 1920,
    height: 1080,
    quality: 80,
    //quality: 100,
    //Delay to take shot
    delay: 0,
    //Save shots in memory
    saveShots: true,
    // Webcam.OutputTypes
    output: "jpeg",
    //Which camera to use
    //Use Webcam.list() for results
    //false for default device
    device: false,
    // [location, buffer, base64]
    // Webcam.CallbackReturnTypes
    callbackReturn: "location",
    //Logging
    verbose: false
};

//delete all previous photos
const exec = require('child_process').exec;
exec('rm -rf *.jpg');

//Creates webcam instance
const Webcam = NodeWebcam.create(opts);

//creates QR recognizer instance
const qr = new QrCode();

let previousImage = '';
let processingPending = false;
setInterval(process, interval);

qr.callback = function (error, result) {
    if (error) {
        console.log(`QR error: ${error}`);
        return;
    }
    if (!qrResults.includes(result.result)) {
        qrResults.push(result.result);
        console.log(`QR success: ${JSON.stringify(result)}`);
        thermal.write(JSON.stringify(result));
    }
}

function process() {
    if (processingPending) return;
    processingPending = true;

    Webcam.capture(uuidv4(), function (err, data) {
        if (err) {
            console.log(err);
        }
        else {
            console.log(`image saved at ${data}`);
            if (previousImage != '' && deleteOldImages)
                fs.unlinkSync(previousImage);
            previousImage = data;
            const c = fs.readFileSync(__dirname + `/${data}`);
            const img = new ImageParser(c);
            img.parse(function (err) {
                if (err) {
                    console.log(err);
                }
                qr.decode({ width: img.width(), height: img.height() }, img._imgBuffer);
            });
        }
        processingPending = false;
    });
}







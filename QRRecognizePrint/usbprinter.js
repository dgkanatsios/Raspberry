'use strict';
const Printer = require('node-printer');
//http://192.168.1.18:631/help/options.html#TEXTOPTIONS
const options = {
    //media: 'Custom.200x600mm',
    //n: 3,
    cpi: 15
};


function UsbPrinter(name) {
    if (!(this instanceof UsbPrinter)) return new UsbPrinter(name);
    this.printer = new Printer(name);
}

UsbPrinter.prototype.printText = function (text, cpi) {
    options.cpi = cpi || 15;
    const jobFromText = this.printer.printText(text, options);
    // Listen events from job 
    jobFromText.once('sent', function () {
        jobFromText.on('completed', function () {
            console.log('Job ' + jobFromText.identifier + 'has been printed');
            jobFromText.removeAllListeners();
        });
    });
}

UsbPrinter.prototype.printFile = function (path) {
    const jobFromFile = this.printer.printFile(path);
    // Listen events from job 
    jobFromFile.once('sent', function () {
        jobFromFile.on('completed', function () {
            console.log('Job ' + jobFromFile.identifier + 'has been printed');
            jobFromFile.removeAllListeners();
        });
    });
}

UsbPrinter.prototype.listPrinters = function () {
    // Get available printers list 
    Printer.list();
}


module.exports = exports = UsbPrinter;


// // Print from a buffer, file path or text 
// var fileBuffer = fs.readFileSync('/path/to/file.ext');
// var jobFromBuffer = printer.printBuffer(fileBuffer);

// var filePath = 'package.json';
// var jobFromFile = printer.printFile(filePath);

// var text = 'Print text directly, when needed: e.g. barcode printers'
// var jobFromText = printer.printText(text);

// // Cancel a job 
// jobFromFile.cancel();

// // Listen events from job 
// jobFromBuffer.once('sent', function () {
//     jobFromBuffer.on('completed', function () {
//         console.log('Job ' + jobFromBuffer.identifier + 'has been printed');
//         jobFromBuffer.removeAllListeners();
//     });
// });
const printer = require('printer');

function printBuffer(buffer, printerName) {
    printer.printDirect({
        data: buffer,
        type: 'PDF',
        printer: printerName,
        options: // supported page sizes may be retrieved using getPrinterDriverOptions, supports CUPS printing options
        {
            //media: 'Letter',
            'fit-to-page': true
        },
        success: function (id) {
            console.log('printed with id ' + id);
        },
        error: function (err) {
            console.error('error on printing: ' + err);
        }
    });
}

module.exports = {
    printBuffer
}
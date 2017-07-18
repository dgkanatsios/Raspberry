'use strict'
var Serial = require('serialport-stream')
var WritableStream = require('stream').Writable
var inherits = require('util').inherits

var defaults = {
    heatDots: 7,
    heatTime: 80,
    heatInterval: 2,
    printDensity: 15,
    printBreakTime: 15,
    justify: 'l',
    bold: false,
    inverse: false,
    underline: false,
    doubleHeight: false,
    lineHeight: 32,
    barcodeHeight: 50,
    textHeight: 's'
}

var barcodeTypes = ['UPC-A', 'UPC-E', 'EAN-13', 'EAN-8', 'CODE-39', 'I-25', 'CODEBAR', 'CODE-93', 'CODE-128', 'CODE-11', 'MSI']
var barcodeTypesObj = barcodeTypes.reduce(function (acc, name, index) {
    acc[simplify(name)] = index
    return acc
}, Object.create(null))

function simplify(name) {
    return (name + '').toLowerCase().replace(/[^a-z0-9]/g, '')
}

function ord(c) { return String(c).charCodeAt(0) }
var SO = 14
var DC2 = 18
var DC4 = 20
var ESC = 27
var GS = 29


inherits(Thermal, WritableStream);
function Thermal(port, baud) {
    if (!(this instanceof Thermal)) return new Thermal(port, baud);

    WritableStream.call(this)

    this._port = new Serial(port, baud || 19200)

    this.awake = false
    this.state = {}
    this.opts = Object.create(defaults)

    this.sleep()
}

Thermal.prototype.printText = function (chunk, encoding, callback) {
    var self = this
    function write(chunk) {
        WritableStream.prototype.write.apply(self, arguments)
    }

    if (!this.awake) write(new Buffer([ESC, ord('='), 1]))
    this.awake = true

    var state = this.state
    var opts = this.opts

    opts.heatDots = opts.heatDots | 0
    opts.heatTime = opts.heatTime | 0
    opts.heatInterval = opts.heatInterval | 0
    if (state.heatTime !== opts.heatTime || state.heatInterval !== opts.heatInterval) {
        write(new Buffer([ESC, ord('7'), opts.heatDots, opts.heatTime, opts.heatInterval]))
    }
    state.heatDots = opts.heatDots
    state.heatTime = opts.heatTime
    state.heatInterval = opts.heatInterval

    opts.printDensity = opts.printDensity | 0
    opts.printBreakTime = opts.printBreakTime | 0
    if (state.printDensity !== opts.printDensity || state.printBreakTime !== opts.printBreakTime) {
        write(new Buffer([DC2, ord('#'), (opts.printDensity << 4) | opts.printBreakTime]))
    }
    state.printDensity = opts.printDensity
    state.printBreakTime = opts.printBreakTime

    opts.justify = String(opts.justify).toLowerCase()
    if (state.justify !== opts.justify) {
        write(new Buffer([ESC, ord('a'), { 'l': 0, 'c': 1, 'r': 2 }[opts.justify]]))
    }
    state.justify = opts.justify

    opts.inverse = Boolean(opts.inverse)
    if (state.inverse !== opts.inverse) {
        write(new Buffer([GS, ord('B'), opts.inverse | 0]))
    }
    state.inverse = opts.inverse

    opts.doubleHeight = Boolean(opts.doubleHeight)
    if (state.doubleHeight !== opts.doubleHeight) {
        write(new Buffer([ESC, opts.doubleHeight ? SO : DC4]))
    }
    state.doubleHeight = opts.doubleHeight

    opts.lineHeight = opts.lineHeight | 0
    if (state.lineHeight !== opts.lineHeight) {
        write(new Buffer([ESC, ord('3'), opts.lineHeight || 32]))
    }
    state.lineHeight = opts.lineHeight

    opts.bold = Boolean(opts.bold)
    if (state.bold !== opts.bold) {
        write(new Buffer([ESC, ord('E'), opts.bold | 0]))
    }
    state.bold = opts.bold

    opts.underline = Boolean(opts.underline)
    if (state.underline !== opts.underline) {
        write(new Buffer([ESC, ord('-'), opts.underline | 0]))
    }
    state.underline = opts.underline

    opts.barcodeHeight = opts.barcodeHeight | 0
    if (state.barcodeHeight !== opts.barcodeHeight) {
        write(new Buffer([GS, ord('h'), opts.barcodeHeight]))
    }
    state.barcodeHeight = opts.barcodeHeight

    opts.textHeight = String(opts.textHeight).toLowerCase()
    if (state.textHeight !== opts.textHeight) {
        write(new Buffer([GS, ord('!'), { 's': 0, 'm': 10, 'l': 25 }[opts.textHeight]]))
    }
    state.textHeight = opts.textHeight

    return WritableStream.prototype.write.apply(this, arguments)
}

Thermal.prototype._write = function _write(buf, _, cb) {
    this._port.write(buf, cb)
}

Thermal.prototype.feed = function () {
    this.write('\n')
    return this
}

Thermal.prototype.wake = function () {
    this.write(new Buffer(0))
    return this
}

Thermal.prototype.sleep = function () {
    WritableStream.prototype.write.call(this, new Buffer([ESC, 61, 0]))
    this.awake = false
}

Thermal.prototype.writeBarcode = function (code, type) {
    type = simplify(type || 'EAN-13')
    var index = barcodeTypesObj[type]
    if (index === undefined) {
        throw new Error("unknown bar code type '" + code + "'")
    }
    this.write(new Buffer([GS, ord('k'), index]))
    return this.write(code + '\0')
}

module.exports = exports = Thermal;
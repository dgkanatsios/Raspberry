'use strict';
function parsedht(value){
    return {
        temperature: value[0],
        humidity: value[1],
        heatIndex: value[2] //https://en.wikipedia.org/wiki/Heat_index
    }
}


module.exports = {
    parsedht
}
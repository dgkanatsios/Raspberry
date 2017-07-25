'use strict';
const request = require('requestretry');

if (process.env.DEBUG)
    require('request').debug = true;

function parsedht(value) {
    let isValid = true;
    value.forEach(function (element) {
        if (element > 100 || element < -100) {
            isValid = false;
            return;
        }
    }, this);
    if (isValid) {
        return {
            temperature: value[0],
            humidity: value[1],
            heatIndex: value[2] //https://en.wikipedia.org/wiki/Heat_index
        };
    } else return null;

}

function round(number, precision) {
    if (isNaN(number)) return NOT_AVAILABLE;
    if(precision == undefined || isNaN(precision))
        precision =2;
    var factor = Math.pow(10, precision);
    var tempNumber = number * factor;
    var roundedTempNumber = Math.round(tempNumber);
    return roundedTempNumber / factor;
}

function postData(data) {
    return new Promise(function (resolve, reject) {
        data.credential = process.env.DEVICE_CREDENTIAL;

        const options = {
            uri: `${process.env.SERVER_URL}/new`,
            method: 'POST',
            json: data,
            maxAttempts: 3, // (default) try 3 times
            retryDelay: 10000, // (default) wait for 10s before trying again
            retryStrategy: request.RetryStrategies.HTTPOrNetworkError // (default) retry on 5xx or network errors
        };

        request(options, function (error, response, body) {
            if (!error && response.statusCode === 200) {
                resolve(`successfully posted ${JSON.stringify(data)}`);
            } else {
                reject(`error: ${error}, details: ${JSON.stringify(response)}`);
            }

        });
    });
}

const NOT_AVAILABLE = 'N/A';

module.exports = {
    parsedht,
    postData,
    round,
    NOT_AVAILABLE
}
'use strict';
const request = require('request');

if (process.env.DEBUG)
    require('request').debug = true;

function parsedht(value) {
    value.forEach(function (element) {
        if (element > 100) return undefined;
    }, this);

    return {
        temperature: value[0],
        humidity: value[1],
        heatIndex: value[2] //https://en.wikipedia.org/wiki/Heat_index
    }
}

function postData(data) {
    return new Promise(function (resolve, reject) {
        data.credential = process.env.DEVICE_CREDENTIAL;

        const options = {
            uri: `${process.env.SERVER_URL}/new`,
            method: 'POST',
            json: data
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

module.exports = {
    parsedht,
    postData
}
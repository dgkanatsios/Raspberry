'use strict';
const request = require('request');

const options = {
  uri: `${process.env.SERVER_URL}/new`,
  method: 'POST'
};

function parsedht(value) {
    return {
        temperature: value[0],
        humidity: value[1],
        heatIndex: value[2] //https://en.wikipedia.org/wiki/Heat_index
    }
}

function postData(data) {
    return new Promise(function (resolve, reject) {
        data.credential = process.env.DEVICE_CREDENTIAL;
        options.json = data;
        request(options, function (error, response, body) {
            if(!error && response.statusCode === 200){
                resolve();
            }
            else{
                reject(`${error} && ${response}`);
            }

        });
    });
}

module.exports = {
    parsedht,
    postData
}
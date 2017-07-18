'use strict';
const request = require('request');

function ApiHelper(url) {
    this.url = url;

}

ApiHelper.prototype.getReservation = function (id) {
    return new Promise((resolve, reject) => {
        request({
            url: `${this.url}/${id}`,
            json: true,
            headers: {
                'Accept': 'application/json'
            }
        }, (error, response, body) => {
            if (!error && response.statusCode === 200) {
                resolve(body);
            } else {
                reject(error, response.statusCode);
            }
        })
    });
}


module.exports = ApiHelper;
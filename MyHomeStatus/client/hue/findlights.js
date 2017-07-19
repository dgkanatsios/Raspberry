const HueApi = require("node-hue-api").HueApi;
require('dotenv').config();

const displayResult = function(result) {
    console.log(JSON.stringify(result, null, 2));
};

var host = process.env.HUE_HOST,
    username = process.env.HUE_USER,
    api;

api = new HueApi(host, username);

// --------------------------
// Using a promise
api.lights()
    .then(displayResult)
    .done();


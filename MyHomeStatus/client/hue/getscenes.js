const HueApi = require("node-hue-api").HueApi;
require('dotenv').config();

const displayResults = function(result) {
    console.log(JSON.stringify(result, null, 2));
};

var host = process.env.HUE_HOST,
    username = process.env.HUE_USER,
    api;

api = new HueApi(host, username);

api.scenes()
    .then(displayResults)
    .done();
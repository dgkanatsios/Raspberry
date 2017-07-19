const HueApi = require("node-hue-api").HueApi;

var displayResults = function(result) {
    console.log(JSON.stringify(result, null, 2));
};

var host = process.env.HUE_HOST,
    username = process.env.HUE_USER,
    api;

api = new HueApi(host, username);
// Obtain all the defined groups in the Bridge

// --------------------------
// Using a promise
api.groups()
    .then(displayResults)
    .done();

// --------------------------
// Using a callback
api.groups(function(err, result) {
    if (err) throw err;
    displayResults(result);
});
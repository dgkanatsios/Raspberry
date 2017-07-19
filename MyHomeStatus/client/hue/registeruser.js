const hue = require("node-hue-api");
const HueApi = require("node-hue-api").HueApi;
const hueClient = new HueApi();

hue.nupnpSearch(function (err, result) {
    if (err) throw err;
    console.log("Hue Bridges Found: " + JSON.stringify(result));

    const host = result[0].ipaddress;

    var displayUserResult = function (result) {
        console.log("Created user: " + JSON.stringify(result));
    };
    console.log(host);
    hueClient.createUser(host, function (err, user) {
        if (err) throw err;
        displayUserResult(user);
    });
});
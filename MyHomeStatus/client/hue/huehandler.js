const hue = require("node-hue-api");
const HueApi = hue.HueApi;
const lightState = hue.lightState;

const groupID = process.env.HUE_GROUPID;
const sensors = require('../sensors');
let rotarySensor, button;

//relax,read, concentrate,energize,bright,savanna,tropical,arctic,spring
const scenes = ["zEYdPRLULcctct3", "BrbDOEPeEwasEFz", "pYLL3k88CMeGi4D", "oqHaEXZudIqElxa",
    "w-NbK00MFwFMDHa", "A9-346ILNimiy6h", "W5qjfm7sAorMLn9", "XbeifJR952JDIAH", "5IUxU8MWzkqSsxl"
];
let sceneIndex = 0;

const handleBrightnessChangeResult = function (result) {
    //console.log(JSON.stringify(result, null, 2));
    if (!result)
        throw new Error('something weird happened on trying to change hue light brightness');
};

const handleSceneChangeResult = function (result) {
    //console.log(JSON.stringify(result, null, 2));
    if (!result)
        throw new Error('something weird happened on trying to change scene');
};

const host = process.env.HUE_HOST;
const username = process.env.HUE_USER;
const api = new HueApi(host, username);


const start = function () {
    rotarySensor = new sensors.RotaryAngleSensor(0);
    rotarySensor.start();
    rotarySensor.on('data', function (res) {
        handleRotaryAngle(res);
    });

    button = new sensors.ButtonSensor(7);
    button.on('down', function (arg) {
        if (arg === 'singlepress') {
            changeScene();
        } else if (arg === 'longpress') {
            turnLightsOnOff();
        }
    });
    button.watch();
}


function handleRotaryAngle(value) {
    api.groups().then(result => {
        //get the group we're interested
        let group = result.filter(x => x.id === groupID.toString())[0];
        if (group.state.all_on) {
            let state = lightState.create().brightness(value);
            api.setGroupLightState(groupID, state).then(handleBrightnessChangeResult).catch(err => console.log(err)).done();
        }
    }).done();
}

function changeScene() {
    let sceneToActivate = scenes[sceneIndex++];
    if (sceneIndex === scenes.length)
        sceneIndex = 0;
    api.activateScene(sceneToActivate).then(handleSceneChangeResult).catch(err => console.log(err)).done();
}

function turnLightsOnOff() {
    api.groups().then(result => {
        //get the group we're interested
        let group = result.filter(x => x.id === groupID.toString())[0];
        let state;
        if (group.state.all_on) {
            state = lightState.create().off();
        } else {
            state = lightState.create().on();
        }
        api.setGroupLightState(groupID, state).catch(err => console.log(err)).done();
    }).done();
}

module.exports = {
    start
}
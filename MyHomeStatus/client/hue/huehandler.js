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
    rotarySensor = new sensors.RotaryAngleSensor(1);
    rotarySensor.on('change', handleRotaryChange);
    rotarySensor.watch();

    button = new sensors.ButtonSensor(4);
    button.on('change', function (res) {
        if (res === 1) { //ignore zeros
            changeScene();
        }
    });
    button.watch();
}

let previousValue = -100;

function handleRotaryChange(value) {
    //value is 0..100

    let diff = Math.abs(value - previousValue);
    if (diff == 0 || diff == 1)
        return;
    else { //value change
        previousValue = value;
        api.groups().then(result => {
            //get the group we're interested
            let group = result.filter(x => x.id === groupID.toString())[0];
            if (group.state.all_on) {
                let state = lightState.create().brightness(value);
                api.setGroupLightState(groupID, state).then(handleBrightnessChangeResult).catch(err=>console.log(err)).done();
            }
        }).done();
    }
}

function changeScene() {
    let sceneToActivate = scenes[sceneIndex++];
    if (sceneIndex === scenes.length)
        sceneIndex = 0;
    api.activateScene(sceneToActivate).then(handleSceneChangeResult).catch(err=>console.log(err)).done();
}

module.exports = {
    start
}
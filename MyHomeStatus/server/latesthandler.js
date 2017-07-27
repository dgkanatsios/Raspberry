const helpers = require('./helpers');
const azure = require('azure-storage');
const moment = require('moment');

let lastMotionDetectionTime = null;

function uploadLatest(rawbody) {
    //add UTC time
    rawbody.datetime = moment.utc();

    return new Promise((resolve, reject) => {
        const blobService = azure.createBlobService(process.env.STORAGE_ACCOUNT,
            process.env.STORAGE_ACCESS_KEY);
        blobService.createContainerIfNotExists(helpers.containerName, function (error, result, response) {
            if (!error) {
                // if result = true, container was created.
                // if result = false, container already existed.
                const body = JSON.stringify(rawbody);
                blobService.createBlockBlobFromText(helpers.containerName, helpers.latestBlob, body, function (error, result, response) {
                    if (!error) {
                        resolve({
                            message: 'Upload OK!'
                        });
                    } else {
                        reject({
                            message: error
                        });
                    }
                });
            } else {
                reject({
                    message: error
                });
            }
        });
    });

}


function getLatest() {
    return new Promise((resolve, reject) => {
        const blobService = azure.createBlobService(process.env.STORAGE_ACCOUNT,
            process.env.STORAGE_ACCESS_KEY);
        blobService.getBlobToText(helpers.containerName, helpers.latestBlob, function (error, result) {
            if (error) {
                reject(error);
            } else {
                result = JSON.parse(result);
                if (result.motionDetectedWithinLastMinute === 'NO') {
                    result.latestMotionDetectionTime = lastMotionDetectionTime || 'N/A';
                } else {
                    result.latestMotionDetectionTime = result.motionDetectedWithinLastMinute;
                    lastMotionDetectionTime = result.motionDetectedWithinLastMinute;
                }
                resolve(JSON.stringify(result));
            }
        });
    });
}

module.exports = {
    uploadLatest,
    getLatest
}
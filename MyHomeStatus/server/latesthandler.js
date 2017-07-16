const helpers = require('./helpers');
const azure = require('azure-storage');
const moment = require('moment');

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
            if (error)
                reject(error);
            else
                resolve(result);
        });
    });
}

module.exports = {
    uploadLatest,
    getLatest
}
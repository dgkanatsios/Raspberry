const helpers = require('./helpers');
const azure = require('azure-storage');
const moment = require('moment');

function appendLatest(rawbody) {
    //add UTC time
    rawbody.datetime = moment.utc();

    return new Promise((resolve, reject) => {
        const blobService = azure.createBlobService(process.env.STORAGE_ACCOUNT,
            process.env.STORAGE_ACCESS_KEY);
        blobService.createContainerIfNotExists(helpers.containerName, function (error, result, response) {
            if (!error) {
                // if result = true, container was created.
                // if result = false, container already existed.
                const body = JSON.stringify(rawbody) + '\n';
                const blobName = moment.utc().format("YYYYMMDD");
                blobService.doesBlobExist(helpers.containerName, blobName, function (error, errorOrResult) {
                    if (error)
                        reject({
                            message: error
                        });
                    if (errorOrResult.exists) { 
                        blobService.appendBlockFromText(helpers.containerName, blobName, body, function (error, result) {
                            postDataHandler(error, result, resolve, reject);
                        });
                    } else {
                        blobService.createAppendBlobFromText(helpers.containerName, blobName, body, function (error, result) {
                            postDataHandler(error, result, resolve, reject);
                        })
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

function postDataHandler(error, result, resolve, reject) {
    if (!error) {
        resolve({
            message: 'Upload OK!'
        });
    } else {
        reject({
            message: error
        });
    }
}

module.exports = {
    appendLatest
}
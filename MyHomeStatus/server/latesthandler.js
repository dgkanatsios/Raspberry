const helpers = require('./helpers');
const azure = require('azure-storage');

function uploadLatest(rawbody) {

    return new Promise((resolve, reject) => {
        const blobService = azure.createBlobService(process.env.STORAGE_ACCOUNT,
            process.env.STORAGE_ACCESS_KEY);
        blobService.createContainerIfNotExists(helpers.containerName, function (error, result, response) {
            if (!error) {
                // if result = true, container was created.
                // if result = false, container already existed.
                const body = helpers.verifyConvertPostBody(rawbody);
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
                })
            } else {
                reject({
                    message: error
                });
            }
        });
    });

}

module.exports = {
    uploadLatest
}
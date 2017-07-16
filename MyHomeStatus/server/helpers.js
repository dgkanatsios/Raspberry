function verifyConvertPostBody(body) {
    if (!body.humidity || !body.temperature)
        throw new Error('body must have humidity and temperature');
    return JSON.stringify(body);
}


const containerName = 'statuscontainer';
const latestBlob = 'latest';

module.exports = {
    verifyConvertPostBody,
    containerName,
    latestBlob
}
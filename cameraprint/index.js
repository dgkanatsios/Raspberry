const NodeWebcam = require( "node-webcam" );

const opts = {
    width: 1280,
    height: 720,
    quality: 100,
    //Delay to take shot
    delay: 0,
    //Save shots in memory
    saveShots: true,
    // Webcam.OutputTypes
    output: "jpeg",
    //Which camera to use
    //Use Webcam.list() for results
    //false for default device
    device: false,
    // [location, buffer, base64]
    // Webcam.CallbackReturnTypes
    callbackReturn: "location",
    //Logging
    verbose: false
};


//Creates webcam instance
const Webcam = NodeWebcam.create( opts );


//Will automatically append location output type

Webcam.capture( "test_picture", function( err, data ) {
    console.log(data);
} );




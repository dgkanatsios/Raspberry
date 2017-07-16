require('dotenv').config();
const express = require('express'),
    app = express(),
    port = process.env.PORT || 3000,
    bodyParser = require('body-parser');


const latesthandler = require('./latesthandler');


app.use(bodyParser.urlencoded({
    extended: true
}));
app.use(bodyParser.json());


app.route("/new").post(function (req, res) {
    latesthandler.uploadLatest(req.body).then(result => res.json(result)).catch(error => res.json(error));
});

app.listen(port);

console.log('RESTful API server started on: ' + port);
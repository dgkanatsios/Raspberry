require('dotenv').config();
const helpers = require('./helpers');
const express = require('express'),
    app = express(),
    port = process.env.PORT || 3000,
    bodyParser = require('body-parser');


const latesthandler = require('./latesthandler');
const appendlatesthandler = require('./appendlatesthandler');

app.use(express.static('public'));
app.use(bodyParser.urlencoded({
    extended: true
}));
app.use(bodyParser.json());


app.route('/new').post(function (req, res) {
    try {
        helpers.verifyRequestBody(req.body);
        helpers.deleteCredentialProperty(req.body);
        Promise.all([latesthandler.uploadLatest(req.body), appendlatesthandler.appendLatest(req.body)])
            .then(results => res.json(results))
            .catch(error => {
                res.status(500).json(error);
                helpers.handleError(error);
            });
    } catch (e) {
        res.status(400).json({
            error: e.message
        });
        helpers.handleError(e.message);
    }
});

app.route('/latest').get(function (req, res) {
    latesthandler.getLatest().then(result => res.json(result)).catch(error => {
        res.status(500).json(error);
        helpers.handleError(error);
    });
});

app.listen(port);

console.log('RESTful API server started on: ' + port);


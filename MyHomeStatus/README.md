# MyHomeStatus - WIP

A utility that monitors home temperature and humidity using GrovePi and Raspberry PI and uploads them to Azure Storage.

### Client

Client is a node.js utility that gets data from Raspberry-GrovePI sensors every minute and sends them to server.

### Server

Server is a node.js utility that 
* gets data via an endpoint and stores them in Azure Storage in two different sections: a) it stores the latest data on a single blob for easy access and b) appends latest data to an append blob for later access and analysis
* exposes latest data via HTTP endpoint in JSON format
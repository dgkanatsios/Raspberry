# MyHomeStatus Server

Node.js script for MyHomeStatus server. 

To run it locally

```bash
npm install -g forever
npm install
export DEVICE_CREDENTIAL=<your device credential>
export STORAGE_ACCOUNT=<your storage account name>
export STORAGE_ACCESS_KEY=<your storage account key>
export PORT=<port>
forever start -o out.log -e err.log server.js
```
Or you can publish it to Docker Hub

```bash
docker build -t YOURNAME/myhomestatus:0.1 .
docker push YOURNAME/myhomestatus
```

and then run it

```bash
export DEVICE_CREDENTIAL=<your device credential>
export STORAGE_ACCOUNT=<your storage account name>
export STORAGE_ACCESS_KEY=<your storage account key>
export PORT=<port>
docker run -d -p 3000:3000 \
-e DEVICE_CREDENTIAL -e STORAGE_ACCOUNT \
-e STORAGE_ACCESS_KEY -e PORT \
--name myhomestatus \
 YOURNAME/myhomestatus:0.1
```
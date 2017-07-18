## Use a Raspberry PI to capture an image with a QR code, decode it and print it on a thermal printer [Work in progress]  

#### Steps

* Get a webcam

For my tests I used Microsoft Lifecam Cinema

* Get a thermal printer

I used [this](https://www.adafruit.com/product/597) one

* Connect and install printer

Connect the thermal printer to Raspberry PI using instructions [here](https://learn.adafruit.com/networked-thermal-printer-using-cups-and-raspberry-pi/first-time-system-setup)

Check the instructions [here](http://scruss.com/blog/2015/07/12/thermal-printer-driver-for-cups-linux-and-raspberry-pi-zj-58/) for ZJ58 USB printer

* Set $SERVER_URL

* Install fswebcam
```bash
sudo apt-get install fswebcam
```

* Install GraphicsMagick

Install this if [lwip](https://github.com/EyalAr/lwip) installation fails during npm install
```bash
sudo apt-get install graphicsmagick
```
* Install Node.js
```bash
sudo wget -O - https://raw.githubusercontent.com/audstanley/NodeJs-Raspberry-Pi/master/Install-Node.sh | sudo bash;
```
* Install Node dependencies
```bash
npm install
```

* Run the app
```bash
node index
```


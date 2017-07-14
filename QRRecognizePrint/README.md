## Use a Raspberry PI to capture an image with a QR code, decode it and print it on a thermal printer [WIP]  

#### Steps

* Get a webcam

For my tests I used Microsoft Lifecam Cinema

* Get a thermal printer

I used [this](https://www.adafruit.com/product/597) one

* Connect printer

Connect the printer to Raspberry PI using instructions [here](https://learn.adafruit.com/networked-thermal-printer-using-cups-and-raspberry-pi/first-time-system-setup)

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


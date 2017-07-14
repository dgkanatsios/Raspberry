## Use a Raspberry PI to capture an image with a QR code, decode it and print it on a thermal printer [WIP]  

#### Steps

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
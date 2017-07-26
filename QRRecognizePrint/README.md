## Use a Raspberry PI to capture an image with a QR code, decode it and print it on a thermal printer [Work in progress]  

#### Steps

* Get a webcam

    - Initially, we used Microsoft Lifecam Cinema. However, a Raspberry Pi Camera v2 works much better!

* Get a thermal printer

    - We used [this](https://www.adafruit.com/product/597) one

* Connect and install printer

    - Connect the thermal printer to Raspberry PI using instructions [here](https://learn.adafruit.com/networked-thermal-printer-using-cups-and-raspberry-pi/first-time-system-setup)

    - Check the instructions [here](http://scruss.com/blog/2015/07/12/thermal-printer-driver-for-cups-linux-and-raspberry-pi-zj-58/) for ZJ58 USB printer

    - Check [here](https://www.howtogeek.com/169679/how-to-add-a-printer-to-your-raspberry-pi-or-other-linux-computer/) for CUPS configuration

* Install zbar

```bash
sudo apt-get update && sudo apt-get install zbar-tools
```

* Modify /etc/rc.local to make /dev/video0 accessible

```bash
sudo modprobe bcm2835-v4l2
```

* *Optional* Install and configure samba on the RaspberryPi

    - Instructions [here](https://www.raspberrypi.org/magpi/samba-file-server/)

* Install Node.js
```bash
sudo wget -O - https://raw.githubusercontent.com/audstanley/NodeJs-Raspberry-Pi/master/Install-Node.sh | sudo bash;
```

* Install Node dependencies
```bash
sudo npm install
```

* Install forever
```bash
sudo npm install -g forever
```

* Set $SERVER_URL

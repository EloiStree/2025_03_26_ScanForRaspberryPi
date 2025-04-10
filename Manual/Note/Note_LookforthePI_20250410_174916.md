

You’re searching for the Raspberry Pi on the LAN because the hostname didn’t resolve.

The script called "ScanIpMono_LetThemCook" will start a scanning process.  
After a few seconds, if an IP is found, we’ll notify you that it has been detected.  
If no Pi is found, we’ll let you know that the scan was unsuccessful.

I attempted a port scan, though it’s not the ideal solution.  
This triggers actions across all the computers in the building.  
So, I specifically scan port 8080 for a Flask website with the `/hostname` page.

If found, I then verify that it’s the correct device.  
For more details on how to do this, check the documentation here:  
[ScanForRaspberryPi Documentation](https://github.com/EloiStree/2025_03_26_ScanForRaspberryPi)


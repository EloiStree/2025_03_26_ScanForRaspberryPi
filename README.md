# Scan Ip For Raspberry Pi

It seems that the Quest 3 is missing some necessary code to detect the Raspberry Pi's mDNS via [Avahi](https://webshed.org/projects/raspberrypi/mdns/).  

I tried using Java Native code, C# DNS, and even a raw connection, but none of them worked.  

The solution I found is a classic IP scan. Since the Raspberry Pi hosts a mini-website on port 8080, an NTP service on port 123, and optionally an APInt server on port 4615 (if you're using my code), the approach is straightforward:  

Take the XXX.XXX.XXX.0 segment of the current IPv4 connection and scan the range from 0 to 255 using HTTP, WebSockets, and TCP. This will help map the Raspberry Pi on your local network.  
As a tip, you can set up endpoints like `/hostname` and `/ipv4` in Flask to return the device's hostname and current IPv4 address.

``` py
import os
hostname=""
hostname_ip=""
def refresh_hostname():
    global hostname_ip, hostname
    if hostname_ip==None or hostname_ip=="":
        output = os.popen("hostname -I").read().strip()
        hostname = output
        ip_addresses = output.split()
        stack=""
        for ip in ip_addresses:
            if "." in ip:  
                stack+= ip+"\n"
        hostname_ip=stack

@app.route('/ipv4')
def get_local_ipv4():
    global hostname_ip
    refresh_hostname()
    return hostname_ip

@app.route('/hostname')
def get_local_hostname():
    global  hostname
    refresh_hostname()
    return hostname
```


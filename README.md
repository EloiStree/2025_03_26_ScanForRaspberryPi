Apparently, the Quest3 is missing some code to be able to find mDNS of the Raspberry Pi [avahi](https://webshed.org/projects/raspberrypi/mdns/).
I tried with Java Native code, with C# DNS, with some raw connection. None did work.

The solution I found is a good old IP Scan. 
On the raspberry you host, you are going to have a mini-website on 8080 and NTP on 123 and optionaly my APInt server if you are using my code on the port 4615

So the idea is simple. Take the XXX.XXX.XXX.0 part of the current IPV4 Connection and scan them from 0-255 with HTTP, WS and TCP.
Making a map of the Raspberryi PI on your LAN network.

I tip to have a page node `/hostname` and `/ipv4` returning the host name and the current IPV4 of the device with Flask.
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


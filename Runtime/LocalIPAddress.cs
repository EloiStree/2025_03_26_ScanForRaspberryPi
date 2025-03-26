using UnityEngine;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.SocialPlatforms;

public class LocalIPAddress : MonoBehaviour
{
    [TextArea(0,5)]
    public string localIP;
    [TextArea(0, 5)]
    public string ipPrefix;
    void Start()
    {
        
    }

    [ContextMenu("Refresh")]
    public void Refresh() {

        localIP = GetLocalIPAddress();
        Debug.Log("Full Local IP: " + localIP);

        ipPrefix = GetFirstThreeParts(localIP);
        Debug.Log("IP Prefix: " + ipPrefix);
    }

    string GetLocalIPAddress()
    {
         localIP = "127.0.0.1"; // Default fallback

        foreach (NetworkInterface netInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
        {
            if (netInterface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                 netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
            {
                foreach (UnicastIPAddressInformation ip in netInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP+="\n"+ ip.Address.ToString(); // Return first valid LAN IP
                    }
                }
            }
        }
        return localIP;
    }

    string GetFirstThreeParts(string ip)
    {
        string[] parts = ip.Split('.');
        if (parts.Length >= 3)
        {
            return $"{parts[0]}.{parts[1]}.{parts[2]}";
        }
        return "Invalid IP";
    }
}


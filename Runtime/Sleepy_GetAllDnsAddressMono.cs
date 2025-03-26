using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using System;
using System.Net.NetworkInformation;
using System.Net;


public class Sleepy_GetAllDnsAddressMono : MonoBehaviour
{
    public string m_hostname = "raspberrypi.local";
    [TextArea(5,10)]
    public string m_ipFound = "";
    public UnityEvent<string> m_onIpFound;
    public void Awake()
    {
        Refresh();
    }

    [ContextMenu("Refresh")]
    public void Refresh() {

        m_ipFound = GetAllIp(m_hostname);
        m_onIpFound.Invoke(m_ipFound);
    }
    public static string GetAllIp(string hostname) {


        GetAllIPAddresses(hostname, out List<string> ipv4, out List<string> ipv6);
        string ipFound = "";
        foreach (string ip in ipv4)
        {
            ipFound += ip + "\n";
        }
        foreach (string ip in ipv6)
        {
            ipFound += ip + "\n";
        }
        return ipFound;
    }
    public static void GetAllIPAddresses(string hostname, out List<string> ipv4Addresses, out List<string> ipv6Addresses)
    {
        try
        {
            IPAddress[] addresses = Dns.GetHostAddresses(hostname);

           ipv4Addresses = new List<string>();
           ipv6Addresses = new List<string>();

            foreach (IPAddress address in addresses)
            {
                try
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        // IPv4 address
                        ipv4Addresses.Add(address.ToString());
                        Debug.Log("IPv4 address: " + address.ToString());
                    }
                    else if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        // IPv6 address
                        ipv6Addresses.Add(address.ToString());
                        Debug.Log("IPv6 address: " + address.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error: " + ex.Message);
                    ipv4Addresses = new List<string>();
                    ipv6Addresses = new List<string>();
                    ipv4Addresses.Add("Error: " + ex.Message + " Stack" + ex.StackTrace);
                }
                }


        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
            ipv4Addresses = new List<string>();
            ipv6Addresses = new List<string>();

            ipv4Addresses.Add("Error: " + e.Message+" Stack"+e.StackTrace);
        }
    }

}





public class NetworkInfoIpv4Mono : MonoBehaviour
{
    public string m_ipFound = "";
    public UnityEvent<string> m_onIpFound;
    void Start()
    {
        Refresh();
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            UnicastIPAddressInformationCollection ipAddresses = networkInterface.GetIPProperties().UnicastAddresses;
            foreach (UnicastIPAddressInformation ip in ipAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Debug.Log("Local IP Address: " + ip.Address.ToString());
                    m_ipFound = ip.Address.ToString();
                    m_onIpFound.Invoke(m_ipFound);
                    return;
                }
            }
            // Check for ipv6
            foreach (UnicastIPAddressInformation ip in ipAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    Debug.Log("Local IPv6 Address: " + ip.Address.ToString());
                    m_ipFound = ip.Address.ToString();
                    m_onIpFound.Invoke(m_ipFound);
                    return;
                }
            }
        }
        m_ipFound = "No IPv4 address found";
        m_onIpFound.Invoke(m_ipFound);
    }
}



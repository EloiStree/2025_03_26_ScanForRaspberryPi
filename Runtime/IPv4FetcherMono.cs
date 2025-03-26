using UnityEngine;
using System;
using UnityEngine.Events;

public class IPv4FetcherMono : MonoBehaviour
{
    // For Android platform
    private static AndroidJavaClass ipv4FetcherClass = null;

    [Tooltip("The hostname to resolve (e.g., raspberrypi.local)")]
    public string hostname = "raspberrypi.local";

    [Tooltip("Log the results on start")]
    public bool logOnStart = true;
    public string m_className = "be.elab.ipfetcher.IPv4Fetcher";
    public static string ms_className = "";

    public string m_ipFound = "";
    public UnityEvent<string> m_onIpFound;

    void Start()
    {
        ms_className = m_className;

        if (logOnStart)
        {
            Refresh();
        }
    }

    [ContextMenu("Refresh")]
    public  void Refresh()
    {
        string ip = GetFirstIPv4Address(hostname);
        Debug.Log("First IPv4 Address for " + hostname + ": " + ip);

        string[] allIPs = GetAllIPv4Addresses(hostname);
        if (allIPs != null)
        {
            Debug.Log("All IPv4 Addresses for " + hostname + ":");
            foreach (string address in allIPs)
            {
                Debug.Log(address);
            }
        }
        ip += " || " + string.Join("\n", allIPs);
        m_ipFound = ip;
        m_onIpFound.Invoke(ip);
    }

    public static string GetIPv4Address(string hostname)
    {
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ipv4FetcherClass == null)
        {
            ipv4FetcherClass = new AndroidJavaClass(ms_className);
        }
        return ipv4FetcherClass.CallStatic<string>("getIPv4Address", hostname);
#else
            // Fallback for non-Android platforms or editor

            var hostEntry = System.Net.Dns.GetHostEntry(hostname);
            foreach (var address in hostEntry.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }
#endif
        }
        catch (Exception e)
        {
            Debug.LogError("Error resolving hostname: " + e.Message);
        }
        return null;
    }

    public static string[] GetAllIPv4Addresses(string hostname)
    {
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ipv4FetcherClass == null)
        {
            ipv4FetcherClass = new AndroidJavaClass(ms_className);
        }
        return ipv4FetcherClass.CallStatic<string[]>("getAllIPv4Addresses", hostname);
#else
        // Fallback for non-Android platforms or editor
            var hostEntry = System.Net.Dns.GetHostEntry(hostname);
            System.Collections.Generic.List<string> ipv4Addresses = new System.Collections.Generic.List<string>();
            foreach (var address in hostEntry.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipv4Addresses.Add(address.ToString());
                }
            }
            return ipv4Addresses.ToArray();
#endif
        }
        catch (Exception e)
        {
            Debug.LogError("Error resolving hostname: " + e.Message);

            return new string[] { "Error: " + e.Message + "Stacks: "+e.StackTrace };
          
        }
    }

    public static string GetFirstIPv4Address(string hostname)
    {
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ipv4FetcherClass == null)
        {
            ipv4FetcherClass = new AndroidJavaClass(ms_className);
        }
        return ipv4FetcherClass.CallStatic<string>("getFirstIPv4Address", hostname);
#else
        // Fallback for non-Android platforms or editor
            var hostEntry = System.Net.Dns.GetHostEntry(hostname);
            foreach (var address in hostEntry.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }
#endif
        }
        catch (Exception e)
        {
            Debug.LogError("Error resolving hostname: " + e.Message);
        }
        return "";
    }
}
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Events;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;
using System;
using UnityEngine.Networking;
using System.Text;
public class LocalNetworkScanMono : MonoBehaviour
{
    public string m_urlFormat = "http://{0}:8080/piv4";
    public string m_ipFound;
    public UnityEvent<string> m_onIpFound;
    public int m_index;

    public void Start()
    {
        Refresh();
    }
    [ContextMenu("Refresh")]
    public void Refresh()
    {
        ScanLocalNetwork("192.168.1");  // Example IP range
    }

    void ScanLocalNetwork(string baseIP)
    {
        for (int i = 1; i <= 254; i++)
        {
            string ip = $"{baseIP}.{i}";
            StartCoroutine(CheckDeviceOnline(ip));
            m_index = i;
        }
    }

    private IEnumerator CheckDeviceOnline(string ip)
    {
        UnityWebRequest request = UnityWebRequest.Get(string.Format(m_urlFormat, ip));
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Device found at: {ip}");
            m_ipFound = ip;
            m_onIpFound.Invoke(m_ipFound);
        }
        else
        {
            Debug.Log($"No response from {ip}. Error: {request.error}");
            
        }
    }
}

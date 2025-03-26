using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;
using System;

public class LocalNetworkScanForWebsocketServerMono : MonoBehaviour
{
    public string m_urlFormat = "ws://{0}:4615";
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
        m_ipFound = "";
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
        string url = string.Format(m_urlFormat, ip);
        using (TcpClient client = new TcpClient())
        {
            yield return StartCoroutine(ConnectToWebSocket(client, ip, 4615));

            if (client.Connected)
            {

                m_ipFound = ip;
                m_onIpFound.Invoke(m_ipFound);
                Debug.Log("YOOOOO " + ip);
                m_ipFound += "\n" + ip;
                yield break;

                //using (NetworkStream stream = client.GetStream())
                //using (StreamReader reader = new StreamReader(stream))
                //using (StreamWriter writer = new StreamWriter(stream))
                //{
                //    string key = GenerateWebSocketKey();
                //    string request = $"GET {url} HTTP/1.1\r\n" +
                //                    $"Host: {ip}:4615\r\n" +
                //                    $"Upgrade: websocket\r\n" +
                //                    $"Connection: Upgrade\r\n" +
                //                    $"Sec-WebSocket-Key: {key}\r\n" +
                //                    $"Sec-WebSocket-Version: 13\r\n\r\n";

                //    writer.Write(request);
                //    writer.Flush();

                //    string response = reader.ReadLine();
                //    if (response.Contains("101 Switching Protocols"))
                //    {
                //        Debug.Log($"Device found at: {ip}");
                //    }
                //    else
                //    {
                //        Debug.Log($"No response from {ip}");
                //    }
                //}
            }
            else
            {
                Debug.Log($"Could not connect to {ip}");
            }
        }
    }

    private IEnumerator ConnectToWebSocket(TcpClient client, string ip, int port)
    {
        client.BeginConnect(ip, port, asyncResult =>
        {
            try
            {
                client.EndConnect(asyncResult);
            }
            catch
            {
                // Handle connection failure
            }
        }, null);

        yield return new WaitForSeconds(1); // Wait for the connection to establish
    }

    private string GenerateWebSocketKey()
    {
        byte[] key = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return Convert.ToBase64String(key);
    }
}

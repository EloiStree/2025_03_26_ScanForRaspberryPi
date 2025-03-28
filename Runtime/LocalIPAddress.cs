using UnityEngine;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Eloi.ScanIP
{

    public class CheckPortCoroutineUtility
    {



        public static void GetAllIpv4ToCheck(out List<string> ipv4Addresses)
        {

            ipv4Addresses = new List<string>();
            GetLocalIpRangeGroup(out List<string> masks);
            foreach (string mask in masks)
            {

                for (int i = 0; i < 255; i++)
                {

                    ipv4Addresses.Add(mask + "." + i);
                }
            }

        }
        public static void GetAllIpv4ToCheck(out List<string> ipv4Addresses, out List<string> masksFound)
        {

            ipv4Addresses = new List<string>();
            GetLocalIpRangeGroup(out masksFound);
            foreach (string mask in masksFound)
            {
                for (int i = 0; i < 255; i++)
                {

                    ipv4Addresses.Add(mask + "." + i);
                }
            }

        }

        public static void GetLocalIpRangeGroup(out List<string> maskListIpv4, bool filterMask = true)
        {
            //ScanLocalNetwork
            maskListIpv4 = new List<string>();

            List<string> ipList = new List<string>();


            //string networkMask = NetworkInfoHelper.GetNetworkMask();
            //Debug.Log("Network Mask: " + networkMask);

            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                //if (netInterface.OperationalStatus == OperationalStatus.Up) // Only active interfaces
                {
                    foreach (UnicastIPAddressInformation ip in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        //if (ip.Address.AddressFamily == AddressFamily.InterNetwork )
                        {
                            string ipString = ip.Address.ToString();
                            if (filterMask)
                            {
                                GetFirstThreeParts(ipString, out bool wasIpv4, out string frontPart, out string ipPart);
                                if (wasIpv4)//&& frontPart!= "127.0.0")
                                {
                                    maskListIpv4.Add(frontPart);
                                }

                            }
                            else
                            {
                                maskListIpv4.Add(ipString);
                            }
                        }
                    }
                }
            }




            // TO WORK ON ANDROID ?
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            foreach (IPAddress address in addresses)
            {
                //if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    string ipString = address.ToString();
                    if (filterMask)
                    {
                        GetFirstThreeParts(ipString, out bool wasIpv4, out string frontPart, out string ipPart);
                        if (wasIpv4)
                        {
                            maskListIpv4.Add(frontPart);
                        }
                    }
                    else
                    {

                        maskListIpv4.Add(ipString);
                    }
                }
            }

            maskListIpv4 = maskListIpv4.Distinct().ToList();


        }

        public static async Task<List<string>> ScanLocalNetwork(int timeout = 100)
        {
            List<string> activeIPs = new List<string>();
            string baseIP = "192.168.1."; // Adjust based on your network

            for (int i = 1; i < 255; i++)
            {
                string ip = baseIP + i;
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                var reply = await ping.SendPingAsync(ip, timeout);

                if (reply.Status == IPStatus.Success)
                {
                    activeIPs.Add(ip);
                }
            }

            return activeIPs;
        }

        public static void GetFirstThreeParts(string ip, out bool wasIpv4, out string ipFrontPart, out string ipPart)
        {
            string[] parts = ip.Split('.');
            if (parts.Length >= 4)
            {
                ipFrontPart = $"{parts[0]}.{parts[1]}.{parts[2]}";
                ipPart = parts[3];
                wasIpv4 = true;
                return;
            }

            wasIpv4 = false;
            ipFrontPart = "";
            ipPart = "";

        }
        public static IEnumerator IsAllPortReachable(string ip, TargetPortToLookForGroup lookFor, Action<string> ipValideFound, CheckPortCallBackResult callback = null)
        {
            if (callback == null)
                callback = new CheckPortCallBackResult();
            yield return IsAllPortReachable(ip, lookFor, callback);
            if (callback.m_isPortReachable == true)
                if (ipValideFound != null)
                    ipValideFound(ip);

        }

        public static IEnumerator IsAllPortReachable(string ip, TargetPortToLookForGroup lookFor, CheckPortCallBackResult callback, CheckPortIpCallBackResult ipCallback)
        {
            yield return IsAllPortReachable(ip, lookFor, callback);
            ipCallback = new CheckPortIpCallBackResult();
            ipCallback.m_ip = ip;
            ipCallback.m_isPortReachable = callback.m_isPortReachable;
            ipCallback.m_finishedCoroutine = callback.m_finishedCoroutine;

        }
        public static IEnumerator IsAllPortReachable(string ip, TargetPortToLookForGroup lookFor, CheckPortCallBackResult callback)
        {

            foreach (TargetPortToLookFor targetPort in lookFor.m_requiredPorts)
            {
                CheckPortCallBackResult result = new CheckPortCallBackResult();
                yield return IsReachable(ip, targetPort, result);
                if (!result.m_isPortReachable)
                {
                    callback.NotifyAsNotReached();
                    yield break;
                }
            }
            callback.NotifyAsReached();

        }



        public static IEnumerator IsReachable(string ip, TargetPortToLookFor lookFor, Action<string> ipValideFound)
        {
            CheckPortCallBackResult callback = new CheckPortCallBackResult();
            yield return IsReachable(ip, lookFor, callback);
            if (callback.m_isPortReachable == true)
                if (ipValideFound != null)
                    ipValideFound(ip);

        }
        public static IEnumerator IsReachable(string ip, TargetPortToLookFor lookFor, Action<TargetIpPortToLookFor> ipValideFound)
        {
            CheckPortCallBackResult callback = new CheckPortCallBackResult();
            yield return IsReachable(ip, lookFor, callback);
            if (callback.m_isPortReachable == true)
                if (ipValideFound != null)
                    ipValideFound(new TargetIpPortToLookFor(ip, lookFor));

        }

        public static IEnumerator IsReachable(string hostname, TargetPortToLookFor lookFor, CheckPortCallBackResult callback)
        {
            if (lookFor == null)
            {

                callback.NotifyAsNotReached();
                yield break;
            }

            if (string.IsNullOrWhiteSpace(hostname))
            {

                callback.NotifyAsNotReached();
                yield return null;
            }

            if (callback == null)
            {
                callback = new CheckPortCallBackResult();
            }

            if (lookFor.m_portType == PortCheckType.TCP)
                yield return IsReachableTcp(hostname, lookFor, callback);
            else if (lookFor.m_portType == PortCheckType.UDP)
                yield return IsReachableUdp(hostname, lookFor, callback);
            else if (lookFor.m_portType == PortCheckType.Websocket
                || lookFor.m_portType == PortCheckType.SecureWebsocket)
                yield return IsReachableWebsocket(hostname, lookFor, callback);
            else if (lookFor.m_portType == PortCheckType.HTTP
                || lookFor.m_portType == PortCheckType.HTTPS)
                yield return IsReachableHTTP(hostname, lookFor, callback);
        }

        private static IEnumerator IsReachableUdp(string ip, TargetPortToLookFor lookFor, CheckPortCallBackResult callback)
        {

            callback.Reset();
            string ipAddress = ip.Trim();
            int port = lookFor.m_port;

            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Client.ReceiveTimeout = 2000;
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                byte[] message = Encoding.UTF8.GetBytes("Ping");
                udpClient.Send(message, message.Length, remoteEndPoint);

                DateTime startTime = DateTime.Now;
                bool received = false;

                while ((DateTime.Now - startTime).TotalSeconds < 2) // 2-second timeout
                {
                    try
                    {
                        if (udpClient.Available > 0)
                        {
                            byte[] receivedData = udpClient.Receive(ref remoteEndPoint);
                            Debug.Log($"Received response: {Encoding.UTF8.GetString(receivedData)}");
                            received = true;
                            break;
                        }
                    }
                    catch (SocketException ex)
                    {
                        Debug.Log($"Error checking port {port}: {ex.Message}");
                        yield break;
                    }
                    yield return null; // Wait for the next frame
                }

                if (received)
                {
                    Debug.Log($"Port {port} is open!");
                    callback.NotifyAsReached();
                    yield break;
                }


            }

            callback.Finished();

        }

        public static IEnumerator IsReachableTcp(string ip, TargetPortToLookFor look, CheckPortCallBackResult callback)
        {
            yield return CheckIfTcpIsReachable(ip, look, callback);
        }
        public static float m_timeoutSecondsWebsocket = 1;
        public static IEnumerator IsReachableWebsocket(string ip, TargetPortToLookFor look, CheckPortCallBackResult callback)
        {
            yield return CheckIfTcpIsReachable(ip, look, callback);

        }

        private static IEnumerator CheckIfTcpIsReachable(string ip, TargetPortToLookFor look, CheckPortCallBackResult callback)
        {
            callback.Reset();
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    client.BeginConnect(ip, look.m_port, asyncResult =>
                    {
                        try
                        {
                            client.EndConnect(asyncResult);
                        }
                        catch
                        {
                        }
                    }, null);
                }
                catch (Exception)
                {

                    callback.NotifyAsNotReached();
                    yield break;
                }

                yield return new WaitForSeconds(m_timeoutSecondsWebsocket);

                if (client.Connected)
                {
                    callback.NotifyAsReached();
                    yield break;
                }
            }
            callback.Finished();
        }

        public static string m_urlWsFormat = "ws://{0}:{1}";
        public static string m_urlWssFormat = "wss://{0}:{1}";
        public static string m_urlFormatHttp = "http://{0}:{1}";
        public static string m_urlFormatHttps = "https://{0}:{1}";
        public static IEnumerator IsReachableHTTP(string ip, TargetPortToLookFor look, CheckPortCallBackResult callback)
        {
            callback.Reset();
            string format = look.m_portType == PortCheckType.HTTP ? m_urlFormatHttp : m_urlFormatHttps;
            UnityWebRequest request = UnityWebRequest.Get(string.Format(format, ip, look.m_port));
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
                callback.NotifyAsReached();
            else callback.NotifyAsNotReached();


        }
        public static IEnumerator IsReachableUrl(string url, Action<string> pageContentIfReach, CheckPortCallBackResult callback = null)
        {
            if (callback == null)
                callback = new CheckPortCallBackResult();
            callback.Reset();
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {

                if (pageContentIfReach != null)
                {
                    pageContentIfReach?.Invoke(request.downloadHandler.text);
                };
                callback.NotifyAsReached();
            }
            else callback.NotifyAsNotReached();


        }




    }

    [System.Serializable]
    public class CheckPortIpCallBackResult
    {

        public string m_ip;
        public bool m_isPortReachable;
        public bool m_finishedCoroutine;

    }

    [System.Serializable]
    public class CheckPortCallBackResult
    {

        public bool m_isPortReachable;
        public bool m_finishedCoroutine;

        public void Reset()
        {
            m_finishedCoroutine = false;
            m_isPortReachable = false;
        }
        public void Finished()
        {
            m_finishedCoroutine = true;
        }
        public void NotifyAsReached()
        {
            m_isPortReachable = true;
            m_finishedCoroutine = true;
        }
        public void NotifyAsNotReached()
        {

            m_isPortReachable = false;
            m_finishedCoroutine = true;
        }
    }


    [System.Serializable]
    public class CheckPortCallListBackResult
    {

        [SerializeField] List<string> m_ipsFound = new List<string>();

        public void GetIpFound(out List<string> ipsFound)
        {

            ipsFound = m_ipsFound;
        }
        public void GetIpFound(out string ipFound)
        {

            if (m_ipsFound.Count > 0)
                ipFound = m_ipsFound[0];
            else ipFound = string.Empty;
        }

        public void AppendIp(string ip)
        {
            m_ipsFound.Add(ip);
        }
        public void Clear()
        {

            m_ipsFound.Clear();
        }
    }
    [System.Serializable]
    public class TargetPortToLookForGroup
    {
        public TargetPortToLookFor[] m_requiredPorts;
    }
    [System.Serializable]
    public class TargetIpPortToLookFor
    {
        public string m_ip;
        public int m_port;
        public PortCheckType m_portType;

        public TargetIpPortToLookFor() { }
        public TargetIpPortToLookFor(string ip, TargetPortToLookFor lookFor)
        {
            m_ip = ip;
            m_port = lookFor.m_port;
            m_portType = lookFor.m_portType;
        }
        public TargetIpPortToLookFor(string ip, int port, PortCheckType portType)
        {
            m_ip = ip;
            m_port = port;
            m_portType = portType;
        }
    }
    [System.Serializable]
    public class TargetPortToLookFor
    {

        public int m_port;
        public PortCheckType m_portType;

        public TargetPortToLookFor() { }
        public TargetPortToLookFor(int port, PortCheckType portType)
        {
            m_port = port;
            m_portType = portType;
        }
    }
    public enum PortCheckType { TCP, UDP, Websocket, SecureWebsocket, HTTP, HTTPS }
}

public class LocalIPAddress : MonoBehaviour
{
    [TextArea(0, 5)]
    public string localIP;
    [TextArea(0, 5)]
    public string ipPrefix;
    void Start()
    {

    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {

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
                        localIP += "\n" + ip.Address.ToString(); // Return first valid LAN IP
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


using Eloi.ScanIP;
using System;
using System.Net;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// I am a class that check if it is possible to reach a give hostname.
/// If it is, I give the ip of the hostname, else I notify that the host was not reachable.
/// Code created because on some devices like the Quest, mDns of the Raspberry Pi don't work  on LAN.
/// </summary>
public class ScanIpMono_DnsHostnameFailSafe : MonoBehaviour
{
    public string m_wantedHostname = "raspberrypi.local";
    public bool m_tryToReachAtEnable=true;
    public bool m_useDebugLogForError;

   

    public string m_ipFound;
    public string [] m_ipsFound;
    public bool m_errorHappened;
    public string m_errorMessage;
    public UnityEvent<string> m_onFoundTargetHostname;
    public UnityEvent<string> m_onFoundTargetIpv4;
    public UnityEvent<string> m_onUnreachableTargetHostname;


    public void OnEnable()
    {
        if (m_tryToReachAtEnable)
            TryToReachAndInvoke();
    }
    [ContextMenu("Try To Reach and Invoke")]
    public void TryToReachAndInvoke()
    {

        //foreach (string notSupportedString in m_notSupportedDNS) {

        //    if (m_wantedHostname.Contains(notSupportedString)) { 
        //        m_errorHappened = true;
        //        m_errorMessage = "This DNS is a mDNS that is not supported on Quest3 local Network.";
        //        m_onUnreachableTargetHostname.Invoke(m_wantedHostname);
        //        if (m_useDebugLogForError)
        //            Debug.Log(m_errorMessage);
        //        return;
        //    }
        //}
        try
        {
            IPAddress[] addresses = Dns.GetHostAddresses(m_wantedHostname);
            m_ipsFound = addresses.Select(x => x.ToString()).ToArray();
            IPAddress ipv4Address = addresses.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            if (ipv4Address != null)
            {
                m_ipFound = ipv4Address.ToString();
                m_onFoundTargetHostname.Invoke(m_wantedHostname);
                m_onFoundTargetIpv4.Invoke(m_ipFound);
                m_errorHappened = false;
                m_errorMessage = "";
            }
            else
            {
                throw new Exception("No IPv4 addresses found for hostname");
            }
        }
        catch (Exception e)
        {
            m_ipFound = "";
            m_errorHappened = true;
            m_errorMessage ="AA "+ e.Message + ":" + e.StackTrace;
            if (m_useDebugLogForError)
                Debug.LogWarning(m_errorMessage);
            m_onUnreachableTargetHostname.Invoke(m_wantedHostname);
        }
    }
}

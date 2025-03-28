using System;
using UnityEngine;
using System.Net;
using UnityEngine.Events;

namespace Eloi.ScanIP
{
    public class DNSResolverMono : MonoBehaviour
    {
        public string hostname = "raspberrypi.local"; // The hostname you want to resolve

        public UnityEvent<string> m_onIpv4Found;  // Event for when an IPv4 address is found
        public UnityEvent m_onHostnameNoFoundOrDnsUnsupported; // Event for DNS errors or unsupported scenarios

        void Start()
        {
            ResolveHostToIP(hostname);
        }

        // Function to resolve hostname to IP address
        public void ResolveHostToIP(string hostname)
        {
            try
            {
                // Get the list of IP addresses for the given hostname
                IPAddress[] addresses = Dns.GetHostAddresses(hostname);

                if (addresses.Length > 0)
                {
                    foreach (var address in addresses)
                    {
                        // Check if the IP address is an IPv4 address
                        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            // Trigger the event with the resolved IPv4 address
                            m_onIpv4Found?.Invoke(address.ToString());
                            Debug.Log("Resolved IPv4 address: " + address);
                            return;  // Stop after finding the first IPv4 address
                        }
                    }

                    // If no IPv4 address is found, handle accordingly
                    m_onHostnameNoFoundOrDnsUnsupported?.Invoke();
                    Debug.LogError("No IPv4 address found for hostname: " + hostname);
                }
                else
                {
                    // Handle the case where no addresses are returned
                    m_onHostnameNoFoundOrDnsUnsupported?.Invoke();
                    Debug.LogError("No IP addresses found for hostname: " + hostname);
                }
            }
            catch (Exception ex)
            {
                // Handle DNS errors (e.g., unsupported DNS or network issues)
                m_onHostnameNoFoundOrDnsUnsupported?.Invoke();
                Debug.LogError("Error resolving hostname: " + ex.Message);
            }
        }
    }
}


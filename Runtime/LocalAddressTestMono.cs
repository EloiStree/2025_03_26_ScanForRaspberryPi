using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class LocalAddressTestMono : MonoBehaviour
{
    public string url = "http://raspberrypi.local:8080/piv4";
    public string m_ipFound = "";
    public UnityEvent<string> m_onIpFound;
    void Start()
    {
        Refresh();
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        StartCoroutine(GetLocalIP());
    }


    private IEnumerator GetLocalIP()
    {

        // Make a request to the Raspberry Pi's service
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Connected to Raspberry Pi! Response: " + request.downloadHandler.text);
            m_ipFound = request.downloadHandler.text;
            m_onIpFound.Invoke(m_ipFound);
        }
        else
        {
            Debug.LogError("Error connecting to Raspberry Pi: " + request.error);
            m_ipFound = "Error: " + request.error;
            m_onIpFound.Invoke(m_ipFound);
        }
    }
}



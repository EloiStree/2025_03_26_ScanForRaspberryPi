using UnityEngine;
using UnityEngine.Android;

public class IpScanMono_RequestPermissionAndroid : MonoBehaviour
{
    public void RequestNetworkPermissions()
    {
        // Define the required permissions
        string[] permissions = new string[]
        {
            "android.permission.ACCESS_NETWORK_STATE",
            "android.permission.ACCESS_WIFI_STATE",
            "android.permission.INTERNET"
        };

        foreach (string permission in permissions)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
    }

    public void Awake() {

        RequestNetworkPermissions();
    }

}

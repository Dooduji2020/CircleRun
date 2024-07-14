using UnityEngine;

public class NetworkConnect : MonoBehaviour
{
    private static bool isConnect = false;
    public static bool CheckConnectInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            isConnect = false;
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            isConnect = true;
        else
            isConnect = true;

        return isConnect;
    }
}

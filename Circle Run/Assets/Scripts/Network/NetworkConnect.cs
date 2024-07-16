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
    private void Update()
    {
        if(CheckConnectInternet())
        { }
        else
        {
            Debug.Log("네트워크 연결 없음");
        }
    }
}

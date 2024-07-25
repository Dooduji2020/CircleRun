using UnityEngine;

public class NetworkConnect : MonoBehaviour
{
    private static bool isConnect = false;
    public static int tryCount = 0;
    public static bool CheckConnectInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            isConnect = false;
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            isConnect = true;
        else
            isConnect = true;

        if (!isConnect) ++tryCount;
        else tryCount = 0;

        return isConnect;
    }
    private void Update()
    {
        if (CheckConnectInternet())
        { }
        else
        {
            Debug.Log("��Ʈ��ũ ���� ����");
        }
    }
}

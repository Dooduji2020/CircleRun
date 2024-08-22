using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppleAuth;
using System.Text;
using AppleAuth.Interfaces;
using AppleAuth.Native;

public class AppleManager : MonoBehaviour
{
    private AppleAuthManager appleAuthManager;

    void Start()
    {
        var deserializer = new PayloadDeserializer();
        appleAuthManager = new AppleAuthManager(deserializer);
    }

    void Update()
    {
        if (appleAuthManager != null)
            appleAuthManager.Update();
    }

    public void SigninWithApple()
    {
        var loginArgs = new AppleAuthLoginArgs(AppleAuth.Enums.LoginOptions.IncludeEmail | AppleAuth.Enums.LoginOptions.IncludeFullName);

        appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    var userId = appleIdCredential.User;
                    var email = appleIdCredential.Email;
                    var fullName = appleIdCredential.FullName;
                    var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
                    Debug.LogWarning("AppleLogin Success");
                    BackEndManager.Instance.LoginInit(identityToken);
                }
            },
            error =>
            {
                Debug.Log("Apple Signin Error");
            });
    }
    public void SigninWithAppleOption()
    {
        var loginArgs = new AppleAuthLoginArgs(AppleAuth.Enums.LoginOptions.IncludeEmail | AppleAuth.Enums.LoginOptions.IncludeFullName);

        appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    var userId = appleIdCredential.User;
                    var email = appleIdCredential.Email;
                    var fullName = appleIdCredential.FullName;
                    var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
                    Debug.LogWarning("AppleLogin Success");
                    BackEndManager.Instance.AppleAuth(identityToken);
                }
            },
            error =>
            {
                Debug.Log("Apple Signin Error");
            });
    }
}

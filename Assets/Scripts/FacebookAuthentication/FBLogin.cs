using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FBLogin : MonoBehaviour {

    void Awake()
    {
        FB.Init(Initialize, OnHideUnity);
    }

    void Initialize()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("Logged in");
        }
        else // is not logged in
        {
            Debug.Log("Not Logged in");
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void FBStartLogin()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");

        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }

    void AuthCallBack(IResult result)
    {
        if (result.Error != null)
        {
            Debug.Log(result.Error);
        }
        else
        {
            if (FB.IsLoggedIn)
            {
                Debug.Log("Logged in");
            }
            else
            {
                Debug.Log("Not logged in");
            }
            FB.API("/me?fields=id", HttpMethod.GET, ShowName);
        }
    }

    void ShowName(IResult result)
    {
        if (result.Error == null)
        {
            Debug.Log(result.ResultDictionary["id"]);
        }
    }

}

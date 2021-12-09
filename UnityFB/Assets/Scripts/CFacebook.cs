using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class CFacebook : MonoBehaviour
{
  public Image ImageProfile;
  private void Start()
  {

  }
  // Awake function from Unity's MonoBehavior
  void Awake()
  {
    if (!FB.IsInitialized)
    {
      // Initialize the Facebook SDK
      FB.Init(InitCallback, OnHideUnity);
    }
    else
    {
      // Already initialized, signal an app activation App Event
      FB.ActivateApp();
    }
  }

  private void InitCallback()
  {
    if (FB.IsInitialized)
    {
      // Signal an app activation App Event
      FB.ActivateApp();
      // Continue with Facebook SDK
      // ...
      Debug.Log("Inited FB");
    }
    else
    {
      Debug.Log("Failed to Initialize the Facebook SDK");
    }
  }

  private void OnHideUnity(bool isGameShown)
  {
    if (!isGameShown)
    {
      // Pause the game - we will need to hide
      Time.timeScale = 0;
    }
    else
    {
      // Resume the game - we're getting focus again
      Time.timeScale = 1;
    }
  }

  public void Login()
  {
    var perms = new List<string>() { "public_profile", "email" };
    FB.LogInWithReadPermissions(perms, AuthCallback);
  }
  private void AuthCallback(ILoginResult result)
  {
    if (FB.IsLoggedIn)
    {
      // AccessToken class will have session details
      var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
      // Print current access token's User ID
      Debug.Log(aToken.UserId);
      // Print current access token's granted permissions
      foreach (string perm in aToken.Permissions)
      {
        Debug.Log(perm);
      }

      // Get the picture.
      StartCoroutine(Coroutine_LoadProfileImage());

    }
    else
    {
      Debug.Log("User cancelled login");
    }
  }

  IEnumerator Coroutine_LoadProfileImage()
  {
    string url = "https" + "://graph.facebook.com/" + AccessToken.CurrentAccessToken.UserId + "/picture";
    url += "?access_token=" + AccessToken.CurrentAccessToken.TokenString;
    WWW www = new WWW(url);
    yield return www;
    ImageProfile.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
  }

  private void LoginStatusCallback(ILoginStatusResult result)
  {
    if (!string.IsNullOrEmpty(result.Error))
    {
      Debug.Log("Error: " + result.Error);
    }
    else if (result.Failed)
    {
      Debug.Log("Failure: Access Token could not be retrieved");
    }
    else
    {
      // Successfully logged user in
      // A popup notification will appear that says "Logged in as <User Name>"
      Debug.Log("Success: " + result.AccessToken.UserId);
    }
  }
}

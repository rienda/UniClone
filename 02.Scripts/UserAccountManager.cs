using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;   // << Remember to add this reference to your scripts which use DatabaseControl

public class UserAccountManager : MonoBehaviour
{
    // 캡슐화를 위해서 프로퍼티를 사용하는 방법을 적극 추천
    public static UserAccountManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one UserAccountManager.");
        }
        else
        {
            Instance = this;
            // 씬이 전환시 사라지지 않고 메모리에 로드되도록 하기 위함
            DontDestroyOnLoad(this);
        }
    }

    public static string LoggedIn_Username { get; protected set; }
    public static string LoggedIn_Password = "";

    public static string LoggedIn_Data { get; protected set; }
    public static bool IsLoggedIn { get; protected set; }

    public string loggedInSceneName = "Lobby";
    public string loggedOutSceneName = "LoginMenu";

    public delegate void OnDataReceivedCallback(string data);

    public void LogOut()
    {
        LoggedIn_Username = "";
        LoggedIn_Password = "";
        IsLoggedIn = false;

        Debug.Log("User Logged Out");

        SceneManager.LoadScene(loggedOutSceneName);
    }

    public void LogIn(string username, string password)
    {
        LoggedIn_Username = username;
        LoggedIn_Password = password;
        IsLoggedIn = true;

        Debug.Log(username + "Logged In");

        SceneManager.LoadScene(loggedInSceneName);
    }

    // 데이터 저장(전송)
    public void SetData(string data)
    {
        if (IsLoggedIn)
        {
            StartCoroutine(sendSetData(data));
        }
    }

    IEnumerator sendSetData(string data)
    {
        IEnumerator e = DCF.SetUserData(LoggedIn_Username, LoggedIn_Password, data); // << Send request to set the player's data string. Provides the username, password and new data string
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            Debug.Log("sendSetData Success");
        }
        else
        {
            Debug.Log("sendSetData Error");
        }
    }

    // 데이터 로드(수신)
    public void GetData(OnDataReceivedCallback onDataReceived)
    {
        if (IsLoggedIn)
        {
            StartCoroutine(sendGetData(onDataReceived));
        }
    }

    IEnumerator sendGetData(OnDataReceivedCallback onDataReceived)
    {
        IEnumerator e = DCF.GetUserData(LoggedIn_Username, LoggedIn_Password); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Error")
        {
            //There was another error. Automatically logs player out. This error message should never appear, but is here just in case.
            Debug.Log("sendGetData Error");
        }
        else
        {
            //The player's data was retrieved. Goes back to loggedIn UI and displays the retrieved data in the InputField
            Debug.Log("sendGetData Success");
        }

        LoggedIn_Data = response;

        if (onDataReceived != null)
        {
            onDataReceived(response);
        }
    }
}

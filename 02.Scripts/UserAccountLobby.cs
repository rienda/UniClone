using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour
{
    [SerializeField]
    private Text usernameText;

    void Start()
    {
        if (UserAccountManager.IsLoggedIn)
        {
            usernameText.text = UserAccountManager.LoggedIn_Username;
        }
    }
}

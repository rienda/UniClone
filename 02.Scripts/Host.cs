using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

public class Host : NetworkBehaviour
{
    private int roomSize;
    
    [SerializeField]
    private Text RoomName;

    private string roomName;
    
    private NetworkManager manager;
    
    public string Hostname
    {
        get;
        private set;
    }

    public int RoomSize { get; set; }

    public void SetNumberOfPeople(string name)
    {
        if (System.Int32.TryParse(name, out roomSize))
        {
            if (roomSize <= 1)
            {
                roomSize = 1;
            }
            else if (roomSize >= 4)
            {
                roomSize = 4;
            }
        }
        else
        {
            Debug.Log("err");
        }

        RoomSize = roomSize;
    }

    void Start()
    {
        manager = NetworkManager.singleton;
        
        if (manager.matchMaker == null)
        {
            manager.StartMatchMaker();
        }
        
        DontDestroyOnLoad(this);
    }

    public void CreateRoom()
    {
        roomName = RoomName.text;

        Debug.Log("Creating room : " + roomName + "with room for " + roomSize + " Players.");

        // 방 생성
        manager.matchMaker.CreateMatch(roomName, (uint)roomSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
        
        Hostname = UserAccountManager.LoggedIn_Username;
    }
}

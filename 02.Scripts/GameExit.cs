using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class GameExit : NetworkBehaviour
{
    NetworkManager manager;

    MatchInfo matchinfo;

    void Start()
    {
        manager = NetworkManager.singleton;
    }

    public void ExitGame()
    {
        MatchInfo info = manager.matchInfo;

        manager.matchMaker.DropConnection(info.networkId, info.nodeId, 0, manager.OnDropConnection);
        manager.StopHost();
    }      
}

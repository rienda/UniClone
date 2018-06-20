using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseScripts : NetworkBehaviour
{
    public static bool pause = false;

    private NetworkManager manager;

    void Start()
    {
        manager = NetworkManager.singleton;
    }

    //public void LeaveRoom()
    //{
    //    MatchInfo info = manager.matchInfo;

    //    manager.matchMaker.DropConnection(info.networkId, info.nodeId, 0, manager.OnDropConnection);
    //    manager.StopHost();
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class RoomListItem : MonoBehaviour
{
    [SerializeField]
    private Text roomNameText;

    public delegate void JoinRoomDelegate(MatchInfoSnapshot match);

    private JoinRoomDelegate joinRoomCallback;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot match, JoinRoomDelegate joinRoomCallback)
    {
        this.match = match;
        this.joinRoomCallback = joinRoomCallback;

        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ") ";
    }

    public void JoinRoom()
    {
        joinRoomCallback(match);        
    }
}

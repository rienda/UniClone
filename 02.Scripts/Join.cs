using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class Join : NetworkBehaviour
{
    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager manager;

    private List<GameObject> roomList = new List<GameObject>();

    void Start()
    {
        manager = NetworkManager.singleton;

        if (manager.matchMaker == null)
        {
            manager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();

        if (manager.matchMaker == null)
        {
            manager.StartMatchMaker();
        }

        manager.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        status.text = "";

        if (!success || matches == null)
        {
            status.text = "Couldn't get room list.";
            return;
        }

        for (int i = 0; i < matches.Count; i++)
        {
            GameObject roomListItemGO = Instantiate(roomListItemPrefab);
            roomListItemGO.transform.SetParent(roomListParent);

            // 룸 리스트 아이템에 셋업
            RoomListItem roomListItem = roomListItemGO.GetComponent<RoomListItem>();

            if (roomListItem != null)
            {
                roomListItem.Setup(matches[i], JoinRoom);
            }

            //룸 리스트에 아이템 추가
            roomList.Add(roomListItemGO);

        }

        if (roomList.Count == 0)
        {
            status.text = "No room at the moment.";
        }
    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot match)
    {
        manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);

        GameObject.Find("HostRoom").GetComponent<Host>().RoomSize = match.maxSize;

        StartCoroutine(WaitForJoin());
    }


    IEnumerator WaitForJoin()
    {
        ClearRoomList();

        int countdown = 10;

        while (countdown > 0)
        {
            status.text = "Joining... (" + countdown + ")";

            yield return new WaitForSeconds(1.0f);

            countdown--;
        }

        // ================= 접속 실패 ====================
        status.text = "Failed to connect.";

        yield return new WaitForSeconds(1.0f);

        // 접속을 끊음

        MatchInfo matchInfo = manager.matchInfo;

        if (matchInfo != null)
        {
            manager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, manager.OnDropConnection);
            manager.StopHost();
        }

        RefreshRoomList();
    }
}

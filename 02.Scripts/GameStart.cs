using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameStart : NetworkBehaviour {     

    [SyncVar]
    public int PlayerNum;

    private int roomSize = 0;

    [SerializeField]
    private Text wait;

    [SerializeField]
    private Text total;

    [SerializeField]
    private Behaviour[] components;

    [SerializeField]
    private GameObject[] gameObjects;
    
    private NetworkManager manager;

    private bool isStart = false;

    public bool GetStart
    {
        get { return isStart; }
        private set { isStart = value; }
    }

    void Start ()
    {
        PauseScripts.pause = true;

        manager = NetworkManager.singleton;

        roomSize = GameObject.Find("HostRoom").GetComponent<Host>().RoomSize;
    }

    void Update()
    {
        if (isStart)
        {
            return;
        }
        else
        {
            PlayerNum = GameDirector.Instance.players.Count;

            wait.text = "게임 시작 대기중 : " + PlayerNum.ToString();
            total.text = " / " + roomSize.ToString();

            // 플레이어의 수가 roomSize일 때 pause 해제
            if (PlayerNum == roomSize)
                PauseScripts.pause = false;  

            // 화면을 가리고 있던 것들을 치운다
            if (!PauseScripts.pause)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].enabled = false;
                }
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    gameObjects[i].SetActive(false);
                }

                isStart = true;
            }

            CheckPause();
        }        
    }

    void CheckPause()
    {
        if (PauseScripts.pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    //// 호스트가 시작 버튼을 누르면
    //void OnclickButton()
    //{
    //    PauseScripts.pause = false;
    //}

    //public void JoinGame()
    //{
    //    PlayerNum += 1;
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour {
    // 싱글턴
    public static GameDirector Instance { get; private set; }

    public Dictionary<string, GameObject> players;

    public string LocalPlayerName { get; set; }

    public bool isPlayerDead = false;

    public GameObject nonPlayerCamera;

    void Awake()
    {
        players = new Dictionary<string, GameObject>();
        players.Clear();

        if (Instance != null)
        {
            // 에러 처리
            Debug.LogError("More than one GameManager in Scene!");
        }
        else
        {
            Instance = this;
        }
    }
}

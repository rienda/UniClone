using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    private GameObject playerHead;

    [SerializeField]
    private GameObject HeadHitBox;

    [SerializeField]
    private GameObject[] dontDraws;

    public string LocalPlayerID { get; private set; }
    public string LocalPlayerName { get; private set; }

    // 클라이언트 시작시 게임 디렉터에 플레이어 등록
    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        string playerID = transform.name + netID;

        // 플레이어 등록
        transform.name = playerID;
        HeadHitBox.name = playerID;

        GameDirector.Instance.players.Add(playerID, this.gameObject);
    }

    // 비활성 시 게임 디렉터에서 플레이어 제거
    void OnDisable()
    {
        if (GameDirector.Instance.players.ContainsKey(transform.name))
        {
            GameDirector.Instance.players.Remove(transform.name);
        }
    }

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        
        // 화면에 그리지 않을 레이어로 변경
        SetLayerRecursively(playerHead, LayerMask.NameToLayer("DontDraw"));

        foreach (GameObject item in dontDraws)
        {
            item.layer = LayerMask.NameToLayer("DontDraw");
        }

        // 로컬 플레이어 ID와 이름을 등록
        LocalPlayerID = GetComponent<NetworkIdentity>().netId.ToString();
        LocalPlayerName = gameObject.name;
        GameDirector.Instance.LocalPlayerName = LocalPlayerName;
    }

    // 자식 오브젝트들의 레이어를 변경
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
            return;

        obj.layer = newLayer;

        // 자식 오브젝트들에 접근
        foreach (Transform child in obj.transform)
        {
            if (child == null)
                continue;

            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}

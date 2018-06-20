using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject[] Item;

    [SerializeField]
    private int numberOfItem;

    public Dictionary<string, GameObject> itemPrefabArray = new Dictionary<string, GameObject>();

    ItemPointEnable itemState;

    GameStart start;

    private bool isOver = false;
    
    [SyncVar]
    private int index = 0;                      // 아이템 마다의 고유 인덱스
    
    void Start()
    {
        start = transform.GetComponent<GameStart>();
        StartCoroutine(itemSetup());
    }

    IEnumerator itemSetup()
    {
        yield return new WaitForSeconds(1f);

        List<GameObject> list  = new List<GameObject>(GameDirector.Instance.players.Values);

        string spawnPlayerName = list[0].name;
        if (( GameDirector.Instance.players.ContainsKey(spawnPlayerName) 
            && spawnPlayerName == GameDirector.Instance.LocalPlayerName))
        {
            GameDirector.Instance.players[spawnPlayerName].
                GetComponent<PlayerNetworkCallFunc>().IntantiateItem(numberOfItem, spawnPlayerName);
        }

    }   

    [ClientRpc]
    public void RpcIntantiateItem(float xPos, float yPos, int itemIndex)
    {
        GameObject item = Instantiate(Item[itemIndex], new Vector3(xPos, 150f, yPos), Quaternion.identity);
        item.gameObject.name = item.gameObject.name + index.ToString();
        index++;

        itemPrefabArray.Add(item.gameObject.name, item.gameObject);
    }
}

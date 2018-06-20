using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerItem : NetworkBehaviour {

    private PlayerSound playerSound;

    public List<string> items;
    //public SyncListString items;

    public string NearestItem;

    [SerializeField]
    private GameObject[] Itemwindow;

    [SerializeField]
    private Text itemname;

    [SerializeField]
    private Text[] ItemAmount;

    [SerializeField]
    private GameObject ShowItemTime;

    [SerializeField]
    private GameObject Enegy;

    [SerializeField]
    private GameObject Hp;

    [SerializeField]
    private GameObject Usetem;

    [SerializeField]
    private Text TimeItem;

    [SerializeField]
    private GameObject playerUi;

    Image Progress; 
    Image HPfill; 
    Image EnegyFill; 

    bool canEnegy = true;
    bool EnegyUsed = false;


    bool canHPkit = true;
    bool isHpKit = false;
    bool HpUsed = false;

    bool Show_F_stop = false;
    bool windowsmall = false;

    public bool isGetItem = false;

    float time = 4.0f;

    float Max = 100f;

    private void Start()
    {
        playerSound = GetComponent<PlayerSound>();
        //items = new SyncListString();
        items = new List<string>();
    }

    private int[,] PresentItem = new int[3, 2]
    {
        // 아이템코드(0, 개수
        { 0, 0 },
        { 1, 0 },
        { 2, 0 }
    };

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        GetItem();
        GetName();
        ItemCount();
        UseItem(); 
        SetItemSize();
        SetProgressTime();
        StopCoroutine();
        DelayEnegy();
        DelayHP();

        CheckNearItem();
    }    

    void SetProgressTime()
    {
        if (Show_F_stop)
        {
            time = time - Time.deltaTime;
            ProgressDown(time);
        }
    }

    void SetItemSize()
    {
        if (GetComponent<PlayerController>().isFPS)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                foreach (GameObject item in Itemwindow)
                {

                    item.SetActive(!item.activeSelf);
                }
            }
        }
    }

    public void GetItem()
    {
        if (NearestItem != null && !isGetItem)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isGetItem = true;
                DeleteItem(NearestItem);
            }
        }
    }

    void GetName()
    {
        if (NearestItem != null)
        {
            itemname.text = NearestItem + " 줍기 ";
        }
    }

    void CountUpItem(int itemCode)
    {
        playerSound.PlayOneShot(gameObject.name, (int)PlayerSound.AudioClipNum.GET_ITEM, 1f);
        int i = itemCode;
        if (i == 0)
        {
            PresentItem[0, 1]++;
        }
        else if (i == 1)
        {
            PresentItem[1, 1]++;
        }
        else if (i == 2)
        {
            PresentItem[2, 1]++;
        }
    }

    void ItemCount()
    {
        for (int i = 0; i < ItemAmount.Length; i++)
        {
            ItemAmount[i].text = PresentItem[i, 1].ToString();
        }
    }

    void UseItem()
    {
        // HP 키트 사용
        if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            if(!isHpKit)
            {
                StartCoroutine(UseHpKit());
                playerSound.PlayOneShot(gameObject.name, (int)PlayerSound.AudioClipNum.ITEM_HP, 1f);
            }
        }

        // 에너지 키트 사용
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (canEnegy && PresentItem[1, 1] > 0)
            {
                UseEnegyKit();
                playerSound.PlayOneShot(gameObject.name, (int)PlayerSound.AudioClipNum.ITEM_ENERGY, 1f);
            }
        }

        // 아이템 업그레이드
        if(Input.GetKeyDown(KeyCode.U))
        {
            // 아이템 업그레이드
            if(PresentItem[0, 1] > 0)
            {
                GetComponent<PlayerWeaponStats>().Upgrade((int)GetComponent<PlayerAttack>().getGunMode, gameObject.name);
                playerSound.PlayOneShot(gameObject.name, (int)PlayerSound.AudioClipNum.ITEM_UPGRADE, 1f);
                PresentItem[0, 1]--;
            }
        }
    }

    private void UseEnegyKit()
    {
        EnegyFill = Enegy.GetComponent<Image>();
        canEnegy = false;
        PresentItem[1, 1]--;
        GameDirector.Instance.players[gameObject.name].GetComponent<PlayerStats>().EnergySet(Max);
        EnegyFill.fillAmount = 0f;
        EnegyUsed = true;
    }

    void DelayEnegy()
    {
        if (EnegyUsed)
        {
            EnegyFill.fillAmount += Time.deltaTime * 0.14f;
            if (EnegyFill.fillAmount >= 1f)
            {
                EnegyFill.fillAmount = 1f;
                EnegyUsed = false;
                canEnegy = true;
            }
        }
    }

    void StopCoroutine()
    {
        if (Input.GetKeyDown(KeyCode.F) && Show_F_stop)
        {
            Progress.fillAmount = 1f;
            time = 4.0f;
            StopAllCoroutines();
            ShowItemTime.SetActive(false);
            Show_F_stop = false;
            isHpKit = false;
        }
    }       

    IEnumerator UseHpKit()
    {
        if (canHPkit && PresentItem[2, 1] > 0)
        {
            isHpKit = true;

            ShowItemTime.SetActive(true);

            Show_F_stop = true;

            Progress = Usetem.GetComponent<Image>();
            HPfill = Hp.GetComponent<Image>();

            Progress.fillAmount = 1f;
            time = 4.0f;
                       
            yield return new WaitForSeconds(4.0f);

            HpUsed = true;
            ShowItemTime.SetActive(false);
            Show_F_stop = false;

            canHPkit = false;
            PresentItem[2, 1]--;
            HPfill.fillAmount = 0f;

            GameDirector.Instance.players[gameObject.name].GetComponent<PlayerStats>().HealthUp(10f);
            yield return new WaitForSeconds(5.5f);
            GameDirector.Instance.players[gameObject.name].GetComponent<PlayerStats>().HealthUp(10f);
            yield return new WaitForSeconds(5.5f);
            GameDirector.Instance.players[gameObject.name].GetComponent<PlayerStats>().HealthUp(10f);
            yield return new WaitForSeconds(5.5f);
            GameDirector.Instance.players[gameObject.name].GetComponent<PlayerStats>().HealthUp(10f);
            yield return new WaitForSeconds(5.5f);
            GameDirector.Instance.players[gameObject.name].GetComponent<PlayerStats>().HealthUp(10f);
            yield return new WaitForSeconds(5.5f);
            GameDirector.Instance.players[gameObject.name].GetComponent<PlayerStats>().HealthUp(10f);
            yield return new WaitForSeconds(5.5f);
            GameDirector.Instance.players[gameObject.name].GetComponent<PlayerStats>().HealthUp(5f);
            yield return new WaitForSeconds(5.5f);
        }
        //yield return new WaitForSeconds(2.5f);
        //HpUsed = false;
        //canHPkit = true;
        //isHpKit = false;
    }

    void DelayHP()
    {
        if(HpUsed)
        {
            HPfill.fillAmount += Time.deltaTime * 0.025f;
            // 이 아래가 바뀌었음
            if (HPfill.fillAmount >= 1f)
            {
                HPfill.fillAmount = 1f;
                HpUsed = false;
                canHPkit = true;
                isHpKit = false;
            }
        }
    }

    void ProgressDown(float t)
    {
        Progress.fillAmount -= (Time.deltaTime * 4.0f * 6) / Max;
        
        TimeItem.text = (t.ToString());
    }

    public void CheckNearItem()
    {
        float dist = 5.0f;
        float tempDistance = 0f;
        
        if (items != null)
        {
            foreach (string itemName in items)
            {
                if (itemName != null)
                {
                    if (GameDirector.Instance.GetComponent<ItemManager>().itemPrefabArray.ContainsKey(itemName))
                    {
                        tempDistance = Vector3.Distance(transform.position,
                            GameDirector.Instance.GetComponent<ItemManager>().itemPrefabArray[itemName].transform.position);
                    }

                    if (tempDistance < dist)
                    {
                        dist = tempDistance;
                        // 가장 가까운 아이템 프리팹을 NearestItem에 저장
                        NearestItem = itemName;

                        if (NearestItem != null)
                        {
                            transform.Find("PlayerUi").transform.Find("ItemInfo").gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        if (items.Count == 0)
        {
            transform.Find("PlayerUi").transform.Find("ItemInfo").gameObject.SetActive(false);
        }
    }

    [Client]
    public void DeleteItem(string itemName)
    {
        CmdDeleteItem(itemName);
    }

    [Command]
    void CmdDeleteItem(string itemName)
    {
        //        GameDirector.Instance.players[gameObject.name].GetComponent<PlayerItem>().RpcDeleteItem(gameObject.name, itemName);
        RpcDeleteItem(gameObject.name, itemName);
    }

    [ClientRpc]
    void RpcDeleteItem(string playerName, string itemName)
    {
        // 리스트에서 먹은 아이템 삭제

        if (items.Contains(itemName))
        {
            items.Remove(itemName);
        }

        if (GameDirector.Instance.GetComponent<ItemManager>().itemPrefabArray.ContainsKey(itemName))
        {
            GameObject item = GameDirector.Instance.GetComponent<ItemManager>().itemPrefabArray[itemName];

            if (item != null)
            {
                // 아이템 제거
                GameDirector.Instance.GetComponent<ItemManager>().itemPrefabArray.Remove(itemName);
                // 먹은아이템 게임 오브젝트 삭제
                Destroy(item);
            }
            if (gameObject.name == playerName)
            {
                CountUpItem(item.GetComponent<ItemInfo>().GetItemCode());
            }
        }
        isGetItem = false;
    }
}

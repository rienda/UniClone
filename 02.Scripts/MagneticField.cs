using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MagneticField : NetworkBehaviour {
    public float MapSizeHalf;

    [SerializeField]
    private Vector3 previosScale;

    [SerializeField]
    private float MagneticFieldSizeTo;

    Vector3 moveTo;

    Vector3 scaleTo;
    
    private float moveX;
    
    private float moveZ;
    
    private float dt;

    private bool start = false;

    private Vector3 previousPos = Vector3.zero;

    void Start()
    {
        scaleTo = new Vector3(MagneticFieldSizeTo, transform.localScale.y, MagneticFieldSizeTo);
        StartCoroutine(Setup());
    }

    [ClientRpc]
    public void RpcMagneticFieldSetup(float moveX, float moveZ)
    {
        moveTo = new Vector3(moveX, 0f, moveZ);
        start = true;
    }

    // 네트워크 지연시간을 준 셋업
    IEnumerator Setup()
    {
        yield return new WaitForSeconds(30f);
        List<GameObject> list = new List<GameObject>(GameDirector.Instance.players.Values);
        string spawnPlayerName = list[0].name;
        if (spawnPlayerName == GameDirector.Instance.LocalPlayerName)
        {
            GameDirector.Instance.players[spawnPlayerName].GetComponent<PlayerNetworkCallFunc>().MagneticFieldSetup();
        }
    }
    
    // 실시간 자기장 이동
    void Update()
    {
        if (start)
        {
            if (dt < 1f)
            {
                transform.localScale = Vector3.Lerp(previosScale, scaleTo, dt);
                transform.position = Vector3.Lerp(previousPos, moveTo, dt);
                dt += Time.deltaTime * 0.002f;                
            }
            else
            {
                transform.localScale = scaleTo;
                transform.position = moveTo;
            }
        }
    }
    
    // 자기장 안
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerStats>().isOutMagneticField = false;
        }
    }

    // 자기장 밖
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerStats>().isOutMagneticField = true;
        }
    }

    //void OnGUI()
    //{
    //    int w = Screen.width;
    //    int h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, w, h * 2 / 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = h * 2 / 100;
    //    style.normal.textColor = new Color(0.0f, 0.5f, 1.0f);
                
    //    string text = string.Format("{0} {1}", moveX, transform.position.x);
    //    GUI.Label(rect, text, style);
    //}
}


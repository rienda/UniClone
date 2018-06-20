using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EnvironmentManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject empZonePrefab;

    [SerializeField]
    private GameObject empExplosionPrefab;

    [SerializeField]
    private GameObject magneticField;

    private GameObject empZone;

    private GameObject empBomb;

    private Vector3 empPos;             // 생성 위치

    private float empTimer;             // EMP 존 생성 타이머
    
    private int useBomb = 0;

    private string spawnPlayerName;         // 스폰할 플레이어 이름
    
    private List<GameObject> list;

    void Update()
    {
        empTimer += Time.deltaTime;

        if (empTimer >= 20f)
        {
            CancelInvoke("ExplostionBomb");
            empTimer = 0f;
                                    
            list = new List<GameObject>(GameDirector.Instance.players.Values);

            spawnPlayerName = list[0].name;

            if (GameDirector.Instance.players.ContainsKey(spawnPlayerName) 
                && (spawnPlayerName == GameDirector.Instance.LocalPlayerName))
            {
                GameDirector.Instance.players[spawnPlayerName].GetComponent<PlayerNetworkCallFunc>().EmpZoneCreate();
            }
        }
    }
    
    [ClientRpc]
    public void RpcEmpZoneCreate(float randX, float randZ)
    {
        float x = magneticField.transform.position.x;
        float z = magneticField.transform.position.z;
        float size = magneticField.transform.localScale.x;
        empPos = new Vector3(x + randX * size * 0.5f, 50f, z + randZ * size * 0.5f);
        
        if (empZone != null)
        {
            Destroy(empZone);
        }
        
        empZone = Instantiate(empZonePrefab, empPos, Quaternion.identity);
        
        if (empZone == null)
        {
            Debug.Log("empZone null");

            return;
        }

        InvokeRepeating("ExplostionBomb", 1f, 2f);
    }

    [Client]
    private void ExplostionBomb()
    {
        if (GameDirector.Instance.players.ContainsKey(spawnPlayerName)
            && (spawnPlayerName == GameDirector.Instance.LocalPlayerName))
        {
            GameDirector.Instance.players[spawnPlayerName].GetComponent<PlayerNetworkCallFunc>().ExplostionBomb();
        }
    }

    [ClientRpc]
    public void RpcExplostionBomb(float bombPosCreateX, float bombPosCreateZ)
    {
        Vector3 vector = new Vector3(empPos.x + bombPosCreateX, 150f, empPos.z + bombPosCreateZ);
        empBomb = Instantiate(empExplosionPrefab, vector, Quaternion.identity);
        
        Destroy(empBomb, 10f);
    }
}
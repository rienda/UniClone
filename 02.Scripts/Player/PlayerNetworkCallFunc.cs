using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkCallFunc : NetworkBehaviour {
    [SerializeField]
    private float mapSizeHalf;

    [SyncVar]
    float randX;

    [SyncVar]
    float randZ;

    [Client]
    public void EmpZoneCreate()
    {
        CmdEmpZoneCreate();
    }
    [Command]
    private void CmdEmpZoneCreate()
    {
        randX = Random.Range(-1f, 1f);
        randZ = Random.Range(-1f, 1f);
        GameDirector.Instance.GetComponent<EnvironmentManager>().RpcEmpZoneCreate(randX, randZ);
    }

    [Client]
    public void ExplostionBomb()
    {
        CmdExplostionBomb();
    }
    [Command]
    private void CmdExplostionBomb()
    {
        randX = Random.Range(-10f, 10f);
        randZ = Random.Range(-10f, 10f);
        GameDirector.Instance.GetComponent<EnvironmentManager>().RpcExplostionBomb(randX, randZ);
    }


    [Client]
    public void MagneticFieldSetup()
    {
        CmdMagneticFieldSetup();
    }
    [Command]
    private void CmdMagneticFieldSetup()
    {
        float MapSizeHalf = GameObject.Find("ManeticField").GetComponent<MagneticField>().MapSizeHalf;
        float moveX = Random.Range(-MapSizeHalf, MapSizeHalf);
        float moveZ = Random.Range(-MapSizeHalf, MapSizeHalf);

        GameObject.Find("ManeticField").GetComponent<MagneticField>().RpcMagneticFieldSetup(moveX, moveZ);
    }


    [Command]
    public void CmdNetworkAnimation(string stateName)
    {
        RpcNetworkAnimation(gameObject.name, stateName);
    }
    [ClientRpc]
    private void RpcNetworkAnimation(string playerName, string stateName)
    {
        if (GameDirector.Instance.players[playerName].GetComponent<Animations>().GetAnimator != null)
        {
            GameDirector.Instance.players[playerName].GetComponent<Animations>().GetAnimator.Play(stateName);
        }
    }

    [Client]
    public void IntantiateItem(int size, string playerName)
    {
        CmdIntantiateItem(size, playerName);
    }
    [Command]
    public void CmdIntantiateItem(int size, string playerName)
    {
        for (int i = 0; i < size; i++)
        {
            int j = Random.Range(0, 3);
            float xPos = Random.Range(-(mapSizeHalf - 10f), (mapSizeHalf - 10f));
            float yPos = Random.Range(-(mapSizeHalf - 10f), (mapSizeHalf - 10f));

            if (playerName == GameDirector.Instance.LocalPlayerName)
            {
                GameDirector.Instance.transform.GetComponent<ItemManager>().RpcIntantiateItem(xPos, yPos, j);
            }
        }
    }
}


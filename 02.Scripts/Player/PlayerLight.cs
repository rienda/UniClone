using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLight : NetworkBehaviour {

    [SerializeField]
    private GameObject light;

    private bool isLightOn = false;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            LightSet(!light.activeSelf);
        }
    }

    [Client]
    private void LightSet(bool lightOn)
    {
        CmdLightSet(gameObject.name, lightOn);
    }

    [Command]
    private void CmdLightSet(string playerName, bool lightOn)
    {
        RpcLightSet(playerName, lightOn);
    }

    [ClientRpc]
    private void RpcLightSet(string playerName, bool lightOn)
    {
        GameDirector.Instance.players[playerName].GetComponent<PlayerLight>().light.SetActive(lightOn);
    }
}

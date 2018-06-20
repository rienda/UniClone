using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHeadPosition : NetworkBehaviour {

    [SerializeField]
    private Transform HeadPosition;

    [SerializeField]
    private GameObject FPSCam;
       
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        FPSCam.transform.position = new Vector3(HeadPosition.position.x, HeadPosition.position.y, HeadPosition.position.z);
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftAndLeft : MonoBehaviour
{
    [SerializeField]
    private Text leftplayer;

    private int aliveplayer;

    void Update()
    {
        aliveplayer = GameDirector.Instance.players.Count;
        
        leftplayer.text = aliveplayer.ToString() + " 생존";
    }
}

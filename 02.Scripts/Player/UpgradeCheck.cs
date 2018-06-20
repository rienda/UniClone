using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject[] AR;

    [SerializeField]
    private GameObject[] SG;

    [SerializeField]
    private GameObject[] SR;

    [SerializeField]
    private GameObject[] RPG;

    [SerializeField]
    private GameObject[] MISSILE;

    [SerializeField]
    private GameObject player;

    int AR_level = 0;
    int SG_level = 0;
    int SR_level = 0;
    int RPG_level = 0;
    int MISSILE_level = 0;

    void Update()
    {
        TurnOnLevel();
    }

    public void TurnOnLevel()
    {
        float[,] level = player.GetComponent<PlayerWeaponStats>().WeaponStats;

        AR_level = (int)level[0, 0];
        SG_level = (int)level[1, 0];
        SR_level = (int)level[2, 0];
        RPG_level = (int)level[3, 0];
        MISSILE_level = (int)level[4, 0];

        for (int i = 1; i <= 5; i++)
        {
            if(AR_level == i)
            {
                AR[i - 1].SetActive(true);
            }
            if (SG_level == i)
            {
                SG[i - 1].SetActive(true);
            }
            if (SR_level == i)
            {
                SR[i - 1].SetActive(true);
            }
            if (RPG_level == i)
            {
                RPG[i - 1].SetActive(true);
            }
            if (MISSILE_level == i)
            {
                MISSILE[i - 1].SetActive(true);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeaponStats : NetworkBehaviour
{
    public enum GunMode { AR, SG, SR, RPG, MISSILE }
    public enum WeaponStat { STAGE, DAMAGE, SPEED, SPREAD, DELAY, CONSUMPTION }

    private float[,] weaponDefaultStats = new float[5, 6]
    {
        // 업그레이드 단계, 데미지, 탄속, 탄 퍼짐, 재장전 속도, 소모량 감소
        { 0, 10f, 0f, 0.1f, 0.16f, 1f },        // ar
        { 0, 5f, 0f, 0.5f, 0.4f, 1f },        // sg
        { 0, 51f, 0f, 0f, 5f, 60f },          // sr  
        { 0, 30f, 30f, 0f, 5f, 30f },         // rpg
        { 0, 20f, 15f, 0f, 5f, 40f },         // missile
    };

    public float[,] WeaponDefaultStats { get; private set; }

    // 총 5단계 업그레이드 가능
    private float[,] weaponStats = new float[5, 6]
    {
        // 업그레이드 단계, 데미지, 탄속, 탄 퍼짐, 재장전 속도, 소모량 감소
        { 0, 10f, 0f, 0.1f, 0.16f, 1f },        // ar
        { 0, 5f, 0f, 0.5f, 0.4f, 1f },        // sg
        { 0, 51f, 0f, 0f, 5f, 60f },          // sr  
        { 0, 30f, 30f, 0f, 5f, 30f },         // rpg
        { 0, 20f, 15f, 0f, 5f, 40f },         // missile
    };

    public float[,] WeaponStats { get; private set; }
    
    void Start()
    {
        WeaponStats = weaponStats;
        WeaponDefaultStats = weaponDefaultStats;
    }

    [Client]
    public void Upgrade(int gunMode, string playerName)
    {
        CmdUprade(gunMode, playerName);
    }

    [Command]
    private void CmdUprade(int gunMode, string playerName)
    {
        GameDirector.Instance.players[playerName].GetComponent<PlayerWeaponStats>().RpcUpgrade(gunMode);
    }

    [ClientRpc]
    private void RpcUpgrade(int gunMode)
    {
        if (weaponStats[gunMode, 0] < 5)
        {
            weaponStats[gunMode, 0]++;

            for (int i = 0; i <= 5; i++)
            {
                if (i <= 2)
                {
                    weaponStats[gunMode, i] = 
                        weaponStats[gunMode, i] + weaponDefaultStats[gunMode, i] * 0.2f;
                }
                else
                {
                    weaponStats[gunMode, i] = 
                        weaponStats[gunMode, i] - weaponDefaultStats[gunMode, i] * 0.1f;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AttackManager : NetworkBehaviour {
    // 미리 발사할 발사체를 오브젝트 풀, 각 캐릭터 마다 가진다 
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private GameObject MissilePrefab;
    // 최대 오브젝트 풀 개수 
    [SerializeField]
    private int MaxCount;
    // 만든 오브젝트 풀 발사체 리스트
    public List<GameObject> missiles;
    public List<GameObject> rpgs;

    private int rpgCount = 0;
    private int missileCount = 0;
    // 시작 시 만듦
    void Start()
    {
        for (int i = 0; i < MaxCount; i++)
        {
            rpgs.Add(Instantiate(bulletPrefab));
            missiles.Add(Instantiate(MissilePrefab));

            rpgs[i].GetComponent<BulletController>().setShooter(gameObject);
            rpgs[i].GetComponent<BulletController>().setRpgCount(i);
            
            missiles[i].GetComponent<MissileController>().setShooter(gameObject);
            missiles[i].GetComponent<MissileController>().setMissileCount(i);
        }
    }
    // 직진 발사체 발사
    public void FireRpg(Vector3 direction)
    {
        if (rpgCount >= MaxCount)
        {
            rpgCount = 0;
        }
        rpgs[rpgCount].GetComponent<BulletController>().Fire(direction);

        rpgCount++;
    }

    // 유도탄 발사
    public void FireMissile(Vector3 direction)
    {
        if (missileCount >= MaxCount)
        {
            missileCount = 0;
        }
        missiles[missileCount].GetComponent<MissileController>().fire(direction);

        missileCount++;
    }
}

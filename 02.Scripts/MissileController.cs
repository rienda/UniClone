using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MissileController : NetworkBehaviour
{
    enum Sound { SHOOT, EXPLOSION}
    [SerializeField]
    private AudioClip[] audioClips;

    [SerializeField]
    private AudioSource audioSource;

    Rigidbody rgbd;
	Vector3 resetPos = new Vector3(0f, -100f, 0f);      // 초기화 위치

    [SyncVar]
	bool isFire = false;

    [SyncVar]
    bool isReset = false;

    [SerializeField]
	private float CheckTarget;                  // 목표 타겟들

	[SerializeField]
	private GameObject ExplosionEffect;         // 폭발 이펙트

    [SerializeField]
    private GameObject trailEffect;             // 트레일 이펙트

    private GameObject Shooter = null;          // 사격 플레이어
	private GameObject target = null;           // 타겟 플레이어
	
    private float damage;

    private bool isExplosion = true;            // 폭발 하였는지 확인

    private int missileCount;                   // 각 미사일 고유 번호

    private Vector3 lastPosition;

	// Use this for initialization
	void Awake()
	{
		rgbd = this.GetComponent<Rigidbody>();
		this.transform.position = resetPos;
        rgbd.velocity = Vector3.zero;
    }
    // 사격 플레이어 설정
    public void setShooter(GameObject gameObject)
    {
        Shooter = gameObject;
    }
    // 미사일 고유 번호 설정
    public void setMissileCount(int num)
    {
        missileCount = num;
    }

    public int getMissileCount()
    {
        return missileCount;
    }

    public GameObject getShooter()
    {
        return Shooter;
    }
    // 파이어
    public void fire(Vector3 direction)
	{      
        // 데미지 설정
        damage = Shooter.GetComponent<PlayerWeaponStats>().
            WeaponStats[(int)PlayerAttack.GunMode.MISSILE, (int)PlayerWeaponStats.WeaponStat.DAMAGE];

        audioSource.PlayOneShot(audioClips[(int)Sound.SHOOT]);

        rgbd.velocity = Vector3.zero;
		isFire = true;
        isReset = false;

        // 발사 위치 지정
        transform.position = Shooter.transform.Find("FireStart").position;
        // 회전
        rgbd.transform.rotation = Quaternion.LookRotation(direction);
        // 사격시에만 콜라이더 설정
		GetComponent<CapsuleCollider>().enabled = true;

        // 발사체 이펙트
        trailEffect.GetComponent<ParticleSystem>().Play(true);

        // 일정 시간 뒤 리셋
        StartCoroutine(ResetCall());
	}

    void FixedUpdate()
    {
        if (isFire)
        {
            rgbd.velocity = rgbd.transform.forward *
                Shooter.GetComponent<PlayerWeaponStats>().
                WeaponStats[(int)PlayerAttack.GunMode.MISSILE, (int)PlayerWeaponStats.WeaponStat.SPEED];

            float minDist = CheckTarget;

            // 타켓이 없으면 주변 플레이어 중 가까운 플레이어 타겟 체크
            if (target == null)
            {
                List<GameObject> list = new List<GameObject>(GameDirector.Instance.players.Values);
                foreach (GameObject player in list)
                {
                    if (player != Shooter)
                    {
                        float dist = Vector3.Distance(player.transform.position, transform.position);

                        if (dist < minDist)
                        {
                            minDist = dist;
                            target = player;
                        }
                    }
                }
            }
            else
            {
                Quaternion before = rgbd.transform.rotation;
                rgbd.transform.LookAt(target.transform);

                Quaternion after = rgbd.transform.rotation;
                rgbd.transform.rotation = before;

                rgbd.transform.rotation = Quaternion.Lerp(before, after, Time.fixedDeltaTime * 10f);
            }
        }
    }

    // 일정 시간 지나면 붐
	IEnumerator ResetCall()
	{
        yield return new WaitForSeconds(5f);
        if (isFire)
        {
            isFire = false;            
            lastPosition = transform.position;
            NetworkResetSynchronize(lastPosition);
        }
    }

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject != Shooter && isFire)
		{		
            isFire = false;
            lastPosition = transform.position;
            NetworkResetSynchronize(lastPosition);
        }
	}

    private void CheckPlayerInRange(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 5f, 1 << LayerMask.NameToLayer("Player"));

        foreach (Collider player in colliders)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, player.transform.position - position, out hit, 5f,
                1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Block")))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    if (Shooter.name == GameDirector.Instance.LocalPlayerName)
                    {
                        float rangeDamage;
                        // 거리에 따른 데미지
                        rangeDamage = ((5f - (player.transform.position - position).magnitude) / (5f)) * damage;
                        Shooter.GetComponent<PlayerAttack>().CmdTakeDamage(hit.transform.gameObject.name, rangeDamage);
                    }
                }
            }
        }
    }

    // 리셋 네트워크 동기화 함수
    private void NetworkResetSynchronize(Vector3 lastPos)
    {
        if (GameDirector.Instance.LocalPlayerName == Shooter.name)
        {
            GameDirector.Instance.players[Shooter.name].GetComponent<PlayerAttack>().CmdMissileReset(Shooter.name, missileCount, lastPos);
        }
        rgbd.velocity = Vector3.zero;
        transform.position = resetPos;
    }

    // 리셋 시 위치 초기화 및 폭발 이펙트 및 거리 체크
    public void Reset(Vector3 lastPos)
    {
        if (isReset)
        {
            return;
        }
        audioSource.PlayOneShot(audioClips[(int)Sound.EXPLOSION]);

        trailEffect.GetComponent<ParticleSystem>().Stop(true);

        isReset = true;
        rgbd.velocity = Vector3.zero;
        transform.position = resetPos;
        transform.rotation = Quaternion.identity;

        if (isExplosion)
        {
            GameObject exp = Instantiate(ExplosionEffect, lastPos, Quaternion.identity);
            Destroy(exp, 3f);
        }

        CheckPlayerInRange(lastPos);

        GetComponent<CapsuleCollider>().enabled = false;

        target = null;
        isExplosion = true;
    }
}

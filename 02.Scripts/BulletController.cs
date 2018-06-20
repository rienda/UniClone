using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletController : NetworkBehaviour {
    enum Sound { SHOOT, EXPLOSION }
    [SerializeField]
    private AudioClip[] audioClips;

    [SerializeField]
    private AudioSource audioSource;

    Rigidbody rgbd;
	Vector3 resetPos = new Vector3(0f, -100f, 0f);

    [SyncVar]
	bool isFire = false;

    [SyncVar]
    bool isReset = false;

	[SerializeField]
	private GameObject ExplosionEffect;

    [SerializeField]
    private GameObject trailEffect;

    private GameObject Shooter;
    
	private Ray ray;

    private float damage;

    private bool isExplosion = true;

    private int rpgCount;

    private Vector3 lastPosition;
    
    // Use this for initialization
    void Awake()
	{
		rgbd = this.GetComponent<Rigidbody>();
        rgbd.velocity = Vector3.zero;
        this.transform.position = resetPos;
    }

    void FixedUpdate()
    {
        if (isFire)
        {
            rgbd.velocity = rgbd.transform.forward *
                   Shooter.GetComponent<PlayerWeaponStats>().
                   WeaponStats[(int)PlayerAttack.GunMode.RPG, (int)PlayerWeaponStats.WeaponStat.SPEED];

            RaycastHit hit;
            // 2km 길이의 레이를 매 순간 체크하여 플레이어가 맞으면 그때 불릿의 위치와 1m 이내이면 폭발 후 리셋
            if (Physics.Raycast(ray, out hit, 2000f, 1 << LayerMask.NameToLayer("Player")
                | 1 << LayerMask.NameToLayer("Block")))
            {
                if (Vector3.Distance(hit.point, transform.position) < 1f && hit.transform.gameObject != Shooter)
                {
                    isFire = false;

                    lastPosition = transform.position;
                    NetworkResetSynchronize(lastPosition);
                }
            }
        }
    }

    public void setRpgCount(int num)
    {
        rpgCount = num;
    }

    public int getRpgCount()
    {
        return rpgCount;
    }

    public void setShooter(GameObject gameObject)
    {
        Shooter = gameObject;
    }

    public GameObject getShooter()
    {
        return Shooter;
    }

    // 발사
	public void Fire(Vector3 direction)
    {
        damage = Shooter.GetComponent<PlayerWeaponStats>().
            WeaponStats[(int)PlayerAttack.GunMode.RPG, (int)PlayerWeaponStats.WeaponStat.DAMAGE];

        audioSource.PlayOneShot(audioClips[(int)Sound.SHOOT]);

        rgbd.velocity = Vector3.zero;
		isFire = true;
        isReset = false;

        rgbd.transform.rotation = Quaternion.LookRotation(direction);

        transform.position = Shooter.transform.Find("FireStart").position;      // 이 위치에서 발사체 시작
        ray = new Ray(transform.position, direction);

        GetComponent<CapsuleCollider>().enabled = true;      // 콜라이더 활성화
        
        trailEffect.GetComponent<ParticleSystem>().Play(true);

        StartCoroutine(ResetCall());
	}
	
    // 발사 3초 후 리셋
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

    // 충돌 시 리셋
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject != Shooter && isFire)
		{
            isFire = false;
            lastPosition = transform.position;
            NetworkResetSynchronize(lastPosition);
        }
	}

    // 범위를 체크하여 범위 안에 있는 플레이어 데미지
    private void CheckPlayerInRange(Vector3 position)
    {
        // 범위안의 모든 플레이어 받아오기
        Collider[] colliders = Physics.OverlapSphere(position, 5f, 1 << LayerMask.NameToLayer("Player"));
        
        // 플레이어가 장애물 뒤에 있는지 체크 후 아니면 데미지 
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
            GameDirector.Instance.players[Shooter.name].GetComponent<PlayerAttack>().CmdRpgReset(Shooter.name, rpgCount, lastPos);
        }
        rgbd.velocity = Vector3.zero;
        transform.position = resetPos;
    }

    // 리셋, 리셋 할 때 플레이어가 데미지 범위 안에 있는지 체크
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

        if (isExplosion)
        {
            GameObject exp = Instantiate(ExplosionEffect, lastPos, Quaternion.identity);
            Destroy(exp, 3f);
        }

        CheckPlayerInRange(lastPos);                            // 범위내 플레이어 체크
        GetComponent<CapsuleCollider>().enabled = false;         // 리셋 위치에선 콜라이더 비활성화
        
        isExplosion = true;
    }
}

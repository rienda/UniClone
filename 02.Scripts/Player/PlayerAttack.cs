using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttack : NetworkBehaviour {
    public enum GunMode { AR, SG, SR, RPG, MISSILE }

    [SerializeField]
    private bool isShotgun;

    [SerializeField]
    private GameObject fireStart;

    [SerializeField]
    private GameObject cam;

    [SerializeField]
    private Camera fpsCam;

    [SerializeField]
    private Camera tpsCam;

    [SerializeField]
    private PlayerUi playerUi;

    [SerializeField]
    private GameObject fireEffect;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private float fireDelay = 0.08f;

    [SerializeField]
    private AttackManager attackManager;

    // 무기 정보
    [SerializeField]
    private PlayerWeaponStats playerWeaponStats;

    [SerializeField]
    private GameObject hitEffect;

    bool canRPG = true;
    bool canMissle = true;
    bool canSG = true;
    bool canSR = true;

    private bool isMouseLock = true;

    GunMode gunMode = GunMode.AR;

    public GunMode getGunMode { get; private set; }

    private const float DEFAULT_ZOOM = 90f;
    public float current_fps_zoom = 75f;
    public float current_tps_zoom = 45f;
    private const float MAX_FPS_ZOOM = 1f;
    private const float MAX_TPS_ZOOM = 60f;

    public bool IsZoom { get; private set; }

    Ray ray;
   
	Vector3 direction;

	GameObject Shooter;

    int arShotCount = 0;        // 반동 제어를 위한 연사 횟수
    float recoil = 0f;
        
    void Awake()
	{
		Shooter = this.gameObject;
        getGunMode = gunMode;
        IsZoom = false;
    }

	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
		Fire();
		Zoom();
		CheckMouseLook();
		ChangeGunMode();
        getGunMode = gunMode;
    }

	[Client]
	private void Fire()
	{
		if (playerStats.EnergyValue > 1f)
		{
			switch (gunMode)
			{
				case GunMode.AR:
					// 연사
					if (Input.GetMouseButtonDown(0))
					{
						InvokeRepeating("ShootAR", 0f, playerWeaponStats.WeaponStats[(int)GunMode.AR, (int)PlayerWeaponStats.WeaponStat.DELAY]);
					}
					break;

				case GunMode.SG:
					// 산탄
					if (Input.GetMouseButtonDown(0) && !playerStats.IsEnegyZero())
					{
                        StartCoroutine(ShootSG());
					}
					break;

                case GunMode.SR:
                    if (Input.GetMouseButtonDown(0) && !playerStats.IsEnegyZero())
                    {
                        StartCoroutine(ShootSR());
                    }
                    break;

                case GunMode.RPG:
					if (Input.GetMouseButtonDown(0))
					{
						StartCoroutine(ShootRPG());
					}
					break;

				case GunMode.MISSILE:
					if (Input.GetMouseButtonDown(0))
					{
						StartCoroutine(ShootMissile());
					}
					break;

				default:
					break;
			}

			if (Input.GetMouseButtonUp(0) || gunMode != GunMode.AR)
			{
				CancelInvoke("ShootAR");
                arShotCount = 0;
                recoil = 0f;
            }
		}
        else
        {
            CancelInvoke("ShootAR");
            arShotCount = 0;
            recoil = 0f;
        }        
	}

    // ==========================  AR / SG 서버 연결 ==============================

    [Command]
    void CmdOnhit(Vector3 pos, Vector3 normal)
    {
        RpcOnHitEffect(pos, normal);
    }

    [ClientRpc]
    void RpcOnHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject fire = Instantiate(fireEffect, pos, Quaternion.LookRotation(normal));

        Destroy(fire, 0.2f);
    }

    [Command]
    void CmdHitPlayer(Vector3 pos, Vector3 normal)
    {
        RpcHitPlayer(pos, normal);
    }

    [ClientRpc]
    void RpcHitPlayer(Vector3 pos, Vector3 normal)
    {
        GameObject hit = Instantiate(hitEffect, pos, Quaternion.LookRotation(normal));

        Destroy(hit, 1f);
    }

    // =============================================================================

    [Client]
	private void ShootAR()
	{
        if (playerStats.EnergyValue < 1f)
        {
            CancelInvoke("ShootAR");
            return;
        }

        CmdNetworkAnimation("AR");
        GetComponent<PlayerSound>().PlayOneShot(gameObject.name, (int)PlayerSound.AudioClipNum.AR, 1f);

        float damage = playerWeaponStats.WeaponStats[(int)GunMode.AR, (int)PlayerWeaponStats.WeaponStat.DAMAGE];
        
        arShotCount++;
        if (arShotCount <= 1)
        {
            ray = new Ray(cam.transform.position, cam.transform.forward);
        }
        else
        {
            recoil += playerWeaponStats.WeaponStats[(int)GunMode.AR, (int)PlayerWeaponStats.WeaponStat.SPREAD] * 0.2f;
            if (recoil > playerWeaponStats.WeaponStats[(int)GunMode.AR, (int)PlayerWeaponStats.WeaponStat.SPREAD])
            {
                recoil = playerWeaponStats.WeaponStats[(int)GunMode.AR, (int)PlayerWeaponStats.WeaponStat.SPREAD];
            }

            ray = new Ray(cam.transform.position, cam.transform.forward
                + new Vector3(Random.Range(-recoil, recoil), Random.Range(-recoil, recoil), Random.Range(-recoil, recoil)));
        }
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 1500f, 1 << LayerMask.NameToLayer("Player")
                | 1 << LayerMask.NameToLayer("Block") 
                | 1 << LayerMask.NameToLayer("HeadHitBox")
                | 1 << LayerMask.NameToLayer("Bullet")))
        {
			ray = new Ray(cam.transform.position,
						(hit.point - fireStart.transform.position));
            
            if (hit.transform.gameObject.tag == "Player")
            {
                CmdHitPlayer(hit.point, hit.normal);
            }
            else
            {
                CmdOnhit(hit.point, hit.normal);
            }


            if (hit.collider.gameObject.tag == "Player")
            {
                CmdTakeDamage(hit.collider.gameObject.name, damage);
                GetComponent<PlayerSound>().PlayOneShot(hit.collider.gameObject.name, (int)PlayerSound.AudioClipNum.DAMAGE, 1f);
            }
            else if (hit.collider.gameObject.tag == "HeadHitBox")
            {
                CmdTakeDamage(hit.collider.gameObject.name, damage * 2f);
                GetComponent<PlayerSound>().PlayOneShot(hit.collider.gameObject.name, (int)PlayerSound.AudioClipNum.DAMAGE, 1f);
            }
            else if (hit.collider.gameObject.tag == "Bullet")
            {
                BulletController bullet = hit.collider.gameObject.GetComponent<BulletController>();
                CmdRpgReset(bullet.getShooter().name, bullet.getRpgCount(), hit.point);
            }
            else if (hit.collider.gameObject.tag == "Missile")
            {
                MissileController missile = hit.collider.gameObject.GetComponent<MissileController>();
                CmdMissileReset(missile.getShooter().name, missile.getMissileCount(), hit.point);
            }
        }
        float energyDown = playerWeaponStats.WeaponStats[(int)GunMode.AR, (int)PlayerWeaponStats.WeaponStat.CONSUMPTION];
        playerStats.EnergyDown(energyDown);
    }


    [Command]
    public void CmdRpgReset(string name, int count, Vector3 lastPosition)
    {
        RpcRPGReset(name, count, lastPosition);
    }
    [ClientRpc]
    private void RpcRPGReset(string name, int count, Vector3 lastPosition)
    {
        BulletController bulletController = 
            GameDirector.Instance.players[name].GetComponent<AttackManager>().rpgs[count].GetComponent<BulletController>();

        if (bulletController != null)
        {
            bulletController.Reset(lastPosition);
        }
    }


    [Command]
    public void CmdMissileReset(string name, int count, Vector3 lastPosition)
    {
        RpcMissileReset(name, count, lastPosition);
    }
    [ClientRpc]
    private void RpcMissileReset(string name, int count, Vector3 lastPosition)
    {
        MissileController missileController =
            GameDirector.Instance.players[name].GetComponent<AttackManager>().missiles[count].GetComponent<MissileController>();

        if (missileController != null)
        {
            missileController.Reset(lastPosition);
        }
    }

    private IEnumerator ShootSG()
    {
        float damage = playerWeaponStats.WeaponStats[(int)GunMode.SG, (int)PlayerWeaponStats.WeaponStat.DAMAGE];
        float spread = playerWeaponStats.WeaponStats[(int)GunMode.SG, (int)PlayerWeaponStats.WeaponStat.SPREAD];
        float energyDown = playerWeaponStats.WeaponStats[(int)GunMode.SG, (int)PlayerWeaponStats.WeaponStat.CONSUMPTION];

        if (canSG && playerStats.EnergyValue >= energyDown * 9f)
        {
            canSG = false;
            CmdNetworkAnimation("SG");
            GetComponent<PlayerSound>().PlayOneShot(gameObject.name, (int)PlayerSound.AudioClipNum.SG, 1f);

            for (int i = 0; i < 9; i++)
            {
                RaycastHit hit;
                ray = new Ray(fireStart.transform.position, cam.transform.forward
                    + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)));

                if (Physics.Raycast(ray, out hit, 100f, 1 << LayerMask.NameToLayer("Player")
                        | 1 << LayerMask.NameToLayer("Block")
                        | 1 << LayerMask.NameToLayer("HeadHitBox")
                        | 1 << LayerMask.NameToLayer("Bullet")))
                {
                    ray = new Ray(fireStart.transform.position,
                                (hit.point - fireStart.transform.position));

                    if (hit.transform.gameObject.tag == "Player")
                    {
                        CmdHitPlayer(hit.point, hit.normal);
                    }
                    else
                    {
                        CmdOnhit(hit.point, hit.normal);
                    }

                    if (hit.collider.gameObject.tag == "Player")
                    {
                        CmdTakeDamage(hit.collider.gameObject.name, damage);
                        GetComponent<PlayerSound>().PlayOneShot(hit.collider.gameObject.name, (int)PlayerSound.AudioClipNum.DAMAGE, 1f);
                    }
                    else if (hit.collider.gameObject.tag == "HeadHitBox")
                    {
                        CmdTakeDamage(hit.collider.gameObject.name, damage * 2f);
                        GetComponent<PlayerSound>().PlayOneShot(hit.collider.gameObject.name, (int)PlayerSound.AudioClipNum.DAMAGE, 1f);
                    }
                    else if (hit.collider.gameObject.tag == "Bullet")
                    {
                        BulletController bullet = hit.collider.gameObject.GetComponent<BulletController>();
                        CmdRpgReset(bullet.getShooter().name, bullet.getRpgCount(), hit.point);
                    }
                    else if (hit.collider.gameObject.tag == "Missile")
                    {
                        MissileController missile = hit.collider.gameObject.GetComponent<MissileController>();
                        CmdMissileReset(missile.getShooter().name, missile.getMissileCount(), hit.point);
                    }
                }

                playerStats.EnergyDown(energyDown);
            }
            yield return new WaitForSeconds(playerWeaponStats.WeaponStats[(int)GunMode.SG, (int)PlayerWeaponStats.WeaponStat.DELAY]);
            canSG = true;
        }
    }

    private IEnumerator ShootSR()
    {
        float damage = playerWeaponStats.WeaponStats[(int)GunMode.SR, (int)PlayerWeaponStats.WeaponStat.DAMAGE];
        float energyDown = playerWeaponStats.WeaponStats[(int)GunMode.SR, (int)PlayerWeaponStats.WeaponStat.CONSUMPTION];

        if (canSR && playerStats.EnergyValue >= energyDown)
        {
            canSR = false;
            CmdNetworkAnimation("SR");
            GetComponent<PlayerSound>().PlayOneShot(gameObject.name, (int)PlayerSound.AudioClipNum.SR, 1f);

            ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3000f, 1 << LayerMask.NameToLayer("Player")
                    | 1 << LayerMask.NameToLayer("Block")
                    | 1 << LayerMask.NameToLayer("HeadHitBox")
                    | 1 << LayerMask.NameToLayer("Bullet")))
            {
                ray = new Ray(cam.transform.position,
                            (hit.point - fireStart.transform.position));
                
                if (hit.transform.gameObject.tag == "Player")
                {
                    CmdHitPlayer(hit.point, hit.normal);
                }
                else
                {
                    CmdOnhit(hit.point, hit.normal);
                }

                if (hit.collider.gameObject.tag == "Player")
                {
                    CmdTakeDamage(hit.collider.gameObject.name, damage);
                    GetComponent<PlayerSound>().PlayOneShot(hit.collider.gameObject.name, (int)PlayerSound.AudioClipNum.DAMAGE, 1f);
                }
                else if(hit.collider.gameObject.tag == "HeadHitBox")
                {
                    CmdTakeDamage(hit.collider.gameObject.name, damage * 2f);
                    GetComponent<PlayerSound>().PlayOneShot(hit.collider.gameObject.name, (int)PlayerSound.AudioClipNum.DAMAGE, 1f);
                }
                else if (hit.collider.gameObject.tag == "Bullet")
                {
                    BulletController bullet = hit.collider.gameObject.GetComponent<BulletController>();
                    CmdRpgReset(bullet.getShooter().name, bullet.getRpgCount(), hit.point);
                }
                else if (hit.collider.gameObject.tag == "Missile")
                {
                    MissileController missile = hit.collider.gameObject.GetComponent<MissileController>();
                    CmdMissileReset(missile.getShooter().name, missile.getMissileCount(), hit.point);
                }
            }

            playerStats.EnergyDown(energyDown);
            yield return new WaitForSeconds(playerWeaponStats.WeaponStats[(int)GunMode.SR, (int)PlayerWeaponStats.WeaponStat.DELAY]);
            canSR = true;
        }
    }

    // 데미지를 주는 경우
    [Command]
    public void CmdTakeDamage(string target, float damage)
    {
        GameDirector.Instance.players[target].GetComponent<PlayerStats>().RpcHealthDown(damage);
    }

	private IEnumerator ShootRPG()
	{
        float energyDown = playerWeaponStats.WeaponStats[(int)GunMode.RPG, (int)PlayerWeaponStats.WeaponStat.CONSUMPTION];

        if (canRPG && playerStats.EnergyValue >= energyDown)
        {
            canRPG = false;
            CmdNetworkAnimation("SR");

            ray = new Ray(cam.transform.position, cam.transform.forward);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 2000f))
			{
				ray = new Ray(fireStart.transform.position,
							(hit.point - fireStart.transform.position));				
			}
            
            direction = ray.direction;

            
            playerStats.EnergyDown(energyDown);

            CmdFireRpg(direction);
            yield return new WaitForSeconds(playerWeaponStats.WeaponStats[(int)GunMode.RPG, (int)PlayerWeaponStats.WeaponStat.DELAY]);
            canRPG = true;
		}
	}

    [Command]
    private void CmdFireRpg(Vector3 direction)
    {
        RpcFireRpg(direction, gameObject.name);
    }

    [ClientRpc]
    private void RpcFireRpg(Vector3 direction, string shooterName)
    {
        GameDirector.Instance.players[shooterName].GetComponent<AttackManager>().FireRpg(direction);
    }

    private IEnumerator ShootMissile()
	{
        float energyDown = playerWeaponStats.WeaponStats[(int)GunMode.MISSILE, (int)PlayerWeaponStats.WeaponStat.CONSUMPTION];

        if (canMissle && playerStats.EnergyValue >= energyDown)
        {
            canMissle = false;
            CmdNetworkAnimation("SR");

            ray = new Ray(cam.transform.position, cam.transform.forward);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 2000f))
			{
				ray = new Ray(fireStart.transform.position,
							(hit.point - fireStart.transform.position));
            }
            direction = ray.direction;

            
            playerStats.EnergyDown(energyDown);

            CmdFireMissile(direction);
            yield return new WaitForSeconds(playerWeaponStats.WeaponStats[(int)GunMode.MISSILE, (int)PlayerWeaponStats.WeaponStat.DELAY]);
            
            canMissle = true;
		}
	}

    [Command]
    private void CmdFireMissile(Vector3 direction)
    {
        RpcFireMissile(direction, gameObject.name);
    }

    [ClientRpc]
    private void RpcFireMissile(Vector3 direction, string shooterName)
    {
        GameDirector.Instance.players[shooterName].GetComponent<AttackManager>().FireMissile(direction);
    }

    // 줌
    private void Zoom()
	{
        // 1인칭과 3인칭에 따라 다르다 최대 량도.
        if (!playerController.isFPS)
        {
            current_tps_zoom -= Input.GetAxis("Mouse ScrollWheel") * 20f;
            if (current_tps_zoom >= DEFAULT_ZOOM)
            {
                current_tps_zoom = DEFAULT_ZOOM;
            }
            else if (current_tps_zoom <= MAX_TPS_ZOOM)
            {
                current_tps_zoom = MAX_TPS_ZOOM;
            }

            cam = tpsCam.gameObject;
        }
        else
        {
            current_fps_zoom -= Input.GetAxis("Mouse ScrollWheel") * 20f;
            if (current_fps_zoom >= DEFAULT_ZOOM)
            {
                current_fps_zoom = DEFAULT_ZOOM;
            }
            else if (current_fps_zoom <= MAX_FPS_ZOOM)
            {
                current_fps_zoom = MAX_FPS_ZOOM;
            }

            cam = fpsCam.gameObject;
        }

        if (Input.GetMouseButton(1))
		{
			if (!playerController.isFPS)
			{
                tpsCam.fieldOfView = current_tps_zoom;
            }
			else
			{
                fpsCam.fieldOfView = current_fps_zoom;
            }

            IsZoom = true;
        }

		if (Input.GetMouseButtonUp(1))
		{
            tpsCam.fieldOfView = DEFAULT_ZOOM;
            fpsCam.fieldOfView = DEFAULT_ZOOM;
            IsZoom = false;
        }        
	}

	void CheckMouseLook()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			isMouseLock = !isMouseLock;
		}

		// 마우스가 잠겨 있다면
		if (isMouseLock)
		{
			// 말 그대로 1
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;   // 커서를 화면 중앙에 고정
		}
		else
		{
			// 말 그대로 2
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;     // 고정 해제
		}
	}

    // 총 모드 변환
	private void ChangeGunMode()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			gunMode = GunMode.AR;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			gunMode = GunMode.SG;
		}
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gunMode = GunMode.SR;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			gunMode = GunMode.RPG;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			gunMode = GunMode.MISSILE;
		}
	}

    [Command]
    private void CmdNetworkAnimation(string stateName)
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
	[SerializeField]
	private	float energyValue = 100f;
	public float EnergyValue { get; private set; }

    [SerializeField]// [SyncVar]
    private float healthValue = 100f;

    public float HealthValue { get; private set; }

    [SerializeField]
	private PlayerController PlayerController;

	[SerializeField]
	private PlayerUi playerUi;
        
    private float enegyUse = 0f;

    [SerializeField]
    private float energyMax = 100f;
    public float EnergyMax { get; private set; }

    [SerializeField]
    private float healthMax = 100f;
    public float HealthMax { get; private set; }
    
    public bool isOutMagneticField = false;
    
    void Start()
    {
        EnergyValue = energyValue;
        HealthValue = healthValue;
        EnergyMax = energyMax;
        HealthMax = healthMax;
    }

    void Update()
	{
        if (!isLocalPlayer)
        {
            return;
        }

        if (EnergyValue <= 5f)
		{
			PlayerController.FlyExit();
		}

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			enegyUse = Time.deltaTime * 1.5f;
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			enegyUse = Time.deltaTime;
		}

        // 자기장에 따라 체력이 달라진다.
        if (isOutMagneticField)
        {
            HealthDown(Time.deltaTime * 2f);
        }
        else
        {
            if (HealthValue <= healthMax * (0.75))
            {
                HealthUp(Time.deltaTime);
            }
        }

        // 비행 중
        if (PlayerController.state == PlayerController.State.Flying)
        {
            EnergyDown(Time.deltaTime);

            if (PlayerController.Vertical != 0 || PlayerController.Horizontal != 0 
                || PlayerController.isUp || PlayerController.isDown)
            {
                EnergyDown(Time.deltaTime);
            }
        }
        else
        {
            EnergyUp(Time.deltaTime * 5f);
        }
    }

	public void EnergyUp(float e)
	{
        if (!isLocalPlayer)
        {
            return;
        }
        EnergyValue += e;
		if (EnergyValue > EnergyMax)
		{
			EnergyValue = EnergyMax;
		}
        if (playerUi.enabled)
        {
            playerUi.SetEnergy(EnergyValue);
        }
	}

	public void EnergyDown(float e)
	{
        if (!isLocalPlayer)
        {
            return;
        }
        EnergyValue -= e;
		if (EnergyValue < 0)
		{
			EnergyValue = 0f;
        }
        if (playerUi.enabled)
        {
            playerUi.SetEnergy(EnergyValue);
        }
    }

    public void EnergySet(float e)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (e < 0f && e > EnergyMax)
        {
            return;
        }
        EnergyValue = e;
        if (playerUi.enabled)
        {
            playerUi.SetEnergy(EnergyValue);
        }
    }

    public void HealthUp(float h)
    {
        HealthValue += h;
        if (HealthValue > HealthMax)
        {
            HealthValue = HealthMax;
        }
        if (playerUi.enabled)
        {
            playerUi.SetHealth(HealthValue);
        }
    }

    void HealthDown(float h)
    {
        HealthValue -= h;
        if (HealthValue < 0)
        {
            HealthValue = 0f;
            GameDirector.Instance.isPlayerDead = true;
            GameDirector.Instance.nonPlayerCamera.GetComponent<AudioListener>().enabled = true;
            GameDirector.Instance.nonPlayerCamera.transform.position = transform.position;
            GameDirector.Instance.nonPlayerCamera.transform.rotation = transform.rotation;
            CmdDead(gameObject.name);
        }
        if (playerUi.enabled)
        {
            playerUi.SetHealth(HealthValue);
        }
    }

    [Command]
    private void CmdDead(string playerName)
    {
        if (GameDirector.Instance.players.ContainsKey(playerName))
        {
            GameDirector.Instance.players[playerName].GetComponent<PlayerStats>().RpcDead(playerName);
        }
    }

    [ClientRpc]
    public void RpcDead(string playerName)
    {
        if (GameDirector.Instance.players.ContainsKey(playerName))
        {
            GameObject deadPlayer = GameDirector.Instance.players[playerName];
            GameDirector.Instance.players.Remove(playerName);
            Destroy(deadPlayer);
        }
    }

    [ClientRpc]
    public void RpcHealthDown(float h)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        HealthDown(h);
    }

    public void HealthSet(float h)
    {
        if (h < 0f && h > HealthMax)
        {
            return;
        }
        HealthValue = h;
        playerUi.SetHealth(HealthValue);
    }

    public bool IsEnegyZero()
	{
		return EnergyValue == 0f ? true : false;
	}

    // 20180511 추가
    public void InsideEmpZone()
    {
        if (EnergyValue > 20.0f)
        {
            EnergyDown(0.3f);
        }
    }

    // 20180511 추가
    public void TouchedEmpBomb()
    {
        EnergyDown(100f);
    }   
}

